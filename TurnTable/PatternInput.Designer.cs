namespace CytoDx
{
    partial class PatternInput
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
            this.label_title = new System.Windows.Forms.Label();
            this.combo_cmd1 = new System.Windows.Forms.ComboBox();
            this.combo_cmd2 = new System.Windows.Forms.ComboBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.combo_cmd3 = new System.Windows.Forms.ComboBox();
            this.combo_cmd5 = new System.Windows.Forms.ComboBox();
            this.combo_cmd4 = new System.Windows.Forms.ComboBox();
            this.label_cmd1 = new MaterialSkin.Controls.MaterialLabel();
            this.editComment = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.label_cmd4 = new MaterialSkin.Controls.MaterialLabel();
            this.label_cmd2 = new MaterialSkin.Controls.MaterialLabel();
            this.editSleep = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Label1 = new MaterialSkin.Controls.MaterialLabel();
            this.editParam1 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.editParam5 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.editParam2 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.Label6 = new MaterialSkin.Controls.MaterialLabel();
            this.Label3 = new MaterialSkin.Controls.MaterialLabel();
            this.editParam7 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.Label7 = new MaterialSkin.Controls.MaterialLabel();
            this.editParam4 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.editParam6 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.Label4 = new MaterialSkin.Controls.MaterialLabel();
            this.Label5 = new MaterialSkin.Controls.MaterialLabel();
            this.editParam3 = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.Label2 = new MaterialSkin.Controls.MaterialLabel();
            this.radio_disable = new MaterialSkin.Controls.MaterialRadioButton();
            this.radio_enable = new MaterialSkin.Controls.MaterialRadioButton();
            this.btnCancel = new MaterialSkin.Controls.MaterialRaisedButton();
            this.btnOk = new MaterialSkin.Controls.MaterialRaisedButton();
            this.groupBox15.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_title
            // 
            this.label_title.BackColor = System.Drawing.Color.DarkCyan;
            this.label_title.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_title.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_title.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label_title.Location = new System.Drawing.Point(0, 0);
            this.label_title.Name = "label_title";
            this.label_title.Size = new System.Drawing.Size(384, 31);
            this.label_title.TabIndex = 16;
            this.label_title.Text = "Input Pattern";
            this.label_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // combo_cmd1
            // 
            this.combo_cmd1.BackColor = System.Drawing.Color.DodgerBlue;
            this.combo_cmd1.DropDownHeight = 140;
            this.combo_cmd1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo_cmd1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.combo_cmd1.ForeColor = System.Drawing.SystemColors.Info;
            this.combo_cmd1.IntegralHeight = false;
            this.combo_cmd1.ItemHeight = 12;
            this.combo_cmd1.Location = new System.Drawing.Point(199, 17);
            this.combo_cmd1.Margin = new System.Windows.Forms.Padding(0);
            this.combo_cmd1.Name = "combo_cmd1";
            this.combo_cmd1.Size = new System.Drawing.Size(156, 20);
            this.combo_cmd1.TabIndex = 16;
            this.combo_cmd1.SelectedValueChanged += new System.EventHandler(this.combo_cmd_SelectedValueChanged);
            // 
            // combo_cmd2
            // 
            this.combo_cmd2.BackColor = System.Drawing.Color.DodgerBlue;
            this.combo_cmd2.DropDownHeight = 140;
            this.combo_cmd2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo_cmd2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.combo_cmd2.ForeColor = System.Drawing.SystemColors.Info;
            this.combo_cmd2.IntegralHeight = false;
            this.combo_cmd2.ItemHeight = 12;
            this.combo_cmd2.Location = new System.Drawing.Point(199, 41);
            this.combo_cmd2.Margin = new System.Windows.Forms.Padding(0);
            this.combo_cmd2.Name = "combo_cmd2";
            this.combo_cmd2.Size = new System.Drawing.Size(156, 20);
            this.combo_cmd2.TabIndex = 16;
            this.combo_cmd2.SelectedIndexChanged += new System.EventHandler(this.combo_param1_SelectedValueChanged);
            // 
            // groupBox15
            // 
            this.groupBox15.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox15.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.groupBox15.Controls.Add(this.combo_cmd3);
            this.groupBox15.Controls.Add(this.combo_cmd5);
            this.groupBox15.Controls.Add(this.combo_cmd4);
            this.groupBox15.Controls.Add(this.combo_cmd2);
            this.groupBox15.Controls.Add(this.combo_cmd1);
            this.groupBox15.Controls.Add(this.label_cmd1);
            this.groupBox15.Controls.Add(this.editComment);
            this.groupBox15.Controls.Add(this.label_cmd4);
            this.groupBox15.Controls.Add(this.label_cmd2);
            this.groupBox15.Controls.Add(this.editSleep);
            this.groupBox15.Controls.Add(this.materialLabel2);
            this.groupBox15.Controls.Add(this.materialLabel1);
            this.groupBox15.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F);
            this.groupBox15.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox15.Location = new System.Drawing.Point(10, 71);
            this.groupBox15.Margin = new System.Windows.Forms.Padding(1);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(366, 144);
            this.groupBox15.TabIndex = 29;
            this.groupBox15.TabStop = false;
            // 
            // combo_cmd3
            // 
            this.combo_cmd3.BackColor = System.Drawing.Color.DodgerBlue;
            this.combo_cmd3.DropDownHeight = 140;
            this.combo_cmd3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo_cmd3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.combo_cmd3.ForeColor = System.Drawing.SystemColors.Info;
            this.combo_cmd3.IntegralHeight = false;
            this.combo_cmd3.ItemHeight = 12;
            this.combo_cmd3.Items.AddRange(new object[] {
            "All",
            "Normal",
            "Cooler",
            "Tip",
            "Tube",
            "Centrifuge"});
            this.combo_cmd3.Location = new System.Drawing.Point(103, 41);
            this.combo_cmd3.Margin = new System.Windows.Forms.Padding(0);
            this.combo_cmd3.Name = "combo_cmd3";
            this.combo_cmd3.Size = new System.Drawing.Size(85, 20);
            this.combo_cmd3.TabIndex = 16;
            this.combo_cmd3.Text = "All";
            this.combo_cmd3.SelectedValueChanged += new System.EventHandler(this.combo_Tpnt_SelectedValueChanged);
            // 
            // combo_cmd5
            // 
            this.combo_cmd5.BackColor = System.Drawing.Color.DodgerBlue;
            this.combo_cmd5.DropDownHeight = 140;
            this.combo_cmd5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo_cmd5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.combo_cmd5.ForeColor = System.Drawing.SystemColors.Info;
            this.combo_cmd5.IntegralHeight = false;
            this.combo_cmd5.ItemHeight = 12;
            this.combo_cmd5.Items.AddRange(new object[] {
            "No Tool",
            "1mL",
            "10uL",
            "300uL",
            "5mL",
            "Laser",
            "Gripper",
            "Calib Tip"});
            this.combo_cmd5.Location = new System.Drawing.Point(103, 65);
            this.combo_cmd5.Margin = new System.Windows.Forms.Padding(0);
            this.combo_cmd5.Name = "combo_cmd5";
            this.combo_cmd5.Size = new System.Drawing.Size(85, 20);
            this.combo_cmd5.TabIndex = 16;
            // 
            // combo_cmd4
            // 
            this.combo_cmd4.BackColor = System.Drawing.Color.DodgerBlue;
            this.combo_cmd4.DropDownHeight = 140;
            this.combo_cmd4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo_cmd4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.combo_cmd4.ForeColor = System.Drawing.SystemColors.Info;
            this.combo_cmd4.IntegralHeight = false;
            this.combo_cmd4.ItemHeight = 12;
            this.combo_cmd4.Items.AddRange(new object[] {
            "No Tool",
            "1mL",
            "10uL",
            "300uL",
            "5mL",
            "Laser",
            "Gripper",
            "Calib Tip"});
            this.combo_cmd4.Location = new System.Drawing.Point(199, 65);
            this.combo_cmd4.Margin = new System.Windows.Forms.Padding(0);
            this.combo_cmd4.Name = "combo_cmd4";
            this.combo_cmd4.Size = new System.Drawing.Size(156, 20);
            this.combo_cmd4.TabIndex = 16;
            this.combo_cmd4.SelectedValueChanged += new System.EventHandler(this.combo_Tool_SelectedValueChanged);
            // 
            // label_cmd1
            // 
            this.label_cmd1.AutoSize = true;
            this.label_cmd1.Depth = 0;
            this.label_cmd1.Font = new System.Drawing.Font("Roboto", 10F);
            this.label_cmd1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label_cmd1.Location = new System.Drawing.Point(6, 19);
            this.label_cmd1.MouseState = MaterialSkin.MouseState.HOVER;
            this.label_cmd1.Name = "label_cmd1";
            this.label_cmd1.Size = new System.Drawing.Size(73, 18);
            this.label_cmd1.TabIndex = 9;
            this.label_cmd1.Text = "Command";
            // 
            // editComment
            // 
            this.editComment.Depth = 0;
            this.editComment.Hint = "";
            this.editComment.Location = new System.Drawing.Point(113, 91);
            this.editComment.MaxLength = 32767;
            this.editComment.MouseState = MaterialSkin.MouseState.HOVER;
            this.editComment.Name = "editComment";
            this.editComment.PasswordChar = '\0';
            this.editComment.SelectedText = "";
            this.editComment.SelectionLength = 0;
            this.editComment.SelectionStart = 0;
            this.editComment.Size = new System.Drawing.Size(242, 22);
            this.editComment.TabIndex = 15;
            this.editComment.TabStop = false;
            this.editComment.UseSystemPasswordChar = false;
            // 
            // label_cmd4
            // 
            this.label_cmd4.AutoSize = true;
            this.label_cmd4.Depth = 0;
            this.label_cmd4.Font = new System.Drawing.Font("Roboto", 10F);
            this.label_cmd4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label_cmd4.Location = new System.Drawing.Point(6, 67);
            this.label_cmd4.MouseState = MaterialSkin.MouseState.HOVER;
            this.label_cmd4.Name = "label_cmd4";
            this.label_cmd4.Size = new System.Drawing.Size(35, 18);
            this.label_cmd4.TabIndex = 9;
            this.label_cmd4.Text = "Tool";
            // 
            // label_cmd2
            // 
            this.label_cmd2.AutoSize = true;
            this.label_cmd2.Depth = 0;
            this.label_cmd2.Font = new System.Drawing.Font("Roboto", 10F);
            this.label_cmd2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label_cmd2.Location = new System.Drawing.Point(7, 43);
            this.label_cmd2.MouseState = MaterialSkin.MouseState.HOVER;
            this.label_cmd2.Name = "label_cmd2";
            this.label_cmd2.Size = new System.Drawing.Size(82, 18);
            this.label_cmd2.TabIndex = 9;
            this.label_cmd2.Text = "Teach Point";
            // 
            // editSleep
            // 
            this.editSleep.Depth = 0;
            this.editSleep.Hint = "";
            this.editSleep.Location = new System.Drawing.Point(216, 114);
            this.editSleep.MaxLength = 32767;
            this.editSleep.MouseState = MaterialSkin.MouseState.HOVER;
            this.editSleep.Name = "editSleep";
            this.editSleep.PasswordChar = '\0';
            this.editSleep.SelectedText = "";
            this.editSleep.SelectionLength = 0;
            this.editSleep.SelectionStart = 0;
            this.editSleep.Size = new System.Drawing.Size(139, 22);
            this.editSleep.TabIndex = 15;
            this.editSleep.TabStop = false;
            this.editSleep.UseSystemPasswordChar = false;
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 10F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(7, 118);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(103, 18);
            this.materialLabel2.TabIndex = 11;
            this.materialLabel2.Text = "Cmd Sleep [ms]";
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 10F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(7, 95);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(69, 18);
            this.materialLabel1.TabIndex = 11;
            this.materialLabel1.Text = "Comment";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.groupBox1.Controls.Add(this.Label1);
            this.groupBox1.Controls.Add(this.editParam1);
            this.groupBox1.Controls.Add(this.editParam5);
            this.groupBox1.Controls.Add(this.editParam2);
            this.groupBox1.Controls.Add(this.Label6);
            this.groupBox1.Controls.Add(this.Label3);
            this.groupBox1.Controls.Add(this.editParam7);
            this.groupBox1.Controls.Add(this.Label7);
            this.groupBox1.Controls.Add(this.editParam4);
            this.groupBox1.Controls.Add(this.editParam6);
            this.groupBox1.Controls.Add(this.Label4);
            this.groupBox1.Controls.Add(this.Label5);
            this.groupBox1.Controls.Add(this.editParam3);
            this.groupBox1.Controls.Add(this.Label2);
            this.groupBox1.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox1.Location = new System.Drawing.Point(10, 214);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(366, 203);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Depth = 0;
            this.Label1.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label1.Location = new System.Drawing.Point(8, 18);
            this.Label1.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(58, 18);
            this.Label1.TabIndex = 9;
            this.Label1.Text = "Param1";
            // 
            // editParam1
            // 
            this.editParam1.Depth = 0;
            this.editParam1.Hint = "";
            this.editParam1.Location = new System.Drawing.Point(252, 14);
            this.editParam1.MaxLength = 32767;
            this.editParam1.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam1.Name = "editParam1";
            this.editParam1.PasswordChar = '\0';
            this.editParam1.SelectedText = "";
            this.editParam1.SelectionLength = 0;
            this.editParam1.SelectionStart = 0;
            this.editParam1.Size = new System.Drawing.Size(104, 22);
            this.editParam1.TabIndex = 12;
            this.editParam1.TabStop = false;
            this.editParam1.UseSystemPasswordChar = false;
            // 
            // editParam5
            // 
            this.editParam5.Depth = 0;
            this.editParam5.Hint = "";
            this.editParam5.Location = new System.Drawing.Point(252, 118);
            this.editParam5.MaxLength = 32767;
            this.editParam5.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam5.Name = "editParam5";
            this.editParam5.PasswordChar = '\0';
            this.editParam5.SelectedText = "";
            this.editParam5.SelectionLength = 0;
            this.editParam5.SelectionStart = 0;
            this.editParam5.Size = new System.Drawing.Size(104, 22);
            this.editParam5.TabIndex = 14;
            this.editParam5.TabStop = false;
            this.editParam5.UseSystemPasswordChar = false;
            // 
            // editParam2
            // 
            this.editParam2.Depth = 0;
            this.editParam2.Hint = "";
            this.editParam2.Location = new System.Drawing.Point(252, 40);
            this.editParam2.MaxLength = 32767;
            this.editParam2.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam2.Name = "editParam2";
            this.editParam2.PasswordChar = '\0';
            this.editParam2.SelectedText = "";
            this.editParam2.SelectionLength = 0;
            this.editParam2.SelectionStart = 0;
            this.editParam2.Size = new System.Drawing.Size(104, 22);
            this.editParam2.TabIndex = 14;
            this.editParam2.TabStop = false;
            this.editParam2.UseSystemPasswordChar = false;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Depth = 0;
            this.Label6.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label6.Location = new System.Drawing.Point(8, 148);
            this.Label6.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(58, 18);
            this.Label6.TabIndex = 11;
            this.Label6.Text = "Param6";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Depth = 0;
            this.Label3.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label3.Location = new System.Drawing.Point(8, 70);
            this.Label3.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(58, 18);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Param3";
            // 
            // editParam7
            // 
            this.editParam7.Depth = 0;
            this.editParam7.Hint = "";
            this.editParam7.Location = new System.Drawing.Point(252, 170);
            this.editParam7.MaxLength = 32767;
            this.editParam7.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam7.Name = "editParam7";
            this.editParam7.PasswordChar = '\0';
            this.editParam7.SelectedText = "";
            this.editParam7.SelectionLength = 0;
            this.editParam7.SelectionStart = 0;
            this.editParam7.Size = new System.Drawing.Size(104, 22);
            this.editParam7.TabIndex = 15;
            this.editParam7.TabStop = false;
            this.editParam7.UseSystemPasswordChar = false;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Depth = 0;
            this.Label7.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label7.Location = new System.Drawing.Point(8, 174);
            this.Label7.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(58, 18);
            this.Label7.TabIndex = 11;
            this.Label7.Text = "Param7";
            // 
            // editParam4
            // 
            this.editParam4.Depth = 0;
            this.editParam4.Hint = "";
            this.editParam4.Location = new System.Drawing.Point(252, 92);
            this.editParam4.MaxLength = 32767;
            this.editParam4.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam4.Name = "editParam4";
            this.editParam4.PasswordChar = '\0';
            this.editParam4.SelectedText = "";
            this.editParam4.SelectionLength = 0;
            this.editParam4.SelectionStart = 0;
            this.editParam4.Size = new System.Drawing.Size(104, 22);
            this.editParam4.TabIndex = 15;
            this.editParam4.TabStop = false;
            this.editParam4.UseSystemPasswordChar = false;
            // 
            // editParam6
            // 
            this.editParam6.Depth = 0;
            this.editParam6.Hint = "";
            this.editParam6.Location = new System.Drawing.Point(252, 144);
            this.editParam6.MaxLength = 32767;
            this.editParam6.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam6.Name = "editParam6";
            this.editParam6.PasswordChar = '\0';
            this.editParam6.SelectedText = "";
            this.editParam6.SelectionLength = 0;
            this.editParam6.SelectionStart = 0;
            this.editParam6.Size = new System.Drawing.Size(104, 22);
            this.editParam6.TabIndex = 15;
            this.editParam6.TabStop = false;
            this.editParam6.UseSystemPasswordChar = false;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Depth = 0;
            this.Label4.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label4.Location = new System.Drawing.Point(8, 96);
            this.Label4.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(58, 18);
            this.Label4.TabIndex = 11;
            this.Label4.Text = "Param4";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Depth = 0;
            this.Label5.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label5.Location = new System.Drawing.Point(8, 122);
            this.Label5.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(58, 18);
            this.Label5.TabIndex = 10;
            this.Label5.Text = "Param5";
            // 
            // editParam3
            // 
            this.editParam3.Depth = 0;
            this.editParam3.Hint = "";
            this.editParam3.Location = new System.Drawing.Point(252, 66);
            this.editParam3.MaxLength = 32767;
            this.editParam3.MouseState = MaterialSkin.MouseState.HOVER;
            this.editParam3.Name = "editParam3";
            this.editParam3.PasswordChar = '\0';
            this.editParam3.SelectedText = "";
            this.editParam3.SelectionLength = 0;
            this.editParam3.SelectionStart = 0;
            this.editParam3.Size = new System.Drawing.Size(104, 22);
            this.editParam3.TabIndex = 15;
            this.editParam3.TabStop = false;
            this.editParam3.UseSystemPasswordChar = false;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Depth = 0;
            this.Label2.Font = new System.Drawing.Font("Roboto", 10F);
            this.Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label2.Location = new System.Drawing.Point(8, 44);
            this.Label2.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(58, 18);
            this.Label2.TabIndex = 10;
            this.Label2.Text = "Param2";
            // 
            // radio_disable
            // 
            this.radio_disable.AutoSize = true;
            this.radio_disable.Depth = 0;
            this.radio_disable.Font = new System.Drawing.Font("Roboto", 10F);
            this.radio_disable.Location = new System.Drawing.Point(123, 40);
            this.radio_disable.Margin = new System.Windows.Forms.Padding(0);
            this.radio_disable.MouseLocation = new System.Drawing.Point(-1, -1);
            this.radio_disable.MouseState = MaterialSkin.MouseState.HOVER;
            this.radio_disable.Name = "radio_disable";
            this.radio_disable.Ripple = true;
            this.radio_disable.Size = new System.Drawing.Size(75, 30);
            this.radio_disable.TabIndex = 17;
            this.radio_disable.TabStop = true;
            this.radio_disable.Text = "Disable";
            this.radio_disable.UseVisualStyleBackColor = true;
            // 
            // radio_enable
            // 
            this.radio_enable.AutoSize = true;
            this.radio_enable.Depth = 0;
            this.radio_enable.Font = new System.Drawing.Font("Roboto", 10F);
            this.radio_enable.Location = new System.Drawing.Point(21, 40);
            this.radio_enable.Margin = new System.Windows.Forms.Padding(0);
            this.radio_enable.MouseLocation = new System.Drawing.Point(-1, -1);
            this.radio_enable.MouseState = MaterialSkin.MouseState.HOVER;
            this.radio_enable.Name = "radio_enable";
            this.radio_enable.Ripple = true;
            this.radio_enable.Size = new System.Drawing.Size(70, 30);
            this.radio_enable.TabIndex = 17;
            this.radio_enable.TabStop = true;
            this.radio_enable.Text = "Enable";
            this.radio_enable.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.Depth = 0;
            this.btnCancel.Icon = null;
            this.btnCancel.Location = new System.Drawing.Point(290, 428);
            this.btnCancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Primary = true;
            this.btnCancel.Size = new System.Drawing.Size(75, 31);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOk.Depth = 0;
            this.btnOk.Icon = null;
            this.btnOk.Location = new System.Drawing.Point(209, 428);
            this.btnOk.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnOk.Name = "btnOk";
            this.btnOk.Primary = true;
            this.btnOk.Size = new System.Drawing.Size(75, 31);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // PatternInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(384, 468);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox15);
            this.Controls.Add(this.radio_disable);
            this.Controls.Add(this.radio_enable);
            this.Controls.Add(this.label_title);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PatternInput";
            this.Text = "PatternInput";
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label_title;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam1;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam2;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam3;
        public MaterialSkin.Controls.MaterialLabel Label1;
        public MaterialSkin.Controls.MaterialLabel Label2;
        public MaterialSkin.Controls.MaterialLabel Label3;
        private MaterialSkin.Controls.MaterialRaisedButton btnCancel;
        private MaterialSkin.Controls.MaterialRaisedButton btnOk;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam4;
        public MaterialSkin.Controls.MaterialLabel Label4;
        public MaterialSkin.Controls.MaterialLabel label_cmd1;
        public System.Windows.Forms.ComboBox combo_cmd1;
        public System.Windows.Forms.ComboBox combo_cmd2;
        public MaterialSkin.Controls.MaterialRadioButton radio_enable;
        public MaterialSkin.Controls.MaterialRadioButton radio_disable;
        public MaterialSkin.Controls.MaterialLabel label_cmd2;
        public MaterialSkin.Controls.MaterialLabel materialLabel1;
        public MaterialSkin.Controls.MaterialSingleLineTextField editComment;
        public MaterialSkin.Controls.MaterialLabel materialLabel2;
        public MaterialSkin.Controls.MaterialSingleLineTextField editSleep;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.GroupBox groupBox1;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam5;
        public MaterialSkin.Controls.MaterialLabel Label6;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam7;
        public MaterialSkin.Controls.MaterialLabel Label7;
        public MaterialSkin.Controls.MaterialSingleLineTextField editParam6;
        public MaterialSkin.Controls.MaterialLabel Label5;
        public System.Windows.Forms.ComboBox combo_cmd3;
        public MaterialSkin.Controls.MaterialLabel label_cmd4;
        public System.Windows.Forms.ComboBox combo_cmd4;
        public System.Windows.Forms.ComboBox combo_cmd5;
    }
}