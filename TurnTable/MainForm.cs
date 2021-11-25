using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Timers;
using System.Drawing;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.Reflection;
using System.ServiceProcess;
using System.Management;
using MvCamCtrl.NET;
using OpenCvSharp;
using OpenCvSharp.Extensions;
//using OpenCvSharp.UserInterface;
using OpenCvSharp.Blob;
//using OpenCvSharp.CPlusPlus;
using System.Linq;
using System.Text;


namespace CytoDx
{
    public partial class MainWindow : MaterialForm
    {
        public enum Recipe_COL
        {
            Enable = 0,
            Command1,
            Command2,
            Param1,
            Param2,
            Param3,
            Param4,
            Param5,
            Param6,
            Param7,
            Sleep,
            Comment,
        }

        public enum Tpnt_COL
        {
            Idx,
            Name,
            X,
            Y,
            Z,
            Gripper,
            Pipett,
        }

        public enum CMD
        {
            STOP = 0,
            RUN = 1,
            ESTOP = 2,
        }

        public enum RW_CMD
        {
            WRITE = 0,
            READ = 1,
        }

        public enum READ
        {
            STOP = 0,
            START = 1,
        }

        public enum EDIT
        {
            SAVE = 0,
            INSERT = 1,
        }

        public enum WAIT_OPT
        {
            NOWAIT = 0,
            WAIT = 1,
        }

        public enum STEP_CMD
        {
            MOVE = 0,
            STOP = 1,
            HOLD = 2,
            HOME = 3,
            ALM_RST = 4,
            SPD_SCALE = 5,
            POS = 6,
            STATUS = 7,
            NONE = 8,
        }

        public enum LOADCELL_CMD
        {
            SET_TARE = 0,
            WEIGHT = 1,
            WR_CAL = 2,
            RD_CAL = 3,
        }

        public enum SENSOR_CMD
        {
            PWR = 0,
            GET = 1,
        }

        public enum POS_OPT
        {
            ABS = 0,
            REL = 1,
            NONE = 2,
        }

        public enum VALUE
        {
            WRITE = 0,
            CLEAR = 1,
        }

        public enum CHAMBER_POS
        {
            CHAMBER1 = 0,
            CELLDOWN1 = 1,
            CHAMBER2 = 2,
            CELLDOWN2 = 3,
        }

        public enum VALVE
        {
            CLOSE = 0,
            OPEN = 1,
        }

        public enum TIP_TYPE
        {
            _10UL = 0,      // index는 매뉴얼 28페이지 참조
            _300UL = 4,
            _1000UL = 6,
            NONE = -1,
        }

        public enum Z_FOLLOW
        {
            OFF = 0,
            ON = 1,
            NONE = -1,
        }

        public enum Z_FOLLOW_DIR
        {
            UP = -1,
            DOWN = 1,
        }

        const bool STATUS_OK = true;
        const bool STATUS_NG = false;

        public enum TEST
        {
            READY,
            RUNNING,
            PASS,
            FAIL,
        }

        public enum OUTPUT
        {
            READY,
            PROCESSING,
            DONE,
            FAIL,
        }

        public enum TestStatus
        {
            NG,
            OK,
        }

        public enum MotorMon
        {
            RPM,
            POS,
            STATUS,
            ALARM,
        }

        public enum SwitchState
        {
            OFF,
            RUN,
            STOP,
            DOOR_RST,
            DOOR_ENA,
            STATUS,
        }

        public enum Direction
        {
            CCW = '0',   //0x30,    // '0'
            CW = '1',   //0x31,    // '1'
            STOP = 'S',   //0x53,    // 'S'
        }

        public string VERSION = "v2021.11.16";
        // 2020.12.02  4 Digit Pump Time (99.9 sec --> 999.9 sec)
        // 2021.07.20 FW Protocol Verified
        // 2021.07.30 FW Protocol Applied
        // 2021.09.30 Position Monitoring Done(one Timer version)
        // 2021.11.08 speed unit modified(for slow motion)
        // 2021.11.15 통신 속도 및 펌웨어 통신 방식을 Ring Buffer를 사용하도록 변경함

        private readonly MaterialSkinManager materialSkinManager;
        private object lockSerial = new object();
        private object lockCamera = new object();

        public static Logger logger = LogManager.GetCurrentClassLogger();
        string basedir = Assembly.GetExecutingAssembly().CodeBase;
        public PatternInput patternInput = new PatternInput();
        public DefineButtonForm defineButtonForm = new DefineButtonForm();
        public TpntEditForm tpntinput = new TpntEditForm();

        public SerialPort Serial = new SerialPort();
        private System.Windows.Media.MediaPlayer audioPlayer = new System.Windows.Media.MediaPlayer();

        public string TabItem = "";
        static int m_current_running_row = -1;
        static bool m_bSaveRecord = false;
        bool bStopFlag = false;
        public bool bSerialStop = false;
        bool bStatusOk = true;
        string m_ErrorMessage = "";
        bool bFullProcess = false;
        bool isForcedStop = false;
        static bool isRunning = false;
        bool isRunningSingle = false;
        bool isRunningManual = false;
        bool bRpmChanged = false;
        bool isRecording = false;
        //bool isConverting = true;

        //Video Play 관련
        bool isScrolled_Video = false;        // TrackBar 움직였는지
        int trackBarBlankSize_Video = 14;     // TrackBar 양옆 빈공간
        int trackBarLength_Video = 0;         // TrackBar의 실제 길이
        int trackBarMouseX_Video = 0;         // TrackBar에서 마우스 클릭 위치

        public int SerialPortsCount = 0;
        public int ComPortIndex = 0;
        public bool bDirectReceive = false;

        private System.Timers.Timer timer_pos = new System.Timers.Timer();
        private System.Timers.Timer timer_com = new System.Timers.Timer();
        private System.Timers.Timer timer_video = new System.Timers.Timer();
        private System.Timers.Timer timer_camera = new System.Timers.Timer();
        
        // Video Log text 관련
        private System.Timers.Timer timer_frame = new System.Timers.Timer();

        public bool bSerialTimerState = false;
        public bool bPosTimerState = false;
        public bool bPosTimerRun = false;
        public bool bMotionDoneWait = false;

        public bool bServoRunState = false;
        public bool bStepRunState = false;
        public bool bPipettMotion = false;

        public int iTimerCount2 = 0;
        public int currentRecipeIndex = 0;
        public int lastRecipeIndex = 0;
        public bool isRunningRecipeChanged = false;
        public double recipeTimeSum = 0;
        public long recipeFrameSum = 0;
        public long estimatedCurrentFrame = 0;
        public double currentIndexStartTime = 0;
        public float fpsFromPrescale = 0;
        public double lastCheckedTime;
        public TimeSpan estimatedVideoTime;
        public float fpsCalcBase;
        static float VIDEO_LOG_FRAME_SCALE = 0.820144f;
        public float currentRecordFps;
        public bool isShakeFpsZero = false;
        public bool isSpinPresetOrFPSZero = false;

        List<string> frameLogWriteBuffer = new List<string>();
        public List<Button> DefineButtons = new List<Button>();
        public int ButtonCount = 0;
        public List<BUTTON_RECIPE> ListButtonRecipe = new List<BUTTON_RECIPE>();

        public Color ButtonSelectColor = Color.Azure;

        private Stopwatch stopwatch = new Stopwatch();
        private Stopwatch flow_stopwatch = new Stopwatch();

        public Config config = new Config();

        public int SerialTimerCnt = 0;
        public float SerialTimerLimit = 0;
        public int PosTimerCnt = 0;
        public float PosTimerLimit = 0;
        public bool bSendCommandTimeFlag = false;

        public byte[] bCommandSendBuffer = new byte[64];
        public byte[] stateRequestBuf = new byte[64];
        public byte[] bSerialRcvDataFrame = new byte[128];
        public int nSendBufferLength = 0;

        public int RcvFramePtr = 0;
        public bool ReceiveFrameFlag = false;
        public int MAX_RCV_FRAME_SIZE = 128;
        public int RcvETXPtr = 0;
        public int nRcvBuffCnt = 0;

        public string g_strRcvCmd = "";
        public string g_strRcvSubCmd = "";
        public string g_strRcvParam = "";
        public string g_strRcvDataFrame = "";

        public string g_strSndCmd = "";
        public string g_strSndSubCmd = "";
        public string g_strSndParam = "";

        public int CS_START_POS = 2;
        public int CHECK_SUM_POS = 28;
        public int END_TEXT_POS = 29;
        public int FRAME_LENGTH = 30;

        public int MAXCNT_TPNT = 200;
        public int MAXCNT_TPNT_SORT = 5;
        
        public bool bScanFlag = false;

        const string MEDIA_PLAY = "\u25B6";
        const string MEDIA_PAUSE = "\u23F8";
        const string MEDIA_BACKWARD = "\u23EE";
        const string MEDIA_FORWARD = "\u23ED";
        const string MEDIA_STOP = "\u23F9";
        const string MEDIA_RECORD = "\u23FA";
        const string MEDIA_EJECT = "\u23CF";
        const string MEDIA_SHUFFLE = "\u1F500";
        const string MEDIA_REPEAT = "\u1F501";
        const string MEDIA_REPEAT_ONCE = "\u1F502";
        const string MEDIA_OPEN_FOLDER = "\u1F4C2";
        const string MEDIA_FOLDER = "\u1F5C0";
        const string MEDIA_RECODER = "\u1F4F9";
        const string MEDIA_PLUS = "\u2795";

        public byte bAutoManual;
        public byte AUTO_MODE = 0;
        public byte MANUAL_MODE = 1;
        public byte boggle = 0;

        public bool holdTimer = false;         // btn을 통한 serial CMD 시에 timer 루프를 continue
        public string lastRcvStr = "";

        public string CurrentRecipeCommand = "";
        public int GetInformPosition;
        public bool bHptDriverWorkFlag = false;
        public const int TimerIntervalMsec = 50;
        public bool bRunningThread = false;
        public bool bShudown = false;

        public static string DIR_HOME = "C:\\TruNser_C2000";
        public static string DIR_LOG = "C:\\TruNser_C2000\\Logs";
        public static string DIR_IMAGE = "C:\\TruNser_C2000\\Image";
        public static string DIR_VIDEO = "C:\\TruNser_C2000\\Video";
        public static string DIR_MUSIC = "C:\\TruNser_C2000\\Music";
        public static string DIR_RECIPE = "C:\\TruNser_C2000\\Recipe";
        public static string DIR_VIDEO_LOG = "C:\\TruNser_C2000\\Video\\VideoLog";

        public string VideoPlayButtonStatus = "";
        public int DISK_SPACE_UCL = 90; // Low space warning Percent

        public bool[] RestartPortList = new bool[14];
        public bool bShowMsgBox = false;
        public SENSOR_STATUS SensorStatus;
        public CMD_RESULT CmdResult;
        public CurrentPosition CurrentPos;
        //INT_REQUEST IntRequest;
        RPM Rpm;
        public Mat srcImage;

        public double Tube_ID_15ml = 14.52;
        public double Tube_ID_50ml = 27.5;
        public double Tube_ID_1_5ml = 9.4;
        public double Lead_AxisZ = 12.0;
        public double Lead_AxisHam = 1.0;

        public double PumpSyringe_Vol = 12500.0;
        public double PumpMax_Increment = 3000.0;
        public double PumpVel_Resolution = 6000.0;

        public double TriPipett_Vol = 5000.0;
        public double TriPipett_Max_Increment = 1600.0;
        public double TriPipett_Vel_Resolution = 6000.0;
        public double TriPipett_ul_per_inc = 3.808; // 매뉴얼에 기재됨
        public int PE1PlungerPosInc = 0;

        public string strcLLD_Sensitivity;
        public string strcLLD_Dir_Axis;
        public string strTpnt_Sort;
        public string strTpnt;
        public string strTpnt_Name;
        public int[] nTpnt_Cnt = new int[5];

        public bool bHamilton_EStopFlag = false;
        public bool bHamilton_Z_Follow_Flag = false;
        public bool bHamilton_ADCFlag = false;

        public int nTipPresence = 0;
        public int nState_cLLD = 0;

        public bool bcLLD_Detected = false;
        public bool bcLLD_IO = false;

        public int nLaserDetected = 0;

        public string strGatherValue;
        public string strAccScale;

        public bool bPosReadStart = false;

        public int nEccentricThreshold = 0;
        public int nEccentricCnt = 0;

        Thread PosMonitorThread;
        Thread StateMonitorThread;
        public bool bPosMonitorTh = false;
        public bool bStateMonitorTh = false;
        public bool bCommunicationActive = false;

        public bool[] bAxisMovingFlag = new bool[6];
        public bool[] bAxisStartFlag = new bool[6];
        public bool bStepMotorInitDoneState = false;
        public bool bSystemInitDoneState = false;
        public double dbMovingDist = 0;

        public bool bPeltRunState = false;
        public float fPeltOffTemp = 50;

        public enum TAB
        {
            MAIN,
            CAL,
            CAMERA,
            MUSIC,
            SETTING,
        }

        public enum COM_Status
        {
            RESET = -1,
            NAK = 0,
            ACK = 1,
        }

        public enum Status
        {
            OFF = 0,
            ON = 1,
            NONE = 2,
        }

        public enum Light
        {
            Chamber = 0,
            Room = 1,
        }

        public enum HOLD_STATE
        {
            FREE = 0,
            HOLD = 1,
            NONE = 2,
        }

        public enum PELT_CMD
        {
            SET_SV = 0,
            SET_FAN = 1,
            SET_BIAS = 2,
            CHAMBER_SV = 3,
            CHAMBER_PV = 4,
            PELTIER_PV = 5,
            HITSINK_PV = 6,
            READ_ALL = 7,
            RD_BIAS = 8,
            RD_FAN = 9,
        }

        public enum HOME_CMD
        {
            SET_SV = 0,
            SET_FAN = 1,
            CHAMBER_SV = 2,
            CHAMBER_PV = 3,
            PELTIER_PV = 4,
            HITSINK_PV = 5,
            READ_ALL = 6,
        }

        public const int NO_ALARM = 0xFFFF;

        public class BUTTON_RECIPE
        {
            public Button button;
            public List<Recipe> recipe = new List<Recipe>();
        }

        //public TOOL_OFFSET Tool_Offset_Val;
        public TOOL_OFFSET offset_val;
                
        public struct TOOL_OFFSET
        {
            public double dbX;
            public double dbY;
            public double dbZ;
            public string SelectedTool;
        };

        public STROKE axis_stroke;

        public struct STROKE
        {
            public double X_min;
            public double X_max;
            public double Y_min;
            public double Y_max;
            public double Z_min;
            public double Z_max;
            public double Grip_min;
            public double Grip_max;
            public double Ham_min;
            public double Ham_max;
            public double Cover_min;
            public double Cover_max;
        };

        public LOADCELLVAL LoadcellVal;

        public struct LOADCELLVAL
        {
            public int nGain;
            public int nScale;
            public int nOffset;
            public float fWeight;
            public float fCalVal;
        };

