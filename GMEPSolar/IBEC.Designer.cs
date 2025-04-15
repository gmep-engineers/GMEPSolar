namespace GMEPSolar
{
    partial class IBEC
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
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.AddPowerStationButton = new System.Windows.Forms.Button();
      this.PowerStationFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(833, 24);
      this.menuStrip1.TabIndex = 1;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // saveToolStripMenuItem
      // 
      this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
      this.saveToolStripMenuItem.Text = "Save";
      this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveButton_Click);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
      this.closeToolStripMenuItem.Text = "Close";
      // 
      // AddPowerStationButton
      // 
      this.AddPowerStationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.AddPowerStationButton.Location = new System.Drawing.Point(12, 388);
      this.AddPowerStationButton.Name = "AddPowerStationButton";
      this.AddPowerStationButton.Size = new System.Drawing.Size(124, 23);
      this.AddPowerStationButton.TabIndex = 2;
      this.AddPowerStationButton.Text = "Add Power Station";
      this.AddPowerStationButton.UseVisualStyleBackColor = true;
      this.AddPowerStationButton.Click += new System.EventHandler(this.AddPowerStationButton_Click);
      // 
      // PowerStationFlowLayoutPanel
      // 
      this.PowerStationFlowLayoutPanel.AutoScroll = true;
      this.PowerStationFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.PowerStationFlowLayoutPanel.Location = new System.Drawing.Point(12, 34);
      this.PowerStationFlowLayoutPanel.Name = "PowerStationFlowLayoutPanel";
      this.PowerStationFlowLayoutPanel.Size = new System.Drawing.Size(809, 348);
      this.PowerStationFlowLayoutPanel.TabIndex = 0;
      this.PowerStationFlowLayoutPanel.WrapContents = false;
      // 
      // IBEC
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(833, 421);
      this.Controls.Add(this.AddPowerStationButton);
      this.Controls.Add(this.PowerStationFlowLayoutPanel);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "IBEC";
      this.Text = "IBEC";
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button AddPowerStationButton;
    private System.Windows.Forms.FlowLayoutPanel PowerStationFlowLayoutPanel;
  }
}