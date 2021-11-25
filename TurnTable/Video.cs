using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
//using OpenCvSharp.UserInterface;
using OpenCvSharp.Blob;
//using OpenCvSharp.CPlusPlus;
using MaterialSkin.Controls;
using System.Windows.Forms;
using System.Diagnostics;

namespace CytoDx
{
    public partial class MainWindow
    {
        //this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);

        
        private void InitMediaButton()
        {
            VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PLAY;
            btnClipStop.Text = MEDIA_STOP;
            btnClipOpen.Text = MEDIA_PLUS; // MEDIA_OPEN_FOLDER;
            btnClipForward.Text = MEDIA_FORWARD;
            btnClipBackward.Text = MEDIA_BACKWARD;
            btnClipRecord.Text = MEDIA_RECORD;
        }

        private void EnableClipButtons(bool enable)
        {
            btnClearImage.Enabled = enable;
            TrackBar_Video.Enabled = enable;
            btnVideoFolder.Enabled = enable;
            btnImageFolder.Enabled = enable;
            btnLoadImage.Enabled = enable;
            btnClipPlay.Enabled = enable;
            btnClipStop.Enabled = enable;
            btnClipOpen.Enabled = enable;
            btnClipForward.Enabled = enable;
            btnClipBackward.Enabled = enable;
            btnClipRecord.Enabled = enable;
        }

        private void CloseClip()
        {
            if (cvCapture != null)
                cvCapture.Dispose();
            //timer_music.Enabled = false;
            //MP3TrackBar.Value = 0;
            //label_music_current_time.Text = "00:00";
            //label_music_total_time.Text = "00:00";
        }
        // 영상 재생 : ▶ Button
        private void btnClipPlay_Click(object sender, EventArgs e)
        {
            if (cvCapture == null)
            {
                if (label_TimeStamp.Text == "")
                    return;
                string fileName = $"{DIR_VIDEO}\\{label_TimeStamp.Text}.avi";
                if (File.Exists(fileName))
                {
                    LoadClip(fileName);
                    return;
                }
                else
                    return;
            }

            if (btnClipPlay.Text == MEDIA_PLAY) // PLAY
            {
                if (cvCapture.PosFrames >= cvCapture.FrameCount)
                    cvCapture.PosFrames = TrackBar_Video.Value = 0;
                VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PAUSE;
                btnClipRecord.Enabled = false;
                timer_video.Enabled = true;
            }
            else    // PAUSE
            {
                VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PLAY;
                timer_video.Enabled = false;
            }
        }

        private void LoadFromCamera()
        {
            if (cvCapture != null)
                cvCapture.Dispose();
            if (cvVideoWriter != null)
                cvVideoWriter.Dispose();
            if (videoCapture != null)
                videoCapture.Dispose();
            if (matFrame != null)
                matFrame.Dispose();

            videoCapture = new VideoCapture(0);
            matFrame = new Mat();

            //cvCapture = CvCapture.FromCamera(CaptureDevice.DShow, 0);
            //cvCapture.SetCaptureProperty(CaptureProperty.FrameWidth, 1440);
            //cvCapture.SetCaptureProperty(CaptureProperty.FrameHeight, 1080);
            timer_camera.Interval = 50;
            timer_camera.Enabled = true;
            //cvCapture = new CvCapture(CaptureDevice.DShow, 0);
            //cvCapture.SetCaptureProperty(OpenCvSharp.CaptureProperty.FrameWidth, 1440);
            //cvCapture.SetCaptureProperty(OpenCvSharp.CaptureProperty.FrameHeight, 1080);

            string save_name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            //cvVideoWriter = new CVideoWriter($"{DIR_VIDEO}\\{save_name}.avi", "XVID", 50, Cv.Size(640, 480));
            //cvVideoWriter = new CvVideoWriter($"{DIR_VIDEO}\\{save_name}.avi", "XVID", 15, Cv.GetSize(src));
            cvVideoWriter = new VideoWriter($"{DIR_VIDEO}\\{save_name}.avi", -1, 50, new Size(640, 480));

        }