        public SERVO_MON ServoMon;

        public struct SERVO_MON
        {
            public int    nCurrServoPosition;
            public int    nCurrServoRpm;
            public double dbCurrServoRcf;
            public int    nServoErrCode;
        };

        public PELT_MON PeltMon;

        public struct PELT_MON
        {
            public double dbSetPeltTemp;
            public double dbTempChamber;
            public double dbTempPeltier;
            public double dbTempCooler;
        };

        public SWITCH_MON SwitchMon;

        public struct SWITCH_MON
        {
            public int Status_Switch;
            public bool bDoorOpenSave;
            public bool bDoorEnable;
            public bool bDoorSW;
            public bool bPower;
            public bool bStop;
            public bool bRun;
        };

        public ERROR_MON ErrorMon;

        public struct ERROR_MON
        {
            public int Status_Error;
            public bool bCoolerChamberSensor_open;
            public bool bCoolerChamberSensor_short;
            public bool bCoolerFanSensor_open;
            public bool bCoolerFanSensor_short;
            public bool bCoolerPeltSensor_open;
            public bool bCoolerPeltSensor_short;
            public bool bLoadcell0_notConnected;
            public bool bLoadcell1_notConnected;
            public bool bLoadcell2_notConnected;
        };

        public SERVOSTATE ServoState;
        public STEP0AXISSTATE Step0AxState;
        public STEP1AXISSTATE Step1AxState;
        public STEP2AXISSTATE Step2AxState;
        public HAMAXISSTATE HamAxState;
        public GRIPAXISSTATE GripAxState;
        public DOORAXISSTATE CoverAxState;

        public struct SERVOSTATE
        {
            public int Status_Servo;
            public bool bHOME_COMP;
            public bool bINJECT_LIMIT_HI;
            public bool bINJECT_LIMIT_LOW;
            public bool bALM;
            //public bool bHOME_END;
            //public bool bPLS_RDY;
            //public bool bREADY;
            //public bool bMOVE;
        };

        public struct STEP0AXISSTATE
        {
            public int Status_Step0;
            public bool bHOME_COMP;
            public bool bHOME_END;
            public bool bPLS_RDY;
            public bool bREADY;
            public bool bMOVE;
            public bool bALM_B;
        };

        public struct STEP1AXISSTATE
        {
            public int Status_Step1;
            public bool bHOME_COMP;
            public bool bHOME_END;
            public bool bPLS_RDY;
            public bool bREADY;
            public bool bMOVE;
            public bool bALM_B;
        };

        public struct STEP2AXISSTATE
        {
            public int Status_Step2;
            public bool bHOME_COMP;
            public bool bHOME_END;
            public bool bPLS_RDY;
            public bool bREADY;
            public bool bMOVE;
            public bool bALM_B;
        };

        public struct HAMAXISSTATE
        {
            public int Status_Ham;
            public bool bHOME_COMP;
            public bool bLIMIT_HI;
            public bool bLIMIT_LOW;
            public bool bMove;
            public bool bALM;
        };

        public struct GRIPAXISSTATE
        {
            public int Status_Grip;
            public bool bHOME_COMP;
            public bool bLIMIT_HI;
            public bool bLIMIT_LOW;
            public bool bMove;
            public bool bALM;
        };

        public struct DOORAXISSTATE
        {
            public int Status_Door;
            public bool bHOME_COMP;
            public bool bLIMIT_HI;
            public bool bLIMIT_LOW;
            public bool bMove;
            public bool bALM;
        };

        public struct SENSOR_STATUS
        {
            public bool Alarm;
            public bool Home;
            public Status LaserSensor;
            public Status DoorSwitch;
            public Status DoorOpenSave;
            public Status DoorEnable;
            public Status AlarmServo;
            public Status AlarmStep_Grip_ax;
            public Status AlarmStep_Ham_ax;
            public Status AlarmStep_Door_ax;
            public Status AlarmStep0_X_ax;
            public Status AlarmStep1_Y_ax;
            public Status AlarmStep2_Z_ax;
            public Status PowerState;
            public Status PowerSwitch;
            public Status RunSwitch;
            public Status StopSwitch;
            public Status RotorCover;
            public Status EccentricProxi; // 근접센서
            public Status AlarmPeri1_tri_pipett;
            public int tri_pipett_errNo;
            public Status AlarmPeri2_ham_pipett;
            public int ham_pipett_errNo;
            public Status AlarmPeri3_tri_pump;
            public int tri_pump_errNo;
            public Status AlarmPeri4_loadcell;
            public bool ErrEccentricCnt;
            public int RPM;
            public Status AlarmCoolerChamberSensor;
            public Status AlarmCoolerFanSensor;
            public Status AlarmCoolerPeltSensor;
            public Status AlarmLoadcell0;
            public Status AlarmLoadcell1;
            public Status AlarmLoadcell2;
        }

        public struct CMD_RESULT
        {
            public COM_Status GetSTATUS;
            public COM_Status SetTestCondition;
            public COM_Status GetTestCondition;
            public COM_Status GetRpm;
            public COM_Status ResetServoAlarm;
            public COM_Status GetVersion;
            public COM_Status Spin;
            public COM_Status Shake;
            public COM_Status DoorLock;
            public COM_Status MovePump;
            public COM_Status PinchValve;
            public COM_Status ReadPeltier;
            public COM_Status WritePeltier;
            public COM_Status StepMotor1Gripper;
            public COM_Status StepMotor2Pipett;
            public COM_Status StepMotor3RotorCover;
            public COM_Status StepMotor4AxisX;
            public COM_Status StepMotor5AxisY;
            public COM_Status StepMotor6AxisZ;
            public COM_Status StepCrdMoveXY;
            public COM_Status ReadMotorPos;
            public COM_Status MoveSyringeMotorCtc;
            public COM_Status MoveSyringeMotorWb;
            public COM_Status ResetMotor;
            public COM_Status ResetPeripheral;
            public COM_Status EccentricClear;
            public COM_Status ReadEccentric;
            public COM_Status Strobe;
            public COM_Status RotorPos;
            public COM_Status LightCond;
            public COM_Status TopLight;
            public COM_Status RotorLight;
            public COM_Status Perpheral1_TriPipett;
            public COM_Status Perpheral2_HamPipett;
            public COM_Status cLLD;
            public COM_Status Perpheral3_TriPump;
            public COM_Status LoadCell;
            public COM_Status Accelometer;
            public COM_Status Flowmeter;
            public COM_Status LaserSensor;
            public COM_Status Cover;
            public COM_Status Stop;
            public COM_Status SystemOff;
            public COM_Status SystemOffRes;
            public COM_Status ServoState;
            public COM_Status ServoOnOff;
            public COM_Status CoolingFan;
            public COM_Status Switch;
            public COM_Status Home;
            public COM_Status ControllerCom;
            public COM_Status SystemCmd;
        }

        //public struct INT_REQUEST
        //{
        //    public bool RunManual;
        //    public bool StopManual;
        //    public bool Stop;
        //}

        public struct RPM
        {
            public double Tick;
            public double Target;
            public double Current;
            public double AccRpm;
            public int prescale;
            public double minChange;
        }

        public enum MOTOR
        {
            SERVO = 0,
            GRIP = 1,
            HAM = 2,
            COVER = 3,
            STEP0 = 4,
            STEP1 = 5,
            STEP2 = 6,
        }

        //public enum AXIS
        //{
        //    NONE = 0,
        //    AXIS_X = 4,
        //    AXIS_Y = 5,
        //    AXIS_Z = 6,
        //}

        public enum PERIPHERAL
        {
            TRI_PIPETT = 1,
            HAM_PIPETT = 2,
            TRI_PUMP = 3,
            LOAD_CELL = 4,
        }

        public enum PROCESS_CMD
        {
            STEP_MOVEA,
            MOV_TOOL_XY,
            MOV_T_PNT,
            PELTIER,
            PIPETT_TRI,
            PIPETT_HAM_DRY,
            PIPETT_HAM_LIQ,
            FIND_SURFACE,
            MOV_Z_AXES,
            SPIN,
            SPIN_PARAM,
            SPIN_POS,
            SEL_TOOL,
            VALVE,
            LIGHT,
            COVER,
            READ_LDCELL,
            READ_LASER,
            SEL_PUMP_PORT,
            ACT_PUMP,
            CAMERA,
            MOV_X,
            MOV_Y,
            MOV_Z,
            MOV_GRIPPER,
            MOV_PIPETT,
            MOT_HOLD,
            MOT_STOP,
            RECORD,
            SLEEP,
            READ_FLOW,
        }

        public struct CurrentPosition
        {
            public double StepGripAxis;
            public double StepHamAxis;
            public double StepRotCover;
            public double Step0AxisX;
            public double Step1AxisY;
            public double Step2AxisZ;
            public double Servo;
            public double Servo_Deg;
        }

        public struct CommandParam
        {
            public string strCmd1;
            public string strCmd2;
            public bool enable;
            public int param_cnt;
            public string param1;
            public string param2;
            public string param3;
            public string param4;
            public string param5;
            public string param6;
            public string param7;
            public string sleep;
            public string comment;
        }

        public struct TpntParam
        {
            public string param1;
            public string param2;
            public string param3;
            public string param4;
            public string param5;
            public string param6;
            public string param7;
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public MainWindow()
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).GetLength(0) > 1)
            {
                MessageBox.Show("Already Running TruNser", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
            CreateFolder();
            LoggerConfig();

            InitializeComponent();

            InitMediaButton();
            m_pMyCamera = new MyCamera();
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            DeviceListAcq();

            trackBarLength_Music = TrackBar_Music.Size.Width - (trackBarBlankSize_Music * 2); // TrackBar의 실제 길이
            trackBarLength_Video = TrackBar_Video.Size.Width - (trackBarBlankSize_Video * 2); // TrackBar의 실제 길이

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            UpdateConfig(RW.READ);
            SelectTheme(config.Theme);
            SelectColorScheme(config.ColorScheme);

            this.Closing += App_Closing;
            this.Closed += App_Closed;

            this.Text = $"CytoDx TruNser ({VERSION})";
            logger.Info("====================================");
            logger.Info("Start " + this.Text);
        }


        public class DBufferTableLayoutPanel:TableLayoutPanel
        {
            public DBufferTableLayoutPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Top = config.FormTop;
            this.Left = config.FormLeft;
            this.Width = config.FormWidth;
            this.Height = config.FormHeight;
            this.TopMost = config.TopMost;

            Rpm.Current = 0;
            Rpm.Target = 0;
            Rpm.Tick = 0;

            m_MusicFolderPath = DIR_MUSIC; //Environment.CurrentDirectory + "\\Music";
            UpdateMusicPlayList(m_MusicFolderPath);
            this.LV_MP3_play.HideSelection = true;
            LV_MP3_play.Width = 0;

            for (int i = 0; i < LV_MP3_play.Columns.Count; i++)
                LV_MP3_play.Width += LV_MP3_play.Columns[i].Width;

            this.label_song_title.Text = "";
            label_TimeStamp.Text = "";
            audioPlayer.Volume = 100;

            DeleteOldFiles();

            bPosReadStart = false;

            SetupDatagridView();

            if (config.bDebugMode)
                DebugStatus.Show();
            else
                DebugStatus.Hide();
            if (!config.bDebugMode)
            {
                SearchComport();
            }

            timer_pos.Elapsed += new ElapsedEventHandler(Pos_TickTimer);
            timer_com.Elapsed += new ElapsedEventHandler(Serial_TickTimer);
            timer_video.Elapsed += new ElapsedEventHandler(Video_TickTimer);
            timer_camera.Elapsed += new ElapsedEventHandler(Camera_TickTimer);
            timer_frame.Elapsed += new ElapsedEventHandler(FrameWatcher_TickTimer);
            
            this.WindowState = FormWindowState.Maximized;
            ComboBox CB = new ComboBox();

            radio_rpm.Checked = true;
            this.UpdateDiskInformation();
            this.SetCtrlWhenClose();
            this.ManageCOMUI();

            DeviceListAcq();
            ReadWriteButtonConfig(RW.READ, config.LastButtonFileName);
            btnButtonReload_Click(this, null);
            ClearError();

            SetAxisStoke();

            bPosMonitorTh = true;
            Thread PosMonitorThread = new Thread(CurPosRead);
            PosMonitorThread.Name = "PosMonitorThread";

            PosMonitorThread.Start();
            bPosReadStart = true;

            bStateMonitorTh = true;
            Thread StateMonitorThread = new Thread(CurStateRead);
            StateMonitorThread.Name = "StateMonitorThread";

            StateMonitorThread.Start();
        }
        
        public void SetAxisStoke()  //Unit: mm
        {
            axis_stroke.X_min = 0.0;    //driver: -2.5mm
            axis_stroke.X_max = 582.0;  //driver: 584mm
            axis_stroke.Y_min = 0.0;    //driver: -2.5mm
            axis_stroke.Y_max = 290.0;  //driver: 292mm
            axis_stroke.Z_min = 0.0;    //driver: -2.5mm
            axis_stroke.Z_max = 169.0;  //driver: 170mm
            axis_stroke.Grip_min = 0.0;
            axis_stroke.Grip_max = 110.0;
            axis_stroke.Ham_min = 0.0;
            axis_stroke.Ham_max = 110.0;
            axis_stroke.Cover_min = 0.0;
            axis_stroke.Cover_max = 117.0;
        }

        private void PositionDataDisplay()
        {
            if (bStateMonitorTh == false || bShudown == true)
                return;

            if (label_WorldPosX.InvokeRequired == true)
            {
                this.label_WorldPosX.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosX.Text = CurrentPos.Step0AxisX.ToString("F2");
                });
            }
            else
            {
                label_WorldPosX.Text = CurrentPos.Step0AxisX.ToString("F2");
            }

