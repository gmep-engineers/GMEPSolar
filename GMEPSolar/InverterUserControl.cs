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
    public partial class InverterUserControl : UserControl
    {
        InverterForm form;

        public InverterUserControl(InverterForm form)
        {
            this.form = form;
            InitializeComponent();
        }

        private void NEW_INVERTER_BUTTON_Click(object sender, EventArgs e)
        {
            this.form.CreateNewPanelTab("Inverter");
        }

        private void REMOVE_BUTTON_Click(object sender, EventArgs e)
        {
            var tabs = this.form.GetInverterTabs();
            if (tabs.TabPages.Count == 1)
            {
                MessageBox.Show("You cannot remove the last tab.");
                return;
            }

            var tab = this.Parent as TabPage;
            var tabControl = tab.Parent as TabControl;
            tabControl.TabPages.Remove(tab);

            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                tabControl.TabPages[i].Text = "Inverter " + (i + 1);
            }

            var tabID = (int)tab.Tag;
            this.form.RemoveFromDCData(tabID);
        }

        private void DC_SOLAR_BUTTON_Click(object sender, EventArgs e)
        {
            var tabID = (int)(this.Parent as TabPage).Tag;
            DC_SOLAR_INPUT form = new DC_SOLAR_INPUT(this.form, tabID);
            if (this.form.DoesDataExist(tabID))
            {
                this.form.PopulateFormWithData(tabID, form);
            }
            form.AlterComponents();
            form.Show();
        }

        internal Dictionary<string, object> GetInverterFormData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["is2P"] = RADIO_2P.Checked;
            data["isMaster"] = RADIO_MASTER.Checked;
            return data;
        }
    }
}
