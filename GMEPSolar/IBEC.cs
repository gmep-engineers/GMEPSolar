using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

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
        string year = "";
        string client = "";
        for (int i = 0; i < directories.Length; i++)
        {
          if (directories[i].Contains(" Jobs"))
          {
            year = directories[i].Replace(" Jobs", "");
            client = directories[i - 1];
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
        GmepDatabase.CreateLots(powerStationUserControl.PowerStation.Lots, ProjectId);
        GmepDatabase.UpdateLots(powerStationUserControl.PowerStation.Lots);
        GmepDatabase.DeleteLots(powerStationUserControl.PowerStation.Lots);
      }
    }
  }
}