            if (label_WorldPosY.InvokeRequired == true)
            {
                this.label_WorldPosY.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosY.Text = CurrentPos.Step1AxisY.ToString("F2");
                });
            }
            else
            {
                label_WorldPosY.Text = CurrentPos.Step1AxisY.ToString("F2");
            }

            if (label_WorldPosZ.InvokeRequired == true)
            {
                this.label_WorldPosZ.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosZ.Text = CurrentPos.Step2AxisZ.ToString("F2");
                });
            }
            else
            {
                label_WorldPosZ.Text = CurrentPos.Step2AxisZ.ToString("F2");
            }

            if (label_WorldPosCenDoor.InvokeRequired == true)
            {
                this.label_WorldPosCenDoor.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosCenDoor.Text = CurrentPos.StepRotCover.ToString("F2");
                });
            }
            else
            {
                label_WorldPosCenDoor.Text = CurrentPos.StepRotCover.ToString("F2");
            }

            if (label_WorldPosGripper.InvokeRequired == true)
            {
                this.label_WorldPosGripper.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosGripper.Text = CurrentPos.StepGripAxis.ToString("F2");
                });
            }
            else
            {
                label_WorldPosGripper.Text = CurrentPos.StepGripAxis.ToString("F2");
            }

            if (label_WorldPosHamPipett.InvokeRequired == true)
            {
                this.label_WorldPosHamPipett.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosHamPipett.Text = CurrentPos.StepHamAxis.ToString("F2");
                });
            }
            else
            {
                label_WorldPosHamPipett.Text = CurrentPos.StepHamAxis.ToString("F2");
            }

            if (label_WorldPosServo.InvokeRequired == true)
            {
                this.label_WorldPosServo.Invoke((MethodInvoker)delegate ()
                {
                    label_WorldPosServo.Text = CurrentPos.Servo_Deg.ToString("F2");
                });
            }
            else
            {
                label_WorldPosServo.Text = CurrentPos.Servo_Deg.ToString("F2");
            }
        }

        private void labelPanelRunState_Click(object sender, EventArgs e)
        {
            if (SensorStatus.RunSwitch == Status.ON)
            {
                SwitchControl(SwitchState.RUN, Status.OFF);
            }
            else if (SensorStatus.RunSwitch == Status.OFF)
            {
                SwitchControl(SwitchState.RUN, Status.ON);
            }
        }

        private void labelPanelStopState_Click(object sender, EventArgs e)
        {
            if (SensorStatus.StopSwitch == Status.ON)
            {
                SwitchControl(SwitchState.STOP, Status.OFF);
            }
            else if (SensorStatus.StopSwitch == Status.OFF)
            {
                SwitchControl(SwitchState.STOP, Status.ON);
            }
        }

        private void labelPanelPowerState_Click(object sender, EventArgs e)
        {
            if (SensorStatus.PowerSwitch == Status.ON)
            {
                SwitchControl(SwitchState.OFF, Status.OFF);
                SwitchMon.bPower = false;
                SensorStatus.PowerSwitch = Status.OFF;
            }
            // 메인보드 전원 off 상태이기 때문에 ON 명령을 수신할 수 없음
            else if (SensorStatus.PowerSwitch == Status.OFF)
            {
                SwitchControl(SwitchState.OFF, Status.ON);
            }
        }

        private void SwitchStateDisplay()
        {
            if (bStateMonitorTh == false || bShudown == true)
                return;

            if (SensorStatus.RunSwitch == Status.ON && SensorStatus.PowerSwitch == Status.ON)
            {
                this.labelPanelRunState.BackColor = Color.Azure;
                this.labelPanelRunState.ForeColor = Color.Black;

                this.btnRecipeTest.BackColor = Color.Azure;
                this.btnRecipeTest.ForeColor = Color.Black;
                this.btnRecipeStop.BackColor = Color.RoyalBlue;
                this.btnRecipeStop.ForeColor = Color.Black;
            }
            else if (SensorStatus.RunSwitch == Status.OFF || SensorStatus.PowerSwitch == Status.OFF)
            {
                this.labelPanelRunState.BackColor = Color.Gray;
                this.labelPanelRunState.ForeColor = Color.DarkGray;

                this.btnRecipeTest.BackColor = Color.Gray;
                this.btnRecipeTest.ForeColor = Color.DarkGray;
                this.btnRecipeStop.BackColor = Color.Gray;
                this.btnRecipeStop.ForeColor = Color.DarkGray;
            }

            if (SensorStatus.StopSwitch == Status.ON && SensorStatus.PowerSwitch == Status.ON)
            {
                this.labelPanelStopState.BackColor = Color.RoyalBlue;
                this.labelPanelStopState.ForeColor = Color.Black;
            }
            else if (SensorStatus.StopSwitch == Status.OFF || SensorStatus.PowerSwitch == Status.OFF)
            {
                this.labelPanelStopState.BackColor = Color.Gray;
                this.labelPanelStopState.ForeColor = Color.DarkGray;
            }

            if (SensorStatus.PowerSwitch == Status.ON)
            {
                this.labelPanelPowerState.BackColor = Color.OrangeRed;
                this.labelPanelPowerState.ForeColor = Color.Black;
            }
            else if (SensorStatus.PowerSwitch == Status.OFF)
            {
                this.labelPanelPowerState.BackColor = Color.Gray;
                this.labelPanelPowerState.ForeColor = Color.DarkGray;
                bSystemInitDoneState = false;

                // power off시 버튼 명칭 변경
                if (btnInitializeAll.InvokeRequired == true)
                {
                    this.btnInitializeAll.Invoke((MethodInvoker)delegate ()
                    {
                        btnInitializeAll.Text = "Initialize All";
                    });
                }
                this.btnInitializeAll.BackColor = Color.LightSkyBlue;

                if (btnComConnect.InvokeRequired == true)
                {
                    this.btnComConnect.Invoke((MethodInvoker)delegate ()
                    {
                        btnComConnect.Text = "Connect";
                    });
                }

                if (ComStatus.InvokeRequired == true)
                {
                    this.ComStatus.Invoke((MethodInvoker)delegate ()
                    {
                        ComStatus.Enabled = false;
                    });
                }
            }

            if (SensorStatus.DoorEnable == Status.ON)
            {
                this.btnDoorActive.BackColor = Color.SteelBlue;

                if (btnDoorActive.InvokeRequired == true)
                {
                    this.btnDoorActive.Invoke((MethodInvoker)delegate ()
                    {
                        btnDoorActive.Text = "Door Active";
                    });
                }
                else
                {
                    btnDoorActive.Text = "Door Active";
                }
            }
            else if (SensorStatus.DoorEnable == Status.OFF)
            {
                this.btnDoorActive.BackColor = Color.LightPink;

                if (btnDoorActive.InvokeRequired == true)
                {
                    this.btnDoorActive.Invoke((MethodInvoker)delegate ()
                    {
                        btnDoorActive.Text = "Door Inactive";
                    });
                }
                else
                {
                    btnDoorActive.Text = "Door Inactive";
                }
            }

            if (SensorStatus.DoorSwitch == Status.ON)
            {
                if (LabelDoorState.InvokeRequired == true)
                {
                    this.LabelDoorState.Invoke((MethodInvoker)delegate ()
                    {
                        LabelDoorState.Text = "Closed";
                    });
                }
                else
                {
                    LabelDoorState.Text = "Closed";
                }
            }
            else if (SensorStatus.DoorSwitch == Status.OFF)
            {
                if (LabelDoorState.InvokeRequired == true)
                {
                    this.LabelDoorState.Invoke((MethodInvoker)delegate ()
                    {
                        LabelDoorState.Text = "Opened";
                    });
                }
                else
                {
                    LabelDoorState.Text = "Opened";
                }
            }

            if (isRunning == false && isRunningSingle == false && isRunningManual == false)
            {
                if (btnRunningStatus.InvokeRequired == true)
                {
                    this.btnRunningStatus.Invoke((MethodInvoker)delegate ()
                    {
                        btnRunningStatus.Enabled = false;
                    });
                }
            }
            else
            {
                if (btnRunningStatus.InvokeRequired == true)
                {
                    this.btnRunningStatus.Invoke((MethodInvoker)delegate ()
                    {
                        btnRunningStatus.Enabled = true;
                    });
                }
            }
        }

        private void LabelStateUpdate()
        {
            if (bStateMonitorTh == false || bShudown == true)
                return;
            
            if (label_elaplsed_time.Enabled == true)
            {
                if (label_elaplsed_time.InvokeRequired == true)
                {
                    this.label_elaplsed_time.Invoke((MethodInvoker)delegate
                    {
                        if (stopwatch.IsRunning)
                            label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                    });
                }
                else
                {
                    if (stopwatch.IsRunning)
                        label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                }
            }

            if (label_rpm.InvokeRequired == true)
            {
                this.label_rpm.Invoke((MethodInvoker)delegate
                {
                    label_rpm.Text = ServoMon.nCurrServoRpm.ToString();
                });
            }
            else
            {
                label_rpm.Text = ServoMon.nCurrServoRpm.ToString();
            }

            if (m_currentRcf.InvokeRequired == true)
            {
                this.m_currentRcf.Invoke((MethodInvoker)delegate
                {
                    m_currentRcf.Text = ServoMon.dbCurrServoRcf.ToString();
                });
            }
            else
            {
                m_currentRcf.Text = ServoMon.dbCurrServoRcf.ToString();
            }

            // instrument tab state
            if (TabItem == "Instrument")
            {
                if (editSetTemp.InvokeRequired == true)
                {
                    this.editSetTemp.Invoke((MethodInvoker)delegate
                    {
                        editSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
                    });
                }
                else
                {
                    editSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
                }

                if (editTempChamber.InvokeRequired == true)
                {
                    this.editTempChamber.Invoke((MethodInvoker)delegate
                    {
                        editTempChamber.Text = PeltMon.dbTempChamber.ToString();
                    });
                }
                else
                {
                    editTempChamber.Text = PeltMon.dbTempChamber.ToString();
                }

                if (editTempPeltier.InvokeRequired == true)
                {
                    this.editTempPeltier.Invoke((MethodInvoker)delegate
                    {
                        editTempPeltier.Text = PeltMon.dbTempPeltier.ToString();
                    });
                }
                else
                {
                    editTempPeltier.Text = PeltMon.dbTempPeltier.ToString();
                }

                if (editTempCooler.InvokeRequired == true)
                {
                    this.editTempCooler.Invoke((MethodInvoker)delegate
                    {
                        editTempCooler.Text = PeltMon.dbTempCooler.ToString();
                    });
                }
                else
                {
                    editTempCooler.Text = PeltMon.dbTempCooler.ToString();
                }
            }

            // main state
            if (LabelSetTemp.InvokeRequired == true)
            {
                this.LabelSetTemp.Invoke((MethodInvoker)delegate
                {
                    LabelSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
                });
            }
            else
            {
                LabelSetTemp.Text = PeltMon.dbSetPeltTemp.ToString();
            }

            if (LabelTempChamber.InvokeRequired == true)
            {
                this.LabelTempChamber.Invoke((MethodInvoker)delegate
                {
                    LabelTempChamber.Text = PeltMon.dbTempChamber.ToString();
                });
            }
            else
            {
                LabelTempChamber.Text = PeltMon.dbTempChamber.ToString();
            }

            if (LabelTempPeltier.InvokeRequired == true)
            {
                this.LabelTempPeltier.Invoke((MethodInvoker)delegate
                {
                    LabelTempPeltier.Text = PeltMon.dbTempPeltier.ToString();
                });
            }
            else
            {
                LabelTempPeltier.Text = PeltMon.dbTempPeltier.ToString();
            }

            if (LabelTempCooler.InvokeRequired == true)
            {
                this.LabelTempCooler.Invoke((MethodInvoker)delegate
                {
                    LabelTempCooler.Text = PeltMon.dbTempCooler.ToString();
                });
            }
            else
            {
                LabelTempCooler.Text = PeltMon.dbTempCooler.ToString();
            }

            if (btnTimer.InvokeRequired == true)
            {
                this.btnTimer.Invoke((MethodInvoker)delegate
                {
                    if(bSerialTimerState == true)
                        btnTimer.Text = "Stop Timer";
                    else
                        btnTimer.Text = "Start Timer";
                });
            }
            else
            {
                if (bSerialTimerState == true)
                    btnTimer.Text = "Stop Timer";
                else
                    btnTimer.Text = "Start Timer";
            }
        }

        public void CurStateRead()
        {
            try
            {
                while (bStateMonitorTh)
                {
                    //if (Serial.IsOpen && bCommunicationActive != true)
                    if (Serial.IsOpen)
                    {
                        lock (sync)
                        {
                            Thread.Sleep(200);

                            if (bStateMonitorTh == false)
                                break;

                            SwitchStateDisplay();
                            LabelStateUpdate();

                            Thread.Sleep(200);

                            if (bStateMonitorTh == false)
                                break;

                            PositionDataDisplay();

                            // 모터 정지 후 타이머 정지를 위해 3초를 추가함
                            if (bServoRunState == true && SerialTimerCnt > (SpinTotalTime + 3))
                            {
                                if (bSerialTimerState == true)
                                {
                                    timer_com.Stop();
                                    bSerialTimerState = false;

                                    iPrintf(string.Format("[Servo] Stop Serial Tick Timer! {0} / {1}", 
                                                          SerialTimerCnt, SpinTotalTime + 3));   // for test

                                    ReadMotorPosition(true, bSilent: true);
                                }

                                if (bServoRunState == true)
                                    bServoRunState = false;

                                if (isRunningManual == true)
                                    isRunningManual = false;
                            }

                            // 근접센서 카운트 값이 임계치를 넘었을 때 Servo Off 처리함
                            if(bServoRunState == true && SensorStatus.ErrEccentricCnt == true)
                            {
                                iPrintf(string.Format("Servo Off! Eccentric Sensor Count Exceed! Count: {0}, Threshold: {1}", 
                                        nEccentricCnt, nEccentricThreshold));
                                DisplayStatusMessage("Servo Off! Eccentric Sensor Count Exceed!", TEST.FAIL);

                                ServoOnOff(HOLD_STATE.FREE);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                iPrintf(ex.Message);
            }
            finally
            {
                iPrintf("Cleaning state thread resources..");
            }
        }

        static object sync = new object();

        public bool bStep0Move_old = false; public bool bStep1Move_old = false; public bool bStep2Move_old = false;
        public bool bStepGripMove_old = false; public bool bStepHamMove_old = false; public bool bStepCoverMove_old = false;

        //public double dbCurPosCover_old = 0;
        //public double diffPosCover = 0;
        //public double diffPosCover_old = 0;

        public double dbCurPosHam_old = 0; public double dbCurPosGrip_old = 0;
        public double diffPosHam = 0; public double diffPosGrip = 0;
        public double diffPosHam_old = 0; public double diffPosGrip_old = 0;  

        //public double dbCurPosX_old = 0; public double dbCurPosY_old = 0; public double dbCurPosZ_old = 0;
        //public double diffPosX = 0; public double diffPosY = 0; public double diffPosZ = 0;
        //public double diffPosX_old = 0; public double diffPosY_old = 0; public double diffPosZ_old = 0;

        public void CurPosRead()
        {
            try
            {
                while (bPosMonitorTh)
                {
                    if (Serial.IsOpen && bCommunicationActive != true && nRcvBuffCnt == 0)
                    {
                        lock (sync)
                        {
                            if (bPosReadStart == true)
                            {
                                if (bPosMonitorTh == false)
                                {
                                    if (bPosTimerState == true)
                                    {
                                        if (bSerialTimerState == true)
                                            btnTimer_Click(this, null);
                                        bPosTimerState = false;
                                        iPrintf("Thread Exit! Pos Monitor Stop!");
                                    }

                                    return;
                                }
                                
                                // Recipe가 구동 중일 때는 step 모터의 위치값 모니터링을 중단함. liquid detection에서는 예외처리 필요함
                                if (bMotionDoneWait == false && 
                                   (isRunning == false && isRunningSingle == false && isRunningManual == false))
                                {
                                    PosThreadMethod();
                                }
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
            }
            catch(ThreadAbortException ex)
            {
                iPrintf(ex.Message);
            }
            finally
            {
                iPrintf("Cleaning pos thread resources..");
            }
        }

        public void PosThreadMethod()
        {
            // pipett 축의 위치값이 빠르게 수렴하지 않아서 in-position 값을 설정하여 운영함
            double in_position = 0.3;

            if (bAxisStartFlag[0] == true || bAxisStartFlag[1] == true || bAxisStartFlag[2] == true ||
                bAxisStartFlag[3] == true || bAxisStartFlag[4] == true || bAxisStartFlag[5] == true)
            {
                if (bPosTimerState == false)
                {
                    if (bMotionDoneWait != true)
                    {
                        Thread.Sleep(50);   //100
                        MonitorStepMotorStatus();
                        Thread.Sleep(100);
                    }
                    if (isRunning == false && isRunningSingle == false && isRunningManual == false)
                    {
                        if (dbMovingDist > 2)
                        {
                            if (bSerialTimerState == false)
                                btnTimer_Click(this, null);
                            bPosTimerState = true;
                            iPrintf("Pos Monitor Start!");
                        }
                    }
                }

                if (bAxisStartFlag[0] == true) bAxisStartFlag[0] = false;
                if (bAxisStartFlag[1] == true) bAxisStartFlag[1] = false;
                if (bAxisStartFlag[2] == true) bAxisStartFlag[2] = false;
                if (bAxisStartFlag[3] == true) bAxisStartFlag[3] = false;
                if (bAxisStartFlag[4] == true) bAxisStartFlag[4] = false;
                if (bAxisStartFlag[5] == true) bAxisStartFlag[5] = false;
            }

            //diffPosX = Math.Abs(CurrentPos.Step0AxisX - dbCurPosX_old);
            //diffPosY = Math.Abs(CurrentPos.Step1AxisY - dbCurPosY_old);
            //diffPosZ = Math.Abs(CurrentPos.Step2AxisZ - dbCurPosZ_old);

            diffPosHam   = Math.Abs(CurrentPos.StepHamAxis  - dbCurPosHam_old);
            diffPosGrip  = Math.Abs(CurrentPos.StepGripAxis - dbCurPosGrip_old);
            //diffPosCover = Math.Abs(CurrentPos.StepRotCover - dbCurPosCover_old);

            if (bPosMonitorTh == false || Serial.IsOpen == false || bStepMotorInitDoneState == false)
            {
                if(bPosTimerState == true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                    bPosTimerState = false;
                    iPrintf("Thread Exit! Pos Monitor Stop!");
                }
                
                return;
            }

            Thread.Sleep(50);

            if ((isRunning == false && isRunningSingle == false && isRunningManual == false) && bStepRunState == true)
            {
                if ((bAxisMovingFlag[0] == true || bAxisMovingFlag[1] == true || bAxisMovingFlag[2] == true ||
                     bAxisMovingFlag[3] == true || bAxisMovingFlag[4] == true || bAxisMovingFlag[5] == true) &&
                   ((Step0AxState.bMOVE == false && bStep0Move_old == true) ||
                    (Step1AxState.bMOVE == false && bStep1Move_old == true) ||
                    (Step2AxState.bMOVE == false && bStep2Move_old == true) ||
                    //((diffPosHam >= 0 && diffPosHam < in_position) && diffPosHam_old > 0) ||
                    //((diffPosGrip >= 0 && diffPosGrip < in_position) && diffPosGrip_old > 0) ||
                    (GripAxState.bMove == false && bStepGripMove_old == true) ||
                    (HamAxState.bMove == false && bStepHamMove_old == true) ||
                    (CoverAxState.bMove == false && bStepCoverMove_old == true)))
                {
                    Thread.Sleep(50);   //200

                    if (bPosTimerState == true)
                    {
                        //Thread.Sleep(50); //200     // interval을 고려해야 함
                        bPosTimerState = false;
                        if (bSerialTimerState == true)
                            btnTimer_Click(this, null);
                        iPrintf("Pos Monitor Stop!");
                    }

                    if (bAxisMovingFlag[0] == true)  bAxisMovingFlag[0] = false;
                    if (bAxisMovingFlag[1] == true)  bAxisMovingFlag[1] = false;
                    if (bAxisMovingFlag[2] == true)  bAxisMovingFlag[2] = false;
                    if (bAxisMovingFlag[3] == true)  bAxisMovingFlag[3] = false;
                    if (bAxisMovingFlag[4] == true)  bAxisMovingFlag[4] = false;
                    if (bAxisMovingFlag[5] == true)  bAxisMovingFlag[5] = false;

                    dbMovingDist = 0;
                    //diffPosX = 0;       diffPosX_old = 0;
                    //diffPosY = 0;       diffPosY_old = 0;
                    //diffPosZ = 0;       diffPosZ_old = 0;
                    diffPosHam = 0;     diffPosHam_old = 0;
                    diffPosGrip = 0;    diffPosGrip_old = 0;
                    //diffPosCover = 0;   diffPosCover_old = 0;
                    bStep0Move_old = false;
                    bStep1Move_old = false;
                    bStep2Move_old = false;
                    bStepGripMove_old = false;
                    bStepHamMove_old = false;
                    bStepCoverMove_old = false;

                    Thread.Sleep(100);  //100
                    MonitorStepMotorStatus();

                    iPrintf("Set Step Run Flag Low!");
                    bStepRunState = false;

                    return;
                }
            }
            
            if ((isRunning == true || isRunningSingle == true || isRunningManual == true) && bStepRunState == true)
            {
                if (bAxisMovingFlag[0] == true && (Step0AxState.bMOVE == false && bStep0Move_old == true))
                {
                    //iPrintf(string.Format("Step0: {0}, Step0_old:{1}", Step0AxState.bMOVE, bStep0Move_old));
                    bAxisMovingFlag[0] = false;
                }
                else if(bAxisMovingFlag[0] == true && (Step0AxState.bMOVE == false && bStep0Move_old == false))
                {
                    bAxisMovingFlag[0] = false;
                }

                if (bAxisMovingFlag[1] == true && (Step1AxState.bMOVE == false && bStep1Move_old == true))
                {
                    //iPrintf(string.Format("Step1: {0}, Step1_old:{1}", Step1AxState.bMOVE, bStep1Move_old));
                    bAxisMovingFlag[1] = false;
                }
                else if (bAxisMovingFlag[1] == true && (Step1AxState.bMOVE == false && bStep1Move_old == false))
                {
                    bAxisMovingFlag[1] = false;
                }

                if (bAxisMovingFlag[2] == true && (Step2AxState.bMOVE == false && bStep2Move_old == true))
                {
                    //iPrintf(string.Format("Step2: {0}, Step2_old:{1}", Step2AxState.bMOVE, bStep2Move_old));
                    bAxisMovingFlag[2] = false;
                }
                else if (bAxisMovingFlag[2] == true && (Step2AxState.bMOVE == false && bStep2Move_old == false))
                {
                    bAxisMovingFlag[2] = false;
                }

                //if (bAxisMovingFlag[3] == true && ((diffPosGrip >= 0 && diffPosGrip < in_position) && diffPosGrip_old > 0))
                if (bAxisMovingFlag[3] == true && (GripAxState.bMove == false && bStepGripMove_old == true))
                {
                    //iPrintf(string.Format("Grip_diff:{0}, Grip_diff_old:{1}", diffPosGrip, diffPosGrip_old));
                    bAxisMovingFlag[3] = false;
                }
                else if (bAxisMovingFlag[3] == true && (GripAxState.bMove == false && bStepGripMove_old == false))
                {
                    bAxisMovingFlag[3] = false;
                }

                //if (bAxisMovingFlag[4] == true && (HamAxState.bMove == false && bStepHamMove_old == true))
                if (bAxisMovingFlag[4] == true && ((diffPosHam >= 0 && diffPosHam < in_position) && diffPosHam_old > 0))
                {
                    //iPrintf(string.Format("Ham_diff: {0}, Ham_diff_old:{1}", diffPosHam, diffPosHam_old));
                    bAxisMovingFlag[4] = false;
                }
                else if (bAxisMovingFlag[4] == true && (HamAxState.bMove == false && bStepHamMove_old == false))
                {
                    bAxisMovingFlag[4] = false;
                }

                if (bAxisMovingFlag[5] == true && (CoverAxState.bMove == false && bStepCoverMove_old == true))
                {
                    //iPrintf(string.Format("Cover_diff:{0}, Cover_diff_old:{1}", diffPosCover, diffPosCover_old));
                    bAxisMovingFlag[5] = false;
                }
                else if (bAxisMovingFlag[5] == true && (CoverAxState.bMove == false && bStepCoverMove_old == false))
                {
                    bAxisMovingFlag[5] = false;
                }

                if (bAxisMovingFlag[0] == false && bAxisMovingFlag[1] == false && bAxisMovingFlag[2] == false &&
                    bAxisMovingFlag[3] == false && bAxisMovingFlag[4] == false && bAxisMovingFlag[5] == false)
                {
                    Thread.Sleep(50);   //50   //200

                    if (bPosTimerState == true)
                    {
                        //Thread.Sleep(50); //200     // interval을 고려해야 함
                        bPosTimerState = false;
                        if (bSerialTimerState == true)
                            btnTimer_Click(this, null);
                        iPrintf("Pos Monitor Stop!");
                        //PosTimerCnt = 0;
                    }

                    dbMovingDist = 0;
                    //diffPosX = 0;     diffPosX_old = 0;
                    //diffPosY = 0;     diffPosY_old = 0;
                    //diffPosZ = 0;     diffPosZ_old = 0;
                    diffPosHam = 0;     diffPosHam_old = 0;
                    diffPosGrip = 0;    diffPosGrip_old = 0;
                    //diffPosCover = 0; diffPosCover_old = 0;
                    bStep0Move_old = false;     bStep1Move_old = false;
                    bStep2Move_old = false;     bStepGripMove_old = false;
                    bStepHamMove_old = false;   bStepCoverMove_old = false;

                    Thread.Sleep(100);  //100

                    iPrintf("[Recipe] Set Step Run Flag Low!");
                    bStepRunState = false;

                    return;
                }
            }


            if (bPosMonitorTh == false || Serial.IsOpen == false || bStepMotorInitDoneState == false)
            {
                if (bPosTimerState == true)
                {
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                    bPosTimerState = false;
                    iPrintf("Thread Exit! Pos Monitor Stop!");
                }

                return;
            }

            bStep0Move_old = Step0AxState.bMOVE;    bStep1Move_old = Step1AxState.bMOVE;
            bStep2Move_old = Step2AxState.bMOVE;    bStepGripMove_old = GripAxState.bMove;
            bStepHamMove_old = HamAxState.bMove;    bStepCoverMove_old = CoverAxState.bMove;

            //dbCurPosX_old = CurrentPos.Step0AxisX;    dbCurPosY_old = CurrentPos.Step1AxisY;
            //dbCurPosZ_old = CurrentPos.Step2AxisZ;    diffPosX_old = diffPosX;
            //diffPosY_old = diffPosY;                  diffPosZ_old = diffPosZ;

            dbCurPosHam_old = CurrentPos.StepHamAxis;   dbCurPosGrip_old = CurrentPos.StepGripAxis;
            //dbCurPosCover_old = CurrentPos.StepRotCover;
            diffPosHam_old = diffPosHam;                diffPosGrip_old = diffPosGrip;
            //diffPosCover_old = diffPosCover;
        }

        public void MonitorStepMotorStatus()
        {
            if (ReceiveFrameFlag == false && bCommunicationActive == false && nRcvBuffCnt == 0)
            {
                GetStatus(waitReceive: false, bSilent: true);
                Thread.Sleep(200);   //200
                ReadMotorPosition(waitReceive: false, bSilent: true);
                Thread.Sleep(100);   //100
            }
        }

        // driven by timer_com
        private void Serial_TickTimer(object sender, ElapsedEventArgs e)
        {
            SerialTimerCnt++;
            SerialTimerLimit = 3600 * 2 * (float.Parse(tbTimerInterval.Text) / 1000);   // 2 hours (timer 1초 기준)
            if (SerialTimerCnt >= SerialTimerLimit)
                SerialTimerCnt = 0;

            if (Serial.IsOpen /*&& !holdTimer*/ && !bDirectReceive)
            {
                if ((bStepRunState == true || bServoRunState == true) && 
                    ReceiveFrameFlag == false && bCommunicationActive == false && nRcvBuffCnt == 0 && bMotionDoneWait == false)
                {
                    Thread.Sleep(150);
                    iPrintf("[Timer] GetStatus");
                    GetStatus(waitReceive: false, bSilent: true);
                    Thread.Sleep(50);
                }

                if (bPeltRunState == true &&
                    ReceiveFrameFlag == false && bCommunicationActive == false && nRcvBuffCnt == 0 && bMotionDoneWait == false)
                {
                    if (isRunning == false && isRunningSingle == false && isRunningManual == false)
                    {
                        Thread.Sleep(150);
                        iPrintf("[Timer] Read Peltier");
                        ReadPeltierTemp();
                        Thread.Sleep(50);
                    }
                }

                if (bStepRunState == true && 
                    ReceiveFrameFlag == false && bCommunicationActive == false && nRcvBuffCnt == 0 && bMotionDoneWait == false)
                {
                    Thread.Sleep(150);
                    iPrintf("[Timer] Read Motor Pos");
                    ReadMotorPosition(true, bSilent: true);
                    Thread.Sleep(50);
                }

                if (bServoRunState == true && 
                    ReceiveFrameFlag == false && bCommunicationActive == false && nRcvBuffCnt == 0 && bMotionDoneWait == false)
                {
                    Thread.Sleep(150);
                    iPrintf("[Timer] Read Servo RPM");
                    ServoMonitor(MotorMon.RPM, bSilent: true);
                    Thread.Sleep(50);
                }
            }
            //iPrintf(string.Format("RcvFlag: {0}, ComActive: {1}, RcvBuff: {2}, MotionDone: {3}, DirectRcv: {4}",
            //            ReceiveFrameFlag, bCommunicationActive, nRcvBuffCnt, bMotionDoneWait, bDirectReceive));
        }

        // driven by timer_pos
        // 통신 송수신 간 충돌로 인해 timer_com으로 통합하여 구현 (21.09.29)
        private void Pos_TickTimer(object sender, ElapsedEventArgs e)
        {
            if (bAxisMovingFlag[0] == true || bAxisMovingFlag[1] == true || bAxisMovingFlag[2] == true ||
                bAxisMovingFlag[3] == true || bAxisMovingFlag[4] == true || bAxisMovingFlag[5] == true)
            {
                PosTimerCnt++;
            }

            PosTimerLimit = 60 * 2;     // 2 min.(timer 1초 기준)

            if (Serial.IsOpen /*&& !holdTimer*/ && !bDirectReceive)
            {
                bPosTimerRun = true;
                Thread.Sleep(100);

                MonitorStepMotorStatus();

                bPosTimerRun = false;
            }
        }

        private void Video_TickTimer(object sender, ElapsedEventArgs e)
        {
            timer_video.Enabled = false;

            try
            {
                if (isScrolled_Video == false)
                {
                    if (cvCapture == null)
                    {
                        timer_video.Enabled = true;

                        return;
                    }

                    if (VideoPlayButtonStatus == MEDIA_PLAY)
                    {
                        timer_video.Enabled = true;

                        return;
                    }

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        if (cvCapture.PosFrames >= 0)
                        {
                            TrackBar_Video.Value = cvCapture.PosFrames;
                            UpdateVideoPlayingTime();
                        }
                        else
                            TrackBar_Video.Value = cvCapture.PosFrames = 0;
                    }));

                    if (cvCapture.FrameCount <= cvCapture.PosFrames)
                    {
                        timer_video.Enabled = false;
                        if (VideoPlayButtonStatus == MEDIA_PAUSE)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PLAY;
                            }));
                        }
                        timer_video.Enabled = true;
                        return;
                    }

                    cvCapture.Read(src);

                    if (src == null)
                    {
                        timer_video.Enabled = false;
                    }
                    else
                    {
                        pictureBox1.Image = src.ToBitmap();
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
                timer_video.Enabled = false;
            }
            timer_video.Enabled = true;

        }

        private void FrameWatcher_TickTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                CheckIndexChange();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }

        }

        private void CreateFolder()
        {
            if (!Directory.Exists(DIR_HOME))
            {
                System.IO.Directory.CreateDirectory(DIR_HOME);
            }
            if (!Directory.Exists(DIR_VIDEO))
            {
                System.IO.Directory.CreateDirectory(DIR_VIDEO);
            }
            if (!Directory.Exists(DIR_VIDEO_LOG))
            {
                System.IO.Directory.CreateDirectory(DIR_VIDEO_LOG);
            }
            if (!Directory.Exists(DIR_IMAGE))
            {
                System.IO.Directory.CreateDirectory(DIR_IMAGE);
            }
            if (!File.Exists(DIR_MUSIC))
            {
                System.IO.Directory.CreateDirectory(DIR_MUSIC);
            }
            if (!File.Exists(DIR_RECIPE))
            {
                System.IO.Directory.CreateDirectory(DIR_RECIPE);
            }
            if (!File.Exists(DIR_LOG))
            {
                System.IO.Directory.CreateDirectory(DIR_LOG);
            }
        }

        private void SetupDatagridView()
        {
            DV_Recipe.Height = 205;
            int Width = 0;

            for (int i = 0; i < DV_Recipe.Columns.Count - 1; i++)
            {
                Width += DV_Recipe.Columns[i].Width;
            }
            
            DV_Recipe.Columns[DV_Recipe.Columns.Count - 1].Width = DV_Recipe.Width - Width - 30;

            if(DV_Recipe != null && DV_Recipe.SelectedRows.Count >= 1 && DV_Recipe.SelectedRows[0].Index != DV_Recipe.RowCount)
            {
                DV_Recipe.CellValidating += new DataGridViewCellValidatingEventHandler(DV_Recipe_CellValidating);
            }                
            DV_Recipe.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(DV_Recipe_EditingControlShowing);
        }

        private void DV_Recipe_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);

            if (DV_Recipe.CurrentCell.ColumnIndex > 1) //Desired Column
            {
                TextBox tb = e.Control as TextBox;

                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DV_Recipe_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                //if (e.RowIndex > 0)
                //    return;

                if (e.ColumnIndex == (int)Recipe_COL.Param1)
                {
                    //int value = int.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param2)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }

                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param3)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param4)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param5)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param6)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (e.ColumnIndex == (int)Recipe_COL.Param7)
                {
                    try
                    {
                        float value = float.Parse(DV_Recipe.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                string log = ex.Message.ToString();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.P))
            {

                return true;
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.B))
            {
                config.bDebugMode = config.bDebugMode ? false : true;
                if (config.bDebugMode)
                {
                    DebugStatus.Show();
                    EnableControlsManualTest(true, false);
                }
                else
                    DebugStatus.Hide();

                Application.DoEvents();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.D1))
            {
                SelectTheme();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.K))
            {
                if (bRunningThread)
                    bRunningThread = false;
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.R))
            {

            }
            else if (keyData == (Keys.Control | Keys.Alt | Keys.Shift | Keys.D2))
            {
                colorSchemeIndex++;
                if (colorSchemeIndex > 17) colorSchemeIndex = 0;

                SelectColorScheme(colorSchemeIndex);
                return true;
            }
            else if (keyData == (Keys.F1))
            {
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D0))
            {                
                btnRecipeTest_Click(this, null);
            }
            else if (keyData == (Keys.Control | Keys.D1))
            {
                GetRecipeFromListView(0);
                if (SelectButton(0))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D2))
            {
                GetRecipeFromListView(1);
                if (SelectButton(1))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D3))
            {
                GetRecipeFromListView(2);
                if (SelectButton(2))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D4))
            {
                GetRecipeFromListView(3);
                if (SelectButton(3))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D5))
            {
                GetRecipeFromListView(4);
                if (SelectButton(4))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D6))
            {
                GetRecipeFromListView(5);
                if (SelectButton(5))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D7))
            {
                GetRecipeFromListView(6);
                if (SelectButton(6))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D8))
            {
                GetRecipeFromListView(7);
                if (SelectButton(7))
                    RunRecipeTable();
            }
            else if (keyData == (Keys.Control | Keys.D9))
            {
                GetRecipeFromListView(8);
                if (SelectButton(8))
                    RunRecipeTable();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoggerConfig()
        {
            try
            {
                LogManager.LoadConfiguration(DIR_HOME + "\\nlog.config");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SelectTheme(int theme = -1)
        {
            if (theme == -1)
                materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? MaterialSkinManager.Themes.LIGHT : MaterialSkinManager.Themes.DARK;
            else if (theme == 0)
                materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            else
                materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            gb_config.BackColor = materialSkinManager.GetApplicationBackgroundColor();
            gb_config.ForeColor = materialSkinManager.GetPrimaryTextColor();
            SerialMessageBox.BackColor = materialSkinManager.GetApplicationBackgroundColor();
            SerialMessageBox.ForeColor = materialSkinManager.GetPrimaryTextColor();

            config.Theme = (int)materialSkinManager.Theme;
            materialSkinManager.GetFlatButtonPressedBackgroundColor();
        }

        private int colorSchemeIndex;

        private void SelectColorScheme(int schemeIndex)
        {
            switch (schemeIndex)
            {
                case 0:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.BlueGrey800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 1:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900, Primary.Red200, Accent.Green200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Red800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 2:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink800, Primary.Pink900, Primary.Pink200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Pink800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 3:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Purple800, Primary.Purple900, Primary.Purple200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Purple800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 4:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepPurple800, Primary.DeepPurple900, Primary.DeepPurple200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.DeepPurple800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 5:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Indigo500);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 6:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue900, Primary.Blue200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Blue800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 7:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue800, Primary.LightBlue900, Primary.LightBlue200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.LightBlue800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 8:                   
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan800, Primary.Cyan900, Primary.Cyan200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Cyan800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 9:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal600, Primary.Teal700, Primary.Teal200, Accent.Pink200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Teal600);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 10:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.Red100, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Green600);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 11:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.LightGreen600, Primary.LightGreen700, Primary.LightGreen200, Accent.Red100, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.LightGreen600);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 12:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Lime600, Primary.Lime700, Primary.Lime200, Accent.Green200, TextShade.BLACK);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Lime600);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 13:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Yellow800, Primary.Yellow900, Primary.Yellow200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Yellow800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 14:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Amber600, Primary.Amber700, Primary.Amber200, Accent.Green200, TextShade.BLACK);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Amber600);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;

                case 15:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange500, Primary.Orange700, Primary.Orange200, Accent.Green200, TextShade.BLACK);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Orange500);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;

                case 16:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepOrange500, Primary.DeepOrange700, Primary.DeepOrange200, Accent.Green200, TextShade.BLACK);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.DeepOrange500);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 17:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Brown700, Primary.Brown900, Primary.Brown200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Brown700);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 18:
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey200, Accent.Red200, TextShade.WHITE);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)Primary.Grey800);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
                case 19:
                    materialSkinManager.ColorScheme = new ColorScheme((Primary)0xBA55D3, (Primary)0x9400D3, Primary.Orange200, Accent.Green200, TextShade.BLACK);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisX.ColumnHeadersDefaultCellStyle.BackColor = DV_AxisY.ColumnHeadersDefaultCellStyle.BackColor = DV_Recipe.ColumnHeadersDefaultCellStyle.BackColor = ColorExtension.ToColor((int)0xBA55D3);
                    DV_World_T_Pnt.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisPipett.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisZ.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisGripper.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisX.ColumnHeadersDefaultCellStyle.ForeColor = DV_AxisY.ColumnHeadersDefaultCellStyle.ForeColor = DV_Recipe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    break;
            }

            ButtonSelectColor = materialSkinManager.ColorScheme.AccentColor;
            config.ColorScheme = colorSchemeIndex = schemeIndex;
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        private void UpdateConfig(RW rw)
        {
            try
            {
                if (rw == RW.READ)
                {
                    config.ReadWriteLastButtonfile(rw);
                    config.ReadWriteConfig(rw);
                    UpdateControlValue(GetSet.SET);
                }
                else if (rw == RW.WRITE)
                {
                    UpdateControlValue(GetSet.GET);
                    config.ReadWriteConfig(rw);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(ex.Message);
            }
        }

        public void ReadWriteButtonConfig(RW rw, string fileName)
        {
            try
            {
                string PathButtonConfig = config.LastButtonFileName = fileName;
                lblButtonFilename.Text = Path.GetFileName(fileName);
                if (rw == RW.READ)
                {
                    if (!File.Exists(PathButtonConfig))
                    {
                        MessageBox.Show(string.Format("{0} file not found", PathButtonConfig), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        ListButtonRecipe.Clear();
                        tableLayout_button.Controls.Clear();
                        tableLayout_button.ColumnCount = 0;
                        ButtonCount = config.Read("DEFINE_BUTTONS", "COUNT", 0, PathButtonConfig);
                        DefineButtons.Clear();
                        for (int i = 0; i < ButtonCount; i++)
                        {
                            Button btn = CreateButton();
                            btn.Name = config.Read($"BUTTON_{i + 1}", "NAME", "", PathButtonConfig);
                            btn.Text = config.Read($"BUTTON_{i + 1}", "TEXT", "", PathButtonConfig);
                            btn.AccessibleName = config.Read($"BUTTON_{i + 1}", "FILE_NAME", "", PathButtonConfig);
                            btn.AccessibleDescription = config.Read($"BUTTON_{i + 1}", "DESCRIPTION", "", PathButtonConfig);
                            btn.Enabled = config.Read($"BUTTON_{i + 1}", "ENABLE", "", PathButtonConfig).ToUpper() == "TRUE" ? true : false;

                            AddButtonRecipe(btn);

                            tableLayout_button.ColumnCount = DefineButtons.Count;
                            tableLayout_button.Controls.Add(btn, DefineButtons.Count - 1, 0);
                        }
                        ResizeButtons();
                    }
                }
                else if (rw == RW.WRITE)
                {
                    ButtonCount = DefineButtons.Count;
                    config.Write("DEFINE_BUTTONS", "COUNT", ButtonCount, PathButtonConfig);
                    for (int i = 0; i < ButtonCount; i++)
                    {
                        config.Write($"BUTTON_{i + 1}", "NAME", DefineButtons[i].Name, PathButtonConfig);
                        config.Write($"BUTTON_{i + 1}", "TEXT", DefineButtons[i].Text, PathButtonConfig);
                        config.Write($"BUTTON_{i + 1}", "FILE_NAME", DefineButtons[i].AccessibleName, PathButtonConfig);
                        config.Write($"BUTTON_{i + 1}", "DESCRIPTION", DefineButtons[i].AccessibleDescription, PathButtonConfig);
                        config.Write($"BUTTON_{i + 1}", "ENABLE", DefineButtons[i].Enabled.ToString(), PathButtonConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(ex.Message);
            }
        }

        public void AddButtonRecipe(Button button)
        {
            DefineButtons.Add(button);

            BUTTON_RECIPE btnRecipe = new BUTTON_RECIPE();
            btnRecipe.button = button;
            config.ReadWriteRecipe(RW.READ, button.AccessibleName, ref btnRecipe.recipe);
            ListButtonRecipe.Add(btnRecipe);
        }

        public void InsertButtonRecipe(int index, Button button)
        {
            DefineButtons.Insert(index, button);

            BUTTON_RECIPE btnRecipe = new BUTTON_RECIPE();
            btnRecipe.button = button;
            config.ReadWriteRecipe(RW.READ, button.AccessibleName, ref btnRecipe.recipe);
            ListButtonRecipe.Insert(index, btnRecipe);
        }

        public void RemoveAtButtonRecipe(int index)
        {
            DefineButtons.RemoveAt(index);
            ListButtonRecipe.RemoveAt(index);
        }

        public void RemoveButtonRecipe(Button button)
        {
            int index = DefineButtons.IndexOf(button);
            DefineButtons.RemoveAt(index);            
            ListButtonRecipe.RemoveAt(index);
        }

        public void UpdateDefineButtons(GetSet getset)
        {
            try
            {
                if (getset == GetSet.SET)
                {
                    tableLayout_button.Controls.Clear();
                    tableLayout_button.ColumnCount = DefineButtons.Count;
                    foreach (Button btn in DefineButtons)
                    {
                        SetDefineButton(DefineButtons.IndexOf(btn));
                        tableLayout_button.Controls.Add(btn);
                    }
                    ResizeButtons();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void UpdateControlValue(GetSet getset)
        {
            try
            {
                if (getset == GetSet.SET) // Set Control
                {
                    editCommPorts.Text = config.ComPort;
                    tbTimerInterval.Text = config.TimerInterval.ToString();
                    edit_rpm.Text = config.SpinRpm.ToString();
                    edit_prescale.Text = config.prescale.ToString();
                    edit_servo_acc.Text = config.risingTime.ToString();
                    edit_servo_dec.Text = config.fallingTime.ToString();
                    editRecipeLineDelay.Text = config.RecipeLineDelay.ToString();
                    edit_r.Text = config.RotorRadius.ToString();
                    edit_servo_duration.Text = config.strSpinDuration.ToString();
                    edit_rpm_offset.Text = config.SpinRpm_Offset.ToString();

                    this.Top = config.FormTop;
                    this.Left = config.FormLeft;
                    this.Width = config.FormWidth;
                    this.Height = config.FormHeight;
                    this.TopMost = config.TopMost;

                    TopMost = chk_TopMost.Checked = config.TopMost;

                    if (config.VisionTriggerMode == 1)
                        radioTriggerMode.Checked = true;
                    else
                        radioContinuesMode.Checked = true;
                    tbExposure1.Text = config.ExposureTime1;
                    tbGamma1.Text = config.Gamma1;
                    tbGain1.Text = config.VisionGain1;
                    tbFrameRate1.Text = config.FrameRate1;

                    tbExposure2.Text = config.ExposureTime2;
                    tbGamma2.Text = config.Gamma2;
                    tbGain2.Text = config.VisionGain2;
                    tbFrameRate2.Text = config.FrameRate2;

                    tbRecordFps.Text = config.RecordFrameRate;

                    chk_play_music_on_test.Checked = config.PlayMusicOntest;

                    tbConfigMaxVideoFile.Text = config.MaxVideoFileNumber.ToString();

                    editStepAxisX_Pos.Text = config.StepAxisX_Pos;
                    editStepAxisY_Pos.Text = config.StepAxisY_Pos;
                    editStepAxisZ_Pos.Text = config.StepAxisZ_Pos;
                    editStepGripper_Pos.Text = config.StepAxisGripper_Pos;
                    editStepPipett_Pos.Text = config.StepAxisPipett_Pos;
                    editCoverOpenPos.Text = config.CoverOpen_Pos.ToString();
                    editCoverClosePos.Text = config.CoverClose_Pos.ToString();

                    editStepAxisX_Speed.Text = config.StepAxisX_Speed.ToString();
                    editStepAxisY_Speed.Text = config.StepAxisY_Speed.ToString();
                    editStepAxisZ_Speed.Text = config.StepAxisZ_Speed.ToString();
                    editStepGripper_Speed.Text = config.StepAxisGripper_Speed.ToString();
                    editStepPipett_Speed.Text = config.StepAxisPipett_Speed.ToString();
                    editCoverOpenSpeed.Text = config.CoverOpen_Speed.ToString();
                    editCoverCloseSpeed.Text = config.CoverClose_Speed.ToString();
                    editStepCoverAcc.Text = config.StepCoverAcc.ToString();
                    editStepCoverDec.Text = config.StepCoverDec.ToString();

                    editStepAxisX_Acc.Text = config.StepAxisX_Acc.ToString();
                    editStepAxisY_Acc.Text = config.StepAxisY_Acc.ToString();
                    editStepAxisZ_Acc.Text = config.StepAxisZ_Acc.ToString();
                    editStepAxisGripper_Acc.Text = config.StepAxisGripper_Acc.ToString();
                    editStepAxisHam_Acc.Text = config.StepAxisPipett_Acc.ToString();
                    editStepCoverAcc.Text = config.RotorCover_Acc.ToString();

                    editStepAxisX_Dec.Text = config.StepAxisX_Dec.ToString();
                    editStepAxisY_Dec.Text = config.StepAxisY_Dec.ToString();
                    editStepAxisZ_Dec.Text = config.StepAxisZ_Dec.ToString();
                    editStepAxisGripper_Dec.Text = config.StepAxisGripper_Dec.ToString();
                    editStepAxisHam_Dec.Text = config.StepAxisPipett_Dec.ToString();
                    editStepCoverDec.Text = config.RotorCover_Dec.ToString();

                    editStepAxisX_Jog.Text = config.StepAxisX_Jog.ToString();
                    editStepAxisY_Jog.Text = config.StepAxisY_Jog.ToString();
                    editStepAxisZ_Jog.Text = config.StepAxisZ_Jog.ToString();
                    editStepGripper_Jog.Text = config.StepAxisGripper_Jog.ToString();
                    editStepPipett_Jog.Text = config.StepAxisPipett_Jog.ToString();
                                        
                    editFastMoveSpd_ZAxis.Text = config.FastMoveSpd_ZAxis.ToString();
                    editFastMoveSpd_GripAxis.Text = config.FastMoveSpd_GripAxis.ToString();
                    editFastMoveSpd_HamAxis.Text = config.FastMoveSpd_HamAxis.ToString();

                    editFastMovePos_ZAxis.Text = config.FastMovePos_ZAxis.ToString();
                    editFastMovePos_GripAxis.Text = config.FastMovePos_GripAxis.ToString();
                    editFastMovePos_HamAxis.Text = config.FastMovePos_HamAxis.ToString();

                    editHomeSearchSpd_X.Text = config.HomeSearchSpd_X.ToString();
                    editHomeSearchSpd_Y.Text = config.HomeSearchSpd_Y.ToString();
                    editHomeSearchSpd_Z.Text = config.HomeSearchSpd_Z.ToString();
                    editHomeSearchSpd_Grip.Text = config.HomeSearchSpd_Grip.ToString();
                    editHomeSearchSpd_Ham.Text = config.HomeSearchSpd_Ham.ToString();
                    editHomeSearchSpd_Cover.Text = config.HomeSearchSpd_Cover.ToString();
                    editHomeSearchSpd_Servo.Text = config.HomeSearchSpd_Servo.ToString();

                    editHomeOffsetPos_X.Text = config.HomeOffsetPos_X.ToString();
                    editHomeOffsetPos_Y.Text = config.HomeOffsetPos_Y.ToString();
                    editHomeOffsetPos_Z.Text = config.HomeOffsetPos_Z.ToString();
                    editHomeOffsetPos_Grip.Text = config.HomeOffsetPos_Grip.ToString();
                    editHomeOffsetPos_Ham.Text = config.HomeOffsetPos_Ham.ToString();
                    editHomeOffsetPos_Cover.Text = config.HomeOffsetPos_Cover.ToString();
                    editHomeOffsetPos_Servo.Text = config.HomeOffsetPos_Servo.ToString();

                    editTriPipettOffsetVolume.Text = config.TriPipett_OffsetVol.ToString();
                    editTriPipettLoadingVolume.Text = config.TriPipett_LoadingVol.ToString();
                    editTriPipettFlowRate.Text = config.TriPipett_FlowRate.ToString();

                    editPumpFlowRate.Text = config.PumpFlowRate.ToString();
                    editPumpLoadingVolume.Text = config.PumpLoadingVolume.ToString();
                    editPumpOffsetVolume.Text = config.PumpOffsetVolume.ToString();
                    editPumpPortNo.Text = config.PumpPortNo.ToString();
                    editValveDelay.Text = config.PumpValveDelay.ToString();

                    editFlowmeterCntUnit.Text = config.FlowmeterUnitScale.ToString();
                    editLoadcellErrWeight.Text = config.LoadcellErrWeight.ToString();

                    edit_pelt_set_temp.Text = config.Pelt_set_temp.ToString();
                    edit_fan_on_temp.Text = config.Fan_on_temp.ToString();
                    edit_fan_off_temp.Text = config.Fan_off_temp.ToString();

                    editRotorLightBright.Text = config.RotorLightBright.ToString();
                    editRoomLightBright.Text = config.RoomLightBright.ToString();

                    editEccentricThreshold.Text = config.Eccentric;
                    edit_strobe_period.Text = config.Strobe_period;

                    label_Tip_BasePosX.Text = config.TipBasePosX;
                    label_Tip_BasePosY.Text = config.TipBasePosY;
                    label_Tube_BasePosX.Text = config.TubeBasePosX;
                    label_Tube_BasePosY.Text = config.TubeBasePosY;
                    label_Cooling_BasePosX.Text = config.CoolingBasePosX;
                    label_Cooling_BasePosY.Text = config.CoolingBasePosY;

                    edit_Tip_OffsetX.Text = config.TipOffsetX;
                    edit_Tip_OffsetY.Text = config.TipOffsetY;
                    edit_Tube_OffsetX.Text = config.TubeOffsetX;
                    edit_Tube_OffsetY.Text = config.TubeOffsetY;
                    edit_Cooling_OffsetX.Text = config.CoolingOffsetX;
                    edit_Cooling_OffsetY.Text = config.CoolingOffsetY;

                    label_Tip_OrgPosX.Text = config.TipOrgPosX;
                    label_Tip_OrgPosY.Text = config.TipOrgPosY;
                    label_Tube_OrgPosX.Text = config.TubeOrgPosX;
                    label_Tube_OrgPosY.Text = config.TubeOrgPosY;
                    label_Cooling_OrgPosX.Text = config.CoolingOrgPosX;
                    label_Cooling_OrgPosY.Text = config.CoolingOrgPosY;

                    editPipett_offsetX_5ml.Text = config.Pipett_offsetX_5ml;
                    editPipett_offsetY_5ml.Text = config.Pipett_offsetY_5ml;
                    editPipett_offsetZ_5ml.Text = config.Pipett_offsetZ_5ml;
                    editPipett_offsetX_gripper.Text = config.Pipett_offsetX_gripper;
                    editPipett_offsetY_gripper.Text = config.Pipett_offsetY_gripper;
                    editPipett_offsetZ_gripper.Text = config.Pipett_offsetZ_gripper;
                    editPipett_offsetX_laser.Text = config.Pipett_offsetX_laser;
                    editPipett_offsetY_laser.Text = config.Pipett_offsetY_laser;
                    editPipett_offsetZ_laser.Text = config.Pipett_offsetZ_laser;
                    edit_laser_Z_dist.Text = config.laser_Z_dist;

                    editPipett_offsetX_1ml.Text = config.Pipett_offsetX_1ml;
                    editPipett_offsetY_1ml.Text = config.Pipett_offsetY_1ml;
                    editPipett_offsetZ_1ml.Text = config.Pipett_offsetZ_1ml;

                    editPipett_offsetZ_10ul.Text = config.Pipett_offsetZ_10ul;
                    editPipett_offsetZ_300ul.Text = config.Pipett_offsetZ_300ul;
                    editPipett_offsetZ_CalibTip.Text = config.Pipett_offsetZ_CalibTip;

                    RefreshRecipeDataView(GetSelectedButtonIndex());
                    SetAxisXDataView();
                    SetAxisYDataView();
                    SetAxisZDataView();
                    SetAxisGripperDataView();
                    SetAxisPipettDataView();
                    SetWorldTeachingPntDataView();
                    SetToolOffsetFromConfig();

                    nEccentricThreshold = int.Parse(editEccentricThreshold.Text);
                }
                else
                {
                    config.ComPort = editCommPorts.Text;
                    config.TimerInterval = int.Parse(tbTimerInterval.Text);
                    config.SpinRpm = int.Parse(edit_rpm.Text);
                    config.prescale = int.Parse(edit_prescale.Text);
                    config.risingTime = int.Parse(edit_servo_acc.Text);
                    config.fallingTime = int.Parse(edit_servo_dec.Text);
                    config.RecipeLineDelay = int.Parse(editRecipeLineDelay.Text);
                    config.RotorRadius = edit_r.Text;
                    config.strSpinDuration = edit_servo_duration.Text;
                    config.SpinRpm_Offset = edit_rpm_offset.Text;

                    config.FormTop = this.Top;
                    config.FormLeft = this.Left;
                    config.FormWidth = this.Width;
                    config.FormHeight = this.Height;

                    config.TopMost = this.TopMost;

                    config.VisionTriggerMode = radioTriggerMode.Checked ? 1 : 0;
                    config.ExposureTime1 = tbExposure1.Text;
                    config.Gamma1 = tbGamma1.Text;
                    config.VisionGain1 = tbGain1.Text;
                    config.FrameRate1 = tbFrameRate1.Text;
                    config.ExposureTime2 = tbExposure2.Text;
                    config.Gamma2 = tbGamma2.Text;
                    config.VisionGain2 = tbGain2.Text;
                    config.FrameRate2 = tbFrameRate2.Text;

                    config.RecordFrameRate = tbRecordFps.Text;

                    config.PlayMusicOntest = chk_play_music_on_test.Checked;
                    config.MaxVideoFileNumber = int.Parse(tbConfigMaxVideoFile.Text);

                    config.StepAxisX_Pos = editStepAxisX_Pos.Text;
                    config.StepAxisY_Pos = editStepAxisY_Pos.Text;
                    config.StepAxisZ_Pos = editStepAxisZ_Pos.Text;
                    config.StepAxisGripper_Pos = editStepGripper_Pos.Text;
                    config.StepAxisPipett_Pos = editStepPipett_Pos.Text;
                    config.CoverOpen_Pos = int.Parse(editCoverOpenPos.Text);
                    config.CoverClose_Pos = int.Parse(editCoverClosePos.Text);

                    config.StepAxisX_Speed = int.Parse(editStepAxisX_Speed.Text);
                    config.StepAxisY_Speed = int.Parse(editStepAxisY_Speed.Text);
                    config.StepAxisZ_Speed = int.Parse(editStepAxisZ_Speed.Text);
                    config.StepAxisGripper_Speed = int.Parse(editStepGripper_Speed.Text);
                    config.StepAxisPipett_Speed = int.Parse(editStepPipett_Speed.Text);
                    config.CoverOpen_Speed = int.Parse(editCoverOpenSpeed.Text);
                    config.CoverClose_Speed = int.Parse(editCoverCloseSpeed.Text);
                    config.StepCoverAcc = int.Parse(editStepCoverAcc.Text);
                    config.StepCoverDec = int.Parse(editStepCoverDec.Text);

                    config.StepAxisX_Acc = int.Parse(editStepAxisX_Acc.Text);
                    config.StepAxisY_Acc = int.Parse(editStepAxisY_Acc.Text);
                    config.StepAxisZ_Acc = int.Parse(editStepAxisZ_Acc.Text);
                    config.StepAxisGripper_Acc = int.Parse(editStepAxisGripper_Acc.Text);
                    config.StepAxisPipett_Acc = int.Parse(editStepAxisHam_Acc.Text);
                    config.RotorCover_Acc = int.Parse(editStepCoverAcc.Text);

                    config.StepAxisX_Dec = int.Parse(editStepAxisX_Dec.Text);
                    config.StepAxisY_Dec = int.Parse(editStepAxisY_Dec.Text);
                    config.StepAxisZ_Dec = int.Parse(editStepAxisZ_Dec.Text);
                    config.StepAxisGripper_Dec = int.Parse(editStepAxisGripper_Dec.Text);
                    config.StepAxisPipett_Dec = int.Parse(editStepAxisHam_Dec.Text);
                    config.RotorCover_Dec = int.Parse(editStepCoverDec.Text);

                    config.StepAxisX_Jog = editStepAxisX_Jog.Text;
                    config.StepAxisY_Jog = editStepAxisY_Jog.Text;
                    config.StepAxisZ_Jog = editStepAxisZ_Jog.Text;
                    config.StepAxisGripper_Jog = editStepGripper_Jog.Text;
                    config.StepAxisPipett_Jog = editStepPipett_Jog.Text;

                    config.FastMoveSpd_ZAxis    = editFastMoveSpd_ZAxis.Text;
                    config.FastMoveSpd_GripAxis = editFastMoveSpd_GripAxis.Text;
                    config.FastMoveSpd_HamAxis  = editFastMoveSpd_HamAxis.Text;

                    config.FastMovePos_ZAxis = editFastMovePos_ZAxis.Text;
                    config.FastMovePos_GripAxis = editFastMovePos_GripAxis.Text;
                    config.FastMovePos_HamAxis = editFastMovePos_HamAxis.Text;

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

                    config.TriPipett_OffsetVol = int.Parse(editTriPipettOffsetVolume.Text);
                    config.TriPipett_LoadingVol = int.Parse(editTriPipettLoadingVolume.Text);
                    config.TriPipett_FlowRate = int.Parse(editTriPipettFlowRate.Text);

                    config.PumpFlowRate = editPumpFlowRate.Text;
                    config.PumpLoadingVolume = editPumpLoadingVolume.Text;
                    config.PumpOffsetVolume = editPumpOffsetVolume.Text;
                    config.PumpPortNo = editPumpPortNo.Text;
                    config.PumpValveDelay = editValveDelay.Text;

                    config.FlowmeterUnitScale = editFlowmeterCntUnit.Text;
                    config.LoadcellErrWeight = editLoadcellErrWeight.Text;

                    config.Pelt_set_temp = edit_pelt_set_temp.Text;
                    config.Fan_on_temp = edit_fan_on_temp.Text;
                    config.Fan_off_temp = edit_fan_off_temp.Text;

                    config.RotorLightBright = editRotorLightBright.Text;
                    config.RoomLightBright = editRoomLightBright.Text;

                    config.Eccentric = editEccentricThreshold.Text;
                    config.Strobe_period = edit_strobe_period.Text;

                    config.TipBasePosX = label_Tip_BasePosX.Text;
                    config.TipBasePosY = label_Tip_BasePosY.Text;
                    config.TubeBasePosX = label_Tube_BasePosX.Text;
                    config.TubeBasePosY = label_Tube_BasePosY.Text;
                    config.CoolingBasePosX = label_Cooling_BasePosX.Text;
                    config.CoolingBasePosY = label_Cooling_BasePosY.Text;

                    config.TipOffsetX = edit_Tip_OffsetX.Text;
                    config.TipOffsetY = edit_Tip_OffsetY.Text;
                    config.TubeOffsetX = edit_Tube_OffsetX.Text;
                    config.TubeOffsetY = edit_Tube_OffsetY.Text;
                    config.CoolingOffsetX = edit_Cooling_OffsetX.Text;
                    config.CoolingOffsetY = edit_Cooling_OffsetY.Text;

                    config.TipOrgPosX = label_Tip_OrgPosX.Text;
                    config.TipOrgPosY = label_Tip_OrgPosY.Text;
                    config.TubeOrgPosX = label_Tube_OrgPosX.Text;
                    config.TubeOrgPosY = label_Tube_OrgPosY.Text;
                    config.CoolingOrgPosX = label_Cooling_OrgPosX.Text;
                    config.CoolingOrgPosY = label_Cooling_OrgPosY.Text;

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

                    GetAxisXPosFromListView();
                    GetAxisYPosFromListView();
                    GetAxisZPosFromListView();
                    GetAxisGripperPosFromListView();
                    GetAxisPipettPosFromListView();
                    GetWorldTeachingPntFromListView();
                    GetToolOffsetFromConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(ex.ToString());
            }
        }
        
        private void ChangeRpm(int currentRpm, int newRpm, int rampTime, int prescale)
        {
            bRpmChanged = false;

            if (bStopFlag == true)
            {
                bRpmChanged = true;
                return;
            }
            if (newRpm == currentRpm)
            {
                bRpmChanged = true;
                return;
            }
            if (!isRunning && !isRunningSingle && !isRunningManual)
            {
                bRpmChanged = true;
                if (SerCmd_Spin(CMD.STOP, 0) == COM_Status.ACK)
                    Rpm.Current = 0;
                return;
            }            

            if(rampTime == 0)   // No need Timer running
            {
                SerCmd_SetParameter(Direction.CCW, newRpm, prescale, 
                    int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));

                if (isRunningManual && newRpm == 0)
                {
                    SerCmd_Spin(CMD.STOP, 0);
                    bStopFlag = true;
                    //if (btnTimer.Text == "Stop Timer")
                    if(bSerialTimerState == true)
                        btnTimer_Click(this, null);
                    EnableControlsManualTest(true, false);
                    isRunningManual = false;
                }
                bRpmChanged = true;
                return;
            }
            double SamplingTime = 100.0; // 100 ms
            Rpm.minChange = 10.0;
            double diffRpm = 0;
            double rpmPerMs = 0.0;

            Rpm.Current = Rpm.AccRpm = currentRpm;
            Rpm.Target = newRpm;
            Rpm.prescale = prescale;
            diffRpm = newRpm - Rpm.Current;
            rpmPerMs = diffRpm / (rampTime * 1000); // rampTime should not 0
            int loopCount = (int)((rampTime*1000) / SamplingTime);
            Rpm.Tick = rpmPerMs * SamplingTime;

            for(int i=0; i<loopCount; i++)
            {
                if (bStopFlag == true)
                {
                    bRpmChanged = true;
                    return;
                }
                label_elaplsed_time.Text = $"{stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}";
                //label_elaplsed_time.Update();
                Application.DoEvents();
                Thread.Sleep((int)SamplingTime);
                if (Rpm.Tick > 0)    // Increment
                {
                    if (Rpm.Target <= Rpm.AccRpm + Rpm.Tick)
                        Rpm.AccRpm = Rpm.Target;
                    else
                        Rpm.AccRpm += Rpm.Tick;

                    if (Math.Abs(Rpm.AccRpm - Rpm.Current) > Rpm.minChange || Rpm.Target == Rpm.AccRpm)
                    {
                        Rpm.Current = Rpm.AccRpm;
                        SerCmd_SetParameter(Direction.CCW, (int)Rpm.Current, Rpm.prescale, 
                                            int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));
                        if(config.bDebugMode)
                            iPrintf($"Set RPM = {(int)Rpm.Current}");
                    }
                    if (Rpm.Target <= Rpm.Current)
                    {
                        bRpmChanged = true;
                        return;
                    }
                }
                else                // Decrement
                {
                    if (Rpm.Target >= Rpm.AccRpm + Rpm.Tick)
                        Rpm.AccRpm = Rpm.Target;
                    else
                        Rpm.AccRpm += Rpm.Tick;

                    if (Math.Abs(Rpm.Current - Rpm.AccRpm) > Rpm.minChange || Rpm.Target == Rpm.AccRpm)
                    {
                        Rpm.Current = Rpm.AccRpm;
                        SerCmd_SetParameter(Direction.CCW, (int)Rpm.Current, Rpm.prescale, 
                                            int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));
                        if (config.bDebugMode)
                            iPrintf($"Set RPM = {(int)Rpm.Current}");
                    }
                    if (Rpm.Target >= Rpm.Current)
                    {
                        if (isRunningManual)
                        {
                            SerCmd_Spin(CMD.STOP, 0);

                            bStopFlag = true;
                            //if (btnTimer.Text == "Stop Timer")
                            if (bSerialTimerState == true)
                                btnTimer_Click(this, null);
                            EnableControlsManualTest(true, false);
                            isRunningManual = false;
                        }
                        bRpmChanged = true;
                        return;
                    }
                }
            }
        }
        
        private void App_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TerminateProgram();
        }

        private void TerminateProgram()
        {
            try
            {
                if (bSerialTimerState == true)
                {
                    timer_com.Stop();
                    bSerialTimerState = false;

                    if (bServoRunState == true)
                        bServoRunState = false;
                }

                if (bPosTimerState == true)
                {
                    timer_pos.Stop();
                    bPosTimerState = false;
                }

                if (bStateMonitorTh)
                {
                    bStateMonitorTh = false;

                    Thread.Sleep(100);
                    if(StateMonitorThread != null)
                    {
                        StateMonitorThread.Abort();
                        StateMonitorThread.Join();
                        StateMonitorThread = null;
                    }
                }

                if (bPosMonitorTh)
                {
                    bPosMonitorTh = false;

                    Thread.Sleep(100);
                    if (PosMonitorThread != null)
                    {
                        PosMonitorThread.Abort();
                        PosMonitorThread.Join();
                        PosMonitorThread = null;
                    }
                }

                if (bShudown)
                    return;

                if (isRunning || isRunningSingle || isRunningManual)
                {
                    timer_camera.Enabled = false;
                    SerCmd_Spin(CMD.STOP, 0);
                    StopRecord();
                    stopwatch.Stop();
                    //if (btnTimer.Text == "Stop Timer")
                    if (bSerialTimerState == true)
                        btnTimer_Click(this, null);
                    StopMusic();
                }

                int nRet = m_pMyCamera.MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    ;
                }

                nRet = m_pMyCamera.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    ;
                }
                iPrintf("Closing ...");

                src.Release();
                if (src != null)
                    src.Dispose();
                if (cvCapture != null)
                    cvCapture.Dispose();

                bRunningThread = false;

                timer_com.Stop();
                bSerialTimerState = false;

                if (bServoRunState == true)
                    bServoRunState = false;

                timer_pos.Stop();
                bPosTimerState = false;

                ClosingOpenedSerialPort();

                iPrintf("Camera, Timer, Thread, Port Closed");

                iPrintf("Log Saved");
                LogFileSave();

                Application.ExitThread();
                iPrintf("Exit Thread");
                Application.Exit();
                iPrintf("Application Exit");
                Environment.Exit(1);
                iPrintf("Environment Exit");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(ex.ToString());
                Environment.Exit(1);
            }
            bShudown = true;
        }

        public void LogFileSave()
        {
            string dtStr = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
            string logFileName = "log_" + dtStr + ".txt";
            string log_path = Path.Combine(DIR_LOG, logFileName);

            DirectoryInfo dir = new DirectoryInfo(DIR_LOG);
            System.Security.AccessControl.DirectorySecurity SecurityRules = null;

            try
            {
                SecurityRules = dir.Parent.GetAccessControl();
                SecurityRules.AddAccessRule(
                    new System.Security.AccessControl.FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    System.Security.AccessControl.FileSystemRights.FullControl,
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                    System.Security.AccessControl.PropagationFlags.None,
                    System.Security.AccessControl.AccessControlType.Allow));
            }
            catch (Exception ex)
            {

                throw ex;
            }

            SerialMessageBox.SaveFile(@log_path, RichTextBoxStreamType.PlainText);

            //SerialMessageBox.AppendText(log_path);
            //System.IO.File.WriteAllText(@log_path, SerialMessageBox.Text.Replace("\n", Environment.NewLine));
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        private void App_Closed(object sender, EventArgs e)
        {
            ;
        }

        private void btnConfigSave_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to Save Config Parameters??",
                                                "Data Saved!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                UpdateConfig(RW.WRITE);
                iPrintf("Config Parameters Saved Done!");
            }
            else
            {
                return;
            }
        }

        private void btnProgressExit_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to Exit Current Progress??",
                                                "All Process Completely Done!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                MessageBox.Show("All Process Completely Done!!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                iPrintf("All Process Completely Done!!");
            }
            else
            {
                return;
            }
        }

        private void btnConfigLoad_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to Load Config Parameters??",
                                                "Data Loaded!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                UpdateConfig(RW.READ);
                iPrintf("Config Parameters Loaded Done!");
            }
            else
            {
                return;
            }
        }

        private string ExtractFileName(string path)
        {
            string fileName = Path.GetFileName(path);
            return fileName.Substring(0, fileName.Length - 4);
        }

        private void chk_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = this.TopMost ? false : true;
        }

        private void btnSetSerialparameter_Click(object sender, EventArgs e)
        {
            try
            {
                COM_Status retVal = SerCmd_SetParameter(Direction.CCW, int.Parse(edit_rpm.Text), int.Parse(edit_prescale.Text), 
                                                        int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));
                if (retVal == COM_Status.NAK)
                    iPrintf("Serial SetParameter NAK");
                else if (retVal == COM_Status.RESET)
                    iPrintf("Serail SetParameter RESET");
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
        }

        private void btnGetSerialStatus_Click(object sender, EventArgs e)
        {
            holdTimer = true;
            COM_Status retVal = GetStatus(true);
            SwitchControl(SwitchState.STATUS, Status.NONE);
            SystemCmd("SYSTEM", "ERRORS", "");

            if (ServoState.bALM == true)
                ServoMonitor(MotorMon.ALARM);

            if (retVal == COM_Status.NAK)
            {
                iPrintf("Serial GetSTATUS NAK");
            }                
            else if (retVal == COM_Status.RESET)
            {
                iPrintf("Serial GetSTATUS RESET");
            }
                
            holdTimer = false;
        }        

        private void btnManualTestRun_Click(object sender, EventArgs e)
        {
            COM_Status retVal;

            try
            {
                bStopFlag = false;
                bSerialStop = false;
                isRunningManual = true;
                Rpm.Current = 0;
                btnClearImage_Click(this, null);
                //btnSetCameraParam_Click(this, null); // reset parameter for Manual run
                DeleteOldFiles();
                btnGetSerialStatus_Click(this, null);

                //if (btnTimer.Text == "Start Timer")
                if (bSerialTimerState == false)
                {
                    btnTimer_Click(this, null);
                }

                SerCmd_SetParameter(Direction.CCW, int.Parse(edit_rpm.Text), int.Parse(edit_prescale.Text), 
                                    int.Parse(edit_servo_acc.Text), int.Parse(edit_servo_dec.Text));
                EccentricClear();
                retVal = SerCmd_Spin(CMD.RUN, int.Parse(edit_servo_duration.Text));

                if (retVal == COM_Status.NAK)
                {
                    iPrintf("Serial CmdRun NAK");
                }                    
                else if (retVal == COM_Status.RESET)
                {
                    iPrintf("Serial CmdRun TIMEOUT");
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
                EnableControlsManualTest(true, false);
            }
        }

        private void btnManualTestStop_Click(object sender, EventArgs e)
        {
            COM_Status retVal = COM_Status.ACK;

            try
            {
                //if (btnTimer.Text == "Stop Timer")
                if(bSerialTimerState == true)
                {
                    btnTimer_Click(this, null);
                }

                GetStatus(true);
                Thread.Sleep(100);
                SwitchControl(SwitchState.STATUS, Status.NONE);
                
                retVal = SerCmd_Spin(CMD.STOP, 0);

                Thread.Sleep(500);
                ServoMonitor(MotorMon.RPM);
                ReadMotorPosition(true, bSilent: true);
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }

            isRunningManual = false;
        }
        
        private void btnDiskStatus_Click(object sender, EventArgs e)
        {
            UpdateDiskInformation();
        }

        private void UpdateDiskInformation()
        {
            long totalSize = 0;
            long freeSpace = 0;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == "C:\\")
                {
                    totalSize = d.TotalSize;  // byte
                    freeSpace = d.TotalFreeSpace;  // byte
                }
                else continue;
            }
            float totalSizeGB = (float)(totalSize / Math.Pow(10, 9));
            float freeSpaceGB = (float)(freeSpace / Math.Pow(10, 9));
            float usedSpaceGB = totalSizeGB - freeSpaceGB;
            float usagePercent = (usedSpaceGB / totalSizeGB) * 100;

            lblDiskUsage.Text = $"{freeSpaceGB:N1} GB free of {totalSizeGB:N1} GB ({usagePercent:N1}% Used)";
            if ((int)usagePercent > DISK_SPACE_UCL)
            {
                prgBarDiskUsage.Enabled = false;
                diskStatus.Enabled = false;
                lblDiskUsage.ForeColor = Color.Red;
            }
            else
            {
                prgBarDiskUsage.Enabled = true;
                diskStatus.Enabled = true;
                lblDiskUsage.ForeColor = Color.DarkGray;
            }

            prgBarDiskUsage.Value = (int)usagePercent;
        }

        private void ValidateDecimalTextBox_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void tbRecordFps_TextChanged(object sender, EventArgs e)
        {
            if(tbRecordFps.Text != "")
            {
                if(int.Parse(tbRecordFps.Text) > 20)
                // 20이 넘어가니 지금 피씨에서는 QueryFrame을 처리못함(동영상 재생시)
                {
                    iPrintf("Recording FPS must be 20 or lower.");
                    tbRecordFps.Text = "20";
                }
            }
        }

        private void ValtdateTextBox_Leave(object sender, EventArgs e)
        {
            if (tbTimerInterval.Text == "")
                tbTimerInterval.Text = config.TimerInterval.ToString();
            else if (edit_rpm.Text == "")
                edit_rpm.Text = config.SpinRpm.ToString();
            else if (edit_servo_acc.Text == "")
                edit_servo_acc.Text = config.risingTime.ToString();
            else if (edit_servo_dec.Text == "")
                edit_servo_dec.Text = config.fallingTime.ToString();
            else if (edit_prescale.Text == "")
                edit_prescale.Text = config.prescale.ToString();
            else if (tbExposure1.Text == "")
                tbExposure1.Text = config.ExposureTime1.ToString();
            else if (tbGain1.Text == "")
                tbGain1.Text = config.VisionGain1.ToString();
            else if (tbGamma1.Text == "")
                tbGamma1.Text = config.Gamma1.ToString();
            else if (tbFrameRate1.Text == "")
                tbFrameRate1.Text = config.FrameRate1.ToString();
            else if (tbRecordFps.Text == "")
                tbRecordFps.Text = config.RecordFrameRate.ToString();
            else if (tbConfigMaxVideoFile.Text == "")
                tbConfigMaxVideoFile.Text = config.MaxVideoFileNumber.ToString();
        }

        private void ValidateIntTextBox_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CheckIndexChange()
        {
            if (isRunningRecipeChanged)
            {
                isRunningRecipeChanged = false;
                WriteVideoLog(true);
                return;
            }

            iTimerCount2++;
            if (iTimerCount2 % 50 == 0) // 2.5sec
            {
                WriteVideoLog();
                iTimerCount2 = 0;
                if (frameLogWriteBuffer.Count >= 10)
                {
                    WriteVideoLogDataToFileAsync(GetVideoLogPath());
                }
            }
        }

        private async void WriteVideoLogDataToFileAsync(string fileName)
        {
            System.IO.File.AppendAllLines(fileName, frameLogWriteBuffer);
            frameLogWriteBuffer = new List<string>();
        }

        private void WriteVideoLog(bool recipeChanged = false)
        {
            if (!File.Exists(GetVideoLogPath()))
            {
                float.TryParse(tbRecordFps.Text.ToString(), out currentRecordFps);
                string fileName = GetVideoLogPath();
                List<string> logHeader = new List<string>();
                string dtStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                logHeader.Add($"Started at: {dtStr}");
                logHeader.Add($"Video File: {label_TimeStamp.Text}.avi (FPS:{currentRecordFps})");
                logHeader.Add($"\n[Recipe Information]");
                logHeader.Add($"{this.Column1.HeaderText}, {this.Column2.HeaderText}, {this.Column3.HeaderText}, {this.Column4.HeaderText}, {this.Column5.HeaderText}");

                for (int rIdx = 0; rIdx < DV_Recipe.Rows.Count - 1; rIdx++)
                {
                    logHeader.Add($"{rIdx + 1}, {config.ListRecipe[rIdx].Enable}, {config.ListRecipe[rIdx].Command1:5s}, {config.ListRecipe[rIdx].Param1}, {config.ListRecipe[rIdx].Param4}, {config.ListRecipe[rIdx].Param3}, {config.ListRecipe[rIdx].Param2}");
                }

                System.IO.File.WriteAllLines(fileName, logHeader);
                timer_frame.Enabled = false;
            }


            if (recipeChanged)
            {

                isRunningRecipeChanged = false;
                if (currentRecipeIndex != 0)
                {
                    recipeTimeSum += stopwatch.Elapsed.TotalSeconds - currentIndexStartTime;
                    recipeFrameSum += Convert.ToInt32(double.Parse(config.ListRecipe[lastRecipeIndex].Param2) * fpsCalcBase);
                    lastRecipeIndex = currentRecipeIndex;
                }
                float recipeFps = 0.0f;
                if (config.ListRecipe[currentRecipeIndex].Command1 == "SHAKE" && recipeFps == 0)
                {
                    isShakeFpsZero = true;
                }
                else if(config.ListRecipe[currentRecipeIndex].Command1 == "SPIN")
                {
                    if (config.ListRecipe[currentRecipeIndex].Param4 == "0" || recipeFps == 0 || 
                        config.ListRecipe[currentRecipeIndex].Param1 == "0")
                    {
                        fpsFromPrescale = 0;
                        isSpinPresetOrFPSZero = true;
                    }
                }

                if (config.ListRecipe[currentRecipeIndex].Param4 == "0")
                    fpsFromPrescale = 0;
                else
                    fpsFromPrescale = int.Parse(config.ListRecipe[currentRecipeIndex].Param1) / 
                                      60 / int.Parse(config.ListRecipe[currentRecipeIndex].Param4);

                fpsCalcBase = recipeFps;
                if (fpsCalcBase >= currentRecordFps)
                    fpsCalcBase = currentRecordFps;
                if (config.ListRecipe[currentRecipeIndex].Command1 == "SPIN")
                    if (fpsCalcBase >= fpsFromPrescale)
                        fpsCalcBase = fpsFromPrescale;
            }

            if ((!isSpinPresetOrFPSZero && !isShakeFpsZero) || recipeChanged)
            {
                estimatedCurrentFrame = Convert.ToInt64((recipeFrameSum + (stopwatch.Elapsed.TotalSeconds - currentIndexStartTime) * fpsCalcBase)*VIDEO_LOG_FRAME_SCALE);
                estimatedVideoTime = TimeSpan.FromSeconds(estimatedCurrentFrame / currentRecordFps);
            }
            string val = $"{estimatedVideoTime.Minutes:d2}:{estimatedVideoTime.Seconds:d2}, {estimatedCurrentFrame:d8}, {stopwatch.Elapsed.Minutes:d2}:{stopwatch.Elapsed.Seconds:d2}, {currentRecipeIndex + 1}";
            frameLogWriteBuffer.Add(val);
        }

        private void materialTabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (isRunning || isRunningSingle)
                e.Cancel = true;
            UpdateControlValue(GetSet.GET);
            TabItem = materialTabControl1.SelectedTab.Text;
        }

        private void radio_rpm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                edit_rcf.Enabled = false;
                edit_rpm.Enabled = true;
                edit_rcf.Text = (1.118 * double.Parse(edit_r.Text) * 
                                Math.Pow(double.Parse(edit_rpm.Text), 2) * 0.000001).ToString("F1");
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void edit_rpm_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (radio_rpm.Checked)
                    edit_rcf.Text = (1.118 * double.Parse(edit_r.Text) * 
                                    Math.Pow(double.Parse(edit_rpm.Text), 2) * 0.000001).ToString("F1");
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void radio_rcf_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                edit_rcf.Enabled = true;
                edit_rpm.Enabled = false;
                edit_rpm.Text = ((int)Math.Sqrt((double.Parse(edit_rcf.Text) * 1000000) / 
                                (1.118 * double.Parse(edit_r.Text)))).ToString();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void edit_rcf_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (radio_rcf.Checked)
                {
                    edit_rpm.Text = ((int)Math.Sqrt((double.Parse(edit_rcf.Text) * 1000000) / 
                                    (1.118 * double.Parse(edit_r.Text)))).ToString();
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnDoorActive_Click(object sender, EventArgs e)
        {
            if(SensorStatus.DoorEnable == Status.ON)
            {
                SwitchControl(SwitchState.DOOR_ENA, Status.OFF);                
            }
            else if (SensorStatus.DoorEnable == Status.OFF)
            {
                SwitchControl(SwitchState.DOOR_ENA, Status.ON);
                //SwitchControl(SwitchState.DOOR_RST, Status.NONE); //door가 한번 열리고 나면 reset해야 함
            }

            SwitchControl(SwitchState.STATUS, Status.NONE);
        }

        private void btnInitializeAll_Click(object sender, EventArgs e)
        {
            COM_Status retVal = COM_Status.RESET;

            // door가 닫혀있거나 run switch가 on되어 있지 않으면 초기화를 진행하지 않음
            if (SensorStatus.RunSwitch == Status.OFF ||
               (SwitchMon.bDoorEnable == true && SwitchMon.bDoorSW == false))
            {
                this.btnInitializeAll.BackColor = Color.LightPink;
                btnInitializeAll.Text = "Initialize Fail";
                iPrintf("Initialize Fail!! Check Switch State");
                DisplayStatusMessage("Initialization Fail!! Check Switch State");
                bSystemInitDoneState = false;

                return;
            }

            //if (this.btnInitializeAll.BackColor == Color.LightSkyBlue)
            if(bSystemInitDoneState == false)
            {
                // Set Motor Homing Parameters: offset, searching speed
                //SetStepMotorHomeMoveParam();
                DisplayStatusMessage("Initializing....");

                // Move all step motors homing
                if (MoveHomeStepMotor() == false)
                {
                    iPrintf("Step Motor Init Fail!");
                }
                                
                ReadMotorPosition(true);
                Thread.Sleep(200);

                // Initialize pipett & pump
                //retVal = InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600A0A10z0V1000a1000a0R", Environment.NewLine));
                //retVal = InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600V1000A0A10R", Environment.NewLine));
                retVal = InitPeripherals(PERIPHERAL.TRI_PIPETT, string.Format("/2z1600V1000A0A1580R", Environment.NewLine));

                if (retVal == COM_Status.ACK)
                {
                    SensorStatus.AlarmPeri1_tri_pipett = Status.OFF;
                    iPrintf("Tricontinent 5ml pipett Init Done!");
                    retVal = COM_Status.RESET;
                }
                else
                {
                    iPrintf("Tricontinent 5ml pipett Init Fail!");
                    retVal = COM_Status.RESET;
                }
                Thread.Sleep(800);  //800 //600

                // 해밀턴 모듈의 초기화 명령 DI 전달 후 응답이 이유는 모르겠지만 첫번째에서는 전달되지 않아 두번 전달함
                //RunPer2_HamiltonPipett("RF", 0, 0, 0, 0, TIP_TYPE.NONE);
                InitPeripherals(PERIPHERAL.HAM_PIPETT);
                retVal = RunPer2_HamiltonPipett("DI", 0, 0, 0, 0, TIP_TYPE.NONE);
                Thread.Sleep(100);

                if (SensorStatus.ham_pipett_errNo == 0)
                    ConfirmTipPresence();

                if (retVal == COM_Status.ACK)
                {
                    SensorStatus.AlarmPeri2_ham_pipett = Status.OFF;
                    iPrintf("Hamilton 1ml pipett Init Done!");
                    retVal = COM_Status.RESET;
                }
                else
                {
                    iPrintf("Hamilton 1ml pipett Init Fail!");
                    retVal = COM_Status.RESET;
                }
                Thread.Sleep(600);

                retVal = InitPeripherals(PERIPHERAL.TRI_PUMP);

                if (retVal == COM_Status.ACK)
                {
                    SensorStatus.AlarmPeri3_tri_pump = Status.OFF;
                    iPrintf("Tricontinent Syringe Pump Init Done!");
                }
                else
                {
                    iPrintf("Tricontinent Syringe Pump Init Fail!");
                }
                Thread.Sleep(200);

                // Move Rotor Chamber1 Position
                if (SelectRotorPosition(CHAMBER_POS.CHAMBER1) == COM_Status.ACK)
                {
                    ServoState.bHOME_COMP = true;
                    iPrintf("Rotor Position Init Done!");
                }
                else
                {
                    iPrintf("Rotor Position Init Fail!");
                }

                WaitForServoStop();

                //ServoState.bHOME_COMP = true;   // FOR TEST

                // verify current status
                GetStatus(true);
                Thread.Sleep(200);
                SwitchControl(SwitchState.STATUS, Status.NONE);
                Thread.Sleep(200);
                ServoMonitor(MotorMon.POS);
                SystemCmd("SYSTEM", "ERRORS", "");

                if (ServoState.bALM == true)
                    ServoMonitor(MotorMon.ALARM);

                if (SensorStatus.Home == true && SensorStatus.Alarm == false)
                {
                    this.btnInitializeAll.BackColor = Color.SteelBlue;
                    btnInitializeAll.Text = "Initialize Done";
                    iPrintf("All Initialize Process Done!!");
                    DisplayStatusMessage("Initialization Done!!");
                    bSystemInitDoneState = true;
                }
                else
                {
                    this.btnInitializeAll.BackColor = Color.LightPink;
                    btnInitializeAll.Text = "Initialize Fail";
                    iPrintf("Initialize Process Fail!!");
                    DisplayStatusMessage("Initialization Fail!!");
                    bSystemInitDoneState = false;
                }
            }
            //else if (this.btnInitializeAll.BackColor == Color.SteelBlue ||
            //         this.btnInitializeAll.BackColor == Color.LightPink)
            else if(bSystemInitDoneState == true && 
                   (isRunning == false && isRunningSingle == false && isRunningManual == false))
            {
                this.btnInitializeAll.BackColor = Color.LightSkyBlue;
                btnInitializeAll.Text = "Initialize All";
                bSystemInitDoneState = false;
            }
        }

        private void btnConnectCamera_Click(object sender, EventArgs e)
        {
            int cameraIdx = 0;

            DeviceListAcq();    // search device(camera)
            if(OpenCamera(cameraIdx) == false)      // open device(camera)
            {
                iPrintf(string.Format("index {0} camera open Fail!", cameraIdx));
            }
            else
            {
                iPrintf(string.Format("index {0} camera open Done!", cameraIdx));
                SetCameraParam(cameraIdx);
                iPrintf(string.Format("index {0} camera parameter set Done!", cameraIdx));
            }
            // set device(camera) mode
            int nRet = m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 
                                                         (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf(string.Format("camera mode setting Fail!"));
            }
            else
            {
                iPrintf(string.Format("camera mode setting Done!"));
            }

            if(StartGrab() == false)        // start device(camera)
            {
                iPrintf(string.Format("camera start grab Fail!"));
            }
            else
            {
                iPrintf(string.Format("camera start grab Done!"));
            }
        }

        private void btnDisconnectCamera_Click(object sender, EventArgs e)
        {
            if (StopGrab() != MyCamera.MV_OK)        // stop device(camera)
            {
                iPrintf(string.Format("camera stop grab Fail!"));
            }
            else
            {
                iPrintf(string.Format("camera stop grab Done!"));
            }
        }
    }
} 

