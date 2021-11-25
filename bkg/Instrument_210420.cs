using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using System.Windows.Forms;
using System.Threading;

namespace CytoDx
{
    public partial class MainWindow
    {
        /////////////////////////////////////////////////
        // Step Motor Home Position Move (Each Axis)
        /////////////////////////////////////////////////
        private void btnHomeStepAxisX_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(editStepAxisX_Speed.Text), 0.0, 0, 0, 0);
        }

        private void btnHomeStepAxisY_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(editStepAxisY_Speed.Text), 0.0, 0, 0, 0);
        }

        private void btnHomeStepAxisZ_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(editStepAxisZ_Speed.Text), 0.0, 0, 0, 0);
        }

        private void btnHomeStepGripperAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(editStepGripper_Speed.Text), 0.0, 0, 0, 0);
        }

        private void btnHomeStepPipettAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(editStepPipett_Speed.Text), 0.0, 0, 0, 0);
        }

        /////////////////////////////////////////////////
        // Step Motor Target Position Move
        /////////////////////////////////////////////////
        private void btnMoveStepAxisX_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(editStepAxisX_Speed.Text), double.Parse(editStepAxisX_Pos.Text), 0, 0, 0);
        }
        private void btnMoveStepAxisY_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(editStepAxisY_Speed.Text), double.Parse(editStepAxisY_Pos.Text), 0, 0, 0);
        }
        private void btnMoveStepAxisZ_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(editStepAxisZ_Speed.Text), double.Parse(editStepAxisZ_Pos.Text), 0, 0, 0);
        }
        private void btnMoveStepGripperAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(editStepGripper_Speed.Text), double.Parse(editStepGripper_Pos.Text), 0, 0, 0);
        }
        private void btnMoveStepPipettAxis_Click(object sender, EventArgs e)
        {
            MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(editStepPipett_Speed.Text), double.Parse(editStepPipett_Pos.Text), 0, 0, 0);
        }

        /////////////////////////////////////////////////
        // Step Motor Home Position Move (All Axis)
        /////////////////////////////////////////////////
        private void btnInitializePos_Click(object sender, EventArgs e)
        {
            MoveHomeStepMotor();
        }

        public bool MoveHomeStepMotor()
        {
            iPrintf("Initializing Step Motor");

            if (CurrentPos.Step1Grip != double.Parse(editStepAxisGripper_HomePos.Text))
            {
                iPrintf("Initializing Step Motor ... Gripper Axis to Home");
                if (MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(editStepGripper_Speed.Text), 
                    double.Parse(editStepAxisGripper_HomePos.Text), 0, 0, 0) != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Gripper Axis to Home ... Fail");
                    return false;
                }
            }

            if (CurrentPos.Step2Pipett != double.Parse(editStepAxisPipett_HomePos.Text))
            {
                iPrintf("Initializing Step Motor ... Pipett Axis to Home");
                // wait for motion done(Pipett), opt2: 1
                if (MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(editStepPipett_Speed.Text), 
                    double.Parse(editStepAxisPipett_HomePos.Text), 0, 1, 0) != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Pipett Axis to Home ... Fail");
                    return false;
                }
            }

            if (CurrentPos.Step6AxisZ != double.Parse(editStepAxisZ_HomePos.Text))
            {
                iPrintf("Initializing Step Motor ... Axis Z to Home");
                // wait for motion done(Z), opt2: 1
                if (MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(editStepAxisZ_Speed.Text), 
                    double.Parse(editStepAxisZ_HomePos.Text), 0, 1, 0) != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis Z to Home ... Fail");
                    return false;
                }
            }

            // 충돌 방지를 위해 XY축 이동을 가장 마지막에 수행해야 함
            if (CurrentPos.Step4AxisX != double.Parse(editStepAxisX_HomePos.Text))
            {
                iPrintf("Initializing Step Motor ... Axis X to Home");
                if (MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(editStepAxisX_Speed.Text), 
                    double.Parse(editStepAxisX_HomePos.Text), 0, 0, 0) != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis X to Home ... Fail");
                    return false;
                }
            }

            if (CurrentPos.Step5AxisY != double.Parse(editStepAxisY_HomePos.Text))
            {
                iPrintf("Initializing Step Motor ... Axis Y to Home");
                if (MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(editStepAxisY_Speed.Text), 
                    double.Parse(editStepAxisY_HomePos.Text), 0, 0, 0) != COM_Status.ACK)
                {
                    iPrintf("Initializing Step Motor ... Axis Y to Home ... Fail");
                    return false;
                }
            }

            iPrintf("Finish Initializing");
            return true;
        }

        /////////////////////////////////////////////////
        // TriContinet Pump Control
        /////////////////////////////////////////////////
        
        private double Volume_mL = 0.0;
        private double Vol_mL_per_Inc = 0.0;

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
                Volume_mL = double.Parse(editTriPipettLoadingVolume.Text) + double.Parse(editTriPipettOffsetVolume.Text);
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
                double FlowRate_uL_per_sec = double.Parse(editTriPipettFlowRate.Text) * 1000;
                //FlowRate_IncPerSecResult = FlowRate_uL_per_sec * (double)(TriPipett_Vel_Resolution / TriPipett_Vol);
                FlowRate_IncPerSecResult = FlowRate_uL_per_sec / TriPipett_ul_per_inc;
            }

            return FlowRate_IncPerSecResult;
        }
        
        private void btnAspirateLiquidPump_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(1);   // Open

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

                //SerPinchValve(0);
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
                SerPinchValve(1);   // Open

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

                //SerPinchValve(0);
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
                //SerPinchValve(1);

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

                SerPinchValve(0);       // Close
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

                RunPer1_TricontinentPipett((byte)'A', 0,                  // 절대위치 0로 복귀
                                           (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                           (byte)'P', (int)Volume_inc);   // aspiration 방향으로 volume값 만큼 이동

                RunPer1_TricontinentPipett((byte)'?', 0,                  // Request Current Plunger Position
                                           0,                             // N/A
                                           (byte)' ', 0);                 // N/A
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

                RunPer1_TricontinentPipett((byte)' ', 0,                  // N/A
                                           (int)FlowRate_inc_per_sec,     // Plunger Speed 설정
                                           (byte)'D', (int)Volume_inc);   // dispense 방향으로 volume값 만큼 이동

                RunPer1_TricontinentPipett((byte)'?', 0,                  // Request Current Plunger Position
                                           0,                             // N/A
                                           (byte)' ', 0);                 // N/A
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

                RunPer1_TricontinentPipett((byte)'?', 0,                  // Request Current Plunger Position
                                           0,                             // N/A
                                           (byte)' ', 0);                 // N/A
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
                RunPer1_TricontinentPipett((byte)'Z', 0,                 // Initialize Pipett
                                            0,                           // N/A
                                           (byte)' ', 0);                // N/A
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Hamilton Pipett Module Control
        /////////////////////////////////////////////////
        private void btnPlungerInit_HamPipett_Click(object sender, EventArgs e)
        {
            try
            {
                RunPer2_HamiltonPipett("DI", 0, 0, 0, 0, 0, 0);
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
                if (double.Parse(editHamPipettFlowRate.Text) < 0.01 || double.Parse(editHamPipettFlowRate.Text) > 15)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 15 mL/sec");
                    return;
                }

                //입력: mL/sec, 보드전송: 10uL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 100.0;    // 1 mL/s -> 10 uL/s
                
                RunPer2_HamiltonPipett("DE", 0, 0,(int) flowrate, 0, 0, 0);

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
                RunPer2_HamiltonPipett("TD", 0, 0, 0, 0, 0, 0);

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
                    TipType = 1;
                }
                else if (label_TipType.Text == "300 uL")
                {
                    TipType = 2;
                }
                else if (label_TipType.Text == "1000 uL" || label_TipType.Text == "Cal_Ball" || label_TipType.Text == "Cal_Pin")
                {
                    TipType = 3;
                }

                //comboBox_TipType.SelectedItem.ToString
                RunPer2_HamiltonPipett("TP", 0, 0, 0, 0, TipType, 0);

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
                if (double.Parse(editHamPipettFlowRate.Text) < 0.01 || double.Parse(editHamPipettFlowRate.Text) > 15)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 15 mL/sec");
                    return;
                }

                if (double.Parse(editHamPipettAirBlowOutVol.Text) < 0.0 || double.Parse(editHamPipettAirBlowOutVol.Text) > 1)
                {
                    iPrintf("Invalid Value! Pipett Air Blow Out Volume range = 0 ~ 1 mL");
                    return;
                }

                //입력: mL/sec, 보드전송: 10uL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 100.0;    // 1 mL/s -> 10 uL/s
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettAirBlowOutVol.Text) * 1000.0;    // mL -> uL
                
                RunPer2_HamiltonPipett("AB",(int) vol1, 0, (int)flowrate, 0, 0, 0);
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnLiquidDispense_HamPipett_Click(object sender, EventArgs e)
        {
            int mov_opt = 0;

            try
            {
                if (double.Parse(editHamPipettFlowRate.Text) < 0.01 || double.Parse(editHamPipettFlowRate.Text) > 15)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 15 mL/sec");
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

                //입력: mL/sec, 보드전송: 10uL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 100.0;  // 1 mL/s -> 10 uL/s
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettDispenseVol.Text) * 1000.0;    // mL -> uL
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol2 = double.Parse(editHamPipettStopBackVol.Text) * 1000.0;    // mL -> uL

                RunPer2_HamiltonPipett("DL", (int)vol1, (int)vol2, (int)flowrate, stop_spd, 0, mov_opt);
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
                if (double.Parse(editHamPipettFlowRate.Text) < 0.01 || double.Parse(editHamPipettFlowRate.Text) > 15)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 15 mL/sec");
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

                //입력: mL/sec, 보드전송: 10uL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 100.0;  // 1 mL/s -> 10 uL/s
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettAspirateVol.Text) * 1000.0;    // mL -> uL
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol2 = double.Parse(editHamPipettOverAspirateVol.Text) * 1000.0;    // mL -> uL

                RunPer2_HamiltonPipett("AL", (int)vol1, (int)vol2, (int)flowrate, stop_spd, 0, mov_opt);
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
                if (double.Parse(editHamPipettFlowRate.Text) < 0.01 || double.Parse(editHamPipettFlowRate.Text) > 15)
                {
                    iPrintf("Invalid Value! Pipett Flow Rate range = 0.01 ~ 15 mL/sec");
                    return;
                }

                if (double.Parse(editHamPipettTranportAirVol.Text) < 0.0 || double.Parse(editHamPipettTranportAirVol.Text) > 1)
                {
                    iPrintf("Invalid Value! Pipett Transport Air Volume range = 0 ~ 1 mL");
                    return;
                }

                //입력: mL/sec, 보드전송: 10uL/sec, 모듈전송: uL/sec
                double flowrate = double.Parse(editHamPipettFlowRate.Text) * 100.0;    // 1 mL/s -> 10 uL/s
                //입력: mL, 보드전송: uL, 모듈전송: 0.1uL
                double vol1 = double.Parse(editHamPipettTranportAirVol.Text) * 1000;    // mL -> uL
                RunPer2_HamiltonPipett("AT", (int)vol1, 0, (int)flowrate, 0, 0, 0);
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
                if(RunPer2_HamiltonPipett("RT", 0, 0, 0, 0, 0, 0) == COM_Status.ACK && nTipPresence == 2)
                {
                    label_TipPresence.Text = "Present";
                    nTipPresence = 0;
                    retVal = true;
                }
                else if(RunPer2_HamiltonPipett("RT", 0, 0, 0, 0, 0, 0) == COM_Status.ACK && nTipPresence == 1)
                {
                    label_TipPresence.Text = "No Tip";
                    nTipPresence = 0;
                    retVal = false;
                }
                else
                {
                    label_TipPresence.Text = "-";
                    nTipPresence = 0;
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
                if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, 0, 0) == COM_Status.ACK && nState_cLLD == 3)
                {
                    label_TipPresence.Text = "Detected";
                    nState_cLLD = 0;
                    retVal = true;
                }
                else if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, 0, 0) == COM_Status.ACK && nState_cLLD == 2)
                {
                    label_TipPresence.Text = "Searching";
                    nState_cLLD = 0;
                    retVal = false;
                }
                else if (RunPer2_HamiltonPipett("RN", 0, 0, 0, 0, 0, 0) == COM_Status.ACK && nState_cLLD == 1)
                {
                    label_TipPresence.Text = "Idle";
                    nState_cLLD = 0;
                    retVal = false;
                }
                else
                {
                    label_TipPresence.Text = "-";
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
            ConfirmcLLD_State();
        }

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

                if (Run_Hamilton_cLLD((byte) 'L', int.Parse(strcLLD_Sensitivity),
                                      (byte) strcLLD_Dir_Axis[0], (byte) strcLLD_Dir_Axis[1],
                                      int.Parse(editcLLD_Speed.Text),
                                      int.Parse(editcLLD_MaxPosition.Text)) == COM_Status.ACK)
                {
                    if(ConfirmcLLD_State() != true)
                    {
                        iPrintf("Liquid Level Not Detected!");
                    }
                    else
                    {
                        iPrintf("Liquid Level Detection Success!");
                    }
                }
                else
                {
                    iPrintf("Liquid Level Detection Fail!");
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
                Run_Hamilton_cLLD((byte)'P', 0, 0, 0, 0, 0);
                iPrintf("Stop Liquid Level Detection!!");
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
            WritePeltier(bPeltier: chkPeltier.Checked, bCooler: chkCooler.Checked);
        }

        private void btnReadTemperature_Click(object sender, EventArgs e)
        {
            ReadPeltier();
        }

        /////////////////////////////////////////////////
        // Step Motor Alarm Reset (Each Axis)
        /////////////////////////////////////////////////
        private void btnResetServo_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Servo);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep1AxisGripper_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step1Gripper);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep2AxisPipett_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step2Pipett);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep3AxisRotorDoor_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step3RotDr);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep4AxisX_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step4AxisX);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep5AxisY_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step5AxisY);
            btnGetSerialStatus_Click(null, null);
        }

        private void btnResetStep6AxisZ_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step6AxisZ);
            btnGetSerialStatus_Click(null, null);
        }

        /////////////////////////////////////////////////
        // Step Motor Alarm Reset (All Axis)
        /////////////////////////////////////////////////
        private void btnResetAllAxis_Click(object sender, EventArgs e)
        {
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Servo);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step1Gripper);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step2Pipett);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step3RotDr);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step4AxisX);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step5AxisY);
            ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Step6AxisZ);
            btnGetSerialStatus_Click(null, null);
        }

        /*
        private void btnDoorOpen_Click(object sender, EventArgs e)
        {
            SerDoorLock(door1: false, door2: false);
        }

        private void btnDoorClose_Click(object sender, EventArgs e)
        {
            SerDoorLock(door1: true, door2: true);
        }
        */

        /////////////////////////////////////////////////
        // Pinch On/Off
        /////////////////////////////////////////////////
        private void btnPinchValveClose_Click(object sender, EventArgs e)
        {
            try
            {
                SerPinchValve(0);
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
                SerPinchValve(1);
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
            double pos = double.Parse(label_Step4AxisX.Text) + double.Parse(editStepAxisX_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(editStepAxisX_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepIncAxisY_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step5AxisY.Text) + double.Parse(editStepAxisY_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(editStepAxisY_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepIncAxisZ_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step6AxisZ.Text) + double.Parse(editStepAxisZ_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(editStepAxisZ_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepIncGripperAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step1GripperAxis.Text) + double.Parse(editStepGripper_Jog.Text);
            MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(editStepGripper_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepIncPipettAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step2PipettAxis.Text) + double.Parse(editStepPipett_Jog.Text);
            MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(editStepPipett_Speed.Text), pos, 0, 0, 0);
        }


        private void btnStepDecAxisX_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step4AxisX.Text) - double.Parse(editStepAxisX_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(editStepAxisX_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepDecAxisY_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step5AxisY.Text) - double.Parse(editStepAxisY_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(editStepAxisY_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepDecAxisZ_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step6AxisZ.Text) - double.Parse(editStepAxisZ_Jog.Text);
            MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(editStepAxisZ_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepDecGripperAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step1GripperAxis.Text) - double.Parse(editStepGripper_Jog.Text);
            MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(editStepGripper_Speed.Text), pos, 0, 0, 0);
        }

        private void btnStepDecPipettAxis_Click(object sender, EventArgs e)
        {
            double pos = double.Parse(label_Step2PipettAxis.Text) - double.Parse(editStepPipett_Jog.Text);
            MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(editStepPipett_Speed.Text), pos, 0, 0, 0);
        }


        /////////////////////////////////////////////////
        // Eccentric Proximity Sensor Count Control
        // 근접센서의 카운트값을 설정하고 리셋하는 기능
        /////////////////////////////////////////////////
        private void btnWriteEccentric_Click(object sender, EventArgs e)
        {
            WriteEccentric(int.Parse(editEccentric.Text), 0);   //opt -> 0: write, 1: clear
        }

        private void btnReadEccentric_Click(object sender, EventArgs e)
        {
            ReadEccentric();
        }

        private void btnResetEccentric_Click(object sender, EventArgs e)
        {
            //ResetMotor(motor_flag: (int)MOTOR_RESET_FLAG.Eccentric);
            WriteEccentric(int.Parse(editEccentric.Text), 1);   //opt -> 0: write, 1: clear
            ReadEccentric();
        }

        /////////////////////////////////////////////////
        // Load Cell Value Read
        /////////////////////////////////////////////////
        private void btnReadLoadCell_Click(object sender, EventArgs e)
        {
            try
            {
                ReadLoadCell();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
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

        private void btnAccReadStart_Click(object sender, EventArgs e)
        {
            //if (Serial.IsOpen == false)
            //    return;

            try
            {
                if (comboBox_AccScale.SelectedIndex.ToString() == "None" ||
                    comboBox_AccScale.SelectedIndex.ToString() == "0" ||
                    comboBox_GatherValue.SelectedIndex.ToString() == "None" ||
                    comboBox_GatherValue.SelectedIndex.ToString() == "0")
                {
                    if (comboBox_AccScale.SelectedIndex.ToString() == "None" ||
                        comboBox_AccScale.SelectedIndex.ToString() == "0")
                        iPrintf("Accelometer Scale Option Not Selected!");
                    if (comboBox_GatherValue.SelectedIndex.ToString() == "None" ||
                        comboBox_GatherValue.SelectedIndex.ToString() == "0")
                        iPrintf("Accelometer Gathering Value Option Not Selected!");
                    return;
                }

                nAccScaleOpt = GetAccScaleOption();
                nAccGatherVal = GetAccGatherValue();
                bAccSenReadState = true;
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnAccReadStop_Click(object sender, EventArgs e)
        {
            //if (Serial.IsOpen == false)
            //    return;

            bAccSenReadState = false;
            iPrintf("Accelometer Gatherd Data..");
            for (int i = 0; i < nAccSenCnt -1; i++)
            {
                iPrintf(szAccArray[i].ToString());
            }
            nAccSenCnt = 0;
            ReadAccelometer(1, 0, 0, 0);
        }

        private void btnAccDataFileSave_Click(object sender, EventArgs e)
        {
            try
            {
                if(bAccSenReadState == false)
                {
                    DateTime dtNow = DateTime.Now;
                    string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss");
                    string PathAccSenfile = DIR_LOG + "\\accdata_" + dtStr + ".txt";
                    iPrintf("Acc Data Save to" + dtStr + ".txt");

                    File.WriteAllLines(PathAccSenfile, szAccArray);

                    Array.Clear(szAccArray, 0, nMaxAccSenCnt);
                }
                else
                {
                    iPrintf("Acc data reading now, Press read stop again!");
                    return;
                }
                
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        /////////////////////////////////////////////////
        // Laser Sensor State Read
        /////////////////////////////////////////////////
        private void btnReadLaserSensor_Click(object sender, EventArgs e)
        {
            if (Serial.IsOpen == false)
                return;

            ReadLaserSensor();
            //nLaserDetected = 1;

            if (nLaserDetected == 0)
            {
                label_LaserSensorResult.Text = "inactive";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.Reject;
            }
            else if (nLaserDetected == 1)
            {
                label_LaserSensorResult.Text = "active";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.Accept;
            }
            else
            {
                label_LaserSensorResult.Text = "-";
                this.pictureBox_LaserSensorResult.Image = global::CytoDx.Properties.Resources.none2;
            }
        }

        /////////////////////////////////////////////////
        // Flow Meter Read/Stop
        /////////////////////////////////////////////////
        private void btnFlowmeterReadStart_Click(object sender, EventArgs e)
        {
            if (Serial.IsOpen == false)
                return;

            ReadFlowMeter(1);

            flow_stopwatch.Start();
        }

        private void btnFlowmeterReadStop_Click(object sender, EventArgs e)
        {
            if (Serial.IsOpen == false)
                return;

            double pulseP = 0.0;
            double pulseN = 0.0;

            ReadFlowMeter(0);

            flow_stopwatch.Stop();

            bool resultP = double.TryParse(label_FlowPlusPulseCntVal.Text, out pulseP);
            bool resultN = double.TryParse(label_FlowMinusPulseCntVal.Text, out pulseN);

            if (resultP == true && resultN == true)
            {
                double volume = (pulseP - pulseN) * double.Parse(editFlowmeterCntUnit.Text);
                label_FlowMeterVolumeVal.Text = Math.Round(volume, 2).ToString();

                double flowrate = volume / flow_stopwatch.Elapsed.Seconds;
                label_FlowMeterFlowRateVal.Text = Math.Round(flowrate, 2).ToString();
            }
            else
            {
                iPrintf("Pulse value not valid!");
                return;
            }

        }

        private void btnFlowmeterReadReset_Click(object sender, EventArgs e)
        {
            label_FlowPlusPulseCntVal.Text  = "0";
            label_FlowMinusPulseCntVal.Text = "0";
            label_FlowMeterVolumeVal.Text   = "0";
            label_FlowMeterFlowRateVal.Text = "0";
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
                        if (MoveStepMotor(STEP_MOTOR.AXIS_X, int.Parse(DV_AxisX.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisX.Rows[e.RowIndex].Cells[2].Value.ToString()), 0, 0, 0) == COM_Status.ACK)
                        {
                            label_Step4AxisX.Text = DV_AxisX.Rows[e.RowIndex].Cells[2].Value.ToString();
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
                        if (MoveStepMotor(STEP_MOTOR.AXIS_Y, int.Parse(DV_AxisY.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisY.Rows[e.RowIndex].Cells[2].Value.ToString()), 0, 0, 0) == COM_Status.ACK)
                        {
                            label_Step5AxisY.Text = DV_AxisY.Rows[e.RowIndex].Cells[2].Value.ToString();
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
                        if (MoveStepMotor(STEP_MOTOR.AXIS_Z, int.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisZ.Rows[e.RowIndex].Cells[2].Value.ToString()), 0, 0, 0) == COM_Status.ACK)
                        {
                            label_Step6AxisZ.Text = DV_AxisZ.Rows[e.RowIndex].Cells[2].Value.ToString();
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
                        if (MoveStepMotor(STEP_MOTOR.GRIPPER, int.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisGripper.Rows[e.RowIndex].Cells[2].Value.ToString()), 0, 0, 0) == COM_Status.ACK)
                        {
                            label_Step1GripperAxis.Text = DV_AxisGripper.Rows[e.RowIndex].Cells[2].Value.ToString();
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
                        if (MoveStepMotor(STEP_MOTOR.PIPETT, int.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[1].Value.ToString()), 
                            double.Parse(DV_AxisPipett.Rows[e.RowIndex].Cells[2].Value.ToString()), 0, 0, 0) == COM_Status.ACK)
                        {
                            label_Step2PipettAxis.Text = DV_AxisPipett.Rows[e.RowIndex].Cells[2].Value.ToString();
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
        // Centrifuge의 위치를 180도 단위로 변경
        /////////////////////////////////////////////////
        private void btnMoveSeperator_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(0);
        }

        private void btnMoveCelldown_Click(object sender, EventArgs e)
        {
            SelectRotorPosition(1);
        }

        /////////////////////////////////////////////////
        // 실내 조명 On/Off
        /////////////////////////////////////////////////
        private void btnTopLightOff_Click(object sender, EventArgs e)
        {
            TopLight(0);
        }

        private void btnTopLightOn_Click(object sender, EventArgs e)
        {
            TopLight(1);
        }

        /////////////////////////////////////////////////
        // Centrifuge 내부 조명 On/Off
        /////////////////////////////////////////////////
        private void btnRotorLightOff_Click(object sender, EventArgs e)
        {
            RotorLight(0);
        }

        private void btnRotorLightOn_Click(object sender, EventArgs e)
        {
            RotorLight(1);
        }

        /////////////////////////////////////////////////
        // 시스템 파워 Off
        /////////////////////////////////////////////////
        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            RunPer2_HamiltonPipett("AV", 0, 0, 0, 0, 0, 0);
            SystemOff();
        }

        /////////////////////////////////////////////////
        // 로터 도어 개폐
        /////////////////////////////////////////////////
        private void btnCoverOpen_Click(object sender, EventArgs e)
        {
            //Cover(int.Parse(editCoverOpenPos.Text), int.Parse(editCoverOpenSpeed.Text));
            MoveStepMotor(STEP_MOTOR.ROTOR_COVER, int.Parse(editCoverOpenSpeed.Text), double.Parse(editCoverOpenPos.Text), 0, 0, 0);
        }

        private void btnCoverClose_Click(object sender, EventArgs e)
        {
            //Cover(int.Parse(editCoverClosePos.Text), int.Parse(editCoverCloseSpeed.Text));
            MoveStepMotor(STEP_MOTOR.ROTOR_COVER, int.Parse(editCoverCloseSpeed.Text), double.Parse(editCoverClosePos.Text), 0, 0, 0);
        }

        private void Insert_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show(sender.ToString());
            //DV_CTC_Vertical.Rows.Insert(DV_CTC_Vertical);
            //DV_Recipe.Rows.Insert(DV_Recipe.SelectedRows[0].Index, false, "", "", "", "", "", "", "", "");

        }

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
                    strTpnt = GetTpntCategory();

                    if (MenuMotor.SourceControl.Name == "DV_AxisX")
                    {
                        idx = DV_AxisX.SelectedRows[0].Index;
                        DV_AxisX.Rows.Insert(idx, "", "", "");
                    }
                    if (MenuMotor.SourceControl.Name == "DV_AxisY")
                    {
                        idx = DV_AxisY.SelectedRows[0].Index;
                        DV_AxisY.Rows.Insert(idx, "", "", "");
                    }
                    if (MenuMotor.SourceControl.Name == "DV_AxisZ")
                    {
                        idx = DV_AxisZ.SelectedRows[0].Index;
                        DV_AxisZ.Rows.Insert(idx, "", "", "");
                    }
                    if (MenuMotor.SourceControl.Name == "DV_AxisGripper")
                    {
                        idx = DV_AxisGripper.SelectedRows[0].Index;
                        DV_AxisGripper.Rows.Insert(idx, "", "", "");
                    }
                    if (MenuMotor.SourceControl.Name == "DV_AxisPipett")
                    {
                        idx = DV_AxisPipett.SelectedRows[0].Index;
                        DV_AxisPipett.Rows.Insert(idx, "", "", "");
                    }
                    //if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0)
                    //{
                    //    //idx = DV_World_T_Pnt.CurrentCell.RowIndex;
                    //    idx = DV_World_T_Pnt.SelectedRows[0].Index;
                    //    DV_World_T_Pnt.Rows.Insert(idx, strTpnt, "", "", "", "", "", "");
                    //}

                    if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count > 0 &&
                        (strTpnt_Sort != "None" && strTpnt_Sort != null))
                    {
                        int nVisibleRowCnt = DV_World_T_Pnt.DisplayedRowCount(true);
                        int nFirstDisplayedRowIdx = DV_World_T_Pnt.FirstDisplayedCell.RowIndex;
                        int nLastVisibleRowIdx = (nFirstDisplayedRowIdx + nVisibleRowCnt);

                        idx = DV_World_T_Pnt.SelectedRows[0].Index;
                        DV_World_T_Pnt.Rows.Insert(idx, strTpnt, "", "", "", "", "", "");

                        //DV_World_T_Pnt.Rows.Insert(DV_World_T_Pnt.SelectedRows[0].Index, strTpnt, "", "", "", "", "", "");
                        //DV_World_T_Pnt.Rows.Insert(DV_World_T_Pnt.SelectedCells[0].RowIndex, strTpnt, "", "", "", "", "", "");
                        //DV_World_T_Pnt.Rows.Insert(nVisibleRowCnt, strTpnt, "", "");
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && 
                        (strTpnt_Sort == "None" || strTpnt_Sort == null))
                    {
                        //DV_World_T_Pnt.Rows.Insert(DV_World_T_Pnt.SelectedRows[0].Index, "", "", "");
                        iPrintf("Invalid Value! Teaching Point Type Not Selected");
                        //return;
                    }
                    else if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt" && DV_World_T_Pnt.SelectedRows.Count <= 0)
                    {
                        idx = DV_World_T_Pnt.Selected;
                        iPrintf("Selected Rows Count is Minus Value!");
                    }
                }
                else if (e.ClickedItem.Name == "Remove")
                {
                    if (MenuMotor.SourceControl.Name == "DV_AxisX")
                        DV_AxisX.Rows.RemoveAt(DV_AxisX.SelectedRows[0].Index);
                    if (MenuMotor.SourceControl.Name == "DV_AxisY")
                        DV_AxisY.Rows.RemoveAt(DV_AxisY.SelectedRows[0].Index);
                    if (MenuMotor.SourceControl.Name == "DV_AxisZ")
                        DV_AxisZ.Rows.RemoveAt(DV_AxisZ.SelectedRows[0].Index);
                    if (MenuMotor.SourceControl.Name == "DV_AxisGripper")
                        DV_AxisGripper.Rows.RemoveAt(DV_AxisGripper.SelectedRows[0].Index);
                    if (MenuMotor.SourceControl.Name == "DV_AxisPipett")
                        DV_AxisPipett.Rows.RemoveAt(DV_AxisPipett.SelectedRows[0].Index);
                    if (MenuMotor.SourceControl.Name == "DV_World_T_Pnt")
                        DV_World_T_Pnt.Rows.RemoveAt(DV_World_T_Pnt.SelectedRows[0].Index);
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }
    }
}
