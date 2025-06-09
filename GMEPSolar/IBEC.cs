using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace GMEPSolar
{
  public partial class IBEC : Form
  {
    private List<PowerStation> PowerStations;
    private GmepDatabase GmepDatabase;
    private string ProjectId;

    public IBEC()
    {
      GmepDatabase = new GmepDatabase();
      string projectName = CadObjectFunctions.GetProjectNameFromFileName();
      ProjectId = GmepDatabase.GetProjectId(projectName);

      if (String.IsNullOrEmpty(ProjectId))
      {
        Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk
          .AutoCAD
          .ApplicationServices
          .Application
          .DocumentManager
          .MdiActiveDocument;
        Editor ed = doc.Editor;
        string path = Autodesk
          .AutoCAD
          .ApplicationServices
          .Application
          .DocumentManager
          .CurrentDocument
          .Database
          .Filename;
        string[] directories = path.Split('\\');
        string year = "2025";
        string client = "";
        for (int i = 0; i < directories.Length; i++)
        {
          if (directories[i].Contains(" Jobs"))
          {
            if (directories[i].Contains(" Jobs"))
            {
              year = directories[i].Replace(" Jobs", "");
              client = directories[i - 1];
            }
            if (directories[i].Contains("jobs"))
            {
              year = directories[i].Replace("jobs", "");
              client = directories[i + 1];
            }
          }
        }
        ProjectId = GmepDatabase.CreateProject(projectName, client.ToUpper(), year, path);
        ed.WriteMessage($"Project {projectName} has been created in the database.");
      }
      PowerStations = GmepDatabase.ReadPowerStations(ProjectId);

      InitializeComponent();
      foreach (PowerStation powerStation in PowerStations)
      {
        powerStation.Lots = GmepDatabase.ReadLots(powerStation.Id);

        PowerStationUserControl powerStationUserControl = new PowerStationUserControl(powerStation);

        PowerStationFlowLayoutPanel.Controls.Add(powerStationUserControl);
      }
    }

    private void AddPowerStationButton_Click(object sender, EventArgs e)
    {
      PowerStation powerStation = new PowerStation(
        Guid.NewGuid().ToString(),
        1,
        2,
        UpdateAction.Create
      );
      PowerStationUserControl powerStationUserControl = new PowerStationUserControl(powerStation);
      PowerStationFlowLayoutPanel.Controls.Add(powerStationUserControl);
    }

    private void SaveButton_Click(Object sender, EventArgs e)
    {
      foreach (
        PowerStationUserControl powerStationUserControl in PowerStationFlowLayoutPanel.Controls
      )
      {
        powerStationUserControl.Save();
        GmepDatabase.CreatePowerStation(powerStationUserControl.PowerStation, ProjectId);
        GmepDatabase.UpdatePowerStation(powerStationUserControl.PowerStation);
        GmepDatabase.DeletePowerStation(powerStationUserControl.PowerStation);
        GmepDatabase.CreateLots(powerStationUserControl.PowerStation.Lots, ProjectId);
        GmepDatabase.UpdateLots(powerStationUserControl.PowerStation.Lots);
        GmepDatabase.DeleteLots(powerStationUserControl.PowerStation.Lots);
      }
    }

    struct LoadSummary
    {
      public List<string> Numbers;
      public List<int> Loads;
      public int Subtotal;
      public double TotalAmps;
      public string Voltage;
      public int RecommendedService;
    }

    private void LoadSummaryButton_Click(Object sender, EventArgs e)
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      Editor ed = doc.Editor;
      List<LoadSummary> loadSummaries = new List<LoadSummary>();
      foreach (
        PowerStationUserControl powerStationUserControl in PowerStationFlowLayoutPanel.Controls
      )
      {
        PowerStation powerStation = powerStationUserControl.PowerStation;
        if (powerStation.Action == UpdateAction.Delete)
        {
          continue;
        }
        powerStationUserControl.Save();
        foreach (Lot lot in powerStation.Lots)
        {
          // create load summary for single lot
          LoadSummary loadSummary = new LoadSummary();
          loadSummary.Numbers = new List<string>();
          loadSummary.Loads = new List<int>();
          loadSummary.Numbers.Add(lot.Number);
          loadSummary.Loads.Add(lot.LoadVa);
          double voltage = 240;
          if (lot.Voltage.Contains("208"))
          {
            voltage = 208;
          }
          if (lot.Voltage.Contains("480"))
          {
            voltage = 480;
          }

          loadSummary.TotalAmps =
            (double)lot.LoadVa / voltage / (lot.Voltage.Contains("4W") ? 1.732 : 1);
          loadSummary.TotalAmps = Math.Round(loadSummary.TotalAmps, 1);
          if (loadSummary.TotalAmps < 200)
          {
            loadSummary.RecommendedService = 200;
          }
          if (loadSummary.TotalAmps < 100)
          {
            loadSummary.RecommendedService = 100;
          }
          if (loadSummary.TotalAmps < 60)
          {
            loadSummary.RecommendedService = 60;
          }
          loadSummary.Voltage = lot.Voltage;
          loadSummaries.Add(loadSummary);
        }
        if (powerStation.Lots.Count > 1)
        {
          LoadSummary loadSummary = new LoadSummary();
          loadSummary.Numbers = new List<string>();
          loadSummary.Loads = new List<int>();
          loadSummary.Subtotal = 0;
          loadSummary.TotalAmps = 0;
          powerStation.Lots.ForEach(l =>
          {
            loadSummary.Loads.Add(l.LoadVa);
            loadSummary.Numbers.Add(l.Number);
            loadSummary.Subtotal += l.LoadVa;
            loadSummary.Voltage = l.Voltage;
            loadSummaries.ForEach(ls =>
            {
              if (ls.Numbers[0] == l.Number)
              {
                loadSummary.TotalAmps += ls.TotalAmps;
              }
            });
          });
          if (loadSummary.TotalAmps < 200)
          {
            loadSummary.RecommendedService = 200;
          }
          if (loadSummary.TotalAmps < 100)
          {
            loadSummary.RecommendedService = 100;
          }
          if (loadSummary.TotalAmps < 60)
          {
            loadSummary.RecommendedService = 60;
          }
          loadSummaries.Add(loadSummary);
        }
      }
      using (
        DocumentLock docLock =
          Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
      )
      {
        Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState = Autodesk
          .AutoCAD
          .Windows
          .Window
          .State
          .Maximized;
        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Window.Focus();
        using (var tr = db.TransactionManager.StartTransaction())
        {
          BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
          BlockTableRecord btr = (BlockTableRecord)
            tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
          Autodesk.AutoCAD.EditorInput.PromptPointOptions pointOption =
            new Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select an origin point: ");
          Autodesk.AutoCAD.EditorInput.PromptPointResult pointResult = ed.GetPoint(pointOption);
          Point3d startPoint = pointResult.Value;

          var textStyleTable = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);
          var textStyleId = textStyleTable["ArialMT"];
          loadSummaries.ForEach(ls =>
          {
            Table tb = new Table();
            tb.TableStyle = db.Tablestyle;
            tb.Position = startPoint;
            int tableRows =
              ls.Numbers.Count == 1 ? 4
              : ls.Numbers.Count == 2 ? 6
              : ls.Numbers.Count + 4;
            int tableColumns = 2;
            tb.SetSize(tableRows, tableColumns);
            tb.SetRowHeight(0.25);
            tb.Columns[0].Width = 4;
            tb.Columns[1].Width = 1;
            tb.Layer = "E-TEXT";
            for (int i = 0; i < tableRows; i++)
            {
              for (int j = 0; j < tableColumns; j++)
              {
                tb.Cells[i, j].TextStyleId = textStyleId;
                tb.Cells[i, j].TextHeight = (0.0938);
                tb.Cells[i, j].Alignment = CellAlignment.MiddleCenter;
              }
            }
            tb.Cells[0, 0].TextString = "LOAD SUMMARY LOT ";
            for (int i = 0; i < ls.Numbers.Count; i++)
            {
              if (i < ls.Numbers.Count - 2)
              {
                tb.Cells[0, 0].TextString += ls.Numbers[i] + ", ";
              }
              else if (i == ls.Numbers.Count - 2)
              {
                tb.Cells[0, 0].TextString += ls.Numbers[i] + "&";
              }
              else if (i < ls.Numbers.Count - 1)
              {
                tb.Cells[0, 0].TextString += ls.Numbers[i];
              }
              tb.Cells[i + 1, 0].TextString =
                $"LOT {ls.Numbers[i]} LARGEST SINGLE UTILIZATION EQUIPMENT IN EACH UNIT PER CEC710.15(A) [SPLIT SYSTEM HVAC]";
              tb.Cells[i + 1, 1].TextString = $"{ls.Loads[i]}VA";
            }
            if (ls.Numbers.Count > 1)
            {
              int index = ls.Numbers.Count + 1;
              tb.Cells[index, 0].TextString = "SUBTOTAL FOR MODELS";
              tb.Cells[index, 1].TextString = $"{ls.Subtotal.ToString()}A";
              tb.Cells[index + 1, 0].TextString = "AMPS @ " + ls.Voltage;
              tb.Cells[index + 1, 1].TextString = $"{ls.TotalAmps}A";
              tb.Cells[index + 2, 0].TextString = "RECOMMENDED SERVICE AMPS @ " + ls.Voltage;
              tb.Cells[index + 2, 1].TextString = $"{ls.RecommendedService}A";
            }
            else
            {
              int index = ls.Numbers.Count + 1;
              tb.Cells[index, 0].TextString = "AMPS @ " + ls.Voltage;
              tb.Cells[index, 1].TextString = $"{ls.TotalAmps}A";
              tb.Cells[index + 1, 0].TextString = "RECOMMENDED SERVICE AMPS @ " + ls.Voltage;
              tb.Cells[index + 1, 1].TextString = $"{ls.RecommendedService}A";
            }
            btr.AppendEntity(tb);
            tr.AddNewlyCreatedDBObject(tb, true);

            startPoint = new Point3d(startPoint.X + 5.25, startPoint.Y, 0);
          });

          tr.Commit();
        }
      }
    }
  }
}