        private void Camera_TickTimer(object sender, EventArgs e)
        {
            try
            {
                if (timer_camera.Enabled)
                {
                    lock (lockCamera)
                    {
                        if (videoCapture.Read(matFrame))
                        {
                            //cvVideoWriter.WriteFrame((IplImage)matFrame);
                            pictureBox1.Image = matFrame.ToBitmap();
                            if (cvVideoWriter != null)
                                cvVideoWriter.Write(matFrame);
                            //src = cvCapture.QueryFrame();
                            //cvVideoWriter.WriteFrame(src);
                        }
                    }
                }
                //pictureBox1.ImageIpl = src;
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
        }

        private void LoadClip(string fileName)
        {
            if (fileName == "")
                return;

            if (cvCapture != null)
                cvCapture.Dispose();

            cvCapture = VideoCapture.FromFile(fileName);
           
            if (cvCapture.IsDisposed == false)
            {
                if (cvCapture.Fps <= 0)
                    cvCapture.Fps = 33;
                timer_video.Interval = (int)(1000 / cvCapture.Fps);
                TrackBar_Video.Minimum = 0;
                TrackBar_Video.Maximum = cvCapture.FrameCount;
                TrackBar_Video.Value = 0;
                cvCapture.PosFrames = cvCapture.FrameCount;
                int totalTime = cvCapture.PosMsec / 1000;
                cvCapture.PosFrames = 0;

                label_video_tot_time.Text = $"{totalTime / 60:d2}:{totalTime % 60:d2}";
                VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PAUSE;
                timer_video.Enabled = true;
            }
        }

        // 동영상 정지 : ■ Button
        private void btnClipStop_Click(object sender, EventArgs e)
        {
            StopClip();
        }


        private void btnClipRecord_Click(object sender, EventArgs e)
        {
            if (isRecording)
            {
                EnableClipButtons(true);
                if(config.bDebugMode)
                    timer_camera.Enabled = false;
                btnClipRecord.Primary = true;
                Thread.Sleep((int)timer_camera.Interval);
                lock (lockCamera)
                {
                    if (cvVideoWriter != null)
                        cvVideoWriter.Dispose();
                }

                src.Release();
                if (src != null)
                    src.Dispose();
            }
            else
            {
                if (config.bDebugMode)
                    LoadFromCamera();
                else
                {
                    if(StartRecord(float.Parse(tbRecordFps.Text)))
                    {
                        EnableClipButtons(false);
                        btnClipRecord.Primary = false;
                        btnClipRecord.Enabled = true;
                    }
                }
            }
        }

        private void StopClip()
        {
            timer_video.Enabled = false;
            VideoPlayButtonStatus = btnClipPlay.Text = MEDIA_PLAY;
            btnClipRecord.Enabled = true;
            TrackBar_Video.Value = 0;
            label_video_cur_time.Text = "00:00";
            if (cvCapture != null)
                cvCapture.PosFrames = 0;

            //this.btnPlayMusic.Icon = global::TurnTable.Properties.Resources.play;
            //isPlaying = false;
            //audioPlayer.Stop();
        }

        // TrackBar 컨트롤 MouseDown 이벤트
        private void clipTrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cvCapture == null)
                {
                    TrackBar_Video.Value = 0;
                    return;
                }

