namespace CytoDx
{
    partial class DefineButtonForm
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
            this.Label2 = new MaterialSkin.Controls.MaterialLabel();
            this.Label1 = new MaterialSkin.Controls.MaterialLabel();
            this.editButtonName = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.btnCancel = new MaterialSkin.Controls.MaterialRaisedButton();
            this.btnOk = new MaterialSkin.Controls.MaterialRaisedButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRecipeOpen = new MaterialSkin.Controls.MaterialRaisedButton();
            this.btnRecipeSave = new MaterialSkin.Controls.MaterialRaisedButton();
            this.btnRecipeSaveAs = new MaterialSkin.Controls.MaterialRaisedButton();
            this.lblRecipeFilename = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_title
            // 
            this.label_title.BackColor = System.Drawing.Color.DarkCyan;
            this.label_title.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_title.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_title.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label_title.Location = new System.Drawing.Point(0, 0);
            this.label_title.Name = "label_title";
            this.label_title.Size = new System.Drawing.Size(581, 31);
            this.label_title.TabIndex = 17;
            this.label_title.Text = "Button Definition";
            this.label_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Depth = 0;
            this.Label2.Font = new System.Drawing.Font("Roboto", 11F);
            this.Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label2.Location = new System.Drawing.Point(35, 90);
            this.Label2.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(77, 19);
            this.Label2.TabIndex = 19;
            this.Label2.Text = "File Name";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Depth = 0;
            this.Label1.Font = new System.Drawing.Font("Roboto", 11F);
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label1.Location = new System.Drawing.Point(35, 64);
            this.Label1.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(49, 19);
            this.Label1.TabIndex = 18;
            this.Label1.Text = "Name";
            // 
            // editButtonName
            // 
            this.editButtonName.Depth = 0;
            this.editButtonName.Hint = "";
            this.editButtonName.Location = new System.Drawing.Point(244, 60);
            this.editButtonName.MaxLength = 32767;
            this.editButtonName.MouseState = MaterialSkin.MouseState.HOVER;
            this.editButtonName.Name = "editButtonName";
            this.editButtonName.PasswordChar = '\0';
            this.editButtonName.SelectedText = "";
            this.editButtonName.SelectionLength = 0;
            this.editButtonName.SelectionStart = 0;
            this.editButtonName.Size = new System.Drawing.Size(310, 23);
            this.editButtonName.TabIndex = 22;
            this.editButtonName.TabStop = false;
            this.editButtonName.UseSystemPasswordChar = false;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.Depth = 0;
            this.btnCancel.Icon = null;
            this.btnCancel.Location = new System.Drawing.Point(473, 266);
            this.btnCancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Primary = true;
            this.btnCancel.Size = new System.Drawing.Size(75, 31);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOk.Depth = 0;
            this.btnOk.Icon = null;
            this.btnOk.Location = new System.Drawing.Point(392, 266);
            this.btnOk.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnOk.Name = "btnOk";
            this.btnOk.Primary = true;
            this.btnOk.Size = new System.Drawing.Size(75, 31);
            this.btnOk.TabIndex = 26;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel12);
            this.groupBox5.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F);
            this.groupBox5.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox5.Location = new System.Drawing.Point(39, 156);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(7, 3, 7, 3);
            this.groupBox5.Size = new System.Drawing.Size(158, 144);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "File";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Controls.Add(this.btnRecipeOpen, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.btnRecipeSave, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.btnRecipeSaveAs, 0, 2);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(7, 22);
            this.tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 4;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(144, 119);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // btnRecipeOpen
            // 
            this.btnRecipeOpen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRecipeOpen.Depth = 0;
            this.btnRecipeOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecipeOpen.Icon = null;
            this.btnRecipeOpen.Location = new System.Drawing.Point(6, 6);
            this.btnRecipeOpen.Margin = new System.Windows.Forms.Padding(6);
            this.btnRecipeOpen.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRecipeOpen.Name = "btnRecipeOpen";
            this.btnRecipeOpen.Primary = true;
            this.btnRecipeOpen.Size = new System.Drawing.Size(132, 21);
            this.btnRecipeOpen.TabIndex = 15;
            this.btnRecipeOpen.Text = "Open";
            this.btnRecipeOpen.UseVisualStyleBackColor = true;
            this.btnRecipeOpen.Click += new System.EventHandler(this.btnRecipeOpen_Click);
            // 
            // btnRecipeSave
            // 
            this.btnRecipeSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRecipeSave.Depth = 0;
            this.btnRecipeSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecipeSave.Icon = null;
            this.btnRecipeSave.Location = new System.Drawing.Point(6, 39);
            this.btnRecipeSave.Margin = new System.Windows.Forms.Padding(6);
            this.btnRecipeSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRecipeSave.Name = "btnRecipeSave";
            this.btnRecipeSave.Primary = true;
            this.btnRecipeSave.Size = new System.Drawing.Size(132, 21);
            this.btnRecipeSave.TabIndex = 13;
            this.btnRecipeSave.Text = "Save";
            this.btnRecipeSave.UseVisualStyleBackColor = true;
            this.btnRecipeSave.Click += new System.EventHandler(this.btnButtonNameSave_Click);
            // 
            // btnRecipeSaveAs
            // 
            this.btnRecipeSaveAs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRecipeSaveAs.Depth = 0;
            this.btnRecipeSaveAs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecipeSaveAs.Icon = null;
            this.btnRecipeSaveAs.Location = new System.Drawing.Point(6, 72);
            this.btnRecipeSaveAs.Margin = new System.Windows.Forms.Padding(6);
            this.btnRecipeSaveAs.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRecipeSaveAs.Name = "btnRecipeSaveAs";
            this.btnRecipeSaveAs.Primary = true;
            this.btnRecipeSaveAs.Size = new System.Drawing.Size(132, 21);
            this.btnRecipeSaveAs.TabIndex = 14;
            this.btnRecipeSaveAs.Text = "Save As";
            this.btnRecipeSaveAs.UseVisualStyleBackColor = true;
            this.btnRecipeSaveAs.Click += new System.EventHandler(this.btnButtonNameSaveAs_Click);
            // 
            // lblRecipeFilename
            // 
            this.lblRecipeFilename.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRecipeFilename.AutoSize = true;
            this.lblRecipeFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblRecipeFilename.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblRecipeFilename.Location = new System.Drawing.Point(241, 92);
            this.lblRecipeFilename.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRecipeFilename.Name = "lblRecipeFilename";
            this.lblRecipeFilename.Size = new System.Drawing.Size(71, 17);
            this.lblRecipeFilename.TabIndex = 47;
            this.lblRecipeFilename.Text = "recipe.rcp";
            // 
            // DefineButtonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(581, 317);
            this.Controls.Add(this.lblRecipeFilename);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.editButtonName);
            this.Controls.Add(this.label_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DefineButtonForm";
            this.Text = "DefineButtonForm";
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label_title;
        public MaterialSkin.Controls.MaterialLabel Label2;
        public MaterialSkin.Controls.MaterialLabel Label1;
        public MaterialSkin.Controls.MaterialSingleLineTextField editButtonName;
        private MaterialSkin.Controls.MaterialRaisedButton btnCancel;
        private MaterialSkin.Controls.MaterialRaisedButton btnOk;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private MaterialSkin.Controls.MaterialRaisedButton btnRecipeOpen;
        private MaterialSkin.Controls.MaterialRaisedButton btnRecipeSave;
        private MaterialSkin.Controls.MaterialRaisedButton btnRecipeSaveAs;
        public System.Windows.Forms.Label lblRecipeFilename;
    }
}