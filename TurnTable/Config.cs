using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CytoDx
{
    public enum RW { READ, WRITE };
    public enum GetSet { GET, SET};

    public enum RecipeCommand
    {
        SHAKE = 0,
        ROTATE = 1,
    }

    public struct CMD_BUTTON
    {
        public string name;
        public string description;
        public List<Recipe> recipe;
    }

    public struct Recipe
    {
        public int Enable;
        public string Command1;
        public string Command2;
        public string Param1;
        public string Param2;
        public string Param3;
        public string Param4;
        public string Param5;
        public string Param6;
        public string Param7;
        public string Sleep;
        public string Comment;
    }

    public struct DefinePos
    {
        public string Name;
        public string Speed;
        public string Position;
        public string Acc;
        public string Dec;
    }

    public struct DefineWorldPos
    {
        public string Idx;
        public string Name;
        public string X;
        public string Y;
        public string Z;
        public string Gripper;
        public string Pipett;
    }

    public struct DefineToolOffset
    {
        public string Idx;
        public string Name;
        public string Z_Dist;
        public string X;
        public string Y;
        public string Z;
    }

    public class Config
    {
        public List<CMD_BUTTON> buttons;
        public List<Recipe> ListRecipe;

        public List<DefinePos> Pos_AxisX;
        public List<DefinePos> Pos_AxisY;
        public List<DefinePos> Pos_AxisZ;
        public List<DefinePos> Pos_AxisGripper;
        public List<DefinePos> Pos_AxisPipett;
        public List<DefineWorldPos> Pos_WorldPos;
        public List<DefineToolOffset> ToolOffset;

        public string PathConfig = "";
        public string PathButtonConfig = "";
        public string PathRecipe = "";
        public string PathLastfile = "";

        public string LastButtonFileName = "";

        public bool bDisplayException = true;
        public bool bDebugMode = false;
        public string ComPort = string.Empty;
        //public int BaudRate = 38400;
        //public int BaudRate = 115200;
        public int BaudRate = 57600;

        public int SpinRpm = 3000;
        public int prescale = 16;
        public int risingTime = 0;
        public int fallingTime = 0;
        public int RecipeLineDelay = 0;
        public string RotorRadius = "0.0";
        public string strSpinDuration = "0";
        public string SpinRpm_Offset = "0";

        public int TimerInterval = 500;

        public int TestCmd = 0;
        public int TestCount = 0;
        public int TestInterval = 0;

        public int FormTop = 100;
        public int FormLeft = 100;
        public int FormWidth = 600;
        public int FormHeight = 400;

        public bool TopMost = false;

        public int Theme = 1;
        public int ColorScheme = 3;
        public bool PlayMusicOntest = true;
        public int MaxVideoFileNumber = 20;
        public int MaxImageFileNumber = 1000;

        public int VisionTriggerMode = 1;
        public int VisionTriggerActivation = 2; // 0:Rising, 1:Falling, 2:Level High, 3:Level Low
        public string ExposureTime1 = "0.0";
        public string Gamma1 = "0.0";
        public string VisionGain1 = "0.0";
        public string FrameRate1 = "0.0";
        public string ExposureTime2 = "0.0";
        public string Gamma2 = "0.0";
        public string VisionGain2 = "0.0";
        public string FrameRate2 = "0.0";
        public string RecordFrameRate = "1";

        public int StepAxisX_Speed = 0;
        public int StepAxisY_Speed = 0;
        public int StepAxisZ_Speed = 0;
        public int StepAxisGripper_Speed = 0;
        public int StepAxisPipett_Speed = 0;
        public int CoverOpen_Speed = 0;
        public int CoverClose_Speed = 0;
        public int StepCoverAcc = 0;
        public int StepCoverDec = 0;

        public int StepAxisX_Acc = 0;
        public int StepAxisY_Acc = 0;
        public int StepAxisZ_Acc = 0;
        public int StepAxisGripper_Acc = 0;
        public int StepAxisPipett_Acc = 0;
        public int RotorCover_Acc = 0;

        public int StepAxisX_Dec = 0;
        public int StepAxisY_Dec = 0;
        public int StepAxisZ_Dec = 0;
        public int StepAxisGripper_Dec = 0;
        public int StepAxisPipett_Dec = 0;
        public int RotorCover_Dec = 0;

        public int cLLD_Speed = 0;

        public int TriPipett_OffsetVol = 0;
        public int TriPipett_LoadingVol = 0;
        public int TriPipett_FlowRate = 0;
        
        public string StepAxisX_Pos = "0.0";
        public string StepAxisY_Pos = "0.0";
        public string StepAxisZ_Pos = "0.0";
        public string StepAxisGripper_Pos = "0.0";
        public string StepAxisPipett_Pos = "0.0";
        public int CoverOpen_Pos = 0;
        public int CoverClose_Pos = 0;

        public string ServoAccDec_MovA = "2";
        public string ServoRpm_MovA = "20";
        public string ServoPos_Chamber1 = "160000";
        public string ServoPos_CellDown1 = "1208576";
        public string ServoPos_Chamber2 = "2257152";
        public string ServoPos_CellDown2 = "3305728";

        public string StepAxisX_Jog = "0.0";
        public string StepAxisY_Jog = "0.0";
        public string StepAxisZ_Jog = "0.0";
        public string StepAxisGripper_Jog = "0.0";
        public string StepAxisPipett_Jog = "0.0";
        
        public string FastMoveSpd_ZAxis = "0.0";
        public string FastMoveSpd_GripAxis = "0.0";
        public string FastMoveSpd_HamAxis = "0.0";

        public string FastMovePos_ZAxis = "0.0";
        public string FastMovePos_GripAxis = "0.0";
        public string FastMovePos_HamAxis = "0.0";

        public string HomeSearchSpd_X     = "0";
        public string HomeSearchSpd_Y     = "0";
        public string HomeSearchSpd_Z     = "0";
        public string HomeSearchSpd_Grip  = "0";
        public string HomeSearchSpd_Ham   = "0";
        public string HomeSearchSpd_Cover = "0";
        public string HomeSearchSpd_Servo = "0";

        public string HomeOffsetPos_X     = "0.0";
        public string HomeOffsetPos_Y     = "0.0";
        public string HomeOffsetPos_Z     = "0.0";
        public string HomeOffsetPos_Grip  = "0.0";
        public string HomeOffsetPos_Ham   = "0.0";
        public string HomeOffsetPos_Cover = "0.0";
        public string HomeOffsetPos_Servo = "0.0";

        public string PumpFlowRate = "0.0";
        public string PumpLoadingVolume = "0.0";
        public string PumpOffsetVolume = "0.0";
        public string PumpPortNo = "1";
        public string PumpValveDelay = "0";

        public string FlowmeterUnitScale = "0.0";
        public string LoadcellErrWeight = "0.0";

        public string Pelt_set_temp = "0.0";
        public string Fan_on_temp = "0.0";
        public string Fan_off_temp = "0.0";

        public string RotorLightBright = "0.0";
        public string RoomLightBright = "0.0";
        
        //public string SyringeHomePos = "8";

        public string Eccentric = "0";
        public string Strobe_period = "0";
        public int ButtonCount = 0;

        public string TipBasePosX = "0.0";
        public string TipBasePosY = "0.0";
        public string TubeBasePosX = "0.0";
        public string TubeBasePosY = "0.0";
        public string CoolingBasePosX = "0.0";
        public string CoolingBasePosY = "0.0";
        public string TipOffsetX = "0.0";
        public string TipOffsetY = "0.0";
        public string TubeOffsetX = "0.0";
        public string TubeOffsetY = "0.0";
        public string CoolingOffsetX = "0.0";
        public string CoolingOffsetY = "0.0";
        public string TipOrgPosX = "0.0";
        public string TipOrgPosY = "0.0";
        public string TubeOrgPosX = "0.0";
        public string TubeOrgPosY = "0.0";
        public string CoolingOrgPosX = "0.0";
        public string CoolingOrgPosY = "0.0";

        public string Pipett_offsetX_5ml = "0.0";
        public string Pipett_offsetY_5ml = "0.0";
        public string Pipett_offsetZ_5ml = "0.0";
        public string Pipett_offsetX_gripper = "0.0";
        public string Pipett_offsetY_gripper = "0.0";
        public string Pipett_offsetZ_gripper = "0.0";
        public string Pipett_offsetX_laser = "0.0";
        public string Pipett_offsetY_laser = "0.0";
        public string Pipett_offsetZ_laser = "0.0";
        public string laser_Z_dist = "0.0";

        public string Pipett_offsetX_1ml = "0.0";
        public string Pipett_offsetY_1ml = "0.0";
        public string Pipett_offsetZ_1ml = "0.0";

        public string Pipett_offsetZ_10ul = "0.0";
        public string Pipett_offsetZ_300ul = "0.0";
        public string Pipett_offsetZ_CalibTip = "0.0";

        string EXE = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileInt(string Section, string Key, int Default, string FilePath);

        public Config()
        {
            this.buttons = new List<CMD_BUTTON>();
            //this.DefineButtons = new List<Button>();
            this.ListRecipe = new List<Recipe>();
            this.Pos_AxisX = new List<DefinePos>();
            this.Pos_AxisY = new List<DefinePos>();
            this.Pos_AxisZ = new List<DefinePos>();
            this.Pos_AxisGripper = new List<DefinePos>();
            this.Pos_AxisPipett = new List<DefinePos>();
            this.Pos_WorldPos = new List<DefineWorldPos>();
            this.ToolOffset = new List<DefineToolOffset>();
        }

        public void ReadWriteConfig(RW rw)
        {
            try
            {
                //this.PathConfig = "C:\\TruNser_C2000\\TruNserC2000.ini";
                this.PathConfig = MainWindow.DIR_HOME +"\\TruNserC2000.ini";
                
                if (rw == RW.READ)
                {
                    if (!File.Exists(PathConfig))
                    {
                        MessageBox.Show(string.Format("{0} file not found", PathConfig),"", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ComPort = "COM1";
                    }
                    else
                    {
                        ComPort   = Read("SERIAL", "PORT", "COM3", PathConfig);

                        TimerInterval = Read("TIMER", "INTERVAL", 1000, PathConfig);

                        SpinRpm  = Read("PARAMETER", "RPM", 3000, PathConfig);
                        prescale = Read("PARAMETER", "PRESCALE", 16, PathConfig);
                        risingTime = Read("PARAMETER", "RISING_TIME", 16, PathConfig);
                        fallingTime = Read("PARAMETER", "FALLING_TIME", 16, PathConfig);
                        RotorRadius = Read("PARAMETER", "ROTOR_RADIUS", RotorRadius, PathConfig);
                        strSpinDuration = Read("PARAMETER", "SPIN_DURATION", strSpinDuration, PathConfig);
                        SpinRpm_Offset = Read("PARAMETER", "RPM_OFFSET", SpinRpm_Offset, PathConfig);

                        TestCmd         = Read("TEST_CONDITION", "COMMAND", 1, PathConfig);
                        TestCount       = Read("TEST_CONDITION", "COUNT", 1, PathConfig);
                        TestInterval    = Read("TEST_CONDITION", "INTERVAL", 5, PathConfig);
                        RecipeLineDelay = Read("TEST_CONDITION", "LINE_DELAY", 0, PathConfig);
                        PlayMusicOntest = Convert.ToBoolean(Read("TEST_CONDITION", "PLAY_MUSIC", 0, PathConfig));

                        FormTop         = Read("WINDOW", "TOP", 100, PathConfig);
                        FormLeft        = Read("WINDOW", "LEFT", 100, PathConfig);
                        FormWidth       = Read("WINDOW", "WIDTH", 500, PathConfig);
                        FormHeight      = Read("WINDOW", "HEIGHT", 500, PathConfig);
                        TopMost         = Convert.ToBoolean(Read("WINDOW", "TOP_MOST", 0, PathConfig));

                        Theme = Read("FORM_COLOR", "THEME", Theme, PathConfig);
                        ColorScheme = Read("FORM_COLOR", "SCHEME", ColorScheme, PathConfig);

                        bDebugMode = Convert.ToBoolean(Read("MACHINE", "DEBUG_MODE", 0, PathConfig));
                        bDisplayException = Convert.ToBoolean(Read("MACHINE", "DISPLAY_EXCEPTION", 1, PathConfig));

                        VisionTriggerMode = Read("CAMERA", "TRIGGER_MODE", VisionTriggerMode, PathConfig);
                        VisionTriggerActivation = Read("CAMERA", "TRIGGER_ACTIVATION", VisionTriggerActivation, PathConfig);
                        ExposureTime1 = Read("CAMERA", "EXPOSURE_TIME_1", ExposureTime1, PathConfig);
                        VisionGain1 = Read("CAMERA", "GAIN_1", VisionGain1, PathConfig);
                        Gamma1 = Read("CAMERA", "GAMMA_1", Gamma1, PathConfig);
                        FrameRate1 = Read("CAMERA", "FRAME_RATE_1", FrameRate1, PathConfig);
                        ExposureTime2 = Read("CAMERA", "EXPOSURE_TIME_2", ExposureTime2, PathConfig);
                        VisionGain2 = Read("CAMERA", "GAIN_2", VisionGain2, PathConfig);
                        Gamma2 = Read("CAMERA", "GAMMA_2", Gamma2, PathConfig);
                        FrameRate2 = Read("CAMERA", "FRAME_RATE_2", FrameRate2, PathConfig);
                        RecordFrameRate = Read("CAMERA", "RECORD_FRAME_RATE", RecordFrameRate, PathConfig);
                        Strobe_period = Read("CAMERA", "STROBE_PERIOD", Strobe_period, PathConfig);

                        MaxVideoFileNumber = Read("MISC", "MAX_VIDEO_FILE_NUMBER", MaxVideoFileNumber, PathConfig);
                        MaxVideoFileNumber = Read("MISC", "MAX_IMAGE_FILE_NUMBER", MaxImageFileNumber, PathConfig);

                        StepAxisX_Speed = Read("MOTOR", "STEP_AXIS_X_SPEED", StepAxisX_Speed, PathConfig);
                        StepAxisY_Speed = Read("MOTOR", "STEP_AXIS_Y_SPEED", StepAxisY_Speed, PathConfig);
                        StepAxisZ_Speed = Read("MOTOR", "STEP_AXIS_Z_SPEED", StepAxisZ_Speed, PathConfig);
                        StepAxisGripper_Speed = Read("MOTOR", "STEP_AXIS_GRIPPER_SPEED", StepAxisGripper_Speed, PathConfig);
                        StepAxisPipett_Speed = Read("MOTOR", "STEP_AXIS_PIPETT_SPEED", StepAxisPipett_Speed, PathConfig);
                        CoverOpen_Speed = Read("MOTOR", "COVER_OPEN_SPEED", CoverOpen_Speed, PathConfig);
                        CoverClose_Speed = Read("MOTOR", "COVER_CLOSE_SPEED", CoverClose_Speed, PathConfig);
                        StepCoverAcc = Read("MOTOR", "COVER_AXIS_ACC", StepCoverAcc, PathConfig);
                        StepCoverDec = Read("MOTOR", "COVER_AXIS_DEC", StepCoverDec, PathConfig);

                        StepAxisX_Acc = Read("MOTOR", "STEP_AXIS_X_ACC", StepAxisX_Acc, PathConfig);
                        StepAxisY_Acc = Read("MOTOR", "STEP_AXIS_Y_ACC", StepAxisY_Acc, PathConfig);
                        StepAxisZ_Acc = Read("MOTOR", "STEP_AXIS_Z_ACC", StepAxisZ_Acc, PathConfig);
                        StepAxisGripper_Acc = Read("MOTOR", "STEP_AXIS_GRIPPER_ACC", StepAxisGripper_Acc, PathConfig);
                        StepAxisPipett_Acc = Read("MOTOR", "STEP_AXIS_PIPETT_ACC", StepAxisPipett_Acc, PathConfig);
                        RotorCover_Acc = Read("MOTOR", "STEP_AXIS_ROTOR_COVER_ACC", RotorCover_Acc, PathConfig);

                        StepAxisX_Dec = Read("MOTOR", "STEP_AXIS_X_DEC", StepAxisX_Dec, PathConfig);
                        StepAxisY_Dec = Read("MOTOR", "STEP_AXIS_Y_DEC", StepAxisY_Dec, PathConfig);
                        StepAxisZ_Dec = Read("MOTOR", "STEP_AXIS_Z_DEC", StepAxisZ_Dec, PathConfig);
                        StepAxisGripper_Dec = Read("MOTOR", "STEP_AXIS_GRIPPER_DEC", StepAxisGripper_Dec, PathConfig);
                        StepAxisPipett_Dec = Read("MOTOR", "STEP_AXIS_PIPETT_DEC", StepAxisPipett_Dec, PathConfig);
                        RotorCover_Dec = Read("MOTOR", "STEP_AXIS_ROTOR_COVER_DEC", RotorCover_Dec, PathConfig);

                        cLLD_Speed = Read("MOTOR", "CLLD_SPD", cLLD_Speed, PathConfig);

                        StepAxisX_Pos = Read("MOTOR", "STEP_AXIS_X_POS", StepAxisX_Pos, PathConfig);
                        StepAxisY_Pos = Read("MOTOR", "STEP_AXIS_Y_POS", StepAxisY_Pos, PathConfig);
                        StepAxisZ_Pos = Read("MOTOR", "STEP_AXIS_Z_POS", StepAxisZ_Pos, PathConfig);
                        StepAxisGripper_Pos = Read("MOTOR", "STEP_AXIS_GRIPPER_POS", StepAxisGripper_Pos, PathConfig);
                        StepAxisPipett_Pos = Read("MOTOR", "STEP_AXIS_PIPETT_POS", StepAxisPipett_Pos, PathConfig);
                        CoverOpen_Pos = Read("MOTOR", "COVER_OPEN_POS", CoverOpen_Pos, PathConfig);
                        CoverClose_Pos = Read("MOTOR", "COVER_CLOSE_POS", CoverClose_Pos, PathConfig);

                        StepAxisX_Jog = Read("MOTOR", "STEP_AXIS_X_JOG", StepAxisX_Jog, PathConfig);
                        StepAxisY_Jog = Read("MOTOR", "STEP_AXIS_Y_JOG", StepAxisY_Jog, PathConfig);
                        StepAxisZ_Jog = Read("MOTOR", "STEP_AXIS_Z_JOG", StepAxisZ_Jog, PathConfig);
                        StepAxisGripper_Jog = Read("MOTOR", "STEP_AXIS_GRIPPER_JOG", StepAxisGripper_Jog, PathConfig);
                        StepAxisPipett_Jog = Read("MOTOR", "STEP_AXIS_PIPETT_JOG", StepAxisPipett_Jog, PathConfig);
                        
                        FastMoveSpd_ZAxis = Read("MOTOR", "FAST_MOVE_SPD_Z_AXIS", FastMoveSpd_ZAxis, PathConfig);
                        FastMoveSpd_GripAxis = Read("MOTOR", "FAST_MOVE_SPD_GRIP_AXIS", FastMoveSpd_GripAxis, PathConfig);
                        FastMoveSpd_HamAxis = Read("MOTOR", "FAST_MOVE_SPD_PIPETT_AXIS", FastMoveSpd_HamAxis, PathConfig);

                        FastMovePos_ZAxis = Read("MOTOR", "FAST_MOVE_POS_Z_AXIS", FastMovePos_ZAxis, PathConfig);
                        FastMovePos_GripAxis = Read("MOTOR", "FAST_MOVE_POS_GRIP_AXIS", FastMovePos_GripAxis, PathConfig);
                        FastMovePos_HamAxis = Read("MOTOR", "FAST_MOVE_POS_PIPETT_AXIS", FastMovePos_HamAxis, PathConfig);

                        HomeSearchSpd_X     = Read("MOTOR", "HOME_SEARCH_SPD_X_AXIS", HomeSearchSpd_X, PathConfig);
                        HomeSearchSpd_Y     = Read("MOTOR", "HOME_SEARCH_SPD_Y_AXIS", HomeSearchSpd_Y, PathConfig);
                        HomeSearchSpd_Z     = Read("MOTOR", "HOME_SEARCH_SPD_Z_AXIS", HomeSearchSpd_Z, PathConfig);
                        HomeSearchSpd_Grip  = Read("MOTOR", "HOME_SEARCH_SPD_GRIP_AXIS", HomeSearchSpd_Grip, PathConfig);
                        HomeSearchSpd_Ham   = Read("MOTOR", "HOME_SEARCH_SPD_PIPETT_AXIS", HomeSearchSpd_Ham, PathConfig);
                        HomeSearchSpd_Cover = Read("MOTOR", "HOME_SEARCH_SPD_COVER_AXIS", HomeSearchSpd_Cover, PathConfig);
                        HomeSearchSpd_Servo = Read("MOTOR", "HOME_SEARCH_SPD_SERVO_AXIS", HomeSearchSpd_Servo, PathConfig);

                        HomeOffsetPos_X     = Read("MOTOR", "HOME_OFFSET_POS_X_AXIS", HomeOffsetPos_X, PathConfig);
                        HomeOffsetPos_Y     = Read("MOTOR", "HOME_OFFSET_POS_Y_AXIS", HomeOffsetPos_Y, PathConfig);
                        HomeOffsetPos_Z     = Read("MOTOR", "HOME_OFFSET_POS_Z_AXIS", HomeOffsetPos_Z, PathConfig);
                        HomeOffsetPos_Grip  = Read("MOTOR", "HOME_OFFSET_POS_GRIP_AXIS", HomeOffsetPos_Grip, PathConfig);
                        HomeOffsetPos_Ham   = Read("MOTOR", "HOME_OFFSET_POS_PIPETT_AXIS", HomeOffsetPos_Ham, PathConfig);
                        HomeOffsetPos_Cover = Read("MOTOR", "HOME_OFFSET_POS_COVER_AXIS", HomeOffsetPos_Cover, PathConfig);
                        HomeOffsetPos_Servo = Read("MOTOR", "HOME_OFFSET_POS_SERVO_AXIS", HomeOffsetPos_Servo, PathConfig);

                        Eccentric = Read("MOTOR", "ECCENTRIC", Eccentric, PathConfig);

                        ServoAccDec_MovA = Read("SERVO", "SERVO_ACCDEC_MOVA", ServoAccDec_MovA, PathConfig);
                        ServoRpm_MovA      = Read("SERVO", "SERVO_RPM_MOVA", ServoRpm_MovA, PathConfig);
                        ServoPos_Chamber1  = Read("SERVO", "SERVO_POS_CHAMBER1", ServoPos_Chamber1, PathConfig);
                        ServoPos_CellDown1 = Read("SERVO", "SERVO_POS_CELLDOWN1", ServoPos_CellDown1, PathConfig);
                        ServoPos_Chamber2 = Read("SERVO", "SERVO_POS_CHAMBER2", ServoPos_Chamber2, PathConfig);
                        ServoPos_CellDown2 = Read("SERVO", "SERVO_POS_CELLDOWN2", ServoPos_CellDown2, PathConfig);

                        TriPipett_OffsetVol = Read("TRI_PIPETT", "OFFSET_VOLUME", TriPipett_OffsetVol, PathConfig);
                        TriPipett_LoadingVol = Read("TRI_PIPETT", "LOAD_VOLUME", TriPipett_LoadingVol, PathConfig);
                        TriPipett_FlowRate = Read("TRI_PIPETT", "FLOW_RATE", TriPipett_FlowRate, PathConfig);

                        PumpFlowRate = Read("PUMP", "FLOW_RATE", PumpFlowRate, PathConfig);
                        PumpLoadingVolume = Read("PUMP", "LOAD_VOLUME", PumpLoadingVolume, PathConfig);
                        PumpOffsetVolume = Read("PUMP", "OFFSET_VOLUME", PumpOffsetVolume, PathConfig);
                        PumpPortNo = Read("PUMP", "PORT_NO", PumpPortNo, PathConfig);
                        PumpValveDelay = Read("PUMP", "VALVE_DELAY", PumpValveDelay, PathConfig);

                        FlowmeterUnitScale = Read("SENSOR", "FLOWMETER_UNIT_SCALE", FlowmeterUnitScale, PathConfig);
                        LoadcellErrWeight = Read("SENSOR", "LOADCELL_ERR_WEIGHT", LoadcellErrWeight, PathConfig);

                        Pelt_set_temp = Read("PELT", "SET_TEMP", Pelt_set_temp, PathConfig);
                        Fan_on_temp = Read("PELT", "FAN_ON_TEMP", Fan_on_temp, PathConfig);
                        Fan_off_temp = Read("PELT", "FAN_OFF_TEMP", Fan_off_temp, PathConfig);

                        RotorLightBright = Read("LIGHT", "ROTOR_LIGHT_BRIGHT", RotorLightBright, PathConfig);
                        RoomLightBright = Read("LIGHT", "ROOM_LIGHT_BRIGHT", RoomLightBright, PathConfig);

                        TipBasePosX = Read("TIP_TEACH", "BASE_POSX", TipBasePosX, PathConfig);
                        TipBasePosY = Read("TIP_TEACH", "BASE_POSY", TipBasePosY, PathConfig);
                        TipOffsetX = Read("TIP_TEACH", "OFFSET_X", TipOffsetX, PathConfig);
                        TipOffsetY = Read("TIP_TEACH", "OFFSET_Y", TipOffsetY, PathConfig);
                        TipOrgPosX = Read("TIP_TEACH", "ORG_POSX", TipOrgPosX, PathConfig);
                        TipOrgPosY = Read("TIP_TEACH", "ORG_POSY", TipOrgPosY, PathConfig);

                        TubeBasePosX = Read("TUBE_TEACH", "BASE_POSX", TubeBasePosX, PathConfig);
                        TubeBasePosY = Read("TUBE_TEACH", "BASE_POSY", TubeBasePosY, PathConfig);
                        TubeOffsetX = Read("TUBE_TEACH", "OFFSET_X", TubeOffsetX, PathConfig);
                        TubeOffsetY = Read("TUBE_TEACH", "OFFSET_Y", TubeOffsetY, PathConfig);
                        TubeOrgPosX = Read("TUBE_TEACH", "ORG_POSX", TubeOrgPosX, PathConfig);
                        TubeOrgPosY = Read("TUBE_TEACH", "ORG_POSY", TubeOrgPosY, PathConfig);

                        CoolingBasePosX = Read("COOL_TEACH", "BASE_POSX", CoolingBasePosX, PathConfig);
                        CoolingBasePosY = Read("COOL_TEACH", "BASE_POSY", CoolingBasePosY, PathConfig); 
                        CoolingOffsetX = Read("COOL_TEACH", "OFFSET_X", CoolingOffsetX, PathConfig);
                        CoolingOffsetY = Read("COOL_TEACH", "OFFSET_Y", CoolingOffsetY, PathConfig);
                        CoolingOrgPosX = Read("COOL_TEACH", "ORG_POSX", CoolingOrgPosX, PathConfig);
                        CoolingOrgPosY = Read("COOL_TEACH", "ORG_POSY", CoolingOrgPosY, PathConfig);

                        Pipett_offsetX_5ml     = Read("PIPETT_OFFSET", "5ML_OFFSET_X", Pipett_offsetX_5ml, PathConfig);
                        Pipett_offsetY_5ml     = Read("PIPETT_OFFSET", "5ML_OFFSET_Y", Pipett_offsetY_5ml, PathConfig);
                        Pipett_offsetZ_5ml     = Read("PIPETT_OFFSET", "5ML_OFFSET_Z", Pipett_offsetZ_5ml, PathConfig);
                        Pipett_offsetX_gripper = Read("PIPETT_OFFSET", "GRIPPER_OFFSET_X", Pipett_offsetX_gripper, PathConfig);
                        Pipett_offsetY_gripper = Read("PIPETT_OFFSET", "GRIPPER_OFFSET_Y", Pipett_offsetY_gripper, PathConfig);
                        Pipett_offsetZ_gripper = Read("PIPETT_OFFSET", "GRIPPER_OFFSET_Z", Pipett_offsetZ_gripper, PathConfig);
                        Pipett_offsetX_laser   = Read("PIPETT_OFFSET", "LASER_OFFSET_X", Pipett_offsetX_laser, PathConfig);
                        Pipett_offsetY_laser   = Read("PIPETT_OFFSET", "LASER_OFFSET_Y", Pipett_offsetY_laser, PathConfig);
                        Pipett_offsetZ_laser   = Read("PIPETT_OFFSET", "LASER_OFFSET_Z", Pipett_offsetZ_laser, PathConfig);
                        laser_Z_dist           = Read("PIPETT_OFFSET", "LASER_Z_DIST", laser_Z_dist, PathConfig);

                        Pipett_offsetX_1ml = Read("PIPETT_OFFSET", "1ML_OFFSET_X", Pipett_offsetX_1ml, PathConfig);
                        Pipett_offsetY_1ml = Read("PIPETT_OFFSET", "1ML_OFFSET_Y", Pipett_offsetY_1ml, PathConfig);
                        Pipett_offsetZ_1ml = Read("PIPETT_OFFSET", "1ML_OFFSET_Z", Pipett_offsetZ_1ml, PathConfig);

                        Pipett_offsetZ_10ul  = Read("PIPETT_OFFSET", "10UL_OFFSET_Z", Pipett_offsetZ_10ul, PathConfig);
                        Pipett_offsetZ_300ul = Read("PIPETT_OFFSET", "300UL_OFFSET_Z", Pipett_offsetZ_300ul, PathConfig);
                        Pipett_offsetZ_CalibTip = Read("PIPETT_OFFSET", "CALIB_TIP_OFFSET_Z", Pipett_offsetZ_CalibTip, PathConfig);

                        Pos_AxisX.Clear();
                        int count = Read("Pos_AxisX", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefinePos pos = new DefinePos();
                            pos.Name = Read($"POS_AXIS_X_{i + 1}", "NAME", "", PathConfig);
                            pos.Speed = Read($"POS_AXIS_X_{i + 1}", "SPEED", "", PathConfig);
                            pos.Position = Read($"POS_AXIS_X_{i + 1}", "POSITION", "", PathConfig);
                            pos.Acc = Read($"POS_AXIS_X_{i + 1}", "ACC", "", PathConfig);
                            pos.Dec = Read($"POS_AXIS_X_{i + 1}", "DEC", "", PathConfig);
                            Pos_AxisX.Add(pos);
                        }

                        Pos_AxisY.Clear();
                        count = Read("Pos_AxisY", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefinePos pos = new DefinePos();
                            pos.Name = Read($"POS_AXIS_Y_{i + 1}", "NAME", "", PathConfig);
                            pos.Speed = Read($"POS_AXIS_Y_{i + 1}", "SPEED", "", PathConfig);
                            pos.Position = Read($"POS_AXIS_Y_{i + 1}", "POSITION", "", PathConfig);
                            pos.Acc = Read($"POS_AXIS_Y_{i + 1}", "ACC", "", PathConfig);
                            pos.Dec = Read($"POS_AXIS_Y_{i + 1}", "DEC", "", PathConfig);
                            Pos_AxisY.Add(pos);
                        }

                        Pos_AxisZ.Clear();
                        count = Read("Pos_AxisZ", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefinePos pos = new DefinePos();
                            pos.Name = Read($"POS_AXIS_Z_{i + 1}", "NAME", "", PathConfig);
                            pos.Speed = Read($"POS_AXIS_Z_{i + 1}", "SPEED", "", PathConfig);
                            pos.Position = Read($"POS_AXIS_Z_{i + 1}", "POSITION", "", PathConfig);
                            pos.Acc = Read($"POS_AXIS_Z_{i + 1}", "ACC", "", PathConfig);
                            pos.Dec = Read($"POS_AXIS_Z_{i + 1}", "DEC", "", PathConfig);
                            Pos_AxisZ.Add(pos);
                        }

                        Pos_AxisGripper.Clear();
                        count = Read("Pos_AxisGripper", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefinePos pos = new DefinePos();
                            pos.Name = Read($"POS_AXIS_GRIPPER_{i + 1}", "NAME", "", PathConfig);
                            pos.Speed = Read($"POS_AXIS_GRIPPER_{i + 1}", "SPEED", "", PathConfig);
                            pos.Position = Read($"POS_AXIS_GRIPPER_{i + 1}", "POSITION", "", PathConfig);
                            pos.Acc = Read($"POS_AXIS_GRIPPER_{i + 1}", "ACC", "", PathConfig);
                            pos.Dec = Read($"POS_AXIS_GRIPPER_{i + 1}", "DEC", "", PathConfig);
                            Pos_AxisGripper.Add(pos);
                        }

                        Pos_AxisPipett.Clear();
                        count = Read("Pos_AxisPipett", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefinePos pos = new DefinePos();
                            pos.Name = Read($"POS_AXIS_PIPETT_{i + 1}", "NAME", "", PathConfig);
                            pos.Speed = Read($"POS_AXIS_PIPETT_{i + 1}", "SPEED", "", PathConfig);
                            pos.Position = Read($"POS_AXIS_PIPETT_{i + 1}", "POSITION", "", PathConfig);
                            pos.Acc = Read($"POS_AXIS_PIPETT_{i + 1}", "ACC", "", PathConfig);
                            pos.Dec = Read($"POS_AXIS_PIPETT_{i + 1}", "DEC", "", PathConfig);
                            Pos_AxisPipett.Add(pos);
                        }

                        Pos_WorldPos.Clear();
                        count = Read("Pos_WorldPos", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefineWorldPos world_pos = new DefineWorldPos();
                            world_pos.Idx = Read($"POS_WORLDPOS_{i + 1}", "IDX", "", PathConfig);
                            world_pos.Name = Read($"POS_WORLDPOS_{i + 1}", "NAME", "", PathConfig);
                            world_pos.X = Read($"POS_WORLDPOS_{i + 1}", "X", "", PathConfig);
                            world_pos.Y = Read($"POS_WORLDPOS_{i + 1}", "Y", "", PathConfig);
                            world_pos.Z = Read($"POS_WORLDPOS_{i + 1}", "Z", "", PathConfig);
                            world_pos.Gripper = Read($"POS_WORLDPOS_{i + 1}", "GRIPPER", "", PathConfig);
                            world_pos.Pipett = Read($"POS_WORLDPOS_{i + 1}", "PIPETT", "", PathConfig);
                            Pos_WorldPos.Add(world_pos);
                        }

                        ToolOffset.Clear();
                        count = Read("Tool_Offset", "COUNT", 0, PathConfig);
                        for (int i = 0; i < count; i++)
                        {
                            DefineToolOffset offset = new DefineToolOffset();
                            offset.Idx = Read($"TOOLOFFSET_{i + 1}", "IDX", "", PathConfig);
                            offset.Name = Read($"TOOLOFFSET_{i + 1}", "NAME", "", PathConfig);
                            offset.Z_Dist = Read($"TOOLOFFSET_{i + 1}", "Z_DIST", "", PathConfig);
                            offset.X = Read($"TOOLOFFSET_{i + 1}", "X", "", PathConfig);
                            offset.Y = Read($"TOOLOFFSET_{i + 1}", "Y", "", PathConfig);
                            offset.Z = Read($"TOOLOFFSET_{i + 1}", "Z", "", PathConfig);
                            ToolOffset.Add(offset);
                        }
                    }
                }
                else if (rw == RW.WRITE)
                {
                    Write("SERIAL", "PORT", ComPort, PathConfig);

                    Write("TIMER", "INTERVAL", TimerInterval, PathConfig);

                    Write("PARAMETER", "RPM", SpinRpm, PathConfig);
                    Write("PARAMETER", "PRESCALE", prescale, PathConfig);
                    Write("PARAMETER", "RISING_TIME", risingTime, PathConfig);
                    Write("PARAMETER", "FALLING_TIME", fallingTime, PathConfig);
                    Write("PARAMETER", "ROTOR_RADIUS", RotorRadius, PathConfig);
                    Write("PARAMETER", "SPIN_DURATION", strSpinDuration, PathConfig);
                    Write("PARAMETER", "RPM_OFFSET", SpinRpm_Offset, PathConfig);

                    Write("TEST_CONDITION", "COMMAND", TestCmd, PathConfig);
                    Write("TEST_CONDITION", "COUNT", TestCount, PathConfig);
                    Write("TEST_CONDITION", "INTERVAL", TestInterval, PathConfig);
                    Write("TEST_CONDITION", "LINE_DELAY", RecipeLineDelay, PathConfig);
                    Write("TEST_CONDITION", "PLAY_MUSIC", Convert.ToInt32(PlayMusicOntest), PathConfig);


                    Write("WINDOW", "TOP", FormTop, PathConfig);
                    Write("WINDOW", "LEFT", FormLeft, PathConfig);
                    Write("WINDOW", "WIDTH", FormWidth, PathConfig);
                    Write("WINDOW", "HEIGHT", FormHeight, PathConfig);
                    Write("WINDOW", "TOP_MOST", Convert.ToInt32(TopMost), PathConfig);

                    Write("FORM_COLOR", "THEME", Theme, PathConfig);
                    Write("FORM_COLOR", "SCHEME", ColorScheme, PathConfig);

                    Write("CAMERA", "TRIGGER_MODE", VisionTriggerMode, PathConfig);
                    Write("CAMERA", "TRIGGER_ACTIVATION", VisionTriggerActivation, PathConfig);
                    Write("CAMERA", "EXPOSURE_TIME_1", ExposureTime1, PathConfig);
                    Write("CAMERA", "GAIN_1", VisionGain1, PathConfig);
                    Write("CAMERA", "GAMMA_1", Gamma1, PathConfig);
                    Write("CAMERA", "FRAME_RATE_1", FrameRate1, PathConfig);
                    Write("CAMERA", "EXPOSURE_TIME_2", ExposureTime2, PathConfig);
                    Write("CAMERA", "GAIN_2", VisionGain2, PathConfig);
                    Write("CAMERA", "GAMMA_2", Gamma2, PathConfig);
                    Write("CAMERA", "FRAME_RATE_2", FrameRate2, PathConfig);
                    Write("CAMERA", "RECORD_FRAME_RATE", RecordFrameRate, PathConfig);
                    Write("CAMERA", "STROBE_PERIOD", Strobe_period, PathConfig);

                    Write("MISC", "MAX_VIDEO_FILE_NUMBER", MaxVideoFileNumber, PathConfig);
                    Write("MISC", "MAX_IMAGE_FILE_NUMBER", MaxImageFileNumber, PathConfig);

                    Write("MOTOR", "STEP_AXIS_X_SPEED", StepAxisX_Speed, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Y_SPEED", StepAxisY_Speed, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Z_SPEED", StepAxisZ_Speed, PathConfig);
                    Write("MOTOR", "STEP_AXIS_GRIPPER_SPEED", StepAxisGripper_Speed, PathConfig);
                    Write("MOTOR", "STEP_AXIS_PIPETT_SPEED", StepAxisPipett_Speed, PathConfig);                    
                    Write("MOTOR", "COVER_OPEN_SPEED", CoverOpen_Speed, PathConfig);
                    Write("MOTOR", "COVER_CLOSE_SPEED", CoverClose_Speed, PathConfig);
                    Write("MOTOR", "COVER_AXIS_ACC", StepCoverAcc, PathConfig);
                    Write("MOTOR", "COVER_AXIS_DEC", StepCoverDec, PathConfig);

                    Write("MOTOR", "STEP_AXIS_X_ACC", StepAxisX_Acc, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Y_ACC", StepAxisY_Acc, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Z_ACC", StepAxisZ_Acc, PathConfig);
                    Write("MOTOR", "STEP_AXIS_GRIPPER_ACC", StepAxisGripper_Acc, PathConfig);
                    Write("MOTOR", "STEP_AXIS_PIPETT_ACC", StepAxisPipett_Acc, PathConfig);
                    Write("MOTOR", "STEP_AXIS_ROTOR_COVER_ACC", RotorCover_Acc, PathConfig);

                    Write("MOTOR", "STEP_AXIS_X_DEC", StepAxisX_Dec, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Y_DEC", StepAxisY_Dec, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Z_DEC", StepAxisZ_Dec, PathConfig);
                    Write("MOTOR", "STEP_AXIS_GRIPPER_DEC", StepAxisGripper_Dec, PathConfig);
                    Write("MOTOR", "STEP_AXIS_PIPETT_DEC", StepAxisPipett_Dec, PathConfig);
                    Write("MOTOR", "STEP_AXIS_ROTOR_COVER_DEC", RotorCover_Dec, PathConfig);

                    Write("MOTOR", "CLLD_SPD", cLLD_Speed, PathConfig);

                    Write("MOTOR", "STEP_AXIS_X_POS", StepAxisX_Pos, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Y_POS", StepAxisY_Pos, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Z_POS", StepAxisZ_Pos, PathConfig);
                    Write("MOTOR", "STEP_AXIS_GRIPPER_POS", StepAxisGripper_Pos, PathConfig);
                    Write("MOTOR", "STEP_AXIS_PIPETT_POS", StepAxisPipett_Pos, PathConfig);
                    Write("MOTOR", "COVER_OPEN_POS", CoverOpen_Pos, PathConfig);
                    Write("MOTOR", "COVER_CLOSE_POS", CoverClose_Pos, PathConfig);

                    Write("MOTOR", "STEP_AXIS_X_JOG", StepAxisX_Jog, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Y_JOG", StepAxisY_Jog, PathConfig);
                    Write("MOTOR", "STEP_AXIS_Z_JOG", StepAxisZ_Jog, PathConfig);
                    Write("MOTOR", "STEP_AXIS_GRIPPER_JOG", StepAxisGripper_Jog, PathConfig);
                    Write("MOTOR", "STEP_AXIS_PIPETT_JOG", StepAxisPipett_Jog, PathConfig);
                    
                    Write("MOTOR", "ECCENTRIC", Eccentric, PathConfig);

                    Write("MOTOR", "FAST_MOVE_SPD_Z_AXIS", FastMoveSpd_ZAxis, PathConfig);
                    Write("MOTOR", "FAST_MOVE_SPD_GRIP_AXIS", FastMoveSpd_GripAxis, PathConfig);
                    Write("MOTOR", "FAST_MOVE_SPD_PIPETT_AXIS", FastMoveSpd_HamAxis, PathConfig);

                    Write("MOTOR", "FAST_MOVE_POS_Z_AXIS", FastMovePos_ZAxis, PathConfig);
                    Write("MOTOR", "FAST_MOVE_POS_GRIP_AXIS", FastMovePos_GripAxis, PathConfig);
                    Write("MOTOR", "FAST_MOVE_POS_PIPETT_AXIS", FastMovePos_HamAxis, PathConfig);

                    Write("MOTOR", "HOME_SEARCH_SPD_X_AXIS", HomeSearchSpd_X, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_Y_AXIS", HomeSearchSpd_Y, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_Z_AXIS", HomeSearchSpd_Z, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_GRIP_AXIS", HomeSearchSpd_Grip, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_PIPETT_AXIS", HomeSearchSpd_Ham, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_COVER_AXIS", HomeSearchSpd_Cover, PathConfig);
                    Write("MOTOR", "HOME_SEARCH_SPD_SERVO_AXIS", HomeSearchSpd_Servo, PathConfig);

                    Write("MOTOR", "HOME_OFFSET_POS_X_AXIS", HomeOffsetPos_X, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_Y_AXIS", HomeOffsetPos_Y, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_Z_AXIS", HomeOffsetPos_Z, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_GRIP_AXIS", HomeOffsetPos_Grip, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_PIPETT_AXIS", HomeOffsetPos_Ham, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_COVER_AXIS", HomeOffsetPos_Cover, PathConfig);
                    Write("MOTOR", "HOME_OFFSET_POS_SERVO_AXIS", HomeOffsetPos_Servo, PathConfig);

                    Write("SERVO", "SERVO_ACCDEC_MOVA", ServoAccDec_MovA, PathConfig);
                    Write("SERVO", "SERVO_RPM_MOVA", ServoRpm_MovA, PathConfig);
                    Write("SERVO", "SERVO_POS_CHAMBER1", ServoPos_Chamber1, PathConfig);
                    Write("SERVO", "SERVO_POS_CELLDOWN1", ServoPos_CellDown1, PathConfig);
                    Write("SERVO", "SERVO_POS_CHAMBER2", ServoPos_Chamber2, PathConfig);
                    Write("SERVO", "SERVO_POS_CELLDOWN2", ServoPos_CellDown2, PathConfig);

                    Write("TRI_PIPETT", "OFFSET_VOLUME", TriPipett_OffsetVol, PathConfig); 
                    Write("TRI_PIPETT", "LOAD_VOLUME", TriPipett_LoadingVol, PathConfig);
                    Write("TRI_PIPETT", "FLOW_RATE", TriPipett_FlowRate, PathConfig);

                    Write("PUMP", "FLOW_RATE", PumpFlowRate, PathConfig);
                    Write("PUMP", "LOAD_VOLUME", PumpLoadingVolume, PathConfig);
                    Write("PUMP", "OFFSET_VOLUME", PumpOffsetVolume, PathConfig);
                    Write("PUMP", "PORT_NO", PumpPortNo, PathConfig);
                    Write("PUMP", "VALVE_DELAY", PumpValveDelay, PathConfig);

                    Write("SENSOR", "FLOWMETER_UNIT_SCALE", FlowmeterUnitScale, PathConfig);
                    Write("SENSOR", "LOADCELL_ERR_WEIGHT", LoadcellErrWeight, PathConfig);

                    Write("PELT", "SET_TEMP", Pelt_set_temp, PathConfig);
                    Write("PELT", "FAN_ON_TEMP", Fan_on_temp, PathConfig);
                    Write("PELT", "FAN_OFF_TEMP", Fan_off_temp, PathConfig);
                    
                    Write("LIGHT", "ROTOR_LIGHT_BRIGHT", RotorLightBright, PathConfig);
                    Write("LIGHT", "ROOM_LIGHT_BRIGHT", RoomLightBright, PathConfig);

                    Write("TIP_TEACH", "BASE_POSX", TipBasePosX, PathConfig);
                    Write("TIP_TEACH", "BASE_POSY", TipBasePosY, PathConfig);
                    Write("TIP_TEACH", "OFFSET_X", TipOffsetX, PathConfig);
                    Write("TIP_TEACH", "OFFSET_Y", TipOffsetY, PathConfig);
                    Write("TIP_TEACH", "ORG_POSX", TipOrgPosX, PathConfig);
                    Write("TIP_TEACH", "ORG_POSY", TipOrgPosY, PathConfig);

                    Write("TUBE_TEACH", "BASE_POSX", TubeBasePosX, PathConfig);
                    Write("TUBE_TEACH", "BASE_POSY", TubeBasePosY, PathConfig);
                    Write("TUBE_TEACH", "OFFSET_X", TubeOffsetX, PathConfig);
                    Write("TUBE_TEACH", "OFFSET_Y", TubeOffsetY, PathConfig);
                    Write("TUBE_TEACH", "ORG_POSX", TubeOrgPosX, PathConfig);
                    Write("TUBE_TEACH", "ORG_POSY", TubeOrgPosY, PathConfig);

                    Write("COOL_TEACH", "BASE_POSX", CoolingBasePosX, PathConfig);
                    Write("COOL_TEACH", "BASE_POSY", CoolingBasePosY, PathConfig);
                    Write("COOL_TEACH", "OFFSET_X", CoolingOffsetX, PathConfig);
                    Write("COOL_TEACH", "OFFSET_Y", CoolingOffsetY, PathConfig);
                    Write("COOL_TEACH", "ORG_POSX", CoolingOrgPosX, PathConfig);
                    Write("COOL_TEACH", "ORG_POSY", CoolingOrgPosY, PathConfig);

                    Write("PIPETT_OFFSET", "5ML_OFFSET_X", Pipett_offsetX_5ml, PathConfig);
                    Write("PIPETT_OFFSET", "5ML_OFFSET_Y", Pipett_offsetY_5ml, PathConfig);
                    Write("PIPETT_OFFSET", "5ML_OFFSET_Z", Pipett_offsetZ_5ml, PathConfig);
                    Write("PIPETT_OFFSET", "GRIPPER_OFFSET_X", Pipett_offsetX_gripper, PathConfig);
                    Write("PIPETT_OFFSET", "GRIPPER_OFFSET_Y", Pipett_offsetY_gripper, PathConfig);
                    Write("PIPETT_OFFSET", "GRIPPER_OFFSET_Z", Pipett_offsetZ_gripper, PathConfig);
                    Write("PIPETT_OFFSET", "LASER_OFFSET_X", Pipett_offsetX_laser, PathConfig);
                    Write("PIPETT_OFFSET", "LASER_OFFSET_Y", Pipett_offsetY_laser, PathConfig);
                    Write("PIPETT_OFFSET", "LASER_OFFSET_Z", Pipett_offsetZ_laser, PathConfig);
                    Write("PIPETT_OFFSET", "LASER_Z_DIST", laser_Z_dist, PathConfig);

                    Write("PIPETT_OFFSET", "1ML_OFFSET_X", Pipett_offsetX_1ml, PathConfig);
                    Write("PIPETT_OFFSET", "1ML_OFFSET_Y", Pipett_offsetY_1ml, PathConfig);
                    Write("PIPETT_OFFSET", "1ML_OFFSET_Z", Pipett_offsetZ_1ml, PathConfig);

                    Write("PIPETT_OFFSET", "10UL_OFFSET_Z", Pipett_offsetZ_10ul, PathConfig);
                    Write("PIPETT_OFFSET", "300UL_OFFSET_Z", Pipett_offsetZ_300ul, PathConfig);
                    Write("PIPETT_OFFSET", "CALIB_TIP_OFFSET_Z", Pipett_offsetZ_CalibTip, PathConfig);

                    int count = Pos_AxisX.Count;
                    Write("Pos_AxisX", "COUNT", Pos_AxisX.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_AXIS_X_{i + 1}", "NAME", Pos_AxisX[i].Name, PathConfig);
                        Write($"POS_AXIS_X_{i + 1}", "SPEED", Pos_AxisX[i].Speed, PathConfig);
                        Write($"POS_AXIS_X_{i + 1}", "POSITION", Pos_AxisX[i].Position, PathConfig);
                        Write($"POS_AXIS_X_{i + 1}", "ACC", Pos_AxisX[i].Acc, PathConfig);
                        Write($"POS_AXIS_X_{i + 1}", "DEC", Pos_AxisX[i].Dec, PathConfig);
                    }

                    count = Pos_AxisY.Count;
                    Write("Pos_AxisY", "COUNT", Pos_AxisY.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_AXIS_Y_{i + 1}", "NAME", Pos_AxisY[i].Name, PathConfig);
                        Write($"POS_AXIS_Y_{i + 1}", "SPEED", Pos_AxisY[i].Speed, PathConfig);
                        Write($"POS_AXIS_Y_{i + 1}", "POSITION", Pos_AxisY[i].Position, PathConfig);
                        Write($"POS_AXIS_Y_{i + 1}", "ACC", Pos_AxisY[i].Acc, PathConfig);
                        Write($"POS_AXIS_Y_{i + 1}", "DEC", Pos_AxisY[i].Dec, PathConfig);
                    }

                    count = Pos_AxisZ.Count;
                    Write("Pos_AxisZ", "COUNT", Pos_AxisZ.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_AXIS_Z_{i + 1}", "NAME", Pos_AxisZ[i].Name, PathConfig);
                        Write($"POS_AXIS_Z_{i + 1}", "SPEED", Pos_AxisZ[i].Speed, PathConfig);
                        Write($"POS_AXIS_Z_{i + 1}", "POSITION", Pos_AxisZ[i].Position, PathConfig);
                        Write($"POS_AXIS_Z_{i + 1}", "ACC", Pos_AxisZ[i].Acc, PathConfig);
                        Write($"POS_AXIS_Z_{i + 1}", "DEC", Pos_AxisZ[i].Dec, PathConfig);
                    }

                    count = Pos_AxisGripper.Count;
                    Write("Pos_AxisGripper", "COUNT", Pos_AxisGripper.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_AXIS_GRIPPER_{i + 1}", "NAME", Pos_AxisGripper[i].Name, PathConfig);
                        Write($"POS_AXIS_GRIPPER_{i + 1}", "SPEED", Pos_AxisGripper[i].Speed, PathConfig);
                        Write($"POS_AXIS_GRIPPER_{i + 1}", "POSITION", Pos_AxisGripper[i].Position, PathConfig);
                        Write($"POS_AXIS_GRIPPER_{i + 1}", "ACC", Pos_AxisGripper[i].Acc, PathConfig);
                        Write($"POS_AXIS_GRIPPER_{i + 1}", "DEC", Pos_AxisGripper[i].Dec, PathConfig);
                    }

                    count = Pos_AxisPipett.Count;
                    Write("Pos_AxisPipett", "COUNT", Pos_AxisPipett.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_AXIS_PIPETT_{i + 1}", "NAME", Pos_AxisPipett[i].Name, PathConfig);
                        Write($"POS_AXIS_PIPETT_{i + 1}", "SPEED", Pos_AxisPipett[i].Speed, PathConfig);
                        Write($"POS_AXIS_PIPETT_{i + 1}", "POSITION", Pos_AxisPipett[i].Position, PathConfig);
                        Write($"POS_AXIS_PIPETT_{i + 1}", "ACC", Pos_AxisPipett[i].Acc, PathConfig);
                        Write($"POS_AXIS_PIPETT_{i + 1}", "DEC", Pos_AxisPipett[i].Dec, PathConfig);
                    }

                    count = Pos_WorldPos.Count;
                    Write("Pos_WorldPos", "COUNT", Pos_WorldPos.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"POS_WORLDPOS_{i + 1}", "IDX", Pos_WorldPos[i].Idx, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "NAME", Pos_WorldPos[i].Name, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "X", Pos_WorldPos[i].X, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "Y", Pos_WorldPos[i].Y, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "Z", Pos_WorldPos[i].Z, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "GRIPPER", Pos_WorldPos[i].Gripper, PathConfig);
                        Write($"POS_WORLDPOS_{i + 1}", "PIPETT", Pos_WorldPos[i].Pipett, PathConfig);
                    }

                    count = ToolOffset.Count;
                    Write("Tool_Offset", "COUNT", ToolOffset.Count, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        Write($"TOOLOFFSET_{i + 1}", "IDX", ToolOffset[i].Idx, PathConfig);
                        Write($"TOOLOFFSET_{i + 1}", "NAME", ToolOffset[i].Name, PathConfig);
                        Write($"TOOLOFFSET_{i + 1}", "Z_DIST", ToolOffset[i].Z_Dist, PathConfig);
                        Write($"TOOLOFFSET_{i + 1}", "X", ToolOffset[i].X, PathConfig);
                        Write($"TOOLOFFSET_{i + 1}", "Y", ToolOffset[i].Y, PathConfig);
                        Write($"TOOLOFFSET_{i + 1}", "Z", ToolOffset[i].Z, PathConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateToolOffsetData(RW rw)
        {
            if(rw == RW.READ)
            {
                if (!File.Exists(PathConfig))
                {
                    MessageBox.Show(string.Format("{0} file not found", PathConfig), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ComPort = "COM1";
                }
                else
                {
                    ToolOffset.Clear();
                    int count = Read("Tool_Offset", "COUNT", 0, PathConfig);
                    for (int i = 0; i < count; i++)
                    {
                        DefineToolOffset offset = new DefineToolOffset();
                        offset.Idx = Read($"TOOLOFFSET_{i + 1}", "IDX", "", PathConfig);
                        offset.Name = Read($"TOOLOFFSET_{i + 1}", "NAME", "", PathConfig);
                        offset.Z_Dist = Read($"TOOLOFFSET_{i + 1}", "Z_DIST", "", PathConfig);
                        offset.X = Read($"TOOLOFFSET_{i + 1}", "X", "", PathConfig);
                        offset.Y = Read($"TOOLOFFSET_{i + 1}", "Y", "", PathConfig);
                        offset.Z = Read($"TOOLOFFSET_{i + 1}", "Z", "", PathConfig);
                        ToolOffset.Add(offset);
                    }
                }
            }
            else if (rw == RW.WRITE)
            {
                int count = ToolOffset.Count;
                Write("Tool_Offset", "COUNT", ToolOffset.Count, PathConfig);
                for (int i = 0; i < count; i++)
                {
                    Write($"TOOLOFFSET_{i + 1}", "IDX", ToolOffset[i].Idx, PathConfig);
                    Write($"TOOLOFFSET_{i + 1}", "NAME", ToolOffset[i].Name, PathConfig);
                    Write($"TOOLOFFSET_{i + 1}", "Z_DIST", ToolOffset[i].Z_Dist, PathConfig);
                    Write($"TOOLOFFSET_{i + 1}", "X", ToolOffset[i].X, PathConfig);
                    Write($"TOOLOFFSET_{i + 1}", "Y", ToolOffset[i].Y, PathConfig);
                    Write($"TOOLOFFSET_{i + 1}", "Z", ToolOffset[i].Z, PathConfig);
                }
            }
        }

        public void ReadWriteJson(RW rw, string fileName="")
        {
            try
            {
                if (fileName == "")
                {
                    if (this.LastButtonFileName == "")
                        this.PathRecipe = new FileInfo(EXE + ".rcp").FullName.ToString();
                    else
                        this.PathRecipe = LastButtonFileName;
                }
                else
                    this.PathRecipe = fileName;

                if (rw == RW.READ)
                {
                    if (!File.Exists(this.PathRecipe))
                    {
                        MessageBox.Show(string.Format("{0} file not found", PathRecipe), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        ListRecipe.Clear();
                        buttons.Clear();
                        string json = System.IO.File.ReadAllText(this.PathRecipe);
                        ListRecipe = JsonConvert.DeserializeObject<List<Recipe>>(json);
                        buttons = JsonConvert.DeserializeObject<List<CMD_BUTTON>>(json);
                    }
                }
                else if (rw == RW.WRITE)
                {
                    string json = JsonConvert.SerializeObject(ListRecipe.ToArray());
                    //string json = JsonConvert.SerializeObject(buttons.ToArray());
                    //write string to file
                    System.IO.File.WriteAllText(this.PathRecipe, json);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void ReadWriteRecipe(RW rw, string fileName, ref List<Recipe> recipe)
        {
            if (fileName == null || fileName == "")
                return;

            //if (fileName != "" && (fileName.ToUpper()).Contains(".JSON"))
            //{
            //    ReadWriteJson(rw, fileName);
            //    return;
            //}
            try
            {
                //if (fileName == "")
                //{
                //    if (this.LastButtonFileName == "")                    
                //        this.PathRecipe = new FileInfo(EXE + ".rcp").FullName.ToString();
                //    else
                //        this.PathRecipe = LastButtonFileName;
                //}
                //else
                this.PathRecipe = fileName;

                if (rw == RW.READ)
                {
                    if (!File.Exists(this.PathRecipe))
                    {
                        MessageBox.Show(string.Format("{0} file not found", PathRecipe), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if(recipe != null)
                            recipe.Clear();
                        int count = Read("RECIPE", "COUNT", 0, PathRecipe);
                        for (int i=0; i< count; i++)
                        {
                            Recipe rcp = new Recipe();
                            rcp.Enable =        Read($"COMMAND_{i+1, 2:D2}", "ENABLE", 1, PathRecipe);
                            rcp.Command1 =   Read($"COMMAND_{i+1, 2:D2}", "COMMAND1", "", PathRecipe);
                            rcp.Command2 =   Read($"COMMAND_{i+1, 2:D2}", "COMMAND2", "", PathRecipe);
                            rcp.Param1 =        Read($"COMMAND_{i+1, 2:D2}", "PARAM1", "", PathRecipe);
                            rcp.Param2 =        Read($"COMMAND_{i+1, 2:D2}", "PARAM2", "", PathRecipe);
                            rcp.Param3 =        Read($"COMMAND_{i+1, 2:D2}", "PARAM3", "", PathRecipe);
                            rcp.Param4 =        Read($"COMMAND_{i+1, 2:D2}", "PARAM4", "", PathRecipe);
                            rcp.Param5 =        Read($"COMMAND_{i + 1,2:D2}", "PARAM5", "", PathRecipe);
                            rcp.Param6 =        Read($"COMMAND_{i + 1,2:D2}", "PARAM6", "", PathRecipe);
                            rcp.Param7 =        Read($"COMMAND_{i + 1,2:D2}", "PARAM7", "", PathRecipe);
                            rcp.Sleep =          Read($"COMMAND_{i+1, 2:D2}", "SLEEP", "", PathRecipe);
                            rcp.Comment =     Read($"COMMAND_{i+1, 2:D2}", "COMMENT", "", PathRecipe);
                            recipe.Add(rcp);
                        }                                               
                    }
                }
                else if (rw == RW.WRITE)
                {
                    int count = recipe.Count;
                    Write("RECIPE", "COUNT", recipe.Count, PathRecipe);
                    for(int i=0; i < count; i++)
                    {
                        Write($"COMMAND_{i + 1,2:D2}", "ENABLE", recipe[i].Enable, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "COMMAND1", recipe[i].Command1, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "COMMAND2", recipe[i].Command2, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM1", recipe[i].Param1, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM2", recipe[i].Param2, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM3", recipe[i].Param3, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM4", recipe[i].Param4, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM5", recipe[i].Param5, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM6", recipe[i].Param6, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "PARAM7", recipe[i].Param7, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "SLEEP", recipe[i].Sleep, PathRecipe);
                        Write($"COMMAND_{i + 1,2:D2}", "COMMENT", recipe[i].Comment, PathRecipe);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void ReadWriteLastButtonfile(RW rw)
        {
            try
            {
                //this.PathLastfile = new FileInfo(EXE + ".cfg").FullName.ToString();
                //this.PathLastfile = "C:\\TruNser_C2000\\lastfile.ini";
                this.PathLastfile = MainWindow.DIR_HOME + "\\lastfile.ini";
                if (rw == RW.READ)
                {
                    if (!File.Exists(PathLastfile))
                    {
                        MessageBox.Show(string.Format("{0} file not found", PathLastfile), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        LastButtonFileName = Read("LAST_BUTTON_FILE", "NAME", "", PathLastfile);
                    }
                }
                else if (rw == RW.WRITE)
                {

                    Write("LAST_BUTTON_FILE", "NAME", LastButtonFileName, PathLastfile);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public string Read(string Section, string Key, string Default, string path)
        {
            var RetVal = new StringBuilder(255);
            //string RetVal;
            GetPrivateProfileString(Section ?? EXE, Key, Default, RetVal, 255, path);
            return RetVal.ToString();
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void Write(string Section, string Key, string Value, string path)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, path);
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public int Read(string Section, string Key, int Default, string path)
        {
            int RetVal = 0;
            RetVal = GetPrivateProfileInt(Section ?? EXE, Key, Default, path);
            return RetVal;
        }

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void Write(string Section, string Key,  int Value, string path)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value.ToString(), path);
        }
    }
}