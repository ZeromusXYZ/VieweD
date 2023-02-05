using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using VieweD.Helpers.System;
using Microsoft.Win32;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class VideoLinkForm : Form
    {
        public PacketTabPage sourceTP { get; set; }
        private bool blockPositionUpdates = false;
        private bool closeOnStop = false;
        private const string DefaultDllName = "libvlc.dll" ;

        public VideoLinkForm()
        {
            InitializeComponent();
        }

        public static string GetVLCLibPath()
        {
            // First try to get from registry
            string res;
            try
            {
                res = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\VideoLAN\\VLC", "InstallDir", "")?.ToString() ??
                      "";
                if (File.Exists(Path.Combine(res, DefaultDllName)))
                    return res;
            }
            catch
            {
                // Ignore
            }

            // Try default location
            res = Path.Combine(
                Environment.GetFolderPath(Environment.Is64BitProcess
                    ? Environment.SpecialFolder.ProgramFiles
                    : Environment.SpecialFolder.ProgramFilesX86),
                "VideoLAN", "VLC");
            if (File.Exists(Path.Combine(res, DefaultDllName)))
                return res;

            res = "";
            return res;
        }

        private void Media_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var dir = GetVLCLibPath();
            if (dir == string.Empty)
                e.VlcLibDirectory = null; // this will throw a exception
            else
                e.VlcLibDirectory = new DirectoryInfo(dir);
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (openVideoDlg.ShowDialog() != DialogResult.OK)
                return;
            if (LoadVideoFromLocalFile(openVideoDlg.FileName))
            {
                if (sourceTP != null)
                    sourceTP.LinkVideoFileName = openVideoDlg.FileName;
                media.VlcMediaPlayer.Play();
                media.VlcMediaPlayer.Pause();
                media.VlcMediaPlayer.NextFrame();
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (media.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                media.Pause();
            else
                media.Play();
        }

        private void VideoLinkForm_Load(object sender, EventArgs e)
        {
            Left = MainForm.ThisMainForm.Right - Width - 16;
            Top = MainForm.ThisMainForm.Bottom - Height - 16;
            if (sourceTP == null)
            {
                Text = "Video not attached to a packet list";
                btnSetOffset.Enabled = false;
                cbFollowPacketList.Checked = false;
                cbFollowPacketList.Enabled = false;
                return;
            }
            if (!File.Exists(sourceTP.LoadedLogFile))
            {
                MessageBox.Show("Can only link video to complete log files", "Video Link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Text = "Video not attached to a packet list";
                sourceTP = null;
                return;
            }
            Text = "Video - " + sourceTP.LoadedLogFile;
            sourceTP.VideoLink = this;
            LoadVideoFromProjectFile();
        }

        public bool LoadVideoFromLocalFile(string filename)
        {
            media.SetMedia(new Uri("file://" + filename));
            return true;
        }

        public async Task<bool> LoadVideoFromYoutubeURLAsync(string URL)
        {
            bool res = false;
            eYoutubeURL.Enabled = false;
            btnStreamYT.Enabled = false;
            try
            {
                var youtube = new YoutubeExplode.YoutubeClient();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(URL);
                // Get media streams & choose the best muxed stream
                var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

                if (streamInfo == null)
                {
                    Console.Error.WriteLine("This videos has no streams");
                    MessageBox.Show(@"This videos has no streams", "Load Youtube Error");
                    res = false;
                }
                else
                {
                    media.SetMedia(new Uri(streamInfo.Url));
                    if (sourceTP != null)
                        sourceTP.LinkYoutubeUrl = streamInfo.Url;
                    res = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load YouTube Exception");
                if (sourceTP != null)
                    sourceTP.LinkYoutubeUrl = string.Empty;
            }
            eYoutubeURL.Enabled = true;
            btnStreamYT.Enabled = true;
            return res;
        }

        public bool LoadVideoFromProjectFile()
        {
            if (sourceTP == null)
                return false;

            try
            {
                if (File.Exists(sourceTP.LinkVideoFileName))
                {
                    if (!LoadVideoFromLocalFile(sourceTP.LinkVideoFileName))
                    {
                        sourceTP.LinkVideoFileName = string.Empty;
                    }
                    else
                    {
                        lYouTube.Enabled = false;
                        eYoutubeURL.Enabled = false;
                        btnStreamYT.Enabled = false;
                    }
                }
                else
                if ((sourceTP.LinkYoutubeUrl.ToLower().StartsWith("http://")) || (sourceTP.LinkYoutubeUrl.ToLower().StartsWith("https://")))
                {
                    var l = LoadVideoFromYoutubeURLAsync(sourceTP.LinkYoutubeUrl);
                    /*
                    if (!LoadVideoFromYoutube(sourceTP.LinkYoutubeURL))
                        sourceTP.LinkYoutubeURL = string.Empty;
                    */
                }
                else
                {
                    sourceTP.LinkVideoFileName = string.Empty;
                    sourceTP.LinkYoutubeUrl = string.Empty;
                }

                if ((sourceTP.LinkVideoFileName != string.Empty) || (sourceTP.LinkYoutubeUrl != string.Empty))
                {

                    media.VlcMediaPlayer.Play();
                    media.VlcMediaPlayer.Pause();
                    media.VlcMediaPlayer.NextFrame();
                }
            }
            catch
            {
                lYouTube.Enabled = true;
                eYoutubeURL.Enabled = true;
                btnStreamYT.Enabled = true;
                sourceTP.LinkVideoFileName = string.Empty;
                sourceTP.LinkYoutubeUrl = string.Empty;
            }

            eYoutubeURL.Text = sourceTP.LinkYoutubeUrl;
            if (sourceTP.LinkYoutubeUrl != string.Empty)
                eYoutubeURL.ReadOnly = true;

            return true;
        }

        private void VideoLinkForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            packetUpdateTimer.Enabled = false;
            if (sourceTP != null)
            {
                sourceTP.VideoLink = null;
                sourceTP = null;
            }

            if (!media.IsPlaying) 
                return;

            try
            {
                e.Cancel = true;
                closeOnStop = true;
                closeFixTimer.Enabled = true;
                media.ResetMedia();
            }
            catch
            {
                // Do nothing
            }
        }

        private void Media_LengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            if (closeOnStop)
                return;

            try
            {
                Invoke((MethodInvoker)delegate
                {
                    tb.Maximum = (int)e.NewLength;
                    tb.Minimum = 0;
                });
            }
            catch
            {
                // Do nothing
            }
        }

        private void UpdateTimeLabelAndList(long pos, long max, bool updatePacketList)
        {
            lVideoCurrentTime.Text = MediaTimeToString(pos);
            lVideoMaxTime.Text = MediaTimeToString(max);
            // lVideoPosition.Text = "Time: " + MediaTimeToString(pos) + " / " + MediaTimeToString(max);

            var p = Math.Min(Math.Max(pos, 0), max);
            tb.Maximum = (int)max;
            tb.Value = (int)p;

            if ((sourceTP != null) && (updatePacketList))
            {
                var start = sourceTP.PLLoaded.FirstPacketTime;
                var videopos = TimeSpan.FromMilliseconds(pos);
                var off = start.Add(videopos).Add(sourceTP.LinkVideoOffset);
                var nowIndex = sourceTP.LbPackets.SelectedIndex;
                var newIndex = sourceTP.PL.FindPacketIndexByDateTime(off, nowIndex);

                // Account for a small bug that I think it related to the redraw or threading, only update if new position is "valid"
                if (newIndex >= 0)
                    sourceTP.LbPackets.SelectedIndex = newIndex;
                sourceTP.CenterListBox();
            }
        }

        private void Media_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (closeOnStop)
                return;

            if (blockPositionUpdates)
                return;

            blockPositionUpdates = true;
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    lWarningLabel.Visible = false;
                    UpdateTimeLabelAndList((long)(e.NewPosition * media.Length), media.Length,
                        cbFollowPacketList.Checked);
                    lVideoSpeed.Text = "x" + media.VlcMediaPlayer.Rate.ToString();
                });
            }
            catch
            {
                // Do nothing
            }
            blockPositionUpdates = false;
        }

        private static string MediaTimeToString(long position)
        {
            var h = (position / 3600000);
            var m = ((position / 60000) % 60);
            var s = ((position / 1000) % 60);
            string res = "";
            if (h > 0)
                res += h.ToString("00") + ":";
            res += m.ToString("00") + "." + s.ToString("00");
            return res;
        }

        private void Tb_Scroll(object sender, EventArgs e)
        {
            if (blockPositionUpdates)
                return;
            blockPositionUpdates = true;

            media.Time = tb.Value;
            //media.Position = ((float)tb.Value / (float)tb.Maximum);
            UpdateTimeLabelAndList(tb.Value, tb.Maximum, cbFollowPacketList.Checked);

            blockPositionUpdates = false;
        }

        public bool IsInTimeRange(DateTime packetTime)
        {
            TimeSpan off = packetTime - sourceTP.PLLoaded.FirstPacketTime;
            off = off.Subtract(sourceTP.LinkVideoOffset);
            if (off.TotalMilliseconds < 0)
            {
                return false;
            }
            else
            if (off.TotalMilliseconds > tb.Maximum)
            {
                return false;
            }
            return true;
        }

        public void MoveToDateTime(DateTime packetTime)
        {
            if (blockPositionUpdates)
                return;
            blockPositionUpdates = true;

            TimeSpan off = packetTime - sourceTP.PL.FirstPacketTime ;
            off = off.Subtract(sourceTP.LinkVideoOffset);
            UpdateTimeLabelAndList((int)off.TotalMilliseconds, tb.Maximum, false);
            media.Time = (long)off.TotalMilliseconds;
            //media.Position = (float)(off.TotalMilliseconds / media.Length);
            if (off.TotalMilliseconds < 0)
            {
                lWarningLabel.Text = "Negative Offset";
                lWarningLabel.Visible = true;
                media.Visible = false;
            }
            else
            if (off.TotalMilliseconds > tb.Maximum)
            {
                lWarningLabel.Text = "Out of video range";
                lWarningLabel.Visible = true;
                media.Visible = false;
            }
            else
            {
                lWarningLabel.Visible = false;
                media.Visible = true;
            }

            blockPositionUpdates = false;
        }

        private void VideoLinkForm_Deactivate(object sender, EventArgs e)
        {
            // pause if playing
            if (media.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                media.Pause();
        }

        private void Media_Paused(object sender, Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs e)
        {
            if (closeOnStop)
                return;
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    //btnPlay.Text = "Play";
                    btnPlay.ImageIndex = 0;
                });
            }
            catch
            {
                // Do nothing
            }
        }

        private void Media_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            if (closeOnStop)
                return;

            try
            {
                Invoke((MethodInvoker)delegate
                {
                    //btnPlay.Text = "Pause";
                    btnPlay.ImageIndex = 1;
                });
            }
            catch 
            {
                // Do nothing
            }
        }

        private void BtnTestYT_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;
            var s = eYoutubeURL.Text;

            var l = LoadVideoFromYoutubeURLAsync(s);
            /*
            if (l.GetAwaiter().GetResult())
            {
                if (sourceTP != null)
                    sourceTP.LinkYoutubeURL= s;
            }
            else
            {
                MessageBox.Show("Link does not seem valid, or could not access the page.", "Test Youtube Link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */
            Application.UseWaitCursor = false;
        }

        private void CbStayOnTop_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = cbStayOnTop.Checked;
        }

        private void BtnSetOffset_Click(object sender, EventArgs e)
        {
            if (sourceTP == null)
            {
                MessageBox.Show("Not linked to a packet log", "Set Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Pause if we're still playing
            if (media.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                media.Pause();

            var thisPacket = sourceTP.GetSelectedPacket();
            if (thisPacket == null)
            {
                MessageBox.Show("No packet selected to offset to", "Set Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var videoTime = TimeSpan.FromMilliseconds(media.Time);// media.Position * media.Length);
            var packetTime = thisPacket.VirtualTimeStamp - sourceTP.PL.FirstPacketTime ;
            var off = packetTime - videoTime;
            var currentVideoLinkOffset = sourceTP.LinkVideoOffset;

            if (MessageBox.Show("Set Link ?\r\n\r\n"+
                "Current Offset: " + currentVideoLinkOffset.ToString() + "\r\n\r\n" +
                "Packet Time: " + packetTime.ToString() + "\r\n" +
                "Video Time: " +videoTime.ToString() + "\r\n\r\n" +
                "Difference: " + off.ToString(),
                "Set Offset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sourceTP.LinkVideoOffset = off;
                cbFollowPacketList.Checked = true;
            }

        }

        private void BtnNextFrame_Click(object sender, EventArgs e)
        {
            if (media.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                media.Pause();
            media.VlcMediaPlayer.NextFrame();
            var newPos = media.Time;// (long)(media.Position * media.Length);
            UpdateTimeLabelAndList(newPos, media.Length, cbFollowPacketList.Checked);
        }

        private void BtnPrevFrame_Click(object sender, EventArgs e)
        {
            if (media.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                media.Pause();
            var newPos = media.Time;// (long)(media.Position * media.Length);
            newPos -= 1000;
            if (newPos < 0)
                newPos = 0;
            media.Time = newPos;
            //media.VlcMediaPlayer.Position = ((float)newPos / (float)media.Length);
            UpdateTimeLabelAndList(newPos, media.Length, cbFollowPacketList.Checked);
        }

        private void Media_MediaChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerMediaChangedEventArgs e)
        {
            lWarningLabel.Visible = false;
            if (closeOnStop)
                Close();
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            media.VlcMediaPlayer.Audio.IsMute = !media.VlcMediaPlayer.Audio.IsMute;
            if (media.VlcMediaPlayer.Audio.IsMute)
            {
                btnMute.Text = "X ";
            }
            else
            {
                btnMute.Text = "X¯";
            }

        }

        private void Media_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            if (!closeOnStop)
                return;

            try
            {
                Invoke((MethodInvoker)Close);
            }
            catch 
            {
                // Do nothing
            }
        }

        private void CloseFixTimer_Tick(object sender, EventArgs e)
        {
            if (closeOnStop)
                Close();
        }

        private float SpeedTrackBarToSpeedRate(int val)
        {
            switch (val)
            {
                case -4: return 0.125f;
                case -3: return 0.25f;
                case -2: return 0.5f;
                case -1: return 0.75f;
                case 1: return 1.5f;
                case 2: return 2f;
                case 3: return 3f;
                case 4: return 4f;
                default: return 1f;
            }
        }

        private void tbPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            media.VlcMediaPlayer.Rate = SpeedTrackBarToSpeedRate(tbPlaybackSpeed.Value);
            lVideoSpeed.Text = "x" + media.VlcMediaPlayer.Rate;
        }
    }
}
