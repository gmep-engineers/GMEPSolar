namespace GMEPSolar
{
    partial class InverterUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.POLES_GROUP = new System.Windows.Forms.GroupBox();
            this.RADIO_3P = new System.Windows.Forms.RadioButton();
            this.RADIO_2P = new System.Windows.Forms.RadioButton();
            this.STATUS_GROUP = new System.Windows.Forms.GroupBox();
            this.RADIO_MASTER = new System.Windows.Forms.RadioButton();
            this.RADIO_SLAVE = new System.Windows.Forms.RadioButton();
            this.DC_SOLAR_BUTTON = new System.Windows.Forms.Button();
            this.NEW_INVERTER_BUTTON = new System.Windows.Forms.Button();
            this.REMOVE_BUTTON = new System.Windows.Forms.Button();
            this.POLES_GROUP.SuspendLayout();
            this.STATUS_GROUP.SuspendLayout();
            this.SuspendLayout();
            // 
            // POLES_GROUP
            // 
            this.POLES_GROUP.Controls.Add(this.RADIO_3P);
            this.POLES_GROUP.Controls.Add(this.RADIO_2P);
            this.POLES_GROUP.Location = new System.Drawing.Point(17, 18);
            this.POLES_GROUP.Name = "POLES_GROUP";
            this.POLES_GROUP.Size = new System.Drawing.Size(59, 69);
            this.POLES_GROUP.TabIndex = 0;
            this.POLES_GROUP.TabStop = false;
            this.POLES_GROUP.Text = "POLES";
            // 
            // RADIO_3P
            // 
            this.RADIO_3P.AutoSize = true;
            this.RADIO_3P.Location = new System.Drawing.Point(11, 43);
            this.RADIO_3P.Name = "RADIO_3P";
            this.RADIO_3P.Size = new System.Drawing.Size(38, 17);
            this.RADIO_3P.TabIndex = 1;
            this.RADIO_3P.Text = "3P";
            this.RADIO_3P.UseVisualStyleBackColor = true;
            // 
            // RADIO_2P
            // 
            this.RADIO_2P.AutoSize = true;
            this.RADIO_2P.Checked = true;
            this.RADIO_2P.Location = new System.Drawing.Point(11, 20);
            this.RADIO_2P.Name = "RADIO_2P";
            this.RADIO_2P.Size = new System.Drawing.Size(38, 17);
            this.RADIO_2P.TabIndex = 0;
            this.RADIO_2P.TabStop = true;
            this.RADIO_2P.Text = "2P";
            this.RADIO_2P.UseVisualStyleBackColor = true;
            // 
            // STATUS_GROUP
            // 
            this.STATUS_GROUP.Controls.Add(this.RADIO_MASTER);
            this.STATUS_GROUP.Controls.Add(this.RADIO_SLAVE);
            this.STATUS_GROUP.Location = new System.Drawing.Point(82, 18);
            this.STATUS_GROUP.Name = "STATUS_GROUP";
            this.STATUS_GROUP.Size = new System.Drawing.Size(88, 69);
            this.STATUS_GROUP.TabIndex = 2;
            this.STATUS_GROUP.TabStop = false;
            this.STATUS_GROUP.Text = "STATUS";
            // 
            // RADIO_MASTER
            // 
            this.RADIO_MASTER.AutoSize = true;
            this.RADIO_MASTER.Location = new System.Drawing.Point(11, 43);
            this.RADIO_MASTER.Name = "RADIO_MASTER";
            this.RADIO_MASTER.Size = new System.Drawing.Size(70, 17);
            this.RADIO_MASTER.TabIndex = 1;
            this.RADIO_MASTER.Text = "MASTER";
            this.RADIO_MASTER.UseVisualStyleBackColor = true;
            // 
            // RADIO_SLAVE
            // 
            this.RADIO_SLAVE.AutoSize = true;
            this.RADIO_SLAVE.Checked = true;
            this.RADIO_SLAVE.Location = new System.Drawing.Point(11, 20);
            this.RADIO_SLAVE.Name = "RADIO_SLAVE";
            this.RADIO_SLAVE.Size = new System.Drawing.Size(59, 17);
            this.RADIO_SLAVE.TabIndex = 0;
            this.RADIO_SLAVE.TabStop = true;
            this.RADIO_SLAVE.Text = "SLAVE";
            this.RADIO_SLAVE.UseVisualStyleBackColor = true;
            // 
            // DC_SOLAR_BUTTON
            // 
            this.DC_SOLAR_BUTTON.Location = new System.Drawing.Point(17, 94);
            this.DC_SOLAR_BUTTON.Name = "DC_SOLAR_BUTTON";
            this.DC_SOLAR_BUTTON.Size = new System.Drawing.Size(153, 32);
            this.DC_SOLAR_BUTTON.TabIndex = 3;
            this.DC_SOLAR_BUTTON.Text = "DC SOLAR MODULE";
            this.DC_SOLAR_BUTTON.UseVisualStyleBackColor = true;
            this.DC_SOLAR_BUTTON.Click += new System.EventHandler(this.DC_SOLAR_BUTTON_Click);
            // 
            // NEW_INVERTER_BUTTON
            // 
            this.NEW_INVERTER_BUTTON.Location = new System.Drawing.Point(17, 132);
            this.NEW_INVERTER_BUTTON.Name = "NEW_INVERTER_BUTTON";
            this.NEW_INVERTER_BUTTON.Size = new System.Drawing.Size(153, 32);
            this.NEW_INVERTER_BUTTON.TabIndex = 4;
            this.NEW_INVERTER_BUTTON.Text = "NEW INVERTER";
            this.NEW_INVERTER_BUTTON.UseVisualStyleBackColor = true;
            this.NEW_INVERTER_BUTTON.Click += new System.EventHandler(this.NEW_INVERTER_BUTTON_Click);
            // 
            // REMOVE_BUTTON
            // 
            this.REMOVE_BUTTON.ForeColor = System.Drawing.Color.IndianRed;
            this.REMOVE_BUTTON.Location = new System.Drawing.Point(17, 170);
            this.REMOVE_BUTTON.Name = "REMOVE_BUTTON";
            this.REMOVE_BUTTON.Size = new System.Drawing.Size(153, 32);
            this.REMOVE_BUTTON.TabIndex = 5;
            this.REMOVE_BUTTON.Text = "REMOVE INVERTER";
            this.REMOVE_BUTTON.UseVisualStyleBackColor = true;
            this.REMOVE_BUTTON.Click += new System.EventHandler(this.REMOVE_BUTTON_Click);
            // 
            // InverterUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.REMOVE_BUTTON);
            this.Controls.Add(this.NEW_INVERTER_BUTTON);
            this.Controls.Add(this.DC_SOLAR_BUTTON);
            this.Controls.Add(this.STATUS_GROUP);
            this.Controls.Add(this.POLES_GROUP);
            this.Name = "InverterUserControl";
            this.Size = new System.Drawing.Size(189, 216);
            this.POLES_GROUP.ResumeLayout(false);
            this.POLES_GROUP.PerformLayout();
            this.STATUS_GROUP.ResumeLayout(false);
            this.STATUS_GROUP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox POLES_GROUP;
        private System.Windows.Forms.RadioButton RADIO_3P;
        private System.Windows.Forms.RadioButton RADIO_2P;
        private System.Windows.Forms.GroupBox STATUS_GROUP;
        private System.Windows.Forms.RadioButton RADIO_MASTER;
        private System.Windows.Forms.RadioButton RADIO_SLAVE;
        private System.Windows.Forms.Button DC_SOLAR_BUTTON;
        private System.Windows.Forms.Button NEW_INVERTER_BUTTON;
        private System.Windows.Forms.Button REMOVE_BUTTON;
    }
}
