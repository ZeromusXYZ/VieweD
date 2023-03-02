using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class VideoForm : Form
    {
        private LibVLC? LibVLC { get; set; }
        private MediaPlayer? MPlayer { get; set; }
        public ViewedProjectTab? ParentProject { get; set; }


        public VideoForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ParentProject == null)
                return;
            if (!File.Exists(ParentProject.VideoSettings.VideoFile))
                return;
            LibVLC = new LibVLC(enableDebugLogs: true);
            MPlayer = new MediaPlayer(LibVLC);
            MPlayer.PositionChanged += OnMediaPlayerPositionChanged;
            VideoViewPort.MediaPlayer = MPlayer;
            var media = new Media(LibVLC, ParentProject.VideoSettings.VideoFile);
            //var media = new Media(LibVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            MPlayer.Play(media);
            //MPlayer.Pause();
            //MPlayer.Position = 0.0f;
            //MPlayer.NextFrame();
            media.Dispose();
        }

        private string VideoPositionToString(float pos)
        {
            var totalLength = MPlayer?.Length ?? -1;
            if (totalLength <= 0)
                return "N/A";

            var posMs = (double)totalLength * pos;
            var span = TimeSpan.FromMilliseconds(posMs);
            return Math.Floor(span.TotalMinutes) + ":" + span.Seconds.ToString("00") + "\nTime";
        }

        private void OnMediaPlayerPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            MPlayer?.SetMarqueeInt(VideoMarqueeOption.Enable, 1);
            MPlayer?.SetMarqueeInt(VideoMarqueeOption.Position, 6);
            MPlayer?.SetMarqueeString(VideoMarqueeOption.Text, VideoPositionToString(e.Position));
        }

        private void VideoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var mediaPlayer = MPlayer;
            MPlayer = null;
            mediaPlayer?.Dispose();
            LibVLC?.Dispose();
            LibVLC = null;
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
    }
}
