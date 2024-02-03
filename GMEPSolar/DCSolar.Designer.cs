namespace GMEPSolar
{
    partial class DC_SOLAR_INPUT
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
            this.MPPT1_INPUT = new System.Windows.Forms.TextBox();
            this.MPPT1_LABEL = new System.Windows.Forms.Label();
            this.MPPT1_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.MPPT1_RADIO_REGULAR = new System.Windows.Forms.RadioButton();
            this.MPPT1_GROUPBOX = new System.Windows.Forms.GroupBox();
            this.MPPT1_RADIO_EMPTY = new System.Windows.Forms.RadioButton();
            this.MPPT1_RADIO_PARALLEL = new System.Windows.Forms.RadioButton();
            this.MPPT1_MODULES_LABEL = new System.Windows.Forms.Label();
            this.MPPT2_LABEL = new System.Windows.Forms.Label();
            this.MPPT2_INPUT = new System.Windows.Forms.TextBox();
            this.MPPT2_RADIO_PARALLEL = new System.Windows.Forms.RadioButton();
            this.MPPT2_RADIO_REGULAR = new System.Windows.Forms.RadioButton();
            this.MPPT2_MODULES_LABEL = new System.Windows.Forms.Label();
            this.MPPT2_GROUPBOX = new System.Windows.Forms.GroupBox();
            this.MPPT2_RADIO_EMPTY = new System.Windows.Forms.RadioButton();
            this.MPPT2_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.MPPT3_LABEL = new System.Windows.Forms.Label();
            this.MPPT3_INPUT = new System.Windows.Forms.TextBox();
            this.MPPT3_RADIO_PARALLEL = new System.Windows.Forms.RadioButton();
            this.MPPT3_RADIO_REGULAR = new System.Windows.Forms.RadioButton();
            this.MPPT3_MODULES_LABEL = new System.Windows.Forms.Label();
            this.MPPT3_GROUPBOX = new System.Windows.Forms.GroupBox();
            this.MPPT3_RADIO_EMPTY = new System.Windows.Forms.RadioButton();
            this.MPPT3_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.MPPT4_LABEL = new System.Windows.Forms.Label();
            this.MPPT4_INPUT = new System.Windows.Forms.TextBox();
            this.MPPT4_RADIO_PARALLEL = new System.Windows.Forms.RadioButton();
            this.MPPT4_RADIO_REGULAR = new System.Windows.Forms.RadioButton();
            this.MPPT4_MODULES_LABEL = new System.Windows.Forms.Label();
            this.MPPT4_GROUPBOX = new System.Windows.Forms.GroupBox();
            this.MPPT4_RADIO_EMPTY = new System.Windows.Forms.RadioButton();
            this.MPPT4_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.CREATE_BUTTON = new System.Windows.Forms.Button();
            this.ENABLE_ALL_BUTTON = new System.Windows.Forms.Button();
            this.ALL_REGULAR_BUTTON = new System.Windows.Forms.Button();
            this.ALL_PARALLEL_BUTTON = new System.Windows.Forms.Button();
            this.SET_ALL_MODULES_BUTTON = new System.Windows.Forms.Button();
            this.NUMBER_ALL_MODULES_TEXTBOX = new System.Windows.Forms.TextBox();
            this.MPPT1_GROUPBOX.SuspendLayout();
            this.MPPT2_GROUPBOX.SuspendLayout();
            this.MPPT3_GROUPBOX.SuspendLayout();
            this.MPPT4_GROUPBOX.SuspendLayout();
            this.SuspendLayout();
            // 
            // MPPT1_INPUT
            // 
            this.MPPT1_INPUT.Enabled = false;
            this.MPPT1_INPUT.Location = new System.Drawing.Point(16, 175);
            this.MPPT1_INPUT.Name = "MPPT1_INPUT";
            this.MPPT1_INPUT.Size = new System.Drawing.Size(130, 20);
            this.MPPT1_INPUT.TabIndex = 0;
            // 
            // MPPT1_LABEL
            // 
            this.MPPT1_LABEL.AutoSize = true;
            this.MPPT1_LABEL.Location = new System.Drawing.Point(13, 13);
            this.MPPT1_LABEL.Name = "MPPT1_LABEL";
            this.MPPT1_LABEL.Size = new System.Drawing.Size(43, 13);
            this.MPPT1_LABEL.TabIndex = 1;
            this.MPPT1_LABEL.Text = "MPPT1";
            // 
            // MPPT1_CHECKBOX
            // 
            this.MPPT1_CHECKBOX.AutoSize = true;
            this.MPPT1_CHECKBOX.Location = new System.Drawing.Point(16, 32);
            this.MPPT1_CHECKBOX.Name = "MPPT1_CHECKBOX";
            this.MPPT1_CHECKBOX.Size = new System.Drawing.Size(68, 17);
            this.MPPT1_CHECKBOX.TabIndex = 2;
            this.MPPT1_CHECKBOX.Text = "ENABLE";
            this.MPPT1_CHECKBOX.UseVisualStyleBackColor = true;
            this.MPPT1_CHECKBOX.CheckedChanged += new System.EventHandler(this.MPPT1_CHECKBOX_CheckedChanged);
            // 
            // MPPT1_RADIO_REGULAR
            // 
            this.MPPT1_RADIO_REGULAR.AutoSize = true;
            this.MPPT1_RADIO_REGULAR.Enabled = false;
            this.MPPT1_RADIO_REGULAR.Location = new System.Drawing.Point(6, 42);
            this.MPPT1_RADIO_REGULAR.Name = "MPPT1_RADIO_REGULAR";
            this.MPPT1_RADIO_REGULAR.Size = new System.Drawing.Size(77, 17);
            this.MPPT1_RADIO_REGULAR.TabIndex = 3;
            this.MPPT1_RADIO_REGULAR.Text = "REGULAR";
            this.MPPT1_RADIO_REGULAR.UseVisualStyleBackColor = true;
            // 
            // MPPT1_GROUPBOX
            // 
            this.MPPT1_GROUPBOX.Controls.Add(this.MPPT1_RADIO_EMPTY);
            this.MPPT1_GROUPBOX.Controls.Add(this.MPPT1_RADIO_PARALLEL);
            this.MPPT1_GROUPBOX.Controls.Add(this.MPPT1_RADIO_REGULAR);
            this.MPPT1_GROUPBOX.Location = new System.Drawing.Point(16, 55);
            this.MPPT1_GROUPBOX.Name = "MPPT1_GROUPBOX";
            this.MPPT1_GROUPBOX.Size = new System.Drawing.Size(86, 94);
            this.MPPT1_GROUPBOX.TabIndex = 4;
            this.MPPT1_GROUPBOX.TabStop = false;
            this.MPPT1_GROUPBOX.Text = "STATUS";
            // 
            // MPPT1_RADIO_EMPTY
            // 
            this.MPPT1_RADIO_EMPTY.AutoSize = true;
            this.MPPT1_RADIO_EMPTY.Checked = true;
            this.MPPT1_RADIO_EMPTY.Location = new System.Drawing.Point(6, 19);
            this.MPPT1_RADIO_EMPTY.Name = "MPPT1_RADIO_EMPTY";
            this.MPPT1_RADIO_EMPTY.Size = new System.Drawing.Size(62, 17);
            this.MPPT1_RADIO_EMPTY.TabIndex = 8;
            this.MPPT1_RADIO_EMPTY.TabStop = true;
            this.MPPT1_RADIO_EMPTY.Text = "EMPTY";
            this.MPPT1_RADIO_EMPTY.UseVisualStyleBackColor = true;
            this.MPPT1_RADIO_EMPTY.CheckedChanged += new System.EventHandler(this.MPPT1_RADIO_EMPTY_CheckedChanged);
            // 
            // MPPT1_RADIO_PARALLEL
            // 
            this.MPPT1_RADIO_PARALLEL.AutoSize = true;
            this.MPPT1_RADIO_PARALLEL.Enabled = false;
            this.MPPT1_RADIO_PARALLEL.Location = new System.Drawing.Point(6, 65);
            this.MPPT1_RADIO_PARALLEL.Name = "MPPT1_RADIO_PARALLEL";
            this.MPPT1_RADIO_PARALLEL.Size = new System.Drawing.Size(79, 17);
            this.MPPT1_RADIO_PARALLEL.TabIndex = 4;
            this.MPPT1_RADIO_PARALLEL.Text = "PARALLEL";
            this.MPPT1_RADIO_PARALLEL.UseVisualStyleBackColor = true;
            // 
            // MPPT1_MODULES_LABEL
            // 
            this.MPPT1_MODULES_LABEL.AutoSize = true;
            this.MPPT1_MODULES_LABEL.Location = new System.Drawing.Point(19, 159);
            this.MPPT1_MODULES_LABEL.Name = "MPPT1_MODULES_LABEL";
            this.MPPT1_MODULES_LABEL.Size = new System.Drawing.Size(127, 13);
            this.MPPT1_MODULES_LABEL.TabIndex = 5;
            this.MPPT1_MODULES_LABEL.Text = "NUMBER OF MODULES";
            // 
            // MPPT2_LABEL
            // 
            this.MPPT2_LABEL.AutoSize = true;
            this.MPPT2_LABEL.Location = new System.Drawing.Point(173, 13);
            this.MPPT2_LABEL.Name = "MPPT2_LABEL";
            this.MPPT2_LABEL.Size = new System.Drawing.Size(43, 13);
            this.MPPT2_LABEL.TabIndex = 7;
            this.MPPT2_LABEL.Text = "MPPT2";
            // 
            // MPPT2_INPUT
            // 
            this.MPPT2_INPUT.Enabled = false;
            this.MPPT2_INPUT.Location = new System.Drawing.Point(176, 175);
            this.MPPT2_INPUT.Name = "MPPT2_INPUT";
            this.MPPT2_INPUT.Size = new System.Drawing.Size(130, 20);
            this.MPPT2_INPUT.TabIndex = 6;
            // 
            // MPPT2_RADIO_PARALLEL
            // 
            this.MPPT2_RADIO_PARALLEL.AutoSize = true;
            this.MPPT2_RADIO_PARALLEL.Enabled = false;
            this.MPPT2_RADIO_PARALLEL.Location = new System.Drawing.Point(6, 65);
            this.MPPT2_RADIO_PARALLEL.Name = "MPPT2_RADIO_PARALLEL";
            this.MPPT2_RADIO_PARALLEL.Size = new System.Drawing.Size(79, 17);
            this.MPPT2_RADIO_PARALLEL.TabIndex = 4;
            this.MPPT2_RADIO_PARALLEL.Text = "PARALLEL";
            this.MPPT2_RADIO_PARALLEL.UseVisualStyleBackColor = true;
            // 
            // MPPT2_RADIO_REGULAR
            // 
            this.MPPT2_RADIO_REGULAR.AutoSize = true;
            this.MPPT2_RADIO_REGULAR.Enabled = false;
            this.MPPT2_RADIO_REGULAR.Location = new System.Drawing.Point(6, 42);
            this.MPPT2_RADIO_REGULAR.Name = "MPPT2_RADIO_REGULAR";
            this.MPPT2_RADIO_REGULAR.Size = new System.Drawing.Size(77, 17);
            this.MPPT2_RADIO_REGULAR.TabIndex = 3;
            this.MPPT2_RADIO_REGULAR.Text = "REGULAR";
            this.MPPT2_RADIO_REGULAR.UseVisualStyleBackColor = true;
            // 
            // MPPT2_MODULES_LABEL
            // 
            this.MPPT2_MODULES_LABEL.AutoSize = true;
            this.MPPT2_MODULES_LABEL.Location = new System.Drawing.Point(179, 159);
            this.MPPT2_MODULES_LABEL.Name = "MPPT2_MODULES_LABEL";
            this.MPPT2_MODULES_LABEL.Size = new System.Drawing.Size(127, 13);
            this.MPPT2_MODULES_LABEL.TabIndex = 10;
            this.MPPT2_MODULES_LABEL.Text = "NUMBER OF MODULES";
            // 
            // MPPT2_GROUPBOX
            // 
            this.MPPT2_GROUPBOX.Controls.Add(this.MPPT2_RADIO_EMPTY);
            this.MPPT2_GROUPBOX.Controls.Add(this.MPPT2_RADIO_PARALLEL);
            this.MPPT2_GROUPBOX.Controls.Add(this.MPPT2_RADIO_REGULAR);
            this.MPPT2_GROUPBOX.Location = new System.Drawing.Point(176, 55);
            this.MPPT2_GROUPBOX.Name = "MPPT2_GROUPBOX";
            this.MPPT2_GROUPBOX.Size = new System.Drawing.Size(86, 94);
            this.MPPT2_GROUPBOX.TabIndex = 9;
            this.MPPT2_GROUPBOX.TabStop = false;
            this.MPPT2_GROUPBOX.Text = "STATUS";
            // 
            // MPPT2_RADIO_EMPTY
            // 
            this.MPPT2_RADIO_EMPTY.AutoSize = true;
            this.MPPT2_RADIO_EMPTY.Checked = true;
            this.MPPT2_RADIO_EMPTY.Location = new System.Drawing.Point(6, 19);
            this.MPPT2_RADIO_EMPTY.Name = "MPPT2_RADIO_EMPTY";
            this.MPPT2_RADIO_EMPTY.Size = new System.Drawing.Size(62, 17);
            this.MPPT2_RADIO_EMPTY.TabIndex = 9;
            this.MPPT2_RADIO_EMPTY.TabStop = true;
            this.MPPT2_RADIO_EMPTY.Text = "EMPTY";
            this.MPPT2_RADIO_EMPTY.UseVisualStyleBackColor = true;
            this.MPPT2_RADIO_EMPTY.CheckedChanged += new System.EventHandler(this.MPPT2_RADIO_EMPTY_CheckedChanged);
            // 
            // MPPT2_CHECKBOX
            // 
            this.MPPT2_CHECKBOX.AutoSize = true;
            this.MPPT2_CHECKBOX.Location = new System.Drawing.Point(176, 32);
            this.MPPT2_CHECKBOX.Name = "MPPT2_CHECKBOX";
            this.MPPT2_CHECKBOX.Size = new System.Drawing.Size(68, 17);
            this.MPPT2_CHECKBOX.TabIndex = 8;
            this.MPPT2_CHECKBOX.Text = "ENABLE";
            this.MPPT2_CHECKBOX.UseVisualStyleBackColor = true;
            this.MPPT2_CHECKBOX.CheckedChanged += new System.EventHandler(this.MPPT2_CHECKBOX_CheckedChanged);
            // 
            // MPPT3_LABEL
            // 
            this.MPPT3_LABEL.AutoSize = true;
            this.MPPT3_LABEL.Location = new System.Drawing.Point(336, 13);
            this.MPPT3_LABEL.Name = "MPPT3_LABEL";
            this.MPPT3_LABEL.Size = new System.Drawing.Size(43, 13);
            this.MPPT3_LABEL.TabIndex = 12;
            this.MPPT3_LABEL.Text = "MPPT3";
            // 
            // MPPT3_INPUT
            // 
            this.MPPT3_INPUT.Enabled = false;
            this.MPPT3_INPUT.Location = new System.Drawing.Point(339, 175);
            this.MPPT3_INPUT.Name = "MPPT3_INPUT";
            this.MPPT3_INPUT.Size = new System.Drawing.Size(130, 20);
            this.MPPT3_INPUT.TabIndex = 11;
            // 
            // MPPT3_RADIO_PARALLEL
            // 
            this.MPPT3_RADIO_PARALLEL.AutoSize = true;
            this.MPPT3_RADIO_PARALLEL.Enabled = false;
            this.MPPT3_RADIO_PARALLEL.Location = new System.Drawing.Point(6, 65);
            this.MPPT3_RADIO_PARALLEL.Name = "MPPT3_RADIO_PARALLEL";
            this.MPPT3_RADIO_PARALLEL.Size = new System.Drawing.Size(79, 17);
            this.MPPT3_RADIO_PARALLEL.TabIndex = 4;
            this.MPPT3_RADIO_PARALLEL.Text = "PARALLEL";
            this.MPPT3_RADIO_PARALLEL.UseVisualStyleBackColor = true;
            // 
            // MPPT3_RADIO_REGULAR
            // 
            this.MPPT3_RADIO_REGULAR.AutoSize = true;
            this.MPPT3_RADIO_REGULAR.Enabled = false;
            this.MPPT3_RADIO_REGULAR.Location = new System.Drawing.Point(6, 42);
            this.MPPT3_RADIO_REGULAR.Name = "MPPT3_RADIO_REGULAR";
            this.MPPT3_RADIO_REGULAR.Size = new System.Drawing.Size(77, 17);
            this.MPPT3_RADIO_REGULAR.TabIndex = 3;
            this.MPPT3_RADIO_REGULAR.Text = "REGULAR";
            this.MPPT3_RADIO_REGULAR.UseVisualStyleBackColor = true;
            // 
            // MPPT3_MODULES_LABEL
            // 
            this.MPPT3_MODULES_LABEL.AutoSize = true;
            this.MPPT3_MODULES_LABEL.Location = new System.Drawing.Point(342, 159);
            this.MPPT3_MODULES_LABEL.Name = "MPPT3_MODULES_LABEL";
            this.MPPT3_MODULES_LABEL.Size = new System.Drawing.Size(127, 13);
            this.MPPT3_MODULES_LABEL.TabIndex = 15;
            this.MPPT3_MODULES_LABEL.Text = "NUMBER OF MODULES";
            // 
            // MPPT3_GROUPBOX
            // 
            this.MPPT3_GROUPBOX.Controls.Add(this.MPPT3_RADIO_EMPTY);
            this.MPPT3_GROUPBOX.Controls.Add(this.MPPT3_RADIO_PARALLEL);
            this.MPPT3_GROUPBOX.Controls.Add(this.MPPT3_RADIO_REGULAR);
            this.MPPT3_GROUPBOX.Location = new System.Drawing.Point(339, 55);
            this.MPPT3_GROUPBOX.Name = "MPPT3_GROUPBOX";
            this.MPPT3_GROUPBOX.Size = new System.Drawing.Size(86, 94);
            this.MPPT3_GROUPBOX.TabIndex = 14;
            this.MPPT3_GROUPBOX.TabStop = false;
            this.MPPT3_GROUPBOX.Text = "STATUS";
            // 
            // MPPT3_RADIO_EMPTY
            // 
            this.MPPT3_RADIO_EMPTY.AutoSize = true;
            this.MPPT3_RADIO_EMPTY.Checked = true;
            this.MPPT3_RADIO_EMPTY.Location = new System.Drawing.Point(6, 19);
            this.MPPT3_RADIO_EMPTY.Name = "MPPT3_RADIO_EMPTY";
            this.MPPT3_RADIO_EMPTY.Size = new System.Drawing.Size(62, 17);
            this.MPPT3_RADIO_EMPTY.TabIndex = 10;
            this.MPPT3_RADIO_EMPTY.TabStop = true;
            this.MPPT3_RADIO_EMPTY.Text = "EMPTY";
            this.MPPT3_RADIO_EMPTY.UseVisualStyleBackColor = true;
            this.MPPT3_RADIO_EMPTY.CheckedChanged += new System.EventHandler(this.MPPT3_RADIO_EMPTY_CheckedChanged);
            // 
            // MPPT3_CHECKBOX
            // 
            this.MPPT3_CHECKBOX.AutoSize = true;
            this.MPPT3_CHECKBOX.Location = new System.Drawing.Point(339, 32);
            this.MPPT3_CHECKBOX.Name = "MPPT3_CHECKBOX";
            this.MPPT3_CHECKBOX.Size = new System.Drawing.Size(68, 17);
            this.MPPT3_CHECKBOX.TabIndex = 13;
            this.MPPT3_CHECKBOX.Text = "ENABLE";
            this.MPPT3_CHECKBOX.UseVisualStyleBackColor = true;
            this.MPPT3_CHECKBOX.CheckedChanged += new System.EventHandler(this.MPPT3_CHECKBOX_CheckedChanged);
            // 
            // MPPT4_LABEL
            // 
            this.MPPT4_LABEL.AutoSize = true;
            this.MPPT4_LABEL.Location = new System.Drawing.Point(498, 13);
            this.MPPT4_LABEL.Name = "MPPT4_LABEL";
            this.MPPT4_LABEL.Size = new System.Drawing.Size(43, 13);
            this.MPPT4_LABEL.TabIndex = 17;
            this.MPPT4_LABEL.Text = "MPPT4";
            // 
            // MPPT4_INPUT
            // 
            this.MPPT4_INPUT.Enabled = false;
            this.MPPT4_INPUT.Location = new System.Drawing.Point(501, 175);
            this.MPPT4_INPUT.Name = "MPPT4_INPUT";
            this.MPPT4_INPUT.Size = new System.Drawing.Size(130, 20);
            this.MPPT4_INPUT.TabIndex = 16;
            // 
            // MPPT4_RADIO_PARALLEL
            // 
            this.MPPT4_RADIO_PARALLEL.AutoSize = true;
            this.MPPT4_RADIO_PARALLEL.Enabled = false;
            this.MPPT4_RADIO_PARALLEL.Location = new System.Drawing.Point(6, 65);
            this.MPPT4_RADIO_PARALLEL.Name = "MPPT4_RADIO_PARALLEL";
            this.MPPT4_RADIO_PARALLEL.Size = new System.Drawing.Size(79, 17);
            this.MPPT4_RADIO_PARALLEL.TabIndex = 4;
            this.MPPT4_RADIO_PARALLEL.Text = "PARALLEL";
            this.MPPT4_RADIO_PARALLEL.UseVisualStyleBackColor = true;
            // 
            // MPPT4_RADIO_REGULAR
            // 
            this.MPPT4_RADIO_REGULAR.AutoSize = true;
            this.MPPT4_RADIO_REGULAR.Enabled = false;
            this.MPPT4_RADIO_REGULAR.Location = new System.Drawing.Point(6, 42);
            this.MPPT4_RADIO_REGULAR.Name = "MPPT4_RADIO_REGULAR";
            this.MPPT4_RADIO_REGULAR.Size = new System.Drawing.Size(77, 17);
            this.MPPT4_RADIO_REGULAR.TabIndex = 3;
            this.MPPT4_RADIO_REGULAR.Text = "REGULAR";
            this.MPPT4_RADIO_REGULAR.UseVisualStyleBackColor = true;
            // 
            // MPPT4_MODULES_LABEL
            // 
            this.MPPT4_MODULES_LABEL.AutoSize = true;
            this.MPPT4_MODULES_LABEL.Location = new System.Drawing.Point(504, 159);
            this.MPPT4_MODULES_LABEL.Name = "MPPT4_MODULES_LABEL";
            this.MPPT4_MODULES_LABEL.Size = new System.Drawing.Size(127, 13);
            this.MPPT4_MODULES_LABEL.TabIndex = 20;
            this.MPPT4_MODULES_LABEL.Text = "NUMBER OF MODULES";
            // 
            // MPPT4_GROUPBOX
            // 
            this.MPPT4_GROUPBOX.Controls.Add(this.MPPT4_RADIO_EMPTY);
            this.MPPT4_GROUPBOX.Controls.Add(this.MPPT4_RADIO_PARALLEL);
            this.MPPT4_GROUPBOX.Controls.Add(this.MPPT4_RADIO_REGULAR);
            this.MPPT4_GROUPBOX.Location = new System.Drawing.Point(501, 55);
            this.MPPT4_GROUPBOX.Name = "MPPT4_GROUPBOX";
            this.MPPT4_GROUPBOX.Size = new System.Drawing.Size(86, 94);
            this.MPPT4_GROUPBOX.TabIndex = 19;
            this.MPPT4_GROUPBOX.TabStop = false;
            this.MPPT4_GROUPBOX.Text = "STATUS";
            // 
            // MPPT4_RADIO_EMPTY
            // 
            this.MPPT4_RADIO_EMPTY.AutoSize = true;
            this.MPPT4_RADIO_EMPTY.Checked = true;
            this.MPPT4_RADIO_EMPTY.Location = new System.Drawing.Point(6, 19);
            this.MPPT4_RADIO_EMPTY.Name = "MPPT4_RADIO_EMPTY";
            this.MPPT4_RADIO_EMPTY.Size = new System.Drawing.Size(62, 17);
            this.MPPT4_RADIO_EMPTY.TabIndex = 10;
            this.MPPT4_RADIO_EMPTY.TabStop = true;
            this.MPPT4_RADIO_EMPTY.Text = "EMPTY";
            this.MPPT4_RADIO_EMPTY.UseVisualStyleBackColor = true;
            this.MPPT4_RADIO_EMPTY.CheckedChanged += new System.EventHandler(this.MPPT4_RADIO_EMPTY_CheckedChanged);
            // 
            // MPPT4_CHECKBOX
            // 
            this.MPPT4_CHECKBOX.AutoSize = true;
            this.MPPT4_CHECKBOX.Location = new System.Drawing.Point(501, 32);
            this.MPPT4_CHECKBOX.Name = "MPPT4_CHECKBOX";
            this.MPPT4_CHECKBOX.Size = new System.Drawing.Size(68, 17);
            this.MPPT4_CHECKBOX.TabIndex = 18;
            this.MPPT4_CHECKBOX.Text = "ENABLE";
            this.MPPT4_CHECKBOX.UseVisualStyleBackColor = true;
            this.MPPT4_CHECKBOX.CheckedChanged += new System.EventHandler(this.MPPT4_CHECKBOX_CheckedChanged);
            // 
            // CREATE_BUTTON
            // 
            this.CREATE_BUTTON.Location = new System.Drawing.Point(16, 273);
            this.CREATE_BUTTON.Name = "CREATE_BUTTON";
            this.CREATE_BUTTON.Size = new System.Drawing.Size(615, 23);
            this.CREATE_BUTTON.TabIndex = 21;
            this.CREATE_BUTTON.Text = "CREATE";
            this.CREATE_BUTTON.UseVisualStyleBackColor = true;
            this.CREATE_BUTTON.Click += new System.EventHandler(this.CREATE_BUTTON_Click);
            // 
            // ENABLE_ALL_BUTTON
            // 
            this.ENABLE_ALL_BUTTON.Location = new System.Drawing.Point(16, 244);
            this.ENABLE_ALL_BUTTON.Name = "ENABLE_ALL_BUTTON";
            this.ENABLE_ALL_BUTTON.Size = new System.Drawing.Size(99, 23);
            this.ENABLE_ALL_BUTTON.TabIndex = 22;
            this.ENABLE_ALL_BUTTON.Text = "ENABLE ALL";
            this.ENABLE_ALL_BUTTON.UseVisualStyleBackColor = true;
            this.ENABLE_ALL_BUTTON.Click += new System.EventHandler(this.ENABLE_ALL_BUTTON_Click);
            // 
            // ALL_REGULAR_BUTTON
            // 
            this.ALL_REGULAR_BUTTON.Location = new System.Drawing.Point(121, 244);
            this.ALL_REGULAR_BUTTON.Name = "ALL_REGULAR_BUTTON";
            this.ALL_REGULAR_BUTTON.Size = new System.Drawing.Size(103, 23);
            this.ALL_REGULAR_BUTTON.TabIndex = 23;
            this.ALL_REGULAR_BUTTON.Text = "ALL REGULAR";
            this.ALL_REGULAR_BUTTON.UseVisualStyleBackColor = true;
            this.ALL_REGULAR_BUTTON.Click += new System.EventHandler(this.ALL_REGULAR_BUTTON_Click);
            // 
            // ALL_PARALLEL_BUTTON
            // 
            this.ALL_PARALLEL_BUTTON.Location = new System.Drawing.Point(230, 244);
            this.ALL_PARALLEL_BUTTON.Name = "ALL_PARALLEL_BUTTON";
            this.ALL_PARALLEL_BUTTON.Size = new System.Drawing.Size(103, 23);
            this.ALL_PARALLEL_BUTTON.TabIndex = 24;
            this.ALL_PARALLEL_BUTTON.Text = "ALL PARALLEL";
            this.ALL_PARALLEL_BUTTON.UseVisualStyleBackColor = true;
            this.ALL_PARALLEL_BUTTON.Click += new System.EventHandler(this.ALL_PARALLEL_BUTTON_Click);
            // 
            // SET_ALL_MODULES_BUTTON
            // 
            this.SET_ALL_MODULES_BUTTON.Location = new System.Drawing.Point(501, 244);
            this.SET_ALL_MODULES_BUTTON.Name = "SET_ALL_MODULES_BUTTON";
            this.SET_ALL_MODULES_BUTTON.Size = new System.Drawing.Size(130, 23);
            this.SET_ALL_MODULES_BUTTON.TabIndex = 25;
            this.SET_ALL_MODULES_BUTTON.Text = "SET ALL MODULES";
            this.SET_ALL_MODULES_BUTTON.UseVisualStyleBackColor = true;
            this.SET_ALL_MODULES_BUTTON.Click += new System.EventHandler(this.SET_ALL_MODULES_BUTTON_Click);
            // 
            // NUMBER_ALL_MODULES_TEXTBOX
            // 
            this.NUMBER_ALL_MODULES_TEXTBOX.Location = new System.Drawing.Point(340, 245);
            this.NUMBER_ALL_MODULES_TEXTBOX.Name = "NUMBER_ALL_MODULES_TEXTBOX";
            this.NUMBER_ALL_MODULES_TEXTBOX.Size = new System.Drawing.Size(155, 20);
            this.NUMBER_ALL_MODULES_TEXTBOX.TabIndex = 26;
            // 
            // DC_SOLAR_INPUT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 310);
            this.Controls.Add(this.NUMBER_ALL_MODULES_TEXTBOX);
            this.Controls.Add(this.SET_ALL_MODULES_BUTTON);
            this.Controls.Add(this.ALL_PARALLEL_BUTTON);
            this.Controls.Add(this.ALL_REGULAR_BUTTON);
            this.Controls.Add(this.ENABLE_ALL_BUTTON);
            this.Controls.Add(this.CREATE_BUTTON);
            this.Controls.Add(this.MPPT4_LABEL);
            this.Controls.Add(this.MPPT4_INPUT);
            this.Controls.Add(this.MPPT4_MODULES_LABEL);
            this.Controls.Add(this.MPPT4_GROUPBOX);
            this.Controls.Add(this.MPPT4_CHECKBOX);
            this.Controls.Add(this.MPPT3_LABEL);
            this.Controls.Add(this.MPPT3_INPUT);
            this.Controls.Add(this.MPPT3_MODULES_LABEL);
            this.Controls.Add(this.MPPT3_GROUPBOX);
            this.Controls.Add(this.MPPT3_CHECKBOX);
            this.Controls.Add(this.MPPT2_LABEL);
            this.Controls.Add(this.MPPT2_INPUT);
            this.Controls.Add(this.MPPT2_MODULES_LABEL);
            this.Controls.Add(this.MPPT2_GROUPBOX);
            this.Controls.Add(this.MPPT2_CHECKBOX);
            this.Controls.Add(this.MPPT1_MODULES_LABEL);
            this.Controls.Add(this.MPPT1_GROUPBOX);
            this.Controls.Add(this.MPPT1_CHECKBOX);
            this.Controls.Add(this.MPPT1_LABEL);
            this.Controls.Add(this.MPPT1_INPUT);
            this.Name = "DC_SOLAR_INPUT";
            this.Text = "DC Solar Input";
            this.MPPT1_GROUPBOX.ResumeLayout(false);
            this.MPPT1_GROUPBOX.PerformLayout();
            this.MPPT2_GROUPBOX.ResumeLayout(false);
            this.MPPT2_GROUPBOX.PerformLayout();
            this.MPPT3_GROUPBOX.ResumeLayout(false);
            this.MPPT3_GROUPBOX.PerformLayout();
            this.MPPT4_GROUPBOX.ResumeLayout(false);
            this.MPPT4_GROUPBOX.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MPPT1_INPUT;
        private System.Windows.Forms.Label MPPT1_LABEL;
        private System.Windows.Forms.CheckBox MPPT1_CHECKBOX;
        private System.Windows.Forms.RadioButton MPPT1_RADIO_REGULAR;
        private System.Windows.Forms.GroupBox MPPT1_GROUPBOX;
        private System.Windows.Forms.RadioButton MPPT1_RADIO_PARALLEL;
        private System.Windows.Forms.Label MPPT1_MODULES_LABEL;
        private System.Windows.Forms.Label MPPT2_LABEL;
        private System.Windows.Forms.TextBox MPPT2_INPUT;
        private System.Windows.Forms.RadioButton MPPT2_RADIO_PARALLEL;
        private System.Windows.Forms.RadioButton MPPT2_RADIO_REGULAR;
        private System.Windows.Forms.Label MPPT2_MODULES_LABEL;
        private System.Windows.Forms.GroupBox MPPT2_GROUPBOX;
        private System.Windows.Forms.CheckBox MPPT2_CHECKBOX;
        private System.Windows.Forms.Label MPPT3_LABEL;
        private System.Windows.Forms.TextBox MPPT3_INPUT;
        private System.Windows.Forms.RadioButton MPPT3_RADIO_PARALLEL;
        private System.Windows.Forms.RadioButton MPPT3_RADIO_REGULAR;
        private System.Windows.Forms.Label MPPT3_MODULES_LABEL;
        private System.Windows.Forms.GroupBox MPPT3_GROUPBOX;
        private System.Windows.Forms.CheckBox MPPT3_CHECKBOX;
        private System.Windows.Forms.Label MPPT4_LABEL;
        private System.Windows.Forms.TextBox MPPT4_INPUT;
        private System.Windows.Forms.RadioButton MPPT4_RADIO_PARALLEL;
        private System.Windows.Forms.RadioButton MPPT4_RADIO_REGULAR;
        private System.Windows.Forms.Label MPPT4_MODULES_LABEL;
        private System.Windows.Forms.GroupBox MPPT4_GROUPBOX;
        private System.Windows.Forms.CheckBox MPPT4_CHECKBOX;
        private System.Windows.Forms.Button CREATE_BUTTON;
        private System.Windows.Forms.Button ENABLE_ALL_BUTTON;
        private System.Windows.Forms.Button ALL_REGULAR_BUTTON;
        private System.Windows.Forms.Button ALL_PARALLEL_BUTTON;
        private System.Windows.Forms.Button SET_ALL_MODULES_BUTTON;
        private System.Windows.Forms.TextBox NUMBER_ALL_MODULES_TEXTBOX;
        private System.Windows.Forms.RadioButton MPPT1_RADIO_EMPTY;
        private System.Windows.Forms.RadioButton MPPT2_RADIO_EMPTY;
        private System.Windows.Forms.RadioButton MPPT3_RADIO_EMPTY;
        private System.Windows.Forms.RadioButton MPPT4_RADIO_EMPTY;
    }
}

