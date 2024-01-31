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

        private static dynamic GetTemplate()
        {
            var template = new
            {
                polyline = new { layer = "", vertices = new List<object>() },
                line = new { layer = "", vertices = new List<object>() },
                mtext = new
                {
                    layer = "",
                    style = "",
                    justification = "",
                    text = "",
                    height = 0.0,
                    lineSpaceDistance = 0.0,
                    location = new
                    {
                        x = 0.0,
                        y = 0.0,
                        z = 0.0
                    }
                },
                circle = new
                {
                    layer = "",
                    center = new
                    {
                        x = 0.0,
                        y = 0.0,
                        z = 0.0
                    },
                    radius = 0.0
                }
            };

            return template;
        }
    }
}
