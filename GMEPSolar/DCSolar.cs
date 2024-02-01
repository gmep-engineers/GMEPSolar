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

        public static void CreateDCSolarObject(Dictionary<string, object> formData)
        {
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
            var data = JArray
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
                Point3d selectedPoint = pointResult.Value;

                var startPoint = new Dictionary<string, double>
                {
                    { "x", selectedPoint.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1))) },
                    { "y", selectedPoint.Y - 0.6098 }
                };

                var cellSpacing = 0.1621;
                var numberOfCellsPerMPPT = 4;
                var totalNumberOfCells = numberOfMPPTs * numberOfCellsPerMPPT;
                var startingLineLength = 1.25;

                for (int i = 0; i <= totalNumberOfCells - 1; i++)
                {
                    var lineLength = startingLineLength + (i * cellSpacing / 2);
                    var currentPoint = new Point3d(
                        startPoint["x"] + (i * cellSpacing),
                        startPoint["y"],
                        selectedPoint.Z
                    );
                    var line = CreateVerticalLine(currentPoint, lineLength);
                }

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

                        default:
                            break;
                    }
                }
            }
        }

        private static object CreateVerticalLine(Point3d currentPoint, double lineLength)
        {
            var startPoint = currentPoint;
            var endPoint = new Point3d(currentPoint.X, currentPoint.Y - lineLength, currentPoint.Z);

            using (
                var transaction =
                    HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()
            )
            {
                var blockTable =
                    transaction.GetObject(
                        HostApplicationServices.WorkingDatabase.BlockTableId,
                        OpenMode.ForRead
                    ) as BlockTable;

                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;

                var line = new Line(startPoint, endPoint);
                line.Layer = "0";
                line.Color = Color.FromColorIndex(ColorMethod.ByAci, 4);
                blockTableRecord.AppendEntity(line);
                transaction.AddNewlyCreatedDBObject(line, true);
                transaction.Commit();
            }

            return currentPoint;
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

        private object GetFormData()
        {
            var data = new Dictionary<string, object>
            {
                {
                    "MPPT1",
                    new Dictionary<string, object>
                    {
                        { "Enabled", MPPT1_CHECKBOX.Checked },
                        { "Regular", MPPT1_RADIO_REGULAR.Checked },
                        { "Parallel", MPPT1_RADIO_PARALLEL.Checked },
                        { "Input", MPPT1_INPUT.Text }
                    }
                },
                {
                    "MPPT2",
                    new Dictionary<string, object>
                    {
                        { "Enabled", MPPT2_CHECKBOX.Checked },
                        { "Regular", MPPT2_RADIO_REGULAR.Checked },
                        { "Parallel", MPPT2_RADIO_PARALLEL.Checked },
                        { "Input", MPPT2_INPUT.Text }
                    }
                },
                {
                    "MPPT3",
                    new Dictionary<string, object>
                    {
                        { "Enabled", MPPT3_CHECKBOX.Checked },
                        { "Regular", MPPT3_RADIO_REGULAR.Checked },
                        { "Parallel", MPPT3_RADIO_PARALLEL.Checked },
                        { "Input", MPPT3_INPUT.Text }
                    }
                },
                {
                    "MPPT4",
                    new Dictionary<string, object>
                    {
                        { "Enabled", MPPT4_CHECKBOX.Checked },
                        { "Regular", MPPT4_RADIO_REGULAR.Checked },
                        { "Parallel", MPPT4_RADIO_PARALLEL.Checked },
                        { "Input", MPPT4_INPUT.Text }
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
            MPPT1_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT2_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT3_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT4_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
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
            var data = GetFormData();
            CreateDCSolarObject((Dictionary<string, object>)data);
        }
    }
}
