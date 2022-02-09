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
    public partial class MainWindow
    {
        /////////////////////////////////////////////////
        // Step Motor Home Position Move (Each Axis)
        /////////////////////////////////////////////////
        private void btnHomeStepAxisX_Click(object sender, EventArgs e)
        {
            StepMotorHomeMove(MOTOR.STEP0, "HOME", "");
        }

        private void btnHomeStepAxisY_Click(object sender, EventArgs e)
        {
            StepMotorHomeMove(MOTOR.STEP1, "HOME", "");
        }

        private void btnHomeStepAxisZ_Click(object sender, EventArgs e)
        {
            StepMotorHomeMove(MOTOR.STEP2, "HOME", "");
        }

        private void btnHomeStepGripperAxis_Click(object sender, EventArgs e)
        {
            StepMotorHomeMove(MOTOR.GRIP, "HOME", "");
            //Thread.Sleep(1000);
        }

        private void btnHomeStepPipettAxis_Click(object sender, EventArgs e)
        {
            StepMotorHomeMove(MOTOR.HAM, "HOME", "");
        }

        /////////////////////////////////////////////////
        // Step Motor Target Position Move
        /////////////////////////////////////////////////
        private void btnMoveStepAxisX_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, int.Parse(editStepAxisX_Speed.Text), double.Parse(editStepAxisX_Pos.Text), 
                int.Parse(editStepAxisX_Acc.Text), int.Parse(editStepAxisX_Dec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }
        private void btnMoveStepAxisY_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, int.Parse(editStepAxisY_Speed.Text), double.Parse(editStepAxisY_Pos.Text),
                int.Parse(editStepAxisY_Acc.Text), int.Parse(editStepAxisY_Dec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }
        private void btnMoveStepAxisZ_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, int.Parse(editStepAxisZ_Speed.Text), double.Parse(editStepAxisZ_Pos.Text),
                int.Parse(editStepAxisZ_Acc.Text), int.Parse(editStepAxisZ_Dec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }
        private void btnMoveStepGripperAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, int.Parse(editStepGripper_Speed.Text), double.Parse(editStepGripper_Pos.Text),
                int.Parse(editStepAxisGripper_Acc.Text), int.Parse(editStepAxisGripper_Dec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }
        private void btnMoveStepPipettAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, int.Parse(editStepPipett_Speed.Text), double.Parse(editStepPipett_Pos.Text),
                int.Parse(editStepAxisHam_Acc.Text), int.Parse(editStepAxisHam_Dec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }

        /////////////////////////////////////////////////
        // Step Motor Home Position Move (All Axis)
        /////////////////////////////////////////////////
        private void btnInitializePos_Click(object sender, EventArgs e)
        {
            int timeout = 1000;
            int i = 0;

            if (bStepMotorInitDoneState != true)
                return;

            GripAxState.bHOME_COMP = false;
            HamAxState.bHOME_COMP = false;
            Step2AxState.bHOME_COMP = false;

            StepMotorHomeMove(MOTOR.STEP2, "HOME", "");
            StepMotorHomeMove(MOTOR.GRIP, "HOME", "");
            StepMotorHomeMove(MOTOR.HAM, "HOME", "");

            while (GripAxState.bHOME_COMP != true || HamAxState.bHOME_COMP != true || Step2AxState.bHOME_COMP != true)
            {
                while (i < timeout)
                {
                    i++;
                    Thread.Sleep(200);
                    GetStatus(bSilent: true);
                    if (GripAxState.bHOME_COMP == true && HamAxState.bHOME_COMP == true && Step2AxState.bHOME_COMP == true)
                        break;
                }
                Thread.Sleep(200);
            }

            StepMotorHomeMove(MOTOR.STEP0, "HOME", "");
            StepMotorHomeMove(MOTOR.STEP1, "HOME", "");
            
            //StepMotorHomeMove(MOTOR.COVER, "HOME", "");

            //MoveHomeStepMotor();
        }

        public bool SetStepMotorHomeMoveParam()
        {
            if (bSystemInitDoneState == true)
            {
                StepMotorHomeMove(MOTOR.HAM, "OFFSET", config.HomeOffsetPos_Ham, timeout:100);
                StepMotorHomeMove(MOTOR.GRIP, "OFFSET", config.HomeOffsetPos_Grip, timeout: 100);
                StepMotorHomeMove(MOTOR.COVER, "OFFSET", config.HomeOffsetPos_Cover, timeout: 100);
            }

            StepMotorHomeMove(MOTOR.HAM, "HOME_SPD", config.HomeSearchSpd_Ham);
            StepMotorHomeMove(MOTOR.GRIP, "HOME_SPD", config.HomeSearchSpd_Grip);
            StepMotorHomeMove(MOTOR.COVER, "HOME_SPD", config.HomeSearchSpd_Cover);

            return true;
        }

        public bool MoveHomeStepMotor(bool bStateCheck = true)
        {
            iPrintf("Initializing Step Motor");

            if (bStateCheck == true)
            {
                // verify current status
                GetStatus(true);
                Thread.Sleep(200);
            }

            if (GripAxState.bHOME_COMP != true)
            {
                iPrintf("Initializing Step Motor ... Gripper Axis to Home");
                if (StepMotorHomeMove(MOTOR.GRIP, "HOME", "") != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Gripper Axis to Home ... Fail");
                    return false;
                }
                else
                {
                    iPrintf("Gripper Axis Homing Done!");
                }
            }

            if (HamAxState.bHOME_COMP != true)
            {
                iPrintf("Initializing Step Motor ... Pipett Axis to Home");
                if (StepMotorHomeMove(MOTOR.HAM, "HOME", "") != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Pipett Axis to Home ... Fail");
                    return false;
                }
                else
                {
                    iPrintf("Hamilton Axis Homing Done!");
                }
            }

            if (Step2AxState.bHOME_COMP != true)
            {
                iPrintf("Initializing Step Motor ... Axis Z to Home");
                if (StepMotorHomeMove(MOTOR.STEP2, "HOME", "") != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis Z to Home ... Fail");
                    return false;
                }
                else
                {
                    iPrintf("Z Axis Homing Done!");
                }
            }
            
            // wait for 3 axes homing done
            int timeout = 1000;
            int i = 0;

            while (GripAxState.bHOME_COMP != true || HamAxState.bHOME_COMP != true || Step2AxState.bHOME_COMP != true)
            {
                while (i < timeout)
                {
                    i++;
                    Thread.Sleep(200);
                    GetStatus(bSilent: true);
                    if (GripAxState.bHOME_COMP == true && HamAxState.bHOME_COMP == true && Step2AxState.bHOME_COMP == true)
                        break;
                }
                Thread.Sleep(200);
            }

            iPrintf("Gripper, Hamilton, Z Motor Homing Done!");

            //if (CoverAxState.bHOME_COMP != true)
            //{
            //    iPrintf("Initializing Step Motor ... Axis Cover to Home");

            //    if (StepMotorHomeMove(MOTOR.COVER, "HOME", "") != COM_Status.ACK)
            //    {
            //        iPrintf("Initializing Step Motor ... Axis Cover to Home ... Fail");
            //        return false;
            //    }
            //    else
            //    {
            //        iPrintf("Cover Axis Homing Done!");
            //    }
            //}

            // 충돌 방지를 위해 XY축 이동을 가장 마지막에 수행해야 함
            Thread.Sleep(500);
            if (Step1AxState.bHOME_COMP != true)
            {
                iPrintf("Initializing Step Motor ... Axis Y to Home");
                if (StepMotorHomeMove(MOTOR.STEP1, "HOME", "") != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis Y to Home ... Fail");
                    return false;
                }
                else
                {
                    iPrintf("Y Axis Homing Done!");
                }
            }

            if (Step0AxState.bHOME_COMP != true)
            {
                iPrintf("Initializing Step Motor ... Axis X to Home");
                if (StepMotorHomeMove(MOTOR.STEP0, "HOME", "") != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis X to Home ... Fail");
                    return false;
                }
                else
                {
                    iPrintf("X Axis Homing Done!");
                }
            }
                        
            // wait for 3 axes homing done
            i = 0;
            while (Step0AxState.bHOME_COMP != true || Step1AxState.bHOME_COMP != true)
                   //|| CoverAxState.bHOME_COMP != true)
            {
                while (i < timeout)
                {
                    i++;
                    Thread.Sleep(200);
                    GetStatus(bSilent: true);
                    
                    //CoverAxState.bHOME_COMP = true; // for test            
                    if (Step0AxState.bHOME_COMP == true && Step1AxState.bHOME_COMP == true)
                        //&& CoverAxState.bHOME_COMP == true)
                        break;
                }
                Thread.Sleep(200);
            }

            iPrintf("X, Y, Cover Motor Homing Done!");
            iPrintf("All Motors Homing Done!!");

            bStepMotorInitDoneState = true;
            Thread.Sleep(200);

            return true;
        }

        /////////////////////////////////////////////////
        // TriContinet Pump Control
        /////////////////////////////////////////////////
        
        private double Volume_mL = 0.0;
        private double Vol_mL_per_Inc = 0.0;
        public double pipett_scale = 0.83;

        // mL 단위로 입력된 체적을 펌프의 increment로 환산
        private double ConvertUnitVol_mL_To_Inc(PERIPHERAL device)
        {
            if (device == PERIPHERAL.TRI_PUMP)
            {
                Volume_mL = double.Parse(editPumpLoadingVolume.Text) + double.Parse(editPumpOffsetVolume.Text);
                Vol_mL_per_Inc = PumpSyringe_Vol / PumpMax_Increment * 0.001;
            }
            else if(device == PERIPHERAL.TRI_PIPETT)
            {
                Volume_mL = pipett_scale * (double.Parse(editTriPipettLoadingVolume.Text) + double.Parse(editTriPipettOffsetVolume.Text));
                Vol_mL_per_Inc = TriPipett_Vol / TriPipett_Max_Increment * 0.001;
            }

            double Vol_IncResult = Volume_mL / Vol_mL_per_Inc;

            return Vol_IncResult;
        }

        private double FlowRate_IncPerSecResult;

        // mL/min 단위로 입력된 flow rate을 inc/sec로 환산
        private double ConvertUnitFlowRate_mlPerSec_To_IncPerSec(PERIPHERAL device)
        {
            if (device == PERIPHERAL.TRI_PUMP)
            {
                double FlowRate_uL_per_sec = double.Parse(editPumpFlowRate.Text) * 1000;
                FlowRate_IncPerSecResult = FlowRate_uL_per_sec * (double)(PumpVel_Resolution / PumpSyringe_Vol);
            }
            else if(device == PERIPHERAL.TRI_PIPETT)
            {
                double FlowRate_uL_per_sec = pipett_scale * double.Parse(editTriPipettFlowRate.Text) * 1000;
                //FlowRate_IncPerSecResult = FlowRate_uL_per_sec * (double)(TriPipett_Vel_Resolution / TriPipett_Vol);
                FlowRate_IncPerSecResult = FlowRate_uL_per_sec / TriPipett_ul_per_inc;
            }

            return FlowRate_IncPerSecResult;
        }
        
        private void btnAspirateLiquidPump_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(VALVE.OPEN);   // Open

                if (double.Parse(editPumpLoadingVolume.Text) < 0 || double.Parse(editPumpLoadingVolume.Text) > 12.5)
                {
                    iPrintf("Invalid Value! Pumping Volume range = 0 ~ 12.5 mL");
                    return;
                }

                if (double.Parse(editPumpOffsetVolume.Text) < 0 || double.Parse(editPumpOffsetVolume.Text) > 1)
                {
                    iPrintf("Invalid Value! Pumping Offset Volume range = 0 ~ 1 mL");
                    return;
                }

                if (double.Parse(editPumpFlowRate.Text) < 0.01 || double.Parse(editPumpFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pump Flow Rate range = 0.01 ~ 1.5 mL/sec");
                    return;
                }

                // 단위환산
                double Volume_inc = Math.Round(ConvertUnitVol_mL_To_Inc(PERIPHERAL.TRI_PUMP));
                double FlowRate_inc_per_sec = Math.Round(ConvertUnitFlowRate_mlPerSec_To_IncPerSec(PERIPHERAL.TRI_PUMP));

                RunPer3_TricontinentPump((byte)' ', int.Parse(editPumpPortNo.Text), // N/A
                                         (byte)'A', 0,                              // 절대위치 0로 복귀
                                         (int) FlowRate_inc_per_sec,                // Plunger Speed 설정
                                         (byte)'P', (int) Volume_inc);              // aspiration 방향으로 volume값 만큼 이동

                RunPer3_TricontinentPump((byte)' ', 0,                              // N/A
                                         (byte)'?', 0,                              // Request Current Plunger Position
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A

                //SerPinchValve(VALVE.CLOSE);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnDispenseLiquidPump_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(VALVE.OPEN);   // Open

                if (double.Parse(editPumpLoadingVolume.Text) < 0 || double.Parse(editPumpLoadingVolume.Text) > 12.5)
                {
                    iPrintf("Invalid Value! Pumping Volume range = 0 ~ 12.5 mL");
                    return;
                }

                if (double.Parse(editPumpOffsetVolume.Text) < 0 || double.Parse(editPumpOffsetVolume.Text) > 1)
                {
                    iPrintf("Invalid Value! Pumping Offset Volume range = 0 ~ 1 mL");
                    return;
                }

                if (double.Parse(editPumpFlowRate.Text) < 0.01 || double.Parse(editPumpFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pump Flow Rate range = 0.01 ~ 1.5 mL/sec");
                    return;
                }

                // 단위환산
                double Volume_inc = Math.Round(ConvertUnitVol_mL_To_Inc(PERIPHERAL.TRI_PUMP));
                double FlowRate_inc_per_sec = Math.Round(ConvertUnitFlowRate_mlPerSec_To_IncPerSec(PERIPHERAL.TRI_PUMP));

                RunPer3_TricontinentPump((byte)' ', int.Parse(editPumpPortNo.Text), // N/A
                                         (byte)' ', 0,                              // N/A
                                         (int)FlowRate_inc_per_sec,                 // Plunger Speed 설정
                                         (byte)'D', (int)Volume_inc);               // dispense 방향으로 volume값 만큼 이동

                RunPer3_TricontinentPump((byte)' ', 0,                              // N/A
                                         (byte)'?', 0,                              // Request Current Plunger Position
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A

                //SerPinchValve(VALVE.CLOSE);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnStopPump_Click(object sender, EventArgs e)
        {
            try
            {
                //SerPinchValve(VALVE.OPEN);

                if (double.Parse(editPumpLoadingVolume.Text) < 0 || double.Parse(editPumpLoadingVolume.Text) > 12.5)
                {
                    iPrintf("Invalid Value! Pumping Volume range = 0 ~ 12.5 mL");
                    return;
                }
                
                if (double.Parse(editPumpOffsetVolume.Text) < 0 || double.Parse(editPumpOffsetVolume.Text) > 1)
                {
                    iPrintf("Invalid Value! Pumping Offset Volume range = 0 ~ 1 mL");
                    return;
                }

                if (double.Parse(editPumpFlowRate.Text) < 0.01 || double.Parse(editPumpFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pump Flow Rate range = 0.01 ~ 1.5 mL/sec");
                    return;
                }

                RunPer3_TricontinentPump((byte)' ', 0,                  // N/A
                                         (byte)'T', 0,                  // Terminate Executing Command
                                         0,                             // N/A
                                         (byte)' ', 0);                 // N/A

                RunPer3_TricontinentPump((byte)' ', 0,                  // N/A
                                         (byte)'?', 0,                  // Request Current Plunger Position
                                         0,                             // N/A
                                         (byte)' ', 0);                 // N/A

                SerPinchValve(VALVE.CLOSE);       // Close
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }
        private void btnSelectPumpPortNoCW_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer3_TricontinentPump((byte)'I', int.Parse(editPumpPortNo.Text), // output port selection
                                         (byte)'?', 0,                              // N/A
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnSelectPumpPortNoCCW_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer3_TricontinentPump((byte)'O', int.Parse(editPumpPortNo.Text), // output port selection
                                         (byte)'?', 0,                              // N/A
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnPumpPriming_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer3_TricontinentPump((byte)' ', 0,                              // N/A
                                         (byte)'P', 0,                              // Priming Pump
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }


        private void btnPumpInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer3_TricontinentPump((byte)' ', int.Parse(editPumpPortNo.Text), // N/A
                                         (byte)'Z', 0,                              // Initialize Pump
                                         0,                                         // N/A
                                         (byte)' ', 0);                             // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        public void ConfirmPlungerPosition()
        {
            RunPer1_TricontinentPipett((byte)'?', 0,                  // Request Current Plunger Position
                                         0,                             // N/A
                                       (byte)' ', 0);                 // N/A

            // 두번 물어봐야 답을 해주는데 이유는 알 수 없음
            RunPer1_TricontinentPipett((byte)'?', 0,                  // Request Current Plunger Position
                                        0,                             // N/A
                                       (byte)' ', 0);                 // N/A
        }
         
        /////////////////////////////////////////////////
        // TriContinet Pipett Module Control
        /////////////////////////////////////////////////
        private void btnAspirateLiquidTriPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editTriPipettLoadingVolume.Text) < 0 || double.Parse(editTriPipettLoadingVolume.Text) > 5)
                {
                    iPrintf("Invalid Value! Pipett Volume range = 0 ~ 5 mL");
                    return;
                }

                if (double.Parse(editTriPipettOffsetVolume.Text) < 0 || double.Parse(editTriPipettOffsetVolume.Text) > 0.5)
                {
                    iPrintf("Invalid Value! Pipett Offset Volume range = 0 ~ 0.5 mL");
                    return;
                }

                if (double.Parse(editTriPipettFlowRate.Text) < 0.01 || double.Parse(editTriPipettFlowRate.Text) > 22.85)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 22.85 mL/sec");
                    return;
                }

                // 단위환산
                double Volume_inc = Math.Round(ConvertUnitVol_mL_To_Inc(PERIPHERAL.TRI_PIPETT));
                double FlowRate_inc_per_sec = Math.Round(ConvertUnitFlowRate_mlPerSec_To_IncPerSec(PERIPHERAL.TRI_PIPETT));
                int duration_ms = (int) Math.Round((Volume_inc / FlowRate_inc_per_sec) * 1000);

                if(Volume_inc < 0 || Volume_inc > TriPipett_Max_Increment)
                {
                    iPrintf(String.Format("Invalid Value! Pipett Volume Pulse Count range = 0 ~ 1600 (input: {0})", Volume_inc));
                    return;
                }

                RunPer1_TricontinentPipett((byte)' ', 0,                  // 절대위치 0로 복귀
                                           (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                           (byte)'P', (int)Volume_inc);   // aspiration 방향으로 volume값 만큼 이동

                Thread.Sleep(duration_ms);
                
                ConfirmPlungerPosition();
                iPrintf(string.Format("duration: {0} ms", duration_ms));
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnDispenseLiquidTriPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editTriPipettLoadingVolume.Text) < 0 || double.Parse(editTriPipettLoadingVolume.Text) > 5)
                {
                    iPrintf("Invalid Value! Pipett Volume range = 0 ~ 5 mL");
                    return;
                }

                if (double.Parse(editTriPipettOffsetVolume.Text) < 0 || double.Parse(editTriPipettOffsetVolume.Text) > 0.5)
                {
                    iPrintf("Invalid Value! Pipett Offset Volume range = 0 ~ 0.5 mL");
                    return;
                }

                if (double.Parse(editTriPipettFlowRate.Text) < 0.01 || double.Parse(editTriPipettFlowRate.Text) > 22.85)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 22.85 mL/sec");
                    return;
                }

                // 단위환산
                double Volume_inc = Math.Round(ConvertUnitVol_mL_To_Inc(PERIPHERAL.TRI_PIPETT));
                double FlowRate_inc_per_sec = Math.Round(ConvertUnitFlowRate_mlPerSec_To_IncPerSec(PERIPHERAL.TRI_PIPETT));
                int duration_ms = (int)Math.Round((Volume_inc / FlowRate_inc_per_sec) * 1000);

                RunPer1_TricontinentPipett((byte)' ', 0,                  // N/A
                                           (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                           (byte)'D', (int)Volume_inc);   // dispense 방향으로 volume값 만큼 이동

                Thread.Sleep(duration_ms);

                ConfirmPlungerPosition();

                iPrintf(string.Format("duration: {0} ms", duration_ms));
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnStopTriPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editTriPipettLoadingVolume.Text) < 0 || double.Parse(editTriPipettLoadingVolume.Text) > 5)
                {
                    iPrintf("Invalid Value! Pipett Volume range = 0 ~ 5 mL");
                    return;
                }

                if (double.Parse(editTriPipettOffsetVolume.Text) < 0 || double.Parse(editTriPipettOffsetVolume.Text) > 0.5)
                {
                    iPrintf("Invalid Value! Pipett Offset Volume range = 0 ~ 0.5 mL");
                    return;
                }

                if (double.Parse(editTriPipettFlowRate.Text) < 0.01 || double.Parse(editTriPipettFlowRate.Text) > 22.85)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 22.85 mL/sec");
                    return;
                }

                RunPer1_TricontinentPipett((byte)'T', 0,                  // Terminate Executing Command
                                           0,                             // N/A
                                           (byte)' ', 0);                 // N/A

                Thread.Sleep(1000);

                ConfirmPlungerPosition();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTriPipettInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600V1000A0A1580R", Environment.NewLine));
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTriPipettPosRead_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer1_TricontinentPipett((byte)'?', 0,                 // Initialize Pipett
                                            0,                           // N/A
                                           (byte)' ', 0);                // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnPE1_PlungerTrgPosRel_Click(object sender, EventArgs e)
        {
            if(int.Parse(editPE1_PlungerTrgPosRel.Text) >= 0)
            {
                RunPer1_TricontinentPipett((byte)' ', 0,  // N/A
                                            0,       // 현재 지정된 속도로 이동
                                           (byte)'P', Math.Abs(int.Parse(editPE1_PlungerTrgPosRel.Text)));     // 지정된 상대 위치로 이동
                Thread.Sleep(1000);
                ConfirmPlungerPosition();
            }
            else
            {
                RunPer1_TricontinentPipett((byte)' ', 0,  // N/A
                                            0,       // 현재 지정된 속도로 이동
                                           (byte)'D', Math.Abs(int.Parse(editPE1_PlungerTrgPosRel.Text)));     // 지정된 상대 위치로 이동
                Thread.Sleep(1000);
                ConfirmPlungerPosition();
            }
        }

        private void btnPE1_PlungerTrgPosAbs_Click(object sender, EventArgs e)
        {
            RunPer1_TricontinentPipett((byte)'A', int.Parse(editPE1_PlungerTrgPosAbs.Text),  // 지정된 절대 위치로 이동
                                        0,       // 현재 지정된 속도로 이동
                                       (byte)' ', 0);     // N/A
        }

        /////////////////////////////////////////////////
        // Hamilton Pipett Module Control
        /////////////////////////////////////////////////
        private void btnPlungerInit_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer2_HamiltonPipett("DI", 0, 0, 0, 0, TIP_TYPE.NONE);

                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTipInit_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.001 || double.Parse(editHamPipettFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.001 ~ 1.5 (* mL/sec)");
                    return;
                }

                //입력: mL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 10000.0;    // mL/s -> uL/s

                RunPer2_HamiltonPipett("DE", 0, 0,(int) flowrate, 0, TIP_TYPE.NONE);

                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTipDiscard_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer2_HamiltonPipett("TD", 0, 0, 0, 0, TIP_TYPE.NONE);
                
                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTipInsert_HamPipett_Click(object sender, EventArgs e)
        {
            int TipType = 0;

            try
            {
                if (label_TipType.Text == "None" || label_TipType.Text == "0")
                {
                    iPrintf("Invalid Value! Hamilton Tip Type Not Selected");
                    return;
                }
                else if (label_TipType.Text == "10 uL")
                {
                    TipType = (int) TIP_TYPE._10UL;
                }
                else if (label_TipType.Text == "300 uL")
                {
                    TipType = (int)TIP_TYPE._300UL;
                }
                else if (label_TipType.Text == "1000 uL" || label_TipType.Text == "Cal_Ball" || label_TipType.Text == "Cal_Pin")
                {
                    TipType = (int)TIP_TYPE._1000UL;
                }

                //comboBox_TipType.SelectedItem.ToString
                RunPer2_HamiltonPipett("TP", 0, 0, 0, 0, (TIP_TYPE) TipType);
                
                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnAirAspirate_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.001 || double.Parse(editHamPipettFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.001 ~ 1.5 (mL/sec)");
                    return;
                }

                if (double.Parse(editHamPipettAirBlowOutVol.Text) < 0.0 || double.Parse(editHamPipettAirBlowOutVol.Text) > 1)
                {
                    iPrintf("Invalid Value! Pipett Air Blow Out Volume range = 0 ~ 1 mL");
                    return;
                }

                //입력: mL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 10000.0;    // mL/s -> uL/s
                //입력: mL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettAirBlowOutVol.Text) * 10000.0;    // mL -> 0.1uL
                
                RunPer2_HamiltonPipett("AB",(int) vol1, 0, (int)flowrate, 0, TIP_TYPE.NONE);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnTransportAir_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.001 || double.Parse(editHamPipettFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.001 ~ 1.5 (mL/sec)");
                    return;
                }

                if (double.Parse(editHamPipettTranportAirVol.Text) < 0.0 || double.Parse(editHamPipettTranportAirVol.Text) > 1)
                {
                    iPrintf("Invalid Value! Pipett Transport Air Volume range = 0 ~ 1 mL");
                    return;
                }

                //입력: mL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 1000.0;    // mL/s -> uL/s
                //입력: mL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettTranportAirVol.Text) * 10000;    // mL -> 0.1uL

                RunPer2_HamiltonPipett("AT", (int)vol1, 0, (int)flowrate, 0, TIP_TYPE.NONE);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnVolumeConfirm_HamPipett_Click(object sender, EventArgs e)
        {
            RunPer2_HamiltonPipett("VT", 0, 0, 0, 0, TIP_TYPE.NONE);
        }

        private void btnLiquidDispense_HamPipett_Click(object sender, EventArgs e)
        {
            int mov_opt = 0;

            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.001 || double.Parse(editHamPipettFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.001 ~ 1.5 (mL/sec)");
                    return;
                }

                if (double.Parse(editHamPipettDispenseVol.Text) < 0.0 || double.Parse(editHamPipettDispenseVol.Text) > 1.1)
                {
                    iPrintf("Invalid Value! Pipett Liquid Dispense Volume range = 0 ~ 1.1 mL");
                    return;
                }

                if (double.Parse(editHamPipettStopBackVol.Text) < 0.0 || double.Parse(editHamPipettStopBackVol.Text) > 0.0325)
                {
                    iPrintf("Invalid Value! Pipett Stop Back Volume range = 0 ~ 0.0325 mL");
                    return;
                }
                int stop_spd = 0;       //입력값: 0~1500 uL/sec, 실험 후 config 혹은 UI에 추가예정
                if(bHamilton_Z_Follow_Flag == true)
                {
                    mov_opt = 1;
                }
                else if(bHamilton_Z_Follow_Flag == false)
                {
                    mov_opt = 0;
                }

                double spd_scale = double.Parse(editZMoveSpdScale.Text);
                //입력: mL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 10000.0;  // mL/s -> uL/s
                //입력: mL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettDispenseVol.Text) * 10000.0;    // mL -> 0.1uL
                //입력: mL, 모듈전송: 0.1uL
                double vol2 = double.Parse(editHamPipettStopBackVol.Text) * 10000.0;    // mL -> 0.1uL
                
                int duration_ms = (int)Math.Round((vol1 / flowrate) * 1000) - 200;
                
                if (mov_opt == 1)
                {
                    Liquid_Z_Follow_Move(vol1*0.0001, flowrate * 0.0001, Tube_ID_15ml, Lead_AxisZ, Z_FOLLOW_DIR.UP, PERIPHERAL.HAM_PIPETT);    //vol: ml, flowrate: ml/s
                }

                RunPer2_HamiltonPipett("DL", (int)vol1, (int)vol2, (int)flowrate, stop_spd, TIP_TYPE.NONE, timeout: 150);

                //if (SensorStatus.ham_pipett_errNo != 0)
                if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                {
                    if (mov_opt == 1)
                        MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);

                    iPrintf("Hamilton Pipett Error! Dispense Aborted!");
                    bStepRunState = false;
                    MonitorStepMotorStatus();
                    bPipettMotion = false;
                    return;
                }

                Thread.Sleep(duration_ms);
                iPrintf(string.Format("Set Step Run Flag Low! duration: {0}", duration_ms));
                bStepRunState = false;
                MonitorStepMotorStatus();
                bPipettMotion = false;
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnLiquidAspirate_HamPipett_Click(object sender, EventArgs e)
        {
            int mov_opt = 0;

            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.001 || double.Parse(editHamPipettFlowRate.Text) > 1.5)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.001 ~ 1.5 (mL/sec)");
                    return;
                }

                if (double.Parse(editHamPipettAspirateVol.Text) < 0.0 || double.Parse(editHamPipettAspirateVol.Text) > 1.1)
                {
                    iPrintf("Invalid Value! Pipett Liquid Aspirate Volume range = 0 ~ 1.1 mL");
                    return;
                }

                if (double.Parse(editHamPipettOverAspirateVol.Text) < 0.0 || double.Parse(editHamPipettOverAspirateVol.Text) > 1)
                {
                    iPrintf("Invalid Value! Pipett Over Aspirate Volume range = 0 ~ 1 mL");
                    return;
                }
                int stop_spd = 0;       //입력값: 0~1500 uL/sec, 실험 후 config 혹은 UI에 추가예정
                if (bHamilton_Z_Follow_Flag == true)
                {
                    mov_opt = 1;
                }
                else if (bHamilton_Z_Follow_Flag == false)
                {
                    mov_opt = 0;
                }

                double spd_scale = double.Parse(editZMoveSpdScale.Text);
                //입력: mL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 10000.0;  // mL/s -> uL/s
                //입력: mL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettAspirateVol.Text) * 10000.0;    // mL -> 0.1uL
                //입력: mL, 모듈전송: 0.1uL
                double vol2 = double.Parse(editHamPipettOverAspirateVol.Text) * 10000.0;    // mL -> 0.1uL

                int duration_ms = (int)Math.Round((vol1 / flowrate) * 1000) - 200;

                if (mov_opt == 1)
                {
                    Liquid_Z_Follow_Move(vol1*0.0001, flowrate*0.0001, Tube_ID_15ml, Lead_AxisZ, Z_FOLLOW_DIR.DOWN, PERIPHERAL.HAM_PIPETT);    //vol: ml, flowrate: ml/s
                }

                RunPer2_HamiltonPipett("AL", (int)vol1, (int)vol2, (int)flowrate, stop_spd, 0, timeout:150);

                //if (SensorStatus.ham_pipett_errNo != 0)
                if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                {
                    if (mov_opt == 1)
                        MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);

                    iPrintf("Hamilton Pipett Error! Aspirate Aborted!");
                    bStepRunState = false;
                    MonitorStepMotorStatus();
                    bPipettMotion = false;
                    return;
                }

                Thread.Sleep(duration_ms);
                iPrintf(string.Format("Set Step Run Flag Low! duration: {0}", duration_ms));
                bStepRunState = false;
                MonitorStepMotorStatus();
                bPipettMotion = false;
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Hamilton Pipett Module State Confirm
        /////////////////////////////////////////////////
        private bool ConfirmTipPresence()
        {
            bool retVal = false;

            try
            {
                checkedListBox_TipPresence.SetItemChecked(0, false);
                checkedListBox_TipPresence.SetItemChecked(1, false);
                checkedListBox_TipPresence.SetItemChecked(2, false);

                if(RunPer2_HamiltonPipett("RT", 0, 0, 0, 0, TIP_TYPE.NONE) == COM_Status.ACK)
                {
                    ;
                }
                else
                {
                    iPrintf("Tip Presence Check Fail!");
                    return false;
                }

                if (nTipPresence == 1)
                {
                    label_TipPresence.Text = "Present";
                    checkedListBox_TipPresence.SetItemChecked(2, true);
                    nTipPresence = 2;
                    retVal = true;
                }
                else if(nTipPresence == 0)
                {
                    label_TipPresence.Text = "No Tip";
                    checkedListBox_TipPresence.SetItemChecked(1, true);
                    nTipPresence = 2;
                    retVal = false;
                }
                else
                {
                    label_TipPresence.Text = "-";
                    checkedListBox_TipPresence.SetItemChecked(0, true);
                    nTipPresence = 2;
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

            return retVal;
        }

        private bool ConfirmcLLD_State()
        {
            bool retVal = false;

            try
            {
                checkedListBox_StateLLD.SetItemChecked(0, false);
                checkedListBox_StateLLD.SetItemChecked(1, false);
                checkedListBox_StateLLD.SetItemChecked(2, false);
                checkedListBox_StateLLD.SetItemChecked(3, false);

                if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, TIP_TYPE.NONE) == COM_Status.ACK && nState_cLLD == 3)
                {
                    label_cLLDState.Text = "Detected";
                    checkedListBox_StateLLD.SetItemChecked(3, true);
                    nState_cLLD = 0;
                    retVal = true;
                }
                else if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, TIP_TYPE.NONE) == COM_Status.ACK && nState_cLLD == 2)
                {
                    label_cLLDState.Text = "Searching";
                    checkedListBox_StateLLD.SetItemChecked(2, true);
                    nState_cLLD = 0;
                    retVal = false;
                }
                else if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, TIP_TYPE.NONE) == COM_Status.ACK && nState_cLLD == 1)
                {
                    label_cLLDState.Text = "Idle";
                    checkedListBox_StateLLD.SetItemChecked(1, true);
                    nState_cLLD = 0;
                    retVal = false;
                }
                else
                {
                    label_cLLDState.Text = "-";
                    checkedListBox_StateLLD.SetItemChecked(0, true);
                    nState_cLLD = 0;
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

            return retVal;
        }

        private void btnTipPresence_Click(object sender, EventArgs e)
        {
            ConfirmTipPresence();
        }

        private void btncLLDState_Click(object sender, EventArgs e)
        {
            //ConfirmcLLD_State();
            Run_Hamilton_cLLD((byte)'V', 0);
        }

        private void btnStartcLLD_Click(object sender, EventArgs e)
        {
            //Run_Hamilton_cLLD((byte)'L', int.Parse(strcLLD_Sensitivity));
            Run_Hamilton_cLLD((byte)'L', 1);
        }

        private void btnStopcLLD_Click(object sender, EventArgs e)
        {
            Run_Hamilton_cLLD((byte)'P', 0);
        }

        public bool bLLD_Stop_Flag = false;

        /////////////////////////////////////////////////
        // Hamilton Pipett cLLD Start/Stop
        /////////////////////////////////////////////////
        private void btnStartcLLD_Ham_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_SensitivitycLLD.SelectedIndex.ToString() == "None" ||
                    comboBox_SensitivitycLLD.SelectedIndex.ToString() == "0" ||
                    comboBox_DirAxiscLLD.SelectedIndex.ToString() == "None" ||
                    comboBox_DirAxiscLLD.SelectedIndex.ToString() == "0")
                {
                    if (comboBox_SensitivitycLLD.SelectedIndex.ToString() == "None" || 
                        comboBox_SensitivitycLLD.SelectedIndex.ToString() == "0")
                        iPrintf("cLLD Sensitivity Option Not Selected!");
                    if (comboBox_DirAxiscLLD.SelectedIndex.ToString() == "None" || 
                        comboBox_DirAxiscLLD.SelectedIndex.ToString() == "0")
                        iPrintf("cLLD Direction & Axis Option Not Selected!");
                    return;
                }

                iPrintf("Start Liquid Level Detection..");

                string axis = "";
                if (strcLLD_Dir_Axis[1] == 'X')
                    axis = "STEP0";
                else if (strcLLD_Dir_Axis[1] == 'Y')
                    axis = "STEP1";
                else if (strcLLD_Dir_Axis[1] == 'Z')
                    axis = "STEP2";
                else
                    return;

                var trg_axis = (MOTOR)Enum.Parse(typeof(MOTOR), axis);
                int dir = 0;
                if (strcLLD_Dir_Axis[0] == '+')
                    dir = 1;
                else if (strcLLD_Dir_Axis[0] == '-')
                    dir = -1;
                double timeout = 1000;
                int i = 0;
                bLLD_Stop_Flag = false;
                timeout = 1.6 * (double.Parse(editcLLD_MaxPosition.Text) / (double.Parse(editcLLD_Speed.Text) * 0.001));

                Run_Hamilton_cLLD((byte)'L', int.Parse(strcLLD_Sensitivity));
                Thread.Sleep(50);
                SystemCmd("ESCAPE", "", "");
                Thread.Sleep(50);

                MoveStepMotor(STEP_CMD.MOVE, trg_axis, int.Parse(editcLLD_Speed.Text), dir * double.Parse(editcLLD_MaxPosition.Text), 
                                        3, 3, POS_OPT.REL, HOLD_STATE.NONE);
                
                if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                {
                    MoveStepMotor(STEP_CMD.STOP, trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                    Run_Hamilton_cLLD((byte)'P', 0);
                    iPrintf("Hamilton Pipett Error! cLLD Aborted!");
                    MonitorStepMotorStatus();
                    return;
                }

                while (i < timeout)
                 {
                    if (bLLD_Stop_Flag == true)
                        break;

                    i++;
                    Thread.Sleep(1);
                    Run_Hamilton_cLLD((byte)'V', 0);

                    if (bcLLD_IO == true || bcLLD_Detected == true)
                    {
                        iPrintf(String.Format("level I/O: {0}, detected msg: {1}", bcLLD_IO, bcLLD_Detected));

                        MoveStepMotor(STEP_CMD.STOP, trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
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

                        if (trg_axis == MOTOR.STEP0)
                        {
                            label_cLLDAxisPos.Text = CurrentPos.Step0AxisX.ToString("F2");
                            iPrintf(String.Format("Axis X: {0}", CurrentPos.Step0AxisX));
                        }
                        else if (trg_axis == MOTOR.STEP1)
                        {
                            label_cLLDAxisPos.Text = CurrentPos.Step1AxisY.ToString("F2");
                            iPrintf(String.Format("Axis Y: {0}", CurrentPos.Step1AxisY));
                        }
                        else if (trg_axis == MOTOR.STEP2)
                        {
                            label_cLLDAxisPos.Text = CurrentPos.Step2AxisZ.ToString("F2");
                            iPrintf(String.Format("Axis Z: {0}", CurrentPos.Step2AxisZ));
                        }

                        break;
                    }

                    if( i >= timeout)
                    {
                        MoveStepMotor(STEP_CMD.STOP, trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                        Run_Hamilton_cLLD((byte)'P', 0);
                        iPrintf("Liquid Level Not Detected!");
                        MonitorStepMotorStatus();
                    }
                 }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnStopcLLD_Ham_Click(object sender, EventArgs e)
        {
            try
            {
                string axis = "";
                if (strcLLD_Dir_Axis[1] == 'X')
                    axis = "STEP0";
                else if (strcLLD_Dir_Axis[1] == 'Y')
                    axis = "STEP1";
                else if (strcLLD_Dir_Axis[1] == 'Z')
                    axis = "STEP2";
                else
                    return;

                var trg_axis = (MOTOR)Enum.Parse(typeof(MOTOR), axis);
                
                MoveStepMotor(STEP_CMD.STOP, trg_axis, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
                Run_Hamilton_cLLD((byte)'P', 0);
                iPrintf("Stop Liquid Level Detection!!");
                bLLD_Stop_Flag = true;

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
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Music On
        /////////////////////////////////////////////////
        private void chk_play_music_on_test_CheckedChanged(object sender, EventArgs e)
        {
            config.PlayMusicOntest = chk_play_music_on_test.Checked;
        }


        /////////////////////////////////////////////////
        // Peltier Module On/Off
        /////////////////////////////////////////////////
        private void btnRunPeltier_Click(object sender, EventArgs e)
        {
            WritePeltier(PELT_CMD.SET_SV, bPeltier: true, float.Parse(edit_pelt_set_temp.Text), 0, 0);
            WritePeltier(PELT_CMD.SET_FAN, false, 0, float.Parse(edit_fan_on_temp.Text), float.Parse(edit_fan_off_temp.Text), timeout: 120);
            ReadPeltierTemp();

            if (bSerialTimerState == false)
                btnTimer_Click(this, null);
        }

        private void btnStopPeltier_Click(object sender, EventArgs e)
        {
            string fan_on_temp_forStep, fan_off_temp_forStep;

            WritePeltier(PELT_CMD.SET_SV, bPeltier: false, 0, 0, 0);
            //Thread.Sleep(200);

            fan_on_temp_forStep = string.Format("{0}", fPeltOffTemp - 10);
            fan_off_temp_forStep = string.Format("{0}", fPeltOffTemp + 10);
            WritePeltier(PELT_CMD.SET_FAN, false, 0, float.Parse(fan_on_temp_forStep), float.Parse(fan_off_temp_forStep), timeout: 120);
            Thread.Sleep(200);
            ReadPeltier(PELT_CMD.CHAMBER_SV, timeout: 100);
            Thread.Sleep(500);
            ReadPeltierTemp();

            if (bSerialTimerState == true)
                btnTimer_Click(this, null);
        }

        private void btnReadTemperature_Click(object sender, EventArgs e)
        {
            ReadPeltierTemp();
        }

        private void btnReadTemperatureMain_Click(object sender, EventArgs e)
        {
            ReadPeltierTemp();
        }

        /////////////////////////////////////////////////
        // Step Motor Stop
        /////////////////////////////////////////////////
        private void btnStepAxisXStop_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP0, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
        }

        private void btnStepAxisYStop_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP1, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
        }

        private void btnStepAxisZStop_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.STOP, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
        }

        private void btnStepAxisGripStop_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.STOP, MOTOR.GRIP, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
        }

        private void btnStepAxisHamStop_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.STOP, MOTOR.HAM, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
        }

        /////////////////////////////////////////////////
        // Step Motor Make Hold Sate
        /////////////////////////////////////////////////
        private void btnHoldStepAxisX_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP0, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.HOLD);
        }

        private void btnHoldStepAxisY_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP1, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.HOLD);
        }

        private void btnHoldStepAxisZ_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.HOLD);
        }

        private void btnHoldStepAxisGrip_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.GRIP, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.HOLD);
        }

        private void btnHoldStepAxisHam_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.HAM, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.HOLD);
        }

        /////////////////////////////////////////////////
        // Step Motor Make Free Sate
        /////////////////////////////////////////////////
        private void btnFreeStepAxisX_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP0, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.FREE);
        }

        private void btnFreeStepAxisY_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP1, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.FREE);
        }

        private void btnFreeStepAxisZ_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.STEP2, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.FREE);
        }

        private void btnFreeStepAxisGrip_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.GRIP, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.FREE);
        }

        private void btnFreeStepAxisHam_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.HOLD, MOTOR.HAM, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.FREE);
        }

        /////////////////////////////////////////////////
        // Step Motor Alarm Reset (Each Axis)
        /////////////////////////////////////////////////
        private void btnResetServo_Click(object sender, EventArgs e)
        {
            ResetMotor(MOTOR.SERVO);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStepAxisGrip_Click(object sender, EventArgs e)
        {
            //ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.StepGripAxis);
            //btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStepAxisHam_Click(object sender, EventArgs e)
        {
            //ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.StepHamAxis);
            //btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStepAxisDoor_Click(object sender, EventArgs e)
        {
            //ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.StepDoorAx);
            //btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep0AxisX_Click(object sender, EventArgs e)
        {
            ResetMotor(MOTOR.STEP0);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep1AxisY_Click(object sender, EventArgs e)
        {
            ResetMotor(MOTOR.STEP1);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep2AxisZ_Click(object sender, EventArgs e)
        {
            ResetMotor(MOTOR.STEP2);
            btnGetSerialStatus_Click(null, null);
        }

        /////////////////////////////////////////////////
        // Step Motor Alarm Reset (All Axis)
        /////////////////////////////////////////////////
        private void btnResetAllAxis_Click(object sender, EventArgs e)
        {
            ResetMotor(MOTOR.SERVO);
            ResetMotor(MOTOR.STEP0);
            ResetMotor(MOTOR.STEP1);
            ResetMotor(MOTOR.STEP2);
            btnGetSerialStatus_Click(null, null);
        }

        /////////////////////////////////////////////////
        // Pinch On/Off
        /////////////////////////////////////////////////
        private void btnPinchValveClose_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(VALVE.CLOSE);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnPinchValveOpen_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(VALVE.OPEN);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Step Motor Jog +/- Move (Each Axis)
        /////////////////////////////////////////////////
        private void btnStepIncAxisX_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(editStepAxisX_Jog.Text);
            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, int.Parse(editStepAxisX_Speed.Text), pos,
                             int.Parse(editStepAxisX_Acc.Text), int.Parse(editStepAxisX_Dec.Text),
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step0AxisXPos.InvokeRequired == true)
                {
                    this.label_Step0AxisXPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step0AxisXPos.Text = (CurrentPos.Step0AxisX + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step0AxisXPos.Text = (CurrentPos.Step0AxisX + pos).ToString("F1");
                }
            }
        }
        private void btnStepIncAxisY_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(editStepAxisY_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, int.Parse(editStepAxisY_Speed.Text), pos,
                             int.Parse(editStepAxisY_Acc.Text), int.Parse(editStepAxisY_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step1AxisYPos.InvokeRequired == true)
                {
                    this.label_Step1AxisYPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step1AxisYPos.Text = (CurrentPos.Step1AxisY + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step1AxisYPos.Text = (CurrentPos.Step1AxisY + pos).ToString("F1");
                }
            }
        }

        private void btnStepIncAxisZ_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(editStepAxisZ_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, int.Parse(editStepAxisZ_Speed.Text), pos,
                             int.Parse(editStepAxisZ_Acc.Text), int.Parse(editStepAxisZ_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step2AxisZPos.InvokeRequired == true)
                {
                    this.label_Step2AxisZPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step2AxisZPos.Text = (CurrentPos.Step2AxisZ + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step2AxisZPos.Text = (CurrentPos.Step2AxisZ + pos).ToString("F1");
                }
            }
        }

        private void btnStepIncGripperAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(editStepGripper_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, int.Parse(editStepGripper_Speed.Text), pos,
                             int.Parse(editStepAxisGripper_Acc.Text), int.Parse(editStepAxisGripper_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_StepGripAxisPos.InvokeRequired == true)
                {
                    this.label_StepGripAxisPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_StepGripAxisPos.Text = (CurrentPos.StepGripAxis + pos).ToString("F1");
                    });
                }
                else
                {
                    label_StepGripAxisPos.Text = (CurrentPos.StepGripAxis + pos).ToString("F1");
                }
            }
        }

        private void btnStepIncPipettAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(editStepPipett_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, int.Parse(editStepPipett_Speed.Text), pos,
                             int.Parse(editStepAxisHam_Acc.Text), int.Parse(editStepAxisHam_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_StepHamAxisPos.InvokeRequired == true)
                {
                    this.label_StepHamAxisPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_StepHamAxisPos.Text = (CurrentPos.StepHamAxis + pos).ToString("F1");
                    });
                }
                else
                {
                    label_StepHamAxisPos.Text = (CurrentPos.StepHamAxis + pos).ToString("F1");
                }
            }
        }


        private void btnStepDecAxisX_Click(object sender, EventArgs e)
        {
            double pos = -double.Parse(editStepAxisX_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, int.Parse(editStepAxisX_Speed.Text), pos,
                             int.Parse(editStepAxisX_Acc.Text), int.Parse(editStepAxisX_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step0AxisXPos.InvokeRequired == true)
                {
                    this.label_Step0AxisXPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step0AxisXPos.Text = (CurrentPos.Step0AxisX + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step0AxisXPos.Text = (CurrentPos.Step0AxisX + pos).ToString("F1");
                }
            }
        }

        private void btnStepDecAxisY_Click(object sender, EventArgs e)
        {
            double pos = -double.Parse(editStepAxisY_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, int.Parse(editStepAxisY_Speed.Text), pos,
                             int.Parse(editStepAxisY_Acc.Text), int.Parse(editStepAxisY_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step1AxisYPos.InvokeRequired == true)
                {
                    this.label_Step1AxisYPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step1AxisYPos.Text = (CurrentPos.Step1AxisY + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step1AxisYPos.Text = (CurrentPos.Step1AxisY + pos).ToString("F1");
                }
            }
        }

        private void btnStepDecAxisZ_Click(object sender, EventArgs e)
        {
            double pos = -double.Parse(editStepAxisZ_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, int.Parse(editStepAxisZ_Speed.Text), pos,
                             int.Parse(editStepAxisZ_Acc.Text), int.Parse(editStepAxisZ_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_Step2AxisZPos.InvokeRequired == true)
                {
                    this.label_Step2AxisZPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_Step2AxisZPos.Text = (CurrentPos.Step2AxisZ + pos).ToString("F1");
                    });
                }
                else
                {
                    label_Step2AxisZPos.Text = (CurrentPos.Step2AxisZ + pos).ToString("F1");
                }
            }
        }

        private void btnStepDecGripperAxis_Click(object sender, EventArgs e)
        {
            double pos = -double.Parse(editStepGripper_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, int.Parse(editStepGripper_Speed.Text), pos,
                             int.Parse(editStepAxisGripper_Acc.Text), int.Parse(editStepAxisGripper_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_StepGripAxisPos.InvokeRequired == true)
                {
                    this.label_StepGripAxisPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_StepGripAxisPos.Text = (CurrentPos.StepGripAxis + pos).ToString("F1");
                    });
                }
                else
                {
                    label_StepGripAxisPos.Text = (CurrentPos.StepGripAxis + pos).ToString("F1");
                }
            }
        }

        private void btnStepDecPipettAxis_Click(object sender, EventArgs e)
        {
            double pos = -double.Parse(editStepPipett_Jog.Text);
            if(MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, int.Parse(editStepPipett_Speed.Text), pos,
                             int.Parse(editStepAxisHam_Acc.Text), int.Parse(editStepAxisHam_Dec.Text), 
                             POS_OPT.REL, HOLD_STATE.NONE) == COM_Status.ACK)
            {
                if (label_StepHamAxisPos.InvokeRequired == true)
                {
                    this.label_StepHamAxisPos.Invoke((MethodInvoker)delegate ()
                    {
                        label_StepHamAxisPos.Text = (CurrentPos.StepHamAxis + pos).ToString("F1");
                    });
                }
                else
                {
                    label_StepHamAxisPos.Text = (CurrentPos.StepHamAxis + pos).ToString("F1");
                }
            }
        }


        /////////////////////////////////////////////////
        // Eccentric Proximity Sensor Count Control
        // 근접센서의 카운트값을 설정하고 리셋하는 기능
        /////////////////////////////////////////////////
        private void btnWriteEccentric_Click(object sender, EventArgs e)
        {
            nEccentricThreshold = int.Parse(editEccentricThreshold.Text);
        }

        private void btnReadEccentric_Click(object sender, EventArgs e)
        {
            ReadEccentric();
        }

        private void btnResetEccentric_Click(object sender, EventArgs e)
        {
            EccentricClear();
            ReadEccentric();
        }

        /////////////////////////////////////////////////
        // Load Cell Value Read
        /////////////////////////////////////////////////
        private void btnReadLoadCellWeight_Click(object sender, EventArgs e)
        {
            try
            {
                ReadLoadCell(LOADCELL_CMD.WEIGHT, 0, timeout:150);

                if (float.Parse(label_LoadCellWeightVal.Text) > float.Parse(editLoadcellErrWeight.Text))
                {
                    checkedListBox_DGM_Weight.SetItemChecked(0, true);  // recipe checkbox
                    checkedListBox_DGM_Weight.SetItemChecked(1, false);
                    this.pictureBox_LoadcellResult.Image = global::CytoDx.Properties.Resources.Accept;
                }
                else
                {
                    checkedListBox_DGM_Weight.SetItemChecked(0, false);  // recipe checkbox
                    checkedListBox_DGM_Weight.SetItemChecked(1, true);
                    this.pictureBox_LoadcellResult.Image = global::CytoDx.Properties.Resources.Reject;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnSetLoadCellZeroPoint_Click(object sender, EventArgs e)
        {
            //ReadLoadCell(LOADCELL_CMD.SET_TARE, 0, timeout: 320);
            ReadLoadCell(LOADCELL_CMD.SET_TARE, 0, timeout: 100);
            ReadLoadCell(LOADCELL_CMD.RD_CAL, 0, timeout: 80);
            ReadLoadCell(LOADCELL_CMD.WR_CAL, 0, LoadcellVal.fCalVal, timeout: 150);
            
        }

        private void btnReadLoadCellSetValue_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnSetLoadCellGain_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnSetLoadCellScale_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Cooling Fan On/Off
        /////////////////////////////////////////////////
        private void btnCoolingFanOn_Click(object sender, EventArgs e)
        {
            CoolingFanControl(Status.ON);
        }

        private void btnCoolingFanOff_Click(object sender, EventArgs e)
        {
            CoolingFanControl(Status.OFF);
        }

        /////////////////////////////////////////////////
        // Accelometer Read/Stop
        /////////////////////////////////////////////////
        private int GetAccScaleOption()
        {
            int retVal = -1;

            if (strAccScale == "±2g")
            {
                retVal = 2;
            }
            else if (strAccScale == "±4g")
            {
                retVal = 4;
            }
            else if (strAccScale == "±8g")
            {
                retVal = 8;
            }

            return retVal;
        }

        private int GetAccGatherValue()
        {
            int retVal = -1;

            if (strGatherValue == "All")
            {
                retVal = 0;
            }
            else if (strGatherValue == "X")
            {
                retVal = 1;
            }
            else if (strGatherValue == "Y")
            {
                retVal = 2;
            }
            else if (strGatherValue == "Z")
            {
                retVal = 3;
            }
            else if (strGatherValue == "Sum")
            {
                retVal = 4;
            }

            return retVal;
        }
        
        /////////////////////////////////////////////////
        // Laser Sensor State Read
        /////////////////////////////////////////////////
        private void btnReadLaserSensor_Click(object sender, EventArgs e)
        {
            if (Serial.IsOpen == false)
                return;

            ReadLaserSensor(SENSOR_CMD.GET, Status.ON);
            //nLaserDetected = 1;

            if (nLaserDetected == 0)
            {
                label_LaserSensorResult.Text = "detected";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.Accept;
            }
            else if (nLaserDetected == 1)
            {
                label_LaserSensorResult.Text = "not detected";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.Reject;
            }
            else
            {
                label_LaserSensorResult.Text = "-";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.none2;
            }
        }

        private void btnLaserSensorOn_Click(object sender, EventArgs e)
        {
            ReadLaserSensor(SENSOR_CMD.PWR, Status.ON);
        }

        private void btnLaserSensorOff_Click(object sender, EventArgs e)
        {
            ReadLaserSensor(SENSOR_CMD.PWR, Status.OFF);
        }

        /////////////////////////////////////////////////
        // Flow Meter Read/Stop
        /////////////////////////////////////////////////
        public void FlowmeterReadStart()
        {
            if (Serial.IsOpen == false)
                return;

            ReadFlowMeter(SENSOR_CMD.PWR, Status.ON);

            flow_stopwatch.Start();
        }

        // flow meter 시작 시점에서 timer를 시작해서 종료 시점에서 timer를 종료하여 도출된 시간의 누적값을 기준으로
        // flowrate을 계산하기 때문에 유체의 흐름이 지속적인 구간에서 timer가 구동되도록 하여야 근사값이 산정될 수 있음
        // 유체가 흐르지 않는 시간 동안 stop을 하지 않으면 그만큼 계산의 오차가 발생하게 됨 (recipe로 테스트하기를 권장함)
        public void FlowmeterReadStop()
        {
            if (Serial.IsOpen == false)
                return;

            double pulse = 0.0;

            ReadFlowMeter(SENSOR_CMD.GET, Status.NONE);

            flow_stopwatch.Stop();

            bool result = double.TryParse(label_FlowPulseCntVal.Text, out pulse);

            if (result == true)
            {
                double volume = pulse * double.Parse(editFlowmeterCntUnit.Text);
                label_FlowMeterVolumeVal.Text = Math.Round(volume, 2).ToString();

                double flowrate = volume / flow_stopwatch.Elapsed.Seconds;
                label_FlowMeterFlowRateVal.Text = Math.Round(flowrate, 2).ToString();

                ReadFlowMeter(SENSOR_CMD.PWR, Status.OFF);
            }
            else
            {
                iPrintf("Pulse value not valid!");
                ReadFlowMeter(SENSOR_CMD.PWR, Status.OFF);
                return;
            }
        }

        private void btnFlowmeterReadStart_Click(object sender, EventArgs e)
        {
            FlowmeterReadStart();
        }

        private void btnFlowmeterReadStop_Click(object sender, EventArgs e)
        {
            FlowmeterReadStop();
        }

        private void btnFlowmeterReadReset_Click(object sender, EventArgs e)
        {
            ReadFlowMeter(SENSOR_CMD.PWR, Status.ON);
            label_FlowPulseCntVal.Text = "0";
            label_FlowMeterVolumeVal.Text   = "0";
            label_FlowMeterFlowRateVal.Text = "0";
            ReadFlowMeter(SENSOR_CMD.PWR, Status.OFF);
        }

        /////////////////////////////////////////////////
        // Strobe On/Off (Camera, 현재 시스템에서 적용되지 않음)
        /////////////////////////////////////////////////
        private void btnStrobeOn_Click(object sender, EventArgs e)
        {
            StrobeTrigger(int.Parse(edit_strobe_period.Text));
        }

        private void btnStrobeOff_Click(object sender, EventArgs e)
        {
            StrobeTrigger(0);
        }

        /////////////////////////////////////////////////
        // DataGridView에서 각행을 더블클릭하면 해당 열을 실행하도록 구현
        // WorldPos Table 제외
        /////////////////////////////////////////////////
        private void DV_AxisX_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!isRunning && !isRunningSingle && !isRunningManual)
                {
                    if (Serial.IsOpen == false)
                        return;
                    if (GetAxisXPosFromListView())
                    {
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, int.Parse(DV_AxisX.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisX.Rows[e.RowIndex].Cells[2].Value.ToString()),
                            int.Parse(DV_AxisX.Rows[e.RowIndex].Cells[3].Value.ToString()), 
                            int.Parse(DV_AxisX.Rows[e.RowIndex].Cells[4].Value.ToString()),
                            POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                        {
                            label_Step0AxisXPos.Text = DV_AxisX.Rows[e.RowIndex].Cells[2].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        private void DV_AxisY_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!isRunning && !isRunningSingle && !isRunningManual)
                {
                    if (Serial.IsOpen == false)
                        return;
                    if (GetAxisYPosFromListView())
                    {
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, int.Parse(DV_AxisY.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisY.Rows[e.RowIndex].Cells[2].Value.ToString()),
                            int.Parse(DV_AxisY.Rows[e.RowIndex].Cells[3].Value.ToString()),
                            int.Parse(DV_AxisY.Rows[e.RowIndex].Cells[4].Value.ToString()),
                            POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                        {
                            label_Step1AxisYPos.Text = DV_AxisY.Rows[e.RowIndex].Cells[2].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        private void DV_AxisZ_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!isRunning && !isRunningSingle && !isRunningManual)
                {
                    if (Serial.IsOpen == false)
                        return;
                    if (GetAxisZPosFromListView())
                    {
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, int.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[2].Value.ToString()),
                            int.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[3].Value.ToString()),
                            int.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[4].Value.ToString()),
                            POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                        {
                            label_Step2AxisZPos.Text = DV_AxisZ.Rows[e.RowIndex].Cells[2].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        private void DV_AxisGripper_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!isRunning && !isRunningSingle && !isRunningManual)
                {
                    if (Serial.IsOpen == false)
                        return;
                    if (GetAxisGripperPosFromListView())
                    {
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, int.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[2].Value.ToString()),
                            int.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[3].Value.ToString()),
                            int.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[4].Value.ToString()),
                            POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                        {
                            label_StepGripAxisPos.Text = DV_AxisGripper.Rows[e.RowIndex].Cells[2].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        private void DV_AxisPipett_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!isRunning && !isRunningSingle && !isRunningManual)
                {
                    if (Serial.IsOpen == false)
                        return;
                    if (GetAxisPipettPosFromListView())
                    {
                        if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, int.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[2].Value.ToString()),
                            int.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[3].Value.ToString()),
                            int.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[4].Value.ToString()),
                            POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                        {
                            label_StepHamAxisPos.Text = DV_AxisPipett.Rows[e.RowIndex].Cells[2].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        /////////////////////////////////////////////////
        // Centrifuge의 위치를 90도 단위로 변경
        /////////////////////////////////////////////////
        private void btnMoveChamber1_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(CHAMBER_POS.CHAMBER1);
            WaitForServoStop();
        }

        private void btnMoveCelldown1_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(CHAMBER_POS.CELLDOWN1);
            WaitForServoStop();
        }

        private void btnMoveChamber2_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(CHAMBER_POS.CHAMBER2);
            WaitForServoStop();
        }

        private void btnMoveCelldown2_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(CHAMBER_POS.CELLDOWN2);
            WaitForServoStop();
        }

        private void btnServoOff_Click(object sender, EventArgs e)
        {
            ServoOnOff(HOLD_STATE.FREE);
            Thread.Sleep(200);
            ServoMonitor(MotorMon.STATUS);
        }

        private void btnServoOn_Click(object sender, EventArgs e)
        {
            ServoOnOff(HOLD_STATE.HOLD);
            Thread.Sleep(200);
            ServoMonitor(MotorMon.STATUS);
        }

        /////////////////////////////////////////////////
        // 실내 조명 On/Off
        /////////////////////////////////////////////////
        private void btnTopLightSet_Click(object sender, EventArgs e)
        {
            SetLightCond(Light.Room, int.Parse(editRoomLightBright.Text), timeout: 150);
        }

        private void btnRotorLightSet_Click(object sender, EventArgs e)
        {
            SetLightCond(Light.Chamber, int.Parse(editRotorLightBright.Text), timeout: 150);
        }

        private void btnTopLightOff_Click(object sender, EventArgs e)
        {
            RoomLight(Status.OFF);
        }

        private void btnTopLightOn_Click(object sender, EventArgs e)
        {
            RoomLight(Status.ON);
        }

        /////////////////////////////////////////////////
        // Centrifuge 내부 조명 On/Off
        /////////////////////////////////////////////////
        private void btnRotorLightOff_Click(object sender, EventArgs e)
        {
            ChamberLight(Status.OFF);
        }

        private void btnRotorLightOn_Click(object sender, EventArgs e)
        {
            ChamberLight(Status.ON);
        }

        /////////////////////////////////////////////////
        // 시스템 파워 Off
        /////////////////////////////////////////////////
        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            RunPer2_HamiltonPipett("AV", 0, 0, 0, 0, TIP_TYPE.NONE);
            SystemOff();
        }

        /////////////////////////////////////////////////
        // 펌웨어 파라미터 공장 초기화
        /////////////////////////////////////////////////
        private void btnSystemFactoryReset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to RESET control board parameter??",
                                                    "Parameter Reseted!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                //BuildCmdPacket(bCommandSendBuffer, "SYSTEM", "INIT", "");
                //SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port

                SystemCmd("SYSTEM", "INIT", "");
            }
            else
            {
                return;
            }
        }

        /////////////////////////////////////////////////
        // 로터 도어 개폐
        /////////////////////////////////////////////////
        private void btnCoverOpen_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, int.Parse(editCoverOpenSpeed.Text), double.Parse(editCoverOpenPos.Text), 
                int.Parse(editStepCoverAcc.Text), int.Parse(editStepCoverDec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }

        private void btnCoverClose_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, int.Parse(editCoverCloseSpeed.Text), double.Parse(editCoverClosePos.Text),
               int.Parse(editStepCoverAcc.Text), int.Parse(editStepCoverDec.Text), POS_OPT.ABS, HOLD_STATE.NONE);
        }

        private void btnCoverSenRead_Click(object sender, EventArgs e)
        {
            //GetStatus(true);
            MoveStepMotor(STEP_CMD.STATUS, MOTOR.COVER, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            Thread.Sleep(50);
            MoveStepMotor(STEP_CMD.POS, MOTOR.COVER, 0, 0, 0, 0, POS_OPT.NONE, HOLD_STATE.NONE);
            Thread.Sleep(50);

            if (CoverAxState.bLIMIT_HI == true)
            {
                this.pictureBox_CoverLimitHigh.Image = global::CytoDx.Properties.Resources.Accept;
            }
            else
            {
                this.pictureBox_CoverLimitHigh.Image = global::CytoDx.Properties.Resources.none2;
            }

            if (CoverAxState.bLIMIT_LOW == true)
            {
                this.pictureBox_CoverLimitLow.Image = global::CytoDx.Properties.Resources.Accept;
            }
            else
            {
                this.pictureBox_CoverLimitLow.Image = global::CytoDx.Properties.Resources.none2;
            }
        }

        private void btnHomeStepAxisDoor_Click(object sender, EventArgs e)
        {
            iPrintf("Initializing Step Motor ... Axis Cover to Home");
            if (StepMotorHomeMove(MOTOR.COVER, "HOME", "") != COM_Status.ACK)
            {
                iPrintf("Initializing Step Motor ... Axis Cover to Home ... Fail");
                return;
            }
            else
            {
                iPrintf("Z Axis Homing Done!");
            }
        }

        private void btnStepIncDoor_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, int.Parse(editCoverCloseSpeed.Text), 1,
                int.Parse(editStepCoverAcc.Text), int.Parse(editStepCoverDec.Text), POS_OPT.REL, HOLD_STATE.NONE);
        }

        private void btnStepDecDoor_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.COVER, int.Parse(editCoverCloseSpeed.Text), -1,
                int.Parse(editStepCoverAcc.Text), int.Parse(editStepCoverDec.Text), POS_OPT.REL, HOLD_STATE.NONE);
        }

        private void Insert_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show(sender.ToString());
            //DV_CTC_Vertical.Rows.Insert(DV_CTC_Vertical);
            //DV_Recipe.Rows.Insert(DV_Recipe.SelectedRows[0].Index, false, "", "", "", "", "", "", "", "");

        }

        public double Z_CopyPos = 0.0;
        public double Grip_CopyPos = 0.0;
        public double Ham_CopyPos = 0.0;

        /////////////////////////////////////////////////
        // 각 DataGridView의 우클릭 메뉴 기능 설정 (insert/remove)
        /////////////////////////////////////////////////
        private void MenuMotor_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int idx;

            try
            {
                if (e.ClickedItem.Name == "Insert")
                {
                    strTpnt = GetTpntCategory(EDIT.INSERT);

                    if (MenuMotor.SourceControl.Name == "DV_AxisX" && DV_AxisX.SelectedRows.Count > 0)
                    {
                        idx = DV_AxisX.SelectedRows[0].Index;
                        DV_AxisX.Rows.Insert(idx, "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_AxisX" && DV_AxisX.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_AxisY" && DV_AxisY.SelectedRows.Count > 0)
                    {
                        idx = DV_AxisY.SelectedRows[0].Index;
                        DV_AxisY.Rows.Insert(idx, "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_AxisY" && DV_AxisY.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_AxisZ" && DV_AxisZ.SelectedRows.Count > 0)
                    {
                        idx = DV_AxisZ.SelectedRows[0].Index;
                        DV_AxisZ.Rows.Insert(idx, "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_AxisZ" && DV_AxisZ.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_AxisGripper" && DV_AxisGripper.SelectedRows.Count > 0)
                    {
                        idx = DV_AxisGripper.SelectedRows[0].Index;
                        DV_AxisGripper.Rows.Insert(idx, "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_AxisGripper" && DV_AxisGripper.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_AxisPipett" && DV_AxisPipett.SelectedRows.Count > 0)
                    {
                        idx = DV_AxisPipett.SelectedRows[0].Index;
                        DV_AxisPipett.Rows.Insert(idx, "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_AxisPipett" && DV_AxisPipett.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0 &&
                        (strTpnt_Sort != "None" && strTpnt_Sort != null))
                    {
                        idx = DV_World_T_Pnt.SelectedRows[0].Index;
                        DV_World_T_Pnt.Rows.Insert(idx, strTpnt, "", "", "", "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" &&
                        (strTpnt_Sort == "None" || strTpnt_Sort == null))
                    {
                        MessageBox.Show("Teaching Point Type Not Selected!", "Input Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Invalid Value! Teaching Point Type Not Selected");
                        return;
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }

                    if (MenuMotor.SourceControl.Name == "DV_Offset" && DV_Offset.SelectedRows.Count > 0)
                    {
                        idx = DV_Offset.SelectedRows[0].Index;
                        DV_Offset.Rows.Insert(idx, strTpnt, "", "", "", "", "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_Offset" && DV_Offset.SelectedRows.Count <= 0)
                    {
                        MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iPrintf("Selected Rows Count is zero!!, Input 1st Line Data!");
                    }
                }
                else if (e.ClickedItem.Name == "Remove")
                {
                    var confirmResult = MessageBox.Show("Are you sure to remove selected row data??",
                                                    "Data Removed!!", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        if (MenuMotor.SourceControl.Name == "DV_AxisX" && DV_AxisX.SelectedRows.Count > 0)
                            DV_AxisX.Rows.RemoveAt(DV_AxisX.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_AxisY" && DV_AxisY.SelectedRows.Count > 0)
                            DV_AxisY.Rows.RemoveAt(DV_AxisY.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_AxisZ" && DV_AxisZ.SelectedRows.Count > 0)
                            DV_AxisZ.Rows.RemoveAt(DV_AxisZ.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_AxisGripper" && DV_AxisGripper.SelectedRows.Count > 0)
                            DV_AxisGripper.Rows.RemoveAt(DV_AxisGripper.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_AxisPipett" && DV_AxisPipett.SelectedRows.Count > 0)
                            DV_AxisPipett.Rows.RemoveAt(DV_AxisPipett.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0)
                            DV_World_T_Pnt.Rows.RemoveAt(DV_World_T_Pnt.SelectedRows[0].Index);
                        if (MenuMotor.SourceControl.Name == "DV_Offset" && DV_Offset.SelectedRows.Count > 0)
                            DV_Offset.Rows.RemoveAt(DV_Offset.SelectedRows[0].Index);
                    }
                    else
                    {
                        return;
                    }
                }
                else if (e.ClickedItem.Name == "Move")
                {
                    var confirmResult = MessageBox.Show("Are you sure to move to Teaching Point??",
                                                    "", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        if ((MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0))
                        {
                            if (MoveTpnt("4000",        // 4000: speed value, 해밀턴, 그리퍼 축은 기구적인 문제로 4000 이상에서 탈조됨
                                DV_World_T_Pnt.SelectedRows[0].Cells[2].Value.ToString(),
                                DV_World_T_Pnt.SelectedRows[0].Cells[3].Value.ToString(),
                                DV_World_T_Pnt.SelectedRows[0].Cells[4].Value.ToString(),
                                DV_World_T_Pnt.SelectedRows[0].Cells[5].Value.ToString(),
                                DV_World_T_Pnt.SelectedRows[0].Cells[6].Value.ToString()) == false)
                            {
                                return;
                            }
                            else
                            {
                                WaitForStepMotionDone();
                            }
                        }
                        else if (MenuMotor.SourceControl.Name == "DV_AxisX" && DV_AxisX.SelectedRows.Count > 0)
                        {
                            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0,
                                             int.Parse(DV_AxisX.SelectedRows[0].Cells[1].Value.ToString()),
                                             double.Parse(DV_AxisX.SelectedRows[0].Cells[2].Value.ToString()),
                                             int.Parse(DV_AxisX.SelectedRows[0].Cells[3].Value.ToString()),
                                             int.Parse(DV_AxisX.SelectedRows[0].Cells[4].Value.ToString()),
                                             POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                            {
                                label_Step0AxisXPos.Text = DV_AxisX.SelectedRows[0].Cells[2].Value.ToString();
                            }
                        }
                        else if (MenuMotor.SourceControl.Name == "DV_AxisY" && DV_AxisY.SelectedRows.Count > 0)
                        {
                            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1,
                                              int.Parse(DV_AxisY.SelectedRows[0].Cells[1].Value.ToString()),
                                              double.Parse(DV_AxisY.SelectedRows[0].Cells[2].Value.ToString()),
                                              int.Parse(DV_AxisY.SelectedRows[0].Cells[3].Value.ToString()),
                                              int.Parse(DV_AxisY.SelectedRows[0].Cells[4].Value.ToString()),
                                              POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                            {
                                label_Step1AxisYPos.Text = DV_AxisY.SelectedRows[0].Cells[2].Value.ToString();
                            }
                        }
                        else if (MenuMotor.SourceControl.Name == "DV_AxisZ" && DV_AxisZ.SelectedRows.Count > 0)
                        {
                            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2,
                                              int.Parse(DV_AxisZ.SelectedRows[0].Cells[1].Value.ToString()),
                                              double.Parse(DV_AxisZ.SelectedRows[0].Cells[2].Value.ToString()),
                                              int.Parse(DV_AxisZ.SelectedRows[0].Cells[3].Value.ToString()),
                                              int.Parse(DV_AxisZ.SelectedRows[0].Cells[4].Value.ToString()),
                                              POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                            {
                                label_Step2AxisZPos.Text = DV_AxisZ.SelectedRows[0].Cells[2].Value.ToString();
                            }
                        }
                        else if (MenuMotor.SourceControl.Name == "DV_AxisGripper" && DV_AxisGripper.SelectedRows.Count > 0)
                        {
                            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP,
                                              int.Parse(DV_AxisGripper.SelectedRows[0].Cells[1].Value.ToString()),
                                              double.Parse(DV_AxisGripper.SelectedRows[0].Cells[2].Value.ToString()),
                                              int.Parse(DV_AxisGripper.SelectedRows[0].Cells[3].Value.ToString()),
                                              int.Parse(DV_AxisGripper.SelectedRows[0].Cells[4].Value.ToString()),
                                              POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                            {
                                label_StepGripAxisPos.Text = DV_AxisGripper.SelectedRows[0].Cells[2].Value.ToString();
                            }
                        }
                        else if (MenuMotor.SourceControl.Name == "DV_AxisPipett" && DV_AxisPipett.SelectedRows.Count > 0)
                        {
                            if (MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM,
                                              int.Parse(DV_AxisPipett.SelectedRows[0].Cells[1].Value.ToString()),
                                              double.Parse(DV_AxisPipett.SelectedRows[0].Cells[2].Value.ToString()),
                                              int.Parse(DV_AxisPipett.SelectedRows[0].Cells[3].Value.ToString()),
                                              int.Parse(DV_AxisPipett.SelectedRows[0].Cells[4].Value.ToString()),
                                              POS_OPT.ABS, HOLD_STATE.NONE) == COM_Status.ACK)
                            {
                                label_StepHamAxisPos.Text = DV_AxisPipett.SelectedRows[0].Cells[2].Value.ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Only T point Moving Allowed! Try Again!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else if(e.ClickedItem.Name == "Copy")
                {
                    if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0)
                    {
                        Z_CopyPos = double.Parse(DV_World_T_Pnt.SelectedRows[0].Cells[4].Value.ToString());
                        Grip_CopyPos = double.Parse(DV_World_T_Pnt.SelectedRows[0].Cells[5].Value.ToString());
                        Ham_CopyPos = double.Parse(DV_World_T_Pnt.SelectedRows[0].Cells[6].Value.ToString());
                        iPrintf(String.Format("Copied Pos Z: {0}, Grip: {1}, Ham: {2}", Z_CopyPos, Grip_CopyPos, Ham_CopyPos));
                    }
                }
                else if (e.ClickedItem.Name == "Paste")
                {
                    var confirmResult = MessageBox.Show("Are you sure to paste Z, Grip, Ham Data??",
                                                    "", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0)
                        {
                            DV_World_T_Pnt.SelectedRows[0].Cells[4].Value = Z_CopyPos.ToString();
                            DV_World_T_Pnt.SelectedRows[0].Cells[5].Value = Grip_CopyPos.ToString();
                            DV_World_T_Pnt.SelectedRows[0].Cells[6].Value = Ham_CopyPos.ToString();
                            iPrintf(String.Format("Pasted Pos Z: {0}, Grip: {1}, Ham: {2}", Z_CopyPos, Grip_CopyPos, Ham_CopyPos));
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnParamSave_Click(object sender, EventArgs e)
        {
            btnConfigSave_Click(sender, e);
        }

        private void btnButtonReload_Click(object sender, EventArgs e)
        {
            DV_Recipe.Rows.Clear();
            ReadWriteButtonConfig(RW.READ, config.LastButtonFileName);

            SelectButton(0);
        }

        private void btnButtonFileSave_Click(object sender, EventArgs e)
        {
            ReadWriteButtonConfig(RW.WRITE, config.LastButtonFileName);
            config.ReadWriteLastButtonfile(RW.WRITE);
        }

        private void btnButtonSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Button files (*.btn)|*.btn|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                dlg.InitialDirectory = DIR_RECIPE;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    DV_Recipe.Rows.Clear();
                    ReadWriteButtonConfig(RW.WRITE, dlg.FileName);
                    config.ReadWriteLastButtonfile(RW.WRITE);
                    SelectButton(0);
                }

                iPrintf("button file save done!!");
            }
        }

        private void btnButtonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Button files (*.btn)|*.btn|All files (*.*)|*.*";
                    dlg.FilterIndex = 1;
                    dlg.RestoreDirectory = true;

                    dlg.InitialDirectory = DIR_RECIPE;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        DV_Recipe.Rows.Clear();
                        ReadWriteButtonConfig(RW.READ, dlg.FileName);
                        config.ReadWriteLastButtonfile(RW.WRITE);
                        SelectButton(0);
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnEstop_HamPipett_Click(object sender, EventArgs e)
        {
            if (this.btnHamEstop_toggel.BackColor == Color.SteelBlue)
            {
                this.btnHamEstop_toggel.BackColor = Color.LightPink;
                RunPer2_HamiltonPipett("ES", 0, 0, 0, 0, TIP_TYPE.NONE); // Estop ON
                bHamilton_EStopFlag = true;

            }
            else if (this.btnHamEstop_toggel.BackColor == Color.LightPink)
            {
                this.btnHamEstop_toggel.BackColor = Color.SteelBlue;
                RunPer2_HamiltonPipett("SR", 0, 0, 0, 0, TIP_TYPE.NONE); // Estop OFF
                bHamilton_EStopFlag = false;
            }
        }

        private void btnZmove_HamPipett_Click(object sender, EventArgs e)
        {
            if (this.btnHamZmove_toggle.BackColor == Color.SteelBlue)
            {
                this.btnHamZmove_toggle.BackColor = Color.LightPink;
                bHamilton_Z_Follow_Flag = true;
            }
            else if (this.btnHamZmove_toggle.BackColor == Color.LightPink)
            {
                this.btnHamZmove_toggle.BackColor = Color.SteelBlue;
                bHamilton_Z_Follow_Flag = false;
            }
        }

        private void btnADC_HamPipett_Click(object sender, EventArgs e)
        {
            if (this.btnHamADC_toggle.BackColor == Color.SteelBlue)
            {
                this.btnHamADC_toggle.BackColor = Color.LightPink;
                RunPer2_HamiltonPipett("AX", 0, 0, 0, 0, TIP_TYPE.NONE); //ADC ON (토출 명령 후 자동으로 꺼짐)
                bHamilton_ADCFlag = true;
            }
            else if (this.btnHamADC_toggle.BackColor == Color.LightPink)
            {
                this.btnHamADC_toggle.BackColor = Color.SteelBlue;
                bHamilton_ADCFlag = false;
            }
        }

        private void btnEstop_Axis_Click(object sender, EventArgs e)
        {
            if (this.btnAxisEstop_toggel.BackColor == Color.SteelBlue)
            {
                this.btnAxisEstop_toggel.BackColor = Color.LightPink;
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

                bStopFlag = true;

                if (bSerialTimerState == true)
                    btnTimer_Click(this, null);
            }
            else if (this.btnAxisEstop_toggel.BackColor == Color.LightPink)
            {
                this.btnAxisEstop_toggel.BackColor = Color.SteelBlue;
                bStopFlag = false;
            }
        }

        private void comboBox_TipType_IdxChanged(object sender, EventArgs e)
        {
            label_TipType.Text = comboBox_TipType.SelectedItem.ToString();
        }

        private void comboBox_SensitivitycLLD_IdxChanged(object sender, EventArgs e)
        {
            strcLLD_Sensitivity = comboBox_SensitivitycLLD.SelectedItem.ToString();
        }

        private void comboBox_DirAxiscLLD_IdxChanged(object sender, EventArgs e)
        {
            strcLLD_Dir_Axis = comboBox_DirAxiscLLD.SelectedItem.ToString();
        }

        // 각 축의 현재 위치를 교시점 table에 저장함
        private void btnPositionSave_Click(object sender, EventArgs e)
        {
            if (DV_World_T_Pnt.SelectedRows.Count > 0)
            {
                strTpnt = GetTpntCategory(EDIT.SAVE);

                if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 2) == strTpnt.Substring(0, 2) &&
                    DV_World_T_Pnt.SelectedRows[0].Cells[2].Value.ToString() == CurrentPos.Step0AxisX.ToString("F2") &&
                    DV_World_T_Pnt.SelectedRows[0].Cells[3].Value.ToString() == CurrentPos.Step1AxisY.ToString("F2") &&
                    DV_World_T_Pnt.SelectedRows[0].Cells[4].Value.ToString() == CurrentPos.Step2AxisZ.ToString("F2") &&
                    DV_World_T_Pnt.SelectedRows[0].Cells[5].Value.ToString() == CurrentPos.StepGripAxis.ToString("F2") &&
                    DV_World_T_Pnt.SelectedRows[0].Cells[6].Value.ToString() == CurrentPos.StepHamAxis.ToString("F2"))
                {
                    MessageBox.Show("Selected Tpnt Sort and Currnet Position is Same!! Retry!", "Input Data Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //strTpnt = GetTpntCategory();

                var confirmResult = MessageBox.Show("Are you sure to update current data??",
                                                    "Data Replaced!!", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    DV_World_T_Pnt.SelectedRows[0].Cells[0].Value = strTpnt;
                    DV_World_T_Pnt.SelectedRows[0].Cells[2].Value = Math.Round(CurrentPos.Step0AxisX, 2);
                    DV_World_T_Pnt.SelectedRows[0].Cells[3].Value = Math.Round(CurrentPos.Step1AxisY, 2);
                    DV_World_T_Pnt.SelectedRows[0].Cells[4].Value = Math.Round(CurrentPos.Step2AxisZ, 2);
                    DV_World_T_Pnt.SelectedRows[0].Cells[5].Value = Math.Round(CurrentPos.StepGripAxis, 2);
                    DV_World_T_Pnt.SelectedRows[0].Cells[6].Value = Math.Round(CurrentPos.StepHamAxis, 2);

                    if (DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString() == "CP1" ||
                       DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString() == "IP1" ||
                       DV_World_T_Pnt.SelectedRows[0].Cells[0].Value.ToString() == "RP1")
                    {
                        DV_World_T_Pnt.SelectedRows[0].Cells[1].Value = strTpnt_Name + "_Origin";
                    }
                    else
                    {
                        DV_World_T_Pnt.SelectedRows[0].Cells[1].Value = strTpnt_Name;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Selected Row Count Zero!!", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // 각 축의 위치 정보를 Monitoring Table에 저장함. 피펫, 그리퍼의 축단 위치 기준으로 표기
        private void btnRefreshWorldPosition_Click(object sender, EventArgs e)
        {
            ReadMotorPosition(true);
            //Thread.Sleep(200);
        }

        private void comboBox_Tpnt_IdxChanged(object sender, EventArgs e)
        {
            strTpnt_Sort = comboBox_Tpnt.SelectedItem.ToString();
            string strIdx = null;

            if (strTpnt_Sort == "None")
            {
                strIdx = "";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    DV_World_T_Pnt.Rows[i].Visible = true;
                }
            }
            else if (strTpnt_Sort == "Normal")
            {
                strIdx = "TP";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;
                    if (Value.Length >= 2 && Value.Substring(0, 2) == strIdx)
                        DV_World_T_Pnt.Rows[i].Visible = true;
                    else
                        DV_World_T_Pnt.Rows[i].Visible = false;
                }
            }
            else if (strTpnt_Sort == "Cooler")
            {
                strIdx = "CP";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;
                    if (Value.Length >= 2 && Value.Substring(0, 2) == strIdx)
                        DV_World_T_Pnt.Rows[i].Visible = true;
                    else
                        DV_World_T_Pnt.Rows[i].Visible = false;
                }
            }
            else if (strTpnt_Sort == "Tip")
            {
                strIdx = "IP";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;
                    if (Value.Length >= 2 && Value.Substring(0, 2) == strIdx)
                        DV_World_T_Pnt.Rows[i].Visible = true;
                    else
                        DV_World_T_Pnt.Rows[i].Visible = false;
                }
            }
            else if (strTpnt_Sort == "Tube")
            {
                strIdx = "RP";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;
                    if (Value.Length >= 2 && Value.Substring(0, 2) == strIdx)
                        DV_World_T_Pnt.Rows[i].Visible = true;
                    else
                        DV_World_T_Pnt.Rows[i].Visible = false;
                }
            }
            else if (strTpnt_Sort == "Centrifuge")
            {
                strIdx = "FP";
                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;
                    if (Value.Length >= 2 && Value.Substring(0, 2) == strIdx)
                        DV_World_T_Pnt.Rows[i].Visible = true;
                    else
                        DV_World_T_Pnt.Rows[i].Visible = false;
                }
            }

            DV_World_T_Pnt.Refresh();
        }

        private void DV_Wrold_T_Pnt_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex == DV_World_T_Pnt.SelectedRows[0].Index)
            {
                btnTpntEdit_Click(sender, e);
            }
            else
            {
                if (m_current_running_row > -1)
                {
                    DV_World_T_Pnt.ClearSelection();
                    DV_World_T_Pnt.Rows[m_current_running_row].Cells[0].Selected = true;
                    DV_World_T_Pnt.Rows[m_current_running_row].Selected = true;
                }
            }
        }

        private void btnTpntEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int row = DV_World_T_Pnt.SelectedRows[0].Index;
                DV_World_T_Pnt.Rows[row].Cells[0].Selected = true;   // make sure selecting command combo box
                DV_World_T_Pnt.Rows[row].Selected = true;

                string strIdx = (string)DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Idx].Value;

                if (SetTpntDialog(strIdx, row) == false)
                    return;
                tpntinput.StartPosition = FormStartPosition.Manual;
                //tpntinput.Location = new System.Drawing.Point(this.Left + (this.Width / 2), this.Top + (this.Height / 2));
                tpntinput.Location = new System.Drawing.Point(this.Left + (this.Width / 4), this.Top + (this.Height / 3));
                if (tpntinput.ShowDialog() == DialogResult.OK)
                {
                    GetTpntDialog(row);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        public TpntParam GetTpntRowParam(int row)
        {
            TpntParam retParam = new TpntParam();

            retParam.param1 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Idx].Value.ToString();
            retParam.param2 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Name].Value.ToString();
            retParam.param3 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.X].Value.ToString();
            retParam.param4 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Y].Value.ToString();
            retParam.param5 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Z].Value.ToString();
            retParam.param6 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Gripper].Value.ToString();
            retParam.param7 = DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Pipett].Value.ToString();

            return retParam;
        }

        private bool SetTpntDialog(string strIdx, int row)
        {
            tpntinput.SetParam(GetTpntRowParam(row), this);

            return true;
        }

        private void GetTpntDialog(int row)
        {
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Idx].Value = tpntinput.editParam1.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Name].Value = tpntinput.editParam2.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.X].Value = tpntinput.editParam3.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Y].Value = tpntinput.editParam4.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Z].Value = tpntinput.editParam5.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Gripper].Value = tpntinput.editParam6.Text;
            DV_World_T_Pnt.Rows[row].Cells[(int)Tpnt_COL.Pipett].Value = tpntinput.editParam7.Text;
        }

        private void btnRecipeButtonAllSave_Click(object sender, EventArgs e)
        {
            btnRecipeSave_Click(sender, e);

            GetRecipeFromListView(GetSelectedButtonIndex());

            if (defineButtonForm.lblRecipeFilename.Text == null || defineButtonForm.lblRecipeFilename.Text == "")
            {
                ListButtonRecipe[defineButtonForm.buttonIndex].button.AccessibleName =
                                            MainWindow.DIR_RECIPE + "\\" + defineButtonForm.editButtonName.Text + ".rcp";
                                            //"C:\\TruNser_C2000\\Recipe\\" + defineButtonForm.editButtonName.Text + ".rcp";
                if (!File.Exists(ListButtonRecipe[defineButtonForm.buttonIndex].button.AccessibleName))
                {
                    defineButtonForm.lblRecipeFilename.Text = defineButtonForm.editButtonName.Text + ".rcp";
                }
                else if (File.Exists(ListButtonRecipe[defineButtonForm.buttonIndex].button.AccessibleName))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("\"{0}.rcp\" Already Same File Name Exist! Rename & Try Again!",
                                                                                    defineButtonForm.editButtonName.Text), "File Exist Error",
                                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ListButtonRecipe[defineButtonForm.buttonIndex].button.AccessibleName = "";
                }
            }

            config.ReadWriteRecipe(RW.WRITE,
                                   ListButtonRecipe[defineButtonForm.buttonIndex].button.AccessibleName,
                                   ref ListButtonRecipe[defineButtonForm.buttonIndex].recipe);

            btnButtonFileSave_Click(sender, e);

            iPrintf("All recipe & button save done!!");
        }
    }
}
