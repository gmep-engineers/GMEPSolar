namespace GMEPSolar
{
    partial class PowerStationUserControl
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
      this.SystemComboBox = new System.Windows.Forms.ComboBox();
      this.LotDataGridView = new System.Windows.Forms.DataGridView();
      this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Voltage = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.Amperage = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.Kaic = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.LoadVa = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SystemLabel = new System.Windows.Forms.Label();
      this.NumBattsComboBox = new System.Windows.Forms.ComboBox();
      this.NumBatts = new System.Windows.Forms.Label();
      this.LotsLabel = new System.Windows.Forms.Label();
      this.RemoveButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.LotDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // SystemComboBox
      // 
      this.SystemComboBox.FormattingEnabled = true;
      this.SystemComboBox.Items.AddRange(new object[] {
            "",
            "30KW",
            "45KW"});
      this.SystemComboBox.Location = new System.Drawing.Point(77, 23);
      this.SystemComboBox.Name = "SystemComboBox";
      this.SystemComboBox.Size = new System.Drawing.Size(121, 21);
      this.SystemComboBox.TabIndex = 0;
      // 
      // LotDataGridView
      // 
      this.LotDataGridView.CausesValidation = false;
      this.LotDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.LotDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Number,
            this.Voltage,
            this.Amperage,
            this.Kaic,
            this.LoadVa,
            this.Id});
      this.LotDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
      this.LotDataGridView.Location = new System.Drawing.Point(216, 23);
      this.LotDataGridView.Name = "LotDataGridView";
      this.LotDataGridView.Size = new System.Drawing.Size(564, 116);
      this.LotDataGridView.TabIndex = 1;
      // 
      // Number
      // 
      this.Number.HeaderText = "Number";
      this.Number.Name = "Number";
      // 
      // Voltage
      // 
      this.Voltage.HeaderText = "Voltage";
      this.Voltage.Items.AddRange(new object[] {
            "120/240/1Φ-3W",
            "120/208/1Φ-3W",
            "120/240/3Φ-4W",
            "120/208/3Φ-4W",
            "277/480/3Φ-4W"});
      this.Voltage.Name = "Voltage";
      this.Voltage.Width = 120;
      // 
      // Amperage
      // 
      this.Amperage.HeaderText = "Amperage";
      this.Amperage.Items.AddRange(new object[] {
            "100",
            "125",
            "150",
            "175",
            "200",
            "225",
            "250",
            "275",
            "300",
            "350",
            "400"});
      this.Amperage.Name = "Amperage";
      // 
      // Kaic
      // 
      this.Kaic.HeaderText = "KAIC";
      this.Kaic.Items.AddRange(new object[] {
            "22",
            "42"});
      this.Kaic.Name = "Kaic";
      this.Kaic.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.Kaic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      // 
      // LoadVa
      // 
      this.LoadVa.HeaderText = "Load VA";
      this.LoadVa.Name = "LoadVa";
      // 
      // Id
      // 
      this.Id.HeaderText = "Id";
      this.Id.Name = "Id";
      this.Id.ReadOnly = true;
      this.Id.Visible = false;
      // 
      // SystemLabel
      // 
      this.SystemLabel.AutoSize = true;
      this.SystemLabel.Location = new System.Drawing.Point(30, 26);
      this.SystemLabel.Name = "SystemLabel";
      this.SystemLabel.Size = new System.Drawing.Size(41, 13);
      this.SystemLabel.TabIndex = 2;
      this.SystemLabel.Text = "System";
      // 
      // NumBattsComboBox
      // 
      this.NumBattsComboBox.FormattingEnabled = true;
      this.NumBattsComboBox.Items.AddRange(new object[] {
            "",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
      this.NumBattsComboBox.Location = new System.Drawing.Point(77, 68);
      this.NumBattsComboBox.Name = "NumBattsComboBox";
      this.NumBattsComboBox.Size = new System.Drawing.Size(121, 21);
      this.NumBattsComboBox.TabIndex = 3;
      // 
      // NumBatts
      // 
      this.NumBatts.AutoSize = true;
      this.NumBatts.Location = new System.Drawing.Point(12, 71);
      this.NumBatts.Name = "NumBatts";
      this.NumBatts.Size = new System.Drawing.Size(59, 13);
      this.NumBatts.TabIndex = 4;
      this.NumBatts.Text = "Num Batts.";
      // 
      // LotsLabel
      // 
      this.LotsLabel.AutoSize = true;
      this.LotsLabel.Location = new System.Drawing.Point(216, 4);
      this.LotsLabel.Name = "LotsLabel";
      this.LotsLabel.Size = new System.Drawing.Size(27, 13);
      this.LotsLabel.TabIndex = 5;
      this.LotsLabel.Text = "Lots";
      // 
      // RemoveButton
      // 
      this.RemoveButton.ForeColor = System.Drawing.Color.Firebrick;
      this.RemoveButton.Location = new System.Drawing.Point(15, 106);
      this.RemoveButton.Name = "RemoveButton";
      this.RemoveButton.Size = new System.Drawing.Size(75, 23);
      this.RemoveButton.TabIndex = 6;
      this.RemoveButton.Text = "Remove";
      this.RemoveButton.UseVisualStyleBackColor = true;
      this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
      // 
      // PowerStationUserControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.RemoveButton);
      this.Controls.Add(this.LotsLabel);
      this.Controls.Add(this.NumBatts);
      this.Controls.Add(this.NumBattsComboBox);
      this.Controls.Add(this.SystemLabel);
      this.Controls.Add(this.LotDataGridView);
      this.Controls.Add(this.SystemComboBox);
      this.Name = "PowerStationUserControl";
      this.Size = new System.Drawing.Size(780, 142);
      ((System.ComponentModel.ISupportInitialize)(this.LotDataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SystemComboBox;
        private System.Windows.Forms.DataGridView LotDataGridView;
        private System.Windows.Forms.Label SystemLabel;
        private System.Windows.Forms.ComboBox NumBattsComboBox;
        private System.Windows.Forms.Label NumBatts;
        private System.Windows.Forms.Label LotsLabel;
        private System.Windows.Forms.Button RemoveButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn Number;
    private System.Windows.Forms.DataGridViewComboBoxColumn Voltage;
    private System.Windows.Forms.DataGridViewComboBoxColumn Amperage;
    private System.Windows.Forms.DataGridViewComboBoxColumn Kaic;
    private System.Windows.Forms.DataGridViewTextBoxColumn LoadVa;
    private System.Windows.Forms.DataGridViewTextBoxColumn Id;
  }
}
