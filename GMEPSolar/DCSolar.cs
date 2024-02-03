using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GMEPSolar
{
    public partial class DC_SOLAR_INPUT : Form
    {
        public DC_SOLAR_INPUT()
        {
            InitializeComponent();
            NUMBER_ALL_MODULES_TEXTBOX.KeyDown += NUMBER_ALL_MODULES_TEXTBOX_KeyDown;
        }

        public static void CreateDCSolarObject(
            Dictionary<string, Dictionary<string, object>> formData
        )
        {
            CreateDesktopJsonFile(formData, "DCSolarFormData.json");
            var numberOfMPPTs = 0;

            foreach (var mppt in formData)
            {
                var mpptData = mppt.Value as Dictionary<string, object>;
                if (Convert.ToBoolean(mpptData["Enabled"]))
                {
                    numberOfMPPTs++;
                }
            }

            var json = File.ReadAllText($"../../block data/DCSolar{numberOfMPPTs}.json");
            var MPPTData = JArray
                .Parse(json)
                .ToObject<List<Dictionary<string, Dictionary<string, object>>>>();

            Editor ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;
            PromptPointOptions pointOptions = new PromptPointOptions("Select a point: ");
            PromptPointResult pointResult = ed.GetPoint(pointOptions);

            if (pointResult.Status == PromptStatus.OK)
            {
                Point3d selectedPoint;
                List<List<Dictionary<string, double>>> mpptEndPoints;

                CreateLinesOffMPPTs(
                    formData,
                    numberOfMPPTs,
                    pointResult,
                    out selectedPoint,
                    out mpptEndPoints
                );

                CreateObjectGivenData(MPPTData, ed, selectedPoint);

                CreateDesktopJsonFile(mpptEndPoints, "DCSolarEndPoints.json");

                CreateStringsOffMPPTS(formData, numberOfMPPTs, selectedPoint, mpptEndPoints);
            }
        }

        private static void CreateStringsOffMPPTS(
            Dictionary<string, Dictionary<string, object>> formData,
            int numberOfMPPTs,
            Point3d selectedPoint,
            List<List<Dictionary<string, double>>> mpptEndPoints
        )
        {
            var ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;

            var lineStartPointX = selectedPoint.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1)));
            var lineStartPointY = selectedPoint.Y - 0.6098;
            var stringMidPointX = lineStartPointX - 3.3606;
            var stringMidPointY = lineStartPointY - 1.2706;

            var stringDataContainer = GetStringData(formData);

            if (areAllModulesEmpty(stringDataContainer))
            {
                return;
            }

            var skipIndices = new List<int>();

            skipIndices = GetIndicesToSkip(formData);

            CreateDesktopJsonFile(skipIndices, "SkipIndices.json");

            var stringTotalHeight = GetTotalHeight(stringDataContainer);

            var stringStartPoint = new Point3d(
                stringMidPointX,
                stringMidPointY + stringTotalHeight / 2,
                0
            );

            CreateDesktopJsonFile(stringDataContainer, "StringData.json");

            var connectionPoints = CreateStringsAndReturnConnectionPoints(
                stringDataContainer,
                stringStartPoint,
                ed
            );

            var mpptEndPointsFlattened = mpptEndPoints.SelectMany(x => x).ToList();

            var aboveAndBelow = GetNumberOfPointsAboveAndBelow(
                connectionPoints,
                mpptEndPointsFlattened
            );

            CreateConnectionLines(
                connectionPoints,
                mpptEndPointsFlattened,
                aboveAndBelow,
                stringMidPointX,
                skipIndices
            );
        }

        private static List<int> GetIndicesToSkip(
            Dictionary<string, Dictionary<string, object>> formData
        )
        {
            var indices = new List<int>();
            var index = 0;
            foreach (var mppt in formData)
            {
                var data = mppt.Value;
                if (
                    Convert.ToBoolean(data["Enabled"])
                    && (Convert.ToBoolean(data["Regular"]) || Convert.ToBoolean(data["Parallel"]))
                    && data["Input"].ToString() != ""
                    && data["Input"].ToString() != "0"
                )
                {
                    if (Convert.ToBoolean(data["Regular"]))
                    {
                        index += 2;
                    }
                    else
                    {
                        index += 1;
                        indices.Add(index);
                        index += 3;
                    }
                }
            }
            return indices;
        }

        private static bool areAllModulesEmpty(List<Dictionary<string, object>> stringDataContainer)
        {
            foreach (var stringData in stringDataContainer)
            {
                if (stringData["module"].ToString() != "0" && stringData["module"].ToString() != "")
                {
                    return false;
                }
            }
            return true;
        }

        private static void CreateConnectionLines(
            List<Dictionary<string, double>> connectionPoints,
            List<Dictionary<string, double>> mpptEndPointsFlattened,
            Dictionary<string, int> aboveAndBelow,
            double stringMidPointX,
            List<int> skipIndices
        )
        {
            var STRING_X = stringMidPointX;
            var LINE_SPACING = 0.1621 / 2;
            var CLOSEST_DISTANCE_FROM_STRING = 0.1621 / 2;
            var initialBelowValue = aboveAndBelow["below"];

            var ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;

            for (var i = 0; i < connectionPoints.Count; i++)
            {
                var mpptEndPoint = mpptEndPointsFlattened[i];
                var connectionPoint = connectionPoints[i];

                var xDistance = Math.Abs(mpptEndPoint["x"] - connectionPoint["x"]);
                var yDistance = Math.Abs(mpptEndPoint["y"] - connectionPoint["y"]);

                var isAbove = connectionPoint["y"] > mpptEndPoint["y"];

                var aboveOrBelowValue = isAbove
                    ? aboveAndBelow["above"] - 1
                    : initialBelowValue - aboveAndBelow["below"];

                var firstHorizontalLineStartPoint = new Dictionary<string, double>
                {
                    { "x", mpptEndPoint["x"] },
                    { "y", mpptEndPoint["y"] }
                };

                if (skipIndices.Contains(i))
                {
                    firstHorizontalLineStartPoint = CreateLineAndArcFromPoints(
                        firstHorizontalLineStartPoint
                    );
                }

                var firstHorizontalLineEndPoint = new Dictionary<string, double>
                {
                    {
                        "x",
                        mpptEndPoint["x"]
                            - (
                                xDistance
                                - CLOSEST_DISTANCE_FROM_STRING
                                - (aboveOrBelowValue * LINE_SPACING)
                            )
                    },
                    { "y", mpptEndPoint["y"] }
                };

                if (isAbove)
                {
                    aboveAndBelow["above"]--;
                }
                else
                {
                    aboveAndBelow["below"]--;
                }

                CreateLineFromPoints(firstHorizontalLineStartPoint, firstHorizontalLineEndPoint);

                var secondVerticalLineStartPoint = new Dictionary<string, double>
                {
                    { "x", firstHorizontalLineEndPoint["x"] },
                    { "y", firstHorizontalLineEndPoint["y"] }
                };

                var secondVerticalLineEndPoint = new Dictionary<string, double>
                {
                    { "x", firstHorizontalLineEndPoint["x"] },
                    { "y", connectionPoint["y"] }
                };

                CreateLineFromPoints(secondVerticalLineStartPoint, secondVerticalLineEndPoint);

                var thirdHorizontalLineStartPoint = new Dictionary<string, double>
                {
                    { "x", secondVerticalLineEndPoint["x"] },
                    { "y", secondVerticalLineEndPoint["y"] }
                };

                var thirdHorizontalLineEndPoint = new Dictionary<string, double>
                {
                    { "x", STRING_X },
                    { "y", secondVerticalLineEndPoint["y"] }
                };

                CreateLineFromPoints(thirdHorizontalLineStartPoint, thirdHorizontalLineEndPoint);
            }
        }

        private static Dictionary<string, double> CreateLineAndArcFromPoints(
            Dictionary<string, double> firstHorizontalLineStartPoint
        )
        {
            var CELL_SPACING = 0.1621;

            var firstHorizontalLineEndPoint = new Dictionary<string, double>
            {
                { "x", firstHorizontalLineStartPoint["x"] - CELL_SPACING / 2 },
                { "y", firstHorizontalLineStartPoint["y"] }
            };

            CreateLineFromPoints(firstHorizontalLineStartPoint, firstHorizontalLineEndPoint);

            var arcStartPoint = new Dictionary<string, double>
            {
                { "x", firstHorizontalLineEndPoint["x"] },
                { "y", firstHorizontalLineEndPoint["y"] }
            };

            var arcEndPoint = new Dictionary<string, double>
            {
                { "x", firstHorizontalLineEndPoint["x"] - CELL_SPACING },
                { "y", firstHorizontalLineEndPoint["y"] }
            };

            CreateArc(arcStartPoint, arcEndPoint, 0, 180);

            return new Dictionary<string, double>
            {
                { "x", arcEndPoint["x"] },
                { "y", arcEndPoint["y"] }
            };
        }

        private static void CreateLineFromPoints(
            Dictionary<string, double> firstHorizontalLineStartPoint,
            Dictionary<string, double> firstHorizontalLineEndPoint
        )
        {
            var ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;

            var line = new Line(
                new Point3d(
                    firstHorizontalLineStartPoint["x"],
                    firstHorizontalLineStartPoint["y"],
                    0
                ),
                new Point3d(firstHorizontalLineEndPoint["x"], firstHorizontalLineEndPoint["y"], 0)
            );

            AddLineToPaperSpace(line);
        }

        private static Dictionary<string, int> GetNumberOfPointsAboveAndBelow(
            List<Dictionary<string, double>> connectionPoints,
            List<Dictionary<string, double>> mpptEndPointsFlattened
        )
        {
            var above = 0;
            var below = 0;

            for (var i = 0; i < connectionPoints.Count; i++)
            {
                var connectionPoint = connectionPoints[i];
                var mpptEndPoint = mpptEndPointsFlattened[i];

                if (connectionPoint["y"] > mpptEndPoint["y"])
                {
                    above++;
                }
                else
                {
                    below++;
                }
            }

            return new Dictionary<string, int>() { { "above", above }, { "below", below } };
        }

        private static List<Dictionary<string, double>> CreateStringsAndReturnConnectionPoints(
            List<Dictionary<string, object>> stringDataContainer,
            Point3d stringStartPoint,
            Editor ed
        )
        {
            var TEXT_HEIGHT = 0.8334;
            var STRING_HEIGHT = 1.0695;
            var CELL_SPACING = 0.1621;
            var connectionPoints = new List<Dictionary<string, double>>();
            var startPoint = new Dictionary<string, double>
            {
                { "x", stringStartPoint.X },
                { "y", stringStartPoint.Y }
            };

            foreach (var stringData in stringDataContainer)
            {
                var items = stringData["items"] as List<string>;
                if (items.Count == 0)
                {
                    continue;
                }
                var module = 0;
                int.TryParse(stringData["module"].ToString(), out module);
                foreach (var item in items)
                {
                    if (item == "text")
                    {
                        CreateText(ed, new Point3d(startPoint["x"], startPoint["y"], 0), module);
                        startPoint["y"] -= TEXT_HEIGHT;
                    }
                    else if (item == "string")
                    {
                        CreateString(ed, new Point3d(startPoint["x"], startPoint["y"], 0));
                        connectionPoints.Add(
                            new Dictionary<string, double>
                            {
                                { "x", startPoint["x"] },
                                { "y", startPoint["y"] - (STRING_HEIGHT / 2) + (CELL_SPACING / 2) }
                            }
                        );
                        connectionPoints.Add(
                            new Dictionary<string, double>
                            {
                                { "x", startPoint["x"] },
                                { "y", startPoint["y"] - (STRING_HEIGHT / 2) - (CELL_SPACING / 2) }
                            }
                        );
                        startPoint["y"] -= STRING_HEIGHT;
                    }
                }
            }
            return connectionPoints;
        }

        private static void CreateText(Editor ed, Point3d startPoint, int module)
        {
            var json = File.ReadAllText($"../../block data/StringText.json");
            var stringTextData = JArray
                .Parse(json)
                .ToObject<List<Dictionary<string, Dictionary<string, object>>>>();

            stringTextData[0]["mtext"]["text"] = stringTextData[0]
                ["mtext"]["text"]
                .ToString()
                .Replace("*", module.ToString().PadLeft(2, '0'));

            CreateObjectGivenData(stringTextData, ed, startPoint);
        }

        private static void CreateString(Editor ed, Point3d stringStartPoint)
        {
            var json = File.ReadAllText($"../../block data/String.json");
            var stringData = JArray
                .Parse(json)
                .ToObject<List<Dictionary<string, Dictionary<string, object>>>>();
            CreateObjectGivenData(stringData, ed, stringStartPoint);
        }

        private static double GetTotalHeight(List<Dictionary<string, object>> stringDataContainer)
        {
            double totalHeight = 0;
            foreach (var stringData in stringDataContainer)
            {
                totalHeight += Convert.ToDouble(stringData["height"]);
            }
            return totalHeight;
        }

        private static List<Dictionary<string, object>> GetStringData(
            Dictionary<string, Dictionary<string, object>> formData
        )
        {
            var stringDataContainer = new List<Dictionary<string, object>>();
            var firstFlag = true;
            var previousModulesCount = 0;

            foreach (var mppt in formData)
            {
                var stringData = new Dictionary<string, object>();
                stringData["height"] = 0.0;
                stringData["items"] = new List<string>();

                var mpptData = mppt.Value;
                stringData["module"] = mpptData["Input"];

                if (Convert.ToBoolean(mpptData["Enabled"]))
                {
                    var isRegular = Convert.ToBoolean(mpptData["Regular"]);
                    var isParallel = Convert.ToBoolean(mpptData["Parallel"]);
                    var modulesCount = 0;
                    if (isRegular || isParallel)
                    {
                        int.TryParse((string)mpptData["Input"], out modulesCount);
                    }

                    if (firstFlag && (isRegular || isParallel))
                    {
                        firstFlag = false;
                        stringData["height"] = (double)stringData["height"] + 0.8334;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }
                    else if ((isRegular || isParallel) && modulesCount != previousModulesCount)
                    {
                        stringData["height"] = (double)stringData["height"] + 0.8334;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }

                    if (isRegular)
                    {
                        stringData["height"] = (double)stringData["height"] + 1.0695;
                        ((List<string>)stringData["items"]).Add("string");
                    }
                    else if (isParallel)
                    {
                        stringData["height"] = (double)stringData["height"] + (1.0695 * 2);
                        ((List<string>)stringData["items"]).Add("string");
                        ((List<string>)stringData["items"]).Add("string");
                    }
                }
                stringDataContainer.Add(stringData);
            }
            return stringDataContainer;
        }

        private static void CreateLinesOffMPPTs(
            Dictionary<string, Dictionary<string, object>> formData,
            int numberOfMPPTs,
            PromptPointResult pointResult,
            out Point3d selectedPoint,
            out List<List<Dictionary<string, double>>> mpptEndPoints
        )
        {
            Double STARTLINE_LENGTH = 1.25;
            Double CELL_SPACING = 0.1621;
            selectedPoint = pointResult.Value;
            mpptEndPoints = new List<List<Dictionary<string, double>>>();
            var startPoint = new Dictionary<string, double>
            {
                { "x", selectedPoint.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1))) },
                { "y", selectedPoint.Y - 0.6098 }
            };

            var endPoint = new Dictionary<string, double>
            {
                { "x", selectedPoint.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1))) },
                { "y", selectedPoint.Y - 0.6098 - STARTLINE_LENGTH }
            };

            if (numberOfMPPTs == 1)
            {
                startPoint["x"] = selectedPoint.X - 0.7944;
                endPoint["x"] = selectedPoint.X - 0.7944;
            }

            foreach (var mpptData in formData)
            {
                var enabled = Convert.ToBoolean(mpptData.Value["Enabled"]);
                var endPoints = new List<Dictionary<string, double>>();

                if (!enabled)
                {
                    continue;
                }

                var isRegular = Convert.ToBoolean(mpptData.Value["Regular"]);
                var isParallel = Convert.ToBoolean(mpptData.Value["Parallel"]);

                var moduleCount = 0;
                if (
                    mpptData.Value["Input"] != null
                    && int.TryParse(mpptData.Value["Input"].ToString(), out int count)
                )
                {
                    moduleCount = count;
                }

                if (enabled && moduleCount > 0)
                {
                    if (isRegular)
                    {
                        endPoints = CreateRegularFirstLines(startPoint, endPoint, CELL_SPACING);
                    }
                    else
                    {
                        endPoints = CreateParallelFirstLines(startPoint, endPoint, CELL_SPACING);
                    }
                }

                mpptEndPoints.Add(endPoints);

                startPoint["x"] += 0.6484;
                endPoint["x"] += 0.6484;
                if (isRegular && enabled)
                {
                    endPoint["y"] -= 0.1621;
                }
                else if (isParallel && enabled)
                {
                    endPoint["y"] -= 0.3242;
                }
            }
        }

        private static List<Dictionary<string, double>> CreateParallelFirstLines(
            Dictionary<string, double> startPoint,
            Dictionary<string, double> endPoint,
            double CELL_SPACING
        )
        {
            var endPoints = new List<Dictionary<string, double>>();
            for (var i = 0; i < 4; i++)
            {
                var startPointUpdated = new Dictionary<string, double>
                {
                    { "x", startPoint["x"] + (CELL_SPACING * i) },
                    { "y", startPoint["y"] }
                };
                var endPointUpdated = new Dictionary<string, double>
                {
                    { "x", endPoint["x"] + (CELL_SPACING * i) },
                    { "y", endPoint["y"] }
                };

                if (i == 1)
                {
                    var arcEndPoint = new Dictionary<string, double>
                    {
                        { "x", endPointUpdated["x"] },
                        { "y", endPointUpdated["y"] - CELL_SPACING }
                    };

                    CreateLineFromPoints(endPointUpdated, arcEndPoint);
                }
                else if (i == 2)
                {
                    endPointUpdated["y"] -= CELL_SPACING / 2;
                }
                else if (i == 3)
                {
                    endPointUpdated["y"] -= CELL_SPACING + CELL_SPACING / 2;
                }

                var line = new Line(
                    new Point3d(startPointUpdated["x"], startPointUpdated["y"], 0),
                    new Point3d(endPointUpdated["x"], endPointUpdated["y"], 0)
                );

                AddLineToPaperSpace(line);

                if (i != 1)
                {
                    endPoints.Add(endPointUpdated);
                }
                else
                {
                    endPoints.Add(
                        new Dictionary<string, double>
                        {
                            { "x", endPointUpdated["x"] },
                            { "y", endPointUpdated["y"] - CELL_SPACING }
                        }
                    );
                }
            }

            endPoints = SwapParallelEndPoints(endPoints);

            return endPoints;
        }

        private static List<Dictionary<string, double>> SwapParallelEndPoints(
            List<Dictionary<string, double>> endPoints
        )
        {
            var temp = endPoints[1];
            endPoints[1] = endPoints[2];
            endPoints[2] = temp;
            return endPoints;
        }

        private static void CreateArc(
            Dictionary<string, double> startPoint,
            Dictionary<string, double> endPoint,
            int startDegrees,
            int endDegrees
        )
        {
            Editor ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;

            Point3d startPt = new Point3d(startPoint["x"], startPoint["y"], 0);
            Point3d endPt = new Point3d(endPoint["x"], endPoint["y"], 0);

            double radius = Math.Abs(endPt.Y - startPt.Y) / 2;
            if (radius == 0)
            {
                radius = Math.Abs(endPt.X - startPt.X) / 2;
            }

            Point3d centerPt = new Point3d((startPt.X + endPt.X) / 2, (startPt.Y + endPt.Y) / 2, 0);

            using (Transaction tr = ed.Document.Database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable =
                    tr.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord =
                    tr.GetObject(blockTable[BlockTableRecord.PaperSpace], OpenMode.ForWrite)
                    as BlockTableRecord;

                using (Arc arc = new Arc())
                {
                    arc.Normal = Vector3d.ZAxis;
                    arc.Center = centerPt;
                    arc.Radius = radius;
                    arc.StartAngle = startDegrees * (Math.PI / 180);
                    arc.EndAngle = endDegrees * (Math.PI / 180);
                    blockTableRecord.AppendEntity(arc);
                    tr.AddNewlyCreatedDBObject(arc, true);
                }

                tr.Commit();
            }
        }

        private static void CreateFilledCircleInPaperSpace(Point3d center, double radius)
        {
            Document acDoc = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                BlockTableRecord acBlkTblRec;

                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite)
                    as BlockTableRecord;

                // Create a circle
                using (Circle acCircle = new Circle())
                {
                    acCircle.Center = center;
                    acCircle.Radius = radius;
                    acCircle.SetDatabaseDefaults();
                    acBlkTblRec.AppendEntity(acCircle);
                    acTrans.AddNewlyCreatedDBObject(acCircle, true);

                    // Create a solid hatch
                    using (Hatch acHatch = new Hatch())
                    {
                        acBlkTblRec.AppendEntity(acHatch);
                        acTrans.AddNewlyCreatedDBObject(acHatch, true);
                        acHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                        acHatch.Associative = true;
                        acHatch.AppendLoop(
                            HatchLoopTypes.Outermost,
                            new ObjectIdCollection() { acCircle.ObjectId }
                        );
                        acHatch.EvaluateHatch(true);
                    }
                }

                acTrans.Commit();
            }
        }

        private static List<Dictionary<string, double>> CreateRegularFirstLines(
            Dictionary<string, double> startPoint,
            Dictionary<string, double> endPoint,
            double CELL_SPACING
        )
        {
            var endPoints = new List<Dictionary<string, double>>();
            Double ENDPOINT_INCREASE = -CELL_SPACING / 2;

            for (var i = 0; i < 4; i++)
            {
                var endPointUpdated = new Dictionary<string, double>
                {
                    { "x", endPoint["x"] + (CELL_SPACING * i) },
                    { "y", endPoint["y"] + (ENDPOINT_INCREASE * Math.Floor((double)i / 2)) }
                };

                var line = new Line(
                    new Point3d(startPoint["x"] + (CELL_SPACING * i), startPoint["y"], 0),
                    new Point3d(endPointUpdated["x"], endPointUpdated["y"], 0)
                );

                if (i == 0 || i == 2)
                {
                    Editor ed = Autodesk
                        .AutoCAD
                        .ApplicationServices
                        .Application
                        .DocumentManager
                        .MdiActiveDocument
                        .Editor;
                    ed.WriteMessage("Creating hatched circle\n");
                    Point3d endPointUpdatedPoint = new Point3d(
                        endPointUpdated["x"],
                        endPointUpdated["y"],
                        0
                    );
                    CreateFilledCircleInPaperSpace(endPointUpdatedPoint, 0.025);
                    endPoints.Add(endPointUpdated);

                    CreateHorizontalLine(endPointUpdated, CELL_SPACING);
                }

                AddLineToPaperSpace(line);
            }

            return endPoints;
        }

        private static void CreateHorizontalLine(
            Dictionary<string, double> endPointUpdated,
            double CELL_SPACING
        )
        {
            var line = new Line(
                new Point3d(endPointUpdated["x"], endPointUpdated["y"], 0),
                new Point3d(endPointUpdated["x"] + CELL_SPACING, endPointUpdated["y"], 0)
            );

            AddLineToPaperSpace(line);
        }

        private static void AddLineToPaperSpace(Line line)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable =
                    tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord =
                    tr.GetObject(blockTable[BlockTableRecord.PaperSpace], OpenMode.ForWrite)
                    as BlockTableRecord;
                blockTableRecord.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                tr.Commit();
            }
        }

        private static void CreateObjectGivenData(
            List<Dictionary<string, Dictionary<string, object>>> data,
            Editor ed,
            Point3d selectedPoint
        )
        {
            foreach (var objData in data)
            {
                var objectType = objData.Keys.First();

                switch (objectType)
                {
                    case "polyline":
                        selectedPoint = CreatePolyline(ed, selectedPoint, objData);
                        break;

                    case "line":
                        selectedPoint = CreateLine(ed, selectedPoint, objData);
                        break;

                    case "mtext":
                        selectedPoint = CreateMText(ed, selectedPoint, objData);
                        break;

                    case "circle":
                        selectedPoint = CreateCircle(ed, selectedPoint, objData);
                        break;

                    case "solid":
                        selectedPoint = CreateSolid(ed, selectedPoint, objData);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void SetMTextStyleByName(MText mtext, string styleName)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable textStyleTable =
                    tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (textStyleTable.Has(styleName))
                {
                    TextStyleTableRecord textStyle =
                        tr.GetObject(textStyleTable[styleName], OpenMode.ForRead)
                        as TextStyleTableRecord;
                    mtext.TextStyleId = textStyle.ObjectId;
                }
                tr.Commit();
            }
        }

        private static Point3d CreateCircle(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, Dictionary<string, object>> objData
        )
        {
            var circleData = objData["circle"] as Dictionary<string, object>;
            var circle = new Circle();

            // Set circle properties
            circle.Layer = circleData["layer"].ToString();

            var centerData = JsonConvert.DeserializeObject<Dictionary<string, double>>(
                circleData["center"].ToString()
            );

            var centerX = Convert.ToDouble(centerData["x"]) + selectedPoint.X;
            var centerY = Convert.ToDouble(centerData["y"]) + selectedPoint.Y;
            var centerZ = Convert.ToDouble(centerData["z"]) + selectedPoint.Z;
            circle.Center = new Point3d(centerX, centerY, centerZ);

            circle.Radius = Convert.ToDouble(circleData["radius"]);

            // Add circle to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(circle);
                transaction.AddNewlyCreatedDBObject(circle, true);
                transaction.Commit();
            }

            return selectedPoint;
        }

        private static Point3d CreateMText(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, Dictionary<string, object>> objData
        )
        {
            var mtextData = objData["mtext"] as Dictionary<string, object>;
            var mtext = new MText();

            // Set mtext properties
            mtext.Layer = mtextData["layer"].ToString();
            SetMTextStyleByName(mtext, mtextData["style"].ToString());
            mtext.Attachment = (AttachmentPoint)
                Enum.Parse(typeof(AttachmentPoint), mtextData["justification"].ToString());
            mtext.Contents = mtextData["text"].ToString();
            mtext.TextHeight = Convert.ToDouble(mtextData["height"]);
            mtext.LineSpaceDistance = Convert.ToDouble(mtextData["lineSpaceDistance"]);

            var locationData = JsonConvert.DeserializeObject<Dictionary<string, double>>(
                mtextData["location"].ToString()
            );

            var locX = Convert.ToDouble(locationData["x"]) + selectedPoint.X;
            var locY = Convert.ToDouble(locationData["y"]) + selectedPoint.Y;
            var locZ = Convert.ToDouble(locationData["z"]) + selectedPoint.Z;
            mtext.Location = new Point3d(locX, locY, locZ);

            // Add mtext to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(mtext);
                transaction.AddNewlyCreatedDBObject(mtext, true);
                transaction.Commit();
            }

            return selectedPoint;
        }

        private static Point3d CreateLine(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, Dictionary<string, object>> objData
        )
        {
            var lineData = objData["line"] as Dictionary<string, object>;
            var line = new Line();

            // Set line properties
            line.Layer = lineData["layer"].ToString();

            if (lineData.ContainsKey("linetype"))
            {
                line.Linetype = lineData["linetype"].ToString();
            }

            var startPointData = JsonConvert.DeserializeObject<Dictionary<string, double>>(
                lineData["startPoint"].ToString()
            );

            var startPtX = Convert.ToDouble(startPointData["x"]) + selectedPoint.X;
            var startPtY = Convert.ToDouble(startPointData["y"]) + selectedPoint.Y;
            var startPtZ = Convert.ToDouble(startPointData["z"]) + selectedPoint.Z;
            line.StartPoint = new Point3d(startPtX, startPtY, startPtZ);

            var endPointData = JsonConvert.DeserializeObject<Dictionary<string, double>>(
                lineData["endPoint"].ToString()
            );

            var endPtX = Convert.ToDouble(endPointData["x"]) + selectedPoint.X;
            var endPtY = Convert.ToDouble(endPointData["y"]) + selectedPoint.Y;
            var endPtZ = Convert.ToDouble(endPointData["z"]) + selectedPoint.Z;
            line.EndPoint = new Point3d(endPtX, endPtY, endPtZ);

            // Add line to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(line);
                transaction.AddNewlyCreatedDBObject(line, true);
                transaction.Commit();
            }

            return selectedPoint;
        }

        private static Point3d CreatePolyline(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, Dictionary<string, object>> objData
        )
        {
            var polylineData = objData["polyline"] as Dictionary<string, object>;
            var polyline = new Polyline();

            polyline.Layer = polylineData["layer"].ToString();

            if (polylineData.ContainsKey("linetype"))
            {
                polyline.Linetype = polylineData["linetype"].ToString();
            }

            var vertices = JArray
                .Parse(polylineData["vertices"].ToString())
                .ToObject<List<Dictionary<string, double>>>();

            foreach (var vertex in vertices)
            {
                var x = vertex["x"] + selectedPoint.X;
                var y = vertex["y"] + selectedPoint.Y;
                var z = vertex["z"] + selectedPoint.Z;
                polyline.AddVertexAt(polyline.NumberOfVertices, new Point2d(x, y), z, 0, 0);
            }

            var closed = Convert.ToBoolean(polylineData["isClosed"]);

            polyline.Closed = closed;

            // Add polyline to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(polyline);
                transaction.AddNewlyCreatedDBObject(polyline, true);
                transaction.Commit();
            }

            return selectedPoint;
        }

        private static Point3d CreateSolid(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, Dictionary<string, object>> objData
        )
        {
            var solidData = objData["solid"];
            var solid = new Solid();
            short i = 0;

            solid.Layer = solidData["layer"].ToString();

            var points = JArray
                .Parse(solidData["vertices"].ToString())
                .ToObject<List<Dictionary<string, double>>>();

            foreach (var point in points)
            {
                var x = point["x"] + selectedPoint.X;
                var y = point["y"] + selectedPoint.Y;
                var z = point["z"] + selectedPoint.Z;
                var point3d = new Point3d(x, y, z);
                solid.SetPointAt(i, point3d);
                i++;
            }

            // Add solid to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(solid);
                transaction.AddNewlyCreatedDBObject(solid, true);
                transaction.Commit();
            }

            return selectedPoint;
        }

        private static Dictionary<string, Dictionary<string, object>> GetFormData(
            DC_SOLAR_INPUT form
        )
        {
            var data = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "MPPT1",
                    new Dictionary<string, object>
                    {
                        { "Enabled", form.MPPT1_CHECKBOX.Checked },
                        { "Regular", form.MPPT1_RADIO_REGULAR.Checked },
                        { "Parallel", form.MPPT1_RADIO_PARALLEL.Checked },
                        { "Input", form.MPPT1_INPUT.Text }
                    }
                },
                {
                    "MPPT2",
                    new Dictionary<string, object>
                    {
                        { "Enabled", form.MPPT2_CHECKBOX.Checked },
                        { "Regular", form.MPPT2_RADIO_REGULAR.Checked },
                        { "Parallel", form.MPPT2_RADIO_PARALLEL.Checked },
                        { "Input", form.MPPT2_INPUT.Text }
                    }
                },
                {
                    "MPPT3",
                    new Dictionary<string, object>
                    {
                        { "Enabled", form.MPPT3_CHECKBOX.Checked },
                        { "Regular", form.MPPT3_RADIO_REGULAR.Checked },
                        { "Parallel", form.MPPT3_RADIO_PARALLEL.Checked },
                        { "Input", form.MPPT3_INPUT.Text }
                    }
                },
                {
                    "MPPT4",
                    new Dictionary<string, object>
                    {
                        { "Enabled", form.MPPT4_CHECKBOX.Checked },
                        { "Regular", form.MPPT4_RADIO_REGULAR.Checked },
                        { "Parallel", form.MPPT4_RADIO_PARALLEL.Checked },
                        { "Input", form.MPPT4_INPUT.Text }
                    }
                }
            };
            return data;
        }

        private static void CreateDesktopJsonFile(object data, string v)
        {
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                v
            );
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                data,
                Newtonsoft.Json.Formatting.Indented
            );
            File.WriteAllText(filePath, json);
        }

        private void ENABLE_ALL_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_CHECKBOX.Checked = true;
            MPPT2_CHECKBOX.Checked = true;
            MPPT3_CHECKBOX.Checked = true;
            MPPT4_CHECKBOX.Checked = true;
        }

        private void ALL_REGULAR_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_RADIO_REGULAR.Checked = true;
            MPPT2_RADIO_REGULAR.Checked = true;
            MPPT3_RADIO_REGULAR.Checked = true;
            MPPT4_RADIO_REGULAR.Checked = true;
        }

        private void ALL_PARALLEL_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_RADIO_PARALLEL.Checked = true;
            MPPT2_RADIO_PARALLEL.Checked = true;
            MPPT3_RADIO_PARALLEL.Checked = true;
            MPPT4_RADIO_PARALLEL.Checked = true;
        }

        private void SET_ALL_MODULES_BUTTON_Click(object sender, EventArgs e)
        {
            if (MPPT1_CHECKBOX.Checked && !MPPT1_RADIO_EMPTY.Checked)
            {
                MPPT1_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            }
            if (MPPT2_CHECKBOX.Checked && !MPPT2_RADIO_EMPTY.Checked)
            {
                MPPT2_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            }
            if (MPPT3_CHECKBOX.Checked && !MPPT3_RADIO_EMPTY.Checked)
            {
                MPPT3_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            }
            if (MPPT4_CHECKBOX.Checked && !MPPT4_RADIO_EMPTY.Checked)
            {
                MPPT4_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            }
        }

        private void NUMBER_ALL_MODULES_TEXTBOX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SET_ALL_MODULES_BUTTON_Click(sender, e);
            }
        }

        private void CREATE_BUTTON_Click(object sender, EventArgs e)
        {
            Close();
            var data = GetFormData(this);
            CreateDCSolarObject(data);
        }

        private void MPPT1_RADIO_EMPTY_CheckedChanged(object sender, EventArgs e)
        {
            MPPT1_INPUT.Enabled = !MPPT1_RADIO_EMPTY.Checked && MPPT1_CHECKBOX.Checked;
            MPPT1_INPUT.Text = "";
        }

        private void MPPT2_RADIO_EMPTY_CheckedChanged(object sender, EventArgs e)
        {
            MPPT2_INPUT.Enabled = !MPPT2_RADIO_EMPTY.Checked && MPPT2_CHECKBOX.Checked;
            MPPT2_INPUT.Text = "";
        }

        private void MPPT3_RADIO_EMPTY_CheckedChanged(object sender, EventArgs e)
        {
            MPPT3_INPUT.Enabled = !MPPT3_RADIO_EMPTY.Checked && MPPT3_CHECKBOX.Checked;
            MPPT3_INPUT.Text = "";
        }

        private void MPPT4_RADIO_EMPTY_CheckedChanged(object sender, EventArgs e)
        {
            MPPT4_INPUT.Enabled = !MPPT4_RADIO_EMPTY.Checked && MPPT4_CHECKBOX.Checked;
            MPPT4_INPUT.Text = "";
        }

        private void MPPT1_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (!MPPT1_CHECKBOX.Checked)
            {
                MPPT1_RADIO_EMPTY.Checked = true;
                MPPT1_INPUT.Enabled = false;
                MPPT1_INPUT.Text = "";
                MPPT1_RADIO_REGULAR.Enabled = false;
                MPPT1_RADIO_PARALLEL.Enabled = false;
            }
            else if (MPPT1_CHECKBOX.Checked && !MPPT1_RADIO_EMPTY.Checked)
            {
                MPPT1_INPUT.Enabled = true;
            }
            else if (MPPT1_CHECKBOX.Checked && MPPT1_RADIO_EMPTY.Checked)
            {
                MPPT1_RADIO_REGULAR.Enabled = true;
                MPPT1_RADIO_PARALLEL.Enabled = true;
            }
        }

        private void MPPT2_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (!MPPT2_CHECKBOX.Checked)
            {
                MPPT2_RADIO_EMPTY.Checked = true;
                MPPT2_INPUT.Enabled = false;
                MPPT2_INPUT.Text = "";
                MPPT2_RADIO_REGULAR.Enabled = false;
                MPPT2_RADIO_PARALLEL.Enabled = false;
            }
            else if (MPPT2_CHECKBOX.Checked && !MPPT2_RADIO_EMPTY.Checked)
            {
                MPPT2_INPUT.Enabled = true;
            }
            else if (MPPT2_CHECKBOX.Checked && MPPT2_RADIO_EMPTY.Checked)
            {
                MPPT2_RADIO_REGULAR.Enabled = true;
                MPPT2_RADIO_PARALLEL.Enabled = true;
            }
        }

        private void MPPT3_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (!MPPT3_CHECKBOX.Checked)
            {
                MPPT3_RADIO_EMPTY.Checked = true;
                MPPT3_INPUT.Enabled = false;
                MPPT3_INPUT.Text = "";
                MPPT3_RADIO_REGULAR.Enabled = false;
                MPPT3_RADIO_PARALLEL.Enabled = false;
            }
            else if (MPPT3_CHECKBOX.Checked && !MPPT3_RADIO_EMPTY.Checked)
            {
                MPPT3_INPUT.Enabled = true;
            }
            else if (MPPT3_CHECKBOX.Checked && MPPT3_RADIO_EMPTY.Checked)
            {
                MPPT3_RADIO_REGULAR.Enabled = true;
                MPPT3_RADIO_PARALLEL.Enabled = true;
            }
        }

        private void MPPT4_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (!MPPT4_CHECKBOX.Checked)
            {
                MPPT4_RADIO_EMPTY.Checked = true;
                MPPT4_INPUT.Enabled = false;
                MPPT4_INPUT.Text = "";
                MPPT4_RADIO_REGULAR.Enabled = false;
                MPPT4_RADIO_PARALLEL.Enabled = false;
            }
            else if (MPPT4_CHECKBOX.Checked && !MPPT4_RADIO_EMPTY.Checked)
            {
                MPPT4_INPUT.Enabled = true;
            }
            else if (MPPT4_CHECKBOX.Checked && MPPT4_RADIO_EMPTY.Checked)
            {
                MPPT4_RADIO_REGULAR.Enabled = true;
                MPPT4_RADIO_PARALLEL.Enabled = true;
            }
        }
    }
}
