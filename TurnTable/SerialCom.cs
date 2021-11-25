using System;
using System.Windows;
using System.Windows.Forms;
using System.IO.Ports;
using MaterialSkin;
using MaterialSkin.Controls;
using Application = System.Windows.Forms.Application;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CytoDx
{
    public partial class MainWindow
    {
        public int SpinDir = 0;
        public int SpinUpTime = 0;
        public int SpinDownTime = 0;
        public int SpinRpm = 0;
        public int Prescale = 0;
        public int SpinDuration = 0;
        public int SpinTotalTime = 0;

        public byte[] m_OneByte = new byte[1];
        public byte[] SOH = new byte[1] { 0x01 };
        public byte[] STX = new byte[1] { 0x02 };
        public byte[] ETX = new byte[1] { 0x03 };
        public byte[] EOT = new byte[1] { 0x04 };
        public byte[] DLE = new byte[1] { 0x10 };
        public byte[] CS = new byte[1];
        public int nChkSum_cnt = 0;
        public int CS_CNT = 4;

        public string strSendCommand;
        public string strSendSubCommand;
        public string strSendParameter;
        public byte[] bSendCmd;
        public byte[] bSendParam;

        public float fMainBoardVersion = 0;

        [Flags]
        public enum MOTOR_RESET_FLAG
        {
            None = 0x00,
            Servo = 0x01,
            StepGripAxis = 0x02,
            StepHamAxis = 0x04,
            StepDoorAx = 0x08,
            Step0AxisX = 0x10,
            Step1AxisY = 0x20,
            Step2AxisZ = 0x40,
            All = 0x7F,
        }

        [Flags]
        public enum PERIPHERAL_RESET_FLAG
        {
            None = 0x00,
            Peri1_TriPipett = 0x01,
            Peri2_HamPipett = 0x02,
            Peri3_TriPump = 0x04,
            All = 0x07,
        }

        [Flags]
        public enum S1_FLAG
        {
            Door = 0x01,
            Servo = 0x02,
            Step_GripAx = 0x04,
            Step_HamAx = 0x08,
        }
        [Flags]
        public enum S2_FLAG
        {
            Step_DoorAx = 0x01,
            Step0_X_Ax = 0x02,
            Step1_Y_Ax = 0x04,
            Step2_Z_Ax = 0x08,
        }
        [Flags]
        public enum S3_FLAG
        {
            Power = 0x01,
            Stop = 0x02,
            RotorCover = 0x04,
            EccentricProxi = 0x08,
        }
        [Flags]
        public enum S4_FLAG
        {
            Peri1_TriPipett = 0x01,
            Peri2_HamPipett = 0x02,
            Peri3_TriPump = 0x04,
            Peri4_LoadCell = 0x08,
        }

        [Flags]
        public enum BYTE_FLAG
        {
            BIT0 = 0x01,
            BIT1 = 0x02,
            BIT2 = 0x04,
            BIT3 = 0x08,
            BIT4 = 0x10,
            BIT5 = 0x20,
            BIT6 = 0x40,
            BIT7 = 0x80,
            BIT8 = 0x100,
            ALL_BIT = 0xff,
        }

        //----------------------------------------------------------------------
        // 사용가능한 씨리얼 포트 탐색
        //----------------------------------------------------------------------
        public void SearchComport()
        {
            try
            {
                // Get a list of serial port names.
                List<string> listPorts = new List<string>();
                listPorts.Add(config.ComPort);

                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    if (port == config.ComPort)
                        continue;
                    listPorts.Add(port);
                }

                ports = listPorts.ToArray();
                foreach (string port in ports)
                {
                    if (Serial.IsOpen)
                        return;

                    if (Serial.IsOpen == false)
                    {
                        if (OpenComPort(port, false))
                        {
                            GetStatus(waitReceive: false);
                            Thread.Sleep(100);
                            //SystemCmd("SYSTEM", "ERRORS", "");

                            int bytes = Serial.BytesToRead;
                            byte[] buffer = new byte[bytes];

                            Serial.Read(buffer, 0, bytes);
                            ReceivedDataParse(bytes, buffer);
                            Serial.Close();

                            btnComConnect.Text = "Connect";
                            if (CmdResult.GetSTATUS == COM_Status.ACK)
                            {
                                OpenComPort(port, true);
                                editCommPorts.Text = port;
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (Serial.IsOpen == false)
            {
                iPrintf("Can not open Serial Port");
            }
        }

        //----------------------------------------------------------------------
        // 열어진 씨리얼 포트 닫기
        //----------------------------------------------------------------------
        private void ClosingOpenedSerialPort()
        {
            // 접속 해제
            if (Serial.IsOpen)
            {
                try
                {
                    Serial.Close();
                }
                catch
                {
                    iPrintf("Fail to Disconnect Serial");
                }

                iPrintf("Disconnected Serial");
            }
        }

        //----------------------------------------------------------------------
        // byte를 string으로 변환
        //----------------------------------------------------------------------
        private string ByteToString(byte[] data, int bytes)
        {
            string str = "";

            str = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            str = string.Format("{0}", str);

            return str;
        }

        //----------------------------------------------------------------------
        // 씨리얼 포트를 열기전 동작 방지를 위한 버튼 활성화/비활성화 정의
        //----------------------------------------------------------------------
        public void ManageCOMUI()
        {
            if (Serial.IsOpen == true && ComStatus.Enabled)
            {
                btn_version.Enabled = true;
                btnManualTestStart.Enabled = true;
                btnManualTestStop.Enabled = true;
                //btn_set_parameter.Enabled = true;
                //btn_get_parameter.Enabled = true;
                btn_get_status.Enabled = true;
                btnTimer.Enabled = true;
                btnRecipeStop.Enabled = true;
                btnRecipeTest.Enabled = true;
            }
            else
            {
                //btn_version.Enabled = false;
                //btnManualTestStart.Enabled = false;
                //btnManualTestStop.Enabled = false;
                //btn_set_parameter.Enabled = false;
                //btn_get_parameter.Enabled = false;
                //btnTimer.Enabled = false;
                Sensor_Laser.Enabled = false;
                Sensor_AlarmServo.Enabled = false;
                Sensor_AlarmStep1Grip.Enabled = false;
                Sensor_AlarmStep2Pipett.Enabled = false;
                Sensor_AlarmStep3Cover.Enabled = false;
                Sensor_AlarmStep0_X_ax.Enabled = false;
                Sensor_AlarmStep1_Y_ax.Enabled = false;
                Sensor_AlarmStep2_Z_ax.Enabled = false;
                EccentricProxi.Enabled = false;
                Sensor_Run.Enabled = false;
                Sensor_TriPipettEnable.Enabled = false;
                Sensor_HamPipettEnable.Enabled = false;
                Sensor_TriPumpEnable.Enabled = false;
                //Sensor_LoadCell.Enabled = false;
                //btn_get_status.Enabled = false;
                //btnRecipeStop.Enabled = false;
                //btnRecipeTest.Enabled = false;
            }
        }

        public void CheckAlarm_ResetDevice()
        {
            if (GetStatus(waitReceive: true) == COM_Status.ACK)
            {
                if (SensorStatus.Alarm)
                {
                    if (ServoState.bALM == true)
                    {
                        ServoMonitor(MotorMon.ALARM);
                        ResetMotor(MOTOR.SERVO);
                    }
                    if (Step0AxState.bALM_B == true)
                    {
                        ResetMotor(MOTOR.STEP0);
                    }
                    if (Step1AxState.bALM_B == true)
                    {
                        ResetMotor(MOTOR.STEP1);
                    }
                    if (Step2AxState.bALM_B == true)
                    {
                        ResetMotor(MOTOR.STEP2);
                    }

                    if (SensorStatus.AlarmPeri1_tri_pipett == Status.ON)
                    {
                        //InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600A0A10z0V1000a1000a0R", Environment.NewLine));
                        //InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600V1000A0A10R", Environment.NewLine));
                        InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600V1000A0A1580R", Environment.NewLine));
                    }
                    if (SensorStatus.AlarmPeri2_ham_pipett == Status.ON)
                    {
                        RunPer2_HamiltonPipett("RE", 0, 0, 0, 0, TIP_TYPE.NONE);    // error code check

                        if (SensorStatus.ham_pipett_errNo != 0)
                        {
                            InitPeripherals(PERIPHERAL.HAM_PIPETT);
                        }
                    }
                    if (SensorStatus.AlarmPeri3_tri_pump == Status.ON)
                    {
                        InitPeripherals(PERIPHERAL.TRI_PUMP);
                    }
                }
            }
        }

        //----------------------------------------------------------------------
        // 씨리얼 포트 열기
        //----------------------------------------------------------------------
        public bool OpenComPort(string strPort, bool bReceiver = true)
        {
            if (strPort == "")
            {
                iPrintf("None of Comport Name");
                return false;
            }

            if (Serial.IsOpen == false)
            {
                try
                {
                    Serial.PortName = config.ComPort = strPort;
                    //Serial.BaudRate = 115200;
                    Serial.BaudRate = 57600;
                    Serial.DataBits = 8;   //	Int16.Parse(lowerComDataBits.Text);
                    Serial.Parity = (Parity)Enum.Parse(typeof(Parity), "None");
                    //	lowerSerial.Parity = (Parity)Enum.Parse(typeof(Parity), "Even");
                    Serial.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                    Serial.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None");
                    Serial.ReadTimeout = 500;
                    Serial.WriteTimeout = 500;
                    Serial.Open();
                }
                catch
                {
                    if (bReceiver)
                    {
                        iPrintf($"Can Not Connet {config.ComPort}");
                        ComStatus.Enabled = false;
                        btnComConnect.Text = "Connect";
                        RefreshSerialPort();
                    }
                    return false;
                }

                // client가 연결되지 않았을 때도 Serial.Isopen이 true가 되는
                // 현상이 있어서 메인보드버전값을 정상적으로 읽어 오는지 확인함. 정상적이지 않으면 리턴함.
                GetFWVersion(0); // 메인보드 버전 읽어오기

                // 메인보드 버전을 정상적으로 체크하였는지를 확인함
                if (fMainBoardVersion <= 0)
                {
                    btnComConnect.Text = "Fail";
                    iPrintf("Version check fail! Client not connected!");
                    return false;
                }
                else
                {
                    btnComConnect.Text = "Close";
                }

                if (Serial.IsOpen == true && bReceiver)
                    ComStatus.Enabled = true;
                else
                    ComStatus.Enabled = false;

                if (Serial.IsOpen)
                {
                    // 파워보드 버전 읽어오기
                    GetFWVersion(1);
                    
                    // Set system parameter
                    SetSystemParam();
                    
                    // Home Detection flag reset
                    bStepMotorInitDoneState = false;

                    //CheckAlarm_ResetDevice();

                    // Cooling Fan ON
                    CoolingFanControl(Status.ON);
                }

                ManageCOMUI();

                if (bReceiver)
                {
                    iPrintf("Connected Serial Comport");
                    Serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
                }

                return true;
            }
            else
            {
                try
                {
                    Serial.Close();
                }
                catch
                {
                    iPrintf("Fail to Disconnect " + strPort);
                }

                if (bReceiver)
                {
                    iPrintf("Disconnected " + strPort);
                }

                label_MainBoardVersion.Text = "";
                label_PowerBoardVersion.Text = "";
                fMainBoardVersion = 0;
                btnComConnect.Text = "Connect";
                ComStatus.Enabled = false;
                ManageCOMUI();
            }
            return false;
        }

        private void SetSystemParam()
        {
            SetStepMotorHomeMoveParam();
            SerCmd_SetParameter(Direction.CCW, int.Parse(edit_rpm.Text), int.Parse(edit_prescale.Text),
                                int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));
        }

        //----------------------------------------------------------------------
        // 사용가능한 씨리얼 포트 재탐색
        //----------------------------------------------------------------------
        public void RefreshSerialPort()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            // Display each port name to the console.
            SerialPortsCount = 0;

            foreach (string port in ports)
            {
                iPrintf($"{port} is Available");

                SerialPortsCount++;
            }

            if (SerialPortsCount == 0)
            {
                iPrintf("None of Available Serial Port");
            }
        }

        //----------------------------------------------------------------------
        // 씨리얼 포트 데이터 수신
        //----------------------------------------------------------------------
        public delegate void UpdateUiTextDelegate(int bytes, byte[] buffer);
        public async void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (bDirectReceive)
                return;

            int bytes = Serial.BytesToRead;
            byte[] buffer = new byte[bytes];

            try
            {
                nRcvBuffCnt = Serial.Read(buffer, 0, bytes);
            }
            catch (System.IO.IOException ex)
            {
                iPrintf(ex.ToString());
                iPrintf(buffer.ToString());
            }

            nRcvBuffCnt = 0;
            this.Invoke(new UpdateUiTextDelegate(ReceivedDataParse), bytes, buffer);
            //var dataParse = Task.Run(() => ReceivedDataParse(bytes, buffer));
            //await dataParse;
        }

        //----------------------------------------------------------------------
        // 1byte 단위로 Serial Data 읽어오기
        //----------------------------------------------------------------------
        public void ReceiveDirect()
        {
            if (Serial.IsOpen == false)
                return;
            try
            {
                Application.DoEvents();

                //bCommunicationActive = true;
                int bytes = Serial.BytesToRead;

                if (bytes > 0)
                {
                    for (int i = 0; i < bytes; i++)
                    {
                        Serial.Read(m_OneByte, 0, 1);
                        RcvFrameParsing(m_OneByte[0]);
                        if (RcvFramePtr == 0)
                            break;
                    }
                }
                //bCommunicationActive = false;
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------------------
        // 정해진 양식으로 수신된 패킷 parsing
        //----------------------------------------------------------------------
        public void ReceivedDataParse(int rbytes, byte[] rdata)
        {
            if (rbytes < 1) return;

            for (int i = 0; i < rbytes && !bDirectReceive; i++)
            {
                RcvFrameParsing(rdata[i]);
            }
        }

        //----------------------------------------------------------------------
        // Receive Packet Format: <SOH><COMMAND><STX><PARAMETER><ETX><CHKSUM>
        // 정해진 양식으로 수신된 패킷 parsing
        //----------------------------------------------------------------------
        private void RcvFrameParsing(byte data)
        {
            try
            {
                if (data == SOH[0])
                {
                    ReceiveFrameFlag = true;
                    RcvFramePtr = 0;
                    bSerialRcvDataFrame[RcvFramePtr++] = data;
                    bSerialRcvDataFrame[RcvFramePtr + 1] = 0;
                }
                else if (data == STX[0] && RcvFramePtr == 0)
                {
                    ReceiveFrameFlag = true;
                    RcvFramePtr = 0;
                    bSerialRcvDataFrame[RcvFramePtr++] = data;
                    bSerialRcvDataFrame[RcvFramePtr + 1] = 0;
                }
                else if (data == ETX[0])
                {
                    bSerialRcvDataFrame[RcvFramePtr++] = data;
                    RcvETXPtr = RcvFramePtr;
                }
                else if (ReceiveFrameFlag == true)
                {
                    bSerialRcvDataFrame[RcvFramePtr++] = data;

                    //if (RcvFramePtr >= MAX_RCV_FRAME_SIZE)
                    //{
                    //    ReceiveFrameFlag = false;
                    //    RcvFramePtr = 0;
                    //}

                    if (RcvETXPtr != 0 && RcvFramePtr == RcvETXPtr + 2)
                    {
                        if (ReceiveFrameFlag == true)
                        {
                            if (isRunning)
                            {
                                ByteToString(bSerialRcvDataFrame, RcvFramePtr);
                                //iPrintf(ByteToString(bSerialRcvDataFrame, RcvFramePtr), false); // for debug
                            }
                            else
                            {
                                iPrintf(ByteToString(bSerialRcvDataFrame, RcvFramePtr), false);
                            }
                        }

                        if (true == CalCheckSum())
                        {
                            RcvDataProcessing();
                        }
                        else
                        {
                            //iPrintf("Check-Sum Error !");
                        }

                        ReceiveFrameFlag = false;
                        RcvFramePtr = 0;
                        RcvETXPtr = 0;

                        Array.Clear(bSerialRcvDataFrame, 0x0, bSerialRcvDataFrame.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString(), true, true, true);
            }
        }


        //----------------------------------------------------------------------
        // 수신된 packet에 대한 check sum 확인
        //----------------------------------------------------------------------
        bool CalCheckSum()
        {
            uint rsum = 0, csum = 0;
            string strRsum = ByteToString(bSerialRcvDataFrame);
            string strCsum = ByteToString(bSerialRcvDataFrame);
            string strTest = ByteToString(bSerialRcvDataFrame);
            string resultCsum = "";

            // ETX를 포함하지 않으면 에러처리            
            if (strCsum.Contains(ByteToString(ETX)) != true)
            {
                iPrintf(string.Format("ETX Not Included! Received:{0}", strCsum));

                g_strRcvDataFrame = "";
                g_strRcvCmd = "";
                g_strRcvParam = "";

                return false;
            }

            // 문자열에서 ETX를 기준으로 문자열 자르기
            // strRsum: ETX 이후 2 바이트, strCsum: ETX앞까지 분리
            if (strRsum != null)
            {
                strRsum = strRsum.Substring(strRsum.IndexOf((char)ETX[0]) + 1, 2);
                if (strRsum.Contains(ByteToString(ETX)) == true)
                {
                    if (strRsum.Length >= strRsum.IndexOf((char)ETX[0]) + 1 + 2)
                    {
                        strRsum = strRsum.Substring(strRsum.IndexOf((char)ETX[0]) + 1, 2);
                    }
                    else
                    {
                        iPrintf("Received Data Separator ETX Not Suitable! - " + strRsum);
                    }
                }
            }
            if (strCsum != null)
                strCsum = strCsum.Substring(0, strCsum.IndexOf((char)ETX[0]));

            // 수신데이터의 체크섬을 rsum 변수에 저장
            if (strRsum != null)
            {
                byte[] arr_byteStrRsum = System.Text.Encoding.Default.GetBytes(strRsum);
                foreach (byte byteStrRsum in arr_byteStrRsum)
                {
                    strRsum += string.Format("{0:X2}", byteStrRsum);
                }

                if (strRsum.Length >= 2)
                {
                    strRsum = strRsum.Substring(0, 2);
                }
                else
                {
                    iPrintf("Received Data Separator ETX Not Suitable! - " + strRsum);
                }

                try
                {
                    rsum = Convert.ToUInt32(strRsum, 16);
                }
                catch (FormatException)
                {
                    iPrintf(String.Format("The {0} value '{1}' is not in a recognizable format.",
                            strRsum.GetType().Name, strRsum));
                    rsum = 0;
                }
            }

            // 수신데이터의 체크섬 계산
            for (int n = 1; n < strCsum.Length; n++)
            {
                csum += strCsum[n];
            }
            csum += ETX[0];

            resultCsum = string.Format("{0:X2}", csum & 0x000000ff);

            // 체크섬 비교
            if (String.Compare(strRsum, resultCsum) != 0)
            {
                iPrintf(string.Format("Data Check Sum Error! R:{0}/{1} C:{2}/{3} -> {4}",
                                      strRsum, rsum, resultCsum, csum, ByteToString(bSerialRcvDataFrame)));

                g_strRcvDataFrame = "";
                g_strRcvCmd = "";
                g_strRcvParam = "";

                return false;
            }
            else
            {
                if (strCsum.IndexOf((char)STX[0]) > 0) // STX가 index 0이 아닌 경우(회신 데이터가 있는 경우)
                {
                    g_strRcvDataFrame = strCsum;
                    g_strRcvCmd = g_strRcvDataFrame.Substring(1, g_strRcvDataFrame.IndexOf((char)STX[0]) - 1);
                    g_strRcvParam = g_strRcvDataFrame.Substring(g_strRcvDataFrame.IndexOf((char)STX[0]) + 1,
                                    g_strRcvDataFrame.Length - g_strRcvDataFrame.IndexOf((char)STX[0]) - 1);
                }
                else    // STX가 index 0인 경우(회신 데이터가 있는 경우)
                {
                    g_strRcvDataFrame = strCsum;
                    g_strRcvCmd = g_strRcvDataFrame.Substring(1, 1);
                    g_strRcvParam = "";
                }

                return true;
            }
        }

        //----------------------------------------------------------------------
        // Send Packet의 Check Sum 생성
        //----------------------------------------------------------------------
        byte[] GenerateChkSum(byte[] bIndata, int nDataLength)
        {
            string bsum;
            byte[] bOutdata = new byte[CS_CNT];
            uint sum = 0;

            for (int n = 1; n < nDataLength; n++)
                sum += bIndata[n];

            //데이터 끝에 NULL삽입 
            bsum = string.Format("{0:X}", sum & 0x000000ff);

            //계산된 cs를 Byte Array로 변환하여 리턴값에 대입
            Array.Copy(StringToByteArray(bsum, (int)bsum.Length), 0, bOutdata, 0, (int)bsum.Length);

            return bOutdata;
        }

        //----------------------------------------------------------------------
        // Send Packet Format: <SOH><COMMAND>,<SUB_COMMAND><STX><PARAMETER><ETX><CHKSUM>
        // SUB_COMMAND, PARAMETER 뒤의 ','를 제외함
        //----------------------------------------------------------------------
        public void BuildCmdPacket(byte[] bCommandSendBuffer, string strSendCommand, string strSendSubCommand, string strSendParameter)
        {
            int nCmdLength = 0, nParamLength = 0;
            string strCmd = "", strParam = "";

            FillCommandSendBuffer((byte)0);

            // Send Cmd, SubCmd 생성, Cmd에 SubCmd 추가
            nCmdLength = strSendCommand.Length + strSendSubCommand.Length + 1;   //2: comma
            strCmd = string.Format("{0},{1}", strSendCommand, strSendSubCommand);
            bSendCmd = StringToByteArray(strCmd, nCmdLength);

            // Send Parameter 생성
            nParamLength = strSendParameter.Length;                              //1: comma
            strParam = string.Format("{0}", strSendParameter);
            bSendParam = StringToByteArray(strParam, nParamLength);

            // bCommandSendBuffer 변수에 Cmd(SubCmd)와 Param 추가
            // bCommandSendBuffer = SOH + bSendCmd + STX + Param + ETX
            Array.Copy(SOH, 0, bCommandSendBuffer, 0, 1);                         // Add SOH
            Array.Copy(bSendCmd, 0, bCommandSendBuffer, 1, bSendCmd.Length);      // Add Cmd, Sub Cmd
            Array.Copy(STX, 0, bCommandSendBuffer, 1 + bSendCmd.Length, 1);       // Add STX
            Array.Copy(bSendParam, 0, bCommandSendBuffer, 1 + bSendCmd.Length + 1, bSendParam.Length);  // Add Param
            Array.Copy(ETX, 0, bCommandSendBuffer, 1 + bSendCmd.Length + 1 + bSendParam.Length, 1);     // Add ETX

            // Check Sum 데이터 생성 후 bCommandSendBuffer 변수에 추가
            nChkSum_cnt = (int)(1 + bSendCmd.Length + 1 + bSendParam.Length + 1);
            CS = GenerateChkSum(bCommandSendBuffer, nChkSum_cnt);                   // Generate ChkSum
            Array.Copy(CS, 0, bCommandSendBuffer, nChkSum_cnt, CS.Length);          // Add ChkSum

            nSendBufferLength = nChkSum_cnt + CS.Length;

            g_strSndCmd = strSendCommand;
            g_strSndSubCmd = strSendSubCommand;
            g_strSndParam = strSendParameter;
        }

        //----------------------------------------------------------------------
        //	public void Write(byte[] buffer, int offset, int count )
        //----------------------------------------------------------------------
        private void SerialByteSend(byte[] data, int counts, bool bSilent = false)
        {
            bool bPrint = false;

            if (Serial.IsOpen)
            {
                try
                {
                    int cnt = 0;
                    bPrint = !bSilent;

                    //ReceiveFrameFlag = false;
                    //nRcvBuffCnt = 0;
                    if (ReceiveFrameFlag == true || nRcvBuffCnt != 0 || bCommunicationActive == true)
                    {
                        while (ReceiveFrameFlag == true || nRcvBuffCnt != 0 || bCommunicationActive == true)
                        {
                            if (cnt >= 20) break;
                            Thread.Sleep(10);
                            iPrintf(string.Format("Now Serial Port Using...State Waiting.({0}/{1}/{2}/{3}) -> ",
                                                  cnt, ReceiveFrameFlag, nRcvBuffCnt, bCommunicationActive) + ByteToString(data));
                            cnt++;
                        }
                    }

                    bCommunicationActive = true;

                    Serial.Write(data, 0, counts);

                    if (isRunning)
                    {
                        ByteToString(data);
                        //iPrintf(ByteToString(data), false, bSilent); //for debug
                    }
                    else
                    {
                        //if(bPrint == true)
                        //    iPrintf(ByteToString(data), false, bSilent);
                        //else
                        //    ByteToString(data);

                        iPrintf(ByteToString(data), false, bSilent);
                    }

                    bCommunicationActive = false;
                }
                catch (Exception ex)
                {
                    iPrintf("Failed to SEND " + data + "\n" + ex + "\n");
                    iPrintf("Failed to SEND String: " + ByteToString(data) + "\n");
                }
            }
            else
            {
            }
        }

        private int HexCharToInt(byte hexChar)
        {
            return (int)hexChar < (int)'A' ? ((int)hexChar - (int)'0') : 10 + ((int)hexChar - (int)'A');
        }

        private void ParseServoStatus(string strRcv)
        {
            ServoState.Status_Servo = int.Parse(strRcv);

            if ((ServoState.Status_Servo & (int)BYTE_FLAG.BIT6) > 0)
            {
                ServoState.bINJECT_LIMIT_HI = true;
                Sensor_InjectLimitHigh.Enabled = false;
            }
            else
            {
                ServoState.bINJECT_LIMIT_HI = false;
                Sensor_InjectLimitHigh.Enabled = true;
            }

            if ((ServoState.Status_Servo & (int)BYTE_FLAG.BIT5) > 0)
            {
                ServoState.bINJECT_LIMIT_LOW = true;
                Sensor_InjectLimitLow.Enabled = false;
            }
            else
            {
                ServoState.bINJECT_LIMIT_LOW = false;
                Sensor_InjectLimitLow.Enabled = true;
            }

            if ((ServoState.Status_Servo & (int)BYTE_FLAG.BIT0) > 0)
                ServoState.bALM = true;
            else
                ServoState.bALM = false;
        }

        private void ParseStep0Status(string strRcv)
        {
            Step0AxState.Status_Step0 = int.Parse(strRcv);

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT7) > 0)
                Step0AxState.bHOME_COMP = true;
            else
                Step0AxState.bHOME_COMP = false;

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT4) > 0)
                Step0AxState.bHOME_END = true;
            else
                Step0AxState.bHOME_END = false;

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT3) > 0)
                Step0AxState.bPLS_RDY = true;
            else
                Step0AxState.bPLS_RDY = false;

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT2) > 0)
                //Step0AxState.bREADY = true;
                Step0AxState.bMOVE = true;
            else
                //Step0AxState.bREADY = false;
                Step0AxState.bMOVE = false;

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT1) > 0)
                //Step0AxState.bMOVE = true;
                Step0AxState.bREADY = true;
            else
                //Step0AxState.bMOVE = false;
                Step0AxState.bREADY = false;

            if ((Step0AxState.Status_Step0 & (int)BYTE_FLAG.BIT0) > 0)
                Step0AxState.bALM_B = true;
            else
                Step0AxState.bALM_B = false;
        }

        private void ParseStep1Status(string strRcv)
        {
            Step1AxState.Status_Step1 = int.Parse(strRcv);

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT7) > 0)
                Step1AxState.bHOME_COMP = true;
            else
                Step1AxState.bHOME_COMP = false;

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT4) > 0)
                Step1AxState.bHOME_END = true;
            else
                Step1AxState.bHOME_END = false;

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT3) > 0)
                Step1AxState.bPLS_RDY = true;
            else
                Step1AxState.bPLS_RDY = false;

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT2) > 0)
                //Step1AxState.bREADY = true;
                Step1AxState.bMOVE = true;
            else
                //Step1AxState.bREADY = false;
                Step1AxState.bMOVE = false;

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT1) > 0)
                //Step1AxState.bMOVE = true;
                Step1AxState.bREADY = true;
            else
                //Step1AxState.bMOVE = false;
                Step1AxState.bREADY = false;

            if ((Step1AxState.Status_Step1 & (int)BYTE_FLAG.BIT0) > 0)
                Step1AxState.bALM_B = true;
            else
                Step1AxState.bALM_B = false;
        }

        private void ParseStep2Status(string strRcv)
        {
            Step2AxState.Status_Step2 = int.Parse(strRcv);

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT7) > 0)
                Step2AxState.bHOME_COMP = true;
            else
                Step2AxState.bHOME_COMP = false;

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT4) > 0)
                Step2AxState.bHOME_END = true;
            else
                Step2AxState.bHOME_END = false;

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT3) > 0)
                Step2AxState.bPLS_RDY = true;
            else
                Step2AxState.bPLS_RDY = false;

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT2) > 0)
                //Step2AxState.bREADY = true;
                Step2AxState.bMOVE = true;
            else
                //Step2AxState.bREADY = false;
                Step2AxState.bMOVE = false;

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT1) > 0)
                //Step2AxState.bMOVE = true;
                Step2AxState.bREADY = true;
            else
                //Step2AxState.bMOVE = false;
                Step2AxState.bREADY = false;

            if ((Step2AxState.Status_Step2 & (int)BYTE_FLAG.BIT0) > 0)
                Step2AxState.bALM_B = true;
            else
                Step2AxState.bALM_B = false;
        }

        private void ParseHamAxStatus(string strRcv)
        {
            HamAxState.Status_Ham = int.Parse(strRcv);

            if ((HamAxState.Status_Ham & (int)BYTE_FLAG.BIT7) > 0)
                HamAxState.bHOME_COMP = true;
            else
                HamAxState.bHOME_COMP = false;

            if ((HamAxState.Status_Ham & (int)BYTE_FLAG.BIT6) > 0)   // B 접점
                HamAxState.bLIMIT_HI = false;
            else
                HamAxState.bLIMIT_HI = true;

            if ((HamAxState.Status_Ham & (int)BYTE_FLAG.BIT5) > 0)   // B 접점
                HamAxState.bLIMIT_LOW = false;
            else
                HamAxState.bLIMIT_LOW = true;

            if ((HamAxState.Status_Ham & (int)BYTE_FLAG.BIT1) > 0)
                HamAxState.bMove = true;
            else
                HamAxState.bMove = false;

            if ((HamAxState.Status_Ham & (int)BYTE_FLAG.BIT0) > 0)
                HamAxState.bALM = true;
            else
                HamAxState.bALM = false;
        }

        private void ParseGripAxStatus(string strRcv)
        {
            GripAxState.Status_Grip = int.Parse(strRcv);

            if ((GripAxState.Status_Grip & (int)BYTE_FLAG.BIT7) > 0)
                GripAxState.bHOME_COMP = true;
            else
                GripAxState.bHOME_COMP = false;

            if ((GripAxState.Status_Grip & (int)BYTE_FLAG.BIT6) > 0)   // B 접점
                GripAxState.bLIMIT_HI = false;
            else
                GripAxState.bLIMIT_HI = true;

            if ((GripAxState.Status_Grip & (int)BYTE_FLAG.BIT5) > 0)   // B 접점
                GripAxState.bLIMIT_LOW = false;
            else
                GripAxState.bLIMIT_LOW = true;

            if ((GripAxState.Status_Grip & (int)BYTE_FLAG.BIT1) > 0)
                GripAxState.bMove = true;
            else
                GripAxState.bMove = false;

            if ((GripAxState.Status_Grip & (int)BYTE_FLAG.BIT0) > 0)
                GripAxState.bALM = true;
            else
                GripAxState.bALM = false;
        }

        private void ParseCoverAxStatus(string strRcv)
        {
            CoverAxState.Status_Door = int.Parse(strRcv);

            if ((CoverAxState.Status_Door & (int)BYTE_FLAG.BIT7) > 0)
                CoverAxState.bHOME_COMP = true;
            else
                CoverAxState.bHOME_COMP = false;

            if ((CoverAxState.Status_Door & (int)BYTE_FLAG.BIT6) > 0)   // B 접점
                CoverAxState.bLIMIT_HI = false;
            else
                CoverAxState.bLIMIT_HI = true;

            if ((CoverAxState.Status_Door & (int)BYTE_FLAG.BIT5) > 0)   // B 접점
                CoverAxState.bLIMIT_LOW = false;
            else
                CoverAxState.bLIMIT_LOW = true;

            if ((CoverAxState.Status_Door & (int)BYTE_FLAG.BIT1) > 0)
                CoverAxState.bMove = true;
            else
                CoverAxState.bMove = false;

            if ((CoverAxState.Status_Door & (int)BYTE_FLAG.BIT0) > 0)
                CoverAxState.bALM = true;
            else
                CoverAxState.bALM = false;
        }

        private void ParseRcvStatusData(string strRcv)
        {
            string[] strRcvSplit = new string[17];
            strRcvSplit = strRcv.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            ParseServoStatus(strRcvSplit[0]);       // 1: SERVO

            ParseStep0Status(strRcvSplit[1]);       // 2: STEP 0

            ParseStep1Status(strRcvSplit[2]);       // 3: STEP 1

            ParseStep2Status(strRcvSplit[3]);       // 4: STEP 2

            ParseHamAxStatus(strRcvSplit[4]);       // 5: HAM

            ParseGripAxStatus(strRcvSplit[5]);      // 6: GRIP

            ParseCoverAxStatus(strRcvSplit[6]);     // 7: Cover

            if (int.Parse(strRcvSplit[7]) > 0)      // 8: POWER
            {
                SensorStatus.PowerState = Status.ON;
                Sensor_power.Enabled = true;
            }
            else
            {
                SensorStatus.PowerState = Status.OFF;
                Sensor_power.Enabled = false;
            }

            if (int.Parse(strRcvSplit[8]) > 0)       // 9: Eccentricity (PNP type, Normal HIGH)
            {
                SensorStatus.EccentricProxi = Status.OFF;
                EccentricProxi.Enabled = false;
            }
            else
            {
                SensorStatus.EccentricProxi = Status.ON;
                EccentricProxi.Enabled = true;
            }

            ParseSwitchStatus(strRcvSplit[9]);      // 10: Switch

            if (int.Parse(strRcvSplit[10]) > 0)     // 11: Laser (PNP type, Normal HIGH)
            {
                SensorStatus.LaserSensor = Status.OFF;
                Sensor_Laser.Enabled = false;
            }
            else
            {
                SensorStatus.LaserSensor = Status.ON;
                Sensor_Laser.Enabled = true;
            }

            if (int.Parse(strRcvSplit[11]) > 0)      // 12: X_HOMP_COMP
            {
                Step0AxState.bHOME_COMP = true;
            }
            else
            {
                Step0AxState.bHOME_COMP = false;
            }

            if (int.Parse(strRcvSplit[12]) > 0)     // 13: Y_HOMP_COMP
            {
                Step1AxState.bHOME_COMP = true;
            }
            else
            {
                Step1AxState.bHOME_COMP = false;
            }

            if (int.Parse(strRcvSplit[13]) > 0)     // 14: Z_HOMP_COMP
            {
                Step2AxState.bHOME_COMP = true;
            }
            else
            {
                Step2AxState.bHOME_COMP = false;
            }

            if (int.Parse(strRcvSplit[14]) > 0)     // 15: HAM_HOMP_COMP
            {
                HamAxState.bHOME_COMP = true;
            }
            else
            {
                HamAxState.bHOME_COMP = false;
            }

            if (int.Parse(strRcvSplit[15]) > 0)     // 16: GRIP_HOMP_COMP
            {
                GripAxState.bHOME_COMP = true;
            }
            else
            {
                GripAxState.bHOME_COMP = false;
            }

            if (int.Parse(strRcvSplit[16]) > 0)     // 17: CV_HOMP_COMP
            {
                CoverAxState.bHOME_COMP = true;
            }
            else
            {
                CoverAxState.bHOME_COMP = false;
            }

            ////////////////////////////////////////////
            if (ServoState.bALM == true)          // 알람 발생시 1, 없을시 0
            {
                SensorStatus.AlarmServo = Status.ON;
                Sensor_AlarmServo.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmServo = Status.OFF;
                Sensor_AlarmServo.Enabled = true;
            }

            if (GripAxState.bALM == true)
            {
                SensorStatus.AlarmStep_Grip_ax = Status.ON;
                Sensor_AlarmStep1Grip.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep_Grip_ax = Status.OFF;
                Sensor_AlarmStep1Grip.Enabled = true;
            }

            if (HamAxState.bALM == true)
            {
                SensorStatus.AlarmStep_Ham_ax = Status.ON;
                Sensor_AlarmStep2Pipett.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep_Ham_ax = Status.OFF;
                Sensor_AlarmStep2Pipett.Enabled = true;
            }

            if (CoverAxState.bALM == true)
            {
                SensorStatus.AlarmStep_Door_ax = Status.ON;
                Sensor_AlarmStep3Cover.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep_Door_ax = Status.OFF;
                Sensor_AlarmStep3Cover.Enabled = true;
            }

            if (Step0AxState.bALM_B == true)
            {
                SensorStatus.AlarmStep0_X_ax = Status.ON;
                Sensor_AlarmStep0_X_ax.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep0_X_ax = Status.OFF;
                Sensor_AlarmStep0_X_ax.Enabled = true;
            }

            if (Step1AxState.bALM_B == true)
            {
                SensorStatus.AlarmStep1_Y_ax = Status.ON;
                Sensor_AlarmStep1_Y_ax.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep1_Y_ax = Status.OFF;
                Sensor_AlarmStep1_Y_ax.Enabled = true;
            }

            if (Step2AxState.bALM_B == true)
            {
                SensorStatus.AlarmStep2_Z_ax = Status.ON;
                Sensor_AlarmStep2_Z_ax.Enabled = false;
            }
            else
            {
                SensorStatus.AlarmStep2_Z_ax = Status.OFF;
                Sensor_AlarmStep2_Z_ax.Enabled = true;
            }

            if (nEccentricCnt >= nEccentricThreshold)
                SensorStatus.ErrEccentricCnt = true;

            if (ServoState.bALM == true ||
                Step0AxState.bALM_B == true ||
                Step1AxState.bALM_B == true ||
                Step2AxState.bALM_B == true ||
                HamAxState.bALM == true ||
                GripAxState.bALM == true ||
                CoverAxState.bALM == true ||
                SensorStatus.ErrEccentricCnt == true ||
                SensorStatus.AlarmPeri1_tri_pipett == Status.ON ||
                SensorStatus.AlarmPeri2_ham_pipett == Status.ON ||
                SensorStatus.AlarmPeri3_tri_pump == Status.ON ||
                CmdResult.ControllerCom == COM_Status.NAK)
            {
                SensorStatus.Alarm = true;
                btnAlarmStatus.Enabled = true;
            }
            else
            {
                SensorStatus.Alarm = false;
                btnAlarmStatus.Enabled = false;
            }

            if (ServoState.bHOME_COMP == true &&
                Step0AxState.bHOME_COMP == true &&
                Step1AxState.bHOME_COMP == true &&
                Step2AxState.bHOME_COMP == true &&
                HamAxState.bHOME_COMP == true &&
                GripAxState.bHOME_COMP == true)
            //GripAxState.bHOME_COMP == true && CoverAxState.bHOME_COMP == true)
            {
                SensorStatus.Home = true;
                btnHomeStatus.Enabled = true;
            }
            else
            {
                SensorStatus.Home = false;
                btnHomeStatus.Enabled = false;
            }
        }

        private void ParseRcvSwitchData()
        {
            if (SwitchMon.bStop == true)
            {
                SensorStatus.StopSwitch = Status.ON;
                Sensor_Stop.Enabled = true;
            }
            else
            {
                SensorStatus.StopSwitch = Status.OFF;
                Sensor_Stop.Enabled = false;
            }

            if (SwitchMon.bRun == true)
            {
                SensorStatus.RunSwitch = Status.ON;
                Sensor_Run.Enabled = true;
            }
            else
            {
                SensorStatus.RunSwitch = Status.OFF;
                Sensor_Run.Enabled = false;
            }

            if (SwitchMon.bPower == true)
            {
                SensorStatus.PowerSwitch = Status.ON;
            }
            else
            {
                SensorStatus.PowerSwitch = Status.OFF;
            }

            if (SwitchMon.bDoorEnable == true)
            {
                SensorStatus.DoorEnable = Status.ON;
            }
            else
            {
                SensorStatus.DoorEnable = Status.OFF;
            }

            if (SwitchMon.bDoorOpenSave == true)
            {
                SensorStatus.DoorOpenSave = Status.ON;
            }
            else
            {
                SensorStatus.DoorOpenSave = Status.OFF;
            }

            if (SwitchMon.bDoorSW == true)
            {
                SensorStatus.DoorSwitch = Status.ON;
            }
            else
            {
                SensorStatus.DoorSwitch = Status.OFF;
            }

            //if ((S3 & (int)S3_FLAG.EccentricProxi) > 0)
            //{
            //    SensorStatus.EccentricProxi = Status.ON;
            //    EccentricProxi.Enabled = true;
            //}
            //else
            //{
            //    SensorStatus.EccentricProxi = Status.OFF;
            //    EccentricProxi.Enabled = false;
            //}
        }

        private void ParseRcvPeltierData(string strRcv)
        {
            string[] strRcvSplit = new string[4];
            strRcvSplit = strRcv.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                PeltMon.dbSetPeltTemp = double.Parse(strRcvSplit[0]);     // Set Value 확인
            }
            catch (FormatException)
            {
                iPrintf(string.Format("dbSetPeltTemp FormatException {0}", strRcvSplit[0]));
                PeltMon.dbSetPeltTemp = 0;
            }

            try
            {
                PeltMon.dbTempChamber = double.Parse(strRcvSplit[1]);     // 챔버(튜브)
            }
            catch (FormatException)
            {
                iPrintf(string.Format("dbTempChamber FormatException {0}", strRcvSplit[1]));
                PeltMon.dbTempChamber = 0;
            }

            try
            {
                PeltMon.dbTempPeltier = double.Parse(strRcvSplit[2]);     // 펠티어
            }
            catch (FormatException)
            {
                iPrintf(string.Format("dbTempPeltier FormatException {0}", strRcvSplit[2]));
                PeltMon.dbTempPeltier = 0;
            }

            try
            {
                PeltMon.dbTempCooler = double.Parse(strRcvSplit[3]);      // 방열판
            }
            catch (FormatException)
            {
                iPrintf(string.Format("dbTempCooler FormatException {0}", strRcvSplit[3]));
                PeltMon.dbTempCooler = 0;
            }
        }

        private void ParseRcvPositionData(string strRcv)
        {
            string[] strRcvSplit = new string[7];
            strRcvSplit = strRcv.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            CurrentPos.Servo = double.Parse(strRcvSplit[0]);
            CurrentPos.Step0AxisX = double.Parse(strRcvSplit[1]);
            CurrentPos.Step1AxisY = double.Parse(strRcvSplit[2]);
            CurrentPos.Step2AxisZ = double.Parse(strRcvSplit[3]);
            CurrentPos.StepHamAxis = double.Parse(strRcvSplit[4]);
            CurrentPos.StepGripAxis = double.Parse(strRcvSplit[5]);
            CurrentPos.StepRotCover = double.Parse(strRcvSplit[6]);

            CurrentPos.Servo_Deg = (((double.Parse(strRcvSplit[0]) - double.Parse(config.ServoPos_Chamber1)) % 4194304) * 360) / 4194304;
        }

        private void ParseSwitchStatus(string strRcv)
        {
            SwitchMon.Status_Switch = int.Parse(strRcv);

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT7) > 0)
                SwitchMon.bDoorOpenSave = true;
            else
                SwitchMon.bDoorOpenSave = false;

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT6) > 0)
                SwitchMon.bDoorEnable = true;
            else
                SwitchMon.bDoorEnable = false;

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT5) > 0)
                SwitchMon.bDoorSW = true;
            else
                SwitchMon.bDoorSW = false;

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT3) > 0)
                SwitchMon.bPower = true;
            else
                SwitchMon.bPower = false;

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT1) > 0)
                SwitchMon.bStop = true;
            else
                SwitchMon.bStop = false;

            if ((SwitchMon.Status_Switch & (int)BYTE_FLAG.BIT0) > 0)
                SwitchMon.bRun = true;
            else
                SwitchMon.bRun = false;
        }

        private void ParseErrorStatus(string strRcv)
        {
            ErrorMon.Status_Error = int.Parse(strRcv);

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT8) > 0)
            {
                ErrorMon.bLoadcell2_notConnected = true;
                iPrintf("Loadcell 2 Not Connected!");
                SensorStatus.AlarmLoadcell2 = Status.ON;
            }
            else
            {
                ErrorMon.bLoadcell2_notConnected = false;
                SensorStatus.AlarmLoadcell2 = Status.OFF;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT7) > 0)
            {
                ErrorMon.bLoadcell1_notConnected = true;
                iPrintf("Loadcell 1 Not Connected!");
                SensorStatus.AlarmLoadcell1 = Status.ON;
            }
            else
            {
                ErrorMon.bLoadcell1_notConnected = false;
                SensorStatus.AlarmLoadcell1 = Status.OFF;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT6) > 0)
            {
                ErrorMon.bLoadcell0_notConnected = true;
                iPrintf("Loadcell 0 Not Connected!");
                SensorStatus.AlarmLoadcell0 = Status.ON;
                Sensor_Loadcell.Enabled = false;
            }
            else
            {
                ErrorMon.bLoadcell0_notConnected = false;
                SensorStatus.AlarmLoadcell0 = Status.OFF;
                Sensor_Loadcell.Enabled = true;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT5) > 0)
            {
                ErrorMon.bCoolerPeltSensor_short = true;
                iPrintf("Cooler Peltier Sensor Line Short!");
                SensorStatus.AlarmCoolerPeltSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerPeltSensor_short = false;
                SensorStatus.AlarmCoolerPeltSensor = Status.OFF;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT4) > 0)
            {
                ErrorMon.bCoolerPeltSensor_open = true;
                iPrintf("Cooler Peltier Sensor Line Opened!");
                SensorStatus.AlarmCoolerPeltSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerPeltSensor_open = false;
                SensorStatus.AlarmCoolerPeltSensor = Status.OFF;
            }

            if (((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT5) > 0) ||
                ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT4) > 0))
            {
                Sensor_PeltThermo.Enabled = false;
            }
            else
            {
                Sensor_PeltThermo.Enabled = true;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT3) > 0)
            {
                ErrorMon.bCoolerFanSensor_short = true;
                iPrintf("Cooler Fan Sensor Line Short!");
                SensorStatus.AlarmCoolerFanSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerFanSensor_short = false;
                SensorStatus.AlarmCoolerFanSensor = Status.OFF;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT2) > 0)
            {
                ErrorMon.bCoolerFanSensor_open = true;
                iPrintf("Cooler Fan Sensor Line Opened!");
                SensorStatus.AlarmCoolerFanSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerFanSensor_open = false;
                SensorStatus.AlarmCoolerFanSensor = Status.OFF;
            }

            if (((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT3) > 0) ||
                ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT2) > 0))
            {
                Sensor_FanThermo.Enabled = false;
            }
            else
            {
                Sensor_FanThermo.Enabled = true;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT1) > 0)
            {
                ErrorMon.bCoolerChamberSensor_short = true;
                iPrintf("Cooler Chamber Sensor Line Short!");
                SensorStatus.AlarmCoolerChamberSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerChamberSensor_short = false;
                SensorStatus.AlarmCoolerChamberSensor = Status.OFF;
            }

            if ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT0) > 0)
            {
                ErrorMon.bCoolerChamberSensor_open = true;
                iPrintf("Cooler Chamber Sensor Line Opened!");
                SensorStatus.AlarmCoolerChamberSensor = Status.ON;
            }
            else
            {
                ErrorMon.bCoolerChamberSensor_open = false;
                SensorStatus.AlarmCoolerChamberSensor = Status.OFF;
            }

            if (((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT1) > 0) ||
                ((ErrorMon.Status_Error & (int)BYTE_FLAG.BIT0) > 0))
            {
                Sensor_ChambThermo.Enabled = false;
            }
            else
            {
                Sensor_ChambThermo.Enabled = true;
            }
        }

        //----------------------------------------------------------------------
        //	frame 수신 완료, check ok
        //----------------------------------------------------------------------
        private void RcvDataProcessing()
        {
            string rcvstr = g_strRcvCmd;

            if (String.Compare(rcvstr, "A") == 0)
            {
                CmdResult.ControllerCom = COM_Status.ACK;

                if ((g_strSndCmd == "SYSTEM" && g_strSndSubCmd == "INIT") ||
                    g_strSndCmd == "ESCAPE")
                {
                    CmdResult.SystemCmd = COM_Status.ACK;
                }

                if (g_strSndCmd == "TEMP" &&
                   (g_strSndSubCmd == "SET_SV" || g_strSndSubCmd == "SET_FAN")) // PELT40 temperature control stop/start
                {
                    CmdResult.WritePeltier = COM_Status.ACK;
                }

                if (g_strSndCmd == "ETC" && g_strSndSubCmd == "PINCH")
                {
                    CmdResult.PinchValve = COM_Status.ACK;
                }

                if (g_strSndCmd == "DISK" &&
                   (g_strSndSubCmd == "TURN" || g_strSndSubCmd == "STOP" || 
                    g_strSndSubCmd == "MOVA"))   // Run/Stop centrifuge motor & camera
                {
                    if (g_strSndSubCmd == "MOVA")
                        CmdResult.RotorPos = COM_Status.ACK;
                    else
                        CmdResult.Spin = COM_Status.ACK;
                }

                if (g_strSndSubCmd == "HOME")
                {
                    CmdResult.Home = COM_Status.ACK;
                }

                if ((g_strSndCmd == "STEP 0" || g_strSndCmd == "STEP 1" || g_strSndCmd == "STEP 2" ||
                     g_strSndCmd == "HAM" || g_strSndCmd == "GRIP" || g_strSndCmd == "COVER") &&
                   (g_strSndSubCmd == "MOVA" || g_strSndSubCmd == "MOVR"))
                {
                    if (g_strSndCmd == "GRIP")
                        CmdResult.StepMotor1Gripper = COM_Status.ACK;
                    if (g_strSndCmd == "HAM")
                        CmdResult.StepMotor2Pipett = COM_Status.ACK;
                    if (g_strSndCmd == "COVER")
                        CmdResult.StepMotor3RotorCover = COM_Status.ACK;
                    if (g_strSndCmd == "STEP 0")
                        CmdResult.StepMotor4AxisX = COM_Status.ACK;
                    if (g_strSndCmd == "STEP 1")
                        CmdResult.StepMotor5AxisY = COM_Status.ACK;
                    if (g_strSndCmd == "STEP 2")
                        CmdResult.StepMotor6AxisZ = COM_Status.ACK;
                }

                if (g_strSndCmd == "LIGHT")
                {
                    if (g_strSndSubCmd == "ROOM_PWR")
                        CmdResult.TopLight = COM_Status.ACK;
                    else if (g_strSndSubCmd == "CHAMBER_PWR")
                        CmdResult.RotorLight = COM_Status.ACK;
                    else if (g_strSndSubCmd == "CHAMBER_SV" || g_strSndSubCmd == "ROOM_SV")
                        CmdResult.LightCond = COM_Status.ACK;
                }
            }
            if (String.Compare(rcvstr, "C") == 0)
            {
                iPrintf("Controller Check Sum Error!");
                CmdResult.ControllerCom = COM_Status.NAK;
                if (g_strSndCmd == "TEMP" &&
                   (g_strSndSubCmd == "SET_SV" || g_strSndSubCmd == "SET_FAN")) // PELT40 temperature control stop/start
                {
                    CmdResult.WritePeltier = COM_Status.NAK;
                }
                if (g_strSndCmd == "DISK" &&
                   (g_strSndSubCmd == "TURN" || g_strSndSubCmd == "STOP" || 
                    g_strSndSubCmd == "MOVA"))   // Run/Stop centrifuge motor & camera
                {
                    if (g_strSndSubCmd == "MOVA")
                        CmdResult.RotorPos = COM_Status.NAK;
                    else
                        CmdResult.Spin = COM_Status.NAK;
                }
                if (g_strSndSubCmd == "HOME")
                {
                    CmdResult.Home = COM_Status.NAK;
                }
                if ((g_strSndCmd == "STEP 0" || g_strSndCmd == "STEP 1" || g_strSndCmd == "STEP 2" ||
                     g_strSndCmd == "HAM" || g_strSndCmd == "GRIP" || g_strSndCmd == "COVER") &&
                   (g_strSndSubCmd == "MOVA" || g_strSndSubCmd == "MOVR"))
                {
                    if (g_strSndCmd == "GRIP")
                        CmdResult.StepMotor1Gripper = COM_Status.NAK;
                    if (g_strSndCmd == "HAM")
                        CmdResult.StepMotor2Pipett = COM_Status.NAK;
                    if (g_strSndCmd == "COVER")
                        CmdResult.StepMotor3RotorCover = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 0")
                        CmdResult.StepMotor4AxisX = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 1")
                        CmdResult.StepMotor5AxisY = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 2")
                        CmdResult.StepMotor6AxisZ = COM_Status.NAK;
                }
            }
            if (String.Compare(rcvstr, "E") == 0)
            {
                iPrintf("Controller Command Error!");
                CmdResult.ControllerCom = COM_Status.NAK;
                if (g_strSndCmd == "TEMP" &&
                   (g_strSndSubCmd == "SET_SV" || g_strSndSubCmd == "SET_FAN")) // PELT40 temperature control stop/start
                {
                    CmdResult.WritePeltier = COM_Status.NAK;
                }
                if (g_strSndCmd == "DISK" &&
                   (g_strSndSubCmd == "TURN" || g_strSndSubCmd == "STOP" || 
                    g_strSndSubCmd == "MOVA"))   // Run/Stop centrifuge motor & camera
                {
                    if (g_strSndSubCmd == "MOVA")
                        CmdResult.RotorPos = COM_Status.NAK;
                    else
                        CmdResult.Spin = COM_Status.NAK;
                }
                if (g_strSndSubCmd == "HOME")
                {
                    CmdResult.Home = COM_Status.NAK;
                }
                if ((g_strSndCmd == "STEP 0" || g_strSndCmd == "STEP 1" || g_strSndCmd == "STEP 2" ||
                     g_strSndCmd == "HAM" || g_strSndCmd == "GRIP" || g_strSndCmd == "COVER") &&
                   (g_strSndSubCmd == "MOVA" || g_strSndSubCmd == "MOVR"))
                {
                    if (g_strSndCmd == "GRIP")
                        CmdResult.StepMotor1Gripper = COM_Status.NAK;
                    if (g_strSndCmd == "HAM")
                        CmdResult.StepMotor2Pipett = COM_Status.NAK;
                    if (g_strSndCmd == "COVER")
                        CmdResult.StepMotor3RotorCover = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 0")
                        CmdResult.StepMotor4AxisX = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 1")
                        CmdResult.StepMotor5AxisY = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 2")
                        CmdResult.StepMotor6AxisZ = COM_Status.NAK;
                }
            }
            if (String.Compare(rcvstr, "F") == 0)
            {
                iPrintf("Controller Sub Command Error!");
                CmdResult.ControllerCom = COM_Status.NAK;
                if (g_strSndCmd == "TEMP" &&
                   (g_strSndSubCmd == "SET_SV" || g_strSndSubCmd == "SET_FAN")) // PELT40 temperature control stop/start
                {
                    CmdResult.WritePeltier = COM_Status.NAK;
                }
                if (g_strSndCmd == "DISK" &&
                   (g_strSndSubCmd == "TURN" || g_strSndSubCmd == "STOP" || 
                    g_strSndSubCmd == "MOVA"))   // Run/Stop centrifuge motor & camera
                {
                    if (g_strSndSubCmd == "MOVA")
                        CmdResult.RotorPos = COM_Status.NAK;
                    else
                        CmdResult.Spin = COM_Status.NAK;
                }
                if (g_strSndSubCmd == "HOME")
                {
                    CmdResult.Home = COM_Status.NAK;
                }
                if ((g_strSndCmd == "STEP 0" || g_strSndCmd == "STEP 1" || g_strSndCmd == "STEP 2" ||
                     g_strSndCmd == "HAM" || g_strSndCmd == "GRIP" || g_strSndCmd == "COVER") &&
                   (g_strSndSubCmd == "MOVA" || g_strSndSubCmd == "MOVR"))
                {
                    if (g_strSndCmd == "GRIP")
                        CmdResult.StepMotor1Gripper = COM_Status.NAK;
                    if (g_strSndCmd == "HAM")
                        CmdResult.StepMotor2Pipett = COM_Status.NAK;
                    if (g_strSndCmd == "COVER")
                        CmdResult.StepMotor3RotorCover = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 0")
                        CmdResult.StepMotor4AxisX = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 1")
                        CmdResult.StepMotor5AxisY = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 2")
                        CmdResult.StepMotor6AxisZ = COM_Status.NAK;
                }
            }
            if (String.Compare(rcvstr, "T") == 0)
            {
                iPrintf("Controller Time Out Error!");
                CmdResult.ControllerCom = COM_Status.NAK;
                if (g_strSndCmd == "TEMP" &&
                   (g_strSndSubCmd == "SET_SV" || g_strSndSubCmd == "SET_FAN")) // PELT40 temperature control stop/start
                {
                    CmdResult.WritePeltier = COM_Status.NAK;
                }
                if (g_strSndCmd == "DISK" &&
                   (g_strSndSubCmd == "TURN" || g_strSndSubCmd == "STOP" || 
                    g_strSndSubCmd == "MOVA"))   // Run/Stop centrifuge motor & camera
                {
                    if (g_strSndSubCmd == "MOVA")
                        CmdResult.RotorPos = COM_Status.NAK;
                    else
                        CmdResult.Spin = COM_Status.NAK;
                }
                if (g_strSndSubCmd == "HOME")
                {
                    CmdResult.Home = COM_Status.NAK;
                }
                if ((g_strSndCmd == "STEP 0" || g_strSndCmd == "STEP 1" || g_strSndCmd == "STEP 2" ||
                     g_strSndCmd == "HAM" || g_strSndCmd == "GRIP" || g_strSndCmd == "COVER") &&
                   (g_strSndSubCmd == "MOVA" || g_strSndSubCmd == "MOVR"))
                {
                    if (g_strSndCmd == "GRIP")
                        CmdResult.StepMotor1Gripper = COM_Status.NAK;
                    if (g_strSndCmd == "HAM")
                        CmdResult.StepMotor2Pipett = COM_Status.NAK;
                    if (g_strSndCmd == "COVER")
                        CmdResult.StepMotor3RotorCover = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 0")
                        CmdResult.StepMotor4AxisX = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 1")
                        CmdResult.StepMotor5AxisY = COM_Status.NAK;
                    if (g_strSndCmd == "STEP 2")
                        CmdResult.StepMotor6AxisZ = COM_Status.NAK;
                }
            }

            if (rcvstr.Contains("all_status") && g_strSndSubCmd == "STATUS")  // Read Status Sensor & Etc
            {
                CmdResult.GetSTATUS = COM_Status.ACK;
                ParseRcvStatusData(g_strRcvParam);

                if (lastRcvStr != rcvstr)  // Returns if rcv is the same as last rcv
                {
                    ;
                }
            }
            else if (rcvstr.Contains("all") && g_strSndSubCmd == "TEMP")  // Read all peltier temperature
            {
                CmdResult.ReadPeltier = COM_Status.ACK;
                ParseRcvPeltierData(g_strRcvParam);
            }
            else if (rcvstr.Contains("errors") && g_strSndSubCmd == "ERRORS")
            {
                CmdResult.SystemCmd = COM_Status.ACK;
                ParseErrorStatus(g_strRcvParam);
            }
            else if (rcvstr.Contains("all_position"))  // Read all axes position
            {
                CmdResult.ReadMotorPos = COM_Status.ACK;
                ParseRcvPositionData(g_strRcvParam);
            }
            else if (rcvstr.Contains("SWITCH"))  // panel switch 누름시
            {
                ParseSwitchStatus(g_strRcvParam);
                ParseRcvSwitchData();
            }
            else if (rcvstr.Contains("switch") && g_strSndSubCmd == "STATUS")  // Read Switch State
            {
                CmdResult.Switch = COM_Status.ACK;
                ParseSwitchStatus(g_strRcvParam);
                ParseRcvSwitchData();
            }
            else if (rcvstr.Contains("vers"))      // check firmware version
            {
                CmdResult.GetVersion = COM_Status.ACK;
                // 수신데이터: SOH,"vers",STX,"c2000 ver 1.00",ETX
                if (g_strSndSubCmd == "MAIN")
                {
                    label_MainBoardVersion.Text = g_strRcvParam.Substring(6, 4);    //main  1.20
                    fMainBoardVersion = float.Parse(label_MainBoardVersion.Text);
                }
                else if (g_strSndSubCmd == "POWER")
                {
                    label_PowerBoardVersion.Text = g_strRcvParam;
                }
            }
            else if (rcvstr.Contains("gripper"))     // step motor 1(gripper axis) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor1Gripper = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_StepGripAxisPos.Text = g_strRcvParam;
                    label_WorldPosGripper.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseGripAxStatus(g_strRcvParam);
                }
            }
            else if (string.Compare(rcvstr, "ham") == 0)     // step motor 2(pipett axis) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor2Pipett = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_StepHamAxisPos.Text = g_strRcvParam;
                    label_WorldPosHamPipett.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseHamAxStatus(g_strRcvParam);
                }
            }
            else if (rcvstr.Contains("cover"))     // step motor 3(rotor cover) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor3RotorCover = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_StepDoorAxis.Text = g_strRcvParam;
                    label_WorldPosCenDoor.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseCoverAxStatus(g_strRcvParam);
                }
            }
            else if (rcvstr.Contains("step x"))     // step motor 4(X Axis) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor4AxisX = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_Step0AxisXPos.Text = g_strRcvParam;
                    label_WorldPosX.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseStep0Status(g_strRcvParam);
                }
            }
            else if (rcvstr.Contains("step y"))     // step motor 5(Y Axis) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor5AxisY = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_Step1AxisYPos.Text = g_strRcvParam;
                    label_WorldPosY.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseStep1Status(g_strRcvParam);
                }
            }
            else if (rcvstr.Contains("step z"))     // step motor 6(Z Axis) position move
            {
                if (g_strSndSubCmd == "HOME")
                    CmdResult.Home = COM_Status.ACK;
                else if (g_strSndSubCmd == "ALM_RST")
                    CmdResult.ResetMotor = COM_Status.ACK;
                else
                    CmdResult.StepMotor6AxisZ = COM_Status.ACK;

                if (g_strSndSubCmd == "POS")
                {
                    label_Step2AxisZPos.Text = g_strRcvParam;
                    label_WorldPosZ.Text = g_strRcvParam;
                }
                if (g_strSndSubCmd == "STATUS")
                {
                    ParseStep2Status(g_strRcvParam);
                }
            }
            else if (rcvstr.Contains("chamber sv") || rcvstr.Contains("chamber pv") ||
                     rcvstr.Contains("peltier pv") || rcvstr.Contains("hitsink pv"))     // PELT40 temperature check
            {
                CmdResult.ReadPeltier = COM_Status.ACK;
                if (g_strSndSubCmd == "CHAMBER_SV")
                {
                    PeltMon.dbSetPeltTemp = double.Parse(g_strRcvParam);         // Set Value 확인
                }
                if (g_strSndSubCmd == "CHAMBER_PV")
                {
                    PeltMon.dbTempChamber = double.Parse(g_strRcvParam);         // 챔버(튜브)
                }
                if (g_strSndSubCmd == "PELTIER_PV")
                {
                    PeltMon.dbTempPeltier = double.Parse(g_strRcvParam);         // 펠티어
                }
                if (g_strSndSubCmd == "HITSINK_PV")
                {
                    PeltMon.dbTempCooler = double.Parse(g_strRcvParam);          // 방열판
                }
            }
            else if (rcvstr.Contains("ecc") && g_strSndSubCmd == "CLEAR")     // write/clear eccentric proximity sensor count value
            {
                CmdResult.EccentricClear = COM_Status.ACK;
            }
            else if (rcvstr.Contains("ecc") && g_strSndSubCmd == "GET")     // read eccentric proximity sensor counted value
            {
                CmdResult.ReadEccentric = COM_Status.ACK;
                label_eccectric.Text = g_strRcvParam;
                nEccentricCnt = int.Parse(g_strRcvParam);
            }
            else if (rcvstr.Contains("POWER OFF?"))
            {
                SystemOff();
                SwitchMon.bPower = false;
                SensorStatus.PowerSwitch = Status.OFF;
            }
            else if (rcvstr.Contains("etc") && g_strSndSubCmd == "FAN")
            {
                CmdResult.CoolingFan = COM_Status.ACK;
            }
            else if (rcvstr.Contains("light"))              // top light on/off
            {
                if (g_strSndSubCmd == "ROOM_PWR")
                    CmdResult.TopLight = COM_Status.ACK;
                else if (g_strSndSubCmd == "CHAMBER_PWR")
                    CmdResult.RotorLight = COM_Status.ACK;
                else if (g_strSndSubCmd == "CHAMBER_SV" || g_strSndSubCmd == "ROOM_SV")
                    CmdResult.LightCond = COM_Status.ACK;
            }
            else if (rcvstr.Contains("pump") && g_strSndSubCmd.Contains("PE1"))  // run peripheral1 tricontinental pipett
            {
                CmdResult.Perpheral1_TriPipett = COM_Status.ACK;
                CmdResult.ResetPeripheral = COM_Status.ACK;

                if (g_strRcvParam.Length >= 3)
                {
                    if (g_strRcvParam.Substring(2, 1) != "`" && g_strRcvParam.Substring(2, 1) != "@")
                    {
                        if (g_strRcvParam.Contains("a") || g_strRcvParam.Contains("A"))
                            label_TriPipettState.Text = "Initialize Err";
                        if (g_strRcvParam.Contains("b") || g_strRcvParam.Contains("B"))
                            label_TriPipettState.Text = "Invalid Cmd";
                        if (g_strRcvParam.Contains("c") || g_strRcvParam.Contains("C"))
                            label_TriPipettState.Text = "Invalid Operand";
                        if (g_strRcvParam.Contains("g") || g_strRcvParam.Contains("G"))
                            label_TriPipettState.Text = "Not Initialized";
                        if (g_strRcvParam.Contains("o") || g_strRcvParam.Contains("O"))
                            label_TriPipettState.Text = "Cmd Overflow";

                        SensorStatus.AlarmPeri1_tri_pipett = Status.ON;
                        Sensor_TriPipettEnable.Enabled = false;
                    }
                    else
                    {
                        if (g_strSndParam.Contains("?") && g_strRcvParam.Substring(2, 1) != "@")
                        {
                            int NoErrCharIdx = g_strRcvParam.IndexOf("`") + 1;
                            PE1PlungerPosInc = int.Parse(g_strRcvParam.Substring(NoErrCharIdx, 
                                                        (g_strRcvParam.Length - NoErrCharIdx)));
                            label_PE1_PlungerCurPos.Text = PE1PlungerPosInc.ToString();
                            label_TriPipettState.Text = "";
                        }
                        else if (g_strSndParam.Contains("?") && g_strRcvParam.Substring(2, 1) == "@")
                        {
                            label_TriPipettState.Text = "Device Busy";
                        }

                        SensorStatus.AlarmPeri1_tri_pipett = Status.OFF;
                        Sensor_TriPipettEnable.Enabled = true;
                    }

                    if (g_strRcvParam.Substring(2, 1) == "@")
                    {
                        label_TriPipettState.Text = "Device Busy";
                    }
                }
            }
            else if (rcvstr.Contains("level") && g_strSndSubCmd.Contains("PE2_LEVEL"))
            {
                CmdResult.Perpheral2_HamPipett = COM_Status.ACK;

                if (String.Compare(g_strSndSubCmd, "PE2_LEVEL") == 0)   // 1: not detected, 0: detected
                {
                    if (int.Parse(g_strRcvParam) == 1)
                        bcLLD_IO = false;
                    else if (int.Parse(g_strRcvParam) == 0)
                        bcLLD_IO = true;
                }
            }
            else if (rcvstr.Contains("pump") && g_strSndSubCmd.Contains("PE2"))  // run peripheral2 hamilton pipett
            {
                CmdResult.Perpheral2_HamPipett = COM_Status.ACK;
                CmdResult.ResetPeripheral = COM_Status.ACK;

                if (g_strSndParam.Contains("RT"))   //00RTid####rt#, rt# is the tip presence status (0 = no tip, 1 = tip present)
                {
                    nTipPresence = int.Parse(g_strRcvParam.Substring(g_strRcvParam.IndexOf("rt") + 2, 1));
                }
                if (g_strSndParam.Contains("RN"))   //00RNrn#, 0=idle, 1=searching, 2= LLD detected
                {
                    nState_cLLD = int.Parse(g_strRcvParam.Substring(g_strRcvParam.IndexOf("rn") + 2, 1));
                }
                if (g_strRcvParam.Contains("CL"))   //검출되기 까지는 CL에 대한 회신이 오지 않음
                {
                    bcLLD_Detected = true;
                }

                if (g_strRcvParam.Contains("er"))
                {
                    SensorStatus.ham_pipett_errNo = int.Parse(g_strRcvParam.Substring(g_strRcvParam.IndexOf("er") + 2, 2));
                    if (SensorStatus.ham_pipett_errNo != 0)
                    {
                        SensorStatus.AlarmPeri2_ham_pipett = Status.ON;
                        Sensor_HamPipettEnable.Enabled = false;
                        iPrintf("Hamilton Pipett Error (No: " + SensorStatus.ham_pipett_errNo + ")");
                    }
                    else
                    {
                        SensorStatus.AlarmPeri2_ham_pipett = Status.OFF;
                        Sensor_HamPipettEnable.Enabled = true;
                    }
                }
                else
                {
                    SensorStatus.AlarmPeri2_ham_pipett = Status.OFF;
                    Sensor_HamPipettEnable.Enabled = true;
                }
            }
            else if (rcvstr.Contains("pump") && g_strSndSubCmd.Contains("PE3"))  // run peripheral3 tricontinental pump
            {
                CmdResult.Perpheral3_TriPump = COM_Status.ACK;
                CmdResult.ResetPeripheral = COM_Status.ACK;

                if (g_strSndParam.Contains("?"))
                {
                    label_PE3_PlungerPos.Text = g_strRcvParam;
                }

                if (g_strRcvParam.Contains("a") || g_strRcvParam.Contains("A") ||
                    g_strRcvParam.Contains("b") || g_strRcvParam.Contains("B") ||
                    g_strRcvParam.Contains("c") || g_strRcvParam.Contains("C") ||
                    g_strRcvParam.Contains("d") || g_strRcvParam.Contains("D") ||
                    g_strRcvParam.Contains("f") || g_strRcvParam.Contains("F") ||
                    g_strRcvParam.Contains("g") || g_strRcvParam.Contains("G") ||
                    g_strRcvParam.Contains("j") || g_strRcvParam.Contains("J") ||
                    g_strRcvParam.Contains("k") || g_strRcvParam.Contains("K") ||
                    g_strRcvParam.Contains("o") || g_strRcvParam.Contains("O"))
                {
                    SensorStatus.AlarmPeri3_tri_pump = Status.ON;
                    Sensor_TriPumpEnable.Enabled = false;
                }
                else
                {
                    SensorStatus.AlarmPeri3_tri_pump = Status.OFF;
                    Sensor_TriPumpEnable.Enabled = true;
                }
            }
            else if (rcvstr.Contains("weight") && g_strSndCmd.Contains("LOADCELL"))  // read peripheral4 loadcell value
            {
                CmdResult.LoadCell = COM_Status.ACK;
                LoadcellVal.fWeight = float.Parse(g_strRcvParam);
                label_LoadCellWeightVal.Text = g_strRcvParam;
            }
            else if (rcvstr.Contains("cal") && g_strSndCmd.Contains("LOADCELL"))
            {
                CmdResult.LoadCell = COM_Status.ACK;
                LoadcellVal.fCalVal = float.Parse(g_strRcvParam);
            }
            else if (rcvstr.Contains("flow") && g_strSndSubCmd == "GET")  // read flowmeter value
            {
                CmdResult.Flowmeter = COM_Status.ACK;
                label_FlowPulseCntVal.Text = g_strRcvParam;
            }
            else if (rcvstr.Contains("laser") && g_strSndSubCmd == "GET") // read laser sensor
            {
                CmdResult.LaserSensor = COM_Status.ACK;
                nLaserDetected = int.Parse(g_strRcvParam);
            }
            else if (string.Compare(rcvstr, "disk_pos") == 0)  // read servo position
            {
                CmdResult.ServoState = COM_Status.ACK;
                ServoMon.nCurrServoPosition = (((int.Parse(g_strRcvParam) - 
                                                 int.Parse(config.ServoPos_Chamber1)) % 4194304) * 360) / 4194304; // Unit: degree
            }
            else if (string.Compare(rcvstr, "disk_rpm") == 0)  // read servo rpm(speed)
            {
                CmdResult.ServoState = COM_Status.ACK;
                ServoMon.nCurrServoRpm = int.Parse(g_strRcvParam);
                ServoMon.dbCurrServoRcf = 11.18 * double.Parse(edit_r.Text) * 
                                          Math.Pow((double)ServoMon.nCurrServoRpm, 2) * 10e-5;
            }
            else if (string.Compare(rcvstr, "disk_status") == 0)  // read servo alarm
            {
                CmdResult.ServoState = COM_Status.ACK;
                ParseServoStatus(g_strRcvParam);
            }
            else if (string.Compare(g_strSndCmd, "DISK") == 0 && string.Compare(rcvstr, "alarm") == 0)  // read servo error No.
            {
                CmdResult.ServoState = COM_Status.ACK;
                ServoMon.nServoErrCode = int.Parse(g_strRcvParam);
                DisplayStatusMessage("ServoError:" + g_strRcvParam, TEST.FAIL);
            }

            lastRcvStr = rcvstr;
            Application.DoEvents();
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        void WriteDataReceiveParsing()
        {
            int i = 0;
            string str;

            byte[] LeftRpm = new byte[5];
            byte[] RightRpm = new byte[5];

            for (i = 0; i < 4; i++) LeftRpm[i] = bSerialRcvDataFrame[i + 10];
            str = ByteToString(LeftRpm, 4);

            for (i = 0; i < 4; i++) RightRpm[i] = bSerialRcvDataFrame[i + 14];
            str = ByteToString(RightRpm, 4);
        }
        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        void ConfigReceiveParsing()
        {
            if (bSerialRcvDataFrame[6] == '0')
            {
                bAutoManual = AUTO_MODE;
            }
            else
            {
                bAutoManual = MANUAL_MODE;
            }
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        void StatusReceiveParsing()
        {
            switch ((char)bSerialRcvDataFrame[3])
            {
                case '0':
                    break;
            }
        }


        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        private void CommandResponseParsing()
        {
            switch ((char)bSerialRcvDataFrame[3])
            {
                case '0':
                    break;
            }
        }
        private void PresetSerialRequestBuffer()
        {
            //--------------------------------------------------------
            //	preset Status Request Buffer
            //--------------------------------------------------------

            //BuildCmdPacket(stateRequestBuf, "RDSTAT", "", "");
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public delegate void ClearDataDelegate();
        private void ClearData()
        {

        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            if (!Serial.IsOpen)
            {
                iPrintf("Serial Port is not Opened");
                return;
            }

            //if (btnTimer.Text == "Stop Timer")
            if (bSerialTimerState == true)
            {
                //btnTimer.Text = "Start Timer";
                bSerialTimerState = false;
                timer_com.Stop();
                bSerialTimerState = false;

                if (bServoRunState == true)
                    bServoRunState = false;

                iPrintf(string.Format("Stop Serial Tick Timer! {0}", SpinTotalTime));   // for test
            }
            else
            {
                //btnTimer.Text = "Stop Timer";
                bSerialTimerState = true;
                if (int.Parse(tbTimerInterval.Text) > 0)
                {
                    timer_com.Interval = int.Parse(tbTimerInterval.Text);
                    timer_com.Stop();
                    timer_com.Start();
                    SerialTimerCnt = 0;
                    bSerialTimerState = true;
                    iPrintf(string.Format("Start Serial Tick Timer! {0}", SpinTotalTime));  // for test
                }
            }
        }

        // description: Read Status Sensor & Etc
        private COM_Status GetStatus(bool waitReceive = false, int timeout = 20, bool bSilent = false)  // RDSTAT
        {
            bool bTemp = bDirectReceive;
            if (waitReceive)
                bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.GetSTATUS = COM_Status.RESET;
            try
            {
                BuildCmdPacket(bCommandSendBuffer, "ALL", "STATUS", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength, bSilent);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout && waitReceive; i++)
            {
                Thread.Sleep(1);
                ReceiveDirect();
                if (CmdResult.GetSTATUS != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.GetSTATUS;
            CmdResult.GetSTATUS = COM_Status.RESET;

            bDirectReceive = bTemp;

            return retVal;
        }

        // description: Write motor setting RPM & Camera Prescale
        // Centrifuge 동작 관련 파라미터의 변수만 저장함 (구 프로토콜: WRMSET)
        private COM_Status SerCmd_SetParameter(Direction dir, int rpm, int prescale, int spinUpTime, int spinDownTime)
        {
            SpinDir = (int)dir;
            SpinUpTime = spinUpTime;
            SpinDownTime = spinDownTime;
            SpinRpm = rpm + int.Parse(edit_rpm_offset.Text);
            Prescale = prescale;

            SpinTotalTime = SpinUpTime + SpinDownTime + SpinDuration;

            iPrintf(string.Format("Set SpinParam Cmd rpm: {0}, Offset rpm: {1}", SpinRpm, int.Parse(edit_rpm_offset.Text)));

            return COM_Status.ACK;
        }

        private void btn_get_parameter_Click(object sender, EventArgs e)
        {
            COM_Status retVal = GetTestCondition();
            if (retVal == COM_Status.NAK)
                iPrintf("Serial GetParameter NAK");
            else if (retVal == COM_Status.RESET)
                iPrintf("Serial GetParameter TIMEOUT");
        }

        private void FillCommandSendBuffer(byte fill_char)
        {
            for (int i = 0; i < FRAME_LENGTH; i++)
            {
                bCommandSendBuffer[i] = fill_char;
            }
        }

        // description: Read servo setting rpm & camera prescale
        private COM_Status GetTestCondition()
        {
            try
            {
                edit_rpm.Text = SpinRpm.ToString();
                edit_prescale.Text = Prescale.ToString();
                edit_servo_acc.Text = SpinUpTime.ToString();
                edit_servo_dec.Text = SpinDownTime.ToString();
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            return COM_Status.ACK;
        }

        private void btnComConnect_Click(object sender, EventArgs e)
        {
            if (OpenComPort(editCommPorts.Text) == true)
            {
                GetStatus(true);
                Thread.Sleep(100);
                SystemCmd("SYSTEM", "ERRORS", "");

                if (ServoState.bALM == true)
                    ServoMonitor(MotorMon.ALARM);

                SwitchControl(SwitchState.STATUS, Status.NONE);
                ReadPeltierTemp();
                btnComConnect.Text = "Close";   // thread로 인해 재설정되므로 Status까지 읽고 나서 설정
            }
        }

        private void btn_version_Click(object sender, EventArgs e)
        {
            holdTimer = true;

            if (GetFWVersion(0) != COM_Status.ACK)
            {
                iPrintf("Serial MainBoard GetVersion TIMEOUT");
            }

            if (GetFWVersion(1) != COM_Status.ACK)
            {
                iPrintf("Serial PowerBoard GetVersion TIMEOUT");
            }

            holdTimer = false;
        }

        // description: Read Firmware Version
        // argument -> opt 0: Main, 1: Power
        private COM_Status GetFWVersion(int opt, int timeout = 50)
        {
            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.GetVersion = COM_Status.RESET;
            try
            {
                if (opt == 0)
                {
                    BuildCmdPacket(bCommandSendBuffer, "VERSION", "MAIN", "");
                }
                else if (opt == 1)
                {
                    BuildCmdPacket(bCommandSendBuffer, "VERSION", "POWER", "");
                }

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.GetVersion != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.GetVersion;
            CmdResult.GetVersion = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Run/Stop centrifuge motor & camera
        // argument -> CMD: RUN / STOP, duration
        private COM_Status SerCmd_Spin(CMD run, int duration, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            //if (SensorStatus.servo != Status.ON || SensorStatus.camera != Status.ON || SensorStatus.door != Status.ON || SensorStatus.power != Status.ON)
            //    return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Spin = COM_Status.RESET;
            holdTimer = true;

            SpinDuration = duration;

            if (run == CMD.RUN)
            {
                string strParam = string.Format("{0},{1},{2},{3},{4}", Convert.ToChar(SpinDir),
                                                SpinUpTime, SpinDuration, SpinDownTime, SpinRpm);

                BuildCmdPacket(bCommandSendBuffer, "DISK", "TURN", strParam);
                bServoRunState = true;
                SerialTimerCnt = 0; // for wait count
            }
            else if (run == CMD.STOP)
            {
                string strParam = "";
                BuildCmdPacket(bCommandSendBuffer, "DISK", "STOP_TIME", strParam);
                bServoRunState = false;
            }
            else if (run == CMD.ESTOP)
            {
                string strParam = "";
                BuildCmdPacket(bCommandSendBuffer, "DISK", "STOP", strParam);
                bServoRunState = false;
            }

            SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port

            holdTimer = false;
            for (int i = 0; i < timeout; i++)
            {
                Thread.Sleep(1);
                ReceiveDirect();
                if (CmdResult.Spin != COM_Status.RESET)
                    break;
            }

            retVal = CmdResult.Spin;
            CmdResult.Spin = COM_Status.RESET;
            bDirectReceive = false;

            SpinTotalTime = SpinUpTime + SpinDownTime + SpinDuration;

            return retVal;
        }

        // description: servo, step motor reset
        // argument -> MOTOR: SERVO = 0, GRIP = 1, HAM = 2, COVER = 3, AXIS_X = 4, AXIS_Y = 5, AXIS_Z = 6
        private COM_Status ResetMotor(MOTOR motor, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ResetMotor = COM_Status.RESET;
            try
            {
                string strCmd = "";
                string strSubCmd = "";

                if (motor == MOTOR.SERVO)
                {
                    strCmd = "DISK";
                }
                else if (motor == MOTOR.STEP0)
                {
                    strCmd = "STEP 0";
                }
                else if (motor == MOTOR.STEP1)
                {
                    strCmd = "STEP 1";
                }
                else if (motor == MOTOR.STEP2)
                {
                    strCmd = "STEP 2";
                }
                else
                {
                    iPrintf("Invalid motor Number input!!");
                    return COM_Status.NAK;
                }

                strSubCmd = "ALM_RST";

                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            Thread.Sleep(100);
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ResetMotor != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ResetMotor;
            CmdResult.ResetMotor = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: step motor homing/home parameter setting
        // argument -> MOTOR: SERVO = 0, GRIP = 1, HAM = 2, COVER = 3, AXIS_X = 4, AXIS_Y = 5, AXIS_Z = 6
        private COM_Status StepMotorHomeMove(MOTOR motor, string strSubCmd, string strParam, int timeout = 10)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Home = COM_Status.RESET;
            try
            {
                string strCmd = "";

                if (motor == MOTOR.GRIP)
                {
                    strCmd = string.Format("GRIP");
                }
                else if (motor == MOTOR.HAM)
                {
                    strCmd = string.Format("HAM");
                }
                else if (motor == MOTOR.COVER)
                {
                    strCmd = string.Format("COVER");
                }
                else if (motor == MOTOR.STEP0)
                {
                    strCmd = string.Format("STEP 0");
                }
                else if (motor == MOTOR.STEP1)
                {
                    strCmd = string.Format("STEP 1");
                }
                else if (motor == MOTOR.STEP2)
                {
                    strCmd = string.Format("STEP 2");
                }
                else
                {
                    iPrintf("Invalid motor Number input!!");
                    return COM_Status.NAK;
                }

                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            Thread.Sleep(100);
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Home != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Home;
            CmdResult.Home = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        private COM_Status SystemCmd(string strCmd = "", string strSubCmd = "", string strParam = "", int timeout = 25)   // timeout: 25
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.SystemCmd = COM_Status.RESET;
            try
            {
                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            Thread.Sleep(100);
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.SystemCmd != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.SystemCmd;
            CmdResult.SystemCmd = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: peripheral device reset
        // argument -> PERIPHERAL: RI_PIPETT = 1, HAM_PIPETT = 2, TRI_PUMP = 3, LOAD_CELL = 4,
        private COM_Status InitPeripherals(PERIPHERAL peripheral, string strParam = "", int timeout = 25)   // timeout: 25
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ResetPeripheral = COM_Status.RESET;
            try
            {
                string strCmd = "PUMP";
                string strSubCmd = "";

                if (peripheral == PERIPHERAL.TRI_PIPETT)
                {
                    strSubCmd = "PE1";
                    //strParam = "/2zA0R";
                    //strParam = string.Format("/21600zA0A10z0R{0}", Environment.NewLine);
                }
                else if (peripheral == PERIPHERAL.HAM_PIPETT)
                {
                    strSubCmd = "PE2";
                    strParam = string.Format("00DIid0000");
                }
                else if (peripheral == PERIPHERAL.TRI_PUMP)
                {
                    strSubCmd = "PE3";
                    //strParam = "/3ZR";
                    strParam = string.Format("/3ZR{0}", Environment.NewLine);
                }
                else
                {
                    iPrintf("Invalid peripheral Number input!!");
                    return COM_Status.NAK;
                }

                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            Thread.Sleep(100);
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ResetPeripheral != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ResetPeripheral;
            CmdResult.ResetPeripheral = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description :step motor 1 ~ 6 position move
        // argumnet -> 
        // STEP_CMD: MOVE = 0, STOP = 1, HOLD = 2, HOME = 3, ALM_RST = 4, POS = 5, STATUS = 6, NONE = 7
        // MOTOR: SERVO = 0, GRIP = 1, HAM = 2, COVER = 3, AXIS_X = 4, AXIS_Y = 5, AXIS_Z = 6
        // POS_OPT: ABS = 0, REL = 1, NONE = 2
        // HOLD_STATE: FREE = 0, HOLD = 1, NONE = 2
        private COM_Status MoveStepMotor(STEP_CMD cmd, MOTOR motorNum, double speed, double position, int acc, int dec,
                                        POS_OPT opt1, HOLD_STATE opt2, int timeout = 10)   // MOVST?
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            COM_Status retVal = COM_Status.RESET;

            try
            {
                if (bStepMotorInitDoneState == false)
                {
                    iPrintf("Step Motor Not Initialized!");
                    return COM_Status.NAK;
                }

                string strCmd = "";
                string strSubCmd = "";
                string strParam = "";

                if (motorNum == MOTOR.STEP0)
                {
                    CmdResult.StepMotor4AxisX = COM_Status.RESET;
                    strCmd = string.Format("STEP 0");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.Step0AxisX);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        // 이동 거리가 소량일 때는 통신 충돌 방지를 위해 타이머를 통한 위치 모니터링을 방지함
                        if (dbMovingDist > 1)
                            bAxisMovingFlag[0] = true;

                        bAxisStartFlag[0] = true;
                        bStepRunState = true;
                    }
                }
                else if (motorNum == MOTOR.STEP1)
                {
                    CmdResult.StepMotor5AxisY = COM_Status.RESET;
                    strCmd = string.Format("STEP 1");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.Step1AxisY);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        if (dbMovingDist > 1)
                            bAxisMovingFlag[1] = true;

                        bAxisStartFlag[1] = true;
                        bStepRunState = true;
                    }
                }
                else if (motorNum == MOTOR.STEP2)
                {
                    CmdResult.StepMotor6AxisZ = COM_Status.RESET;
                    strCmd = string.Format("STEP 2");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.Step2AxisZ);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        if (dbMovingDist > 1)
                            bAxisMovingFlag[2] = true;

                        bAxisStartFlag[2] = true;
                        bStepRunState = true;
                    }
                }
                else if (motorNum == MOTOR.GRIP)
                {
                    CmdResult.StepMotor1Gripper = COM_Status.RESET;
                    strCmd = string.Format("GRIP");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.StepGripAxis);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        if (dbMovingDist > 1)
                            bAxisMovingFlag[3] = true;

                        bAxisStartFlag[3] = true;
                        bStepRunState = true;
                    }
                }
                else if (motorNum == MOTOR.HAM)
                {
                    CmdResult.StepMotor2Pipett = COM_Status.RESET;
                    strCmd = string.Format("HAM");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.StepHamAxis);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        if (dbMovingDist > 1)
                            bAxisMovingFlag[4] = true;

                        bAxisStartFlag[4] = true;
                        bStepRunState = true;
                    }
                }
                else if (motorNum == MOTOR.COVER)
                {
                    CmdResult.StepMotor3RotorCover = COM_Status.RESET;
                    strCmd = string.Format("COVER");
                    if (cmd == STEP_CMD.MOVE)
                    {
                        if (opt1 == POS_OPT.ABS)
                            dbMovingDist = Math.Abs(position - CurrentPos.StepRotCover);
                        else if (opt1 == POS_OPT.REL)
                            dbMovingDist = Math.Abs(position);

                        if (dbMovingDist > 1)
                            bAxisMovingFlag[5] = true;

                        bAxisStartFlag[5] = true;
                        bStepRunState = true;
                    }
                }
                else
                {
                    iPrintf("Invalid motor Number input!!");
                    return COM_Status.NAK;
                }

                if (cmd == STEP_CMD.MOVE && opt1 == POS_OPT.ABS)
                {
                    strSubCmd = string.Format("MOVA");
                    strParam = string.Format("{0},{1},{2},{3}", acc, speed, dec, position);
                }
                else if (cmd == STEP_CMD.MOVE && opt1 == POS_OPT.REL)
                {
                    strSubCmd = string.Format("MOVR");
                    strParam = string.Format("{0},{1},{2},{3}", acc, speed, dec, position);
                }
                else if (cmd == STEP_CMD.HOME)
                {
                    strSubCmd = string.Format("HOME");
                }
                else if (cmd == STEP_CMD.ALM_RST)
                {
                    strSubCmd = string.Format("ALM_RST");
                }
                else if (cmd == STEP_CMD.SPD_SCALE)
                {
                    strSubCmd = string.Format("SPD_SCALE");
                    strParam = string.Format("{0}", speed);     // 배수
                }
                else if (cmd == STEP_CMD.STOP)
                {
                    strSubCmd = string.Format("STOP");
                }
                else if (cmd == STEP_CMD.HOLD)
                {
                    strSubCmd = string.Format("HOLD");
                    strParam = string.Format("{0}", (int)opt2);
                }
                else if (cmd == STEP_CMD.POS)
                {
                    strSubCmd = string.Format("POS");
                }
                else if (cmd == STEP_CMD.STATUS)
                {
                    strSubCmd = string.Format("STATUS");
                }

                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port

                for (int i = 0; i < timeout; i++)
                {
                    ReceiveDirect();
                    Thread.Sleep(1);
                }

                if (motorNum == MOTOR.STEP0)
                {
                    retVal = CmdResult.StepMotor4AxisX;
                    CmdResult.StepMotor4AxisX = COM_Status.RESET;
                }
                else if (motorNum == MOTOR.STEP1)
                {
                    retVal = CmdResult.StepMotor5AxisY;
                    CmdResult.StepMotor5AxisY = COM_Status.RESET;
                }
                else if (motorNum == MOTOR.STEP2)
                {
                    retVal = CmdResult.StepMotor6AxisZ;
                    CmdResult.StepMotor6AxisZ = COM_Status.RESET;
                }
                else if (motorNum == MOTOR.GRIP)
                {
                    retVal = CmdResult.StepMotor1Gripper;
                    CmdResult.StepMotor1Gripper = COM_Status.RESET;
                }
                else if (motorNum == MOTOR.HAM)
                {
                    retVal = CmdResult.StepMotor2Pipett;
                    CmdResult.StepMotor2Pipett = COM_Status.RESET;
                }
                else if (motorNum == MOTOR.COVER)
                {
                    retVal = CmdResult.StepMotor3RotorCover;
                    CmdResult.StepMotor3RotorCover = COM_Status.RESET;
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                retVal = COM_Status.NAK;
            }
            bDirectReceive = false;
            return retVal;
        }

        // description : Z direction axes fase move (Z, Grip, Pipett Axis)
        // argumnet -> WAIT_OPT: NOWAIT = 0, WAIT = 1
        private COM_Status FastZ_UpMotion(WAIT_OPT wait)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;
            int cnt = 0;

            int grip_acc = int.Parse(editStepAxisGripper_Acc.Text);
            int grip_dec = int.Parse(editStepAxisGripper_Dec.Text);
            int grip_spd = int.Parse(editFastMoveSpd_GripAxis.Text);
            double grip_pos = double.Parse(editFastMovePos_GripAxis.Text);

            MoveStepMotor(STEP_CMD.MOVE, MOTOR.GRIP, grip_spd, grip_pos, grip_acc, grip_dec, POS_OPT.ABS, HOLD_STATE.NONE);
            Thread.Sleep(50);

            int ham_acc = int.Parse(editStepAxisHam_Acc.Text);
            int ham_dec = int.Parse(editStepAxisHam_Dec.Text);
            int ham_spd = int.Parse(editFastMoveSpd_HamAxis.Text);
            double ham_pos = double.Parse(editFastMovePos_HamAxis.Text);

            MoveStepMotor(STEP_CMD.MOVE, MOTOR.HAM, ham_spd, ham_pos, ham_acc, ham_dec, POS_OPT.ABS, HOLD_STATE.NONE);
            Thread.Sleep(50);

            int z_acc = int.Parse(editStepAxisZ_Acc.Text);
            int z_dec = int.Parse(editStepAxisZ_Dec.Text);
            int z_spd = int.Parse(editFastMoveSpd_ZAxis.Text);
            double z_pos = double.Parse(editFastMovePos_ZAxis.Text);

            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, z_spd, z_pos, z_acc, z_dec, POS_OPT.ABS, HOLD_STATE.NONE);
            Thread.Sleep(100);

            if (wait == WAIT_OPT.WAIT)
            {
                do
                {
                    if (cnt >= 20) break;
                    bMotionDoneWait = true;

                    GetStatus(true, bSilent: true);
                    Thread.Sleep(150);  //300

                    iPrintf(string.Format("Z Moving: {0} ({1})", Step2AxState.bMOVE, cnt));
                    cnt++;

                    if (SensorStatus.Alarm)
                    {
                        if (isRunningSingle == true)
                        {
                            isRunningSingle = false;
                        }
                        DisplayStatusMessage("System Alarm !!!", TEST.FAIL);
                        break;
                    }

                    if (!Serial.IsOpen || bStopFlag == true)
                    {
                        if (isRunningSingle == true)
                        {
                            isRunningSingle = false;
                        }
                        if (Serial.IsOpen == false)
                        {
                            DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                        }
                        if (bStopFlag == true)
                        {
                            DisplayStatusMessage("Recipe Stopped !!!", TEST.FAIL);
                        }
                        break;
                    }
                } while (Step2AxState.bMOVE == true || GripAxState.bMove == true);
                //} while (Step2AxState.bMOVE == true);
                //} while (bAxisStartFlag[2] == true) ;

                GetStatus(true, bSilent: true);
                Thread.Sleep(100);
                bMotionDoneWait = false;
                iPrintf(string.Format("[Exit] Z Moving: {0}", Step2AxState.bMOVE));

                if (bAxisMovingFlag[2] == true) bAxisMovingFlag[2] = false;
            }
            else
            {
                ;
            }

            //ReadMotorPosition(true, bSilent: true);

            return COM_Status.ACK;
        }

        // description : XY Axis Coordinated simultaneous move
        // argumnet -> WAIT_OPT: NOWAIT = 0, WAIT = 1
        private COM_Status MoveStepXYCrdMotion(int spdX, double posX, int spdY, double posY, WAIT_OPT wait)
        {
            if (!Serial.IsOpen || bStopFlag == true)
                return COM_Status.NAK;

            int cnt = 0;

            int x_acc = int.Parse(editStepAxisX_Acc.Text);
            int x_dec = int.Parse(editStepAxisX_Dec.Text);

            int y_acc = int.Parse(editStepAxisY_Acc.Text);
            int y_dec = int.Parse(editStepAxisY_Dec.Text);

            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP0, spdX, posX, x_acc, x_dec, POS_OPT.ABS, HOLD_STATE.NONE);
            Thread.Sleep(50);
            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP1, spdY, posY, y_acc, y_dec, POS_OPT.ABS, HOLD_STATE.NONE);
            Thread.Sleep(100);

            if (wait == WAIT_OPT.WAIT)
            {
                do
                {
                    if (cnt >= 50) break;
                    bMotionDoneWait = true;

                    GetStatus(true, bSilent: true);
                    Thread.Sleep(200);  //300

                    iPrintf(string.Format("X Moving: {0} , Y Moving: {1} ({2})", Step0AxState.bMOVE, Step1AxState.bMOVE, cnt));
                    cnt++;

                    if (SensorStatus.Alarm)
                    {
                        if (isRunningSingle == true)
                        {
                            isRunningSingle = false;
                        }
                        DisplayStatusMessage("System Alarm !!!", TEST.FAIL);
                        break;
                    }

                    if (!Serial.IsOpen || bStopFlag == true)
                    {
                        if (isRunningSingle == true)
                        {
                            isRunningSingle = false;
                        }
                        if (Serial.IsOpen == false)
                        {
                            DisplayStatusMessage("Serial Not Opened !!!", TEST.FAIL);
                        }
                        if (bStopFlag == true)
                        {
                            DisplayStatusMessage("Recipe Stopped !!!", TEST.FAIL);
                        }
                        break;
                    }
                } while (Step0AxState.bMOVE == true || Step1AxState.bMOVE == true);

                //GetStatus(true, bSilent: true);
                //Thread.Sleep(100);
                bMotionDoneWait = false;
                iPrintf(string.Format("[Exit] X Moving: {0}/{1} , Y Moving: {2}/{3}",
                                      Step0AxState.bMOVE, bAxisMovingFlag[0], Step1AxState.bMOVE, bAxisMovingFlag[1]));
                if (bAxisMovingFlag[0] == true) bAxisMovingFlag[0] = false;
                if (bAxisMovingFlag[1] == true) bAxisMovingFlag[1] = false;
            }
            else
            {
                ;
            }

            //ReadMotorPosition(true, bSilent: true);

            return COM_Status.ACK;
        }

        // description: read all motor position
        private COM_Status ReadMotorPosition(bool waitReceive = false, int timeout = 20, bool bSilent = false)
        {
            bool bTemp = bDirectReceive;
            if (waitReceive)
                bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ReadMotorPos = COM_Status.RESET;

            try
            {
                BuildCmdPacket(bCommandSendBuffer, "ALL", "POSITION", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength, bSilent);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout && waitReceive; i++)
            {
                Thread.Sleep(1);
                ReceiveDirect();
                if (CmdResult.ReadMotorPos != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ReadMotorPos;
            CmdResult.ReadMotorPos = COM_Status.RESET;

            bDirectReceive = bTemp;

            return retVal;
        }

        private void UpdateRecipeTempData()
        {
            if (label_Recipe_PeltSetTemp.InvokeRequired == true)
            {
                this.label_Recipe_PeltSetTemp.Invoke((MethodInvoker)delegate
                {
                    label_Recipe_PeltSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
                });
            }
            else
            {
                label_Recipe_PeltSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
            }

            if (label_Recipe_PeltChamberTemp.InvokeRequired == true)
            {
                this.label_Recipe_PeltChamberTemp.Invoke((MethodInvoker)delegate
                {
                    label_Recipe_PeltChamberTemp.Text = PeltMon.dbTempChamber.ToString();
                });
            }
            else
            {
                label_Recipe_PeltChamberTemp.Text = PeltMon.dbTempChamber.ToString();
            }

            if (label_Recipe_PeltPeltierTemp.InvokeRequired == true)
            {
                this.label_Recipe_PeltPeltierTemp.Invoke((MethodInvoker)delegate
                {
                    label_Recipe_PeltPeltierTemp.Text = PeltMon.dbTempPeltier.ToString();
                });
            }
            else
            {
                label_Recipe_PeltPeltierTemp.Text = PeltMon.dbTempPeltier.ToString();
            }

            if (label_Recipe_PeltCoolerTemp.InvokeRequired == true)
            {
                this.label_Recipe_PeltCoolerTemp.Invoke((MethodInvoker)delegate
                {
                    label_Recipe_PeltCoolerTemp.Text = PeltMon.dbTempCooler.ToString();
                });
            }
            else
            {
                label_Recipe_PeltCoolerTemp.Text = PeltMon.dbTempCooler.ToString();
            }
        }

        // description: read peltier temperature (chamber, peltier, hitsnik)
        private COM_Status ReadPeltierTemp(bool bSilent = false)
        {
            COM_Status pelt = ReadPeltier(PELT_CMD.READ_ALL, bSilent: bSilent);

            if (pelt == COM_Status.ACK)
            {
                UpdateRecipeTempData();

                return COM_Status.ACK;
            }
            else
            {
                return COM_Status.NAK;
            }
        }

        // description: PELT40 temperature check
        private COM_Status ReadPeltier(PELT_CMD cmd, int timeout = 20, bool bSilent = false)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ReadPeltier = COM_Status.RESET;

            try
            {
                string strSubCmd = "";

                if (cmd != PELT_CMD.READ_ALL)
                {
                    if (cmd == PELT_CMD.CHAMBER_SV)
                        strSubCmd = "CHAMBER_SV";
                    else if (cmd == PELT_CMD.CHAMBER_PV)
                        strSubCmd = "CHAMBER_PV";
                    else if (cmd == PELT_CMD.PELTIER_PV)
                        strSubCmd = "PELTIER_PV";
                    else if (cmd == PELT_CMD.HITSINK_PV)
                        strSubCmd = "HITSINK_PV";
                    else if (cmd == PELT_CMD.RD_BIAS)
                        strSubCmd = "RD_BIAS";
                    else if (cmd == PELT_CMD.RD_FAN)
                        strSubCmd = "RD_FAN";

                    BuildCmdPacket(bCommandSendBuffer, "TEMP", strSubCmd, "");
                }
                else
                {
                    BuildCmdPacket(bCommandSendBuffer, "ALL", "TEMP", "");
                }


                SerialByteSend(bCommandSendBuffer, nSendBufferLength, bSilent);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ReadPeltier != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ReadPeltier;
            CmdResult.ReadPeltier = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: PELT40 & FAN temperature control stop/start
        // argument ->
        // PELT_CMD: SET_SV = 0, SET_FAN = 1, CHAMBER_SV = 2, CHAMBER_PV = 3, PELTIER_PV = 4, HITSINK_PV = 5,
        // bPeltier: 1 -> on, 0 -> off
        private COM_Status WritePeltier(PELT_CMD cmd, bool bPeltier, float plt_temp,
                                                     float fan_on_temp, float fan_off_temp, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            fPeltOffTemp = 50;
            float fPeltBiasTemp = -5;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.WritePeltier = COM_Status.RESET;

            try
            {
                string strSubCmd = "";
                string strParam = "";

                byte[] bCmd = new byte[1];
                bCmd[0] = (byte)(0 + (bPeltier ? 1 : 0));

                if (cmd == PELT_CMD.SET_SV)
                {
                    bPeltRunState = bPeltier;

                    if (bCmd[0] == 0)
                    {
                        strSubCmd = string.Format("SET_SV");
                        strParam = string.Format("{0}", fPeltOffTemp); // off: 상온보다 높은 온도를 지정하면 off됨
                    }
                    else if (bCmd[0] == 1)
                    {
                        strSubCmd = string.Format("SET_SV");
                        strParam = string.Format("{0}", plt_temp);
                    }
                }
                else if (cmd == PELT_CMD.SET_BIAS)
                {
                    strSubCmd = string.Format("SET_BIAS");
                    strParam = string.Format("{0}", fPeltBiasTemp);
                }
                else if (cmd == PELT_CMD.SET_FAN)
                {
                    strSubCmd = string.Format("SET_FAN");
                    strParam = string.Format("{0},{1}", fan_on_temp, fan_off_temp);
                }

                BuildCmdPacket(bCommandSendBuffer, "TEMP", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.WritePeltier != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.WritePeltier;
            CmdResult.WritePeltier = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: cooling fan control(on/off)
        // argument -> Status: OFF = 0, ON = 1, NONE = 2,
        private COM_Status CoolingFanControl(Status opt, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.CoolingFan = COM_Status.RESET;

            try
            {
                string strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "ETC", "FAN", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.CoolingFan != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.CoolingFan;
            CmdResult.CoolingFan = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: pinch valve open/close
        // argument-> VALVE opt: Close = 0, Open = 1 (Normal Closed Type Valve)
        private COM_Status SerPinchValve(VALVE opt, int timeout = 50) // PVALVE
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.PinchValve = COM_Status.RESET;

            try
            {
                string strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "ETC", "PINCH", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.PinchValve != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.PinchValve;
            CmdResult.PinchValve = COM_Status.RESET;
            bDirectReceive = false;

            Thread.Sleep(int.Parse(editValveDelay.Text));

            return retVal;
        }

        // description :write/clear eccentric proximity sensor count value
        private COM_Status EccentricClear(int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.EccentricClear = COM_Status.RESET;

            try
            {
                BuildCmdPacket(bCommandSendBuffer, "ECC", "CLEAR", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.EccentricClear != COM_Status.RESET)
                    break;
            }

            retVal = CmdResult.EccentricClear;
            CmdResult.EccentricClear = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: read eccentric proximity sensor counted value
        private COM_Status ReadEccentric(int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ReadEccentric = COM_Status.RESET;

            try
            {
                BuildCmdPacket(bCommandSendBuffer, "ECC", "GET", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ReadEccentric != COM_Status.RESET)
                    break;
            }

            retVal = CmdResult.ReadEccentric;
            CmdResult.ReadEccentric = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Power off command(Host -> board(FW))
        private COM_Status SystemOff(int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.SystemOff = COM_Status.RESET;
            try
            {
                BuildCmdPacket(bCommandSendBuffer, "SWITCH", "OFF", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);   // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.SystemOff != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.SystemOff;
            CmdResult.SystemOff = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: set light condition (room & chamber)
        // argument -> Light: Chamber = 0, Room = 1, percent: illuminance
        private COM_Status SetLightCond(Light opt, int percent, int timeout = 10)   // TLIGHT
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.LightCond = COM_Status.RESET;
            try
            {
                string strSubCmd = "";
                string strParam = "";

                if (opt == Light.Room)
                {
                    strSubCmd = "ROOM_SV";
                }
                else if (opt == Light.Chamber)
                {
                    strSubCmd = "CHAMBER_SV";
                }
                strParam = string.Format("{0}", percent);

                BuildCmdPacket(bCommandSendBuffer, "LIGHT", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.LightCond != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.LightCond;
            CmdResult.LightCond = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: top light on/off
        // argument: Status opt: 1 -> on, 0 -> off
        private COM_Status RoomLight(Status opt, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.TopLight = COM_Status.RESET;
            try
            {
                string strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "LIGHT", "ROOM_PWR", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.TopLight != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.TopLight;
            CmdResult.TopLight = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: rotor light on/off
        // argument: Status opt: 1 -> on, 0 -> off
        private COM_Status ChamberLight(Status opt, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.RotorLight = COM_Status.RESET;
            try
            {
                string strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "LIGHT", "CHAMBER_PWR", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.RotorLight != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.RotorLight;
            CmdResult.RotorLight = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: select rotor position 90 deg
        // argument: CHAMBER_POS opt: 0=Main Chamber1, 1=Cell Down1, 2=Main Chamber2, 3=Cell Down12
        private COM_Status SelectRotorPosition(CHAMBER_POS opt, int timeout = 1000)   // SELROT
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.RotorPos = COM_Status.RESET;
            try
            {
                //bAxisMovingFlag = true;
                //bAxisStartFlag = true;

                string strParam = "";

                if (opt == CHAMBER_POS.CHAMBER1)
                {
                    strParam = string.Format("{0},{1},{2}", int.Parse(config.ServoAccDec_MovA),
                                                int.Parse(config.ServoRpm_MovA), int.Parse(config.ServoPos_Chamber1));
                }
                else if (opt == CHAMBER_POS.CELLDOWN1)
                {
                    strParam = string.Format("{0},{1},{2}", int.Parse(config.ServoAccDec_MovA),
                                                int.Parse(config.ServoRpm_MovA), int.Parse(config.ServoPos_CellDown1));
                }
                if (opt == CHAMBER_POS.CHAMBER2)
                {
                    strParam = string.Format("{0},{1},{2}", int.Parse(config.ServoAccDec_MovA),
                                                int.Parse(config.ServoRpm_MovA), int.Parse(config.ServoPos_Chamber2));
                }
                else if (opt == CHAMBER_POS.CELLDOWN2)
                {
                    strParam = string.Format("{0},{1},{2}", int.Parse(config.ServoAccDec_MovA),
                                                int.Parse(config.ServoRpm_MovA), int.Parse(config.ServoPos_CellDown2));
                }

                BuildCmdPacket(bCommandSendBuffer, "DISK", "MOVA", strParam);
                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.RotorPos != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.RotorPos;
            CmdResult.RotorPos = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Servo On/Off
        // argument: HOLD_STATE opt: FREE = 0, HOLD = 1, NONE = 2
        private COM_Status ServoOnOff(HOLD_STATE opt, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ServoOnOff = COM_Status.RESET;
            try
            {
                string strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "DISK", "HOLD", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ServoOnOff != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ServoOnOff;
            CmdResult.ServoOnOff = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Servo State Monitor
        // argument: MotorMon RPM=0, POS=1, STATUS=2, ALARM = 3
        private COM_Status ServoMonitor(MotorMon opt, int timeout = 20, bool bSilent = false)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.ServoState = COM_Status.RESET;
            try
            {
                string strSubCmd = string.Format("{0}", opt);

                BuildCmdPacket(bCommandSendBuffer, "DISK", strSubCmd, "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength, bSilent);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.ServoState != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.ServoState;
            CmdResult.ServoState = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Servo State Monitor
        // argument: MotorMon RPM=0, POS=1, STATUS=2,
        private COM_Status SwitchControl(SwitchState cmd, Status opt, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Switch = COM_Status.RESET;
            try
            {
                string strSubCmd = string.Format("{0}", cmd);
                string strParam = "";

                if (opt != Status.NONE)
                    strParam = string.Format("{0}", (int)opt);

                BuildCmdPacket(bCommandSendBuffer, "SWITCH", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Switch != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Switch;
            CmdResult.Switch = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: run peripheral1(PE1) tricontinental pipett
        // argument ->
        // cmd1: z -> Initialize, A -> Move motor to absolute position(0~1600), ? -> Ask Abs. Current Position
        // cmd2: P -> Move motor relative number of steps in the aspirate direction(0~1600)
        //       D -> Move motor relative number of steps in the dispense direction(0~1600)
        private COM_Status RunPer1_TricontinentPipett(byte cmdChar1, int absPos, int topSpd, byte cmdChar2, int relPos,
                                                      int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            COM_Status retVal = COM_Status.RESET;
            CmdResult.Perpheral1_TriPipett = COM_Status.RESET;

            try
            {
                // Set DT Protocol
                string strParam = "";
                string StartChar = "/";
                string PumpAddr = "2";

                // 경우에 따라 p, d, a 등의 명령을 소문자 혹은 대문자로 적용할지 테스트 필요
                if (cmdChar1 == 'Z')
                {
                    // 마지막 종료 시점과 상관없이 플런저를 최상단으로 보낸 후 zero 값으로 초기화함
                    //strParam = string.Format("z0R{0}", Environment.NewLine);
                    //strParam = string.Format("z1600A0A10z0R{0}", Environment.NewLine);
                    //strParam = string.Format("z1600A0A10R{0}", Environment.NewLine);
                    strParam = string.Format("z1600A0A10R");
                }
                else if (cmdChar1 == 'A')    // 절대 위치 복귀 후 상대이동
                {
                    if (cmdChar2 == 'P')
                    {
                        strParam = string.Format("a{0}V{1}d{2}R", absPos, topSpd, relPos);   // aspirate
                        //strParam = string.Format("a{0}V{1}d{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);   // aspirate
                        //strParam = string.Format("A{0}V{1}P{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);   // aspirate
                    }
                    else if (cmdChar2 == 'D')
                    {
                        strParam = string.Format("a{0}V{1}p{2}R", absPos, topSpd, relPos);   // dispense
                        //strParam = string.Format("a{0}V{1}p{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);   // dispense
                        //strParam = string.Format("A{0}V{1}D{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);   // dispense
                    }
                    else
                    {
                        strParam = string.Format("a{0}R", absPos);   // abs move plunger
                        //strParam = string.Format("a{0}R{1}", absPos, Environment.NewLine);   // abs move plunger
                        //strParam = string.Format("A{0}R{1}", absPos, Environment.NewLine);   // abs move plunger
                    }
                }
                else if (cmdChar1 == ' ')    // 절대 위치 복귀없이 상대이동
                {
                    if (cmdChar2 == 'P')
                    {
                        if (topSpd != 0)
                            strParam = string.Format("V{0}d{1}R", topSpd, relPos);   // aspirate
                                                                                     //strParam = string.Format("V{0}d{1}R{2}", topSpd, relPos, Environment.NewLine);   // aspirate
                                                                                     //strParam = string.Format("V{0}P{1}R{2}", topSpd, relPos, Environment.NewLine);   // aspirate
                        else
                            strParam = string.Format("d{0}R", relPos);   // aspirate
                                                                         //strParam = string.Format("d{0}R{1}", relPos, Environment.NewLine);   // aspirate
                                                                         //strParam = string.Format("P{0}R{1}", relPos, Environment.NewLine);   // aspirate
                    }
                    else if (cmdChar2 == 'D')
                    {
                        if (topSpd != 0)
                            strParam = string.Format("V{0}p{1}R", topSpd, relPos);   // dispense
                                                                                     //strParam = string.Format("V{0}p{1}R{2}", topSpd, relPos, Environment.NewLine);   // dispense
                                                                                     //strParam = string.Format("V{0}D{1}R{2}", topSpd, relPos, Environment.NewLine);   // dispense
                        else
                            strParam = string.Format("p{0}R", relPos);   // dispense
                                                                         //strParam = string.Format("p{0}R{1}", relPos, Environment.NewLine);   // dispense
                                                                         //strParam = string.Format("D{0}R{1}", relPos, Environment.NewLine);   // dispense
                    }
                    else
                    {
                        ;
                    }
                }
                else if (cmdChar1 == 'T')   // 현재 명령 종료
                {
                    strParam = string.Format("TR");
                    //strParam = string.Format("TR{0}", Environment.NewLine);
                }
                else if (cmdChar1 == '?')    // 현재 위치 확인
                {
                    strParam = string.Format("?R");
                    //strParam = string.Format("?R{0}", Environment.NewLine);
                }

                strParam = StartChar + PumpAddr + strParam;

                BuildCmdPacket(bCommandSendBuffer, "PUMP", "PE1", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                retVal = COM_Status.NAK;
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Perpheral1_TriPipett != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Perpheral1_TriPipett;
            CmdResult.Perpheral1_TriPipett = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: run peripheral2(PE2) hamilton pipett
        //////////////////////////////////////////////////////////////////////////////
        // argument(Command Set)
        // 1. Tip 장착 및 흡입/토출 명령
        //  DI: 초기화(플런저 초기화 후 tip 탈거)    00DIid0000                             vol1: NULL, vol2: NULL, flow: NULL, ss:NULL
        //  TP: Tip 삽입 명령어(tip type 지정필요)   00TPid0000tt06(1ml tip)                vol1: NULL, vol2: NULL, flow: NULL, ss:NULL, tip type
        //  AB: 액체 흡입전 공기흡입                 00ABid0000bv03000fr15000               vol1: blow out air vol, vol2: NULL,flow: fr(flow rate),ss:NULL
        //  AL: 액체 흡입                            00ALid0000av07000oa00000fr05000ss00000 vol1: av(aspirate vol), vol2: oa(over asp vol)
        //  DL: 액체 토출                            00DLid0000dv10000sv000fr15000ss00000   vol1: dv(dispense vol), vol2: sv(stop back vol)
        //  AT: 이송 공기 흡입                       00ATid0000tv10000fr15000               vol1: tv(aspirate transport-air vol)
        //  AX: ADC 시작(토출 명령 후 자동으로 꺼짐) 00AXid0000                             vol1: NULL, vol2: NULL, flow: NULL, ss:NULL
        //  MA: Mixing 흡입                          00MAid0000ma05000fr05000               vol1: ma(mixing vol), vol2: NULL
        //  MD: Mixing 토출                          00MDid0000fr05000                      vol1: NULL, vol2: NULL, flow: fr(flow rate), ss:NULL
        //  DE: Tip 초기화                           00DEid0000fr15000ss00000               vol1: NULL, vol2: NULL
        //  TD: Tip 탈거                             00TDid0000                             vol1: NULL, vol2: NULL, flow: NULL, ss:NULL
        // 2. 상태 확인 명령
        //  RT: Tip 장착 여부 확인                   00RTid0000                             vol1: NULL, vol2: NULL, flow: NULL, ss:NULL
        //  RN: cLLD 상태 확인                       00RNid0000                             vol1: NULL, vol2: NULL, flow: NULL, ss:NULL
        //////////////////////////////////////////////////////////////////////////////
        // tip type	(TIP_TYPE)
        // 0: 10 μL CO-RE tip, conductive, non-filtered(tt00)
        // 1: 300 μL CO-RE tip, conductive, non-filtered(tt04)
        // 2: 1000 μL CO-RE tip, conductive, non-filtered(tt06)
        //////////////////////////////////////////////////////////////////////////////

        /// 해밀턴 모듈의 응답 속도가 상대적으로 더 느림, 충분한 응답 시간 확보 필요 timeout 60 이상
        private COM_Status RunPer2_HamiltonPipett(string szCmdChar, int volume1, int volume2, int flowrate,
                                                  int stopSpd, TIP_TYPE tipType, int timeout = 60)   // timeout: 50, 
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            COM_Status retVal = COM_Status.RESET;
            CmdResult.Perpheral2_HamPipett = COM_Status.RESET;

            try
            {
                string strParam = "";

                // Z_FOLLOW(Z축 연동)에 대한 처리가 필요함(210630) - 별도 명령으로 처리함(210727)
                if (szCmdChar == "DI")          //  DI: 초기화(플런저 초기화 후 tip 탈거)
                {
                    strParam = string.Format("00DIid0000");
                }
                else if (szCmdChar == "DE")     //  DE: Tip 초기화
                {
                    //strParam = string.Format("00DEid0000fr{0:D5}ss{1:D5}", flowrate, stopSpd);
                    strParam = string.Format("00DEid0000fr{0:D5}", flowrate);
                }
                else if (szCmdChar == "TP")     //  TP: Tip 삽입 명령어(tip type 지정필요)
                {
                    strParam = string.Format("00TPid0000tt0{0}", (int)tipType);
                }
                else if (szCmdChar == "TD")     //  TD: Tip 탈거
                {
                    strParam = string.Format("00TDid0000");
                }
                else if (szCmdChar == "AB")     //  AB: 액체 흡입전 공기흡입
                {
                    strParam = string.Format("00ABid0000bv{0:D5}fr{1:D5}", volume1, flowrate);
                }
                else if (szCmdChar == "AT")     //  AT: 이송 공기 흡입
                {
                    strParam = string.Format("00ATid0000tv{0:D5}fr{1:D5}", volume1, flowrate);
                }
                else if (szCmdChar == "RT")     //  RT: Tip 장착 여부 확인
                {
                    strParam = string.Format("00RTid0000");
                }
                else if (szCmdChar == "DL")     //  DL: 액체 토출
                {
                    strParam = string.Format("00DLid0000dv{0:D5}sv{1:D3}fr{2:D5}ss{3:D5}", volume1, volume2, flowrate, stopSpd);
                }
                else if (szCmdChar == "AL")     //  AL: 액체 흡입
                {
                    strParam = string.Format("00ALid0000av{0:D5}oa{1:D5}fr{2:D5}ss{3:D5}", volume1, volume2, flowrate, stopSpd);
                }
                else if (szCmdChar == "MA")     //  MA: Mixing Asp
                {
                    strParam = string.Format("00DLid0000ma{0:D5}fr{1:D5}", volume1, flowrate);
                }
                else if (szCmdChar == "MD")     //  MD: Mixing Disp
                {
                    strParam = string.Format("00ALid0000fr{0:D5}", flowrate);
                }
                else if (szCmdChar == "RN")     //  RN: cLLD 상태 확인
                {
                    strParam = string.Format("00RNid0000");
                }
                else if (szCmdChar == "AV")     // AV: Cycle과 Lifetime 카운터를 파워 off전 저장
                {
                    strParam = string.Format("00AVid0000");
                }
                else if (szCmdChar == "ES")     // ES: Emergency Stop ON
                {
                    strParam = string.Format("00ESid0000");
                }
                else if (szCmdChar == "SR")     // SR: Emergency Stop OFF
                {
                    strParam = string.Format("00SRid0000");
                }
                else if (szCmdChar == "AX")     //  AX: ADC 시작(토출 명령 후 자동으로 꺼짐)
                {
                    strParam = string.Format("00AXid0000");
                }
                else if (szCmdChar == "RF")     //  RF: Firmware Version
                {
                    strParam = string.Format("00RFid0000");
                }
                else if (szCmdChar == "RE")     //  RF: check error code
                {
                    strParam = string.Format("00REid0000");
                }

                BuildCmdPacket(bCommandSendBuffer, "PUMP", "PE2", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                retVal = COM_Status.NAK;
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Perpheral2_HamPipett != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Perpheral2_HamPipett;
            CmdResult.Perpheral2_HamPipett = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Z Axis Follow Motion, when liquid handling(aspirate/dispense)
        // 튜브 내경 및 lead를 고려한 축의 속도 계산
        // argument -> tube_ID: target tube inner diameter, axis_lead: axis lead
        // Z_FOLLOW_DIR UP = -1, DOWN = 1
        private void Liquid_Z_Follow_Move(double volume, double flowrate, double tube_ID, double axis_lead, Z_FOLLOW_DIR dir, PERIPHERAL pipett)
        {
            double travel_time = volume / flowrate;   // sec
            double travel_dist = Math.Round((volume * 1000) / (Math.Pow(tube_ID, 2) * (Math.PI / 4)), 2);    // mm
            double travel_spd_mm = travel_dist / travel_time * 100;  // mm/sec * 100
            int acc = 0; int dec = 0;

            // 1ml, 5ml은 acc, dec를 따로 관리해야 함
            if (pipett == PERIPHERAL.HAM_PIPETT)
            {
                acc = (int)(travel_spd_mm * 2.0);
                dec = (int)(travel_spd_mm * 3.0);
            }
            else if (pipett == PERIPHERAL.TRI_PIPETT)
            {
                acc = (int)(travel_spd_mm * 0.3);   //0.5
                dec = (int)(travel_spd_mm * 2.7);   //1.2
                travel_dist = travel_dist * 1.4;
                travel_spd_mm = travel_spd_mm * 1.4;
            }

            if (acc <= 0) acc = 1;
            if (dec <= 0) dec = 1;

            bPipettMotion = true;

            iPrintf(string.Format("time: {0}, dist: {1}, spd_mm: {2}",
                                  travel_time, (int)dir * travel_dist, travel_spd_mm * 0.01));

            MoveStepMotor(STEP_CMD.MOVE, MOTOR.STEP2, (int)travel_spd_mm, (int)dir * travel_dist,
                                   acc, dec, POS_OPT.REL, HOLD_STATE.NONE);
        }

        // description: run peripheral2(PE2) hamilton pipett cLLD
        // argument ->
        // sensitivity: cLLD sensitivity 1~4 (1:low, 4: high)
        // axis: cLLD 수행 축 지정
        // cLLD speed: step2(pipett axis) speed
        // max pos: max pos를 넘어서면 LLD 입력없이도 모션 정지
        //  cLLD 진행 순서                                   Example
        // 1.지정된 속도로 Z축 하강					      				
        // 2.cLLD 시작 명령 전송(cs는 0으로 지정)            00CLid0000cr0cs1
        // 3.LLD 신호 입력 대기						      			
        // 4.cLLD 정지 명령 전송                             00CPid0000
        // 5.신호 입력시 Z축 모션 정지
        //  * 전송된 Max Pos를 넘어서면 LLD 입력없이도 모션 정지									
        // 6.Z축 현재 값 저장
        private COM_Status Run_Hamilton_cLLD(byte cmdChar, int sensitivity, int timeout = 5)   // RUNCLD
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            COM_Status retVal = COM_Status.RESET;
            CmdResult.cLLD = COM_Status.RESET;

            try
            {
                string strParam = "";
                string strSubCmd = "";

                // 축 연동에 대한 처리가 필요함(210630) - 별도 명령으로 처리함(210727)
                if (cmdChar == 'L')     // CL: Start cLLD process
                {
                    strSubCmd = "PE2";
                    strParam = string.Format("00CLid0000cr0cs{0}", sensitivity);
                }
                else if (cmdChar == 'P')     //  CP: Stop cLLD process
                {
                    strSubCmd = "PE2";
                    strParam = string.Format("00CPid0000");

                    bcLLD_Detected = false;
                    bcLLD_IO = false;
                }
                else if (cmdChar == 'V')     //  check level I/O
                {
                    strSubCmd = "PE2_LEVEL";
                    strParam = "";
                }

                BuildCmdPacket(bCommandSendBuffer, "PUMP", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                retVal = COM_Status.NAK;
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.cLLD != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.cLLD;
            CmdResult.cLLD = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: run peripheral3(PE3) tricontinental pipett
        // argument ->
        // cmd1: I -> CW motion port selection, O -> CCW motion port selection(1~6)
        // cmd2: z -> Initialize, A -> Move motor to absolute position(0~3000), ? -> Ask Current Abs Position
        // cmd3: P -> Move motor relative number of steps in the aspirate direction(0~3000)
        //       D -> Move motor relative number of steps in the dispense direction(0~3000)
        private COM_Status RunPer3_TricontinentPump(byte cmdChar1, int port, byte cmdChar2, int absPos, int topSpd,
                                                    byte cmdChar3, int relPos, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;
            COM_Status retVal = COM_Status.RESET;
            CmdResult.Perpheral3_TriPump = COM_Status.RESET;

            try
            {
                // Set DT Protocol
                string strParam = "";
                string StartChar = "/";
                string PumpAddr = "3";

                if (cmdChar1 == 'I')        // CW
                {
                    strParam = string.Format("I{0}R{1}", port, Environment.NewLine);
                }
                else if (cmdChar1 == 'O')   // CCW
                {
                    strParam = string.Format("O{0}R{1}", port, Environment.NewLine);
                }
                else if (cmdChar1 == ' ')
                {
                    if (cmdChar2 == 'Z')
                    {
                        strParam = string.Format("ZR{0}", Environment.NewLine);
                    }
                    else if (cmdChar2 == 'A')
                    {
                        if (cmdChar3 == 'P')
                        {
                            strParam = string.Format("A{0}V{1}P{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);
                        }
                        else if (cmdChar3 == 'D')
                        {
                            strParam = string.Format("A{0}V{1}D{2}R{3}", absPos, topSpd, relPos, Environment.NewLine);
                        }
                    }
                    else if (cmdChar2 == ' ')
                    {
                        if (cmdChar3 == 'P')
                        {
                            strParam = string.Format("V{0}P{1}R{2}", topSpd, relPos, Environment.NewLine);
                        }
                        else if (cmdChar3 == 'D')
                        {
                            strParam = string.Format("V{0}D{1}R{2}", topSpd, relPos, Environment.NewLine);
                        }
                    }
                    else if (cmdChar2 == 'T')
                    {
                        strParam = string.Format("TR{0}", Environment.NewLine);
                    }
                    else if (cmdChar2 == '?')
                    {
                        strParam = string.Format("?R{0}", Environment.NewLine);
                    }
                    else if (cmdChar2 == 'P')
                    {
                        strParam = string.Format("ZV6000gIA3000OA0G3R{0}", Environment.NewLine);
                    }
                }

                strParam = StartChar + PumpAddr + strParam;

                BuildCmdPacket(bCommandSendBuffer, "PUMP", "PE3", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                retVal = COM_Status.NAK;
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Perpheral3_TriPump != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Perpheral3_TriPump;
            CmdResult.Perpheral3_TriPump = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: read peripheral4 loadcell value
        // argument -> LOADCELL_CMD: SET_TARE = 0, WEIGHT = 1,
        private COM_Status ReadLoadCell(LOADCELL_CMD cmd, int channel, float val = 0, int timeout = 50)   // RDLDCL
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.LoadCell = COM_Status.RESET;
            try
            {
                string strCmd = string.Format("LOADCELL {0}", channel);
                string strSubCmd = string.Format("{0}", cmd);
                string strParam = "";
                if (cmd == LOADCELL_CMD.WR_CAL)
                    strParam = string.Format("{0}", val);
                else
                    strParam = "";

                BuildCmdPacket(bCommandSendBuffer, strCmd, strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.LoadCell != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.LoadCell;
            CmdResult.LoadCell = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: read flowmeter value
        // argument -> SENSOR_CMD cmd: PWR = 0, GET = 1,
        private COM_Status ReadFlowMeter(SENSOR_CMD cmd, Status param, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Flowmeter = COM_Status.RESET;
            try
            {
                string strSubCmd = string.Format("{0}", cmd);
                string strParam = string.Format("{0}", (int)param);

                BuildCmdPacket(bCommandSendBuffer, "FLOW", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Flowmeter != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.Flowmeter;
            CmdResult.Flowmeter = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: read laser sensor
        // argument -> SENSOR_CMD cmd: PWR = 0, GET = 1,
        // result: 0 -> inactive, 1 -> active
        private COM_Status ReadLaserSensor(SENSOR_CMD cmd, Status param, int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.LaserSensor = COM_Status.RESET;
            try
            {
                string strSubCmd = string.Format("{0}", cmd);
                string strParam = string.Format("{0}", (int)param);

                BuildCmdPacket(bCommandSendBuffer, "LASER", strSubCmd, strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.LaserSensor != COM_Status.RESET)
                    break;
            }
            retVal = CmdResult.LaserSensor;
            CmdResult.LaserSensor = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: camera2 & strobe trig (N/A)
        private COM_Status StrobeTrigger(int period, int timeout = 50)   // STROBE
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Strobe = COM_Status.RESET;

            try
            {
                string strParam = string.Format("{0}", period);

                BuildCmdPacket(bCommandSendBuffer, "STROBE", "", strParam);

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            for (int i = 0; i < timeout; i++)
            {
                ReceiveDirect();
                Thread.Sleep(1);
                if (CmdResult.Strobe != COM_Status.RESET)
                    break;
            }

            retVal = CmdResult.Strobe;
            CmdResult.Strobe = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }

        // description: Emergency stop (board(FW) -> Host)의 response
        private COM_Status ResponseEmergency(int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.Stop = COM_Status.RESET;
            try
            {
                BuildCmdPacket(bCommandSendBuffer, "SWITCH", "STOP", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            retVal = CmdResult.Stop;
            CmdResult.Stop = COM_Status.RESET;

            return retVal;
        }

        // description: Power off의 response(board(FW) -> Host)
        private COM_Status ResponseSystemOff(int timeout = 50)
        {
            if (!Serial.IsOpen)
                return COM_Status.NAK;

            bDirectReceive = true;

            COM_Status retVal = COM_Status.RESET;
            CmdResult.SystemOffRes = COM_Status.RESET;
            try
            {
                BuildCmdPacket(bCommandSendBuffer, "pwroff", "", "");

                SerialByteSend(bCommandSendBuffer, nSendBufferLength);      // Send Cmd to Serial Port
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            retVal = CmdResult.SystemOffRes;
            CmdResult.SystemOffRes = COM_Status.RESET;
            bDirectReceive = false;

            return retVal;
        }
    }
}
