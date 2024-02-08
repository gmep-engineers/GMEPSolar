using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GMEPSolar
{
    public partial class InverterForm : Form
    {
        Dictionary<int, Dictionary<string, Dictionary<string, object>>> DCSolarData =
            new Dictionary<int, Dictionary<string, Dictionary<string, object>>>();
        int initialID;

        public InverterForm()
        {
            initialID = 1;
            InitializeComponent();
            CreateNewPanelTab("Inverter");
        }

        public InverterUserControl CreateNewPanelTab(string tabName)
        {
            InverterUserControl inverterUserControl = new InverterUserControl(this);
            this.Controls.Add(inverterUserControl);

            var numberOfTabs = INVERTER_TABS.TabPages.Count + 1;
            var tabNameWithNumber = tabName + " " + numberOfTabs;

            TabPage newTabPage = new TabPage(tabNameWithNumber);
            newTabPage.Tag = initialID;
            initialID++;

            INVERTER_TABS.TabPages.Add(newTabPage);
            INVERTER_TABS.SelectedTab = newTabPage;

            AddUserControlToTab(inverterUserControl, newTabPage);

            return inverterUserControl;
        }

        public void AddUserControlToTab(UserControl control, TabPage tabPage)
        {
            control.Location = new Point(0, 0);
            control.Dock = DockStyle.Fill;

            tabPage.Controls.Add(control);
        }

        public void PutObjectInJson(object data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            System.IO.File.WriteAllText(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ObjectData.json",
                json
            );
        }

        internal TabControl GetInverterTabs()
        {
            return INVERTER_TABS;
        }

        internal void SaveDCData(
            Dictionary<int, Dictionary<string, Dictionary<string, object>>> newDataObject
        )
        {
            DCSolarData = newDataObject;
            PutObjectInJson(DCSolarData);
        }

        internal Dictionary<int, Dictionary<string, Dictionary<string, object>>> GetCurrentDCData()
        {
            return DCSolarData;
        }

        internal void RemoveFromDCData(int tabID)
        {
            DCSolarData.Remove(tabID);
            PutObjectInJson(DCSolarData);
        }

        internal bool DoesDataExist(int tabID)
        {
            return DCSolarData.ContainsKey(tabID);
        }

        internal void PopulateFormWithData(int tabID, DC_SOLAR_INPUT form)
        {
            var data = DCSolarData[tabID];
            foreach (var mpptData in data)
            {
                var mpptName = mpptData.Key.ToString();
                if (mpptName == "MPPT1")
                {
                    var mppt1Data = mpptData.Value;
                    var mppt1Enabled = Convert.ToBoolean(mppt1Data["Enabled"]);
                    var mppt1Regular = Convert.ToBoolean(mppt1Data["Regular"]);
                    var mppt1Parallel = Convert.ToBoolean(mppt1Data["Parallel"]);
                    var mppt1Input = mppt1Data["Input"];
                    form.UpdateValues(
                        "MPPT1",
                        mppt1Enabled,
                        mppt1Regular,
                        mppt1Parallel,
                        mppt1Input
                    );
                }
                else if (mpptName == "MPPT2")
                {
                    var mppt2Data = mpptData.Value;
                    var mppt2Enabled = Convert.ToBoolean(mppt2Data["Enabled"]);
                    var mppt2Regular = Convert.ToBoolean(mppt2Data["Regular"]);
                    var mppt2Parallel = Convert.ToBoolean(mppt2Data["Parallel"]);
                    var mppt2Input = mppt2Data["Input"];
                    form.UpdateValues(
                        "MPPT2",
                        mppt2Enabled,
                        mppt2Regular,
                        mppt2Parallel,
                        mppt2Input
                    );
                }
                else if (mpptName == "MPPT3")
                {
                    var mppt3Data = mpptData.Value;
                    var mppt3Enabled = Convert.ToBoolean(mppt3Data["Enabled"]);
                    var mppt3Regular = Convert.ToBoolean(mppt3Data["Regular"]);
                    var mppt3Parallel = Convert.ToBoolean(mppt3Data["Parallel"]);
                    var mppt3Input = mppt3Data["Input"];
                    form.UpdateValues(
                        "MPPT3",
                        mppt3Enabled,
                        mppt3Regular,
                        mppt3Parallel,
                        mppt3Input
                    );
                }
                else if (mpptName == "MPPT4")
                {
                    var mppt4Data = mpptData.Value;
                    var mppt4Enabled = Convert.ToBoolean(mppt4Data["Enabled"]);
                    var mppt4Regular = Convert.ToBoolean(mppt4Data["Regular"]);
                    var mppt4Parallel = Convert.ToBoolean(mppt4Data["Parallel"]);
                    var mppt4Input = mppt4Data["Input"];
                    form.UpdateValues(
                        "MPPT4",
                        mppt4Enabled,
                        mppt4Regular,
                        mppt4Parallel,
                        mppt4Input
                    );
                }
            }
        }

        private void CREATE_BUTTON_Click(object sender, EventArgs e)
        {
            var inverterFormData = GetInverterFormData();
            PutObjectInJson(inverterFormData);
        }

        private object GetInverterFormData()
        {
            var inverterFormData = new Dictionary<string, object>();
            var inverterEncapsulation = new List<Dictionary<string, object>>();
            var inverterEncapsulationExtra = new Dictionary<string, object>();
            var inverterTabs = INVERTER_TABS.TabPages;

            foreach (TabPage tab in inverterTabs)
            {
                var tabID = (int)tab.Tag;
                if (DCSolarData.ContainsKey(tabID))
                {
                    inverterFormData.Add("DCSolarData", DCSolarData[tabID]);
                }
                var control = tab.Controls[0] as InverterUserControl;
                var controlData = control.GetInverterFormData();
                inverterFormData.Add("is2P", controlData["is2P"]);
                inverterFormData.Add("isMaster", controlData["isMaster"]);
                inverterEncapsulation.Add(inverterFormData);
                inverterFormData = new Dictionary<string, object>();
            }

            var configuration = GetConfigurationOfInverters();

            inverterEncapsulationExtra.Add("InverterData", inverterEncapsulation);
            inverterEncapsulationExtra.Add("Configuration", configuration);
            return inverterEncapsulationExtra;
        }

        private object GetConfigurationOfInverters()
        {
            if (EMPTY_RADIO.Checked)
            {
                return "EMPTY";
            }
            else if (GRID_RADIO.Checked)
            {
                return "GRID";
            }
            else if (LOAD_RADIO.Checked)
            {
                return "LOAD";
            }
            else if (GRID_LOAD_RADIO.Checked)
            {
                return "GRID_LOAD";
            }
            else
            {
                return "EMPTY";
            }
        }
    }
}
