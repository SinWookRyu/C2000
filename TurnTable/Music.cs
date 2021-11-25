using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace CytoDx
{
    public partial class MainWindow
    {
        string m_MusicFolderPath = "";
        int m_musicIndex = -1;
        bool isPlayingMusic = false;

        bool isScrolled_Music = false;        // TrackBar 움직였는지
        int trackBarBlankSize_Music = 14;     // TrackBar 양옆 빈공간
        int trackBarLength_Music = 0;         // TrackBar의 실제 길이
        int trackBarMouseX_Music = 0;         // TrackBar에서 마우스 클릭 위치

        // Timer 컨트롤 Tick 이벤트
        TimeSpan timeSpan;

        private void Music_TickTimer(object sender, EventArgs e)
        {
            if (isScrolled_Music == false && isRunning == false)
            {
                try
                {
                    //if (materialTabControl1.SelectedIndex != (int)TAB.MUSIC)
                    if(TabItem != "Music")
                    {
                        StopMusic();
                        timer_music.Stop();
                    }
                    timeSpan = audioPlayer.Position;
                    if ((int)timeSpan.TotalMilliseconds <= TrackBar_Music.Maximum)
                        TrackBar_Music.Value = (int)timeSpan.TotalMilliseconds;
                    else
                    {
                        TrackBar_Music.Value = TrackBar_Music.Maximum;
                        StopMusic();
                    }
                    label_music_current_time.Text = timeSpan.ToString("mm':'ss");
                }
                catch (Exception ex)
                {
                    iPrintf(ex.ToString());
                }

            }
        }

        private void PlayNextSong(object sendor, EventArgs e)
        {
            //if (isRunning || materialTabControl1.SelectedIndex == (int)TAB.MUSIC)
            if (isRunning || TabItem == "Music")
            {
                if (this.LV_MP3_play.Items.Count <= ++m_musicIndex)
                    m_musicIndex = 0;
                StopMusic();
                LoadMusicByIndex(m_musicIndex);
            }
        }

        // 파일 열기
        private void btnOpenMusic_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                folder.SelectedPath = Environment.CurrentDirectory + "\\Music";
                //folder.RootFolder = ()Environment.CurrentDirectory;
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    m_MusicFolderPath = folder.SelectedPath;
                    UpdateMusicPlayList(m_MusicFolderPath);
                }
            }
        }

        private void OpenMusicFileDialog()
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "Mp3 File|*.mp3";

                open.InitialDirectory = Environment.CurrentDirectory;

                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenMusic(open.FileName);
                }
            }
        }

        private void OpenMusic(String fileName)
        {
            TrackBar_Music.Maximum = (int)audioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            label_music_current_time.Text = audioPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

            timer_music.Interval = 1000;
            timer_music.Tick += Music_TickTimer;
            timer_music.Start();
        }

        public void UpdateMusicPlayList(string folderPath)
        {
            String[] arrStr = new String[3];
            LV_MP3_play.Items.Clear();

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(folderPath, "*.MP3");
            //Directory.GetFiles()
            foreach (string fullName in fileEntries)
            {
                arrStr[0] = ExtractFileName(fullName);
                arrStr[1] = ""; // songInfo.Artist;
                arrStr[2] = ""; // songInfo.Album;
                ListViewItem item = new ListViewItem(arrStr);
                LV_MP3_play.Items.Add(item);
            }
        }
        // 파일 닫기
        private void btnCloseMusic_Click(object sender, EventArgs e)
        {
            CloseMusic();
        }

        private void CloseMusic()
        {
            timer_music.Enabled = false;
            TrackBar_Music.Value = 0;
            label_music_current_time.Text = "00:00";
            label_music_total_time.Text = "00:00";
        }
        // 음악 재생 : ▶ Button
        private void btnPlayMusic_Click(object sender, EventArgs e)
        {
            UpdateMusicPlayList(m_MusicFolderPath);
            PlayMusic();
        }

        private void PlayMusic()
        {
            if (isRunning && config.PlayMusicOntest == false)
                return;
            if (this.label_song_title.Text == "")
                LoadMusicByIndex(0);

            if (isPlayingMusic)
            {
                this.btnPlayMusic.Icon = global::CytoDx.Properties.Resources.play;
                isPlayingMusic = false;
                audioPlayer.Pause();
            }
            else
            {
                this.btnPlayMusic.Icon = global::CytoDx.Properties.Resources.pause;
                isPlayingMusic = true;
                audioPlayer.Play();
            }
        }

        private void btnStopMusic_Click(object sender, EventArgs e)
        {
            StopMusic();
        }

        private void StopMusic()
        {
            this.btnPlayMusic.Icon = global::CytoDx.Properties.Resources.play;
            isPlayingMusic = false;
            audioPlayer.Stop();
            
        }

        // TrackBar 컨트롤 MouseDown 이벤트
        private void MP3TrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            isScrolled_Music = true;
            trackBarMouseX_Music = e.X - trackBarBlankSize_Music;     // 마우스 클릭 좌표
            SetPositionByMouse(trackBarMouseX_Music);
        }

        // TrackBar 컨트롤  MouseMove 이벤트
        private void MP3TrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isScrolled_Music)
            {
                trackBarMouseX_Music = e.X - trackBarBlankSize_Music; // 마우스 클릭 좌표
                SetPositionByMouse(trackBarMouseX_Music);
            }
        }

        // TrackBar 컨트롤 MouseUp 이벤트
        private void MP3TrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (audioPlayer.CanPause)
            {
                audioPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)TrackBar_Music.Value);
            }

            isScrolled_Music = false;
        }

        // TrackBar ▼ 이렇게 생긴애(클릭해서 끌어당길 수 있는 애) 트랙바 클릭시 마우스 따라가게 하는 메소드
        private void SetPositionByMouse(int position)
        {
            if (position < 0 || position > trackBarLength_Music)
                return;

            float rate = (float)position / (float)trackBarLength_Music;
            TrackBar_Music.Value = (int)(rate * (float)(TrackBar_Music.Maximum - TrackBar_Music.Minimum));
        }

        //음악 타이머 메소드
        private void UpdateMusicCurrentTime()
        {
            if (audioPlayer.CanPause)
            {
                TimeSpan t = audioPlayer.Position;
                label_music_current_time.Text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            }
        }

        private void listView_mp3_play_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            //string str = item.Text;
        }

        private void listView_mp3_play_DoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                StopMusic();
                CloseMusic();
                m_musicIndex = this.LV_MP3_play.FocusedItem.Index;
                //ListView.SelectedListViewItemCollection items = this.listView_mp3_play.SelectedItems;
                //m_musicIndex = items[0].Index;
                LoadMusicByIndex(m_musicIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void loadedMusic(object sender, EventArgs e)
        {
            TrackBar_Music.Maximum = (int)audioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            label_music_total_time.Text = audioPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            timer_music.Interval = 100;
            timer_music.Tick += Music_TickTimer;
            audioPlayer.MediaEnded += PlayNextSong;

            timer_music.Start();
            PlayMusic();
        }

        private void btnVolume_Click(object sender, EventArgs e)
        {
            if (audioPlayer.Volume == 0)
            {
                audioPlayer.Volume = 100;
                this.btnVolume.Icon = global::CytoDx.Properties.Resources.speakout;
            }
            else
            {
                audioPlayer.Volume = 0;
                this.btnVolume.Icon = global::CytoDx.Properties.Resources.volume_low;
            }
        }

        private void LoadMusicByIndex(int index)
        {
            try
            {
                if (isRunning && config.PlayMusicOntest == false)
                    return;

                if (LV_MP3_play.Items.Count <= 0)
                    return;

                if (LV_MP3_play.Items.Count <= index)
                    index = 0;

                string fileName = m_MusicFolderPath + "\\" + LV_MP3_play.Items[index].SubItems[0].Text + ".mp3";
                LV_MP3_play.Items[index].Focused = true;
                LV_MP3_play.Items[index].EnsureVisible();
                LV_MP3_play.Items[index].Selected = true;
                LV_MP3_play.Focus();
                if (label_song_title.Text != LV_MP3_play.Items[index].SubItems[0].Text)
                {
                    audioPlayer.Open(new Uri(fileName));
                    label_song_title.Text = LV_MP3_play.Items[index].SubItems[0].Text;
                    audioPlayer.MediaOpened += loadedMusic;
                }
                else
                {
                    loadedMusic(this, null);

                }
            }
            catch (Exception ex)
            {
                iPrintf(ex.ToString());
            }
        }

        private void btnMusicForward_Click(object sender, EventArgs e)
        {
            if (isRunning && config.PlayMusicOntest == false)
                return;
            StopMusic();
            if (m_musicIndex == -1)
            {
                Random random = new Random(LV_MP3_play.Items.Count);
                m_musicIndex = random.Next() % LV_MP3_play.Items.Count;
            }
            if (this.LV_MP3_play.Items.Count <= ++m_musicIndex)
                m_musicIndex = 0;
            LoadMusicByIndex(m_musicIndex);
        }

        private void btnMusicBackward_Click(object sender, EventArgs e)
        {
            if (isRunning && config.PlayMusicOntest == false)
                return;
            StopMusic();
            if (--m_musicIndex < 0)
                m_musicIndex = this.LV_MP3_play.Items.Count - 1;
            LoadMusicByIndex(m_musicIndex);
        }
    }
}
