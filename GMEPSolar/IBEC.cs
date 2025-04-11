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
    List<PowerStationUserControl> PowerStationUserControls;

    public IBEC()
    {
      GmepDatabase = new GmepDatabase();
      string projectId = GmepDatabase.GetProjectId(CadObjectFunctions.GetProjectNameFromFileName());
      PowerStations = GmepDatabase.ReadPowerStations(projectId);
      Lots = GmepDatabase.ReadLots(projectId);
      PowerStationUserControls = new List<PowerStationUserControl>();
      foreach (PowerStation powerStation in PowerStations)
      {
        powerStation.Lots = Lots.FindAll(l => l.PowerStationId == powerStation.Id);
      }
      InitializeComponent();
    }

    private void PopulatePowerStations() { }
  }
}
