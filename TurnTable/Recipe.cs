using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MvCamCtrl.NET;
//using OpenCvSharp;
//using OpenCvSharp.Extensions;
//using OpenCvSharp.UserInterface;
//using OpenCvSharp.Blob;
//using OpenCvSharp.CPlusPlus;

namespace CytoDx
{
    public partial class MainWindow
    {
        //public MainWindow.TOOL_OFFSET Tool_Offset_Val = new MainWindow.TOOL_OFFSET();

        public static Dictionary<PROCESS_CMD, CommandParam> dicCmd = new Dictionary<PROCESS_CMD, CommandParam>
        {
            {PROCESS_CMD.SPIN,
              new CommandParam {strCmd1=PROCESS_CMD.SPIN.ToString(),
              param_cnt=2, param1="Stop=0/Start=1", param2="Duration [sec]", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.SPIN_PARAM,
              new CommandParam {strCmd1=PROCESS_CMD.SPIN_PARAM.ToString(),
              param_cnt=3, param1="RPM [50~4000]", param2="Acceleration [sec]", param3="Deceleration [sec]", param4="",
              param5="", param6="", param7=""}},

            {PROCESS_CMD.STEP_MOVEA,
              new CommandParam {strCmd1=PROCESS_CMD.STEP_MOVEA.ToString(),
              param_cnt = 6, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "ABS=0/REL=1", param3 = "Speed", param4 = "Position", 
              param5 = "Acc", param6 = "Dec", param7 = ""} },
            
            {PROCESS_CMD.MOV_T_PNT,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_T_PNT.ToString(),
              param_cnt=7, param1 = "Speed", param2 = "X Position", param3 = "Y Position", param4 = "Z Position" ,
              param5 = "Gripper Position", param6 = "Pipett Position", param7 = "None=0/Z MoveUp=1"}},

            {PROCESS_CMD.MOV_Z_AXES,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_Z_AXES.ToString(),
              param_cnt=5, param1 = "Speed", param2 = "Z Pos", param3 = "Gripper Pos", param4 = "Ham Pos" ,
              param5 = "None=0/Z MoveUp=1", param6 = "", param7 = ""}},

            {PROCESS_CMD.SEL_TOOL,
              new CommandParam {strCmd1=PROCESS_CMD.SEL_TOOL.ToString(),
              param_cnt=4, param1 = "Z Distance", param2 = "Offset X", param3 = "Offset Y", param4 = "Offset Z" ,
              param5 = "", param6 = "", param7 = ""}},

            {PROCESS_CMD.MOV_TOOL_XY,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_TOOL_XY.ToString(),
              param_cnt=7, param1 = "Speed", param2 = "X Position", param3 = "Y Position", param4 = "X ToolOffset" ,
              param5 = "Y ToolOffset", param6 = "X Offset", param7 = "Y Offset"}},

            {PROCESS_CMD.PELTIER,
              new CommandParam {strCmd1=PROCESS_CMD.PELTIER.ToString(),
              param_cnt = 5, param1 = "Write=0/Read=1", param2 = "Pelt Stop=0/Run=1", param3 = "Pelt Temp",
              param4 = "FAN ON Temp", param5 = "FAN OFF Temp", param6 = "", param7 = ""}},

