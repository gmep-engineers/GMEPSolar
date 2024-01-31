using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GMEPSolar
{
    public partial class DC_SOLAR_INPUT : Form
    {
        public DC_SOLAR_INPUT()
        {
            InitializeComponent();
            NUMBER_ALL_MODULES_TEXTBOX.KeyDown += NUMBER_ALL_MODULES_TEXTBOX_KeyDown;
        }

        private void ENABLE_ALL_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_CHECKBOX.Checked = true;
            MPPT2_CHECKBOX.Checked = true;
            MPPT3_CHECKBOX.Checked = true;
            MPPT4_CHECKBOX.Checked = true;
        }

        private void ALL_REGULAR_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_RADIO_REGULAR.Checked = true;
            MPPT2_RADIO_REGULAR.Checked = true;
            MPPT3_RADIO_REGULAR.Checked = true;
            MPPT4_RADIO_REGULAR.Checked = true;
        }

        private void ALL_PARALLEL_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_RADIO_PARALLEL.Checked = true;
            MPPT2_RADIO_PARALLEL.Checked = true;
            MPPT3_RADIO_PARALLEL.Checked = true;
            MPPT4_RADIO_PARALLEL.Checked = true;
        }

        private void SET_ALL_MODULES_BUTTON_Click(object sender, EventArgs e)
        {
            MPPT1_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT2_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT3_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
            MPPT4_INPUT.Text = NUMBER_ALL_MODULES_TEXTBOX.Text;
        }

        private void NUMBER_ALL_MODULES_TEXTBOX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SET_ALL_MODULES_BUTTON_Click(sender, e);
            }
        }

        private void CREATE_BUTTON_Click(object sender, EventArgs e)
        {
            Close();
            var data = GetFormData();
        }

        private object GetFormData()
        {
            var data = new
            {
                MPPT1 = new
                {
                    Enabled = MPPT1_CHECKBOX.Checked,
                    Regular = MPPT1_RADIO_REGULAR.Checked,
                    Parallel = MPPT1_RADIO_PARALLEL.Checked,
                    Input = MPPT1_INPUT.Text
                },
                MPPT2 = new
                {
                    Enabled = MPPT2_CHECKBOX.Checked,
                    Regular = MPPT2_RADIO_REGULAR.Checked,
                    Parallel = MPPT2_RADIO_PARALLEL.Checked,
                    Input = MPPT2_INPUT.Text
                },
                MPPT3 = new
                {
                    Enabled = MPPT3_CHECKBOX.Checked,
                    Regular = MPPT3_RADIO_REGULAR.Checked,
                    Parallel = MPPT3_RADIO_PARALLEL.Checked,
                    Input = MPPT3_INPUT.Text
                },
                MPPT4 = new
                {
                    Enabled = MPPT4_CHECKBOX.Checked,
                    Regular = MPPT4_RADIO_REGULAR.Checked,
                    Parallel = MPPT4_RADIO_PARALLEL.Checked,
                    Input = MPPT4_INPUT.Text
                }
            };
            return data;
        }
    }
}
