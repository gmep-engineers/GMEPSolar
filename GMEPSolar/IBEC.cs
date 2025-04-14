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
  public partial class IBEC : Form
  {
    private List<PowerStation> PowerStations;
    private List<Lot> Lots;
    private GmepDatabase GmepDatabase;
    private string ProjectId;
    List<PowerStationUserControl> PowerStationUserControls;

    public IBEC()
    {
      GmepDatabase = new GmepDatabase();
      ProjectId = GmepDatabase.GetProjectId(CadObjectFunctions.GetProjectNameFromFileName());
      PowerStations = GmepDatabase.ReadPowerStations(ProjectId);
      Lots = GmepDatabase.ReadLots(ProjectId);
      PowerStationUserControls = new List<PowerStationUserControl>();
      foreach (PowerStation powerStation in PowerStations)
      {
        powerStation.Lots = Lots.FindAll(l => l.PowerStationId == powerStation.Id);
        PowerStationUserControl powerStationUserControl = new PowerStationUserControl(powerStation);
        PowerStationFlowLayoutPanel.Controls.Add(powerStationUserControl);
      }
      InitializeComponent();
    }

    private void PopulatePowerStations() { }

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
        GmepDatabase.CreateLots(powerStationUserControl.Lots, ProjectId);
        GmepDatabase.UpdateLots(powerStationUserControl.Lots);
        GmepDatabase.DeleteLots(powerStationUserControl.Lots);
      }
    }
  }
}
