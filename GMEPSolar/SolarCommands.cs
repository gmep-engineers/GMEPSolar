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
    public class SolarCommands
    {
        [CommandMethod("DCSolar")]
        public static void DCSolar()
        {
            DC_SOLAR_INPUT form = new DC_SOLAR_INPUT();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form);
        }

        [CommandMethod("GetObjectData")]
        public static void GetObjectData()
        {
            var data = new List<Dictionary<string, object>>();

            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk
                .AutoCAD
                .ApplicationServices
                .Application
                .DocumentManager
                .MdiActiveDocument
                .Editor;
            Autodesk.AutoCAD.EditorInput.PromptSelectionResult selectionResult = ed.GetSelection();
            if (selectionResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
            {
                Autodesk.AutoCAD.EditorInput.SelectionSet selectionSet = selectionResult.Value;

                // Prompt the user to select an origin point
                Autodesk.AutoCAD.EditorInput.PromptPointOptions originOptions =
                    new Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select an origin point: ");
                Autodesk.AutoCAD.EditorInput.PromptPointResult originResult = ed.GetPoint(
                    originOptions
                );
                if (originResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
                {
                    Point3d origin = originResult.Value;

                    foreach (
                        Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in selectionSet.GetObjectIds()
                    )
                    {
                        using (
                            Transaction transaction =
                                objectId.Database.TransactionManager.StartTransaction()
                        )
                        {
                            Autodesk.AutoCAD.DatabaseServices.DBObject obj = transaction.GetObject(
                                objectId,
                                Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead
                            );

                            ed.WriteMessage(obj.GetType().ToString());

                            // Check the type of the selected object
                            if (obj is Autodesk.AutoCAD.DatabaseServices.Polyline)
                            {
                                // Handle polyline
                                data = HandlePolyline(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Polyline,
                                    data,
                                    origin
                                );
                            }
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.Line)
                            {
                                // Handle line
                                data = HandleLine(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Line,
                                    data,
                                    origin
                                );
                            }
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.MText)
                            {
                                // Handle MText
                                data = HandleMText(
                                    obj as Autodesk.AutoCAD.DatabaseServices.MText,
                                    data,
                                    origin
                                );
                            }
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.Circle)
                            {
                                // Handle circle
                                data = HandleCircle(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Circle,
                                    data,
                                    origin
                                );
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                    }
                }
            }

            SaveDataToJsonFile(data, "data.json");
        }

        [CommandMethod("CreateDCSolarObject")]
        public static void CreateDCSolarObject()
        {
            var json = File.ReadAllText("../../DCSolarBlock.json");
            var data = JArray.Parse(json).ToObject<List<Dictionary<string, object>>>();

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

        private static Point3d CreateCircle(
            Editor ed,
            Point3d selectedPoint,
            Dictionary<string, object> objData
        )
        {
            var circleData = objData["circle"] as Dictionary<string, object>;
            var circle = new Circle();

            // Set circle properties
            circle.Layer = circleData["layer"].ToString();

            var centerData = circleData["center"] as Dictionary<string, object>;
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
                        blockTable[BlockTableRecord.ModelSpace],
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
            Dictionary<string, object> objData
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

            var locationData = mtextData["location"] as Dictionary<string, object>;
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
                        blockTable[BlockTableRecord.ModelSpace],
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
            Dictionary<string, object> objData
        )
        {
            var lineData = objData["line"] as Dictionary<string, object>;
            var line = new Line();

            // Set line properties
            line.Layer = lineData["layer"].ToString();

            var startPointData = lineData["startPoint"] as Dictionary<string, object>;
            var startPtX = Convert.ToDouble(startPointData["x"]) + selectedPoint.X;
            var startPtY = Convert.ToDouble(startPointData["y"]) + selectedPoint.Y;
            var startPtZ = Convert.ToDouble(startPointData["z"]) + selectedPoint.Z;
            line.StartPoint = new Point3d(startPtX, startPtY, startPtZ);

            var endPointData = lineData["endPoint"] as Dictionary<string, object>;
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
                        blockTable[BlockTableRecord.ModelSpace],
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
            Dictionary<string, object> objData
        )
        {
            var polylineData = objData["polyline"] as Dictionary<string, object>;
            var polyline = new Polyline();

            // Set polyline properties
            polyline.Layer = polylineData["layer"].ToString();

            var vertices = polylineData["vertices"] as List<object>;
            foreach (var vertex in vertices)
            {
                var vertexData = vertex as Dictionary<string, object>;
                var x = Convert.ToDouble(vertexData["x"]) + selectedPoint.X;
                var y = Convert.ToDouble(vertexData["y"]) + selectedPoint.Y;
                var z = Convert.ToDouble(vertexData["z"]) + selectedPoint.Z;
                polyline.AddVertexAt(polyline.NumberOfVertices, new Point2d(x, y), z, 0, 0);
            }

            // Add polyline to the drawing
            using (var transaction = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var blockTable =
                    transaction.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead)
                    as BlockTable;
                var blockTableRecord =
                    transaction.GetObject(
                        blockTable[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite
                    ) as BlockTableRecord;
                blockTableRecord.AppendEntity(polyline);
                transaction.AddNewlyCreatedDBObject(polyline, true);
                transaction.Commit();
            }

            return selectedPoint;
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

        private static void SaveDataToJsonFile(
            List<Dictionary<string, object>> data,
            string fileName
        )
        {
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                fileName
            );
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                data,
                Newtonsoft.Json.Formatting.Indented
            );
            File.WriteAllText(filePath, json);
        }

        private static List<Dictionary<string, object>> HandlePolyline(
            Autodesk.AutoCAD.DatabaseServices.Polyline polyline,
            List<Dictionary<string, object>> data,
            Point3d origin
        )
        {
            var polylineData = new Dictionary<string, object>();
            polylineData.Add("layer", polyline.Layer);

            var vertices = new List<object>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                var vertex = new Dictionary<string, object>
                {
                    { "x", polyline.GetPoint3dAt(i).X - origin.X },
                    { "y", polyline.GetPoint3dAt(i).Y - origin.Y },
                    { "z", polyline.GetPoint3dAt(i).Z - origin.Z }
                };
                vertices.Add(vertex);
            }

            polylineData.Add("vertices", vertices);

            var encapsulate = new Dictionary<string, object>();
            encapsulate.Add("polyline", polylineData);

            data.Add(encapsulate);

            return data;
        }

        private static List<Dictionary<string, object>> HandleLine(
            Autodesk.AutoCAD.DatabaseServices.Line line,
            List<Dictionary<string, object>> data,
            Point3d origin
        )
        {
            var lineData = new Dictionary<string, object>();
            lineData.Add("layer", line.Layer);

            var startPoint = new Dictionary<string, object>
            {
                { "x", line.StartPoint.X - origin.X },
                { "y", line.StartPoint.Y - origin.Y },
                { "z", line.StartPoint.Z - origin.Z }
            };
            lineData.Add("startPoint", startPoint);

            var endPoint = new Dictionary<string, object>
            {
                { "x", line.EndPoint.X - origin.X },
                { "y", line.EndPoint.Y - origin.Y },
                { "z", line.EndPoint.Z - origin.Z }
            };
            lineData.Add("endPoint", endPoint);

            var encapsulate = new Dictionary<string, object>();
            encapsulate.Add("line", lineData);

            data.Add(encapsulate);

            return data;
        }

        private static List<Dictionary<string, object>> HandleMText(
            Autodesk.AutoCAD.DatabaseServices.MText mtext,
            List<Dictionary<string, object>> data,
            Point3d origin
        )
        {
            var mtextData = new Dictionary<string, object>();
            mtextData.Add("layer", mtext.Layer);
            mtextData.Add("style", mtext.TextStyleName);
            mtextData.Add("justification", mtext.Attachment.ToString());
            mtextData.Add("text", mtext.Contents);
            mtextData.Add("height", mtext.TextHeight);
            mtextData.Add("lineSpaceDistance", mtext.LineSpaceDistance);

            var location = new Dictionary<string, object>
            {
                { "x", mtext.Location.X - origin.X },
                { "y", mtext.Location.Y - origin.Y },
                { "z", mtext.Location.Z - origin.Z }
            };
            mtextData.Add("location", location);

            var encapsulate = new Dictionary<string, object>();
            encapsulate.Add("mtext", mtextData);

            data.Add(encapsulate);

            return data;
        }

        private static List<Dictionary<string, object>> HandleCircle(
            Autodesk.AutoCAD.DatabaseServices.Circle circle,
            List<Dictionary<string, object>> data,
            Point3d origin
        )
        {
            var circleData = new Dictionary<string, object>();
            circleData.Add("layer", circle.Layer);

            var center = new Dictionary<string, object>
            {
                { "x", circle.Center.X - origin.X },
                { "y", circle.Center.Y - origin.Y },
                { "z", circle.Center.Z - origin.Z }
            };
            circleData.Add("center", center);

            circleData.Add("radius", circle.Radius);

            var encapsulate = new Dictionary<string, object>();
            encapsulate.Add("circle", circleData);

            data.Add(encapsulate);

            return data;
        }
    }
}
