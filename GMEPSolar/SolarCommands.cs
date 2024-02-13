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
using static GMEPUtilities.HelperMethods;
using static GMEPUtilities.RetrieveObjectData;

namespace GMEPSolar
{
    public class SolarCommands
    {
        [CommandMethod("DCSolar")]
        public static void DCSolar()
        {
            if (IsInModel())
            {
                MessageBox.Show("You are in model space. Please switch to paper space.");
                return;
            }
            DC_SOLAR_INPUT form = new DC_SOLAR_INPUT();
            form.Show();
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
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.Solid)
                            {
                                data = HandleSolid(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Solid,
                                    data,
                                    origin
                                );
                            }
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.Arc)
                            {
                                data = HandleArc(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Arc,
                                    data,
                                    origin
                                );
                            }
                            else if (obj is Autodesk.AutoCAD.DatabaseServices.Ellipse)
                            {
                                data = HandleEllipse(
                                    obj as Autodesk.AutoCAD.DatabaseServices.Ellipse,
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

        [CommandMethod("Inverter")]
        public static void Inverter()
        {
            if (IsInModel())
            {
                MessageBox.Show("You are in model space. Please switch to paper space.");
                return;
            }
            InverterForm form = new InverterForm();
            form.Show();
        }

        [CommandMethod("GetRelativePoint")]
        public static void GetRelativePoint()
        {
            PromptPointResult pointResult;
            GetUserToClick(out _, out pointResult);
            PromptPointResult pointResult2;
            GetUserToClick(out _, out pointResult2);

            if (pointResult.Status == PromptStatus.OK && pointResult2.Status == PromptStatus.OK)
            {
                var point = pointResult.Value;
                var point2 = pointResult2.Value;

                var relativePoint = new Point3d(
                    point.X - point2.X,
                    point.Y - point2.Y,
                    point.Z - point2.Z
                );

                SaveDataToJsonFile(relativePoint, "relativePoint.json");
            }
        }
    }
}
