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
    
    public partial class PatternInput : Form
    {
        public PatternInput()
        {
            InitializeComponent();
            this.label_title.MouseDown += new MouseEventHandler(form_MouseDown);
            this.label_title.MouseMove += new MouseEventHandler(form_MouseMove);
            combo_cmd1.DataSource = Enum.GetValues(typeof(MainWindow.PROCESS_CMD));
            combo_cmd3.Text = "All";
            combo_cmd4.Text = "No Tool";
        }

        private Point mousePoint;
        public MainWindow mainWindow;
        public MainWindow.CommandParam param;

        private string strFindIdx = null;
        public bool bTpntSortSelectFlag = false;
        
        private string strToolIdx = null;
        public bool bToolSelectFlag = false;

        public MainWindow.TOOL_OFFSET offset_val = new MainWindow.TOOL_OFFSET();
        public int toolIdx = 0;

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
            int param_cnt = MainWindow.dicCmd[(MainWindow.PROCESS_CMD)combo_cmd1.SelectedIndex].param_cnt;

            for (int i = 0; i < param_cnt; i++)
            {
                if (i == 0)
                {
                    if (editParam1.Text == "")
                    {
                        MessageBox.Show("Please Insert Param1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 1)
                {
                    if (editParam2.Text == "")
                    {
                        MessageBox.Show("Please Insert Param2", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 2)
                {
                    if (editParam3.Text == "")
                    {
                        MessageBox.Show("Please Insert Param3", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 3)
                {
                    if (editParam4.Text == "")
                    {
                        MessageBox.Show("Please Insert Param4", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 4)
                {
                    if (editParam5.Text == "")
                    {
                        MessageBox.Show("Please Insert Param5", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 5)
                {
                    if (editParam6.Text == "")
                    {
                        MessageBox.Show("Please Insert Param6", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i == 6)
                {
                    if (editParam7.Text == "")
                    {
                        MessageBox.Show("Please Insert Param7", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

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

        public StringBuilder GetPosNames(MainWindow.PROCESS_CMD cmd)
        {
            StringBuilder str = new StringBuilder();

            switch (cmd)
            {
                case MainWindow.PROCESS_CMD.MOV_X:
                    str = GetNames(mainWindow.config.Pos_AxisX);
                    break;
                case MainWindow.PROCESS_CMD.MOV_Y:
                    str = GetNames(mainWindow.config.Pos_AxisY);
                    break;
                case MainWindow.PROCESS_CMD.MOV_Z:
                    str = GetNames(mainWindow.config.Pos_AxisZ);
                    break;
                case MainWindow.PROCESS_CMD.MOV_GRIPPER:
                    str = GetNames(mainWindow.config.Pos_AxisGripper);
                    break;
                case MainWindow.PROCESS_CMD.MOV_PIPETT:
                    str = GetNames(mainWindow.config.Pos_AxisPipett);
                    break;
                case MainWindow.PROCESS_CMD.MOV_TOOL_XY:
                case MainWindow.PROCESS_CMD.MOV_T_PNT:
                    str = GetWorldNames(mainWindow.config.Pos_WorldPos);
                    break;
                case MainWindow.PROCESS_CMD.SEL_TOOL:
                    str = GetToolOffsetNames(mainWindow.config.ToolOffset);
                    break;
            }
            return str;
        }

        public StringBuilder GetToolNames(MainWindow.PROCESS_CMD cmd)
        {
            StringBuilder str = new StringBuilder();

            switch (cmd)
            {
                case MainWindow.PROCESS_CMD.MOV_TOOL_XY:
                    str = GetToolOffsetNames(mainWindow.config.ToolOffset);
                    break;
                case MainWindow.PROCESS_CMD.SEL_TOOL:
                    str = GetToolOffsetNames(mainWindow.config.ToolOffset);
                    break;
            }
            return str;
        }

        public StringBuilder GetNames(List<DefinePos> defPos)
        {
            StringBuilder str = new StringBuilder();
            foreach (DefinePos pos in defPos)
                str.Append(pos.Name).AppendLine();
            return str;
        }

        public StringBuilder GetWorldNames(List<DefineWorldPos> defPos)
        {
            StringBuilder str = new StringBuilder();
            String IdxValue = strFindIdx as string;

            foreach (DefineWorldPos pos in defPos)
            {
                if(IdxValue != null)
                {
                    if (pos.Idx.Contains(IdxValue))
                    {
                        str.Append(pos.Idx).AppendLine();
                    }
                }
                else
                {
                    str.Append(pos.Idx).AppendLine();
                }
            }
            
            return str;
        }

        public StringBuilder GetToolOffsetNames(List<DefineToolOffset> defOffset)
        {
            StringBuilder str = new StringBuilder();

            foreach (DefineToolOffset offset in defOffset)
                str.Append(offset.Name).AppendLine();

            return str;
        }

        private void combo_Tpnt_SelectedValueChanged(object sender, EventArgs e)
        {
            MainWindow.CommandParam param = new MainWindow.CommandParam();
            String strTpnt = combo_cmd3.SelectedItem as string;

            bTpntSortSelectFlag = true;

            if (strTpnt == "All")
            {
                strFindIdx = "";
            }
            else if (strTpnt == "Normal")
            {
                strFindIdx = "TP";
            }
            else if (strTpnt == "Cooler")
            {
                strFindIdx = "CP";
            }
            else if (strTpnt == "Tip")
            {
                strFindIdx = "IP";
            }
            else if (strTpnt == "Tube")
            {
                strFindIdx = "RP";
            }
            else if (strTpnt == "Centrifuge")
            {
                strFindIdx = "FP";
            }

            SetParam((MainWindow.PROCESS_CMD)combo_cmd1.SelectedIndex, param, null);
            combo_param1_SelectedValueChanged(sender, e);
        }
        
        public String strBaseTool;
        public String strTrgTool;

        private void combo_Tool_SelectedValueChanged(object sender, EventArgs e)
        {
            if (mainWindow == null)
                return;

            //MainWindow.CommandParam param = new MainWindow.CommandParam();
            MainWindow.PROCESS_CMD cmd = mainWindow.KeyByValue(MainWindow.dicCmd, combo_cmd1.Text);

            if(cmd == MainWindow.PROCESS_CMD.MOV_TOOL_XY)
                strBaseTool = combo_cmd5.SelectedItem as string;
            else
                strBaseTool = combo_cmd2.SelectedItem as string;

            strTrgTool = combo_cmd4.SelectedItem as string;

            if (combo_cmd4.Focused != true)
                return;

            double BaseX = 0;
            double BaseY = 0;
            double BaseZ = 0;

            // Base Tool Selection
            if (strBaseTool == "No Tool")
            {
                BaseX = 0.0;
                BaseY = 0.0;
                BaseZ = 0.0;
            }
            else if (strBaseTool == "1mL")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_1ml);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_1ml);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_1ml);
            }
            else if (strBaseTool == "10uL")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_1ml);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_1ml);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_10ul);
            }
            else if (strBaseTool == "300uL")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_1ml);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_1ml);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_300ul);
            }
            else if (strBaseTool == "5mL")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_5ml);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_5ml);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_5ml);
            }
            else if (strBaseTool == "Laser")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_laser);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_laser);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_laser);
            }
            else if (strBaseTool == "Gripper")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_gripper);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_gripper);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_gripper);
            }
            else if (strBaseTool == "Calib Tip")
            {
                BaseX = double.Parse(mainWindow.config.Pipett_offsetX_1ml);
                BaseY = double.Parse(mainWindow.config.Pipett_offsetY_1ml);
                BaseZ = double.Parse(mainWindow.config.Pipett_offsetZ_CalibTip);
            }

            // Target Tool Selection
            if (strTrgTool == "No Tool")
            {
                strToolIdx = "";
                offset_val.dbX = 0.0;
                offset_val.dbY = 0.0;
                offset_val.dbZ = 0.0;
            }
            else if (strTrgTool == "1mL")
            {
                strToolIdx = "1mL";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_1ml) - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_1ml) - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_1ml) - BaseZ;
            }
            else if (strTrgTool == "10uL")
            {
                strToolIdx = "10uL";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_1ml)  - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_1ml)  - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_10ul) - BaseZ;
            }
            else if (strTrgTool == "300uL")
            {
                strToolIdx = "300uL";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_1ml)   - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_1ml)   - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_300ul) - BaseZ;
            }
            else if (strTrgTool == "5mL")
            {
                strToolIdx = "5mL";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_5ml) - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_5ml) - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_5ml) - BaseZ;
            }
            else if (strTrgTool == "Laser")
            {
                strToolIdx = "Laser";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_laser) - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_laser) - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_laser) - BaseZ;
            }
            else if (strTrgTool == "Gripper")
            {
                strToolIdx = "Gripper";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_gripper) - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_gripper) - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_gripper) - BaseZ;
            }
            else if (strTrgTool == "Calib Tip")
            {
                strToolIdx = "Calib Tip";
                offset_val.dbX = double.Parse(mainWindow.config.Pipett_offsetX_1ml) - BaseX;
                offset_val.dbY = double.Parse(mainWindow.config.Pipett_offsetY_1ml) - BaseY;
                offset_val.dbZ = double.Parse(mainWindow.config.Pipett_offsetZ_CalibTip) - BaseZ;
            }

            offset_val.SelectedTool = strToolIdx;
            bToolSelectFlag = true;

            //SetParam((MainWindow.PROCESS_CMD)combo_cmd1.SelectedIndex, param, null);
            combo_param1_SelectedValueChanged(sender, e);
            bToolSelectFlag = false;
        }

        public void SetLabel(MainWindow.PROCESS_CMD cmd, MainWindow.CommandParam param)
        {
            if (mainWindow == null)
                return;

            label_title.Text = MainWindow.dicCmd[cmd].strCmd1;

            HideLabel();
            HideParam();

            switch (cmd)
            {
                case MainWindow.PROCESS_CMD.MOV_X:
                case MainWindow.PROCESS_CMD.MOV_Y:
                case MainWindow.PROCESS_CMD.MOV_Z:
                case MainWindow.PROCESS_CMD.MOV_GRIPPER:
                case MainWindow.PROCESS_CMD.MOV_PIPETT:
                    combo_cmd2.DataSource = GetPosNames(cmd).ToString().Split(new[] { Environment.NewLine }, 
                                                                              StringSplitOptions.RemoveEmptyEntries);
                    label_cmd2.Text = "Teach Point";
                    label_cmd4.Text = "Tool";

                    combo_cmd2.Text = param.strCmd2;
                    combo_cmd2.Show();
                    label_cmd2.Show();
                    combo_cmd3.Hide();
                    label_cmd4.Hide();
                    combo_cmd4.Hide();
                    combo_cmd5.Hide();

                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5 = "";
                    editParam6.Text = param.param6 = "";
                    editParam7.Text = param.param7 = "";
                    break;
                case MainWindow.PROCESS_CMD.MOV_Z_AXES:
                    combo_cmd2.DataSource = null;
                    combo_cmd4.DataSource = null;
                    combo_cmd2.Text = "";
                    combo_cmd4.Text = "";
                    combo_cmd2.Hide();
                    label_cmd2.Hide();
                    combo_cmd3.Hide();
                    label_cmd4.Hide();
                    combo_cmd4.Hide();
                    combo_cmd5.Hide();

                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5;
                    editParam6.Text = param.param6 = "";
                    editParam7.Text = param.param7 = "";
                    break;
                case MainWindow.PROCESS_CMD.MOV_T_PNT:
                    combo_cmd2.DataSource = GetPosNames(cmd).ToString().Split(new[] { Environment.NewLine },
                                                                              StringSplitOptions.RemoveEmptyEntries);

                    if (bTpntSortSelectFlag == true)
                    {
                        combo_cmd2.Text = ((string[])combo_cmd2.DataSource)[0];
                        bTpntSortSelectFlag = false;
                    }
                    else
                    {
                        combo_cmd2.Text = param.strCmd2;
                    }
                    
                    label_cmd2.Text = "Teach Point";
                    label_cmd4.Text = "Tool";

                    combo_cmd2.Show();
                    label_cmd2.Show();
                    combo_cmd3.Show();
                    label_cmd4.Hide();
                    combo_cmd4.Hide();
                    combo_cmd5.Hide();

                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5;
                    editParam6.Text = param.param6;
                    editParam7.Text = param.param7;
                    break;
                case MainWindow.PROCESS_CMD.MOV_TOOL_XY:
                    combo_cmd2.DataSource = GetPosNames(cmd).ToString().Split(new[] { Environment.NewLine }, 
                                                                              StringSplitOptions.RemoveEmptyEntries);

                    string[] strSplit = new string[2];
                    if (param.strCmd2 != null)
                        strSplit = param.strCmd2.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        strSplit = null;

                    if (bTpntSortSelectFlag == true)
                    {
                        //combo_cmd2.Text = ((string[])combo_cmd2.DataSource)[0];
                        combo_cmd2.Text = ((string[])combo_cmd2.DataSource)[combo_cmd2.SelectedIndex];
                        bTpntSortSelectFlag = false;
                        editParam2.Text = param.param2;
                        editParam3.Text = param.param3;
                    }
                    else
                    {
                        if(strSplit != null)
                            combo_cmd2.Text = strSplit[0];
                        else
                            combo_cmd2.Text = param.strCmd2;

                        editParam2.Text = param.param2;
                        editParam3.Text = param.param3;
                        editParam4.Text = param.param4;
                        editParam5.Text = param.param5;
                        editParam6.Text = param.param6;
                        editParam7.Text = param.param7;
                    }

                    combo_cmd4.DataSource = GetToolNames(cmd).ToString().Split(new[] { Environment.NewLine }, 
                                                                               StringSplitOptions.RemoveEmptyEntries);
                    if (bToolSelectFlag == true)
                    {
                        //combo_cmd4.Text = ((string[])combo_cmd4.DataSource)[0];
                        combo_cmd4.Text = ((string[])combo_cmd4.DataSource)[combo_cmd4.SelectedIndex];
                        bToolSelectFlag = false;
                        editParam4.Text = param.param4;
                        editParam5.Text = param.param5;
                        editParam6.Text = param.param6;
                        editParam7.Text = param.param7;
                    }
                    else
                    {
                        if (strSplit != null && strSplit.Length > 1)
                            combo_cmd4.Text = strSplit[1];
                        else
                            combo_cmd4.Text = param.strCmd2;
                        bToolSelectFlag = false;

                        editParam2.Text = param.param2;
                        editParam3.Text = param.param3;
                        editParam4.Text = param.param4;
                        editParam5.Text = param.param5;
                        editParam6.Text = param.param6;
                        editParam7.Text = param.param7;
                    }

                    combo_cmd5.DataSource = GetToolNames(cmd).ToString().Split(new[] { Environment.NewLine },
                                                                               StringSplitOptions.RemoveEmptyEntries);
                    if (bToolSelectFlag == true)
                    {
                        combo_cmd5.Text = ((string[])combo_cmd5.DataSource)[combo_cmd5.SelectedIndex];
                        bToolSelectFlag = false;
                    }
                    else
                    {
                        combo_cmd5.Text = param.strCmd2;
                        bToolSelectFlag = false;
                    }

                    label_cmd2.Text = "Teach Point";
                    label_cmd4.Text = "Tool";

                    combo_cmd2.Show();
                    label_cmd2.Show();
                    combo_cmd3.Show();
                    label_cmd4.Show();
                    combo_cmd4.Show();
                    combo_cmd5.Show();

                    editParam1.Text = param.param1;
                    editParam6.Text = param.param6;
                    editParam7.Text = param.param7;
                    break;
                case MainWindow.PROCESS_CMD.SEL_TOOL:
                    combo_cmd4.DataSource = GetToolNames(cmd).ToString().Split(new[] { Environment.NewLine },
                                                                               StringSplitOptions.RemoveEmptyEntries);
                    //string[] strSplit = new string[2];
                    if (param.strCmd2 != null)
                        strSplit = param.strCmd2.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        strSplit = null;

                    if (bToolSelectFlag == true)
                    {
                        combo_cmd4.Text = ((string[])combo_cmd4.DataSource)[combo_cmd4.SelectedIndex];
                        bToolSelectFlag = false;
                    }
                    else
                    {
                        if (strSplit != null)
                            combo_cmd4.Text = strSplit[1];
                        else
                            combo_cmd4.Text = param.strCmd2;
                        bToolSelectFlag = false;
                    }

                    combo_cmd2.DataSource = GetToolNames(cmd).ToString().Split(new[] { Environment.NewLine },
                                                                               StringSplitOptions.RemoveEmptyEntries);
                    if (bToolSelectFlag == true)
                    {
                        combo_cmd2.Text = ((string[])combo_cmd2.DataSource)[combo_cmd2.SelectedIndex];
                        bToolSelectFlag = false;
                    }
                    else
                    {
                        if (strSplit != null)
                            combo_cmd2.Text = strSplit[0];
                        else
                            combo_cmd2.Text = param.strCmd2;
                        bToolSelectFlag = false;
                    }

                    label_cmd2.Text = "Base";
                    label_cmd4.Text = "Target";

                    combo_cmd2.Show();
                    label_cmd2.Show();
                    combo_cmd3.Hide();
                    label_cmd4.Show();
                    combo_cmd4.Show();
                    combo_cmd5.Hide();

                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5 = "";
                    editParam6.Text = param.param6 = "";
                    editParam7.Text = param.param7 = "";
                    break;
                default:
                    combo_cmd2.DataSource = null;
                    combo_cmd4.DataSource = null;
                    combo_cmd2.Text = "";
                    combo_cmd4.Text = "";
                    combo_cmd2.Hide();
                    label_cmd2.Hide();
                    combo_cmd3.Hide();
                    label_cmd4.Hide();
                    combo_cmd4.Hide();
                    combo_cmd5.Hide();
                    break;
            }

            for (int i = 0; i < MainWindow.dicCmd[cmd].param_cnt; i++)
            {
                switch (i)
                {
                    case 0:
                        Label1.Text = MainWindow.dicCmd[cmd].param1;
                        editParam1.Show();
                        Label1.Show();
                        break;
                    case 1:
                        Label2.Text = MainWindow.dicCmd[cmd].param2;
                        editParam2.Show();
                        Label2.Show();
                        break;
                    case 2:
                        Label3.Text = MainWindow.dicCmd[cmd].param3;
                        editParam3.Show();
                        Label3.Show();
                        break;
                    case 3:
                        Label4.Text = MainWindow.dicCmd[cmd].param4;
                        editParam4.Show();
                        Label4.Show();
                        break;
                    case 4:
                        Label5.Text = MainWindow.dicCmd[cmd].param5;
                        editParam5.Show();
                        Label5.Show();
                        break;
                    case 5:
                        Label6.Text = MainWindow.dicCmd[cmd].param6;
                        editParam6.Show();
                        Label6.Show();
                        break;
                    case 6:
                        Label7.Text = MainWindow.dicCmd[cmd].param7;
                        editParam7.Show();
                        Label7.Show();
                        break;
                }
            }
        }

        public void SetParam(MainWindow.PROCESS_CMD cmd, MainWindow.CommandParam param, MainWindow mainWin)
        {
            try
            {
                if (mainWin != null)
                {
                    mainWindow = mainWin;
                    combo_cmd1.Text = param.strCmd1;
                    if (param.strCmd1 == "" || param.strCmd1 == null)
                    {
                        combo_cmd1.SelectedIndex = 0;
                    }
                    combo_cmd2.Text = param.strCmd2;
                    combo_cmd3.Text = "All";
                    combo_cmd4.Text = "No Tool";
                    editParam1.Text = param.param1;
                    editParam2.Text = param.param2;
                    editParam3.Text = param.param3;
                    editParam4.Text = param.param4;
                    editParam5.Text = param.param5;
                    editParam6.Text = param.param6;
                    editParam7.Text = param.param7;
                    editSleep.Text = param.sleep;
                    editComment.Text = param.comment;
                    //combo_cmd_SelectedValueChanged(null, null);
                    if (param.enable)
                        radio_enable.Checked = true;
                    else
                        radio_disable.Checked = true;
                }
                else
                {
                    //combo_cmd1.Text = "";
                    //combo_cmd2.Text = "";

                    editParam1.Text = param.param1 = "";
                    editParam2.Text = param.param2 = "";
                    editParam3.Text = param.param3 = "";
                    editParam4.Text = param.param4 = "";
                    editParam5.Text = param.param5 = "";
                    editParam6.Text = param.param6 = "";
                    editParam7.Text = param.param7 = "";
                    editSleep.Text = param.sleep = "";
                    editComment.Text = param.comment = "";
                }

                SetLabel(cmd, param);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);               
            }
        }

        private void combo_cmd_SelectedValueChanged(object sender, EventArgs e)
        {
            //editParam1.Text = combo_cmd.Text;
            MainWindow.CommandParam param = new MainWindow.CommandParam();

            if(param.strCmd1 != null)
            {
                SetParam((MainWindow.PROCESS_CMD)combo_cmd1.SelectedIndex, param, null);
            }
            else
            {
                //combo_cmd1.SelectedIndex = 0;
                SetParam((MainWindow.PROCESS_CMD)combo_cmd1.SelectedIndex, param, null);
            }
        }

        private void combo_param1_SelectedValueChanged(object sender, EventArgs e)
        {
            //MainWindow.CommandParam param = new MainWindow.CommandParam();
            if (mainWindow == null)
                return;

            MainWindow.PROCESS_CMD cmd = mainWindow.KeyByValue(MainWindow.dicCmd, combo_cmd1.Text);
            int worldPosflag = 0;

            switch (cmd)
            {
                case MainWindow.PROCESS_CMD.MOV_X:
                    for (int i = 0; i < mainWindow.config.Pos_AxisX.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_AxisX[i].Name, combo_cmd2.Text) == 0)
                        {
                            param.param1 = mainWindow.config.Pos_AxisX[i].Speed.ToString();
                            param.param2 = mainWindow.config.Pos_AxisX[i].Position.ToString();
                            param.param3 = mainWindow.config.Pos_AxisX[i].Acc.ToString();
                            param.param4 = mainWindow.config.Pos_AxisX[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_Y:
                    for (int i = 0; i < mainWindow.config.Pos_AxisY.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_AxisY[i].Name, combo_cmd2.Text) == 0)
                        {
                            param.param1 = mainWindow.config.Pos_AxisY[i].Speed.ToString();
                            param.param2 = mainWindow.config.Pos_AxisY[i].Position.ToString();
                            param.param3 = mainWindow.config.Pos_AxisY[i].Acc.ToString();
                            param.param4 = mainWindow.config.Pos_AxisY[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_Z:
                    for (int i = 0; i < mainWindow.config.Pos_AxisZ.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_AxisZ[i].Name, combo_cmd2.Text) == 0)
                        {
                            param.param1 = mainWindow.config.Pos_AxisZ[i].Speed.ToString();
                            param.param2 = mainWindow.config.Pos_AxisZ[i].Position.ToString();
                            param.param3 = mainWindow.config.Pos_AxisZ[i].Acc.ToString();
                            param.param4 = mainWindow.config.Pos_AxisZ[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_GRIPPER:
                    for (int i = 0; i < mainWindow.config.Pos_AxisGripper.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_AxisGripper[i].Name, combo_cmd2.Text) == 0)
                        {
                            param.param1 = mainWindow.config.Pos_AxisGripper[i].Speed.ToString();
                            param.param2 = mainWindow.config.Pos_AxisGripper[i].Position.ToString();
                            param.param3 = mainWindow.config.Pos_AxisGripper[i].Acc.ToString();
                            param.param4 = mainWindow.config.Pos_AxisGripper[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_PIPETT:
                    for (int i = 0; i < mainWindow.config.Pos_AxisPipett.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_AxisPipett[i].Name, combo_cmd2.Text) == 0)
                        {
                            param.param1 = mainWindow.config.Pos_AxisPipett[i].Speed.ToString();
                            param.param2 = mainWindow.config.Pos_AxisPipett[i].Position.ToString();
                            param.param3 = mainWindow.config.Pos_AxisPipett[i].Acc.ToString();
                            param.param4 = mainWindow.config.Pos_AxisPipett[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_TOOL_XY:
                    for (int i = 0; i < mainWindow.config.Pos_WorldPos.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_WorldPos[i].Idx, combo_cmd2.Text) == 0)
                        {
                            param.param2 = mainWindow.config.Pos_WorldPos[i].X.ToString();
                            param.param3 = mainWindow.config.Pos_WorldPos[i].Y.ToString();
                        }
                    }
                    for (int i = 0; i < mainWindow.config.ToolOffset.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.ToolOffset[i].Name, combo_cmd4.Text) == 0)
                        {
                            //param.param4 = mainWindow.config.ToolOffset[i].X.ToString();
                            //param.param5 = mainWindow.config.ToolOffset[i].Y.ToString();
                            param.param4 = offset_val.dbX.ToString();
                            param.param5 = offset_val.dbY.ToString();
                        }
                    }
                    worldPosflag = 2;
                    break;
                case MainWindow.PROCESS_CMD.MOV_T_PNT:
                    for (int i = 0; i < mainWindow.config.Pos_WorldPos.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.Pos_WorldPos[i].Idx, combo_cmd2.Text) == 0)
                        {
                            param.param2 = mainWindow.config.Pos_WorldPos[i].X.ToString();
                            param.param3 = mainWindow.config.Pos_WorldPos[i].Y.ToString();
                            param.param4 = mainWindow.config.Pos_WorldPos[i].Z.ToString();
                            param.param5 = mainWindow.config.Pos_WorldPos[i].Gripper.ToString();
                            param.param6 = mainWindow.config.Pos_WorldPos[i].Pipett.ToString();
                        }
                    }
                    worldPosflag = 1;
                    break;
                case MainWindow.PROCESS_CMD.SEL_TOOL:
                    for (int i = 0; i < mainWindow.config.ToolOffset.Count; i++)
                    {
                        if (String.Compare(mainWindow.config.ToolOffset[i].Name, combo_cmd4.Text) == 0)
                        {
                            param.strCmd2 = mainWindow.config.ToolOffset[i].Name.ToString();
                            param.param1 = mainWindow.config.ToolOffset[i].Z_Dist.ToString();
                            //param.param2 = mainWindow.config.ToolOffset[i].X.ToString();
                            //param.param3 = mainWindow.config.ToolOffset[i].Y.ToString();
                            //param.param4 = mainWindow.config.ToolOffset[i].Z.ToString();
                            param.param2 = offset_val.dbX.ToString();
                            param.param3 = offset_val.dbY.ToString();
                            param.param4 = offset_val.dbZ.ToString();
                        }
                    }
                    worldPosflag = 3;
                    break;
                default:
                    break;
            }

            if(worldPosflag == 1)
            {
                editParam2.Text = param.param2;
                editParam3.Text = param.param3;
                editParam4.Text = param.param4;
                editParam5.Text = param.param5;
                editParam6.Text = param.param6;

                editParam2.Show();
                Label2.Show();
                editParam3.Show();
                Label3.Show();
                editParam4.Show();
                Label4.Show();
                editParam5.Show();
                Label5.Show();
                editParam6.Show();
                Label6.Show();
                
                return;
            }
            else if (worldPosflag == 2)
            {
                editParam2.Text = param.param2;
                editParam3.Text = param.param3;
                editParam4.Text = param.param4;
                editParam5.Text = param.param5;

                editParam2.Show();
                Label2.Show();
                editParam3.Show();
                Label3.Show();
                editParam4.Show();
                Label4.Show();
                editParam5.Show();
                Label5.Show();

                return;
            }
            else if (worldPosflag == 3)
            {
                editParam1.Text = param.param1;
                editParam2.Text = param.param2;
                editParam3.Text = param.param3;
                editParam4.Text = param.param4;
                editParam1.Show();
                Label1.Show();
                editParam2.Show();
                Label2.Show();
                editParam3.Show();
                Label3.Show();
                editParam4.Show();
                Label4.Show();

                return;
            }

            editParam1.Text = param.param1;
            editParam2.Text = param.param2;
            editParam3.Text = param.param3;
            editParam4.Text = param.param4;
            editParam1.Show();
            Label1.Show();
            editParam2.Show();
            Label2.Show();
            editParam3.Show();
            Label3.Show();
            editParam4.Show();
            Label4.Show();
        }

    }
}
