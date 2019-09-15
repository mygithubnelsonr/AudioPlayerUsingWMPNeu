using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsMediaPlayerTest
{
    public partial class Form1 : Form
    {
        double _duration = 0;
        double _position = 0;

        public Form1()
        {
            InitializeComponent();
            this.Text = "WindowsMediaPlayer2";
            axWindowsMediaPlayer1.settings.volume = 10;
        }

        private void listBoxFiles_DragEnter(object sender, DragEventArgs e)
        {
            Debug.Print("listBoxFiles_DragEnter");

            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            Debug.Print("listBoxFiles_DragDrop");
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                listBoxFiles.Items.Clear();

                foreach(string s in filenames)
                {
                    listBoxFiles.Items.Add(s);
                }

                listBoxFiles.SelectedIndex = 0;
            }
        }

        private void listBoxFiles_Click(object sender, EventArgs e)
        {
            Debug.Print("listBoxFiles_Click");
            buttonPlay.PerformClick();
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.Print("listBoxFiles_SelectedIndexChanged");
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBarVolume.Value;
            Debug.Print(axWindowsMediaPlayer1.settings.volume.ToString());
        }

        private void buttonPlay_Click(object sender, System.EventArgs e)
        {
            axWindowsMediaPlayer1.URL = listBoxFiles.Text;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            timerDuration.Enabled = true;
        }

        private void buttonPause_Click(object sender, System.EventArgs e)
        {
            if(buttonPause.Text == "Pause")
            {
                buttonPause.Text = "Resume";
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                timerDuration.Enabled = false;
            }
            else
            {
                buttonPause.Text = "Pause";
                axWindowsMediaPlayer1.Ctlcontrols.play();
                timerDuration.Enabled = true;
            }
        }

        private void buttonStop_Click(object sender, System.EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            timerDuration.Enabled = false;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Debug.Print("buttonTest_Click");
            listBoxFiles.SelectedIndex = listBoxFiles.SelectedIndex + 1;
        }

        private void trackBarPosition_Scroll(object sender, System.EventArgs e)
        {
            int intval = trackBarPosition.Value;
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = intval;
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            Debug.Print($"PlayStateChange: newState={e.newState}");

            string fileName = "";

            if(e.newState == (int)WMPPlayState.wmppsStopped)
            {
                if(listBoxFiles.SelectedIndex < listBoxFiles.Items.Count - 1)
                {
                    listBoxFiles.SelectedIndex++;
                    fileName = listBoxFiles.Text;
                }
                else
                    fileName = "";

                BeginInvoke(
                    new Action(() => { axWindowsMediaPlayer1.URL = fileName; })
                );
            }

            if(e.newState == (int)WMPPlayState.wmppsPlaying)
            {
                _duration = axWindowsMediaPlayer1.currentMedia.duration;
                trackBarPosition.Maximum = (int)_duration;
                toolStripStatusLabelDuration.Text = axWindowsMediaPlayer1.currentMedia.durationString;
                Debug.Print(_duration.ToString());
            }
        }

        private void axWindowsMediaPlayer1_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            Debug.Print("OpenStateChange");
        }

        private void axWindowsMediaPlayer1_EndOfStream(object sender, AxWMPLib._WMPOCXEvents_EndOfStreamEvent e)
        {
            Debug.Print("EndOfStream");
        }

        private void axWindowsMediaPlayer1_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            Debug.Print("PositionChange: oldposition=" + e.oldPosition);
            Debug.Print("PositionChange: newposition=" + e.newPosition);
            toolStripStatusLabelPosition.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

        private void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
        {
            Debug.Print("StatusChange: " + e.ToString());
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            Debug.Print($"WindowsMediaPlayer Enter: {e.ToString()}");
        }

        private void timerDuration_Tick(object sender, EventArgs e)
        {
            _position = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            trackBarPosition.Value = (int)_position;
            toolStripStatusLabelPosition.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

    }
}