                isScrolled_Video = true;
                trackBarMouseX_Video = e.X - trackBarBlankSize_Video;     // 마우스 클릭 좌표
                SetPositionByMouseClip(trackBarMouseX_Video);
            }
            catch (Exception ex)
            {
                TrackBar_Video.Value = 0;
                //iPrintf(ex.Message);
            }

        }

        // TrackBar 컨트롤  MouseMove 이벤트
        private void clipTrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (cvCapture == null)
            {
                TrackBar_Video.Value = 0;
                return;
            }
            bool tempTimerStatus = timer_video.Enabled;
            timer_video.Enabled = false;
            try
            {
                if (isScrolled_Video)
                {
                    trackBarMouseX_Video = e.X - trackBarBlankSize_Video; // 마우스 클릭 좌표
                    SetPositionByMouseClip(trackBarMouseX_Video);
                    if (TrackBar_Video.Value >= 0 && TrackBar_Video.Value <= cvCapture.FrameCount)
                    {
                        cvCapture.PosFrames = TrackBar_Video.Value;
                        UpdateVideoPlayingTime();

                        if (timer_video.Enabled == false)
                        {
                            cvCapture.Read(src);
                            if (src.Empty())
                            {
                                timer_video.Enabled = false;
                            }
                            else
                                pictureBox1.Image = src.ToBitmap();
                            pictureBox1.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TrackBar_Video.Value = 0;
            }
            timer_video.Enabled = tempTimerStatus;
        }

        // TrackBar 컨트롤 MouseUp 이벤트
        private void clipTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (cvCapture == null)
            {
                TrackBar_Video.Value = 0;
                return;
            }
            bool tempTimerStatus = timer_video.Enabled;
            timer_video.Enabled = false;
            try
            {
                isScrolled_Video = false;

                //cvCapture.SetCaptureProperty(OpenCvSharp.CaptureProperty.PosFrames, clipTrackBar.Value);
                if (TrackBar_Video.Value >= 0 && TrackBar_Video.Value <= cvCapture.FrameCount)
                {
                    cvCapture.PosFrames = TrackBar_Video.Value;
                    UpdateVideoPlayingTime();

                    if (timer_video.Enabled == false)
                    {
                        cvCapture.Read(src);
                        if (src.Empty())
                        {
                            timer_video.Enabled = false;
                        }
                        else
                            pictureBox1.Image = src.ToBitmap();
                    }
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
            timer_video.Enabled = tempTimerStatus;
        }

        // TrackBar ▼ 이렇게 생긴애(클릭해서 끌어당길 수 있는 애) 트랙바 클릭시 마우스 따라가게 하는 메소드
        private void SetPositionByMouseClip(int position)
        {
            if (position < 0 || position > trackBarLength_Video)
                return;

            float rate = (float)position / (float)trackBarLength_Video;
            TrackBar_Video.Value = (int)(rate * (float)(TrackBar_Video.Maximum - TrackBar_Video.Minimum));
        }

        private void UpdateVideoPlayingTime()
        {
            if (cvCapture != null)
            {
                int sec = cvCapture.PosMsec / 1000;
                label_video_cur_time.Text = $"{sec / 60:d2}:{sec % 60:d2}";
            }
        }

        private void SaveImage(object sender, EventArgs e)
        {
            if (isRunning)
            {
                try
                {
                    // Returns full path of Image file 
                    DateTime dtNow = DateTime.Now;
                    string dtStr = dtNow.ToString("yyyy-MM-dd_HHmm_ss_f");
                    //iPrintf(dtStr);
                    string imgPath = DIR_IMAGE + "\\" + dtStr + ".jpg";
                    iPrintf(dtStr + ".jpg");
                    //pictureBox1.ImageIpl.SaveImage(imgPath);
                    pictureBox1.Image.Save(imgPath);
                }
                catch (Exception)
                {

                    ;
                }

            }
        }

        private void btnClearImage_Click(object sender, EventArgs e)
        {
            ClearImage();
        }

        public void ClearImage()
        {
            //pictureBox1.Image = null;
            this.pictureBox1.Image = global::CytoDx.Properties.Resources.Logo;
            pictureBox3.Image = null;
            pictureBox16.Image = null;
            m_ImageFileName = "";
            label_add_volume.Hide();

            //this.pictureBox1.Location = new System.Drawing.Point(23, -1);
            //this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            //this.pictureBox1.Size = new System.Drawing.Size(1151, 860);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        }

        private void DeleteOldFiles()
        {
            DeleteOldVideoFile();
            DeleteOldImageFile();
        }

        private void DeleteOldVideoFile()
        {
            DirectoryInfo info = new DirectoryInfo(DIR_VIDEO);
            FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
            int fileCount = files.Count();
            if (fileCount <= config.MaxVideoFileNumber)
                return;

            int i = 0;
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".avi"))
                {
                    i++;
                    if (i > config.MaxVideoFileNumber)
                        file.Delete();
                }
            }
        }


        private void DeleteOldImageFile()
        {
            DirectoryInfo info = new DirectoryInfo(DIR_IMAGE);
            FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
            int fileCount = files.Count();
            if (fileCount <= config.MaxImageFileNumber)
                return;

            int i = 0;
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".jpg"))
                {
                    i++;
                    if (i > config.MaxImageFileNumber)
                        file.Delete();
                }
            }
        }
        private string GetLastVideoFileName()
        {
            string path = "";
            DirectoryInfo info = new DirectoryInfo(DIR_VIDEO);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (FileInfo file in files)
            {
                if (file.Extension.ToUpper() == ".AVI")
                {
                    path = $"{DIR_VIDEO}\\{file.Name}";
                }
            }
            return path;
        }

        private void btnClipBackward_Click(object sender, EventArgs e)
        {
            try
            {
                if (cvCapture == null)
                    return;

                isScrolled_Video = true;

                if (this.TrackBar_Video.Value - (int)(TrackBar_Video.Maximum * 0.1) < 0)
                    TrackBar_Video.Value = 0;
                else
                    TrackBar_Video.Value -= (int)(TrackBar_Video.Maximum * 0.1);

                cvCapture.PosFrames = TrackBar_Video.Value;
                if (cvCapture != null && timer_video.Enabled == false)
                {
                    UpdateVideoPlayingTime();
                    cvCapture.Read(src);
                    if (src.Empty())
                    {
                        timer_video.Enabled = false;
                    }
                    else
                        pictureBox1.Image = src.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
            isScrolled_Video = false;
        }

        private void btnClipForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (cvCapture == null)
                    return;

                isScrolled_Video = true;

                if (this.TrackBar_Video.Value + (int)(TrackBar_Video.Maximum * 0.1) > TrackBar_Video.Maximum)
                    TrackBar_Video.Value = TrackBar_Video.Maximum;
                else
                    TrackBar_Video.Value += (int)(TrackBar_Video.Maximum * 0.1);

                cvCapture.PosFrames = TrackBar_Video.Value;
                if (cvCapture != null && timer_video.Enabled == false)
                {
                    UpdateVideoPlayingTime();
                    cvCapture.Read(src);
                    if (src.Empty())
                    {
                        timer_video.Enabled = false;
                    }
                    else
                        pictureBox1.Image = src.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message);
            }
            isScrolled_Video = false;
        }

        private void btnImageFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DIR_IMAGE))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = DIR_IMAGE;
                psi.FileName = "explorer.exe";
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", DIR_IMAGE), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVideoFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DIR_VIDEO))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = DIR_VIDEO;
                psi.FileName = "explorer.exe";
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", DIR_VIDEO), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnClearMessage_Click(object sender, EventArgs e)
        {
            SerialMessageBox.Clear();
        }
        
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (btnClipPlay.Text == MEDIA_PAUSE)
                return;
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            dialog.InitialDirectory = DIR_IMAGE;
            dialog.Title = "Select file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                srcImage = new Mat(dialog.FileName, ImreadModes.AnyColor);
                //srcImage = new Mat(dialog.FileName, LoadMode.AnyColor);
                //using (IplImage ipl = new IplImage(dialog.FileName, LoadMode.AnyColor))
                {
                    //this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = srcImage.ToBitmap();// ipl;
                    m_ImageFileName = dialog.FileName;
                    //srcImage = ipl;
                }
            }
        }

        private void btnOpenClip_Click(object sender, EventArgs e)
        {
            if (timer_video.Enabled)
            {
                timer_video.Enabled = false;
                return;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "AVI files (*.avi)|*.avi|MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            dialog.InitialDirectory = DIR_VIDEO;
            dialog.Title = "Select file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                label_TimeStamp.Text = ExtractFileName(dialog.FileName);
                LoadClip(dialog.FileName);
            }
        }
    }
}
