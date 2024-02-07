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
    }
}
