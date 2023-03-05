using System.Globalization;
using System.Reflection.Metadata;
using LibVLCSharp.Shared;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class VideoForm : Form
    {
        private LibVLC? LibVlc { get; set; }
        public MediaPlayer? MPlayer { get; set; }
        public ViewedProjectTab? ParentProject { get; set; }


        public VideoForm()
        {
            InitializeComponent();
        }

        public bool OpenVideoFile(string filePath)
        {
            try
            {
                if (LibVlc == null)
                    LibVlc = new LibVLC(enableDebugLogs: true);
                if (MPlayer == null)
                {
                    MPlayer = new MediaPlayer(LibVlc);
                    MPlayer.PositionChanged += OnMediaPlayerPositionChanged;
                    MPlayer.EnableHardwareDecoding = true;
                    MPlayer.EnableKeyInput = false;
                    MPlayer.EnableMouseInput = false;
                    VideoViewPort.MediaPlayer = MPlayer;
                }

                var media = new Media(LibVlc, filePath);
                //var media = new Media(LibVlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
                MPlayer.Play(media);
                //MPlayer.Pause();
                //MPlayer.Position = 0.0f;
                //MPlayer.NextFrame();
                media.Dispose();
                PMFollowPackets.Enabled = (ParentProject != null);
                PMSync.Enabled = PMFollowPackets.Enabled;

                return true;
            }
            catch
            {
                // Ignore
            }
            PMFollowPackets.Enabled = false;
            PMSync.Enabled = false;

            return false;
        }

        public bool OpenVideoFromProject()
        {
            if (ParentProject == null)
                return false;
            if (!File.Exists(ParentProject.VideoSettings.VideoFile))
                return false;

            return OpenVideoFile(ParentProject.VideoSettings.VideoFile);
        }

        private void BtnOpenVideoFile_Click(object sender, EventArgs e)
        {
            OpenVideoFileDialog.InitialDirectory = ParentProject?.ProjectFolder ?? "";
            OpenVideoFileDialog.FileName = ParentProject?.VideoSettings.VideoFile ?? "";
            if (OpenVideoFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (OpenVideoFile(OpenVideoFileDialog.FileName))
                {
                    if (ParentProject != null)
                    {
                        ParentProject.VideoSettings.VideoFile = OpenVideoFileDialog.FileName;
                        ParentProject.IsDirty = true;
                    }
                    BtnPause.Focus();
                }
            }
        }

        private string VideoPositionToString(double pos)
        {
            var totalLength = MPlayer?.Length ?? -1;
            if (totalLength <= 0)
                return "N/A";

            var posMs = totalLength * pos;
            var span = TimeSpan.FromMilliseconds(posMs);
            return Math.Floor(span.TotalMinutes) + ":" + span.Seconds.ToString("00");// + "\nTime";
        }

        private void SetProgressBar(int pos, int max)
        {
            void MethodInvokerDelegate()
            {
                ProgressBarVideo.Minimum = 0;
                ProgressBarVideo.Maximum = max;
                ProgressBarVideo.Value = Math.Clamp(pos, 0, max);
            }

            //This will be true if Current thread is not UI thread.
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)MethodInvokerDelegate);
            else
                MethodInvokerDelegate();
        }

        private void UpdateProjectPositionFromVideo(int pos)
        {
            void MethodInvokerDelegate()
            {
                // update parent's list position
                ParentProject?.GotoVideoOffset(pos);
            }

            if (ParentProject == null)
                return;

            //This will be true if Current thread is not UI thread.
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)MethodInvokerDelegate);
            else
                MethodInvokerDelegate();
        }

        private void OnMediaPlayerPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            if (MPlayer != null)
            {
                var pos = (int)Math.Floor(e.Position * (double)MPlayer.Length);
                if ((pos >= 0) && (MPlayer.Length > 0))
                {
                    SetProgressBar(pos, (int)MPlayer.Length);
                }
                else
                {
                    SetProgressBar(0, 100);
                }

                MPlayer?.SetMarqueeInt(VideoMarqueeOption.Enable, 1);
                MPlayer?.SetMarqueeInt(VideoMarqueeOption.Position, 6);
                MPlayer?.SetMarqueeString(VideoMarqueeOption.Text, VideoPositionToString(e.Position));

                if (PMFollowPackets.Checked)
                    UpdateProjectPositionFromVideo(pos);
            }
            else
            {
                SetProgressBar(0, 100);
            }
        }

        private void VideoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var mediaPlayer = MPlayer;
            MPlayer = null;
            mediaPlayer?.Dispose();
            LibVlc?.Dispose();
            LibVlc = null;
            if (ParentProject != null)
                ParentProject.Video = null;
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            MPlayer?.Play();
        }

        private void VideoForm_Deactivate(object sender, EventArgs e)
        {
            MPlayer?.SetPause(true);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            MPlayer?.Stop();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            MPlayer?.Pause();
        }

        private void MPlayerSeekTo(TimeSpan pos)
        {
            if ((MPlayer != null) && (pos >= TimeSpan.Zero) && (pos <= TimeSpan.FromMilliseconds(MPlayer.Length)))
                MPlayer.SeekTo(pos);
        }

        private void BtnFastForward_Click(object sender, EventArgs e)
        {
            // Skip forward 20 seconds (or 1 if shift is held)
            var delta = (ModifierKeys & Keys.ShiftKey) != 0 ? 1000.0 : 20000.0;
            if ((MPlayer?.IsSeekable ?? false) && (MPlayer.Length > 0))
                MPlayerSeekTo(TimeSpan.FromMilliseconds((MPlayer.Position * MPlayer.Length) + delta));
        }

        private void BtnRewind_Click(object sender, EventArgs e)
        {
            // Skip back 20 seconds (or 1 if shift is held)
            var delta = (ModifierKeys & Keys.ShiftKey) != 0 ? -1000.0 : -20000.0;
            if ((MPlayer?.IsSeekable ?? false) && (MPlayer.Length > 0))
                MPlayerSeekTo(TimeSpan.FromMilliseconds((MPlayer.Position * MPlayer.Length) + delta));
        }

        private void BtnSeekStart_Click(object sender, EventArgs e)
        {
            // Seek to beginning of the video
            if (MPlayer?.IsSeekable ?? false)
                MPlayerSeekTo(TimeSpan.FromMilliseconds(1));
        }

        private void BtnSeekEnd_Click(object sender, EventArgs e)
        {
            // Seek to the end of the video
            if ((MPlayer?.IsSeekable ?? false) && (MPlayer.Length > 0))
                MPlayerSeekTo(TimeSpan.FromMilliseconds(MPlayer.Length - 1));
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            if (MPlayer != null)
            {
                MPlayer.Mute = !MPlayer.Mute;
                BtnMute.Image = MPlayer.Mute ? Resources.audio_on_32 : Resources.audio_volume_muted_32;
            }
        }

        private void VideoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MPlayer?.Stop();
        }

        private void VideoViewPort_Click(object sender, EventArgs e)
        {
            if (MouseButtons.HasFlag(MouseButtons.Right))
            {
                // Right-click opens popup menu
                PopupMenuVideo.Show(MousePosition, ToolStripDropDownDirection.Default);
            }
            else
            {
                // otherwise, treat it as a pause button
                BtnPause.Focus();
                BtnPause_Click(sender, e);
            }
        }

        private void PMChangeSpeed_Click(object sender, EventArgs e)
        {
            if ((sender is ToolStripMenuItem { Tag: string multiplierString }))
            {
                if (float.TryParse(multiplierString, NumberStyles.Float, CultureInfo.InvariantCulture, out var multiplier))
                    if (multiplier > 0)
                        MPlayer?.SetRate(multiplier);
            }
        }

        private void SeekVideoFromProgressMouseClick(int x)
        {
            if (MPlayer is { Length: > 0 })
            {
                var barPositionX = x - ProgressBarVideo.Left;
                var barPosition = (double)barPositionX / ProgressBarVideo.Width;
                var barMs = barPosition * MPlayer.Length;
                MPlayer?.SeekTo(TimeSpan.FromMilliseconds(barMs));
            }
        }

        private void ProgressBarVideo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
                SeekVideoFromProgressMouseClick(e.X);
        }

        private void ProgressBarVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
                SeekVideoFromProgressMouseClick(e.X);
        }

        public void UpdateVideoPositionFromProject(TimeSpan packetDataOffset)
        {
            if (PMFollowPackets.Checked == false)
                return;

            // Only allow updating from the packet list if the video is not playing
            // This will prevent circular reference updates
            if (MPlayer is { IsPlaying: false })
            {
                var pos = (int)Math.Floor(MPlayer.Position * (double)MPlayer.Length);
                if ((pos >= 0) && (MPlayer.Length > 0) && (packetDataOffset >= TimeSpan.Zero) && (pos < MPlayer.Length))
                {
                    MPlayerSeekTo(packetDataOffset);

                    SetProgressBar(pos, (int)MPlayer.Length);

                    MPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 1);
                    MPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 6);
                    MPlayer.SetMarqueeString(VideoMarqueeOption.Text, VideoPositionToString(MPlayer.Position));
                }
                else
                {
                    SetProgressBar(0, 100);

                    MPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 1);
                    MPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 6);
                    MPlayer.SetMarqueeString(VideoMarqueeOption.Text, "Out of range");
                }
            }
        }

        private void PMFollowPackets_Click(object sender, EventArgs e)
        {
            PMFollowPackets.Checked = !PMFollowPackets.Checked;
        }

        private void PMKeepOnTop_Click(object sender, EventArgs e)
        {
            PMKeepOnTop.Checked = !PMKeepOnTop.Checked;
            TopMost = PMKeepOnTop.Checked;
        }

        private void PMSync_Click(object sender, EventArgs e)
        {
            if (MPlayer == null)
                return;
            if (ParentProject == null)
                return;
            if (ParentProject.PacketsListBox.SelectedItem is not BasePacketData data)
                return;

            MPlayer.SetPause(true);

            var currentVideoOffset = TimeSpan.FromMilliseconds(MPlayer.Length * MPlayer.Position);
            var dataOffset = data.VirtualOffsetFromStart;
            var newOffset = dataOffset - currentVideoOffset;
            var delta = newOffset - ParentProject.VideoSettings.VideoOffset;

            // Ignore if delta is too low
            if ((delta.TotalMilliseconds >= -10) && (delta.TotalMilliseconds <= 10))
            {
                MessageBox.Show("Already synced to this position", "Sync with project", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                    "Do you want to update the video syncronization offset to\n" +
                    newOffset.ToString()+ "? \n\n" +
                    "Previous offset was\n" +
                    ParentProject.VideoSettings.VideoOffset.ToString()+"\n\n" +
                    "Difference = " + delta.ToString(),
                    "Sync with project", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            ParentProject.VideoSettings.VideoOffset = newOffset;
            ParentProject.IsDirty = true;
        }
    }
}
