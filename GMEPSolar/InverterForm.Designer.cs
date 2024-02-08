namespace GMEPSolar
{
    partial class InverterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.INVERTER_TABS = new System.Windows.Forms.TabControl();
            this.CONFIGURATION_GROUP = new System.Windows.Forms.GroupBox();
            this.GRID_LOAD_RADIO = new System.Windows.Forms.RadioButton();
            this.LOAD_RADIO = new System.Windows.Forms.RadioButton();
            this.GRID_RADIO = new System.Windows.Forms.RadioButton();
            this.EMPTY_RADIO = new System.Windows.Forms.RadioButton();
            this.CREATE_BUTTON = new System.Windows.Forms.Button();
            this.CONFIGURATION_GROUP.SuspendLayout();
            this.SuspendLayout();
            // 
            // INVERTER_TABS
            // 
            this.INVERTER_TABS.Location = new System.Drawing.Point(12, 12);
            this.INVERTER_TABS.Name = "INVERTER_TABS";
            this.INVERTER_TABS.SelectedIndex = 0;
            this.INVERTER_TABS.Size = new System.Drawing.Size(197, 242);
            this.INVERTER_TABS.TabIndex = 0;
            // 
            // CONFIGURATION_GROUP
            // 
            this.CONFIGURATION_GROUP.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CONFIGURATION_GROUP.Controls.Add(this.GRID_LOAD_RADIO);
            this.CONFIGURATION_GROUP.Controls.Add(this.LOAD_RADIO);
            this.CONFIGURATION_GROUP.Controls.Add(this.GRID_RADIO);
            this.CONFIGURATION_GROUP.Controls.Add(this.EMPTY_RADIO);
            this.CONFIGURATION_GROUP.Location = new System.Drawing.Point(12, 260);
            this.CONFIGURATION_GROUP.Name = "CONFIGURATION_GROUP";
            this.CONFIGURATION_GROUP.Size = new System.Drawing.Size(195, 110);
            this.CONFIGURATION_GROUP.TabIndex = 1;
            this.CONFIGURATION_GROUP.TabStop = false;
            this.CONFIGURATION_GROUP.Text = "CONFIGURATION";
            // 
            // GRID_LOAD_RADIO
            // 
            this.GRID_LOAD_RADIO.AutoSize = true;
            this.GRID_LOAD_RADIO.Location = new System.Drawing.Point(6, 88);
            this.GRID_LOAD_RADIO.Name = "GRID_LOAD_RADIO";
            this.GRID_LOAD_RADIO.Size = new System.Drawing.Size(93, 17);
            this.GRID_LOAD_RADIO.TabIndex = 3;
            this.GRID_LOAD_RADIO.Text = "GRID + LOAD";
            this.GRID_LOAD_RADIO.UseVisualStyleBackColor = true;
            // 
            // LOAD_RADIO
            // 
            this.LOAD_RADIO.AutoSize = true;
            this.LOAD_RADIO.Location = new System.Drawing.Point(6, 65);
            this.LOAD_RADIO.Name = "LOAD_RADIO";
            this.LOAD_RADIO.Size = new System.Drawing.Size(54, 17);
            this.LOAD_RADIO.TabIndex = 2;
            this.LOAD_RADIO.Text = "LOAD";
            this.LOAD_RADIO.UseVisualStyleBackColor = true;
            // 
            // GRID_RADIO
            // 
            this.GRID_RADIO.AutoSize = true;
            this.GRID_RADIO.Location = new System.Drawing.Point(6, 42);
            this.GRID_RADIO.Name = "GRID_RADIO";
            this.GRID_RADIO.Size = new System.Drawing.Size(52, 17);
            this.GRID_RADIO.TabIndex = 1;
            this.GRID_RADIO.Text = "GRID";
            this.GRID_RADIO.UseVisualStyleBackColor = true;
            // 
            // EMPTY_RADIO
            // 
            this.EMPTY_RADIO.AutoSize = true;
            this.EMPTY_RADIO.Checked = true;
            this.EMPTY_RADIO.Location = new System.Drawing.Point(6, 19);
            this.EMPTY_RADIO.Name = "EMPTY_RADIO";
            this.EMPTY_RADIO.Size = new System.Drawing.Size(62, 17);
            this.EMPTY_RADIO.TabIndex = 0;
            this.EMPTY_RADIO.TabStop = true;
            this.EMPTY_RADIO.Text = "EMPTY";
            this.EMPTY_RADIO.UseVisualStyleBackColor = true;
            // 
            // CREATE_BUTTON
            // 
            this.CREATE_BUTTON.Location = new System.Drawing.Point(12, 376);
            this.CREATE_BUTTON.Name = "CREATE_BUTTON";
            this.CREATE_BUTTON.Size = new System.Drawing.Size(197, 32);
            this.CREATE_BUTTON.TabIndex = 2;
            this.CREATE_BUTTON.Text = "CREATE";
            this.CREATE_BUTTON.UseVisualStyleBackColor = true;
            this.CREATE_BUTTON.Click += new System.EventHandler(this.CREATE_BUTTON_Click);
            // 
            // InverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 422);
            this.Controls.Add(this.CREATE_BUTTON);
            this.Controls.Add(this.CONFIGURATION_GROUP);
            this.Controls.Add(this.INVERTER_TABS);
            this.Name = "InverterForm";
            this.Text = "Inverter Form";
            this.CONFIGURATION_GROUP.ResumeLayout(false);
            this.CONFIGURATION_GROUP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl INVERTER_TABS;
        private System.Windows.Forms.GroupBox CONFIGURATION_GROUP;
        private System.Windows.Forms.RadioButton GRID_LOAD_RADIO;
        private System.Windows.Forms.RadioButton LOAD_RADIO;
        private System.Windows.Forms.RadioButton GRID_RADIO;
        private System.Windows.Forms.RadioButton EMPTY_RADIO;
        private System.Windows.Forms.Button CREATE_BUTTON;
    }
}