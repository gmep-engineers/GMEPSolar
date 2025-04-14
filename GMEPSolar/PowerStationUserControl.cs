using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
