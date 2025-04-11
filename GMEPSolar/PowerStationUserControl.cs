using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPSolar
{
  public partial class PowerStationUserControl : UserControl
  {
    private PowerStation PowerStation;

    public PowerStationUserControl(PowerStation powerStation)
    {
      InitializeComponent();
      PowerStation = powerStation;
      SystemComboBox.SelectedIndex = powerStation.KwId;
      NumBattsComboBox.SelectedIndex = powerStation.NumBatts;
      LotDataGridView.Rows.AddCopies(0, powerStation.Lots.Count);
      for (int i = 0; i < powerStation.Lots.Count; i++)
      {
        LotDataGridView.Rows[i].Cells[0].Value = powerStation.Lots[i].Number;
        LotDataGridView.Rows[i].Cells[1].Value = powerStation.Lots[i].Voltage; // HERE update datatype everywher to actual value, not ID
        LotDataGridView.Rows[i].Cells[2].Value = powerStation.Lots[i].Amp;
        LotDataGridView.Rows[i].Cells[3].Value = powerStation.Lots[i].Kaic;
        LotDataGridView.Rows[i].Cells[4].Value = powerStation.Lots[i].LoadVa;

        // the last cell is hidden and reserved for Lot ID
        LotDataGridView.Rows[i].Cells[LotDataGridView.Rows[i].Cells.Count - 1].Value = powerStation
          .Lots[i]
          .Id;
      }
    }

    public void Save()
    {
      PowerStation.KwId = SystemComboBox.SelectedIndex;
      PowerStation.NumBatts = NumBattsComboBox.SelectedIndex;
      for (int i = 0; i < LotDataGridView.Rows.Count; i++)
      {
        Lot lot = PowerStation.Lots.Find(l =>
          l.Id
          == LotDataGridView.Rows[i].Cells[LotDataGridView.Rows[i].Cells.Count - 1].Value as string
        );
        if (lot != null)
        {
          lot.Number = LotDataGridView.Rows[i].Cells[0].Value as string;
          // HERE continue assigning values from data grid to lot
        }
      }
    }
  }
}
