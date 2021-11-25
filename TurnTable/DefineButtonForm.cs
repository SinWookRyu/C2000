using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CytoDx
{
    public partial class DefineButtonForm : Form
    {
        public DefineButtonForm()
        {
            InitializeComponent();
            this.label_title.MouseDown += new MouseEventHandler(form_MouseDown);
            this.label_title.MouseMove += new MouseEventHandler(form_MouseMove);
        }

        private Point mousePoint;
        public MainWindow mainWindow;
        public int buttonIndex;

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void form_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X),
                    this.Top - (mousePoint.Y - e.Y));
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                this.btnCancel_Click(this, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnRecipeOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Recipe files (*.rcp)|*.rcp|JSON files (*.json)|*.json|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                dlg.InitialDirectory = Environment.CurrentDirectory + "\\Recipe";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    mainWindow.config.ReadWriteRecipe(RW.READ, dlg.FileName, ref mainWindow.ListButtonRecipe[buttonIndex].recipe);
                    lblRecipeFilename.Text = Path.GetFileName(dlg.FileName);
                    mainWindow.DefineButtons[buttonIndex].AccessibleName = dlg.FileName;
                    mainWindow.RefreshRecipeDataView(buttonIndex);
                }
            }
        }

        public void btnButtonNameSave_Click(object sender, EventArgs e)
        {
            mainWindow.GetRecipeFromListView(mainWindow.GetSelectedButtonIndex());

            if (lblRecipeFilename.Text == null || lblRecipeFilename.Text == "")
            {
                mainWindow.ListButtonRecipe[buttonIndex].button.AccessibleName =
                                            MainWindow.DIR_RECIPE+ "\\" + editButtonName.Text + ".rcp";
                                            //"C:\\TruNser_C2000\\Recipe\\" + editButtonName.Text + ".rcp";
                if (!File.Exists(mainWindow.ListButtonRecipe[buttonIndex].button.AccessibleName))
                {
                    lblRecipeFilename.Text = editButtonName.Text + ".rcp";
                }
                else if (File.Exists(mainWindow.ListButtonRecipe[buttonIndex].button.AccessibleName))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("\"{0}.rcp\" Already Same File Name Exist! Rename & Try Again!",
                                                                                   editButtonName.Text), "File Exist Error", 
                                                                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    mainWindow.ListButtonRecipe[buttonIndex].button.AccessibleName = "";
                }
            }

            mainWindow.config.ReadWriteRecipe(RW.WRITE, 
                                              mainWindow.ListButtonRecipe[buttonIndex].button.AccessibleName, 
                                              ref mainWindow.ListButtonRecipe[buttonIndex].recipe);
        }

        private void btnButtonNameSaveAs_Click(object sender, EventArgs e)
        {
            mainWindow.GetRecipeFromListView(mainWindow.GetSelectedButtonIndex());
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Recipe files (*.rcp)|*.rcp|JSON files (*.json)|*.json|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                dlg.InitialDirectory = Environment.CurrentDirectory + "\\Recipe";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    mainWindow.config.ReadWriteRecipe(RW.WRITE, dlg.FileName, ref mainWindow.ListButtonRecipe[buttonIndex].recipe);
                    mainWindow.DefineButtons[buttonIndex].AccessibleName = dlg.FileName;
                    lblRecipeFilename.Text = Path.GetFileName(dlg.FileName);
                }
            }
        }
    }
}
