using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GMEPUtilities;
using Newtonsoft.Json.Linq;

namespace GMEPSolar
{
    public partial class DC_SOLAR_INPUT : Form
    {
        readonly InverterForm inverterForm;
        readonly int id;

        public DC_SOLAR_INPUT(InverterForm inverterForm = null, int id = 0)
        {
            this.inverterForm = inverterForm;
            this.id = id;
            InitializeComponent();
            NUMBER_ALL_MODULES_TEXTBOX.KeyDown += NUMBER_ALL_MODULES_TEXTBOX_KeyDown;
        }

        public static void CreateDCSolarObject(
            Dictionary<string, Dictionary<string, object>> formData,
            string INCREASE_Y_TEXTBOX,
            string INCREASE_X_TEXTBOX,
            Point3d selectedPoint,
            bool smallStrings = false
        )
        {
            Editor ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;

            if (selectedPoint == new Point3d(0, 0, 0))
            {
                HelperMethods.GetUserToClick(out ed, out PromptPointResult pointResult);
                selectedPoint = pointResult.Value;
            }

            var numberOfMPPTs = GetNumberOfMPPTs(formData);

            if (numberOfMPPTs == 0)
            {
                MessageBox.Show("Please enable at least one MPPT");
                return;
            }

            string path = $"block data/DCSolar{numberOfMPPTs}.json";
            var MPPTData = BlockDataMethods.GetData(path);

            CreateLinesOffMPPTs(
                formData,
                numberOfMPPTs,
                selectedPoint,
                out List<List<Dictionary<string, double>>> mpptEndPoints
            );

            BlockDataMethods.CreateObjectGivenData(MPPTData, ed, selectedPoint);

            CreateStringsOffMPPTS(
                formData,
                numberOfMPPTs,
                selectedPoint,
                mpptEndPoints,
                INCREASE_Y_TEXTBOX,
                INCREASE_X_TEXTBOX,
                smallStrings
            );
        }

        private static int GetNumberOfMPPTs(Dictionary<string, Dictionary<string, object>> formData)
        {
            int numberOfMPPTs = 0;
            foreach (var mppt in formData)
            {
                var mpptData = mppt.Value as Dictionary<string, object>;
                if (Convert.ToBoolean(mpptData["Enabled"]))
                {
                    numberOfMPPTs++;
                }
            }

            return numberOfMPPTs;
        }

