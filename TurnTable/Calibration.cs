using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace CytoDx
{
    //class Calibration
    public partial class MainWindow
    {
        private void btnSetPipettOffset_Click(object sender, EventArgs e)
        {
            config.Pipett_offsetX_5ml = editPipett_offsetX_5ml.Text;
            config.Pipett_offsetY_5ml = editPipett_offsetY_5ml.Text;
            config.Pipett_offsetZ_5ml = editPipett_offsetZ_5ml.Text;
            config.Pipett_offsetX_gripper = editPipett_offsetX_gripper.Text;
            config.Pipett_offsetY_gripper = editPipett_offsetY_gripper.Text;
            config.Pipett_offsetZ_gripper = editPipett_offsetZ_gripper.Text;
            config.Pipett_offsetX_laser = editPipett_offsetX_laser.Text;
            config.Pipett_offsetY_laser = editPipett_offsetY_laser.Text;
            config.Pipett_offsetZ_laser = editPipett_offsetZ_laser.Text;
            config.laser_Z_dist = edit_laser_Z_dist.Text;

            config.Pipett_offsetX_1ml = editPipett_offsetX_1ml.Text;
            config.Pipett_offsetY_1ml = editPipett_offsetY_1ml.Text;
            config.Pipett_offsetZ_1ml = editPipett_offsetZ_1ml.Text;

            config.Pipett_offsetZ_10ul = editPipett_offsetZ_10ul.Text;
            config.Pipett_offsetZ_300ul = editPipett_offsetZ_300ul.Text;
            config.Pipett_offsetZ_CalibTip = editPipett_offsetZ_CalibTip.Text;

            setToolOffsetConfig();
        }

        private void setToolOffsetConfig()
        {
            DefineToolOffset tmp_tool = new DefineToolOffset();

            tmp_tool.Idx = "0";
            tmp_tool.Name = "1mL";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_1ml;
            tmp_tool.Y = config.Pipett_offsetY_1ml;
            tmp_tool.Z = config.Pipett_offsetZ_1ml;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "1";
            tmp_tool.Name = "10uL";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_1ml;
            tmp_tool.Y = config.Pipett_offsetY_1ml;
            tmp_tool.Z = config.Pipett_offsetZ_10ul;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "2";
            tmp_tool.Name = "300uL";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_1ml;
            tmp_tool.Y = config.Pipett_offsetY_1ml;
            tmp_tool.Z = config.Pipett_offsetZ_300ul;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "3";
            tmp_tool.Name = "5mL";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_5ml;
            tmp_tool.Y = config.Pipett_offsetY_5ml;
            tmp_tool.Z = config.Pipett_offsetZ_5ml;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "4";
            tmp_tool.Name = "Gripper";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_gripper;
            tmp_tool.Y = config.Pipett_offsetY_gripper;
            tmp_tool.Z = config.Pipett_offsetZ_gripper;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "5";
            tmp_tool.Name = "Laser";
            tmp_tool.Z_Dist = config.laser_Z_dist;
            tmp_tool.X = config.Pipett_offsetX_laser;
            tmp_tool.Y = config.Pipett_offsetY_laser;
            tmp_tool.Z = config.Pipett_offsetZ_laser;

            config.ToolOffset.Add(tmp_tool);

            tmp_tool.Idx = "6";
            tmp_tool.Name = "Calib Tip";
            tmp_tool.Z_Dist = "0";
            tmp_tool.X = config.Pipett_offsetX_1ml;
            tmp_tool.Y = config.Pipett_offsetY_1ml;
            tmp_tool.Z = config.Pipett_offsetZ_CalibTip;

            config.ToolOffset.Add(tmp_tool);
            SetToolOffsetFromConfig();
        }

        private void btnSetHomeMoveParam_Click(object sender, EventArgs e)
        {
            config.HomeSearchSpd_X = editHomeSearchSpd_X.Text;
            config.HomeSearchSpd_Y = editHomeSearchSpd_Y.Text;
            config.HomeSearchSpd_Z = editHomeSearchSpd_Z.Text;
            config.HomeSearchSpd_Grip = editHomeSearchSpd_Grip.Text;
            config.HomeSearchSpd_Ham = editHomeSearchSpd_Ham.Text;
            config.HomeSearchSpd_Cover = editHomeSearchSpd_Cover.Text;
            config.HomeSearchSpd_Servo = editHomeSearchSpd_Servo.Text;

            config.HomeOffsetPos_X = editHomeOffsetPos_X.Text;
            config.HomeOffsetPos_Y = editHomeOffsetPos_Y.Text;
            config.HomeOffsetPos_Z = editHomeOffsetPos_Z.Text;
            config.HomeOffsetPos_Grip = editHomeOffsetPos_Grip.Text;
            config.HomeOffsetPos_Ham = editHomeOffsetPos_Ham.Text;
            config.HomeOffsetPos_Cover = editHomeOffsetPos_Cover.Text;
            config.HomeOffsetPos_Servo = editHomeOffsetPos_Servo.Text;

            SetStepMotorHomeMoveParam();
        }

        private void btnSetFastZMoveParam_Click(object sender, EventArgs e)
        {
            config.FastMoveSpd_ZAxis = editFastMoveSpd_ZAxis.Text;
            config.FastMoveSpd_GripAxis = editFastMoveSpd_GripAxis.Text;
            config.FastMoveSpd_HamAxis = editFastMoveSpd_HamAxis.Text;

            config.FastMovePos_ZAxis = editFastMovePos_ZAxis.Text;
            config.FastMovePos_GripAxis = editFastMovePos_GripAxis.Text;
            config.FastMovePos_HamAxis = editFastMovePos_HamAxis.Text;
        }
    }
}
