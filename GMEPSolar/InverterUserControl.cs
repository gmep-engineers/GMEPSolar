using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GMEPSolar
{
  public partial class InverterUserControl : UserControl
  {
    private InverterForm form;
    private bool is2P;

    public InverterUserControl(InverterForm form, bool is2P)
    {
      this.form = form;
      this.is2P = is2P;
      InitializeComponent();
      SetFirstUserControlToMaster();
      Set2POr3P();
    }

    private void Set2POr3P()
    {
      if (is2P)
      {
        RADIO_2P.Checked = true;
      }
      else
      {
        RADIO_3P.Checked = true;
      }
    }

    private void SetFirstUserControlToMaster()
    {
      if (this.form.GetInverterTabs().TabPages.Count == 0)
      {
        RADIO_MASTER.Checked = true;
      }
    }

    private void NEW_INVERTER_BUTTON_Click(object sender, EventArgs e)
    {
      var is2P = RADIO_2P.Checked;
      this.form.CreateNewPanelTab("Inverter", is2P);
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
      if (this.form.DoesDCSolarDataHaveThisID(tabID))
      {
        this.form.FillDCSolarFormWithExistingData(tabID, form);
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