        private static void CreateStringsOffMPPTS(
            Dictionary<string, Dictionary<string, object>> formData,
            int numberOfMPPTs,
            Point3d selectedPoint,
            List<List<Dictionary<string, double>>> mpptEndPoints,
            string INCREASE_TEXTBOX_Y,
            string INCREASE_TEXTBOX_X,
            bool smallStrings = false
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

            if (numberOfMPPTs == 1)
            {
                lineStartPointX = selectedPoint.X - (1.0213 + (0.6484 * (numberOfMPPTs - 1)));
            }

            var lineStartPointY = selectedPoint.Y - 0.6098;
            var stringMidPointX = lineStartPointX - 3.3606;
            var stringMidPointY = lineStartPointY - 1.2706;

            var stringDataContainer = GetStringData(formData, smallStrings);

            if (AreAllModulesEmpty(stringDataContainer))
            {
                return;
            }

            var skipIndices = new List<int>();

            skipIndices = GetIndicesToSkip(formData);

            var stringTotalHeight = GetTotalHeightOfStringModule(stringDataContainer);

            var stringStartPoint = new Point3d(
                stringMidPointX,
                stringMidPointY + stringTotalHeight / 2,
                0
            );

            var increaseY = 0.0;
            var increaseX = 0.0;

            if (INCREASE_TEXTBOX_X != "")
            {
                increaseX = Convert.ToDouble(INCREASE_TEXTBOX_X);
                stringStartPoint = new Point3d(stringMidPointX + increaseX, stringStartPoint.Y, 0);
            }

            if (INCREASE_TEXTBOX_Y != "")
            {
                increaseY = Convert.ToDouble(INCREASE_TEXTBOX_Y);
                stringStartPoint = new Point3d(
                    stringStartPoint.X,
                    stringStartPoint.Y + increaseY,
                    0
                );
            }

            var stringsPerChunk = GetStringsPerChunk(stringDataContainer);

            var connectionPoints = CreateStringsAndReturnConnectionPoints(
                stringDataContainer,
                stringStartPoint,
                stringsPerChunk,
                ed,
                smallStrings
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

        private static List<int> GetStringsPerChunk(
            List<Dictionary<string, object>> stringDataContainer
        )
        {
            var stringsPerChunk = new List<int>();
            int stringCount = 0;

            var allItems = stringDataContainer.SelectMany(dict => dict["items"] as List<string>);

            foreach (var itemType in allItems)
            {
                if (itemType == "string")
                {
                    stringCount++;
                }
                else
                {
                    if (stringCount > 0)
                    {
                        stringsPerChunk.Add(stringCount);
                        stringCount = 0;
                    }
                }
            }

            // If the last item was a "string", add the count to the list
            if (stringCount > 0)
            {
                stringsPerChunk.Add(stringCount);
            }

            return stringsPerChunk;
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

        private static bool AreAllModulesEmpty(List<Dictionary<string, object>> stringDataContainer)
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

            for (var i = 0; i < connectionPoints.Count; i++)
            {
                var mpptEndPoint = mpptEndPointsFlattened[i];
                var connectionPoint = connectionPoints[i];

                var xDistance = Math.Abs(mpptEndPoint["x"] - connectionPoint["x"]);

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
                                - (aboveOrBelowValue * LINE_SPACING / 2)
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
                    { "x", connectionPoints[0]["x"] },
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
                { "x", firstHorizontalLineStartPoint["x"] - CELL_SPACING * 3 / 4 },
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
                { "x", firstHorizontalLineEndPoint["x"] - CELL_SPACING / 2 },
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

            if (!LayerExists("E-CONDUIT"))
            {
                CreateLayer("E-CONDUIT", 4);
            }

            line.Layer = "E-CONDUIT";

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
            List<int> stringsPerChunk,
            Editor ed,
            bool smallStrings = false
        )
        {
            var TEXT_HEIGHT = 0.8334;
            var STRING_HEIGHT = 1.0695;
            var STRING_HEIGHT_SMALL = 0.7648;
            var CELL_SPACING = 0.1621;
            int stringIndex = 0;
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
                int.TryParse(stringData["module"].ToString(), out int module);
                foreach (var item in items)
                {
                    if (item == "text")
                    {
                        var stringAmountPerChunk = stringsPerChunk[stringIndex];
                        stringIndex++;

                        CreateStringTextInCAD(
                            ed,
                            new Point3d(startPoint["x"], startPoint["y"], 0),
                            stringAmountPerChunk,
                            module
                        );
                        startPoint["y"] -= TEXT_HEIGHT;
                    }
                    else if (item == "string")
                    {
                        CreateStringObjectInCAD(
                            ed,
                            new Point3d(startPoint["x"], startPoint["y"], 0),
                            smallStrings
                        );
                        var stringHeight = (smallStrings ? STRING_HEIGHT_SMALL : STRING_HEIGHT);
                        connectionPoints.Add(
                            new Dictionary<string, double>
                            {
                                { "x", startPoint["x"] },
                                { "y", startPoint["y"] - (stringHeight / 2) + (CELL_SPACING / 2) }
                            }
                        );
                        connectionPoints.Add(
                            new Dictionary<string, double>
                            {
                                { "x", startPoint["x"] },
                                { "y", startPoint["y"] - (stringHeight / 2) - (CELL_SPACING / 2) }
                            }
                        );
                        startPoint["y"] -= stringHeight;
                    }
                }
            }
            return connectionPoints;
        }

        private static void CreateStringTextInCAD(
            Editor ed,
            Point3d startPoint,
            int stringAmountPerChunk,
            int module
        )
        {
            var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var jsonPath = Path.Combine(dllPath, $"block data/StringText.json");
            var json = File.ReadAllText(jsonPath);
            var stringTextData = JArray
                .Parse(json)
                .ToObject<List<Dictionary<string, Dictionary<string, object>>>>();

            stringTextData[0]["mtext"]["text"] =
                stringTextData[0]
                    ["mtext"]["text"]
                    .ToString()
                    .Replace("*", module.ToString().PadLeft(2, '0'))
                    .Replace("STRING 1", $"{stringAmountPerChunk} STRINGS") + " EA.";

            BlockDataMethods.CreateObjectGivenData(stringTextData, ed, startPoint);
        }

        private static void CreateStringObjectInCAD(
            Editor ed,
            Point3d stringStartPoint,
            bool smallStrings = false
        )
        {
            var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var jsonPath = Path.Combine(dllPath, $"block data/String.json");
            if (smallStrings)
            {
                jsonPath = Path.Combine(dllPath, $"block data/StringSmallScale.json");
            }
            var json = File.ReadAllText(jsonPath);
            var stringData = JArray
                .Parse(json)
                .ToObject<List<Dictionary<string, Dictionary<string, object>>>>();

            BlockDataMethods.CreateObjectGivenData(stringData, ed, stringStartPoint);
        }

        private static double GetTotalHeightOfStringModule(
            List<Dictionary<string, object>> stringDataContainer
        )
        {
            double totalHeight = 0;
            foreach (var stringData in stringDataContainer)
            {
                totalHeight += Convert.ToDouble(stringData["height"]);
            }
            return totalHeight;
        }

        private static List<Dictionary<string, object>> GetStringData(
            Dictionary<string, Dictionary<string, object>> formData,
            bool smallStrings = false
        )
        {
            var TEXT_HEIGHT = 0.8334;
            var STRING_HEIGHT = smallStrings ? 0.7648 : 1.0695;
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
                        stringData["height"] = (double)stringData["height"] + TEXT_HEIGHT;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }
                    else if ((isRegular || isParallel) && modulesCount != previousModulesCount)
                    {
                        stringData["height"] = (double)stringData["height"] + TEXT_HEIGHT;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }

                    if (isRegular)
                    {
                        stringData["height"] = (double)stringData["height"] + STRING_HEIGHT;
                        ((List<string>)stringData["items"]).Add("string");
                    }
                    else if (isParallel)
                    {
                        stringData["height"] = (double)stringData["height"] + (STRING_HEIGHT * 2);
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
            Point3d selectedPoint,
            out List<List<Dictionary<string, double>>> mpptEndPoints
        )
        {
            Double STARTLINE_LENGTH = 1.25;
            Double CELL_SPACING = 0.1621;
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
                    endPoint["y"] -= CELL_SPACING / 2;
                }
                else if (isParallel && enabled)
                {
                    endPoint["y"] -= CELL_SPACING;
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
                        { "y", endPointUpdated["y"] - CELL_SPACING / 2 }
                    };

                    CreateLineFromPoints(endPointUpdated, arcEndPoint);
                }
                else if (i == 2)
                {
                    endPointUpdated["y"] -= CELL_SPACING / 4;
                }
                else if (i == 3)
                {
                    endPointUpdated["y"] -= CELL_SPACING / 2 + CELL_SPACING / 4;
                }

                var line = new Line(
                    new Point3d(startPointUpdated["x"], startPointUpdated["y"], 0),
                    new Point3d(endPointUpdated["x"], endPointUpdated["y"], 0)
                );

                if (!LayerExists("E-CONDUIT"))
                {
                    CreateLayer("E-CONDUIT", 4);
                }

                line.Layer = "E-CONDUIT";

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
                            { "y", endPointUpdated["y"] - CELL_SPACING / 2 }
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

                    if (!LayerExists("E-CONDUIT"))
                    {
                        CreateLayer("E-CONDUIT", 4);
                    }

                    arc.Layer = "E-CONDUIT";
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

                    if (!LayerExists("E-CONDUIT"))
                    {
                        CreateLayer("E-CONDUIT", 4);
                    }

                    acCircle.Layer = "E-CONDUIT";

                    acCircle.SetDatabaseDefaults();
                    acBlkTblRec.AppendEntity(acCircle);
                    acTrans.AddNewlyCreatedDBObject(acCircle, true);

                    using (Hatch acHatch = new Hatch())
                    {
                        acBlkTblRec.AppendEntity(acHatch);
                        acTrans.AddNewlyCreatedDBObject(acHatch, true);
                        acHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                        acHatch.Associative = true;
                        acHatch.Layer = "E-CONDUIT";
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
            Double ENDPOINT_INCREASE = -CELL_SPACING / 4;

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

                if (!LayerExists("E-CONDUIT"))
                {
                    CreateLayer("E-CONDUIT", 4);
                }

                line.Layer = "E-CONDUIT";

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

        private static void CreateLayer(string layerName, short colorIndex)
        {
            if (layerName.Contains("SYM"))
            {
                colorIndex = 6;
            }

            if (layerName.Contains("Inverter"))
            {
                colorIndex = 2;
            }

            using (
                var transaction =
                    HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()
            )
            {
                var layerTable =
                    transaction.GetObject(
                        HostApplicationServices.WorkingDatabase.LayerTableId,
                        OpenMode.ForRead
                    ) as LayerTable;

                if (!layerTable.Has(layerName))
                {
                    var layerTableRecord = new LayerTableRecord();
                    layerTableRecord.Name = layerName;
                    layerTableRecord.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                    layerTableRecord.LineWeight = LineWeight.LineWeight050;
                    layerTableRecord.IsPlottable = true;

                    layerTable.UpgradeOpen();
                    layerTable.Add(layerTableRecord);
                    transaction.AddNewlyCreatedDBObject(layerTableRecord, true);
                    transaction.Commit();
                }
            }
        }

        private static bool LayerExists(string layerName)
        {
            using (
                var transaction =
                    HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()
            )
            {
                var layerTable =
                    transaction.GetObject(
                        HostApplicationServices.WorkingDatabase.LayerTableId,
                        OpenMode.ForRead
                    ) as LayerTable;

                return layerTable.Has(layerName);
            }
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

        private bool CheckForEmptyInputs()
        {
            if (
                (MPPT1_RADIO_REGULAR.Checked || MPPT1_RADIO_PARALLEL.Checked)
                && MPPT1_INPUT.Text == ""
            )
            {
                MessageBox.Show("Please enter the number of modules for MPPT1");
                return false;
            }

            if (
                (MPPT2_RADIO_REGULAR.Checked || MPPT2_RADIO_PARALLEL.Checked)
                && MPPT2_INPUT.Text == ""
            )
            {
                MessageBox.Show("Please enter the number of modules for MPPT2");
                return false;
            }

            if (
                (MPPT3_RADIO_REGULAR.Checked || MPPT3_RADIO_PARALLEL.Checked)
                && MPPT3_INPUT.Text == ""
            )
            {
                MessageBox.Show("Please enter the number of modules for MPPT3");
                return false;
            }

            if (
                (MPPT4_RADIO_REGULAR.Checked || MPPT4_RADIO_PARALLEL.Checked)
                && MPPT4_INPUT.Text == ""
            )
            {
                MessageBox.Show("Please enter the number of modules for MPPT4");
                return false;
            }

            return true;
        }

        internal void AlterComponents()
        {
            if (INCREASE_Y_TEXTBOX != null)
            {
                INCREASE_Y_TEXTBOX.Dispose();
                INCREASE_Y_TEXTBOX = null;
            }

            if (INCREASE_Y_LABEL != null)
            {
                INCREASE_Y_LABEL.Dispose();
                INCREASE_Y_LABEL = null;
            }

            if (INCREASE_X_TEXTBOX != null)
            {
                INCREASE_X_TEXTBOX.Dispose();
                INCREASE_X_TEXTBOX = null;
            }

            if (INCREASE_X_LABEL != null)
            {
                INCREASE_X_LABEL.Dispose();
                INCREASE_X_LABEL = null;
            }

            CREATE_BUTTON.Text = "DONE";
        }

        internal void UpdateValues(
            string name,
            bool mppt1Enabled,
            bool mppt1Regular,
            bool mppt1Parallel,
            object mppt1Input
        )
        {
            if (name == "MPPT1")
            {
                MPPT1_CHECKBOX.Checked = mppt1Enabled;
                MPPT1_RADIO_REGULAR.Checked = mppt1Regular;
                MPPT1_RADIO_PARALLEL.Checked = mppt1Parallel;
                MPPT1_INPUT.Text = mppt1Input.ToString();
            }
            else if (name == "MPPT2")
            {
                MPPT2_CHECKBOX.Checked = mppt1Enabled;
                MPPT2_RADIO_REGULAR.Checked = mppt1Regular;
                MPPT2_RADIO_PARALLEL.Checked = mppt1Parallel;
                MPPT2_INPUT.Text = mppt1Input.ToString();
            }
            else if (name == "MPPT3")
            {
                MPPT3_CHECKBOX.Checked = mppt1Enabled;
                MPPT3_RADIO_REGULAR.Checked = mppt1Regular;
                MPPT3_RADIO_PARALLEL.Checked = mppt1Parallel;
                MPPT3_INPUT.Text = mppt1Input.ToString();
            }
            else if (name == "MPPT4")
            {
                MPPT4_CHECKBOX.Checked = mppt1Enabled;
                MPPT4_RADIO_REGULAR.Checked = mppt1Regular;
                MPPT4_RADIO_PARALLEL.Checked = mppt1Parallel;
                MPPT4_INPUT.Text = mppt1Input.ToString();
            }
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
            if (MPPT1_RADIO_REGULAR.Enabled)
            {
                MPPT1_RADIO_REGULAR.Checked = true;
            }
            if (MPPT2_RADIO_REGULAR.Enabled)
            {
                MPPT2_RADIO_REGULAR.Checked = true;
            }
            if (MPPT3_RADIO_REGULAR.Enabled)
            {
                MPPT3_RADIO_REGULAR.Checked = true;
            }
            if (MPPT4_RADIO_REGULAR.Enabled)
            {
                MPPT4_RADIO_REGULAR.Checked = true;
            }
        }

        private void ALL_PARALLEL_BUTTON_Click(object sender, EventArgs e)
        {
            if (MPPT1_RADIO_PARALLEL.Enabled)
            {
                MPPT1_RADIO_PARALLEL.Checked = true;
            }
            if (MPPT2_RADIO_PARALLEL.Enabled)
            {
                MPPT2_RADIO_PARALLEL.Checked = true;
            }
            if (MPPT3_RADIO_PARALLEL.Enabled)
            {
                MPPT3_RADIO_PARALLEL.Checked = true;
            }
            if (MPPT4_RADIO_PARALLEL.Enabled)
            {
                MPPT4_RADIO_PARALLEL.Checked = true;
            }
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
            var inputHasValue = CheckForEmptyInputs();

            if (!inputHasValue)
            {
                return;
            }

            if (CREATE_BUTTON.Text == "DONE" && this.inverterForm != null)
            {
                var dcData = GetFormData(this);
                var oldDataObject = this.inverterForm.GetCurrentDCData();
                oldDataObject[this.id] = dcData;
                this.inverterForm.SaveDCData(oldDataObject);
                Close();
                return;
            }

            var data = GetFormData(this);

            var isToBeLeftOpen = GetNumberOfMPPTs(data) == 0;

            var increaseY = INCREASE_Y_TEXTBOX.Text;
            var increaseX = INCREASE_X_TEXTBOX.Text;

            if (!isToBeLeftOpen)
            {
                Close();
            }

            using (
                DocumentLock docLock =
                    Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
            )
            {
                CreateDCSolarObject(data, increaseY, increaseX, new Point3d(0, 0, 0));
            }
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
