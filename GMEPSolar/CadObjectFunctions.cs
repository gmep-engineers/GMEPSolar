using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace GMEPSolar
{
  public class CadObjectFunctions
  {
    public CadObjectFunctions() { }

    public static string GetProjectNameFromFileName()
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Core
        .Application
        .DocumentManager
        .MdiActiveDocument;
      return Path.GetFileName(doc.Name).Replace(".dwg", "").Replace(".DWG", "").ToUpper();
    }

    public static void MakeBlock(Point3d point, string blockName, string layerName = "E-CONDUIT")
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord btr = (BlockTableRecord)
          tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
        ObjectId symbol = bt[blockName];
        using (
          BlockReference acBlkRef = new BlockReference(new Point3d(point.X, point.Y, 0), symbol)
        )
        {
          acBlkRef.Layer = layerName;
          BlockTableRecord acCurSpaceBlkTblRec;
          acCurSpaceBlkTblRec =
            tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
          acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
          tr.AddNewlyCreatedDBObject(acBlkRef, true);
        }
        tr.Commit();
      }
    }

    public static void MakeLine(Point3d start, Point3d end, string layerName = "E-CONDUIT")
    {
      Line line = new Line();
      line.Layer = layerName;
      line.StartPoint = start;
      line.EndPoint = end;
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord btr = (BlockTableRecord)
          tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
        btr.AppendEntity(line);
        tr.AddNewlyCreatedDBObject(line, true);
        tr.Commit();
      }
    }

    public static void MakeText(Point3d point, string text, bool vertical = false)
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord btr = (BlockTableRecord)
          tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
        var textStyleTable = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);
        DBText dBText = new DBText
        {
          TextString = text,
          Height = 0.0938,
          WidthFactor = 1,
          Layer = "E-TEXT",
          Rotation = vertical ? 1.5708 : 0,
          TextStyleId = textStyleTable["ArialMT"],
          Position = point,
        };
        btr.AppendEntity(dBText);
        tr.AddNewlyCreatedDBObject(dBText, true);
        tr.Commit();
      }
    }
  }
}
