using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace GMEPSolar
{
  public partial class PowerStationUserControl : UserControl
  {
    public PowerStation PowerStation;

    public PowerStationUserControl(PowerStation powerStation)
    {
      InitializeComponent();
      PowerStation = powerStation;
      SystemComboBox.SelectedIndex = powerStation.KwId;
      NumBattsComboBox.SelectedIndex = powerStation.NumBatts;
      if (powerStation.Lots.Count > 0)
      {
        LotDataGridView.Rows.AddCopies(0, powerStation.Lots.Count);
      }
      for (int i = 0; i < powerStation.Lots.Count; i++)
      {
        LotDataGridView.Rows[i].Cells[0].Value = powerStation.Lots[i].Number;
        LotDataGridView.Rows[i].Cells[1].Value = powerStation.Lots[i].Voltage;
        LotDataGridView.Rows[i].Cells[2].Value = powerStation.Lots[i].Amp;
        LotDataGridView.Rows[i].Cells[3].Value = powerStation.Lots[i].Kaic;
        LotDataGridView.Rows[i].Cells[4].Value = powerStation.Lots[i].LoadVa;

        // the last cell is hidden and reserved for Lot ID
        LotDataGridView.Rows[i].Cells[LotDataGridView.Rows[i].Cells.Count - 1].Value = powerStation
          .Lots[i]
          .Id;
      }
    }

    public int GetSafeInt(string str)
    {
      if (Int32.TryParse(str, out int i))
      {
        return i;
      }
      return 0;
    }

    public void RemoveButton_Click(object sender, EventArgs e)
    {
      PowerStation.Action = UpdateAction.Delete;
      this.Visible = false;
    }

    private List<Lot> GetActiveLots()
    {
      return PowerStation.Lots.FindAll(l => l.Action != UpdateAction.Delete);
    }

    private void MakeSingleLine45kw(Point3d startingPoint)
    {
      List<Lot> ActiveLots = GetActiveLots();
      Point3d panelPoint = new Point3d(
        startingPoint.X + 4.0365,
        startingPoint.Y
          - 6.3512
          - (PowerStation.NumBatts > 4 ? 0.7026 * (PowerStation.NumBatts - 4) : 0),
        0
      );
      if (ActiveLots.Count > 1)
      {
        CadObjectFunctions.MakeBlock(panelPoint, "IBEC 200A PANEL");
        Point3d upperLeft = new Point3d(panelPoint.X - 0.35, panelPoint.Y, 0);
        Point3d upperRight = new Point3d(panelPoint.X + 0.35, panelPoint.Y, 0);
        Point3d lowerLeft = new Point3d(
          panelPoint.X - 0.35,
          panelPoint.Y - 1.1282 - ((ActiveLots.Count - 2) * 0.2934),
          0
        );
        Point3d lowerRight = new Point3d(
          panelPoint.X + 0.35,
          panelPoint.Y - 1.1282 - ((ActiveLots.Count - 2) * 0.2934),
          0
        );
        CadObjectFunctions.MakeLine(upperLeft, upperRight, "E-SYM1");
        CadObjectFunctions.MakeLine(upperLeft, lowerLeft, "E-SYM1");
        CadObjectFunctions.MakeLine(lowerLeft, lowerRight, "E-SYM1");
        CadObjectFunctions.MakeLine(upperRight, lowerRight, "E-SYM1");
        Point3d feederStart = new Point3d(
          panelPoint.X,
          panelPoint.Y
            + 1.7885
            + (PowerStation.NumBatts > 5 ? 0.7026 * (PowerStation.NumBatts - 5) : 0),
          0
        );
        CadObjectFunctions.MakeLine(feederStart, panelPoint);
        CadObjectFunctions.MakeText(
          new Point3d(panelPoint.X - 0.0625, panelPoint.Y + 0.5, 0),
          "3WG200",
          true
        );
      }
      else
      {
        Point3d point1 = new Point3d(startingPoint.X + 4.0355, startingPoint.Y - 5.2653, 0);
        Point3d point2 = new Point3d(
          point1.X,
          point1.Y - 1.7 - (PowerStation.NumBatts > 4 ? 0.7026 * (PowerStation.NumBatts - 4) : 0),
          0
        );
        Point3d point3 = new Point3d(point2.X + 4.0490, point2.Y, 0);
        Point3d point4 = new Point3d(startingPoint.X + 8.0855, startingPoint.Y - 4.2534, 0);
        CadObjectFunctions.MakeLine(point1, point2);
        CadObjectFunctions.MakeLine(point2, point3);
        CadObjectFunctions.MakeLine(point3, point4);
      }

      List<Point3d> panelBreakerPoints = new List<Point3d>();
      List<Point3d> meterComboPoints = new List<Point3d>();
      double panelBreakerPointYOffset = 0;
      double meterComboPointXOffset = 0;
      for (int i = 0; i < ActiveLots.Count; i++)
      {
        Point3d meterComboPoint = new Point3d(
          startingPoint.X + 8.0855 + meterComboPointXOffset,
          startingPoint.Y - 4.2534,
          0
        );
        meterComboPoints.Add(meterComboPoint);
        if (ActiveLots.Count > 1)
        {
          Point3d panelBreakerPoint = new Point3d(
            panelPoint.X + 0.1416,
            panelPoint.Y - 0.6151 + panelBreakerPointYOffset,
            0
          );
          panelBreakerPoints.Add(panelBreakerPoint);
          Point3d intersectionPoint = new Point3d(meterComboPoint.X, panelBreakerPoint.Y, 0);
          CadObjectFunctions.MakeLine(panelBreakerPoint, intersectionPoint);
          CadObjectFunctions.MakeLine(intersectionPoint, meterComboPoint);
        }
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.928, 0),
          "(E)METER COMBINATION AT LOT " + ActiveLots[i].Number
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.7373, 0),
          $"{ActiveLots[i].Voltage}, {ActiveLots[i].Amp}A"
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.5823, 0),
          ActiveLots[i].Kaic + " KAIC"
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 1.3979, meterComboPoint.Y + 2.4252, 0),
          $"{ActiveLots[i].Amp}A/" + (ActiveLots[i].Voltage.Contains("3") ? "3P" : "2P")
        );
        if (i == ActiveLots.Count - 1)
        {
          CadObjectFunctions.MakeBlock(
            new Point3d(meterComboPoint.X + 1.1472, meterComboPoint.Y + 1.3900, 0),
            "IBEC 60A-2P TEMP POWER BREAKER"
          );
          if (ActiveLots.Count > 1)
          {
            CadObjectFunctions.MakeText(
              new Point3d(meterComboPoint.X + 2.6945, meterComboPoint.Y + 0.2734, 0),
              "(TYP.)"
            );
          }
        }

        meterComboPointXOffset += 3.0941;
        panelBreakerPointYOffset -= 0.2934;
      }

      List<Point3d> batteryPoints = new List<Point3d>();
      double batteryPointYOffset = 0;
      for (int i = 0; i < PowerStation.NumBatts; i++)
      {
        batteryPoints.Add(
          new Point3d(startingPoint.X + 6.7770, startingPoint.Y - 4.0957 + batteryPointYOffset, 0)
        );
        if (i < PowerStation.NumBatts - 1)
        {
          Point3d line1Start = new Point3d(
            startingPoint.X + 6.7770,
            startingPoint.Y - 4.0957 - 0.4748 + batteryPointYOffset,
            0
          );
          Point3d line1End = new Point3d(line1Start.X, line1Start.Y - 0.2278, 0);
          CadObjectFunctions.MakeLine(line1Start, line1End);

          Point3d line2Start = new Point3d(line1Start.X + 0.3514, line1Start.Y, 0);
          Point3d line2End = new Point3d(line2Start.X, line2Start.Y - 0.2278, 0);
          CadObjectFunctions.MakeLine(line2Start, line2End);
        }
        batteryPointYOffset -= 0.7026;
      }

      CadObjectFunctions.MakeBlock(startingPoint, "IBEC 45KW POWER STATION");
      panelBreakerPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC 60A-2P PANEL BREAKER"));
      meterComboPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC METER COMBO EXISTING"));
      batteryPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC 30KWH BATTERY BANK"));
    }

    private void MakeSingleLine30kw(Point3d startingPoint)
    {
      List<Lot> ActiveLots = GetActiveLots();
      Point3d panelPoint = new Point3d(
        startingPoint.X + 2.3873,
        startingPoint.Y
          - 6.3512
          - (PowerStation.NumBatts > 4 ? 0.7026 * (PowerStation.NumBatts - 4) : 0),
        0
      );
      if (ActiveLots.Count > 1)
      {
        CadObjectFunctions.MakeBlock(panelPoint, "IBEC 100A PANEL");
        Point3d upperLeft = new Point3d(panelPoint.X - 0.35, panelPoint.Y, 0);
        Point3d upperRight = new Point3d(panelPoint.X + 0.35, panelPoint.Y, 0);
        Point3d lowerLeft = new Point3d(
          panelPoint.X - 0.35,
          panelPoint.Y - 1.1282 - ((ActiveLots.Count - 2) * 0.2934),
          0
        );
        Point3d lowerRight = new Point3d(
          panelPoint.X + 0.35,
          panelPoint.Y - 1.1282 - ((ActiveLots.Count - 2) * 0.2934),
          0
        );
        CadObjectFunctions.MakeLine(upperLeft, upperRight, "E-SYM1");
        CadObjectFunctions.MakeLine(upperLeft, lowerLeft, "E-SYM1");
        CadObjectFunctions.MakeLine(lowerLeft, lowerRight, "E-SYM1");
        CadObjectFunctions.MakeLine(upperRight, lowerRight, "E-SYM1");
        Point3d feederStart = new Point3d(
          panelPoint.X,
          panelPoint.Y
            + 1.7885
            + (PowerStation.NumBatts > 5 ? 0.7026 * (PowerStation.NumBatts - 5) : 0),
          0
        );
        CadObjectFunctions.MakeLine(feederStart, panelPoint);
        CadObjectFunctions.MakeText(
          new Point3d(panelPoint.X - 0.0625, panelPoint.Y + 0.5, 0),
          "3WG100",
          true
        );
      }
      else
      {
        Point3d point1 = new Point3d(startingPoint.X + 2.3883, startingPoint.Y - 5.2653, 0);
        Point3d point2 = new Point3d(
          point1.X,
          point1.Y - 1.7 - (PowerStation.NumBatts > 4 ? 0.7026 * (PowerStation.NumBatts - 4) : 0),
          0
        );
        Point3d point3 = new Point3d(point2.X + 4.0491, point2.Y, 0);
        Point3d point4 = new Point3d(startingPoint.X + 6.4373, startingPoint.Y - 4.2534, 0);
        CadObjectFunctions.MakeLine(point1, point2);
        CadObjectFunctions.MakeLine(point2, point3);
        CadObjectFunctions.MakeLine(point3, point4);
      }
      List<Point3d> meterComboPoints = new List<Point3d>();
      List<Point3d> panelBreakerPoints = new List<Point3d>();
      double meterComboPointXOffset = 0;
      double panelBreakerPointYOffset = 0;

      for (int i = 0; i < ActiveLots.Count; i++)
      {
        Point3d meterComboPoint = new Point3d(
          startingPoint.X + 6.4373 + meterComboPointXOffset,
          startingPoint.Y - 4.2534,
          0
        );
        meterComboPoints.Add(meterComboPoint);
        if (ActiveLots.Count > 1)
        {
          Point3d panelBreakerPoint = new Point3d(
            panelPoint.X + 0.1416,
            panelPoint.Y - 0.6151 + panelBreakerPointYOffset,
            0
          );
          panelBreakerPoints.Add(panelBreakerPoint);
          Point3d intersectionPoint = new Point3d(meterComboPoint.X, panelBreakerPoint.Y, 0);
          CadObjectFunctions.MakeLine(panelBreakerPoint, intersectionPoint);
          CadObjectFunctions.MakeLine(intersectionPoint, meterComboPoint);
        }
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.928, 0),
          "(E)METER COMBINATION AT LOT " + ActiveLots[i].Number
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.7373, 0),
          $"{ActiveLots[i].Voltage}, {ActiveLots[i].Amp}A"
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 0.2076, meterComboPoint.Y + 4.5823, 0),
          ActiveLots[i].Kaic + " KAIC"
        );
        CadObjectFunctions.MakeText(
          new Point3d(meterComboPoint.X + 1.3979, meterComboPoint.Y + 2.4252, 0),
          $"{ActiveLots[i].Amp}A/" + (ActiveLots[i].Voltage.Contains("4W") ? "3P" : "2P")
        );
        if (i == ActiveLots.Count - 1)
        {
          CadObjectFunctions.MakeBlock(
            new Point3d(meterComboPoint.X + 1.1472, meterComboPoint.Y + 1.3900, 0),
            "IBEC 60A-2P TEMP POWER BREAKER"
          );
          if (ActiveLots.Count > 1)
          {
            CadObjectFunctions.MakeText(
              new Point3d(meterComboPoint.X + 2.6945, meterComboPoint.Y + 0.2734, 0),
              "(TYP.)"
            );
          }
        }
        meterComboPointXOffset += 3.0941;
      }
      List<Point3d> batteryPoints = new List<Point3d>();
      double batteryPointYOffset = 0;
      for (int i = 0; i < PowerStation.NumBatts; i++)
      {
        batteryPoints.Add(
          new Point3d(startingPoint.X + 5.1288, startingPoint.Y - 4.0957 + batteryPointYOffset, 0)
        );
        if (i < PowerStation.NumBatts - 1)
        {
          Point3d line1Start = new Point3d(
            startingPoint.X + 5.1288,
            startingPoint.Y - 4.0957 - 0.4748 + batteryPointYOffset,
            0
          );
          Point3d line1End = new Point3d(line1Start.X, line1Start.Y - 0.2278, 0);
          CadObjectFunctions.MakeLine(line1Start, line1End);

          Point3d line2Start = new Point3d(line1Start.X + 0.3514, line1Start.Y, 0);
          Point3d line2End = new Point3d(line2Start.X, line2Start.Y - 0.2278, 0);
          CadObjectFunctions.MakeLine(line2Start, line2End);
        }
        batteryPointYOffset -= 0.7026;
      }
      CadObjectFunctions.MakeBlock(startingPoint, "IBEC 30KW POWER STATION");
      panelBreakerPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC 60A-2P PANEL BREAKER"));
      meterComboPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC METER COMBO EXISTING"));
      batteryPoints.ForEach(p => CadObjectFunctions.MakeBlock(p, "IBEC 30KWH BATTERY BANK"));
    }

    public void GenerateButton_Click(object sender, EventArgs args)
    {
      Save();
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      Editor ed = doc.Editor;
      var currentView = ed.GetCurrentView();
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
        Point3d startingPoint;

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
          var promptOptions = new PromptPointOptions("\nSelect upper left point:");
          var promptResult = ed.GetPoint(promptOptions);
          currentView = ed.GetCurrentView();
          if (promptResult.Status == PromptStatus.OK)
            startingPoint = promptResult.Value;
          else
          {
            return;
          }
        }
        if (PowerStation.KwId == 1)
        {
          MakeSingleLine30kw(startingPoint);
        }
        if (PowerStation.KwId == 2)
        {
          MakeSingleLine45kw(startingPoint);
        }
      }
      ed.SetCurrentView(currentView);
    }

    public void DeleteLot_Click(object sender, EventArgs e)
    {
      if (sender is DataGridView dataGridView)
      {
        string id =
          dataGridView.SelectedRows[0].Cells[LotDataGridView.Rows[0].Cells.Count - 1].Value
          as string;
        Lot lot = PowerStation.Lots.Find(l =>
          l.Id
          == dataGridView.SelectedRows[0].Cells[LotDataGridView.Rows[0].Cells.Count - 1].Value
            as string
        );
        if (lot != null)
        {
          lot.Action = UpdateAction.Delete;
          dataGridView.SelectedRows[0].Visible = false;
        }
      }
    }

    public void Save()
    {
      LotDataGridView.EndEdit();
      PowerStation.KwId = SystemComboBox.SelectedIndex;
      PowerStation.NumBatts = NumBattsComboBox.SelectedIndex;
      for (int i = 0; i < LotDataGridView.Rows.Count - 1; i++)
      {
        Lot lot = PowerStation.Lots.Find(l =>
          l.Id
          == LotDataGridView.Rows[i].Cells[LotDataGridView.Rows[i].Cells.Count - 1].Value as string
        );
        if (lot != null && String.IsNullOrEmpty(LotDataGridView.Rows[i].Cells[0].Value as string))
        {
          lot.Action = UpdateAction.Delete;
          LotDataGridView.Rows[i].Visible = false;
        }
        if (lot != null && lot.Action != UpdateAction.Delete)
        {
          // update previously saved lot
          lot.Number = LotDataGridView.Rows[i].Cells[0].Value as string;
          lot.Voltage = LotDataGridView.Rows[i].Cells[1].Value as string;
          lot.Amp = LotDataGridView.Rows[i].Cells[2].Value as string;
          lot.Kaic = LotDataGridView.Rows[i].Cells[3].Value as string;
          lot.LoadVa = GetSafeInt(LotDataGridView.Rows[i].Cells[4].Value as string);
        }
        else if (lot == null)
        {
          // create new lot
          string id = Guid.NewGuid().ToString();
          string number = LotDataGridView.Rows[i].Cells[0].Value as string;
          string voltage = LotDataGridView.Rows[i].Cells[1].Value as string;
          string amp = LotDataGridView.Rows[i].Cells[2].Value as string;
          string kaic = LotDataGridView.Rows[i].Cells[3].Value as string;
          int loadVa = GetSafeInt(LotDataGridView.Rows[i].Cells[4].Value as string);
          lot = new Lot(
            id,
            number,
            kaic,
            loadVa,
            voltage,
            amp,
            PowerStation.Id,
            UpdateAction.Create
          );
          LotDataGridView.Rows[i].Cells[LotDataGridView.Rows[i].Cells.Count - 1].Value = id;
          PowerStation.Lots.Add(lot);
        }
      }
    }
  }
}