            {PROCESS_CMD.VALVE,
              new CommandParam {strCmd1=PROCESS_CMD.VALVE.ToString(),
              param_cnt=1, param1="Open=1/Close=0", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.LIGHT,
              new CommandParam {strCmd1=PROCESS_CMD.LIGHT.ToString(),
              param_cnt=2, param1="Room ON=1/OFF=0", param2="Chamber ON=1/OFF=0",
              param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.SPIN_POS,
              new CommandParam {strCmd1=PROCESS_CMD.SPIN_POS.ToString(),
              param_cnt=1, param1="Ch1=0/CDown1=1/Ch2=2/CDown2=3", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.COVER,
              new CommandParam {strCmd1=PROCESS_CMD.COVER.ToString(),
              param_cnt=1, param1="Open=0/Close=1", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.READ_LDCELL,
              new CommandParam {strCmd1=PROCESS_CMD.READ_LDCELL.ToString(),
              param_cnt=1, param1="OFF=0/READ=1", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.READ_FLOW,
              new CommandParam {strCmd1=PROCESS_CMD.READ_FLOW.ToString(),
              param_cnt=1, param1="STOP=0/START=1", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.READ_LASER,
              new CommandParam {strCmd1=PROCESS_CMD.READ_LASER.ToString(),
              param_cnt=1, param1="OFF=0/READ=1", param2="", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.PIPETT_TRI,
              new CommandParam {strCmd1=PROCESS_CMD.PIPETT_TRI.ToString(),
              param_cnt = 7, param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(0~1600,if Abs=1)", param3 = "No=0/Asp=1/Disp=2",
              param4 = "Follow NA=0/1.5=1/15=2/50=3", param5 = "FlowRate(0.01~22.85 mL/sec)", param6 = "Vol(0~5mL)", param7 = "Offset Vol(0~0.5mL)"}},

            {PROCESS_CMD.PIPETT_HAM_DRY,
              new CommandParam {strCmd1=PROCESS_CMD.PIPETT_HAM_DRY.ToString(),
              param_cnt = 7, param1 = "No=0/PInit=1/TInit=2", param2 = "No=0/TipInsert=1/TipDiscard=2", param3 = "10uL=1/300uL=2/1000uL=3",
              param4 = "No=0/AirAsp=1/AirTrans=2", param5 = "FlowRate 0.01~15(0.1mL/s)", param6 = "Vol(0~1mL)", param7 = "State No=0/Tip=1/cLLD=2"}},

            {PROCESS_CMD.PIPETT_HAM_LIQ,
              new CommandParam {strCmd1=PROCESS_CMD.PIPETT_HAM_LIQ.ToString(),
              param_cnt = 7, param1 = "Asp=1/Disp=2/ADC=3/MA=4/MD=5", param2 = "Follow NA=0/1.5=1/15=2/50=3(ml)", param3 = "FlowRate 0.01~15(0.1mL/s)",
              param4 = "Asp/Disp Vol(0~1.1mL)", param5 = "StopBackVol(0~0.0325mL)", param6 = "OverAspVol(0~1mL)", param7 = "StopSpd(0.01 ~ 15 mL/sec)"}},

            {PROCESS_CMD.FIND_SURFACE,
              new CommandParam {strCmd1=PROCESS_CMD.FIND_SURFACE.ToString(),
              param_cnt = 6, param1 = "Start=1/Stop=2", param2 = "Sensitivity", param3 = "Dir Plus=1/Minus=2", 
              param4 = "Axis X=1/Y=2/Z=3", param5 = "Speed", param6 = "Max Position", param7 = ""}},
            
            {PROCESS_CMD.SEL_PUMP_PORT,
              new CommandParam {strCmd1=PROCESS_CMD.SEL_PUMP_PORT.ToString(),
              param_cnt=2, param1 = "CCW=0/CW=1", param2 = "Port No(1~6)", param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""}},

            {PROCESS_CMD.ACT_PUMP,
              new CommandParam {strCmd1=PROCESS_CMD.ACT_PUMP.ToString(),
              param_cnt=6, param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(if Abs=1)", param3 = "No=0/Asp=1/Disp=2", 
              param4 = "FlowRate(0.01~1.5 mL/sec)", param5 = "Load Vol(0~5mL)", param6 = "Offset Vol(0~1mL)", param7 = "" }},

            {PROCESS_CMD.CAMERA,
              new CommandParam {strCmd1=PROCESS_CMD.CAMERA.ToString(),
              param_cnt=2, param1="0=SpinCAM/1=StopCAM", param2="Close=0/Open=1", param3="", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.MOT_HOLD,
              new CommandParam {strCmd1=PROCESS_CMD.MOT_HOLD.ToString(),
              param_cnt = 2, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "FREE=0/HOLD=1",
              param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""} },

            {PROCESS_CMD.MOT_STOP,
              new CommandParam {strCmd1=PROCESS_CMD.MOT_STOP.ToString(),
              param_cnt = 2, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "FREE=0/HOLD=1",
               param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""} },

            {PROCESS_CMD.MOV_X,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_X.ToString(),
              param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
              param5="", param6="", param7=""}},

            {PROCESS_CMD.MOV_Y,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_Y.ToString(),
              param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
              param5="", param6="", param7=""}},

            {PROCESS_CMD.MOV_Z,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_Z.ToString(),
              param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
              param5="", param6="", param7=""}},

            {PROCESS_CMD.MOV_GRIPPER,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_GRIPPER.ToString(),
              param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
              param5="", param6="", param7=""}},

            {PROCESS_CMD.MOV_PIPETT,
              new CommandParam {strCmd1=PROCESS_CMD.MOV_PIPETT.ToString(),
              param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
              param5="", param6="", param7=""}},

            {PROCESS_CMD.RECORD,
              new CommandParam {strCmd1=PROCESS_CMD.RECORD.ToString(),
              param_cnt=3, param1="Record FPS", param2="Stop=0/Start=1", param3="Image=0/AVI=1", param4="", param5="", param6="", param7=""}},

            {PROCESS_CMD.SLEEP,
              new CommandParam {strCmd1=PROCESS_CMD.SLEEP.ToString(),
              param_cnt=1, param1="Time [ms]", param2="", param3="", param4="", param5="", param6="", param7=""}},
        };

        public bool ExeRecipe(CommandParam param)
        {
            bool retVal = true;
            int i = 0;
            offset_val = patternInput.offset_val;

            try
            {
                if (bStopFlag)
                    return false;
                PROCESS_CMD cmd = KeyByValue(dicCmd, param.strCmd1);
                switch (cmd)
                {
                    case PROCESS_CMD.SPIN:
                        //param_cnt=2, param1="Stop=0/Start=1", param2="Duration [sec]",
                        //param3="", param4="", param5="", param6="", param7=""}},
                        EccentricClear();
                        if (SerCmd_Spin((CMD)int.Parse(param.param1), int.Parse(param.param2)) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Command Spin Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            if (int.Parse(param.param1) == 1)
                            {
                                CurrentRecipeCommand = "SPIN";
                                DisplayStatusMessage("Spin Move Running", TEST.RUNNING);
                            }
                            else if (int.Parse(param.param1) == 0)
                            {
                                CurrentRecipeCommand = "";
                                DisplayStatusMessage("Spin Move Stopped", TEST.PASS);
                            }
                        }

                        //WaitForServoStop();

                        // waitTime 구조 변경 필요
                        //int waitTime = param.param1 == "1" ? SpinUpTime : SpinDownTime;
                        //for (i = 0; i < waitTime; i++)
                        //{
                        //    if (bStopFlag)
                        //        return false;
                        //    Thread.Sleep(1000);
                        //    Application.DoEvents();
                        //}
                        break;
                    case PROCESS_CMD.SPIN_PARAM:
                        //param_cnt=3, param1="RPM [50~4000]", param2="Acceleration [sec]", param3="Deceleration [sec]", 
                        //param4="", param5 = "", param6 = "", param7 = ""
                        if (SerCmd_SetParameter(Direction.CCW, int.Parse(param.param1), 0,
                            int.Parse(param.param2), int.Parse(param.param3)) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("SetSpinParameter Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("SetSpinParameter Done", TEST.PASS);

                            // for test variables status
                            //label_Recipe_PeltSetTemp.Text = param.param1;
                            //label_Recipe_PeltChamberTemp.Text = param.param2;
                            //label_Recipe_PeltPeltierTemp.Text = param.param3;
                            //if (int.Parse(param.param1) > 1)
                            //    checkedListBox_TipPresence.SetItemChecked(0, true);
                            //else
                            //    checkedListBox_StateLLD.SetItemChecked(1, true);
                        }
                        break;
                    case PROCESS_CMD.STEP_MOVEA:
                        //param_cnt = 6, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "ABS=0/REL=1", param3 = "Speed", 
                        //param4 = "Position", param5 = "Acc", param6 = "Dec", param7 = ""}},
                        if (MoveStepMotor(cmd: STEP_CMD.MOVE, motorNum: (MOTOR)int.Parse(param.param1),
                            speed: int.Parse(param.param3), position: double.Parse(param.param4), acc: int.Parse(param.param5), dec: int.Parse(param.param6),
                            opt1: (POS_OPT)int.Parse(param.param2), opt2: HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Step Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            //WaitForStepStop((MOTOR)int.Parse(param.param1));
                            DisplayStatusMessage("Move Step Motor Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.MOT_HOLD:
                        //param_cnt = 2, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "FREE=0/HOLD=1",
                        //param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""}
                        if (int.Parse(param.param1) == (int)MOTOR.SERVO)
                        {
                            ServoOnOff((HOLD_STATE)int.Parse(param.param2));
                        }
                        else
                        {
                            if (MoveStepMotor(cmd: STEP_CMD.HOLD, motorNum: (MOTOR)int.Parse(param.param1),
                            0, 0, 0, 0, opt1: POS_OPT.NONE, opt2: (HOLD_STATE)int.Parse(param.param2)) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Motor Hold/Free Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Motor Hold/Free Done", TEST.PASS);
                            }
                        }
                        break;
                    case PROCESS_CMD.MOT_STOP:
                        //param_cnt = 2, param1 = "MOT GR=1/PI=2/COV=3/X=4/Y=5/Z=6", param2 = "FREE=0/HOLD=1",
                        //param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""}
                        if (int.Parse(param.param1) == (int)MOTOR.SERVO)
                        {
                            SerCmd_Spin(run: CMD.STOP, 0);
                        }
                        else
                        {
                            if (MoveStepMotor(cmd: STEP_CMD.STOP, motorNum: (MOTOR)int.Parse(param.param1),
                            0, 0, 0, 0, opt1: POS_OPT.NONE, opt2: HOLD_STATE.NONE) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Motor Stop Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Motor Stop Done", TEST.PASS);
                            }
                        }
                        break;
                    case PROCESS_CMD.MOV_X:
                        //param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
                        //param5 = "", param6 = "", param7 = ""}
                        for (i = 0; i < config.Pos_AxisX.Count; i++)
                        {
                            if (String.Compare(config.Pos_AxisX[i].Name, param.strCmd2) == 0)
                            {
                                param.param1 = config.Pos_AxisX[i].Speed.ToString();
                                param.param2 = config.Pos_AxisX[i].Position.ToString();
                                param.param3 = config.Pos_AxisX[i].Acc.ToString();
                                param.param4 = config.Pos_AxisX[i].Dec.ToString();
                            }
                        }
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0,
                            speed: int.Parse(param.param1), position: double.Parse(param.param2),
                            acc: int.Parse(param.param3), dec: int.Parse(param.param4),
                            POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Axis X Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Move Axis X Motor Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.MOV_Y:
                        //param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
                        //param5 = "", param6 = "", param7 = ""}
                        for (i = 0; i < config.Pos_AxisY.Count; i++)
                        {
                            if (String.Compare(config.Pos_AxisY[i].Name, param.strCmd2) == 0)
                            {
                                param.param1 = config.Pos_AxisY[i].Speed.ToString();
                                param.param2 = config.Pos_AxisY[i].Position.ToString();
                                param.param3 = config.Pos_AxisY[i].Acc.ToString();
                                param.param4 = config.Pos_AxisY[i].Dec.ToString();
                            }
                        }
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1,
                            speed: int.Parse(param.param1), position: double.Parse(param.param2),
                            acc: int.Parse(param.param3), dec: int.Parse(param.param4),
                            POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Axis Y Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Move Axis Y Motor Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.MOV_Z:
                        //param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
                        //param5 = "", param6 = "", param7 = ""}
                        for (i = 0; i < config.Pos_AxisZ.Count; i++)
                        {
                            if (String.Compare(config.Pos_AxisZ[i].Name, param.strCmd2) == 0)
                            {
                                param.param1 = config.Pos_AxisZ[i].Speed.ToString();
                                param.param2 = config.Pos_AxisZ[i].Position.ToString();
                                param.param3 = config.Pos_AxisZ[i].Acc.ToString();
                                param.param4 = config.Pos_AxisZ[i].Dec.ToString();
                            }
                        }

                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2,
                            speed: int.Parse(param.param1), position: double.Parse(param.param2),
                            acc: int.Parse(param.param3), dec: int.Parse(param.param4),
                            POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Axis Z Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Move Axis Z Motor Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.MOV_GRIPPER:
                        //param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
                        //param5 = "", param6 = "", param7 = ""}
                        for (i = 0; i < config.Pos_AxisGripper.Count; i++)
                        {
                            if (String.Compare(config.Pos_AxisGripper[i].Name, param.strCmd2) == 0)
                            {
                                param.param1 = config.Pos_AxisGripper[i].Speed.ToString();
                                param.param2 = config.Pos_AxisGripper[i].Position.ToString();
                                param.param3 = config.Pos_AxisGripper[i].Acc.ToString();
                                param.param4 = config.Pos_AxisGripper[i].Dec.ToString();
                            }
                        }
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP,
                            speed: int.Parse(param.param1), position: double.Parse(param.param2),
                            acc: int.Parse(param.param3), dec: int.Parse(param.param4),
                            POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Gripper Axis Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Move Gripper Axis Motor Done", TEST.PASS);

                        }
                        break;
                    case PROCESS_CMD.MOV_PIPETT:
                        //param_cnt=4, param1="Speed", param2="Position", param3="Acc", param4="Dec" ,
                        //param5 = "", param6 = "", param7 = ""}
                        for (i = 0; i < config.Pos_AxisPipett.Count; i++)
                        {
                            if (String.Compare(config.Pos_AxisPipett[i].Name, param.strCmd2) == 0)
                            {
                                param.param1 = config.Pos_AxisPipett[i].Speed.ToString();
                                param.param2 = config.Pos_AxisPipett[i].Position.ToString();
                                param.param3 = config.Pos_AxisPipett[i].Acc.ToString();
                                param.param4 = config.Pos_AxisPipett[i].Dec.ToString();
                            }
                        }
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM,
                            speed: int.Parse(param.param1), position: double.Parse(param.param2),
                            acc: int.Parse(param.param3), dec: int.Parse(param.param4),
                            POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Move Pipett Axis Motor Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Move Pipett Axis Motor Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.MOV_T_PNT:
                        //param1 = "Speed", param2 = "X Position", param3 = "Y Position", param4 = "Z Position" ,
                        //param5 = "Gripper Position", param6 = "Pipett Position", param7 = "None=0/Z MoveUp=1"

                        if (int.Parse(param.param7) == 0)
                        {
                            if (MoveTpnt(param.param1, param.param2, param.param3, param.param4, param.param5, param.param6) == false)
                            {
                                return false;
                            }
                        }
                        else if (int.Parse(param.param7) == 1)
                        {
                            if (FastZ_UpMotion(WAIT_OPT.WAIT) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Teaching Point Z Fast Move Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Teaching Point Z Fast Move Done", TEST.PASS);
                            }
                        }

                        break;
                    case PROCESS_CMD.SEL_TOOL:
                        // param1 = "Z Distance", param2 = "Offset X", param3 = "Offset Y",
                        // param4 = "Offset Z" ,param5 = "", param6 = "", param7 = ""

                        if(SelectToolMoveXYZ(param.param1, param.param2, param.param3, param.param4) == false)
                        {
                            return false;
                        }

                        break;
                    case PROCESS_CMD.MOV_TOOL_XY:
                        //param1 = "Speed", param2 = "X Position", param3 = "Y Position", param4 = "X ToolOffset" ,
                        //param5 = "Y ToolOffset", param6 = "X Offset", param7 = "Y Offset"

                        if (MoveToolXY(param.param1, param.param2, param.param3, param.param4, 
                                       param.param5, param.param6, param.param7) == false)
                        {
                            return false;
                        }

                        break;
                    case PROCESS_CMD.MOV_Z_AXES:
                        // param1 = "Speed", param2 = "Z Pos", param3 = "Gripper Pos", param4 = "Ham Pos" ,
                        // param5 = "None=0/Z MoveUp=1", param6 = "", param7 = ""
                        if (int.Parse(param.param5) == 0)
                        {
                            if (MoveZAxes(param.param1, param.param2, param.param3, param.param4) == false)
                            {
                                return false;
                            }
                        }
                        else if (int.Parse(param.param5) == 1)
                        {
                            if (FastZ_UpMotion(WAIT_OPT.WAIT) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Teaching Point Z Fast Move Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Teaching Point Z Fast Move Done", TEST.PASS);
                            }
                        }
                        break;
                    case PROCESS_CMD.PELTIER:
                        //param_cnt = 5, param1 = "Write=0/Read=1", param2 = "Pelt Stop=0/Run=1", param3 = "Pelt Temp",
                        //param4 = "FAN ON Temp", param5 = "FAN OFF Temp", param6 = "", param7 = ""}
                        if (int.Parse(param.param1) == (int) RW_CMD.WRITE && int.Parse(param.param2) == (int) CMD.RUN)
                        {
                            if(WritePeltier(PELT_CMD.SET_SV, bPeltier: true, float.Parse(param.param3), 0, 0) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Run Peltier Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                WritePeltier(PELT_CMD.SET_FAN, false, 0, float.Parse(param.param4), float.Parse(param.param5), timeout: 120);
                                ReadPeltier(PELT_CMD.CHAMBER_SV);
                                DisplayStatusMessage("Run Peltier Done", TEST.PASS);
                            }
                        }
                        else if (int.Parse(param.param1) == (int)RW_CMD.WRITE && int.Parse(param.param2) == (int)CMD.STOP)
                        {
                            if (WritePeltier(PELT_CMD.SET_SV, bPeltier: false, 0, 0, 0) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Stop Peltier Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                WritePeltier(PELT_CMD.SET_FAN, false, 0, float.Parse(param.param4), float.Parse(param.param5), timeout: 120);
                                ReadPeltier(PELT_CMD.CHAMBER_SV);
                                DisplayStatusMessage("Stop Peltier Done", TEST.PASS);
                            }
                        }
                        else if (int.Parse(param.param1) == (int)RW_CMD.READ)
                        {
                            if (ReadPeltierTemp() != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Read Peltier Temperature Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayOutputMessage("Temp:" + editTempChamber.Text, OUTPUT.DONE);
                                
                                label_Recipe_PeltSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
                                label_Recipe_PeltChamberTemp.Text = PeltMon.dbTempChamber.ToString();
                                label_Recipe_PeltPeltierTemp.Text = PeltMon.dbTempPeltier.ToString();
                                label_Recipe_PeltCoolerTemp.Text = PeltMon.dbTempCooler.ToString();
                            }
                        }
                        break;
                    case PROCESS_CMD.VALVE:
                        //param_cnt = 1, param1 = "Open=1/Close=0", param2 = "", param3 = "",
                        //param4 = "", param5 = "", param6 = "", param7 = ""}
                        if (SerPinchValve((VALVE) int.Parse(param.param1)) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Pinch Operation Valve Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Pinch Operation Valve Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.LIGHT:
                        //param_cnt = 2, param1 = "Room ON=1/OFF=0", param2 = "Chamber ON=1/OFF=0",
                        //param3 = "", param4 = "", param5 = "", param6 = "", param7 = ""}
                        if (RoomLight((Status) int.Parse(param.param1)) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Set Room Light Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Set Room Light Done", TEST.PASS);
                        }
                        if (ChamberLight((Status) int.Parse(param.param2)) != COM_Status.ACK)
                        {
                            DisplayStatusMessage("Set Chamber Light Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            DisplayStatusMessage("Set Chamber Light Done", TEST.PASS);
                        }
                        break;
                    case PROCESS_CMD.SPIN_POS:
                        //param_cnt=1, param1="Ch1=0/CDown1=1/Ch2=2/CDown2=3", param2="", param3="",
                        //param4="", param5="", param6="", param7=""}},
                        
                        // 먼저 로터가 회전하고 있다면 기다렸다가 진행할 것
                        WaitForServoStop();

                        if (SelectRotorPosition((CHAMBER_POS) int.Parse(param.param1)) != COM_Status.ACK)
                        {
                            //ServoOnOff(HOLD_STATE.FREE);  // FREE 상태가 필요할 때를 위해 추가함
                            DisplayStatusMessage("Set Spin Position Error", TEST.FAIL);
                            return false;
                        }
                        else
                        {
                            WaitForServoStop();
                            DisplayStatusMessage("Set Spin Position Done", TEST.PASS);
                        }
                        //Thread.Sleep(500);
                        //ServoOnOff(HOLD_STATE.FREE);  // FREE 상태가 필요할 때를 위해 추가함
                        break;
                    case PROCESS_CMD.COVER:
                        //param_cnt = 1, param1 = "Open=0/Close=1", param2 = "", param3 = "", param4 = "",
                        //param5 = "", param6 = "", param7 = ""}

                        if(CentrifugeCoverOpenClose(param.param1) == false)
                        {
                            return false;
                        }                         
                        break;
                    case PROCESS_CMD.READ_LDCELL:
                        //param_cnt=1, param1="OFF=0/READ=1", param2="", param3="",
                        //param4="", param5="", param6="", param7=""}},
                        if (int.Parse(param.param1) == 1)
                        {
                            checkedListBox_DGM_Weight.SetItemChecked(0, false);  // recipe checkbox
                            checkedListBox_DGM_Weight.SetItemChecked(1, false);

                            if (ReadLoadCell(LOADCELL_CMD.WEIGHT, 0, timeout: 150) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Loadcell Value Read Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayOutputMessage("Weight: "+ label_LoadCellWeightVal.Text, OUTPUT.DONE);
                                if(float.Parse(label_LoadCellWeightVal.Text) > float.Parse(editLoadcellErrWeight.Text))
                                {
                                    checkedListBox_DGM_Weight.SetItemChecked(0, true);
                                    checkedListBox_DGM_Weight.SetItemChecked(1, false);
                                }
                                else
                                {
                                    checkedListBox_DGM_Weight.SetItemChecked(0, false);
                                    checkedListBox_DGM_Weight.SetItemChecked(1, true);
                                }
                            }
                        }
                        break;
                    case PROCESS_CMD.READ_FLOW:
                        //param_cnt=1, param1="STOP=0/START=1", param2="", param3="",
                        //param4="", param5="", param6="", param7=""}},
                        if (int.Parse(param.param1) == 1)
                        {
                            FlowmeterReadStart();
                        }
                        else
                        {
                            FlowmeterReadStop();
                            DisplayOutputMessage("Vol: "+ label_FlowMeterVolumeVal.Text, OUTPUT.DONE);
                            label_Recipe_FlowMeterVolumeVal.Text = label_FlowMeterVolumeVal.Text;
                        }
                        break;
                    case PROCESS_CMD.READ_LASER:
                        //param_cnt=1, param1="OFF=0/READ=1", param2="", param3="",
                        //param4="", param5="", param6="", param7=""}},                        
                        if (int.Parse(param.param1) == 1)
                        {
                            //ReadLaserSensor(SENSOR_CMD.PWR, Status.ON);

                            if (ReadLaserSensor(SENSOR_CMD.GET, Status.NONE) != COM_Status.ACK)
                            {
                                DisplayStatusMessage("Laser Sensor Value Read Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                checkedListBox_LaserSensor.SetItemChecked(0, false);
                                checkedListBox_LaserSensor.SetItemChecked(1, false);

                                if (nLaserDetected == 0)    // detected
                                {
                                    DisplayOutputMessage("Laser: ON", OUTPUT.DONE);
                                    iPrintf("Laser Sensor Detected!");
                                    checkedListBox_LaserSensor.SetItemChecked(1, true);
                                }
                                else
                                {
                                    DisplayOutputMessage("Laser: OFF", OUTPUT.DONE);
                                    iPrintf("Laser Sensor Not Detected!");
                                    checkedListBox_LaserSensor.SetItemChecked(0, true);
                                }
                            }

                            //ReadLaserSensor(SENSOR_CMD.PWR, Status.OFF);
                        }
                        break;
                    case PROCESS_CMD.PIPETT_TRI:
                        //param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(if Abs=1)", param3 = "No=0/Asp=1/Disp=2",
                        //param4 = "Z Follow OFF=0/1.5ml=1/15ml=2/50ml=3", param5 = "FlowRate(0.01~22.85 mL/sec)",
                        //param6 = "Vol(0~5mL)", param7 = "Offset Vol(0~0.5mL)" 

                        if(OperateTricontinentalPipett(param.param1, param.param2, param.param3, 
                                                       param.param4, param.param5, param.param6, param.param7) == false)
                        {
                            return false;
                        }
                        break;
                    case PROCESS_CMD.PIPETT_HAM_DRY:
                        //  param1 = "No=0/PInit=1/TInit=2", param2 = "No=0/TipInsert=1/TipDiscard=2",
                        //  param3 = "10uL=1/300uL=2/1000uL=3", param4 = "No=0/AirAsp=1/AirTrans=2", 
                        //  param5 = "FlowRate 0.01 ~ 15(0.1mL/s)", param6 = "AirVol(0~1mL)", param7 = "State No=0/Tip=1/cLLD=2" 

                        if (OperateHamiltonPipettDry(param.param1, param.param2, param.param3, param.param4, 
                                                    param.param5, param.param6, param.param7) == false)
                        {
                            return false;
                        }
                        break;
                    case PROCESS_CMD.PIPETT_HAM_LIQ:
                        //  param1 = "Asp=1/Disp=2/ADC=3/MA=4/MD=5", param2 = "Follow NA=0/1.5=1/15=2/50=3(ml)",
                        //  param3 = "FlowRate 0.01 ~ 15(0.1mL/s)",param4 = "Asp/Disp Vol(0~1.1mL)", 
                        //  param5 = "StopBackVol(0~0.0325mL)", param6 = "OverAspVol(0~1mL)", param7 = "StopSpd(0.01 ~ 15 mL/sec)" 

                        if (OperateHamiltonPipettLiquid(param.param1, param.param2, param.param3, 
                                                       param.param4, param.param5, param.param6, param.param7) == false)
                        {
                            return false;
                        }
                        break;
                    case PROCESS_CMD.FIND_SURFACE:
                        //param_cnt = 6, param1 = "Start=1/Stop=2", param2 = "Sensitivity", param3 = "Dir Plus=1/Minus=2",
                        //param4 = "Axis X=1/Y=2/Z=3", param5 = "Speed", param6 = "Max Position", param7 = ""

                        if(FindLiquidSurface(param.param1, param.param2, param.param3, 
                                             param.param4, param.param5, param.param6) == false)
                        {
                            return false;
                        }
                        break;
                    case PROCESS_CMD.SEL_PUMP_PORT:
                        //param1 = "CCW=0/CW=1", param2 = "Port No(1~6)", param3 = "",
                        //param4 = "", param5 = "", param6 = "", param7 = "" 
                                                
                        if (int.Parse(param.param1) == (int)Direction.CCW)
                        {
                            RunPer3_TricontinentPump((byte)'O', int.Parse(param.param2), // output port selection
                                         (byte)'?', 0,                              // N/A
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A
                        }
                        else if (int.Parse(param.param1) == (int)Direction.CW)
                        {
                            RunPer3_TricontinentPump((byte)'I', int.Parse(param.param2), // output port selection
                                        (byte)'?', 0,                              // N/A
                                        0,                                         // N/A
                                        (byte)' ', 0);                             // N/A
                        }
                        else
                        {
                            DisplayStatusMessage($"Select Pump Port Parameter Input Error! P1:{param.param1}, P2: {param.param2}", TEST.FAIL);
                            return false;
                        }
                        break;
                    case PROCESS_CMD.ACT_PUMP:
                        //param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(if Abs=1)", param3 = "No=0/Asp=1/Disp=2",
                        //param4 = "FlowRate(0.01~1.5 mL/sec)", param5 = "Load Vol(0~5mL)", param6 = "Offset Vol(0~1mL)", param7 = "" 

                        if(OperateTricontinentSyringePump(param.param1, param.param2, param.param3, 
                                                          param.param4, param.param5, param.param6) == false)
                        {
                            return false;
                        }
                        break;
                    case PROCESS_CMD.CAMERA:
                        //param_cnt=2, param1="0=SpinCAM/1=StopCAM", param2="Close=0/Open=1", param3="",
                        //param4="", param5="", param6="", param7=""}},
                        if (int.Parse(param.param2) == 0)
                        {
                            if (StopGrab() != MyCamera.MV_OK)
                            {
                                DisplayStatusMessage("Stop Grabbing Error", TEST.FAIL);
                                retVal = false;
                            }
                            else
                            {
                                DisplayStatusMessage("Stop Grabbing Done", TEST.PASS);
                            }
                            if (CloseCamera() == false)
                            {
                                DisplayStatusMessage("Close Camer Error", TEST.FAIL);
                                retVal = false;
                            }
                            else
                            {
                                DisplayStatusMessage("Close Camer Done", TEST.PASS);
                            }
                        }
                        if (int.Parse(param.param2) == 1)
                        {
                            if(OpenCamera(int.Parse(param.param1)) == false)
                            {
                                DisplayStatusMessage("Open Camera Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Open Camera Done", TEST.PASS);
                            }
                            //if(SetCameraParam(int.Parse(param.param1)) == false)
                            if (SetCameraParam(int.Parse(param.param1)) == false)
                            {
                                DisplayStatusMessage("Set Camera Parameter Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Set Camera Parameter Done", TEST.PASS);
                            }
                            if(StartGrab() == false)
                            {
                                DisplayStatusMessage("Start Grabbing Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Start Grabbing Done", TEST.PASS);
                            }
                        }
                        break;
                    case PROCESS_CMD.RECORD:
                        //param_cnt=3, param1="Record FPS", param2="Stop=0/Start=1", param3="Image=0/AVI=1",
                        //param4="", param5="", param6="", param7=""}},
                        if (int.Parse(param.param2) == 1)
                        {
                            if (StartRecord(int.Parse(param.param1)) == false)
                            {
                                DisplayStatusMessage("Start Recording Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Start Recording Done", TEST.PASS);
                            }
                        }
                        else if(int.Parse(param.param2) == 0)
                        {
                            if(StopRecord() == false)
                            {
                                DisplayStatusMessage("Stop Recording Error", TEST.FAIL);
                                return false;
                            }
                            else
                            {
                                DisplayStatusMessage("Stop Recording Done", TEST.PASS);
                            }
                        }
                        m_bSaveRecord = param.param3 == "0" ? false : true;
                        break;
                    case PROCESS_CMD.SLEEP:
                        //param_cnt=1, param1="Time [ms]", param2="", param3="",
                        //param4="", param5="", param6="", param7=""}},
                        Thread.Sleep(int.Parse(param.param1));
                        break;

                    default:
                        break;
                }

                if (param.sleep != "" && retVal)
                {
                    for ( i = 0; i < int.Parse(param.sleep); i++)
                    {
                        if (bStopFlag)
                            return false;
                        Thread.Sleep(1);
                        Application.DoEvents(); // 실행 속도 개선을 위해 테스트 필요
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
                return false;
            }
            return retVal;
        }

        public bool WaitForServoStop()
        {
            int cnt = 0;
            double dbCurPosServo_old = 0;
            double diffPosServo = 0;
            double diffPosServo_old = 0;

            do
            {
                if (cnt >= 200) break;
                bMotionDoneWait = true;

                ReadMotorPosition(true, bSilent: true);
                Thread.Sleep(200);
                ServoMonitor(MotorMon.RPM, bSilent: true);
                Thread.Sleep(200);

                diffPosServo = Math.Abs(CurrentPos.Servo - dbCurPosServo_old);

                //iPrintf(string.Format("Servo Moving: {0} ({1})", diffPosServo, cnt));
                cnt++;

                dbCurPosServo_old = CurrentPos.Servo;
                diffPosServo_old = diffPosServo;

                if (SensorStatus.Alarm)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    DisplayStatusMessage("[Wait for Servo Stop] System Alarm !!!", TEST.FAIL);
                    EnableControls(true);
                    break;
                }

                if (!Serial.IsOpen || bStopFlag == true)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    if (Serial.IsOpen == false)
                        DisplayStatusMessage("Serial Not Opened!!!", TEST.FAIL);
                    if (bStopFlag == true)
                        DisplayStatusMessage("Stop State Active!!!", TEST.FAIL);
                    break;
                }
            } while (diffPosServo > 1 && diffPosServo_old > 1);

            //ReadMotorPosition(true, bSilent: true);
            //Thread.Sleep(100);
            bMotionDoneWait = false;
            iPrintf(string.Format("[Exit] Servo Moving: {0} ({1})", diffPosServo, cnt));

            return true;
        }
        
        public bool WaitForStepStop(MOTOR Axis)
        {
            int cnt = 0;
            double dbCurPos = 0;
            double dbCurPos_old = 0;
            double diffPos = 0;
            double diffPos_old = 0;

            do
            {
                if (cnt >= 200) break;
                bMotionDoneWait = true;

                ReadMotorPosition(true, bSilent: true);
                Thread.Sleep(400);

                if (Axis == MOTOR.STEP0)
                {
                    dbCurPos = CurrentPos.Step0AxisX;
                }
                else if (Axis == MOTOR.STEP1)
                {
                    dbCurPos = CurrentPos.Step1AxisY;
                }
                else if (Axis == MOTOR.STEP2)
                {
                    dbCurPos = CurrentPos.Step2AxisZ;
                }
                else if (Axis == MOTOR.GRIP)
                {
                    dbCurPos = CurrentPos.StepGripAxis;
                }
                else if (Axis == MOTOR.HAM)
                {
                    dbCurPos = CurrentPos.StepHamAxis;
                }
                else if (Axis == MOTOR.COVER)
                {
                    dbCurPos = CurrentPos.StepRotCover;
                }

                diffPos = Math.Abs(dbCurPos - dbCurPos_old);

                iPrintf(string.Format("Step {0} Moving: {1} ({2})", Axis, diffPos, cnt));
                cnt++;

                dbCurPos_old = dbCurPos;
                diffPos_old = diffPos;

                if (SensorStatus.Alarm)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    DisplayStatusMessage("[Wait for Step Stop] System Alarm !!!", TEST.FAIL);
                    EnableControls(true);
                    break;
                }

                if (!Serial.IsOpen || bStopFlag == true)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    if (Serial.IsOpen == false)
                        DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                    if (bStopFlag == true)
                        DisplayStatusMessage("Recipe Stopped !!!", TEST.FAIL);
                    break;
                }
            } while (diffPos > 0 && diffPos_old > 0);

            //ReadMotorPosition(true, bSilent: true);
            Thread.Sleep(100);
            bMotionDoneWait = false;
            iPrintf(string.Format("[Exit] Step {0} Moving: {1} ({2})", Axis, diffPos, cnt));

            return true;
        }

        //param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(if Abs=1)", param3 = "No=0/Asp=1/Disp=2",
        //param4 = "FlowRate(0.01~1.5 mL/sec)", param5 = "Load Vol(0~5mL)", param6 = "Offset Vol(0~1mL)", param7 = "" 
        private bool OperateTricontinentSyringePump(string Func1, string AbsPos, string AspDisp, 
                                                    string FlowRate, string Vol, string OffsetVol)
        {
            // Volume값을 incremental(step) 값으로 전환
            double PumpVolume_mL = double.Parse(Vol) + double.Parse(OffsetVol);
            double PumpVol_mL_per_Inc = PumpSyringe_Vol / PumpMax_Increment * 0.001;
            double PumpVol_IncResult = PumpVolume_mL / PumpVol_mL_per_Inc;
            double PumpVolume_inc = Math.Round(PumpVol_IncResult);

            // Flow Rate값을 incremental(step) 값으로 전환
            double PumpFlowRate_uL_per_sec = double.Parse(FlowRate) * 1000;
            double PumpFlowRate_IncPerSecResult = PumpFlowRate_uL_per_sec * (double)(PumpVel_Resolution / PumpSyringe_Vol);
            double PumpFlowRate_inc_per_sec = Math.Round(PumpFlowRate_IncPerSecResult);

            if (int.Parse(AspDisp) == 1 || int.Parse(AspDisp) == 2)
            {
                if (SerPinchValve(VALVE.OPEN) != COM_Status.ACK)
                {
                    DisplayStatusMessage("Set Pinch Valve Error", TEST.FAIL);
                    return false;
                }
            }

            if (int.Parse(Func1) == 0 && int.Parse(AspDisp) == 1)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                              (byte)' ', 0,   // N/A
                              (int)PumpFlowRate_inc_per_sec,       // Plunger Speed 설정
                              (byte)'P', (int)PumpVolume_inc);     // aspiration 방향으로 volume값 만큼 이동
            }
            else if (int.Parse(Func1) == 0 && int.Parse(AspDisp) == 2)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                               (byte)' ', 0,    // N/A
                               (int)PumpFlowRate_inc_per_sec,       // Plunger Speed 설정
                               (byte)'D', (int)PumpVolume_inc);     // dispense 방향으로 volume값 만큼 이동
            }
            else if (int.Parse(Func1) == 1 && int.Parse(AspDisp) == 1)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                                (byte)'A', int.Parse(AbsPos),  // 절대위치로 이동
                                (int)PumpFlowRate_inc_per_sec,       // Plunger Speed 설정
                                (byte)'P', (int)PumpVolume_inc);     // aspiration 방향으로 volume값 만큼 이동
            }
            else if (int.Parse(Func1) == 1 && int.Parse(AspDisp) == 2)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                                (byte)'A', int.Parse(AbsPos),  // 절대위치로 이동
                                (int)PumpFlowRate_inc_per_sec,       // Plunger Speed 설정
                                (byte)'D', (int)PumpVolume_inc);     // dispense 방향으로 volume값 만큼 이동
            }
            else if (int.Parse(Func1) == 2)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                              (byte)'T', 0,        // Terminate Executing Command
                               0,                  // N/A
                               (byte)' ', 0);      // N/A
            }
            else if (int.Parse(Func1) == 3)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                               (byte)'Z', 0,        // Initialize Pump
                                0,                  // N/A
                               (byte)' ', 0);       // N/A
            }
            else if (int.Parse(Func1) == 4)
            {
                RunPer3_TricontinentPump((byte)' ', 0,
                               (byte)'?', 0,        // Initialize Pump
                                0,                  // N/A
                               (byte)' ', 0);       // N/A

                label_Recipe_PE3_PlungerPos.Text = label_PE3_PlungerPos.Text;
            }
            else
            {
                DisplayStatusMessage("Tricontinental Pipett Parameter Input Error!", TEST.FAIL);
                return false;
            }

            if (int.Parse(Func1) == 2 || int.Parse(Func1) == 3)
            {
                if (SerPinchValve(VALVE.CLOSE) != COM_Status.ACK)
                {
                    DisplayStatusMessage("Set Pinch Valve Error", TEST.FAIL);
                    return false;
                }
            }

            return true;
        }


        //param_cnt = 6, param1 = "Start=1/Stop=2", param2 = "Sensitivity", param3 = "Dir Plus=1/Minus=2",
        //param4 = "Axis X=1/Y=2/Z=3", param5 = "Speed", param6 = "Max Position", param7 = ""
        private bool FindLiquidSurface(string StartStop, string Sensitivity, string Dir, 
                                       string Axis, string Spd, string MaxPos)
        {
            int dir = 0;
            int timeout = 1000;
            int i = 0;
            bLLD_Stop_Flag = false;

            string strDir = "";
            if (int.Parse(Dir) == 1)
            {
                strDir = "+";
                dir = 1;
            }
            else if (int.Parse(Dir) == 2)
            {
                strDir = "-";
                dir = -1;
            }
            int trg_axis = int.Parse(Axis) + 3;

            if (int.Parse(StartStop) == 1)
            {
                MoveStepMotor(STEP_CMD.MOVE, (MOTOR)trg_axis,
                    int.Parse(Spd), dir * double.Parse(MaxPos),
                    3, 3, POS_OPT.REL, HOLD_STATE.NONE);
                Thread.Sleep(20);
                Run_Hamilton_cLLD((byte)'L', int.Parse(Sensitivity));

                while (i < timeout)
                {
                    if (bLLD_Stop_Flag == true)
                        break;
                    
                    if(i == 0)
                        RunPer2_HamiltonPipett("RE", 0, 0, 0, 0, TIP_TYPE.NONE);    // error code check

                    i++;
                    Thread.Sleep(1);
                    if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                    {
                        RunPer2_HamiltonPipett("RE", 0, 0, 0, 0, TIP_TYPE.NONE);    // error code check
                        iPrintf("Hamilton Pipett Error (No: " + SensorStatus.ham_pipett_errNo + ")");

                        if (SensorStatus.ham_pipett_errNo != 0)
                        {
                            MoveStepMotor(STEP_CMD.STOP, (MOTOR)trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                            Run_Hamilton_cLLD((byte)'P', 0);
                            
                            iPrintf("Hamilton Pipett Error! cLLD Aborted!");
                            DisplayStatusMessage("Hamilton Pipett Error!", TEST.FAIL);
                            
                            if (bPosTimerState == true)
                            {
                                Thread.Sleep(50); //200     // interval을 고려해야 함
                                bPosTimerState = false;
                                if (bSerialTimerState == true)
                                    btnTimer_Click(this, null);
                                iPrintf("Pos Monitor Stop!");
                            }
                            MonitorStepMotorStatus();

                            iPrintf("Set Step Run Flag Low!");
                            bStepRunState = false;

                            return false;
                        }
                    }
                    else
                    {
                        Run_Hamilton_cLLD((byte)'V', 0);
                    }

                    //if (ConfirmcLLD_State() == true)
                    if (bcLLD_IO == true || bcLLD_Detected == true)
                    {
                        iPrintf(String.Format("level I/O: {0}, detected msg: {1}", bcLLD_IO, bcLLD_Detected));

                        MoveStepMotor(STEP_CMD.STOP, (MOTOR)trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                        Run_Hamilton_cLLD((byte)'P', 0);
                        iPrintf("Liquid Level Detection Success!");

                        if (bPosTimerState == true)
                        {
                            Thread.Sleep(50); //200     // interval을 고려해야 함
                            bPosTimerState = false;
                            if (bSerialTimerState == true)
                                btnTimer_Click(this, null);
                            iPrintf("Pos Monitor Stop!");
                        }
                        MonitorStepMotorStatus();

                        iPrintf("Set Step Run Flag Low!");
                        bStepRunState = false;

                        if (trg_axis == (int)MOTOR.STEP0)
                        {
                            strcLLD_Dir_Axis = strDir + "X";
                            label_cLLDAxisPos.Text = CurrentPos.Step0AxisX.ToString("F2");
                            iPrintf(String.Format("Axis X: {0}", CurrentPos.Step0AxisX));
                        }
                        else if (trg_axis == (int)MOTOR.STEP1)
                        {
                            strcLLD_Dir_Axis = strDir + "Y";
                            label_cLLDAxisPos.Text = CurrentPos.Step1AxisY.ToString("F2");
                            iPrintf(String.Format("Axis Y: {0}", CurrentPos.Step1AxisY));
                        }
                        else if (trg_axis == (int)MOTOR.STEP2)
                        {
                            strcLLD_Dir_Axis = strDir + "Z";
                            label_cLLDAxisPos.Text = CurrentPos.Step2AxisZ.ToString("F2");
                            iPrintf(String.Format("Axis Z: {0}", CurrentPos.Step2AxisZ));
                        }

                        label_Recipe_strcLLD_Dir_Axis.Text = strcLLD_Dir_Axis;
                        label_Recipe_cLLDAxisPos.Text = label_cLLDAxisPos.Text;

                        break;
                    }

                    if (i >= timeout)
                    {
                        MoveStepMotor(STEP_CMD.STOP, (MOTOR)trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                        Run_Hamilton_cLLD((byte)'P', 0);
                        iPrintf("Liquid Level Not Detected!");
                        DisplayStatusMessage("Hamilton Pipett Liquid Level Not Detected!", TEST.FAIL);
                        MonitorStepMotorStatus();

                        return false;
                    }
                }
            }
            else if (int.Parse(StartStop) == 2)
            {
                MoveStepMotor(STEP_CMD.STOP, (MOTOR)trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                Run_Hamilton_cLLD((byte)'P', 0);

                if (bPosTimerState == true)
                {
                    Thread.Sleep(50); //200     // interval을 고려해야 함
                    bPosTimerState = false;
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                    iPrintf("Pos Monitor Stop!");
                }
                MonitorStepMotorStatus();

                iPrintf("Set Step Run Flag Low!");
                bStepRunState = false;
            }
            else
            {
                DisplayStatusMessage($"Hamilton Pipett cLLD Parameter Input Error! P1: {StartStop}", TEST.FAIL);
                return false;
            }

            return true;
        }
        
        //param1 = "Speed", param2 = "X Position", param3 = "Y Position", param4 = "X Offset" ,
        //param5 = "Y Offset", param6 = "", param7 = ""
        private bool MoveToolXY(string Spd, string PosX, string PosY, 
                                string ToolOffsetX, string ToolOffsetY, string OffsetX, string OffsetY)
        {
            double TrgToolX = double.Parse(PosX) - double.Parse(ToolOffsetX) - double.Parse(OffsetX);
            double TrgToolY = double.Parse(PosY) - double.Parse(ToolOffsetY) - double.Parse(OffsetY);

            if ((axis_stroke.X_max < TrgToolX || axis_stroke.X_min > TrgToolX) ||
                (axis_stroke.Y_max < TrgToolY || axis_stroke.Y_min > TrgToolY))
            {
                if (axis_stroke.X_max < TrgToolX || axis_stroke.X_min > TrgToolX)
                    DisplayStatusMessage("Select Tool X Move Out of Stroke Error", TEST.FAIL);
                if (axis_stroke.Y_max < TrgToolY || axis_stroke.Y_min > TrgToolY)
                    DisplayStatusMessage("Select Tool Y Move Out of Stroke Error", TEST.FAIL);
                return false;
            }

            double ToolDistX = Math.Abs(TrgToolX - CurrentPos.Step0AxisX);
            double ToolDistY = Math.Abs(TrgToolY - CurrentPos.Step1AxisY);
            double ToolDistVect = Math.Sqrt(Math.Pow(ToolDistX, 2) + Math.Pow(ToolDistY, 2));
            double ToolAngVect = Math.Atan2(ToolDistY, ToolDistX) * 180 / Math.PI;

            // 각 축의 scale value를 적절한 속도 비율로 조절해야 함.
            // XY의 경우는 직선 운동이 되도록 검증 및 튜닝 필요 - calibration
            double scaleToolX = Math.Cos(ToolAngVect * Math.PI / 180) * 1.0f;
            double scaleToolY = Math.Sin(ToolAngVect * Math.PI / 180) * 1.0f;

            double spdToolX = scaleToolX * Convert.ToDouble(Spd);
            double spdToolY = scaleToolY * Convert.ToDouble(Spd);

            //XY Move
            if (MoveStepXYCrdMotion(Convert.ToInt32(spdToolX), TrgToolX,
                                    Convert.ToInt32(spdToolY), TrgToolY, WAIT_OPT.WAIT) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point XY Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point XY Move Done", TEST.PASS);
            }

            return true;
        }

        //param_cnt = 4, param1 = "Z Distance", param2 = "Offset X", param3 = "Offset Y",
        // param4 = "Offset Z" ,param5 = "", param6 = "", param7 = ""
        //ReadMotorPosition(true);
        private bool SelectToolMoveXYZ(string Z_Dist, string OffsetX, string OffsetY, string OffsetZ)
        {
            double trg_x = CurrentPos.Step0AxisX - double.Parse(OffsetX);
            double trg_y = CurrentPos.Step1AxisY - double.Parse(OffsetY);
            double trg_z = CurrentPos.Step2AxisZ + (double.Parse(OffsetZ) - double.Parse(Z_Dist));

            if ((axis_stroke.X_max < trg_x || axis_stroke.X_min > trg_x) ||
                (axis_stroke.Y_max < trg_y || axis_stroke.Y_min > trg_y) ||
                (axis_stroke.Z_max < trg_z || axis_stroke.Z_min > trg_z))
            {
                if (axis_stroke.X_max < trg_x || axis_stroke.X_min > trg_x)
                    DisplayStatusMessage("Select Tool X Move Out of Stroke Error", TEST.FAIL);
                if (axis_stroke.Y_max < trg_y || axis_stroke.Y_min > trg_y)
                    DisplayStatusMessage("Select Tool Y Move Out of Stroke Error", TEST.FAIL);
                if (axis_stroke.Z_max < trg_z || axis_stroke.Z_min > trg_z)
                    DisplayStatusMessage("Select Tool Z Move Out of Stroke Error", TEST.FAIL);

                return false;
            }

            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, config.StepAxisX_Speed, trg_x,
                config.StepAxisX_Acc, config.StepAxisX_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Select Tool X Move Error", TEST.FAIL);
                return false;
            }
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, config.StepAxisY_Speed, trg_y,
                config.StepAxisY_Acc, config.StepAxisY_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Select Tool Y Move Error", TEST.FAIL);
                return false;
            }
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, config.StepAxisZ_Speed, trg_z,
               config.StepAxisZ_Acc, config.StepAxisZ_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Select Tool Z Move Error", TEST.FAIL);
                return false;
            }

            return true;
        }


        //  param1 = "Asp=1/Disp=2/ADC=3/MA=4/MD=5", param2 = "Follow NA=0/1.5=1/15=2/50=3(ml)",
        //  param3 = "FlowRate 0.01 ~ 15 (0.1mL/s)",param4 = "Asp/Disp Vol(0~1.1mL)", 
        //  param5 = "StopBackVol(0~0.0325mL)", param6 = "OverAspVol(0~1mL)", param7 = "StopSpd(0.01 ~ 15 mL/sec)" 
        private bool OperateHamiltonPipettLiquid(string Func1, string Tube, string FlowRate, string AspDisp,
                                                 string SB_Vol, string OA_Vol, string StopSpd)
        {
            int stop_spd = int.Parse(StopSpd) * 1000;
            double flowrate = double.Parse(FlowRate) * 1000;
            double volume = double.Parse(AspDisp) * 10000;
            double stopback_volume = double.Parse(SB_Vol) * 10000;
            double overAsp_volume = double.Parse(OA_Vol) * 10000;
            double trg_tube_Ham = 0;
            int duration_ms = (int)Math.Round((volume / flowrate) * 1000) - 200;

            if (int.Parse(Tube) == 1)
            {
                trg_tube_Ham = Tube_ID_1_5ml;
            }
            else if (int.Parse(Tube) == 2)
            {
                trg_tube_Ham = Tube_ID_15ml;
            }
            else if (int.Parse(Tube) == 3)
            {
                trg_tube_Ham = Tube_ID_50ml;
            }

            if (int.Parse(Func1) == 1)
            {
                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(volume*0.0001, flowrate*0.0001, trg_tube_Ham, Lead_AxisZ, Z_FOLLOW_DIR.DOWN, PERIPHERAL.HAM_PIPETT);
                    bMotionDoneWait = true;
                }                

                RunPer2_HamiltonPipett("AL", (int) volume, (int) overAsp_volume, (int) flowrate, stop_spd, 0);  //Liquid Aspirate

                //if (SensorStatus.ham_pipett_errNo != 0)
                if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                {
                    if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                    {
                        MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                    }

                    iPrintf("Hamilton Pipett Error! Aspirate Aborted!");
                    bStepRunState = false;
                    if (bMotionDoneWait == true)
                        bMotionDoneWait = false;
                    bPipettMotion = false;
                    MonitorStepMotorStatus();
                    return false;
                }

                Thread.Sleep(duration_ms);
                MonitorStepMotorStatus();
                if(bMotionDoneWait == true)
                    bMotionDoneWait = false;
                bStepRunState = false;
                bPipettMotion = false;
            }
            else if (int.Parse(Func1) == 2)
            {
                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(volume*0.0001, flowrate*0.0001, trg_tube_Ham, Lead_AxisZ, Z_FOLLOW_DIR.UP, PERIPHERAL.HAM_PIPETT);
                    bMotionDoneWait = true;
                }                

                RunPer2_HamiltonPipett("DL", (int) volume, (int) stopback_volume, (int) flowrate, stop_spd, 0);  //Liquid Dispense

                //if (SensorStatus.ham_pipett_errNo != 0)
                if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                {
                    if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                    {
                        MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                    }

                    iPrintf("Hamilton Pipett Error! Dispense Aborted!");
                    bStepRunState = false;
                    if (bMotionDoneWait == true)
                        bMotionDoneWait = false;
                    bPipettMotion = false;
                    MonitorStepMotorStatus();
                    return false;
                }

                Thread.Sleep(duration_ms);
                MonitorStepMotorStatus();
                if (bMotionDoneWait == true)
                    bMotionDoneWait = false;
                bStepRunState = false;
                bPipettMotion = false;
            }
            else if (int.Parse(Func1) == 3)
            {
                RunPer2_HamiltonPipett("AX", 0, 0, 0, 0, TIP_TYPE.NONE);     // ADC ON
            }
            else if (int.Parse(Func1) == 4)
            {
                RunPer2_HamiltonPipett("MA", (int) volume, 0, (int) flowrate, 0, TIP_TYPE.NONE);     // Mixing Asp
            }
            else if (int.Parse(Func1) == 5)
            {
                RunPer2_HamiltonPipett("MD", 0, 0, (int) flowrate, 0, TIP_TYPE.NONE);     // Mixing Disp
            }
            else
            {
                DisplayStatusMessage("Hamilton Pipett Parameter Input Error! Check 1st Param", TEST.FAIL);
                return false;
            }

            return true;
        }

        //  param1 = "No=0/PInit=1/TInit=2", param2 = "No=0/TipInsert=1/TipDiscard=2",
        //  param3 = "10uL=1/300uL=2/1000uL=3", param4 = "No=0/AirAsp=1/AirTrans=2", 
        //  param5 = "FlowRate 0.01 ~ 15 (0.1mL/s)", param6 = "AirVol(0~1mL)", param7 = "State No=0/Tip=1/cLLD=2" 
        private bool OperateHamiltonPipettDry(string Func1, string TipFunc, string Tip, string AirFunc, 
                                              string FlowRate, string AirVol, string State)
        {
            double airFlowrate = double.Parse(FlowRate) * 100;
            double airVol = double.Parse(AirVol) * 1000;

            if (int.Parse(Func1) == 1)
            {
                RunPer2_HamiltonPipett("DI", 0, 0, 0, 0, TIP_TYPE.NONE); // Plunger Init
                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();
            }
            else if (int.Parse(Func1) == 2)
            {
                RunPer2_HamiltonPipett("DE", 0, 0, (int) airFlowrate, 0, TIP_TYPE.NONE); // Tip Init
            }

            if (int.Parse(TipFunc) == 1 && (int.Parse(Tip) >= 1 && int.Parse(Tip) <= 3))
            {
                int tiptype = -1;
                if (int.Parse(Tip) == 1)
                {
                    tiptype = 0;
                    label_TipType.Text = "10 uL";
                }
                else if (int.Parse(Tip) == 2)
                {
                    tiptype = 4;
                    label_TipType.Text = "300 uL";
                }
                else if (int.Parse(Tip) == 3)
                {
                    tiptype = 6;
                    label_TipType.Text = "1000 uL";
                }

                RunPer2_HamiltonPipett("TP", 0, 0, 0, 0, (TIP_TYPE) tiptype); // Tip Insert
            }
            else if (int.Parse(TipFunc) == 2)
            {
                RunPer2_HamiltonPipett("TD", 0, 0, 0, 0, TIP_TYPE.NONE);     // Tip Discard
            }

            if (int.Parse(AirFunc) == 1)
            {
                RunPer2_HamiltonPipett("AB", (int) airVol, 0, (int) airFlowrate, 0, TIP_TYPE.NONE); // Air Aspirate
            }
            else if (int.Parse(AirFunc) == 2)
            {
                RunPer2_HamiltonPipett("AT", (int) airVol, 0, (int) airFlowrate, 0, TIP_TYPE.NONE);  // Air Transport
            }

            if (int.Parse(State) == 1)
            {
                if (ConfirmTipPresence() == true)
                    DisplayOutputMessage("Tip: " + label_TipPresence.Text, OUTPUT.DONE);
                else
                    DisplayOutputMessage("Tip: " + label_TipPresence.Text, OUTPUT.FAIL);
            }
            else if (int.Parse(State) == 2)
            {
                if (ConfirmcLLD_State() == true)
                    DisplayOutputMessage("cLLD: " + label_cLLDState.Text, OUTPUT.DONE);
                else
                    DisplayOutputMessage("cLLD: " + label_cLLDState.Text, OUTPUT.FAIL);
            }

            return true;
        }


        //param1 = "No=0/Abs=1/Kill=2/Init=3/Pos?=4", param2 = "AbsPos(if Abs=1)", param3 = "No=0/Asp=1/Disp=2",
        //param4 = "Follow NA=0/1.5=1/15=2/50=3(ml)", param5 = "FlowRate(0.01~22.85 mL/sec)",
        //param6 = "Vol(0~5mL)", param7 = "Offset Vol(0~0.5mL)" 
        private bool OperateTricontinentalPipett(string Func1, string AbsPos, string AspDisp, string Tube, 
                                                 string FlowRate, string Vol, string OffsetVol)
        {
            // Volume값을 incremental(step) 값으로 전환
            double Volume_mL = pipett_scale * (double.Parse(Vol) + double.Parse(OffsetVol));
            double Vol_mL_per_Inc = (TriPipett_Vol * 0.001) / TriPipett_Max_Increment;
            double Vol_IncResult = Volume_mL / Vol_mL_per_Inc;
            double Volume_inc = Math.Round(Vol_IncResult, 0);

            // Flow Rate값을 incremental(step) 값으로 전환
            double FlowRate_mL_per_sec = pipett_scale * double.Parse(FlowRate);
            double FlowRate_uL_per_sec = FlowRate_mL_per_sec * 1000;
            double FlowRate_IncPerSecResult = FlowRate_uL_per_sec / TriPipett_ul_per_inc;
            double FlowRate_inc_per_sec = Math.Round(FlowRate_IncPerSecResult, 0);

            int duration_ms = (int) Math.Round((Volume_inc / FlowRate_inc_per_sec) * 1000, 0);

            double trg_tube_Tri = 0;
            if (int.Parse(Tube) == 1)
            {
                trg_tube_Tri = Tube_ID_1_5ml;
            }
            else if (int.Parse(Tube) == 2)
            {
                trg_tube_Tri = Tube_ID_15ml;
            }
            else if (int.Parse(Tube) == 3)
            {
                trg_tube_Tri = Tube_ID_50ml;
            }

            if (Volume_inc < 0 || Volume_inc > TriPipett_Max_Increment)
            {
                DisplayStatusMessage("Pipett Volume Invalid Value Error", TEST.FAIL);
                iPrintf(String.Format("Invalid Value! Pipett Volume Pulse Count range = 0 ~ 1600 (input: {0})", Volume_inc));
                return false;
            }

            double trg_z = 0;
            double travel_dist = Math.Round((Volume_mL * 1000) / (Math.Pow(trg_tube_Tri, 2) * (Math.PI / 4)), 2);    // mm
            double travel_time = Volume_mL / FlowRate_mL_per_sec;   // sec

            ConfirmPlungerPosition();
            Thread.Sleep(200);
                        
            if(int.Parse(AspDisp) == 1) // asp
                trg_z = CurrentPos.Step2AxisZ + travel_dist;
            else if(int.Parse(AspDisp) == 1) // disp
                trg_z = CurrentPos.Step2AxisZ - travel_dist;

            if (int.Parse(Tube) != 0)
            {
                if (axis_stroke.Z_max < trg_z || axis_stroke.Z_min > trg_z)
                {
                    if (axis_stroke.Z_max < trg_z || axis_stroke.Z_min > trg_z)
                        DisplayStatusMessage("Z Move Range Out of Stroke Error", TEST.FAIL);

                    return false;
                }
            }

            int PlungerTrg = 0;

            if (int.Parse(Func1) == 0 && int.Parse(AspDisp) == 1)   // asp
            {
                PlungerTrg = PE1PlungerPosInc - (int)Volume_inc;
                if (PlungerTrg < 0)
                {
                    RunPer1_TricontinentPipett((byte)'A', Math.Abs((int)Volume_inc),  // 지정된 절대 위치로 이동
                                                0,       // 현재 지정된 속도로 이동
                                               (byte)' ', 0, timeout: 100);     // N/A
                    Thread.Sleep(duration_ms);
                }

                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(Volume_mL, FlowRate_mL_per_sec, trg_tube_Tri, Lead_AxisZ, Z_FOLLOW_DIR.DOWN, PERIPHERAL.TRI_PIPETT);

                    RunPer1_TricontinentPipett((byte)' ', 0,     // N/A
                              (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                              (byte)'P', (int)Volume_inc, timeout: 120);   // aspiration 방향으로 volume값 만큼 이동

                    iPrintf(string.Format("flowrate(inc/sec): {0}, vol_inc: {1}, flowrate(ml/sec): {2}, vol_ml: {3}, time:{4}",
                                  FlowRate_inc_per_sec, Volume_inc, FlowRate_mL_per_sec, Volume_mL, travel_time));

                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                    bStepRunState = false;
                    bPipettMotion = false;
                }
                else
                {
                    RunPer1_TricontinentPipett((byte)' ', 0,     // N/A
                              (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                              (byte)'P', (int)Volume_inc, timeout: 100);   // aspiration 방향으로 volume값 만큼 이동
                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                }
            }
            else if (int.Parse(Func1) == 0 && int.Parse(AspDisp) == 2)  //disp
            {
                PlungerTrg = PE1PlungerPosInc + (int)Volume_inc;
                if (PlungerTrg > (int)TriPipett_Max_Increment)
                {
                    RunPer1_TricontinentPipett((byte)'A', Math.Abs((int)TriPipett_Max_Increment - (int)Volume_inc),  // 지정된 절대 위치로 이동
                                                0,       // 현재 지정된 속도로 이동
                                               (byte)' ', 0, timeout: 100);     // N/A
                    Thread.Sleep(duration_ms);
                }

                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(Volume_mL, FlowRate_mL_per_sec, trg_tube_Tri, Lead_AxisZ, Z_FOLLOW_DIR.UP, PERIPHERAL.TRI_PIPETT);

                    RunPer1_TricontinentPipett((byte)' ', 0,     // N/A
                               (int)FlowRate_inc_per_sec,    // Plunger Speed 설정
                               (byte)'D', (int)Volume_inc, timeout: 120);  // dispense 방향으로 volume값 만큼 이동

                    iPrintf(string.Format("flowrate(inc/sec): {0}, vol_inc: {1}, flowrate(ml/sec): {2}, vol_ml: {3}, time:{4}",
                                  FlowRate_inc_per_sec, Volume_inc, FlowRate_mL_per_sec, Volume_mL, travel_time));

                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                    bStepRunState = false;
                    bPipettMotion = false;
                }
                else
                {
                    RunPer1_TricontinentPipett((byte)' ', 0,     // N/A
                               (int)FlowRate_inc_per_sec,    // Plunger Speed 설정
                               (byte)'D', (int)Volume_inc, timeout: 100);  // dispense 방향으로 volume값 만큼 이동
                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                }
            }
            else if (int.Parse(Func1) == 1 && int.Parse(AspDisp) == 1)   // aspirate
            {
                PlungerTrg = int.Parse(AbsPos) - (int)Volume_inc;
                if (PlungerTrg < 0)
                {
                    RunPer1_TricontinentPipett((byte)'A', (int)Volume_inc,   // 초기 절대위치로 이동 후 aspirate
                                (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                (byte)'D', (int)Volume_inc, timeout: 60);   // dispense 방향으로 volume값 만큼 이동
                }
                else
                {
                    RunPer1_TricontinentPipett((byte)'A', int.Parse(AbsPos),   // 초기 절대위치로 이동 후 aspirate
                                (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                (byte)'P', (int)Volume_inc, timeout: 60);   // aspiration 방향으로 volume값 만큼 이동
                }

                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(Volume_mL, FlowRate_mL_per_sec, trg_tube_Tri, Lead_AxisZ, Z_FOLLOW_DIR.DOWN, PERIPHERAL.TRI_PIPETT);
                    //Thread.Sleep(duration_ms);
                    //RunPer1_TricontinentPipett((byte)'?', 0,        // Request Current Plunger Position
                    //                                      0,              // N/A
                    //                                     (byte)' ', 0);   // N/A
                }
                else
                {
                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                }
            }
            else if (int.Parse(Func1) == 1 && int.Parse(AspDisp) == 2)  // dispense
            {
                PlungerTrg = int.Parse(AbsPos) + (int)Volume_inc;
                if (PlungerTrg > (int)TriPipett_Max_Increment)
                {
                    RunPer1_TricontinentPipett((byte)'A', Math.Abs((int)TriPipett_Max_Increment - (int)Volume_inc),   // 초기 절대위치로 이동 후 dispense
                                    (int)FlowRate_inc_per_sec,       // Plunger Speed 설정
                                    (byte)'P', (int)Volume_inc, timeout: 60);     // aspiration 방향으로 volume값 만큼 이동
                }
                else
                {
                    RunPer1_TricontinentPipett((byte)'A', int.Parse(AbsPos),   // 초기 절대위치로 이동 후 dispense
                                    (int)FlowRate_inc_per_sec,       // Plunger Speed 설정
                                    (byte)'D', (int)Volume_inc, timeout: 60);     // dispense 방향으로 volume값 만큼 이동
                }

                if (int.Parse(Tube) >= (int)Z_FOLLOW.ON)
                {
                    Liquid_Z_Follow_Move(Volume_mL, FlowRate_mL_per_sec, trg_tube_Tri, Lead_AxisZ, Z_FOLLOW_DIR.UP, PERIPHERAL.TRI_PIPETT);
                    //Thread.Sleep(duration_ms);
                    //RunPer1_TricontinentPipett((byte)'?', 0,        // Request Current Plunger Position
                    //                                      0,              // N/A
                    //                                     (byte)' ', 0);   // N/A
                }
                else
                {
                    Thread.Sleep(duration_ms);
                    ConfirmPlungerPosition();
                }
            }
            else if (int.Parse(Func1) == 2)
            {
                RunPer1_TricontinentPipett((byte)'T', 0,        // Terminate Executing Command
                               0,                               // N/A
                               (byte)' ', 0);                   // N/A
            }
            else if (int.Parse(Func1) == 3)
            {
                RunPer1_TricontinentPipett((byte)'Z', 0,        // Initialize Pipett
                                0,                              // N/A
                               (byte)' ', 0);                   // N/A
            }
            else if (int.Parse(Func1) == 4)
            {
                RunPer1_TricontinentPipett((byte)'?', 0,        // Request Current Plunger Position
                                                      0,             // N/A
                                                     (byte)' ', 0);  // N/A

                label_Recipe_PE1_PlungerPos.Text = PE1PlungerPosInc.ToString();
                label_PE1_PlungerCurPos.Text = PE1PlungerPosInc.ToString();
            }
            else
            {
                DisplayStatusMessage("Tricontinental Pipett Parameter Input Error!", TEST.FAIL);
                return false;
            }

            label_Recipe_PE1_PlungerPos.Text = PE1PlungerPosInc.ToString();

            return true;
        }


        public bool CentrifugeCoverOpenClose(string opt)
        {
            if (int.Parse(opt) == 0)           // Open
            {
                if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, config.CoverOpen_Speed, (double)config.CoverOpen_Pos,
                    config.RotorCover_Acc, config.RotorCover_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                {
                    DisplayStatusMessage("Rotor Cover Open Error", TEST.FAIL);
                    return false;
                }
                else
                {
                    DisplayStatusMessage("Rotor Cover Open Done", TEST.PASS);
                }
            }
            else if (int.Parse(opt) == 1)     // Close
            {
                if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, config.CoverClose_Speed, (double)config.CoverClose_Pos,
                    config.RotorCover_Acc, config.RotorCover_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
                {
                    DisplayStatusMessage("Rotor Cover Close Error", TEST.FAIL);
                    return false;
                }
                else
                {
                    DisplayStatusMessage("Rotor Cover Close Done", TEST.PASS);
                }
            }

            return true;
        }

        // param1 = "Speed", param2 = "Z Pos", param3 = "Gripper Pos", param4 = "Ham Pos" ,
        // param5 = "None=0/Z MoveUp=1", param6 = "", param7 = ""
        public bool MoveZAxes(string Spd, string TrgZ, string TrgGrip, string TrgHam)
        {
            double TrgposZ = double.Parse(TrgZ);
            double TrgposGripper = double.Parse(TrgGrip);
            double TrgposPipett = double.Parse(TrgHam);

            double scaleZ = 1.0f;
            double scaleGripper = 5.5f;
            double scalePipett = 5.5f;

            double spdZ = scaleZ * Convert.ToDouble(Spd);
            double spdGripper = scaleGripper * Convert.ToDouble(Spd);
            double spdPipett = scalePipett * Convert.ToDouble(Spd);

            iPrintf(string.Format("[Zmove Spd] Spd: {0}, z: {1:F2}, grip: {2:F2}, pipett: {3:F2}",
                    Spd, spdZ, spdGripper, spdPipett));

            //Z Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, Convert.ToInt32(spdZ), TrgposZ,
                config.StepAxisZ_Acc, config.StepAxisZ_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Z Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Z Move Done", TEST.PASS);
            }

            // Gripper Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, Convert.ToInt32(spdGripper), TrgposGripper,
                config.StepAxisGripper_Acc, config.StepAxisGripper_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Gripper Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Gripper Move Done", TEST.PASS);
            }

            // Pipett Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, Convert.ToInt32(spdPipett), TrgposPipett,
                config.StepAxisPipett_Acc, config.StepAxisPipett_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Pipett Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Pipett Move Done", TEST.PASS);
            }

            return true;
        }

        public bool MoveTpnt(string Spd, string TrgX, string TrgY, string TrgZ, string TrgGrip, string TrgHam)
        {
            double TrgposX = double.Parse(TrgX);
            double TrgposY = double.Parse(TrgY);
            double TrgposZ = double.Parse(TrgZ);
            double TrgposGripper = double.Parse(TrgGrip);
            double TrgposPipett = double.Parse(TrgHam);

            double DistX = Math.Abs(TrgposX - CurrentPos.Step0AxisX);
            double DistY = Math.Abs(TrgposY - CurrentPos.Step1AxisY);
            double DistVect = Math.Sqrt(Math.Pow(DistX, 2) + Math.Pow(DistY, 2));
            double AngVect = Math.Atan2(DistY, DistX) * 180 / Math.PI;

            // 각 축의 scale value를 적절한 속도 비율로 조절해야 함.
            // XY의 경우는 직선 운동이 되도록 검증 및 튜닝 필요 - calibration
            double scaleX = Math.Cos(AngVect * Math.PI / 180) * 1.0f;
            double scaleY = Math.Sin(AngVect * Math.PI / 180) * 1.0f;
            double scaleZ = 1.0f;
            double scaleGripper = 5.5f;
            double scalePipett = 5.5f;

            double spdX = scaleX * Convert.ToDouble(Spd);
            double spdY = scaleY * Convert.ToDouble(Spd);
            double spdZ = scaleZ * Convert.ToDouble(Spd);
            double spdGripper = scaleGripper * Convert.ToDouble(Spd);
            double spdPipett = scalePipett * Convert.ToDouble(Spd);

            iPrintf(string.Format("[Tmove Spd] Spd: {0}, x: {1:F2}, y: {2:F2}, z: {3:F2}, grip: {4:F2}, pipett: {5:F2}",
                    Spd, spdX, spdY, spdZ, spdGripper, spdPipett));

            if (FastZ_UpMotion(WAIT_OPT.WAIT) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Z Fast Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Z Fast Move Done", TEST.PASS);
            }

            //XY Move
            if (MoveStepXYCrdMotion(Convert.ToInt32(spdX), TrgposX,
                                    Convert.ToInt32(spdY), TrgposY, WAIT_OPT.WAIT) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point XY Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point XY Move Done", TEST.PASS);
            }

            if (!Serial.IsOpen || bStopFlag)
            {
                return false;
            }
            bMotionDoneWait = true;

            //Z Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, Convert.ToInt32(spdZ), TrgposZ,
                config.StepAxisZ_Acc, config.StepAxisZ_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Z Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Z Move Done", TEST.PASS);
            }

            // Gripper Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, Convert.ToInt32(spdGripper), TrgposGripper,
                config.StepAxisGripper_Acc, config.StepAxisGripper_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Gripper Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Gripper Move Done", TEST.PASS);
            }

            // Pipett Move
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, Convert.ToInt32(spdPipett), TrgposPipett,
                config.StepAxisPipett_Acc, config.StepAxisPipett_Dec, POS_OPT.ABS, HOLD_STATE.NONE) != COM_Status.ACK)
            {
                DisplayStatusMessage("Teaching Point Pipett Move Error", TEST.FAIL);
                return false;
            }
            else
            {
                DisplayStatusMessage("Teaching Point Pipett Move Done", TEST.PASS);
            }

            bMotionDoneWait = false;

            return true;
        }

        private void SetHeight(ListView LV, int height)
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new System.Drawing.Size(1, height);
            LV.SmallImageList = imgList;
        }

        public string GetTpntCategory(EDIT opt)
        {
            int[,] nIdxCnt = new int[MAXCNT_TPNT, MAXCNT_TPNT];

            // 현재 입력된 data들 중에 각 교시점 범주별 인덱스를 임시 행렬에 입력함
            for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
            {
                if(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(0, 2) == "TP")
                {
                    if(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Length == 3)    // 1자리수
                    {
                        nIdxCnt[0, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 1));
                    }
                    else        // 1자리수 이상
                    {
                        nIdxCnt[0, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 2));
                    }
                }
                if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(0, 2) == "CP")
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Length == 3)    // 1자리수
                    {
                        nIdxCnt[1, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 1));
                    }
                    else        // 1자리수 이상
                    {
                        nIdxCnt[1, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 2));
                    }
                }
                if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(0, 2) == "IP")
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Length == 3)    // 1자리수
                    {
                        nIdxCnt[2, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 1));
                    }
                    else        // 1자리수 이상
                    {
                        nIdxCnt[2, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 2));
                    }
                }
                if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(0, 2) == "RP")
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Length == 3)    // 1자리수
                    {
                        nIdxCnt[3, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 1));
                    }
                    else        // 1자리수 이상
                    {
                        nIdxCnt[3, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 2));
                    }
                }
                if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(0, 2) == "FP")
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Length == 3)    // 1자리수
                    {
                        nIdxCnt[4, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 1));
                    }
                    else        // 1자리수 이상
                    {
                        nIdxCnt[4, i] = int.Parse(DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString().Substring(2, 2));
                    }
                }
            }

            // 임시로 입력된 행렬의 인덱스값들에서 각각의 최대값을 산출함
            for (int i = 0; i < MAXCNT_TPNT_SORT; i++)
            {
                int max_val = nIdxCnt[i, 0];

                for (int j = 0; j < DV_World_T_Pnt.Rows.Count - 1; j++)
                {
                    if (max_val < nIdxCnt[i, j])
                    {
                        max_val = nIdxCnt[i, j];
                    }
                }
                nTpnt_Cnt[i] = max_val;
            }

            if(opt == EDIT.SAVE)
            {
                if (DV_World_T_Pnt.SelectedRows.Count > 0)
                {
                    if (DV_World_T_Pnt.SelectedRows[0].Cells[1].Value.ToString() == null || 
                        DV_World_T_Pnt.SelectedRows[0].Cells[1].Value.ToString() == "")
                    {
                        strTpnt = DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString();

                        if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == "TP")
                            strTpnt_Name = "Teach";
                        else if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == "CP")
                            strTpnt_Name = "Cooler";
                        else if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == "IP")
                            strTpnt_Name = "Tip";
                        else if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == "RP")
                            strTpnt_Name = "Tube";
                        else if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == "FP")
                            strTpnt_Name = "Centri";

                        return strTpnt;
                    }
                    else
                    {
                        strTpnt = DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString();
                        strTpnt_Name  = DV_World_T_Pnt.SelectedRows[0].Cells[1].Value.ToString();

                        return strTpnt;
                    }
                }
            }
            
                // 새로 생성될 교시점의 인덱스와 명칭을 생성함
            if (strTpnt_Sort == "None")
            {
                strTpnt = "TP";
                strTpnt_Name = "Teach";
                nTpnt_Cnt[0] = nTpnt_Cnt[0] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[0].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[0].ToString();
            }
            else if (strTpnt_Sort == "Normal")
            {
                strTpnt = "TP";
                strTpnt_Name = "Teach";
                nTpnt_Cnt[0] = nTpnt_Cnt[0] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[0].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[0].ToString();
            }
            else if (strTpnt_Sort == "Cooler")
            {
                strTpnt = "CP";
                strTpnt_Name = "Cooler";
                nTpnt_Cnt[1] = nTpnt_Cnt[1] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[1].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[1].ToString();
            }
            else if (strTpnt_Sort == "Tip")
            {
                strTpnt = "IP";
                strTpnt_Name = "Tip";
                nTpnt_Cnt[2] = nTpnt_Cnt[2] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[2].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[2].ToString();
            }
            else if (strTpnt_Sort == "Tube")
            {
                strTpnt = "RP";
                strTpnt_Name = "Tube";
                nTpnt_Cnt[3] = nTpnt_Cnt[3] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[3].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[3].ToString();
            }
            else if (strTpnt_Sort == "Centrifuge")
            {
                strTpnt = "FP";
                strTpnt_Name = "Centri";
                nTpnt_Cnt[4] = nTpnt_Cnt[4] + 1;
                strTpnt = strTpnt + nTpnt_Cnt[4].ToString();
                strTpnt_Name = strTpnt_Name + nTpnt_Cnt[4].ToString();
            }

            return strTpnt;
        }

        private void GetPosCmdParam(int idx, string strCmd1, string strCmd2)
        {
            MainWindow.PROCESS_CMD cmd = KeyByValue(MainWindow.dicCmd, strCmd1);
            offset_val = patternInput.offset_val;

            switch (cmd)
            {
                case MainWindow.PROCESS_CMD.MOV_X:
                    for (int i = 0; i < config.Pos_AxisX.Count; i++)
                    {
                        if (String.Compare(config.Pos_AxisX[i].Name,strCmd2) == 0)
                        {
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param1].Value = config.Pos_AxisX[i].Speed.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_AxisX[i].Position.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_AxisX[i].Acc.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_AxisX[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_Y:
                    for (int i = 0; i < config.Pos_AxisY.Count; i++)
                    {
                        if (String.Compare(config.Pos_AxisY[i].Name, strCmd2) == 0)
                        {
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param1].Value = config.Pos_AxisY[i].Speed.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_AxisY[i].Position.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_AxisY[i].Acc.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_AxisY[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_Z:
                    for (int i = 0; i < config.Pos_AxisZ.Count; i++)
                    {
                        if (String.Compare(config.Pos_AxisZ[i].Name, strCmd2) == 0)
                        {
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param1].Value = config.Pos_AxisZ[i].Speed.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_AxisZ[i].Position.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_AxisZ[i].Acc.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_AxisZ[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_GRIPPER:
                    for (int i = 0; i < config.Pos_AxisGripper.Count; i++)
                    {
                        if (String.Compare(config.Pos_AxisGripper[i].Name, strCmd2) == 0)
                        {
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param1].Value = config.Pos_AxisGripper[i].Speed.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_AxisGripper[i].Position.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_AxisGripper[i].Acc.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_AxisGripper[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_PIPETT:
                    for (int i = 0; i < config.Pos_AxisPipett.Count; i++)
                    {
                        if (String.Compare(config.Pos_AxisPipett[i].Name, strCmd2) == 0)
                        {
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param1].Value = config.Pos_AxisPipett[i].Speed.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_AxisPipett[i].Position.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_AxisPipett[i].Acc.ToString();
                           DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_AxisPipett[i].Dec.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.MOV_TOOL_XY:
                    break;
                case MainWindow.PROCESS_CMD.MOV_Z_AXES:
                    break;
                case MainWindow.PROCESS_CMD.MOV_T_PNT:
                    for (int i = 0; i < config.Pos_WorldPos.Count; i++)
                    {
                        if (String.Compare(config.Pos_WorldPos[i].Idx, strCmd2) == 0)
                        {
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param2].Value = config.Pos_WorldPos[i].X.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param3].Value = config.Pos_WorldPos[i].Y.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param4].Value = config.Pos_WorldPos[i].Z.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param5].Value = config.Pos_WorldPos[i].Gripper.ToString();
                            DV_Recipe.Rows[idx].Cells[(int)Recipe_COL.Param6].Value = config.Pos_WorldPos[i].Pipett.ToString();
                        }
                    }
                    break;
                case MainWindow.PROCESS_CMD.SEL_TOOL:
                    break;
                default:
                    break;
            }
        }

        public bool GetRecipeFromListView(int index)
        {
            try
            {
                ListButtonRecipe[index].recipe.Clear();
                DV_Recipe.Update();

                for (int i = 0; i < DV_Recipe.Rows.Count - 1; i++)
                {
                    GetPosCmdParam(i,
                                   DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command1].Value.ToString(),
                                   DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command2].Value.ToString());

                    Recipe rcp = new Recipe();
                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Enable].Value == null)   // Enable
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Enable].Value = false;
                    rcp.Enable = (bool)DV_Recipe.Rows[i].Cells[0].Value ? 1 : 0;

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command1].Value == null)   // Command1
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command1].Value = "";
                    rcp.Command1 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command1].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command2].Value == null)   // Command2
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command2].Value = "";
                    rcp.Command2 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command2].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value == null)   // Param1
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value = "";
                    rcp.Param1 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value == null)   // Param2
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value = "";
                    rcp.Param2 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value == null)   // Param3
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value = "";
                    rcp.Param3 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value == null)   // Param4
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value = "";
                    rcp.Param4 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value == null)   // Param5
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value = "";
                    rcp.Param5 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value == null)   // Param6
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value = "";
                    rcp.Param6 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value == null)   // Param7
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value = "";
                    rcp.Param7 = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Sleep].Value == null)   // Sleep
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Sleep].Value = "";
                    rcp.Sleep = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Sleep].Value.ToString();

                    if (DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Comment].Value == null)   // Comment
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Comment].Value = "";
                    rcp.Comment = DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Comment].Value.ToString();
                    
                    ListButtonRecipe[index].recipe.Add(rcp);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetAxisXPosFromListView()
        {
            try
            {
                config.Pos_AxisX.Clear();
                for (int i = 0; i < DV_AxisX.Rows.Count - 1; i++)
                {
                    DefinePos pos = new DefinePos();
                    if (DV_AxisX.Rows[i].Cells[0].Value == null)   // Name
                        DV_AxisX.Rows[i].Cells[0].Value = "";
                    pos.Name = DV_AxisX.Rows[i].Cells[0].Value.ToString();

                    if (DV_AxisX.Rows[i].Cells[1].Value == null)   // Speed
                        DV_AxisX.Rows[i].Cells[1].Value = "0";
                    pos.Speed = DV_AxisX.Rows[i].Cells[1].Value.ToString();

                    if (DV_AxisX.Rows[i].Cells[2].Value == null)   // Position
                        DV_AxisX.Rows[i].Cells[2].Value = "0";
                    pos.Position = DV_AxisX.Rows[i].Cells[2].Value.ToString();

                    if (DV_AxisX.Rows[i].Cells[3].Value == null)   // Acc
                        DV_AxisX.Rows[i].Cells[3].Value = "0";
                    pos.Acc = DV_AxisX.Rows[i].Cells[3].Value.ToString();

                    if (DV_AxisX.Rows[i].Cells[4].Value == null)   // Dec
                        DV_AxisX.Rows[i].Cells[4].Value = "0";
                    pos.Dec = DV_AxisX.Rows[i].Cells[4].Value.ToString();

                    config.Pos_AxisX.Add(pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetAxisYPosFromListView()
        {
            try
            {
                config.Pos_AxisY.Clear();
                for (int i = 0; i < DV_AxisY.Rows.Count - 1; i++)
                {
                    DefinePos pos = new DefinePos();
                    if (DV_AxisY.Rows[i].Cells[0].Value == null)   // Name
                        DV_AxisY.Rows[i].Cells[0].Value = "";
                    pos.Name = DV_AxisY.Rows[i].Cells[0].Value.ToString();

                    if (DV_AxisY.Rows[i].Cells[1].Value == null)   // Speed
                        DV_AxisY.Rows[i].Cells[1].Value = "0";
                    pos.Speed = DV_AxisY.Rows[i].Cells[1].Value.ToString();

                    if (DV_AxisY.Rows[i].Cells[2].Value == null)   // Position
                        DV_AxisY.Rows[i].Cells[2].Value = "0";
                    pos.Position = DV_AxisY.Rows[i].Cells[2].Value.ToString();

                    if (DV_AxisY.Rows[i].Cells[3].Value == null)   // Acc
                        DV_AxisY.Rows[i].Cells[3].Value = "0";
                    pos.Acc = DV_AxisY.Rows[i].Cells[3].Value.ToString();

                    if (DV_AxisY.Rows[i].Cells[4].Value == null)   // Dec
                        DV_AxisY.Rows[i].Cells[4].Value = "0";
                    pos.Dec = DV_AxisY.Rows[i].Cells[4].Value.ToString();

                    config.Pos_AxisY.Add(pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetAxisZPosFromListView()
        {
            try
            {
                config.Pos_AxisZ.Clear();
                for (int i = 0; i < DV_AxisZ.Rows.Count - 1; i++)
                {
                    DefinePos pos = new DefinePos();
                    if (DV_AxisZ.Rows[i].Cells[0].Value == null)   // Name
                        DV_AxisZ.Rows[i].Cells[0].Value = "";
                    pos.Name = DV_AxisZ.Rows[i].Cells[0].Value.ToString();

                    if (DV_AxisZ.Rows[i].Cells[1].Value == null)   // Speed
                        DV_AxisZ.Rows[i].Cells[1].Value = "0";
                    pos.Speed = DV_AxisZ.Rows[i].Cells[1].Value.ToString();

                    if (DV_AxisZ.Rows[i].Cells[2].Value == null)   // Position
                        DV_AxisZ.Rows[i].Cells[2].Value = "0";
                    pos.Position = DV_AxisZ.Rows[i].Cells[2].Value.ToString();

                    if (DV_AxisZ.Rows[i].Cells[3].Value == null)   // Acc
                        DV_AxisZ.Rows[i].Cells[3].Value = "0";
                    pos.Acc = DV_AxisZ.Rows[i].Cells[3].Value.ToString();

                    if (DV_AxisZ.Rows[i].Cells[4].Value == null)   // Dec
                        DV_AxisZ.Rows[i].Cells[4].Value = "0";
                    pos.Dec = DV_AxisZ.Rows[i].Cells[4].Value.ToString();

                    config.Pos_AxisZ.Add(pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetAxisGripperPosFromListView()
        {
            try
            {
                config.Pos_AxisGripper.Clear();
                for (int i = 0; i < DV_AxisGripper.Rows.Count - 1; i++)
                {
                    DefinePos pos = new DefinePos();
                    if (DV_AxisGripper.Rows[i].Cells[0].Value == null)   // Name
                        DV_AxisGripper.Rows[i].Cells[0].Value = "";
                    pos.Name = DV_AxisGripper.Rows[i].Cells[0].Value.ToString();

                    if (DV_AxisGripper.Rows[i].Cells[1].Value == null)   // Speed
                        DV_AxisGripper.Rows[i].Cells[1].Value = "0";
                    pos.Speed = DV_AxisGripper.Rows[i].Cells[1].Value.ToString();

                    if (DV_AxisGripper.Rows[i].Cells[2].Value == null)   // Position
                        DV_AxisGripper.Rows[i].Cells[2].Value = "0";
                    pos.Position = DV_AxisGripper.Rows[i].Cells[2].Value.ToString();

                    if (DV_AxisGripper.Rows[i].Cells[3].Value == null)   // Acc
                        DV_AxisGripper.Rows[i].Cells[3].Value = "0";
                    pos.Acc = DV_AxisGripper.Rows[i].Cells[3].Value.ToString();

                    if (DV_AxisGripper.Rows[i].Cells[4].Value == null)   // Dec
                        DV_AxisGripper.Rows[i].Cells[4].Value = "0";
                    pos.Dec = DV_AxisGripper.Rows[i].Cells[4].Value.ToString();

                    config.Pos_AxisGripper.Add(pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetAxisPipettPosFromListView()
        {
            try
            {
                config.Pos_AxisPipett.Clear();
                for (int i = 0; i < DV_AxisPipett.Rows.Count - 1; i++)
                {
                    DefinePos pos = new DefinePos();
                    if (DV_AxisPipett.Rows[i].Cells[0].Value == null)   // Name
                        DV_AxisPipett.Rows[i].Cells[0].Value = "";
                    pos.Name = DV_AxisPipett.Rows[i].Cells[0].Value.ToString();

                    if (DV_AxisPipett.Rows[i].Cells[1].Value == null)   // Speed
                        DV_AxisPipett.Rows[i].Cells[1].Value = "0";
                    pos.Speed = DV_AxisPipett.Rows[i].Cells[1].Value.ToString();

                    if (DV_AxisPipett.Rows[i].Cells[2].Value == null)   // Position
                        DV_AxisPipett.Rows[i].Cells[2].Value = "0";
                    pos.Position = DV_AxisPipett.Rows[i].Cells[2].Value.ToString();

                    if (DV_AxisPipett.Rows[i].Cells[3].Value == null)   // Acc
                        DV_AxisPipett.Rows[i].Cells[3].Value = "0";
                    pos.Acc = DV_AxisPipett.Rows[i].Cells[3].Value.ToString();

                    if (DV_AxisPipett.Rows[i].Cells[4].Value == null)   // Dec
                        DV_AxisPipett.Rows[i].Cells[4].Value = "0";
                    pos.Dec = DV_AxisPipett.Rows[i].Cells[4].Value.ToString();

                    config.Pos_AxisPipett.Add(pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetWorldTeachingPntFromListView()
        {
            try
            {
                config.Pos_WorldPos.Clear();
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    DefineWorldPos world_pos = new DefineWorldPos();

                    if (DV_World_T_Pnt.Rows[i].Cells[0].Value == null)   // Idx
                        DV_World_T_Pnt.Rows[i].Cells[0].Value = "0";
                    world_pos.Idx = DV_World_T_Pnt.Rows[i].Cells[0].Value.ToString();
                    
                    if (DV_World_T_Pnt.Rows[i].Cells[1].Value == null)   // Name
                        DV_World_T_Pnt.Rows[i].Cells[1].Value = "0";
                    world_pos.Name = DV_World_T_Pnt.Rows[i].Cells[1].Value.ToString();

                    if (DV_World_T_Pnt.Rows[i].Cells[2].Value == null)   // X
                        DV_World_T_Pnt.Rows[i].Cells[2].Value = "0";
                    world_pos.X = DV_World_T_Pnt.Rows[i].Cells[2].Value.ToString();

                    if (DV_World_T_Pnt.Rows[i].Cells[3].Value == null)   // Y
                        DV_World_T_Pnt.Rows[i].Cells[3].Value = "0";
                    world_pos.Y = DV_World_T_Pnt.Rows[i].Cells[3].Value.ToString();

                    if (DV_World_T_Pnt.Rows[i].Cells[4].Value == null)   // Z
                        DV_World_T_Pnt.Rows[i].Cells[4].Value = "0";
                    world_pos.Z = DV_World_T_Pnt.Rows[i].Cells[4].Value.ToString();

                    if (DV_World_T_Pnt.Rows[i].Cells[5].Value == null)   // Gripper
                        DV_World_T_Pnt.Rows[i].Cells[5].Value = "0";
                    world_pos.Gripper = DV_World_T_Pnt.Rows[i].Cells[5].Value.ToString();

                    if (DV_World_T_Pnt.Rows[i].Cells[6].Value == null)   // Pipett
                        DV_World_T_Pnt.Rows[i].Cells[6].Value = "0";
                    world_pos.Pipett = DV_World_T_Pnt.Rows[i].Cells[6].Value.ToString();

                    config.Pos_WorldPos.Add(world_pos);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private bool GetToolOffsetFromConfig()
        {
            try
            {
                config.ToolOffset.Clear();

                for (int i = 0; i < DV_Offset.Rows.Count - 1; i++)
                {
                    DefineToolOffset offset = new DefineToolOffset();

                    if (DV_Offset.Rows[i].Cells[0].Value == null)   // Idx
                        DV_Offset.Rows[i].Cells[0].Value = "0";
                    offset.Idx = DV_Offset.Rows[i].Cells[0].Value.ToString();

                    if (DV_Offset.Rows[i].Cells[1].Value == null)   // Name
                        DV_Offset.Rows[i].Cells[1].Value = "0";
                    offset.Name = DV_Offset.Rows[i].Cells[1].Value.ToString();

                    if (DV_Offset.Rows[i].Cells[2].Value == null)   // Z_Dist
                        DV_Offset.Rows[i].Cells[2].Value = "0";
                    offset.Z_Dist = DV_Offset.Rows[i].Cells[2].Value.ToString();

                    if (DV_Offset.Rows[i].Cells[3].Value == null)   // X
                        DV_Offset.Rows[i].Cells[3].Value = "0";
                    offset.X = DV_Offset.Rows[i].Cells[3].Value.ToString();

                    if (DV_Offset.Rows[i].Cells[4].Value == null)   // Y
                        DV_Offset.Rows[i].Cells[4].Value = "0";
                    offset.Y = DV_Offset.Rows[i].Cells[4].Value.ToString();

                    if (DV_Offset.Rows[i].Cells[5].Value == null)   // Z
                        DV_Offset.Rows[i].Cells[5].Value = "0";
                    offset.Z = DV_Offset.Rows[i].Cells[5].Value.ToString();

                    config.ToolOffset.Add(offset);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private void btnRecipeLoad_Click(object sender, EventArgs e)
        {
            int index = GetSelectedButtonIndex();
            if (index < 0)
                return;
            config.ReadWriteRecipe(RW.READ, ListButtonRecipe[index].button.AccessibleName, ref ListButtonRecipe[index].recipe);
            RefreshRecipeDataView(index);
        }

        public void RefreshRecipeDataView(int index)
        {
            try
            {
                DV_Recipe.Rows.Clear();
                if (ListButtonRecipe[index].recipe == null)
                    return;
                for (int i = 0; i < ListButtonRecipe[index].recipe.Count; i++)
                {
                    PROCESS_CMD cmd = KeyByValue(MainWindow.dicCmd, ListButtonRecipe[index].recipe[i].Command1);

                    DV_Recipe.Rows.Add();
                                                            
                    DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Enable].Value = ListButtonRecipe[index].recipe[i].Enable == 1 ? true : false;
                    DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command1].Value = ListButtonRecipe[index].recipe[i].Command1;
                    DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Command2].Value = ListButtonRecipe[index].recipe[i].Command2;

                    if (ListButtonRecipe[index].recipe[i].Command2 != null && ListButtonRecipe[index].recipe[i].Command2 != "")
                    {
                        if (cmd == PROCESS_CMD.MOV_X || cmd == PROCESS_CMD.MOV_Y || cmd == PROCESS_CMD.MOV_Z ||
                       cmd == PROCESS_CMD.MOV_GRIPPER || cmd == PROCESS_CMD.MOV_PIPETT)
                        {
                            GetPosCmdParam(i,
                                              ListButtonRecipe[index].recipe[i].Command1,
                                              ListButtonRecipe[index].recipe[i].Command2);

                            //DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value = ListButtonRecipe[index].recipe[i].Param3;
                            //DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value = ListButtonRecipe[index].recipe[i].Param4;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value = ListButtonRecipe[index].recipe[i].Param5;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value = ListButtonRecipe[index].recipe[i].Param6;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value = ListButtonRecipe[index].recipe[i].Param7;
                        }
                        else if (cmd == PROCESS_CMD.MOV_T_PNT)
                        {
                            GetPosCmdParam(i,
                                             ListButtonRecipe[index].recipe[i].Command1,
                                             ListButtonRecipe[index].recipe[i].Command2);

                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value = ListButtonRecipe[index].recipe[i].Param1;
                            //DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value = ListButtonRecipe[index].recipe[i].Param2;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value = ListButtonRecipe[index].recipe[i].Param7;
                        }
                        else if (cmd == PROCESS_CMD.MOV_TOOL_XY)
                        {
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value = ListButtonRecipe[index].recipe[i].Param1;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value = ListButtonRecipe[index].recipe[i].Param2;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value = ListButtonRecipe[index].recipe[i].Param3;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value = ListButtonRecipe[index].recipe[i].Param4;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value = ListButtonRecipe[index].recipe[i].Param5;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value = ListButtonRecipe[index].recipe[i].Param6;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value = ListButtonRecipe[index].recipe[i].Param7;
                        }
                        else if (cmd == PROCESS_CMD.SEL_TOOL)
                        {
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value = ListButtonRecipe[index].recipe[i].Param1;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value = ListButtonRecipe[index].recipe[i].Param2;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value = ListButtonRecipe[index].recipe[i].Param3;
                            DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value = ListButtonRecipe[index].recipe[i].Param4;
                        }
                    }
                    else
                    {
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param1].Value = ListButtonRecipe[index].recipe[i].Param1;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param2].Value = ListButtonRecipe[index].recipe[i].Param2;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param3].Value = ListButtonRecipe[index].recipe[i].Param3;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param4].Value = ListButtonRecipe[index].recipe[i].Param4;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param5].Value = ListButtonRecipe[index].recipe[i].Param5;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param6].Value = ListButtonRecipe[index].recipe[i].Param6;
                        DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Param7].Value = ListButtonRecipe[index].recipe[i].Param7;
                    }

                    DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Sleep].Value = ListButtonRecipe[index].recipe[i].Sleep;
                    DV_Recipe.Rows[i].Cells[(int)Recipe_COL.Comment].Value = ListButtonRecipe[index].recipe[i].Comment;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private void SetAxisXDataView()
        {
            try
            {
                DV_AxisX.Rows.Clear();
                DV_AxisX.Refresh();
                for (int i = 0; i < config.Pos_AxisX.Count; i++)
                {
                    DV_AxisX.Rows.Add();
                    DV_AxisX.Rows[i].Cells[0].Value = config.Pos_AxisX[i].Name;
                    DV_AxisX.Rows[i].Cells[1].Value = config.Pos_AxisX[i].Speed;
                    DV_AxisX.Rows[i].Cells[2].Value = config.Pos_AxisX[i].Position;
                    DV_AxisX.Rows[i].Cells[3].Value = config.Pos_AxisX[i].Acc;
                    DV_AxisX.Rows[i].Cells[4].Value = config.Pos_AxisX[i].Dec;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private void SetAxisYDataView()
        {
            try
            {
                DV_AxisY.Rows.Clear();
                DV_AxisY.Refresh();
                for (int i = 0; i < config.Pos_AxisY.Count; i++)
                {
                    DV_AxisY.Rows.Add();
                    DV_AxisY.Rows[i].Cells[0].Value = config.Pos_AxisY[i].Name;
                    DV_AxisY.Rows[i].Cells[1].Value = config.Pos_AxisY[i].Speed;
                    DV_AxisY.Rows[i].Cells[2].Value = config.Pos_AxisY[i].Position;
                    DV_AxisY.Rows[i].Cells[3].Value = config.Pos_AxisY[i].Acc;
                    DV_AxisY.Rows[i].Cells[4].Value = config.Pos_AxisY[i].Dec;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private void SetAxisZDataView()
        {
            try
            {
                DV_AxisZ.Rows.Clear();
                DV_AxisZ.Refresh();
                for (int i = 0; i < config.Pos_AxisZ.Count; i++)
                {
                    DV_AxisZ.Rows.Add();
                    DV_AxisZ.Rows[i].Cells[0].Value = config.Pos_AxisZ[i].Name;
                    DV_AxisZ.Rows[i].Cells[1].Value = config.Pos_AxisZ[i].Speed;
                    DV_AxisZ.Rows[i].Cells[2].Value = config.Pos_AxisZ[i].Position;
                    DV_AxisZ.Rows[i].Cells[3].Value = config.Pos_AxisZ[i].Acc;
                    DV_AxisZ.Rows[i].Cells[4].Value = config.Pos_AxisZ[i].Dec;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private void SetAxisGripperDataView()
        {
            try
            {
                DV_AxisGripper.Rows.Clear();
                DV_AxisGripper.Refresh();
                for (int i = 0; i < config.Pos_AxisGripper.Count; i++)
                {
                    DV_AxisGripper.Rows.Add();
                    DV_AxisGripper.Rows[i].Cells[0].Value = config.Pos_AxisGripper[i].Name;
                    DV_AxisGripper.Rows[i].Cells[1].Value = config.Pos_AxisGripper[i].Speed;
                    DV_AxisGripper.Rows[i].Cells[2].Value = config.Pos_AxisGripper[i].Position;
                    DV_AxisGripper.Rows[i].Cells[3].Value = config.Pos_AxisGripper[i].Acc;
                    DV_AxisGripper.Rows[i].Cells[4].Value = config.Pos_AxisGripper[i].Dec;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private void SetAxisPipettDataView()
        {
            try
            {
                DV_AxisPipett.Rows.Clear();
                DV_AxisPipett.Refresh();
                for (int i = 0; i < config.Pos_AxisPipett.Count; i++)
                {
                    DV_AxisPipett.Rows.Add();
                    DV_AxisPipett.Rows[i].Cells[0].Value = config.Pos_AxisPipett[i].Name;
                    DV_AxisPipett.Rows[i].Cells[1].Value = config.Pos_AxisPipett[i].Speed;
                    DV_AxisPipett.Rows[i].Cells[2].Value = config.Pos_AxisPipett[i].Position;
                    DV_AxisPipett.Rows[i].Cells[3].Value = config.Pos_AxisPipett[i].Acc;
                    DV_AxisPipett.Rows[i].Cells[4].Value = config.Pos_AxisPipett[i].Dec;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        public void HighlightOrgPointRow(int idx)
        {
            String Value_Idx = DV_World_T_Pnt.Rows[idx].Cells[0].Value as string;
            String Value_Name = DV_World_T_Pnt.Rows[idx].Cells[1].Value as string;

            if (Value_Idx == "CP1" || Value_Idx == "IP1" || Value_Idx == "RP1")
            {
                DV_World_T_Pnt.Rows[idx].DefaultCellStyle.BackColor = Color.Yellow;
                DV_World_T_Pnt.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(0x66, 0x66, 0x66);
                if (Value_Name != null && Value_Name != "" && Value_Name.Contains("_Origin") != true)
                {
                    String ReName = DV_World_T_Pnt.Rows[idx].Cells[1].Value as string;

                    if(ReName.Contains("Auto") == true)
                    {
                        DV_World_T_Pnt.Rows[idx].Cells[1].Value = ReName.Replace("(Auto)", "_Origin");
                    }
                    else
                    {
                        DV_World_T_Pnt.Rows[idx].Cells[1].Value = ReName + "_Origin";
                    }
                }
            }
        }

        private void SetWorldTeachingPntDataView()
        {
            try
            {
                DV_World_T_Pnt.Rows.Clear();
                DV_World_T_Pnt.Refresh();
                for (int i = 0; i < config.Pos_WorldPos.Count; i++)
                {
                    DV_World_T_Pnt.Rows.Add();
                    DV_World_T_Pnt.Rows[i].Cells[0].Value = config.Pos_WorldPos[i].Idx;
                    DV_World_T_Pnt.Rows[i].Cells[1].Value = config.Pos_WorldPos[i].Name;
                    DV_World_T_Pnt.Rows[i].Cells[2].Value = config.Pos_WorldPos[i].X;
                    DV_World_T_Pnt.Rows[i].Cells[3].Value = config.Pos_WorldPos[i].Y;
                    DV_World_T_Pnt.Rows[i].Cells[4].Value = config.Pos_WorldPos[i].Z;
                    DV_World_T_Pnt.Rows[i].Cells[5].Value = config.Pos_WorldPos[i].Gripper;
                    DV_World_T_Pnt.Rows[i].Cells[6].Value = config.Pos_WorldPos[i].Pipett;

                    HighlightOrgPointRow(i);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
        }

        private bool SetToolOffsetFromConfig()
        {
            try
            {
                DV_Offset.Rows.Clear();
                DV_Offset.Refresh();
                for (int i = 0; i < config.ToolOffset.Count; i++)
                {
                    DV_Offset.Rows.Add();
                    DV_Offset.Rows[i].Cells[0].Value = config.ToolOffset[i].Idx;
                    DV_Offset.Rows[i].Cells[1].Value = config.ToolOffset[i].Name;
                    DV_Offset.Rows[i].Cells[2].Value = config.ToolOffset[i].Z_Dist;
                    DV_Offset.Rows[i].Cells[3].Value = config.ToolOffset[i].X;
                    DV_Offset.Rows[i].Cells[4].Value = config.ToolOffset[i].Y;
                    DV_Offset.Rows[i].Cells[5].Value = config.ToolOffset[i].Z;
                }

                config.ToolOffset.Clear();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }
            return true;
        }

        private void LV_TestPattern_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = DV_Recipe.Columns[e.ColumnIndex].Width;
        }

        private void DV_Recipe_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_current_running_row > -1)
            {
                DV_Recipe.ClearSelection();
                DV_Recipe.Rows[m_current_running_row].Cells[0].Selected = true;
                DV_Recipe.Rows[m_current_running_row].Selected = true;
            }
        }

        // Row Header에 Number를 자동입력
        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);

        }

        private void DV_CTC_Vertical_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);

        }

        // Row Header Double Click 시 실행
        private void DV_Recipe_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!isRunning && !isRunningSingle && !isRunningManual)
            {
                if (Serial.IsOpen == false)
                    return;
                ClearError();
                if (SensorStatus.Alarm)
                {
                    CheckAlarm_ResetDevice();
                }
                GetStatus(true);

                string log_msg = "";
                if (GetRecipeFromListView(GetSelectedButtonIndex()))
                {
                    TestSelectedRow(e.RowIndex);
                }
                    
                if (bStatusOk)
                {
                    log_msg = String.Format("{0} Line Step Run Finished", e.RowIndex+1);
                    DisplayStatusMessage(log_msg, TEST.PASS);
                }
                else
                {
                    DisplayStatusMessage(m_ErrorMessage, TEST.FAIL);
                }
                    
                if (bStopFlag)
                {
                    log_msg = String.Format("{0} Line Step Run Stopped", e.RowIndex + 1);
                    DisplayStatusMessage(log_msg, TEST.FAIL);
                }

                if (e.RowIndex != -1)
                {
                    // increase next step index
                    if (DV_Recipe.SelectedRows[0].Index < DV_Recipe.RowCount - 1)
                        DV_Recipe.Rows[DV_Recipe.SelectedRows[0].Index + 1].Selected = true;
                    else
                        DV_Recipe.Rows[0].Selected = true;
                }
            }
        }

        // Step Run Button Click 시 실행
        private void btnRecipeStepRun_Click(object sender, EventArgs e)
        {
            if (!isRunning && !isRunningSingle && !isRunningManual)
            {
                if (Serial.IsOpen == false)
                    return;
                ClearError();
                if (SensorStatus.Alarm)
                {
                    CheckAlarm_ResetDevice();
                }
                GetStatus(true);

                string log_msg = "";
                if (GetRecipeFromListView(GetSelectedButtonIndex()))
                {
                    TestSelectedRow(DV_Recipe.SelectedRows[0].Index);
                }
                    
                if (bStatusOk)
                {
                    log_msg = String.Format("{0} Line Step Run Finished", DV_Recipe.SelectedRows[0].Index + 1);
                    DisplayStatusMessage(log_msg, TEST.PASS);
                }
                else
                {
                    DisplayStatusMessage(m_ErrorMessage, TEST.FAIL);
                }

                if (bStopFlag)
                {
                    log_msg = String.Format("{0} Line Step Run Stopped", DV_Recipe.SelectedRows[0].Index + 1);
                    DisplayStatusMessage(log_msg, TEST.FAIL);
                }   

                // increase next step index
                if (DV_Recipe.SelectedRows[0].Index < DV_Recipe.RowCount - 1)
                    DV_Recipe.Rows[DV_Recipe.SelectedRows[0].Index + 1].Selected = true;
                else
                    DV_Recipe.Rows[0].Selected = true;
            }
        }

        private void btnSelectedContinuousRecipeRun_Click(object sender, EventArgs e)
        {
            RunSelectedRecipeTable(DV_Recipe.SelectedRows[0].Index);
        }

        private void TestSelectedRow(int idx)
        {
            ClearRecipeStatusVariables();

            if (m_current_running_row == -1 || m_current_running_row == idx)
            {
                if (DV_Recipe.Rows[idx].DefaultCellStyle.SelectionBackColor != Color.DeepPink)
                {
                    DV_Recipe.Rows[idx].DefaultCellStyle.SelectionBackColor = Color.DeepPink;
                    DV_Recipe.ReadOnly = true;
                    m_current_running_row = idx;
                    stopwatch.Restart();
                    isRunningSingle = true;

                    TestSingleLine(idx);    // execute single line
                    
                    stopwatch.Stop();
                    m_current_running_row = -1;
                    UpdateRowColor(idx, false);
                }
                else
                {
                    UpdateAllRowColor(Color.DodgerBlue);
                    DV_Recipe.ReadOnly = false;
                    m_current_running_row = -1;
                }
            }
            else
                DV_Recipe.Rows[m_current_running_row].Selected = true;
        }

        private void UpdateRowColor(int idx, bool run)
        {
            try
            {
                DV_Recipe.ClearSelection();
                if (run)
                {
                    UpdateAllRowColor(Color.DodgerBlue);
                    DV_Recipe.Rows[idx].DefaultCellStyle.SelectionBackColor = Color.DeepPink;
                    DV_Recipe.ReadOnly = true;
                    m_current_running_row = idx;
                    DV_Recipe.Rows[idx].Cells[0].Selected = true;
                    DV_Recipe.Rows[idx].Selected = true;
                }
                else
                {
                    UpdateAllRowColor(Color.DodgerBlue);
                    DV_Recipe.ReadOnly = false;
                    m_current_running_row = -1;
                    DV_Recipe.Rows[idx].Selected = true;
                }
                DV_Recipe.Refresh();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void UpdateAllRowColor(Color color)
        {
            for (int i = 0; i < DV_Recipe.Rows.Count; i++)
            {
                DV_Recipe.Rows[i].DefaultCellStyle.SelectionBackColor = color;
            }
        }

        private void DV_Recipe_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex == DV_Recipe.SelectedRows[0].Index)
            {
                btnRecipeEdit_Click(sender, e);
            }
            else
            {
                if (m_current_running_row > -1)
                {
                    DV_Recipe.ClearSelection();
                    DV_Recipe.Rows[m_current_running_row].Cells[0].Selected = true;
                    DV_Recipe.Rows[m_current_running_row].Selected = true;
                }
            }
        }

        private void InitVideoCapture()
        {
            if (cvCapture != null)
            {
                cvCapture.Dispose();
                cvCapture = null;
            }
            TrackBar_Video.Value = 0;
            label_video_cur_time.Text = "00:00";
            label_video_tot_time.Text = "00:00";
        }

        private void btnRecipeTest_Click(object sender, EventArgs e)
        {
            //if ((isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF) && config.bDebugMode == false)
            if (isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF)
                return;
            RunFullProcess();
        }

        public async void RunRecipeTable()
        {
            //if ((isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF) && config.bDebugMode == false)
            if (isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF)
                return;

            DisplayStatusMessage("Test Running ...", TEST.RUNNING);
            ClearError();
            InitVideoCapture();
            bStopFlag = false;
            bSerialStop = false;
            bStatusOk = true;

            // start timer
            if (bSerialTimerState == false)
                btnTimer_Click(this, null);

            EnableControls(false);
            EnableElapsedTimeLables(true);
            isRunning = true;
            DeleteOldFiles();

            var taskTurnTable = Task.Run(() => TestRun());
            await taskTurnTable;
            //TestRun();

            EnableControls(true);
            UpdateDiskInformation();
            if (bStatusOk)
                DisplayStatusMessage("Test Finished", TEST.PASS);
            else
                DisplayStatusMessage(m_ErrorMessage, TEST.FAIL);
            if (bStopFlag)
                DisplayStatusMessage("Test Stopped", TEST.FAIL);
            isRunning = false;
        }

        public async void RunSelectedRecipeTable(int SelectedRowIdx)
        {
            //if ((isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF) && config.bDebugMode == false)
            if (isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF)
                return;

            DisplayStatusMessage("Test Running ...", TEST.RUNNING);
            ClearError();
            InitVideoCapture();
            bStopFlag = false;
            bSerialStop = false;
            bStatusOk = true;

            // start timer
            if (bSerialTimerState == false)
                btnTimer_Click(this, null);

            EnableControls(false);
            EnableElapsedTimeLables(true);
            isRunning = true;
            DeleteOldFiles();

            var taskTurnTable = Task.Run(() => TestSelectedRowToEndRun(SelectedRowIdx));
            await taskTurnTable;
            //TestRun();

            EnableControls(true);
            UpdateDiskInformation();
            if (bStatusOk)
                DisplayStatusMessage("Test Finished", TEST.PASS);
            else
                DisplayStatusMessage(m_ErrorMessage, TEST.FAIL);
            if (bStopFlag)
                DisplayStatusMessage("Test Stopped", TEST.FAIL);
            isRunning = false;
        }

        private void ClearRecipeStatusVariables()
        {
            label_Recipe_PeltSetTemp.Text = "-";
            label_Recipe_PeltChamberTemp.Text = "-";
            label_Recipe_PeltPeltierTemp.Text = "-";
            label_Recipe_PeltCoolerTemp.Text = "-";

            label_Recipe_FlowMeterVolumeVal.Text = "-";

            label_Recipe_strcLLD_Dir_Axis.Text = "-";
            label_Recipe_cLLDAxisPos.Text = "-";

            label_Recipe_PE1_PlungerPos.Text = "-";
            label_Recipe_PE3_PlungerPos.Text = "-";

            checkedListBox_TipPresence.SetItemChecked(0, false);
            checkedListBox_TipPresence.SetItemChecked(1, false);
            checkedListBox_TipPresence.SetItemChecked(2, false);

            checkedListBox_StateLLD.SetItemChecked(0, false);
            checkedListBox_StateLLD.SetItemChecked(1, false);
            checkedListBox_StateLLD.SetItemChecked(2, false);
            checkedListBox_StateLLD.SetItemChecked(3, false);

            checkedListBox_LaserSensor.SetItemChecked(0, false);
            checkedListBox_LaserSensor.SetItemChecked(1, false);

            checkedListBox_DGM_Weight.SetItemChecked(0, false);
            checkedListBox_DGM_Weight.SetItemChecked(1, false);
        }

        public bool WaitForStepMotionDone()
        {
            int cnt = 0;

            do
            {
                //if (cnt >= 15)
                if (cnt >= 20)
                {
                    iPrintf("step wait count full!!");
                    break;
                }
                bMotionDoneWait = true;

                GetStatus(waitReceive: true, bSilent: true);
                Thread.Sleep(150);   //300  //50   //200
                ReadMotorPosition(waitReceive: true, bSilent: true);
                Thread.Sleep(150);

                PosThreadMethod();
                //iPrintf(string.Format("Wait for Line Step Motion.. (cnt:{0}, flag:{1}, Gip:{2}/{3}, Ham:{4}/{5}, Cov:{6}/{7}, Z:{8}/{9})",
                //                      cnt, bStepRunState, CurrentPos.StepGripAxis, GripAxState.bMove,
                //                      CurrentPos.StepHamAxis, HamAxState.bMove, CurrentPos.StepRotCover, CoverAxState.bMove,
                //                      CurrentPos.Step2AxisZ, Step2AxState.bMOVE));
                cnt++;

                if (SensorStatus.Alarm)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    DisplayStatusMessage("[Wait for Step Done] System Alarm !!!", TEST.FAIL);
                    EnableControls(true);
                    break;
                }

                if (!Serial.IsOpen || bStopFlag == true)
                {
                    if (isRunningSingle == true)
                        isRunningSingle = false;
                    if (Serial.IsOpen == false)
                        DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                    if (bStopFlag == true)
                        DisplayStatusMessage("Recipe Stopped !!!", TEST.FAIL);
                    break;
                }
            } while (bStepRunState == true);

            //GetStatus(waitReceive: true, bSilent: true);
            //Thread.Sleep(200);   //50
            //ReadMotorPosition(waitReceive: true, bSilent: true);
            bMotionDoneWait = false;
            iPrintf("Wait Step Done Passed!");

            //iPrintf(string.Format("[Exit] Line Step Motion Done (cnt:{0}, flag:{1}, Gip:{2}/{3}, Ham:{4}/{5}, Ham_diff:{6}/{7})",
            //                       cnt, bStepRunState, CurrentPos.StepGripAxis, GripAxState.bMove,
            //                       CurrentPos.StepHamAxis, HamAxState.bMove, diffPosHam, diffPosHam_old));

            return true;
        }

        // run line by line
        private bool TestSingleLine(int idx)
        {
            bool retVal = true;
            CommandParam cmdParam = GetRowParam(idx);

            if (cmdParam.enable == false || cmdParam.strCmd1 == "")
            {
                isRunningSingle = false;
                return false;
            }

            if (SwitchMon.bRun == false || SwitchMon.bPower == false)
            {
                isRunningSingle = false;
                DisplayStatusMessage("Not Run State! Run Fail!", TEST.FAIL);
                iPrintf("Not Run State! Run Fail!");
                EnableControls(true);
                return false;
            }

            if (nEccentricCnt >= nEccentricThreshold)
                SensorStatus.ErrEccentricCnt = true;

            if (SensorStatus.Alarm)
            {
                if (config.bDebugMode == false)
                {
                    isRunningSingle = false;
                    DisplayStatusMessage("[Singline Recipe Run] System Alarm !!!", TEST.FAIL);
                    EnableControls(true);
                    return false;
                }
            }
            
            if (SensorStatus.RunSwitch != Status.ON || SensorStatus.PowerState != Status.ON)
            {
                if (config.bDebugMode == false)
                {
                    isRunningSingle = false;
                    if(SensorStatus.RunSwitch != Status.ON)
                        DisplayStatusMessage("Run Switch OFF !!!", TEST.FAIL);
                    if (SensorStatus.PowerState != Status.ON)
                        DisplayStatusMessage("Power OFF !!!", TEST.FAIL);
                    return false;
                }
            }

            if (!Serial.IsOpen)
            {
                isRunningSingle = false;
                DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                return false;
            }

            if (isRunningSingle == true)
            {
                bStopFlag = false;
                bSerialStop = false;

                Rpm.Current = 0;

                if (SensorStatus.Alarm)
                {
                    isRunningSingle = false;
                    DisplayStatusMessage("[Singline Recipe Run] System Alarm !!!", TEST.FAIL);
                    EnableControls(true);
                    return false;
                }

                if (!Serial.IsOpen)
                {
                    isRunningSingle = false;
                    DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                    return false;
                }

                // start timer
                if (bSerialTimerState == false)
                    btnTimer_Click(this, null);

                btnClearImage_Click(this, null);
                EnableControls(false);
                EnableElapsedTimeLables(true);
                Rpm.Current = 0;
            }

            UpdateRowColor(idx, true);
            if (timer_frame.Enabled)
            {
                currentIndexStartTime = stopwatch.Elapsed.TotalSeconds;
                isRunningRecipeChanged = true;
                currentRecipeIndex = idx;
            }

            iPrintf($"{idx + 1}. {cmdParam.strCmd1} {cmdParam.strCmd2} : {cmdParam.param1}, {cmdParam.param2}, " +
                $"{cmdParam.param3}, {cmdParam.param4}, {cmdParam.param5}, {cmdParam.param6}, {cmdParam.param7}");

            if (bAxisMovingFlag[0] == true) bAxisMovingFlag[0] = false;
            if (bAxisMovingFlag[1] == true) bAxisMovingFlag[1] = false;
            if (bAxisMovingFlag[2] == true) bAxisMovingFlag[2] = false;
            if (bAxisMovingFlag[3] == true) bAxisMovingFlag[3] = false;
            if (bAxisMovingFlag[4] == true) bAxisMovingFlag[4] = false;
            if (bAxisMovingFlag[5] == true) bAxisMovingFlag[5] = false;

            // parsing cmd & execute
            retVal = ExeRecipe(cmdParam);

            if (bStepRunState == true && bPipettMotion != true)
            {
                WaitForStepMotionDone();
            }

            if (bPeltRunState == true)
            {
                ReadPeltierTemp();
            }
            Thread.Sleep(config.RecipeLineDelay);

            m_current_running_row = -1;
            UpdateRowColor(idx, false);

            if (isRunningSingle == true)
            {
                // stop timer
                if (bPeltRunState != true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                }

                EnableControls(true);
                isRunningSingle = false;
            }

            return retVal;
        }

        public async void RunFullProcess()
        //public void RunFullProcess()
        {
            //if ((isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF) && config.bDebugMode == false)
            if (isRunning || Serial.IsOpen == false || SensorStatus.RunSwitch == Status.OFF)
                return;

            isRunning = true;
            bFullProcess = true;
            ClearError();
            DisplayStatusMessage("Test Running ...", TEST.RUNNING);
            InitVideoCapture();
            UpdateMusicPlayList(m_MusicFolderPath);

            // if (btnTimer.Text == "Start Timer")
            if (bSerialTimerState == false)
                btnTimer_Click(this, null);

            timer_music.Enabled = false;
            EnableControls(false);
            EnableElapsedTimeLables(true);
            DeleteOldFiles();
            btnMusicForward_Click(this, null);

            var taskTurnTable = Task.Run(() => RunButtons());
            await taskTurnTable;
            //RunButtons();

            //TestRun();
            EnableControls(true);
            StopMusic();
            UpdateDiskInformation();

            if (bStatusOk)
                DisplayStatusMessage("Test Finished", TEST.PASS);
            else
                DisplayStatusMessage(m_ErrorMessage, TEST.FAIL);
            if (bStopFlag)
                DisplayStatusMessage("Test Stopped", TEST.FAIL);

            bFullProcess = false;
            isRunning = false;
        }

        public void ClearError()
        {
            bStopFlag = false;
            bSerialStop = false;
            bStatusOk = true;
            m_ErrorMessage = "";
            pictureBox3.Image = null;
            label_add_volume.Hide();
        }

        private void waitForServoRun()
        {
            int cnt = 0;

            if (bServoRunState == true)
            {
                while (bServoRunState == true)
                {
                    //if (cnt >= SerialTimerLimit) break;
                    if (cnt >= SpinTotalTime + 3) break;
                    bMotionDoneWait = true;

                    Thread.Sleep(500);
                    GetStatus(true, bSilent: true);
                    Thread.Sleep(500);
                    ServoMonitor(MotorMon.RPM, bSilent: true);
                    
                    iPrintf(string.Format("Now Centrifuge Servo Running...Waiting for Stop ({0}/{1})", cnt, SpinTotalTime + 3));
                    cnt++;

                    if (SensorStatus.Alarm)
                    {
                        if (isRunningSingle == true)
                            isRunningSingle = false;
                        DisplayStatusMessage("[Wait for Servo Run] System Alarm !!!", TEST.FAIL);
                        EnableControls(true);
                        break;
                    }

                    if (!Serial.IsOpen || bStopFlag == true)
                    {
                        if (isRunningSingle == true)
                            isRunningSingle = false;
                        if(Serial.IsOpen == false)
                            DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                        if(bStopFlag == true)
                            DisplayStatusMessage("Recipe Stopped !!!", TEST.FAIL);
                        break;
                    }
                }

                bMotionDoneWait = false;
            }
        }

        private void RunButtons()
        {
            if(bFullProcess == false)
            {
                ClearError();
            }

            this.Invoke(new MethodInvoker(delegate ()
            {
                DateTime dtNow = DateTime.Now;
                string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f");
                label_TimeStamp.Text = dtStr;

                ClearRecipeStatusVariables();

                Rpm.Current = 0;
                ClearImage();

                stopwatch.Restart();

                iPrintf(" ");

                iPrintf("Serial = " + label_TimeStamp.Text);
                string fileName = $"{DIR_VIDEO}\\{label_TimeStamp.Text}.avi";
                iPrintf($"Recording to {fileName}");

                // check alarm state
                if (GetStatus(waitReceive: true) == COM_Status.ACK)
                {
                    SwitchControl(SwitchState.STATUS, Status.NONE);

                    if (SensorStatus.Alarm)
                    {
                        if (SensorStatus.Alarm)
                        {
                            CheckAlarm_ResetDevice();
                        }

                        GetStatus(waitReceive: true);
                        
                        if (SensorStatus.AlarmServo == Status.ON || SensorStatus.AlarmStep_Grip_ax == Status.ON ||
                       SensorStatus.AlarmStep_Ham_ax == Status.ON || SensorStatus.AlarmStep_Door_ax == Status.ON ||
                       SensorStatus.AlarmStep0_X_ax == Status.ON || SensorStatus.AlarmStep1_Y_ax == Status.ON ||
                       SensorStatus.AlarmStep2_Z_ax == Status.ON)
                        {
                            DisplayStatusMessage("Motor Reset Fail (Alarm)", TEST.FAIL);
                        }

                        if (SensorStatus.AlarmPeri1_tri_pipett == Status.ON || SensorStatus.AlarmPeri2_ham_pipett == Status.ON ||
                               SensorStatus.AlarmPeri3_tri_pump == Status.ON)
                        {
                            DisplayStatusMessage("Peripheral Reset Fail (Alarm)", TEST.FAIL);
                        }
                    }
                }

                int rowIndex = 0;

                // run defined button in order
                for (int btnIndex=0; btnIndex < ListButtonRecipe.Count && bStatusOk && bStopFlag == false; btnIndex++)
                {
                    if (SelectButton(btnIndex) == false)
                        DisplayStatusMessage("None of Button to Select", TEST.FAIL);

                    if (bStatusOk && GetRecipeFromListView(GetSelectedButtonIndex()) == false)
                        DisplayStatusMessage("Invalid Recipe Parameter", TEST.FAIL);

                    // run defined row in order
                    for (rowIndex = 0; rowIndex < ListButtonRecipe[btnIndex].recipe.Count && 
                         bStatusOk && bStopFlag == false; rowIndex++)
                    {
                        if (ListButtonRecipe[btnIndex].recipe[rowIndex].Enable == 0 || 
                            ListButtonRecipe[btnIndex].recipe[rowIndex].Command1 == "")
                            continue;

                        m_current_running_row = -1;

                        // execute line by line
                        if (TestSingleLine(rowIndex) == false)
                        {
                            bStatusOk = false;
                        }

                        label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                    }
                }

                if (CurrentRecipeCommand == "SPIN")
                {
                    waitForServoRun();
                }

                if (m_bSaveRecord)
                    StopRecord();
                else
                    bnStopGrab_Click(this, null);

                label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                iPrintf("Elapsed Time = " + label_elaplsed_time.Text);
                
                stopwatch.Stop();
                //if (btnTimer.Text == "Stop Timer")
                
                if (bPeltRunState != true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                }

                m_current_running_row = -1;

                isRunning = false;

                timer_frame.Enabled = false;
                isRunningRecipeChanged = false;
                recipeTimeSum = 0;
                estimatedCurrentFrame = 0;
                recipeFrameSum = 0;
                currentIndexStartTime = 0;
                lastCheckedTime = 0;
                fpsFromPrescale = 0;
                estimatedVideoTime = TimeSpan.Zero;
                isShakeFpsZero = false;
                isSpinPresetOrFPSZero = false;
                currentRecipeIndex = 0;
                lastRecipeIndex = 0;

                StopMusic();
                if (isForcedStop)
                {
                    iPrintf(">>Test Stopped.");
                    isForcedStop = false;
                }
                else
                    iPrintf(">>Test Finished.");
                iPrintf(" ");

            }));
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void DisplayStatusMessage(string str, TEST ready_run_pass_fail=TEST.READY)
        {
            try
            {
                if (bShudown == true)
                    return;
                if (str == "")
                    return;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    label_msg.Text = str;

                    if (ready_run_pass_fail == TEST.READY)
                    {
                        label_msg.BackColor = materialSkinManager.ColorScheme.PrimaryColor;
                        label_msg.ForeColor = Color.White;
                    }
                    else if (ready_run_pass_fail == TEST.RUNNING)
                    {
                        label_msg.ForeColor = Color.Black;
                        label_msg.BackColor = Color.LawnGreen;  // GreenYellow;    // SteelBlue;  // DodgerBlue;
                    }
                    else if (ready_run_pass_fail == TEST.PASS)
                    {
                        label_msg.ForeColor = Color.White;
                        label_msg.BackColor = Color.Green;
                    }
                    else
                    {
                        label_msg.ForeColor = Color.White;
                        label_msg.BackColor = Color.OrangeRed;
                        if (m_ErrorMessage == "")
                            m_ErrorMessage = str;
                        bStatusOk = false;
                    }
                    SerialMessageBox.AppendText(str);
                    SerialMessageBox.AppendText("\r\n");
                    SerialMessageBox.ScrollToCaret();

                    logger.Debug($"{str}");
                }));
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
            }
        }

        public void DisplayOutputMessage(string str, OUTPUT ready_run_pass_fail = OUTPUT.READY)
        {
            try
            {
                if (bShudown == true)
                    return;
                if (str == "")
                    return;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    label_output.Text = str;

                    if (ready_run_pass_fail == OUTPUT.READY)
                    {
                        label_output.BackColor = materialSkinManager.ColorScheme.PrimaryColor;
                        label_output.ForeColor = Color.White;

                        //bStatusOk = true;
                    }
                    else if (ready_run_pass_fail == OUTPUT.PROCESSING)
                    {
                        label_output.ForeColor = Color.Black;
                        label_output.BackColor = Color.LawnGreen;  // GreenYellow;    // SteelBlue;  // DodgerBlue;
                        //bStatusOk = true;
                    }
                    else if (ready_run_pass_fail == OUTPUT.DONE)
                    {
                        label_output.ForeColor = Color.White;
                        label_output.BackColor = Color.Green;
                        //bStatusOk = true;
                    }
                    else
                    {
                        label_output.ForeColor = Color.White;
                        label_output.BackColor = Color.OrangeRed;
                        if (m_ErrorMessage == "")
                            m_ErrorMessage = str;
                        bStatusOk = false;
                    }
                    SerialMessageBox.AppendText(str);
                    SerialMessageBox.AppendText("\r\n");
                    SerialMessageBox.ScrollToCaret();

                    logger.Debug($"{str}");
                }));
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
            }
        }

        private void TestRun()
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                //PlayMusic();
                DateTime dtNow = DateTime.Now;
                string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f");
                label_TimeStamp.Text = dtStr;

                ClearRecipeStatusVariables();

                Rpm.Current = 0;
                btnClearImage_Click(this, null);

                int btnIndex = GetSelectedButtonIndex();
                if (GetRecipeFromListView(btnIndex) == false)
                {
                    iPrintf("Invalid Recipe Parameter");
                    bStopFlag = true;
                }
                stopwatch.Restart();

                iPrintf(" ");
                iPrintf("Serial = " + label_TimeStamp.Text);
                string fileName = $"{DIR_VIDEO}\\{label_TimeStamp.Text}.avi";
                iPrintf($"Recording to {fileName}");

                int rowIndex = 0;
                
                for (rowIndex = 0; rowIndex < ListButtonRecipe[btnIndex].recipe.Count && 
                    bStatusOk && bStopFlag == false; rowIndex++)
                {
                    if (ListButtonRecipe[btnIndex].recipe[rowIndex].Enable == 0 || 
                        ListButtonRecipe[btnIndex].recipe[rowIndex].Command1 == "")
                        continue;

                    m_current_running_row = -1;
                    if (TestSingleLine(rowIndex) == false)
                    {
                        bStatusOk = false;
                        break;
                    }

                    label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                }

                if (CurrentRecipeCommand == "SPIN")
                {
                    waitForServoRun();
                }

                if (m_bSaveRecord)
                    StopRecord();
                else
                    bnStopGrab_Click(this, null);
                
                label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                iPrintf("Elapsed Time = " + label_elaplsed_time.Text);
                stopwatch.Stop();
                //if (btnTimer.Text == "Stop Timer")
                
                if (bPeltRunState != true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                }
                m_current_running_row = -1;

                isRunning = false;

                timer_frame.Enabled = false;
                isRunningRecipeChanged = false;
                recipeTimeSum = 0;
                estimatedCurrentFrame = 0;
                recipeFrameSum = 0;
                currentIndexStartTime = 0;
                lastCheckedTime = 0;
                fpsFromPrescale = 0;
                estimatedVideoTime = TimeSpan.Zero;
                isShakeFpsZero = false;
                isSpinPresetOrFPSZero = false;
                currentRecipeIndex = 0;
                lastRecipeIndex = 0;

                StopMusic();
                if (isForcedStop)
                {
                    iPrintf(">>Test Stopped.");
                    isForcedStop = false;
                }
                else
                    iPrintf(">>Test Finished.");
                iPrintf(" ");

            }));
        }

        private void TestSelectedRowToEndRun(int Idx)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                //PlayMusic();
                DateTime dtNow = DateTime.Now;
                string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f");
                label_TimeStamp.Text = dtStr;

                ClearRecipeStatusVariables();

                Rpm.Current = 0;
                btnClearImage_Click(this, null);

                int btnIndex = GetSelectedButtonIndex();
                if (GetRecipeFromListView(btnIndex) == false)
                {
                    iPrintf("Invalid Recipe Parameter");
                    bStopFlag = true;
                }
                stopwatch.Restart();

                iPrintf(" ");
                iPrintf("Serial = " + label_TimeStamp.Text);
                string fileName = $"{DIR_VIDEO}\\{label_TimeStamp.Text}.avi";
                iPrintf($"Recording to {fileName}");

                int rowIndex = Idx;

                for (rowIndex = Idx; rowIndex < ListButtonRecipe[btnIndex].recipe.Count &&
                    bStatusOk && bStopFlag == false; rowIndex++)
                {
                    if (ListButtonRecipe[btnIndex].recipe[rowIndex].Enable == 0 ||
                        ListButtonRecipe[btnIndex].recipe[rowIndex].Command1 == "")
                        continue;

                    m_current_running_row = -1;
                    if (TestSingleLine(rowIndex) == false)
                    {
                        bStatusOk = false;
                        break;
                    }

                    label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                }

                if (CurrentRecipeCommand == "SPIN")
                {
                    waitForServoRun();
                }

                if (m_bSaveRecord)
                    StopRecord();
                else
                    bnStopGrab_Click(this, null);

                label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                iPrintf("Elapsed Time = " + label_elaplsed_time.Text);
                stopwatch.Stop();
                //if (btnTimer.Text == "Stop Timer")

                if (bPeltRunState != true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                }
                m_current_running_row = -1;

                isRunning = false;

                timer_frame.Enabled = false;
                isRunningRecipeChanged = false;
                recipeTimeSum = 0;
                estimatedCurrentFrame = 0;
                recipeFrameSum = 0;
                currentIndexStartTime = 0;
                lastCheckedTime = 0;
                fpsFromPrescale = 0;
                estimatedVideoTime = TimeSpan.Zero;
                isShakeFpsZero = false;
                isSpinPresetOrFPSZero = false;
                currentRecipeIndex = 0;
                lastRecipeIndex = 0;

                StopMusic();
                if (isForcedStop)
                {
                    iPrintf(">>Test Stopped.");
                    isForcedStop = false;
                }
                else
                    iPrintf(">>Test Finished.");
                iPrintf(" ");

            }));
        }

        private void btnRecipeStop_Click(object sender, EventArgs e)
        {
            //if (btnTimer.Text == "Stop Timer")
            if(bSerialTimerState == true)
                btnTimer_Click(this, null);

            //BuildCmdPacket(bCommandSendBuffer, "ESCAPE", "", "");
            //SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port

            GetStatus(true);

            if (GripAxState.bMove == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.GRIP, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            if (HamAxState.bMove == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.HAM, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            if (CoverAxState.bMove == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.COVER, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            if (Step0AxState.bMOVE == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP0, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            if (Step1AxState.bMOVE == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP1, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            if (Step2AxState.bMOVE == true)
                MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);

            SystemCmd("ESCAPE", "", "");

            EnableControls(true);
            bStopFlag = true;
            bSerialStop = true;
            isForcedStop = true;
            bLLD_Stop_Flag = true;
            StopMusic();
        }


        private void btnErrorReset_Click(object sender, EventArgs e)
        {
            //GetStatus(true);
            SwitchControl(SwitchState.STATUS, Status.NONE);
            if (SensorStatus.Alarm)
            {
                CheckAlarm_ResetDevice();
            }
            GetStatus(true);

            if(SensorStatus.Alarm == false)
            {
                DisplayStatusMessage("Error Reset Done !!!", TEST.PASS);
            }
            else
            {
                DisplayStatusMessage("Error Reset Fail !!!", TEST.FAIL);
            }
        }

        private void EnableTab(bool enable)
        {
            for (int i = 1; i < materialTabControl1.TabCount; i++)
                materialTabControl1.Controls[i].Enabled = enable;
        }

        public PROCESS_CMD KeyByValue(Dictionary<PROCESS_CMD, CommandParam> dict, string val)
        {
            PROCESS_CMD key = 0;
            foreach (KeyValuePair<PROCESS_CMD, CommandParam> pair in dict)
            {
                if (((CommandParam) pair.Value).strCmd1 == val)
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }

        public CommandParam GetRowParam(int row)
        {
            CommandParam retParam = new CommandParam();

            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value = "";
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value = "";
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Enable].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Enable].Value = false;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param1].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param1].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param2].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param2].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param3].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param3].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param4].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param4].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param5].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param5].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param6].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param6].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param7].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param7].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Sleep].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Sleep].Value = 0;
            if (DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Comment].Value == null)
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Comment].Value = "";

            retParam.strCmd1 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value.ToString();
            retParam.strCmd2 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value.ToString();
            retParam.enable = (bool) DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Enable].Value;            
            retParam.param1 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param1].Value.ToString();
            retParam.param2 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param2].Value.ToString();
            retParam.param3 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param3].Value.ToString();
            retParam.param4 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param4].Value.ToString();
            retParam.param5 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param5].Value.ToString();
            retParam.param6 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param6].Value.ToString();
            retParam.param7 = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param7].Value.ToString();
            retParam.sleep = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Sleep].Value.ToString();
            retParam.comment = DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Comment].Value.ToString();

            return retParam;
        }

        public void SetRowParam(int row, CommandParam param)
        {
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value = param.strCmd1;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value = param.strCmd2;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Enable].Value = param.enable;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param1].Value = param.param1;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param2].Value = param.param2;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param3].Value = param.param3;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param4].Value = param.param4;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param5].Value = param.param5;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param6].Value = param.param6;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param7].Value = param.param7;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Sleep].Value = param.sleep;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Comment].Value = param.comment;
        }

        private void btnRecipeEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int row = DV_Recipe.SelectedRows[0].Index;
                DV_Recipe.Rows[row].Cells[0].Selected = true;   // make sure selecting command combo box
                DV_Recipe.Rows[row].Selected = true;

                string strCmd = (string)DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value;

                if (SetRecipeDialog(strCmd, row) == false)
                    return;
                patternInput.StartPosition = FormStartPosition.Manual;
                patternInput.Location = new System.Drawing.Point(this.Left + (this.Width / 2), this.Top + (this.Height / 2));
                if (patternInput.ShowDialog() == DialogResult.OK)
                {
                    GetRecipeDialog(row);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        public StringBuilder CopyNames(DefinePos[] defPos)
        {
            StringBuilder str = new StringBuilder();
            foreach (DefinePos pos in defPos)
            {
                str.Append(pos.Name);
            }

            return str;
        }

        public bool SetRecipeDialog(string strCmd, int row)
        {
            patternInput.SetParam(KeyByValue(dicCmd, strCmd), GetRowParam(row), this);

            return true;
        }

        private void GetRecipeDialog(int row)
        {
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Enable].Value = patternInput.radio_enable.Checked;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value = patternInput.combo_cmd1.Text;

            if((string) DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value == PROCESS_CMD.SEL_TOOL.ToString())
            {
                //DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value = patternInput.combo_cmd4.Text;
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value =
                                       patternInput.combo_cmd2.Text + "/" + patternInput.combo_cmd4.Text;
            }
            else if ((string)DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command1].Value == PROCESS_CMD.MOV_TOOL_XY.ToString())
            {
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value = 
                                       patternInput.combo_cmd2.Text + "/" + patternInput.combo_cmd4.Text;
            }
            else
            {
                DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Command2].Value = patternInput.combo_cmd2.Text;
            }
                
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param1].Value = patternInput.editParam1.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param2].Value = patternInput.editParam2.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param3].Value = patternInput.editParam3.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param4].Value = patternInput.editParam4.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param5].Value = patternInput.editParam5.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param6].Value = patternInput.editParam6.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Param7].Value = patternInput.editParam7.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Sleep].Value = patternInput.editSleep.Text;
            DV_Recipe.Rows[row].Cells[(int)Recipe_COL.Comment].Value = patternInput.editComment.Text;
        }

        private void btnRecipeInsert_Click(object sender, EventArgs e)
        {
            try
            {
                DV_Recipe.Rows.Insert(DV_Recipe.SelectedRows[0].Index, false, "", "", "", "", "", "", "", "");
                GetRecipeFromListView(GetSelectedButtonIndex());
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnRecipeRemove_Click(object sender, EventArgs e)
        {
            try
            {
                DV_Recipe.Rows.RemoveAt(DV_Recipe.SelectedRows[0].Index);
                GetRecipeFromListView(GetSelectedButtonIndex());
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnRecipeDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (DV_Recipe.RowCount <= DV_Recipe.SelectedRows[0].Index + 2)
                    return;
                int idx = DV_Recipe.SelectedRows[0].Index;
                CommandParam param1 = GetRowParam(idx);
                CommandParam param2 = GetRowParam(idx + 1);
                SetRowParam(idx, param2);
                SetRowParam(idx + 1, param1);
                DV_Recipe.Rows[idx + 1].Selected = true;
                GetRecipeFromListView(GetSelectedButtonIndex());
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnRecipeUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (DV_Recipe.SelectedRows[0].Index <= 0)
                    return;
                int idx = DV_Recipe.SelectedRows[0].Index;
                CommandParam param1 = GetRowParam(idx);
                CommandParam param2 = GetRowParam(idx - 1);
                SetRowParam(idx, param2);
                SetRowParam(idx - 1, param1);
                DV_Recipe.Rows[idx - 1].Selected = true;
                GetRecipeFromListView(GetSelectedButtonIndex());
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        public void btnRecipeSave_Click(object sender, EventArgs e)
        {
            try
            {
                int index = GetSelectedButtonIndex();
                GetRecipeFromListView(index);
                config.ReadWriteRecipe(RW.WRITE, ListButtonRecipe[index].button.AccessibleName, ref ListButtonRecipe[index].recipe);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void EnableControls(bool en_dis)
        {
            EnableRecipeButtons(en_dis);
            EnableCameraGroups(en_dis);
            EnableComGroups(en_dis);
            EnableTab(en_dis);
        }

        private void EnableRecipeButtons(bool en_dis)
        {
            btnRecipeSelectedRun.Enabled = en_dis;
            btnRecipeSave.Enabled = en_dis;
            btnRecipeRemove.Enabled = en_dis;
            btnRecipeInsert.Enabled = en_dis;
            btnRecipeEdit.Enabled = en_dis;
            btnRecipeUp.Enabled = en_dis;
            btnRecipeDown.Enabled = en_dis;
            btnButtonSave.Enabled = en_dis;
            btnButtonSaveAs.Enabled = en_dis;
            btnButtonOpen.Enabled = en_dis;
            btnRecipeTest.Enabled = en_dis;
            btnButtonReload.Enabled = en_dis;
        }

        private void EnableComGroups(bool en_dis)
        {
            grpBxSystemCom.Enabled = en_dis;
            btn_get_status.Enabled = en_dis;
        }

        private void EnableControlsManualTest(bool en_dis, bool isRun = true)
        {
            grpBxSystemCom.Enabled = en_dis;
            btnRecipeTest.Enabled = en_dis;
            btnRecipeStop.Enabled = en_dis;
            btnCloseDevice.Enabled = en_dis;
            btnOpenDevice.Enabled = en_dis;
            if (isRun)
            {
                EnableElapsedTimeLables(false);
                btnManualTestStart.Enabled = false;
            }
            else
            {
                btnManualTestStart.Enabled = true;
            }

        }
        private void EnableCameraGroups(bool en_dis)
        {
            grpBxPrameters.Enabled = en_dis;
            grpBxInitialization.Enabled = en_dis;
        }

        private void EnableElapsedTimeLables(bool en_dis)
        {
            label_elaplsed_time.Enabled = en_dis;
            label_elaplsed_time_title.Enabled = en_dis;
        }

        private void DefineButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
			
            int index = DefineButtons.IndexOf(button);
            SelectButton(index);
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                GetRecipeFromListView(index);
                RunRecipeTable();
            }
        }

        private void DefineButton_DoubleClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            int index = DefineButtons.IndexOf(button);
			GetRecipeFromListView(index);
            SelectButton(index);
            RunRecipeTable();
        }

        public bool SelectButton(int index)
        {
            try
            {
                if (DefineButtons.Count < index)
                    return false;

                foreach (Button btn in DefineButtons)
                {
                    btn.BackColor = materialSkinManager.ColorScheme.PrimaryColor;
                }
                DefineButtons[index].BackColor = ButtonSelectColor;
                Application.DoEvents();
                RefreshRecipeDataView(index);
                return true;
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
            return false;
        }

        public Button CreateButton(string text="", string name="")
        {
            Button btn = new Button();
            if (text == "")
                btn.Text = $"Button{DefineButtons.Count + 1}";
            if (name == "")
                btn.Name = $"btnRecipe{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}";   // Should be Unique
            btn.FlatStyle = FlatStyle.Flat;
            btn.Dock = DockStyle.Fill;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.BackColor = materialSkinManager.ColorScheme.PrimaryColor;
            btn.ForeColor = Color.Black;    // materialSkinManager.GetPrimaryTextColor();
            btn.Font = new Font(btn.Font.Name, btn.Font.Size, FontStyle.Bold);
            btn.ContextMenuStrip = MenuRecipe;
            btn.Click += new EventHandler(this.DefineButton_Clicked);
            btn.DoubleClick += new EventHandler(this.DefineButton_Clicked);
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            return btn;
        }

        public void SetDefineButton(int index)
        {  
            DefineButtons[index].FlatStyle = FlatStyle.Flat;
            DefineButtons[index].Dock = DockStyle.Fill;
            DefineButtons[index].TextAlign = ContentAlignment.MiddleCenter;
            DefineButtons[index].BackColor = materialSkinManager.ColorScheme.PrimaryColor;
            DefineButtons[index].ForeColor = Color.Black;    // materialSkinManager.GetPrimaryTextColor();
            DefineButtons[index].Font = new Font(DefineButtons[index].Font.Name, DefineButtons[index].Font.Size, FontStyle.Bold);
            DefineButtons[index].ContextMenuStrip = MenuRecipe;
            DefineButtons[index].DoubleClick += new EventHandler(this.DefineButton_Clicked);
            DefineButtons[index].Click += new EventHandler(this.DefineButton_Clicked);
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);

        }

        private void ResizeButtons()
        {
            tableLayout_button.ColumnStyles.Clear();
            for (var i = 0; i < tableLayout_button.ColumnCount; i++)
            {
                var percent = 100f / (float)tableLayout_button.ColumnCount;
                tableLayout_button.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
            }
        }

        private void MenuRecipe_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int idx = GetSelectedButtonIndex();

                if (e.ClickedItem.Name == "AddButton")
                {
                    Button btn = CreateButton();
                    AddButtonRecipe(btn);
                    tableLayout_button.ColumnCount = DefineButtons.Count;

                    ResizeButtons();
                    tableLayout_button.Controls.Add(btn, DefineButtons.Count - 1, 0);
                }
                else if (e.ClickedItem.Name == "InsertButton")
                {
                    Button btn = CreateButton();

                    InsertButtonRecipe(idx, btn);
                    tableLayout_button.ColumnCount = DefineButtons.Count;

                    tableLayout_button.Controls.Add(btn, idx, 0);

                    for (int i = tableLayout_button.Controls.Count - 1; i >= idx; i--)
                    {
                        var control = tableLayout_button.GetControlFromPosition(i, 0);
                        if (control != null)
                        {
                            tableLayout_button.SetColumn(control, DefineButtons.IndexOf((Button)control));
                        }
                    }

                    ResizeButtons();
                }
                else if (e.ClickedItem.Name == "RemoveButton")
                {
                    if (idx < 0)
                        return;

                    tableLayout_button.Controls.Remove(DefineButtons[idx]);
                    RemoveAtButtonRecipe(idx);
                    for (int i = idx + 1; i <= tableLayout_button.Controls.Count; i++)
                    {
                        var control = tableLayout_button.GetControlFromPosition(i, 0);

                        if (control != null)
                        {
                            tableLayout_button.SetColumn(control, i - 1);
                        }
                    }
                    tableLayout_button.ColumnCount = DefineButtons.Count;

                    if (idx > 0)
                        SelectButton(idx - 1);
                    else
                        SelectButton(0);
                }
                else if (e.ClickedItem.Name == "EditButton")
                {
                    defineButtonForm.mainWindow = this;
                    defineButtonForm.buttonIndex = idx;
                    defineButtonForm.StartPosition = FormStartPosition.Manual;
                    defineButtonForm.Location = new System.Drawing.Point(this.Left + (this.Width / 2), this.Top + (this.Height / 2));
                    defineButtonForm.editButtonName.Text = DefineButtons[idx].Text;
                    defineButtonForm.lblRecipeFilename.Text = Path.GetFileName(DefineButtons[idx].AccessibleName);
                    if (defineButtonForm.ShowDialog() == DialogResult.OK)
                    {
                        DefineButtons[idx].Text = defineButtonForm.editButtonName.Text;
                    }
                }
                else if (e.ClickedItem.Name == "ExecuteButton")
                {
                    GetRecipeFromListView(idx);
                    SelectButton(idx);

                    RunRecipeTable();
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        public int GetSelectedButtonIndex()
        {
            int idx = -1;
            foreach(Button btn in DefineButtons)
            {
                idx++;
                if (btn.BackColor == ButtonSelectColor)
                    return idx;
            }
            return -1;
        }
    }
}
