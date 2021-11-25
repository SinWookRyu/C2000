using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CytoDx
{
    public partial class TpntEditForm : Form
    {
        public TpntEditForm()
        {
            InitializeComponent();
            this.label_title.MouseDown += new MouseEventHandler(form_MouseDown);
            this.label_title.MouseMove += new MouseEventHandler(form_MouseMove);
        }
       
        private Point mousePoint;
        private MainWindow mainWindow;

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

        public void HideLabel()
        {
            this.Label1.Hide();
            this.Label2.Hide();
            this.Label3.Hide();
            this.Label4.Hide();
            this.Label5.Hide();
            this.Label6.Hide();
            this.Label7.Hide();
        }

        public void HideParam()
        {
            this.editParam1.Hide();
            this.editParam2.Hide();
            this.editParam3.Hide();
            this.editParam4.Hide();
            this.editParam5.Hide();
            this.editParam6.Hide();
            this.editParam7.Hide();
        }

        public void ClearParam()
        {
            editParam1.Text = "";
            editParam2.Text = "";
            editParam3.Text = "";
            editParam4.Text = "";
            editParam5.Text = "";
            editParam6.Text = "";
            editParam7.Text = "";
        }

        public void SetParam(MainWindow.TpntParam param, MainWindow mainWin)
        {
            try
            {
                if (mainWin != null)
                {
                    mainWindow = mainWin;
                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5;
                    editParam6.Text = param.param6;
                    editParam7.Text = param.param7;
                }
                else
                {
                    editParam1.Text = param.param1 = "";
                    editParam2.Text = param.param2 = "";
                    editParam3.Text = param.param3 = "";
                    editParam4.Text = param.param4 = "";
                    editParam5.Text = param.param5 = "";
                    editParam6.Text = param.param6 = "";
                    editParam7.Text = param.param7 = "";
                }

                //SetLabel(tpnt, param);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
