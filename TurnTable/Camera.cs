using System;
using System.Drawing;
using MvCamCtrl.NET;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using OpenCvSharp;
using OpenCvSharp.Extensions;
//using OpenCvSharp.UserInterface;
using OpenCvSharp.Blob;
//using OpenCvSharp.CPlusPlus;
using System.Threading;


namespace CytoDx
{
    public partial class MainWindow
    {
        //CvCapture cvCapture;
        VideoCapture cvCapture;
        VideoWriter cvVideoWriter;
        static Mat src = new Mat();
        static Mat dst = new Mat();

        VideoCapture videoCapture;
        Mat matFrame;


        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        static MyCamera m_pMyCamera = new MyCamera();
        public MyCamera.cbOutputExdelegate ImageCallback;
        public IntPtr pCallBackImage = IntPtr.Zero;
        bool m_bGrabbing;
        static string m_ImageFileName;

        // actual frame size: 1440 * 1080 
        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        UInt32 m_nBufSizeForDriver = 3072 * 2048 * 3;
        byte[] m_pBufForDriver = new byte[3072 * 2048 * 3];
        
        // ch:用于保存图像的缓存 | en:Buffer for saving image
        static UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        static byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];

        // ch:显示错误信息 | en:Show error message
        private void ShowErrorMsg(string csMessage, int nErrorNum, string title = "PROMPT")
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }

            iPrintf(errorMsg);//MessageBox.Show(errorMsg, title);
        }

        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }

        /************************************************************************
         *  @fn     IsColorData()
         *  @brief  判断是否是彩色数据
         *  @param  enGvspPixelType         [IN]           像素格式
         *  @return 成功，返回0；错误，返回-1 
         ************************************************************************/
        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }

        public void ImageCallbackFunc(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                //ConvertImage(pFrameInfo.nWidth, pFrameInfo.nHeight, ref pData);
                //IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
                //SaveCallbackImage(pData, ref pFrameInfo);
                ////IplImage.FromPixelData(pFrameInfo.nWidth, pFrameInfo.nHeight, 3, pData);
                ////pictureBox2.ImageIpl = srcImage;
                //if (isConverting)
                //{
                //    string path = ShowCallbackImage(pData, ref pFrameInfo);
                //    Cv2.CvtColor(src, src, ColorConversionCodes.BayerRG2BGR);
                //    pictureBox2.Image = src.ToBitmap();
                //    return;
                //}
                                
                ShowCallbackImage(pData, ref pFrameInfo);
                                
                if (isRecording == false)
                    return;
                if (m_current_running_row > -1)
                {
                    if (isRunning == false)
                        return;
                    if (m_bSaveRecord)
                        SaveCallbackRecord(pData, ref pFrameInfo);
                    else
                    {
                        string path = SaveCallbackImage(pData, ref pFrameInfo);
                        //Cv2.CvtColor(src, src, ColorConversionCodes.BayerRG2BGR);
                        //pictureBox2.Image =  src.ToBitmap();
                        //if(path != "")
                        //{
                        //    srcImage = new IplImage(path, LoadMode.AnyColor);
                        //    pictureBox2.ImageIpl = srcImage;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void btnSearchDevice_Click(object sender, EventArgs e)
        {
            DeviceListAcq();
        }

        public void DeviceListAcq()
        {
            int nRet;
            // ch:创建设备列表 en:Create Device List
            System.GC.Collect();
            cbDeviceList.Items.Clear();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (m_pDeviceList.nDeviceNum != 0)
            {
                cbDeviceList.SelectedIndex = 0;
            }
            else
            {
                SetCtrlWhenClose();
            }
        }

        public bool OpenCamera(int cameraIndex)
        {
            int nRet = -1;
            if (m_pDeviceList.nDeviceNum < 1)
            {
                ShowErrorMsg("No Device to Open !", 0);
                return false;
            }
            cbDeviceList.SelectedIndex = cameraIndex;
            // Get selected device information
            MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[cameraIndex],
                                                                   typeof(MyCamera.MV_CC_DEVICE_INFO));

            // Open device
            if (null == m_pMyCamera)
            {
                m_pMyCamera = new MyCamera();
                if (null == m_pMyCamera)
                {
                    return false;
                }
            }

            //CloseCamera();

            nRet = m_pMyCamera.MV_CC_CreateDevice_NET(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            nRet = m_pMyCamera.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_pMyCamera.MV_CC_DestroyDevice_NET();
                ShowErrorMsg("Device open fail!", nRet);
                return false;
            }

            Thread.Sleep(100);
            // en:Register image callback
            ImageCallback = new MyCamera.cbOutputExdelegate(ImageCallbackFunc);
            nRet = m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(ImageCallback, IntPtr.Zero);
            
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf("Register image callback failed!");
                return false;
            }

            //nRet = m_pMyCamera.MV_CC_SetIntValue_NET("Width", 640);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    iPrintf("Set Int Value failed:{0:x8}", nRet);
            //    return;
            //}

            //nRet = m_pMyCamera.MV_CC_SetIntValue_NET("Height", 480);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    iPrintf("Set Int Value failed:{0:x8}", nRet);
            //    return;
            //}

            //MyCamera.MVCC_INTVALUE stIntValue = new MyCamera.MVCC_INTVALUE();
            //nRet = m_pMyCamera.MV_CC_GetIntValue_NET("WidthMax", ref stIntValue);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    iPrintf("Get Int Value failed:{0:x8}", nRet);
            //    return;
            //}

            //nRet = m_pMyCamera.MV_CC_GetIntValue_NET("HeightMax", ref stIntValue);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    iPrintf("Get Int Value failed:{0:x8}", nRet);
            //    return;
            //}
            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = m_pMyCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = m_pMyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        iPrintf($"Warning: Set Packet Size failed {nRet:x8}");
                    }
                }
                else
                {
                    iPrintf($"Warning: Get Packet Size failed {nPacketSize:x8}");
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
            //m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);    // ch:连续模式 | en:Continuous
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);    // ch:连续模式 | en:Continuous

            //btnGetCameraParam_Click(null, null);// ch:获取参数 | en:Get parameters

            // ch:控件操作 | en:Control operation
            SetCtrlWhenOpen();
            return true;
        }

        public bool CloseCamera()
        {
            // ch:关闭设备 | en:Close Device
            int nRet;

            nRet = m_pMyCamera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            nRet = m_pMyCamera.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            // ch:控件操作 | en:Control Operation
            SetCtrlWhenClose();

            // ch:取流标志位清零 | en:Reset flow flag bit
            m_bGrabbing = false;
            isRecording = false;
            return true;
        }

        private void bnOpenCamera_Click(object sender, EventArgs e)
        {
            if (m_pDeviceList.nDeviceNum == 0 || cbDeviceList.SelectedIndex == -1)
            {
                //ShowErrorMsg("No device, please select", 0);
                iPrintf("No device, please select");
                return;
            }

            OpenCamera(cbDeviceList.SelectedIndex);
            SetCameraParam(cbDeviceList.SelectedIndex);
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            CloseCamera();
        }

        private void SetCtrlWhenOpen()
        {
            btnSearchDevice.Enabled = false;
            CameraStatus.Enabled = true;
            btnOpenDevice.Enabled = false;
            btnCloseDevice.Enabled = true;
            btnStartGrab.Enabled = true;
            btnStopGrab.Enabled = false;
            radioContinuesMode.Enabled = true;
            radioTriggerMode.Enabled = true;
            radioTriggerMode.Checked = true;

            if (cbDeviceList.SelectedIndex == 0)
            {
                tbExposure1.Enabled = true;
                tbGain1.Enabled = true;
                tbGamma1.Enabled = true;
                tbFrameRate1.Enabled = true;
            }
            if (cbDeviceList.SelectedIndex == 1)
            {
                tbExposure2.Enabled = true;
                tbGain2.Enabled = true;
                tbGamma2.Enabled = true;
                tbFrameRate2.Enabled = true;
            }

            btnGetCameraParam.Enabled = true;
            btnSetCameraParam.Enabled = true;
        }

        private void SetCtrlWhenClose()
        {
            CameraStatus.Enabled = false;
            btnSearchDevice.Enabled = true;
            btnOpenDevice.Enabled = true;
            btnCloseDevice.Enabled = false;
            btnStartGrab.Enabled = false;
            btnStopGrab.Enabled = false;
            radioContinuesMode.Enabled = false;
            radioTriggerMode.Enabled = false;

            //bnSaveJpg.Enabled = false;

            tbExposure1.Enabled = false;
            tbGain1.Enabled = false;
            tbGamma1.Enabled = false;
            tbFrameRate1.Enabled = false;


            tbExposure2.Enabled = false;
            tbGain2.Enabled = false;
            tbGamma2.Enabled = false;
            tbFrameRate2.Enabled = false;

            btnGetCameraParam.Enabled = false;
            btnSetCameraParam.Enabled = false;
        }

        private void bnContinuesMode_CheckedChanged(object sender, EventArgs e)
        {
            if (radioContinuesMode.Checked)
            {
                m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                //chkSoftTrigger.Enabled = false;
                //btnSofTriggerExec.Enabled = false;
                // Trigger Activation = Rising Edge:0, Falling Edge:1, Level High:2, Level Low:3
                
            }
        }

        private void bnTriggerMode_CheckedChanged(object sender, EventArgs e)
        {
            // ch:打开触发模式 | en:Open Trigger Mode
            if (radioTriggerMode.Checked && btnCloseDevice.Enabled)
            {
                m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
                int nRet = m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerActivation", 2); // Set as Level High
                if (MyCamera.MV_OK != nRet)
                {
                    ShowErrorMsg("Set Trigger Activation Level High Failed", nRet);
                }
                // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
                //           1 - Line1;
                //           2 - Line2;
                //           3 - Line3;
                //           4 - Counter;
                //           7 - Software;
                //if (chkSoftTrigger.Checked)
                //{
                //    m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
                //    if (m_bGrabbing)
                //    {
                //        btnSofTriggerExec.Enabled = true;
                //    }
                //}
                //else
                //{
                //m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
                //}
                //chkSoftTrigger.Enabled = true;
            }

        }

        public void SetCtrlWhenStartGrab()
        {
            btnStartGrab.Enabled = false;
            btnStopGrab.Enabled = true;

            //if (radioTriggerMode.Checked && chkSoftTrigger.Checked)
            //{
            //    btnSofTriggerExec.Enabled = true;
            //}

            btnLoadImage.Enabled = false;
            btnClipOpen.Enabled = false;
            btnClipPlay.Enabled = false;
            btnClipStop.Enabled = false;
            btnClipForward.Enabled = false;
            btnClipBackward.Enabled = false;
            btnClipOpen.Enabled = false;
            //btnSaveImage.Enabled = false;
            btnImageFolder.Enabled = false;
            btnVideoFolder.Enabled = false;
            //btnSetCameraParam.Enabled = false;
            //btnGetCameraParam.Enabled = false;
        }

        public bool StartGrab()
        {
            int nRet;
            StopRecord();
            // ch:开始采集 | en:Start Grabbing
            nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Trigger Fail!", nRet);
                return false;
            }

            // ch:控件操作 | en:Control Operation
            SetCtrlWhenStartGrab();
            
            // ch:标志位置位true | en:Set position bit true
            m_bGrabbing = true;

            // ch:显示 | en:Display
            nRet = m_pMyCamera.MV_CC_Display_NET(pictureBox16.Handle);

            // Rotate Img
            //Image img = pictureBox1.Image;
            //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            //pictureBox1.Image = img;

            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Display Fail！", nRet);
            }
            if (MyCamera.MV_OK != nRet)
                return false;
            else
                return true;
        }

        private void btnStartGrab_Click(object sender, EventArgs e)
        {
            StartGrab();
        }

        private void cbSoftTrigger_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkSoftTrigger.Checked)
            //{

            //    // ch:触发源设为软触发 | en:Set trigger source as Software
            //    m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
            //    if (m_bGrabbing)
            //    {
            //        btnSofTriggerExec.Enabled = true;
            //    }
            //}
            //else
            //{
            //    m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 0);
            //    btnSofTriggerExec.Enabled = false;
            //}
        }

        private void bnTriggerExec_Click(object sender, EventArgs e)
        {
            int nRet;

            // ch:触发命令 | en:Trigger command
            nRet = m_pMyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Trigger Fail!", nRet);
            }
        }

        private void SetCtrlWhenStopGrab()
        {
            btnStartGrab.Enabled = true;
            btnStopGrab.Enabled = false;

            btnLoadImage.Enabled = true;
            btnClipOpen.Enabled = true;
            btnClipPlay.Enabled = true;
            btnClipStop.Enabled = true;
            btnClipForward.Enabled = true;
            btnClipBackward.Enabled = true;
            btnClipOpen.Enabled = true;
            btnImageFolder.Enabled = true;
            btnVideoFolder.Enabled = true;
            btnSaveImage.Enabled = true;
            btnSetCameraParam.Enabled = true;
            btnGetCameraParam.Enabled = true;
        }

        public int StopGrab()
        {
            if (m_bGrabbing == false)
                return 0;
            int nRet = -1;
            // ch:停止采集 | en:Stop Grabbing
            nRet = m_pMyCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Stop Grabbing Fail!", nRet);
            }

            // ch:标志位设为false | en:Set flag bit false
            m_bGrabbing = false;
            isRecording = false;
            // ch:控件操作 | en:Control Operation
            SetCtrlWhenStopGrab();
            return nRet;
        }

        private void bnStopGrab_Click(object sender, EventArgs e)
        {
            StopGrab();
        }

        private void bnSaveBmp_Click(object sender, EventArgs e)
        {
            string path = GetImagePath("bmp");
            pictureBox1.Image.Save(path);
            iPrintf($"Image has been saved to {path}");
            UpdateDiskInformation();
            return;
        }

        public void ShowCallbackImage(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            //IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            pCallBackImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();

            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg;
            stSaveParam.enPixelType = pFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = pFrameInfo.nFrameLen;
            stSaveParam.nHeight = pFrameInfo.nHeight;
            stSaveParam.nWidth = pFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pCallBackImage;
            stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
            stSaveParam.nImageLen = 10;
            stSaveParam.nJpgQuality = 99;
            int nRet = m_pMyCamera.MV_CC_SaveImageEx_NET(ref stSaveParam);
            
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf("Show Fail!");
                return;
            }

            string path = $"{DIR_IMAGE}\\runimg.jpg";
            try
            {
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                file.Write(m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
                file.Close();

                Bitmap img = new Bitmap(path, true);
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                //Size resize = new Size(pFrameInfo.nWidth, pFrameInfo.nHeight);
                pictureBox1.Image = (Image) img;

                // Rotation된 이미지를 파일로 확인
                //string path2 = $"{DIR_IMAGE}\\runimg2.jpg";
                //Bitmap img2 = (Bitmap) img.Clone();
                //img2.Save(path2, ImageFormat.Jpeg);
                //img2.Dispose();
            }
            catch (Exception ex)
            {
                string log = ex.Message.ToString();
                //iPrintf(ex.Message.ToString());
                //return "";
            }
            //this.Invoke(new MethodInvoker(delegate ()
            //{
            //    label_save_file_name.Text = path;
            //}));

            //UpdateDiskInformation();
            return;
        }

        static string SaveCallbackImage(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg;
            stSaveParam.enPixelType = pFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = pFrameInfo.nFrameLen;
            stSaveParam.nHeight = pFrameInfo.nHeight;
            stSaveParam.nWidth = pFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
            stSaveParam.nJpgQuality = 80;
            int nRet = m_pMyCamera.MV_CC_SaveImageEx_NET(ref stSaveParam);

            //src = new Mat(pFrameInfo.nHeight, pFrameInfo.nWidth, MatType.CV_8U, m_pBufForSaveImage);
            //img = IplImage.FromPixelData(pFrameInfo.nWidth, pFrameInfo.nHeight, channels:3, data:m_pBufForSaveImage);
            //src = IplImage.FromPixelData(pFrameInfo.nWidth, pFrameInfo.nHeight, 3, m_pBufForSaveImage);
            //src = IplImage.FromPixelData(pFrameInfo.nWidth, pFrameInfo.nHeight, BitDepth.U8, 3, m_pBufForSaveImage);
            //src = IplImage.FromPixelData(new CvSize(pFrameInfo.nWidth, pFrameInfo.nHeight), 3, m_pBufForSaveImage);


            if (MyCamera.MV_OK != nRet)
            {
                //iPrintf("Save Fail!");
                return "";
            }
            string path = GetImagePath("jpg");
            try
            {
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                file.Write(m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
                file.Close();
                m_ImageFileName = path;
            }
            catch (Exception ex)
            {
                string log = ex.Message.ToString();
                //iPrintf(ex.Message.ToString());
                return "";
            }
            //this.Invoke(new MethodInvoker(delegate ()
            //{
            //    label_save_file_name.Text = path;
            //}));

            //UpdateDiskInformation();
            return path;
        }

        public void SaveCallbackRecord(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            int nRet = MyCamera.MV_OK;
            if (pData == IntPtr.Zero)
            {
                return;
            }
            MyCamera.MV_CC_INPUT_FRAME_INFO stInputFrameInfo = new MyCamera.MV_CC_INPUT_FRAME_INFO();

            if (nRet == MyCamera.MV_OK)
            {
                stInputFrameInfo.pData = pData;
                stInputFrameInfo.nDataLen = pFrameInfo.nFrameLen;
                nRet = m_pMyCamera.MV_CC_InputOneFrame_NET(ref stInputFrameInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    iPrintf($"Input one frame failed: nRet {nRet:x8}");
                }
            }
            else
            {
                iPrintf($"No data:{nRet:x8}");
            }
        }

        public bool SetCameraParam(int cameraIndex)
        {
            int nRet;
            m_pMyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);

            float Exposure = 0.0f;
            float Gain = 0.0f;
            float FrameRate = 0.0f;
            float Gamma = 0.0f;
            int ReverseScanDirection = 0;
            try
            {
                if (cameraIndex == 0)
                {
                    Exposure = float.Parse(tbExposure1.Text);
                    Gain = float.Parse(tbGain1.Text);
                    FrameRate = float.Parse(tbFrameRate1.Text);
                    Gamma = float.Parse(tbGamma1.Text);
                    ReverseScanDirection = int.Parse(tbReverseDirection1.Text);
                }
                if (cameraIndex == 1)
                {
                    Exposure = float.Parse(tbExposure2.Text);
                    Gain = float.Parse(tbGain2.Text);
                    FrameRate = float.Parse(tbFrameRate2.Text);
                    Gamma = float.Parse(tbGamma2.Text);
                    ReverseScanDirection = int.Parse(tbReverseDirection2.Text);
                }

                nRet = m_pMyCamera.MV_CC_SetBoolValue_NET("GammaEnable", true);
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set Gamma Enable Fail!", nRet);
                }

                nRet = m_pMyCamera.MV_CC_SetGammaSelector_NET((int)MyCamera.MV_CAM_GAMMA_SELECTOR.MV_GAMMA_SELECTOR_SRGB);
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set Gamma Selector Fail!", nRet);
                }
                
                nRet = m_pMyCamera.MV_CC_SetGamma_NET(Gamma);
                if (nRet != MyCamera.MV_OK)
                {
                    //ShowErrorMsg("Set Gamma Fail!", nRet);
                }

                nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("ExposureTime", Exposure);
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set Exposure Time Fail!", nRet);
                }
                
                m_pMyCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
                nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("Gain", Gain);
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set Gain Fail!", nRet);
                }

                nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", FrameRate);
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set Frame Rate Fail!", nRet);
                }

                //nRet = m_pMyCamera.MV_CC_SetEnumValue_NET("GammaSelector", (int)MyCamera.MV_CAM_GAMMA_SELECTOR.MV_GAMMA_SELECTOR_USER);
                //if (nRet != MyCamera.MV_OK)
                //{
                //    ShowErrorMsg("Set Gamma Selector Fail!", nRet);
                //}

                //nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("Gamma", float.Parse("1.0"));
                //if (nRet != MyCamera.MV_OK)
                //{
                //    ShowErrorMsg("Set Gamma Selector Fail!", nRet);
                //}

                //nRet = m_pMyCamera.MV_CC_SetGammaSelector_NET((int)MyCamera.MV_CAM_GAMMA_SELECTOR.MV_GAMMA_SELECTOR_USER);


                // reverse 테스트 중
                //nRet = m_pMyCamera.MV_CC_SetIntValue_NET("ReverseScanDirection", (uint)ReverseScanDirection);
                //nRet = m_pMyCamera.MV_CC_SetBoolValue_NET("ReverseScanDirection", true);
                //if (nRet != MyCamera.MV_OK)
                //{
                //    ShowErrorMsg("Set ReverseScanDirection Fail!", nRet);
                //}

                //nRet = m_pMyCamera.MV_CC_SetBoolValue_NET("ReverseX", ReverseScanDirection > 0 ? true : false);
                //if (nRet != MyCamera.MV_OK)
                //{
                //    ShowErrorMsg("Set ReverseX Fail!", nRet);
                //}

                //nRet = m_pMyCamera.MV_CC_SetBoolValue_NET("ReverseY", ReverseScanDirection > 0 ? true : false);
                //if (nRet != MyCamera.MV_OK)
                //{
                //    ShowErrorMsg("Set ReverseY Fail!", nRet);
                //}
            }
            catch
            {
                ShowErrorMsg("Please enter correct type!", 0);
                return false;
            }
            if (nRet != MyCamera.MV_OK)
                return false;
            return true;
        }

        public void GetCameraParam(int cameraIndex)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            //MyCamera.MVCC_INTVALUE intVal = new MyCamera.MVCC_INTVALUE();
            //bool bBoolVal = false;

            int nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                if (cameraIndex == 0)
                    tbExposure1.Text = stParam.fCurValue.ToString("F1");
                if (cameraIndex == 1)
                    tbExposure2.Text = stParam.fCurValue.ToString("F1");
            }
            else
                iPrintf("Get Paramter Fail (ExposureTime)");

            nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                //m_pMyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
                if (cameraIndex == 0)
                    tbGain1.Text = stParam.fCurValue.ToString("F1");
                if (cameraIndex == 1)
                    tbGain2.Text = stParam.fCurValue.ToString("F1");
            }
            else
                iPrintf("Get Paramter Fail (Gain)");

            nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("AcquisitionFrameRate", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                //m_pMyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
                if (cameraIndex == 0)
                    tbFrameRate1.Text = stParam.fCurValue.ToString("F1");
                if (cameraIndex == 1)
                    tbFrameRate2.Text = stParam.fCurValue.ToString("F1");
            }
            else
                iPrintf("Get Paramter Fail (AcquisitionFrameRate)");

            nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("Gamma", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                //m_pMyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
                if (cameraIndex == 0)
                    tbGamma1.Text = stParam.fCurValue.ToString("F1");
                if (cameraIndex == 1)
                    tbGamma2.Text = stParam.fCurValue.ToString("F1");
            }
            else
                iPrintf("Get Paramter Fail (Gamma)");

            //nRet = m_pMyCamera.MV_CC_GetIntValue_NET("ReverseScanDirection", ref intVal);
            //if (MyCamera.MV_OK == nRet)
            //{
            //    //m_pMyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
            //    if (cameraIndex == 0)
            //        tbReverseDirection1.Text = intVal.nCurValue.ToString();
            //    if (cameraIndex == 1)
            //        tbReverseDirection2.Text = intVal.nCurValue.ToString();
            //}
            //else
            //    iPrintf("Get Paramter Fail (ReverseScanDirection)");

            //nRet = m_pMyCamera.MV_CC_GetBoolValue_NET("ReverseX", ref bBoolVal);
            //if (MyCamera.MV_OK == nRet)
            //{
            //    //if (cameraIndex == 0)
            //    //    tbReverseDirection1.Text = intVal.nCurValue.ToString();
            //    //if (cameraIndex == 1)
            //    //    tbReverseDirection2.Text = intVal.nCurValue.ToString();
            //}
            //else
            //    iPrintf("Get Paramter Fail (ReverseX)");

            //nRet = m_pMyCamera.MV_CC_GetBoolValue_NET("ReverseY", ref bBoolVal);
            //if (MyCamera.MV_OK == nRet)
            //{
            //    //if (cameraIndex == 0)
            //    //    tbReverseDirection1.Text = intVal.nCurValue.ToString();
            //    //if (cameraIndex == 1)
            //    //    tbReverseDirection2.Text = intVal.nCurValue.ToString();
            //}
            //else
            //    iPrintf("Get Paramter Fail (ReverseY)");
        }

        private void btnSetCameraParam_Click(object sender, EventArgs e)
        {
            //SetCameraParam(cbDeviceList.SelectedIndex);
            SetCameraParam(0);
        }

        public bool SetCameraParameter(string fps)
        {
            try
            {
                int nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", float.Parse(fps));
                if (nRet != MyCamera.MV_OK)
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }
        static string GetImagePath(string fmt = "chk")
        {
            DateTime dtNow = DateTime.Now;
            string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f") + (m_current_running_row > -1 ? $"_{m_current_running_row + 1:00}" : "_XX");
            return $"{DIR_IMAGE}\\{dtStr}.{fmt}";
        }

        public string GetVideoPath()
        {
            return $"{DIR_VIDEO}\\{label_TimeStamp.Text}.avi";
        }

        public string GetVideoLogPath()
        {
            string dtStr = label_TimeStamp.Text;
            if (dtStr == "")
            {
                DateTime dtNow = DateTime.Now;
                dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f");
            }
            return $"{DIR_VIDEO_LOG}\\{dtStr}.txt";
        }

        static uint g_nPayloadSize = 0;


        public bool StartRecord(float fFPS)
        {
            int nRet = MyCamera.MV_OK;
            bool retVal = true;

            StopRecord();
            // ch:设置触发模式为off || en:set trigger mode as off
            //if (MyCamera.MV_OK != device.MV_CC_SetEnumValue_NET("TriggerMode", 0))
            //{
            //    iPrintf("Set TriggerMode failed!");
            //    break;
            //}

            // ch:获取包大小 || en: Get Payload Size
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Get PayloadSize failed:{nRet:x8}");
                return false;
            }
            g_nPayloadSize = stParam.nCurValue;

            MyCamera.MV_CC_RECORD_PARAM stRecordPar = new MyCamera.MV_CC_RECORD_PARAM();
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Get Width failed: nRet {nRet:x8}");
                return false;
            }
            stRecordPar.nWidth = (ushort)stParam.nCurValue;

            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Get Height failed: nRet {nRet:x8}");
                return false;
            }
            stRecordPar.nHeight = (ushort)stParam.nCurValue;

            MyCamera.MVCC_ENUMVALUE stEnumValue = new MyCamera.MVCC_ENUMVALUE();
            nRet = m_pMyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stEnumValue);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Get Width failed: nRet {nRet:x8}");
                return false;
            }
            stRecordPar.enPixelType = (MyCamera.MvGvspPixelType)stEnumValue.nCurValue;

            MyCamera.MVCC_FLOATVALUE stFloatValue = new MyCamera.MVCC_FLOATVALUE();
            nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stFloatValue);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Get Float value failed: nRet {nRet:x8}");
                return false;
            }
            // ch:帧率(1/16-120)fps | en:Frame Rate (1/16-120)fps
            //stRecordPar.fFrameRate = stFloatValue.fCurValue;
            stRecordPar.fFrameRate = fFPS;

            // ch:码率kbps(128kbps-16Mbps) | en:Bitrate kbps(128kbps-16Mbps)
            stRecordPar.nBitRate = 1000;
            // ch:录像格式(仅支持AVI) | en:Record Format(AVI is only supported)
            stRecordPar.enRecordFmtType = MyCamera.MV_RECORD_FORMAT_TYPE.MV_FormatType_AVI;
            stRecordPar.strFilePath = GetVideoPath();

            // ch:控件操作 | en:Control Operation
            SetCtrlWhenStartGrab();


            nRet = m_pMyCamera.MV_CC_StartRecord_NET(ref stRecordPar);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Start Record failed: nRet {nRet:x8}");
                //return false;
            }

            // ch:开启抓图 | en:start grab
            nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Start grabbing failed:{nRet:x8}");
                //return false;
            }


            // Trigger Activation = Rising Edge:0, Falling Edge:1, Level High:2, Level Low:3
            nRet = m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerActivation", 2); // Set as Level High
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Set Trigger Activation Level High Failed ({nRet:x8})");
                retVal = false;
            }

            // ch:标志位置位true | en:Set position bit true
            m_bGrabbing = true;


            // ch:显示 | en:Display
            nRet = m_pMyCamera.MV_CC_Display_NET(pictureBox16.Handle);
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Display Fail！ ({nRet:x8})");
                retVal = false;
            }
            if (retVal)
                isRecording = true;
            return retVal;
        }

        public bool StopRecord()
        {
            if (m_bGrabbing == false)
                return true;
            // ch:停止抓图 | en:Stop grab image
            int nRet = m_pMyCamera.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Stop grabbing failed ({nRet:x8})");
                //return false;
            }

            // ch:停止录像 | en:Stop record
            nRet = m_pMyCamera.MV_CC_StopRecord_NET();
            if (MyCamera.MV_OK != nRet)
            {
                iPrintf($"Stop Record failed ({nRet:x8})");
                //return false;
            }
            m_bGrabbing = false;
            isRecording = false;
            SetCtrlWhenStopGrab();
            return true;
        }

        private void btnGetCameraParam_Click(object sender, EventArgs e)
        {
            //GetCameraParam(cbDeviceList.SelectedIndex);
            GetCameraParam(0);
        }
        //private void makeAvi(string imageInputfolderName, string outVideoFileName, float fps = 12.0f, string imgSearchPattern = "*.png")
        //{   // reads all images in folder 
        //    VideoWriter w = new VideoWriter(outVideoFileName,
        //        new Accord.Extensions.Size(480, 640), fps, true);
        //    Accord.Extensions.Imaging.ImageDirectoryReader ir =
        //        new ImageDirectoryReader(imageInputfolderName, imgSearchPattern);
        //    while (ir.Position < ir.Length)
        //    {
        //        IImage i = ir.Read();
        //        w.Write(i);
        //    }
        //    w.Close();
        //}


        //public bool CreateVideo(List<Bitmap> bitmaps, string outputFile, double fps)
        //{
        //    int width = 640;
        //    int height = 480;
        //    if (bitmaps == null || bitmaps.Count == 0) return false;
        //    try
        //    {
        //        using (ITimeline timeline = new DefaultTimeline(fps))
        //        {
        //            IGroup group = timeline.AddVideoGroup(32, width, height);
        //            ITrack videoTrack = group.AddTrack();

        //            int i = 0;
        //            double miniDuration = 1.0 / fps;
        //            foreach (var bmp in bitmaps)
        //            {
        //                IClip clip = videoTrack.AddImage(bmp, 0, i * miniDuration, (i + 1) * miniDuration);
        //                System.Diagnostics.Debug.WriteLine(++i);

        //            }
        //            timeline.AddAudioGroup();
        //            IRenderer renderer = new WindowsMediaRenderer(timeline, outputFile, WindowsMediaProfiles.HighQualityVideo);
        //            renderer.Render();
        //        }
        //    }
        //    catch { return false; }
        //    return true;
        //}        


        IntPtr m_BufForDriver;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //voidMain(m_ImageFileName);
                //return;
                
                int nRet = 0;
                if (m_bGrabbing == false)
                {
                    nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
                    if (nRet == MyCamera.MV_OK)
                    {
                        m_bGrabbing = true;
                    }
                }

                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    iPrintf($"Get PayloadSize failed:{nRet:x8}");
                    return;
                }
                g_nPayloadSize = stParam.nCurValue;

                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                m_BufForDriver = Marshal.AllocHGlobal((Int32)g_nPayloadSize);

                if (m_BufForDriver == IntPtr.Zero)
                {
                    return;
                }

                MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

                nRet = m_pMyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, g_nPayloadSize, ref stFrameInfo, 1000);
                nRet = MyCamera.MV_OK;  //for test
                if (nRet == MyCamera.MV_OK)
                {
                    //m_stFrameInfo = stFrameInfo;
                }
                if (nRet == MyCamera.MV_OK)
                {
                    if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                    {
                        return;
                    }
                    //stDisplayInfo.hWnd = pictureBox2.Handle;
                    stDisplayInfo.hWnd = pictureBox16.Handle;
                    stDisplayInfo.pData = m_BufForDriver;
                    stDisplayInfo.nDataLen = stFrameInfo.nFrameLen;
                    stDisplayInfo.nWidth = stFrameInfo.nWidth;
                    stDisplayInfo.nHeight = stFrameInfo.nHeight;
                    stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                    m_pMyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                }
                //src = pictureBox1.ImageIpl;
                //pictureBox2.Image = CaptureControl(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height);
                //pictureBox2.ImageIpl = src;
                //pictureBox2.Image = pictureBox1.Image;
                //RotateImage();
                //ConvertImage();

                string path = GetImagePath("jpg");
                pictureBox1.Image = CaptureControl(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                iPrintf($"Image has been saved to {path}");
                UpdateDiskInformation();
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image == null)
                {
                    iPrintf($"There is no image to save. \n Load an image first by opening a video/image file or starting the camera.");
                    return;
                }
                ClearImage();

                int nRet = 0;
                if (m_bGrabbing == false)
                {
                    nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
                    if (nRet == MyCamera.MV_OK)
                    {
                        m_bGrabbing = true;
                    }
                }

                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    iPrintf($"Get PayloadSize failed:{nRet:x8}");
                    return;
                }
                g_nPayloadSize = stParam.nCurValue;

                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                m_BufForDriver = Marshal.AllocHGlobal((Int32)g_nPayloadSize);

                if (m_BufForDriver == IntPtr.Zero)
                {
                    return;
                }

                MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

                nRet = m_pMyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, g_nPayloadSize, ref stFrameInfo, 1000);
                nRet = MyCamera.MV_OK;  //for test
                if (nRet == MyCamera.MV_OK)
                {
                    //m_stFrameInfo = stFrameInfo;
                }
                if (nRet == MyCamera.MV_OK)
                {
                    if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                    {
                        return;
                    }
                    //stDisplayInfo.hWnd = pictureBox2.Handle;
                    stDisplayInfo.hWnd = pictureBox1.Handle;
                    stDisplayInfo.pData = m_BufForDriver;
                    stDisplayInfo.nDataLen = stFrameInfo.nFrameLen;
                    stDisplayInfo.nWidth = stFrameInfo.nWidth;
                    stDisplayInfo.nHeight = stFrameInfo.nHeight;
                    stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                    m_pMyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                }

                //src = pictureBox1.ImageIpl;
                //pictureBox2.Image = CaptureControl(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height);
                //pictureBox2.ImageIpl = src;
                //pictureBox2.Image = pictureBox1.Image;
                //RotateImage();
                //ConvertImage();

                string path = GetImagePath("jpg");
                pictureBox1.Image = CaptureControl(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                iPrintf($"Image has been saved to {path}");
                UpdateDiskInformation();
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
        }

        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // PictureBox.Image now contains the data that was drawn to it

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        public Bitmap CaptureControl(IntPtr handle, int width, int height)
        {
            Bitmap controlBmp;
            using (Graphics g1 = Graphics.FromHwnd(handle))
            {
                controlBmp = new Bitmap(width, height, g1);
                using (Graphics g2 = Graphics.FromImage(controlBmp))
                {
                    // 획득하기 위한 이미지의 위치를 도출하기 위한 컨트롤의 배치를 고려해야 함
                    g2.CopyFromScreen(this.Location.X + pictureBox1.Left,
                                      tableLayoutMain.Location.Y + panel2.Location.Y + this.Location.Y + pictureBox1.Top, 
                                      0, 0, pictureBox1.Size);

                    IntPtr dc1 = g1.GetHdc();
                    IntPtr dc2 = g2.GetHdc();

                    BitBlt(dc2, 0, 0, width, height, handle, 0, 0, 13369376);
                    g1.ReleaseHdc(dc1);
                    g2.ReleaseHdc(dc2);
                }
            }

            return controlBmp;
        }

        public void ConvertImage(int nWidth, int nHeight, ref IntPtr pData)
        {
            
            //pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox16.Image = pictureBox1.Image;
            //pictureBox2.Image = m_pBufForSaveImage;

            return;
            //VideoCapture video;
            //Mat frame = new Mat();
            //VideoCapture video = new VideoCapture(0);
            //video.FrameWidth = 640;
            //video.FrameHeight = 480;
            //video.Read(frame);
            //pictureBox2.ImageIpl = frame.ToIplImage();

            if (srcImage == null)
                return;
            // Flip
            //IplImage newImg = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Flip(srcImage, symm, FlipMode.X);
            // pictureBox2.ImageIpl = newImg;

            // Gray
            //IplImage newImg = new IplImage(srcImage.Size, BitDepth.U8, 1);
            //Cv.CvtColor(srcImage, newImg, ColorConversion.BgrToGray);
            //pictureBox2.ImageIpl = newImg;

            // Reverse
            //IplImage newImg = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Not(srcImage, newImg);
            //pictureBox2.ImageIpl = newImg;

            // Binary
            //IplImage bin = new IplImage(srcImage.Size, BitDepth.U8, 1);
            //Cv.CvtColor(srcImage, bin, ColorConversion.RgbToGray);
            //Cv.Threshold(bin, bin, 100, 255, ThresholdType.Binary);
            //pictureBox2.ImageIpl = bin;

            // Blur
            //IplImage blur = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Smooth(srcImage, blur, SmoothType.Gaussian);
            //pictureBox2.ImageIpl = blur;

            // Canny Edge
            //IplImage canny = new IplImage(srcImage.Size, BitDepth.U8, 1);
            //Cv.Canny(srcImage, canny, 0, 100);
            //pictureBox2.ImageIpl = canny;

            // Sobel Edge
            //IplImage sobel = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Copy(srcImage, sobel);
            //Cv.Sobel(sobel, sobel, 1, 0, ApertureSize.Size3);
            //pictureBox2.ImageIpl = sobel;

            // Laplace Edge
            //IplImage laplace = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Laplace(srcImage, laplace);
            //pictureBox2.ImageIpl = laplace;

            // HSV
            //IplImage hsv = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //IplImage h = new IplImage(srcImage.Size, BitDepth.U8, 1);
            //IplImage s = new IplImage(srcImage.Size, BitDepth.U8, 1);
            //IplImage v = new IplImage(srcImage.Size, BitDepth.U8, 1);

            //Cv.CvtColor(srcImage, hsv, ColorConversion.BgrToHsv);
            //Cv.Split(hsv, h, s, v, null);
            //hsv.SetZero();

            //Hue//        
            //Cv.InRangeS(h, 90, 135, h);
            //Cv.Copy(srcImage, hsv, h);
            //pictureBox2.ImageIpl = hsv;

            //Saturation
            //Cv.InRangeS(s, 90, 135, s);
            //Cv.Copy(srcImage, hsv, s);
            //pictureBox2.ImageIpl = hsv;

            //Value
            //Cv.InRangeS(v, 90, 135, v);
            //Cv.Copy(srcImage, hsv, v);
            //pictureBox2.ImageIpl = hsv;


            //IplImage draw = new IplImage(srcImage.Size, BitDepth.U8, 3);
            //Cv.Copy(srcImage, draw);

            //Cv.DrawLine(draw, 10, 10, 630, 10, CvColor.Blue, 10);
            //Cv.DrawLine(draw, new CvPoint(10, 40), new CvPoint(630, 40), new CvColor(255, 100, 100), 5);

            //Cv.DrawCircle(draw, 60, 150, 50, CvColor.Orange, 2);
            //Cv.DrawCircle(draw, new CvPoint(200, 150), 50, CvColor.Plum, -1);

            //Cv.DrawRect(draw, 300, 100, 400, 200, CvColor.Green, 2);
            //Cv.DrawRect(draw, new CvPoint(450, 100), new CvPoint(550, 200), CvColor.Red, -1);

            //Cv.DrawEllipse(draw, new CvPoint(100, 300), new CvSize(50, 50), 0, 45, 360, CvColor.Beige);

            //Cv.PutText(draw, "Open CV", new CvPoint(200, 300), new CvFont(FontFace.HersheyComplex, 0.7, 0.7), new CvColor(15, 255, 100));
            //Cv.PutText(draw, "Open CV", new CvPoint(350, 300), new CvFont(FontFace.HersheyTriplex, 0.1, 3.0), new CvColor(15, 255, 100));
            //pictureBox2.ImageIpl = draw;

            //Mat frame = new Mat();
            //pictureBox1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            //frame = pictureBox1.Image.Clone();

            //pictureBox2.ImageIpl = frame.ToIplImage();// pictureBox1.Image;

            //pictureBox2.ImageIpl = Contour(srcImage); // 외곽검출
            //pictureBox2.ImageIpl = Corner(srcImage);  // 코너검출
            //pictureBox2.ImageIpl = HoughLines(srcImage);  // 선검출
            //pictureBox2.ImageIpl = HoughCircles(srcImage);    // 원검출
            //pictureBox2.ImageIpl = DilateImage(srcImage); // 팽창
            //pictureBox2.ImageIpl = ErodeImage(srcImage);    // 침식
            //pictureBox2.ImageIpl = Morphology(srcImage);    // 모폴로지 연산
            //pictureBox2.ImageIpl = GammaCorrect(srcImage);    // 감마보정
            //pictureBox2.ImageIpl = BinarizerMethod(srcImage);    // 모폴로지 연산
        }

        //Mat bin;
        //Mat gray;
        //IplImage con;
        //IplImage canny;
        //IplImage corner;
        //public Mat Binary(Mat src)
        //{
        //    bin = new Mat(src.Size, BitDepth.U8, 1);
        //    Cv.CvtColor(src, bin, ColorConversion.RgbToGray);
        //    Cv.Threshold(bin, bin, 150, 255, ThresholdType.Binary);
        //    return bin;
        //}

        //public IplImage BinarizerMethod(IplImage src)
        //{
        //    IplImage bina = new IplImage(src.Size, BitDepth.U8, 1);
        //    gray = this.GrayScale(src);

        //    //Binarizer.Nick(gray, bina, 61, 0.3);
        //    //Binarizer.Niblack(gray, bina, 61, -0.5);
        //    Binarizer.NiblackFast(gray, bina, 61, -0.5);
        //    //Binarizer.Sauvola(gray, bina, 77, 0.2, 64);
        //    //Binarizer.SauvolaFast(gray, bina, 77, 0.2, 64);
        //    //Binarizer.Bernsen(gray, bina, 51, 60, 150);

        //    return bina;
        //}
        //public IplImage GrayScale(IplImage src)
        //{
        //    gray = new IplImage(src.Size, BitDepth.U8, 1);
        //    Cv.CvtColor(src, gray, ColorConversion.BgrToGray);
        //    return gray;
        //}

        //public IplImage CannyEdge(IplImage src)
        //{
        //    canny = new IplImage(src.Size, BitDepth.U8, 1);
        //    Cv.Canny(src, canny, 50, 100);
        //    return canny;
        //}

        //public IplImage HoughCircles(IplImage src)
        //{
        //    IplImage houcircle = new IplImage(src.Size, BitDepth.U8, 3);
        //    IplImage gray = new IplImage(src.Size, BitDepth.U8, 1);

        //    Cv.Copy(src, houcircle);
        //    Cv.CvtColor(src, gray, ColorConversion.BgrToGray);
        //    Cv.Smooth(gray, gray, SmoothType.Gaussian, 9);

        //    CvMemStorage Storage = new CvMemStorage();
        //    CvSeq<CvCircleSegment> circles = Cv.HoughCircles(gray, Storage, HoughCirclesMethod.Gradient, 1, 100, 150, 50, 0, 0);

        //    foreach (CvCircleSegment item in circles)
        //    {
        //        Cv.Circle(houcircle, item.Center, (int)item.Radius, CvColor.Blue, 3);
        //    }
        //    return houcircle;
        //}

        //public IplImage HoughLines(IplImage src)
        //{
        //    IplImage houline = new IplImage(src.Size, BitDepth.U8, 3);
        //    canny = new IplImage(src.Size, BitDepth.U8, 1);

        //    canny = this.CannyEdge(this.Binary(src));
        //    Cv.CvtColor(canny, houline, ColorConversion.GrayToBgr);

        //    CvMemStorage Storage = new CvMemStorage();

        //    //Standard 방법
        //    CvSeq lines = canny.HoughLines2(Storage, HoughLinesMethod.Standard, 1, Math.PI / 180, 50, 0, 0);

        //    for (int i = 0; i < Math.Min(lines.Total, 3); i++)
        //    {
        //        CvLineSegmentPolar element = lines.GetSeqElem<CvLineSegmentPolar>(i).Value;

        //        float r = element.Rho;
        //        float theta = element.Theta;

        //        double a = Math.Cos(theta);
        //        double b = Math.Sin(theta);
        //        double x0 = r * a;
        //        double y0 = r * b;
        //        int scale = src.Size.Width + src.Size.Height;

        //        CvPoint pt1 = new CvPoint(Convert.ToInt32(x0 - scale * b), Convert.ToInt32(y0 + scale * a));
        //        CvPoint pt2 = new CvPoint(Convert.ToInt32(x0 + scale * b), Convert.ToInt32(y0 - scale * a));

        //        houline.Circle(new CvPoint((int)x0, (int)y0), 5, CvColor.Yellow, -1);
        //        houline.Line(pt1, pt2, CvColor.Red, 1, LineType.AntiAlias);
        //    }

        //    //Probabilistic 방법
        //    //CvSeq lines = canny.HoughLines2(Storage, HoughLinesMethod.Probabilistic, 1, Math.PI / 180, 140, 50, 10);

        //    //for (int i = 0; i < lines.Total; i++)
        //    //{
        //    //    CvLineSegmentPoint element = lines.GetSeqElem<CvLineSegmentPoint>(i).Value;
        //    //    houline.Line(element.P1, element.P2, CvColor.Yellow, 1, LineType.AntiAlias);
        //    //}

        //    return houline;
        //}
        //public IplImage Corner(IplImage src)
        //{
        //    corner = new IplImage(src.Size, BitDepth.U8, 3);
        //    gray = new IplImage(src.Size, BitDepth.U8, 1);
        //    IplImage eigImg = new IplImage(src.Size, BitDepth.U8, 1);
        //    IplImage tempImg = new IplImage(src.Size, BitDepth.U8, 1);

        //    Cv.Copy(src, corner);
        //    gray = this.GrayScale(src);

        //    CvPoint2D32f[] corners;
        //    int cornerCount = 150;

        //    Cv.GoodFeaturesToTrack(gray, eigImg, tempImg, out corners, ref cornerCount, 0.01, 5);

        //    Cv.FindCornerSubPix(gray, corners, cornerCount, new CvSize(3, 3), new CvSize(-1, -1), new CvTermCriteria(20, 0.03));

        //    for (int i = 0; i < cornerCount; i++)
        //    {
        //        Cv.Circle(corner, corners[i], 3, CvColor.Black, 2);
        //    }

        //    return corner;
        //}

        //public IplImage Contour(IplImage src)
        //{
        //    con = new IplImage(src.Size, BitDepth.U8, 3);
        //    bin = new IplImage(src.Size, BitDepth.U8, 1);

        //    Cv.Copy(src, con);
        //    bin = this.Binary(src);

        //    CvMemStorage Storage = new CvMemStorage();
        //    CvSeq<CvPoint> contours;

        //    CvContourScanner scanner = Cv.StartFindContours(bin, Storage, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxNone);

        //    // #1        
        //    while (true)
        //    {
        //        contours = Cv.FindNextContour(scanner);

        //        if (contours == null) break;
        //        else
        //        {
        //            Cv.DrawContours(con, contours, CvColor.Yellow, CvColor.Red, 1, 4, LineType.AntiAlias);
        //        }
        //    }
        //    Cv.EndFindContours(scanner);

        //    // #2        
        //    //foreach (CvSeq<CvPoint> c in scanner)
        //    //{
        //    //    con.DrawContours(c, CvColor.Yellow, CvColor.Red, 1, 4, LineType.AntiAlias);
        //    //}
        //    //Cv.ClearSeq(contours);

        //    Cv.ReleaseMemStorage(Storage);

        //    return con;
        //}

        //public IplImage DilateImage(IplImage src)
        //{
        //    IplImage dil = new IplImage(src.Size, BitDepth.U8, 3);

        //    IplConvKernel element = new IplConvKernel(4, 4, 2, 2, ElementShape.Custom, new int[3, 3]);
        //    Cv.Dilate(src, dil, element, 3);
        //    return dil;
        //}

        //public IplImage ErodeImage(IplImage src)
        //{
        //    IplImage ero = new IplImage(src.Size, BitDepth.U8, 3);

        //    IplConvKernel element = new IplConvKernel(4, 4, 2, 2, ElementShape.Custom, new int[3, 3]);
        //    Cv.Erode(src, ero, element, 3);
        //    return ero;
        //}

        //public IplImage Morphology(IplImage src)
        //{
        //    IplImage morp = new IplImage(src.Size, BitDepth.U8, 3);

        //    IplConvKernel element = new IplConvKernel(3, 3, 1, 1, ElementShape.Ellipse);
        //    Cv.MorphologyEx(src, morp, src, element, MorphologyOperation.Open, 3);

        //    return morp;
        //}

        //public IplImage GammaCorrect(IplImage src)
        //{
        //    IplImage gamma = new IplImage(src.Size, BitDepth.U8, 3);

        //    double gamma_value = 0.5;

        //    byte[] lut = new byte[256];
        //    for (int i = 0; i < lut.Length; i++)
        //    {
        //        lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / gamma_value) * 255.0);
        //    }

        //    Cv.LUT(src, gamma, lut);

        //    return gamma;
        //}

        //public unsafe int RGB2BGR(IntPtr pRgbData, int nWidth, int nHeight)
        //{
        //    if (null == pRgbData)
        //    {
        //        return MyCamera.MV_E_PARAMETER;
        //    }

        //    for (int j = 0; j < nHeight; j++)
        //    {
        //        for (int i = 0; i < nWidth; i++)
        //        {
        //            byte red = pRgbData[j * (nWidth * 3) + i * 3];
        //            pRgbData[j * (nWidth * 3) + i * 3] = pRgbData[j * (nWidth * 3) + i * 3 + 2];
        //            pRgbData[j * (nWidth * 3) + i * 3 + 2] = red;
        //        }
        //    }

        //    return MyCamera.MV_OK;
        //}

        //// convert data stream in Ipl format
        //public unsafe bool Convert2Ipl(ref MyCamera.MV_FRAME_OUT_INFO_EX pstImageInfo, IntPtr pData)
        //{
        //    //IplImage srcImage = new IplImage();
        //    IplImage srcImage = Cv.CreateImage(Cv.Size(pstImageInfo.nWidth, pstImageInfo.nHeight), BitDepth.U8, 3);

        //    if (pstImageInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
        //    {
        //        RGB2BGR(pData, pstImageInfo.nWidth, pstImageInfo.nHeight);
        //        srcImage = Cv.CreateImage(Cv.Size(pstImageInfo.nWidth, pstImageInfo.nHeight), BitDepth.U8, 3);
        //    }
        //    else
        //    {
        //        iPrintf("unsupported pixel format\n");
        //        return false;
        //    }
        //    if (null == srcImage)
        //    {
        //        iPrintf("CreatImage failed.\n");
        //        return false;
        //    }
        //    IplImage.FromPixelData(pstImageInfo.nWidth, pstImageInfo.nHeight, 3, pData);
            
        //    //srcImage.ImageDataPtr = pData;

        //    // save converted image in a local file

        //    Cv.ReleaseImage(srcImage);
        //    return true;
        //}
    }
}
