using System.Windows.Forms;
using System.Drawing;
using VieweD.Properties;

namespace VieweD.Forms
{
    partial class VideoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoForm));
            BtnOpenVideoFile = new Button();
            VideoViewPort = new LibVLCSharp.WinForms.VideoView();
            BtnPlay = new Button();
            PanelControls = new Panel();
            BtnOpenVideoURI = new Button();
            BtnMute = new Button();
            BtnSeekEnd = new Button();
            BtnSeekStart = new Button();
            BtnFastForward = new Button();
            BtnRewind = new Button();
            BtnStop = new Button();
            BtnPause = new Button();
            ProgressBarVideo = new ProgressBar();
            PopupMenuVideo = new ContextMenuStrip(components);
            PMFollowPackets = new ToolStripMenuItem();
            PMKeepOnTop = new ToolStripMenuItem();
            PMN1 = new ToolStripSeparator();
            PMSpeed = new ToolStripMenuItem();
            PMSpeed1 = new ToolStripMenuItem();
            PMSpeedN1 = new ToolStripSeparator();
            PMSpeed2 = new ToolStripMenuItem();
            PMSpeed4 = new ToolStripMenuItem();
            PMSpeed8 = new ToolStripMenuItem();
            PMSpeedN2 = new ToolStripSeparator();
            PMSpeedBy2 = new ToolStripMenuItem();
            PMSpeedBy4 = new ToolStripMenuItem();
            PMN2 = new ToolStripSeparator();
            PMSync = new ToolStripMenuItem();
            OpenVideoFileDialog = new OpenFileDialog();
            MarqueeTimer = new Timer(components);
            ((System.ComponentModel.ISupportInitialize)VideoViewPort).BeginInit();
            PanelControls.SuspendLayout();
            PopupMenuVideo.SuspendLayout();
            SuspendLayout();
            // 
            // BtnOpenVideoFile
            // 
            BtnOpenVideoFile.Image = Resources.media_eject_32;
            BtnOpenVideoFile.Location = new Point(12, 20);
            BtnOpenVideoFile.Name = "BtnOpenVideoFile";
            BtnOpenVideoFile.Size = new Size(40, 40);
            BtnOpenVideoFile.TabIndex = 0;
            BtnOpenVideoFile.UseVisualStyleBackColor = true;
            BtnOpenVideoFile.Click += BtnOpenVideoFile_Click;
            // 
            // VideoViewPort
            // 
            VideoViewPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewPort.BackColor = Color.Black;
            VideoViewPort.Location = new Point(1, 1);
            VideoViewPort.MediaPlayer = null;
            VideoViewPort.Name = "VideoViewPort";
            VideoViewPort.Size = new Size(522, 268);
            VideoViewPort.TabIndex = 21;
            VideoViewPort.Text = "videoView1";
            VideoViewPort.Click += VideoViewPort_Click;
            // 
            // BtnPlay
            // 
            BtnPlay.Image = Resources.media_playback_start_32;
            BtnPlay.Location = new Point(260, 20);
            BtnPlay.Name = "BtnPlay";
            BtnPlay.Size = new Size(40, 40);
            BtnPlay.TabIndex = 4;
            BtnPlay.UseVisualStyleBackColor = true;
            BtnPlay.Click += BtnPlay_Click;
            // 
            // PanelControls
            // 
            PanelControls.Controls.Add(BtnOpenVideoURI);
            PanelControls.Controls.Add(BtnMute);
            PanelControls.Controls.Add(BtnSeekEnd);
            PanelControls.Controls.Add(BtnSeekStart);
            PanelControls.Controls.Add(BtnFastForward);
            PanelControls.Controls.Add(BtnRewind);
            PanelControls.Controls.Add(BtnStop);
            PanelControls.Controls.Add(BtnPause);
            PanelControls.Controls.Add(ProgressBarVideo);
            PanelControls.Controls.Add(BtnPlay);
            PanelControls.Controls.Add(BtnOpenVideoFile);
            PanelControls.Dock = DockStyle.Bottom;
            PanelControls.Location = new Point(0, 271);
            PanelControls.Name = "PanelControls";
            PanelControls.Size = new Size(524, 70);
            PanelControls.TabIndex = 3;
            // 
            // BtnOpenVideoURI
            // 
            BtnOpenVideoURI.Image = Resources.im_youtube_32;
            BtnOpenVideoURI.Location = new Point(58, 20);
            BtnOpenVideoURI.Name = "BtnOpenVideoURI";
            BtnOpenVideoURI.Size = new Size(40, 40);
            BtnOpenVideoURI.TabIndex = 21;
            BtnOpenVideoURI.UseVisualStyleBackColor = true;
            BtnOpenVideoURI.Visible = false;
            BtnOpenVideoURI.Click += BtnOpenVideoURI_Click;
            // 
            // BtnMute
            // 
            BtnMute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnMute.Image = Resources.audio_volume_muted_32;
            BtnMute.Location = new Point(466, 20);
            BtnMute.Name = "BtnMute";
            BtnMute.Size = new Size(40, 40);
            BtnMute.TabIndex = 8;
            BtnMute.UseVisualStyleBackColor = true;
            BtnMute.Click += BtnMute_Click;
            // 
            // BtnSeekEnd
            // 
            BtnSeekEnd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnSeekEnd.Image = Resources.media_skip_forward_32;
            BtnSeekEnd.Location = new Point(398, 20);
            BtnSeekEnd.Name = "BtnSeekEnd";
            BtnSeekEnd.Size = new Size(40, 40);
            BtnSeekEnd.TabIndex = 7;
            BtnSeekEnd.UseVisualStyleBackColor = true;
            BtnSeekEnd.Click += BtnSeekEnd_Click;
            // 
            // BtnSeekStart
            // 
            BtnSeekStart.Image = Resources.media_skip_backward_32;
            BtnSeekStart.Location = new Point(168, 20);
            BtnSeekStart.Name = "BtnSeekStart";
            BtnSeekStart.Size = new Size(40, 40);
            BtnSeekStart.TabIndex = 2;
            BtnSeekStart.UseVisualStyleBackColor = true;
            BtnSeekStart.Click += BtnSeekStart_Click;
            // 
            // BtnFastForward
            // 
            BtnFastForward.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnFastForward.Image = Resources.media_seek_forward_32;
            BtnFastForward.Location = new Point(352, 20);
            BtnFastForward.Name = "BtnFastForward";
            BtnFastForward.Size = new Size(40, 40);
            BtnFastForward.TabIndex = 6;
            BtnFastForward.UseVisualStyleBackColor = true;
            BtnFastForward.Click += BtnFastForward_Click;
            // 
            // BtnRewind
            // 
            BtnRewind.Image = Resources.media_seek_backward_32;
            BtnRewind.Location = new Point(214, 20);
            BtnRewind.Name = "BtnRewind";
            BtnRewind.Size = new Size(40, 40);
            BtnRewind.TabIndex = 3;
            BtnRewind.UseVisualStyleBackColor = true;
            BtnRewind.Click += BtnRewind_Click;
            // 
            // BtnStop
            // 
            BtnStop.Image = Resources.media_playback_stop_32;
            BtnStop.Location = new Point(113, 20);
            BtnStop.Name = "BtnStop";
            BtnStop.Size = new Size(40, 40);
            BtnStop.TabIndex = 1;
            BtnStop.UseVisualStyleBackColor = true;
            BtnStop.Click += BtnStop_Click;
            // 
            // BtnPause
            // 
            BtnPause.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnPause.Image = Resources.media_playback_pause_32;
            BtnPause.Location = new Point(306, 20);
            BtnPause.Name = "BtnPause";
            BtnPause.Size = new Size(40, 40);
            BtnPause.TabIndex = 5;
            BtnPause.UseVisualStyleBackColor = true;
            BtnPause.Click += BtnPause_Click;
            // 
            // ProgressBarVideo
            // 
            ProgressBarVideo.Cursor = Cursors.VSplit;
            ProgressBarVideo.Dock = DockStyle.Top;
            ProgressBarVideo.Location = new Point(0, 0);
            ProgressBarVideo.Name = "ProgressBarVideo";
            ProgressBarVideo.Size = new Size(524, 14);
            ProgressBarVideo.TabIndex = 20;
            ProgressBarVideo.MouseClick += ProgressBarVideo_MouseClick;
            ProgressBarVideo.MouseMove += ProgressBarVideo_MouseMove;
            // 
            // PopupMenuVideo
            // 
            PopupMenuVideo.Items.AddRange(new ToolStripItem[] { PMFollowPackets, PMKeepOnTop, PMN1, PMSpeed, PMN2, PMSync });
            PopupMenuVideo.Name = "PopupMenuVideo";
            PopupMenuVideo.Size = new Size(166, 104);
            // 
            // PMFollowPackets
            // 
            PMFollowPackets.Checked = true;
            PMFollowPackets.CheckState = CheckState.Checked;
            PMFollowPackets.Enabled = false;
            PMFollowPackets.Name = "PMFollowPackets";
            PMFollowPackets.Size = new Size(165, 22);
            PMFollowPackets.Text = "Follow packets";
            PMFollowPackets.Click += PMFollowPackets_Click;
            // 
            // PMKeepOnTop
            // 
            PMKeepOnTop.Checked = true;
            PMKeepOnTop.CheckState = CheckState.Checked;
            PMKeepOnTop.Name = "PMKeepOnTop";
            PMKeepOnTop.Size = new Size(165, 22);
            PMKeepOnTop.Text = "Keep on top";
            PMKeepOnTop.Click += PMKeepOnTop_Click;
            // 
            // PMN1
            // 
            PMN1.Name = "PMN1";
            PMN1.Size = new Size(162, 6);
            // 
            // PMSpeed
            // 
            PMSpeed.DropDownItems.AddRange(new ToolStripItem[] { PMSpeed1, PMSpeedN1, PMSpeed2, PMSpeed4, PMSpeed8, PMSpeedN2, PMSpeedBy2, PMSpeedBy4 });
            PMSpeed.Name = "PMSpeed";
            PMSpeed.Size = new Size(165, 22);
            PMSpeed.Text = "Playback Speed";
            // 
            // PMSpeed1
            // 
            PMSpeed1.Name = "PMSpeed1";
            PMSpeed1.Size = new Size(137, 22);
            PMSpeed1.Tag = "1";
            PMSpeed1.Text = "x1 (Normal)";
            PMSpeed1.Click += PMChangeSpeed_Click;
            // 
            // PMSpeedN1
            // 
            PMSpeedN1.Name = "PMSpeedN1";
            PMSpeedN1.Size = new Size(134, 6);
            // 
            // PMSpeed2
            // 
            PMSpeed2.Name = "PMSpeed2";
            PMSpeed2.Size = new Size(137, 22);
            PMSpeed2.Tag = "2";
            PMSpeed2.Text = "x2 (Double)";
            PMSpeed2.Click += PMChangeSpeed_Click;
            // 
            // PMSpeed4
            // 
            PMSpeed4.Name = "PMSpeed4";
            PMSpeed4.Size = new Size(137, 22);
            PMSpeed4.Tag = "4";
            PMSpeed4.Text = "x4";
            PMSpeed4.Click += PMChangeSpeed_Click;
            // 
            // PMSpeed8
            // 
            PMSpeed8.Name = "PMSpeed8";
            PMSpeed8.Size = new Size(137, 22);
            PMSpeed8.Tag = "8";
            PMSpeed8.Text = "x8";
            PMSpeed8.Click += PMChangeSpeed_Click;
            // 
            // PMSpeedN2
            // 
            PMSpeedN2.Name = "PMSpeedN2";
            PMSpeedN2.Size = new Size(134, 6);
            // 
            // PMSpeedBy2
            // 
            PMSpeedBy2.Name = "PMSpeedBy2";
            PMSpeedBy2.Size = new Size(137, 22);
            PMSpeedBy2.Tag = "0.5";
            PMSpeedBy2.Text = "/2 (Half)";
            PMSpeedBy2.Click += PMChangeSpeed_Click;
            // 
            // PMSpeedBy4
            // 
            PMSpeedBy4.Name = "PMSpeedBy4";
            PMSpeedBy4.Size = new Size(137, 22);
            PMSpeedBy4.Tag = "0.25";
            PMSpeedBy4.Text = "/4";
            PMSpeedBy4.Click += PMChangeSpeed_Click;
            // 
            // PMN2
            // 
            PMN2.Name = "PMN2";
            PMN2.Size = new Size(162, 6);
            // 
            // PMSync
            // 
            PMSync.Enabled = false;
            PMSync.Name = "PMSync";
            PMSync.Size = new Size(165, 22);
            PMSync.Text = Resources.SyncWithProjectTitle;
            PMSync.Click += PMSync_Click;
            // 
            // OpenVideoFileDialog
            // 
            OpenVideoFileDialog.Filter = "All files|*.*";
            OpenVideoFileDialog.Title = "Open video file";
            // 
            // MarqueeTimer
            // 
            MarqueeTimer.Interval = 50;
            MarqueeTimer.Tick += MarqueeTimer_Tick;
            // 
            // VideoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 341);
            ContextMenuStrip = PopupMenuVideo;
            Controls.Add(PanelControls);
            Controls.Add(VideoViewPort);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(540, 380);
            Name = "VideoForm";
            Text = "Video";
            TopMost = true;
            Deactivate += VideoForm_Deactivate;
            FormClosing += VideoForm_FormClosing;
            FormClosed += VideoForm_FormClosed;
            Load += VideoForm_Load;
            ((System.ComponentModel.ISupportInitialize)VideoViewPort).EndInit();
            PanelControls.ResumeLayout(false);
            PopupMenuVideo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button BtnOpenVideoFile;
        private LibVLCSharp.WinForms.VideoView VideoViewPort;
        private Button BtnPlay;
        private Panel PanelControls;
        private ProgressBar ProgressBarVideo;
        private Button BtnStop;
        private Button BtnPause;
        private Button BtnSeekEnd;
        private Button BtnSeekStart;
        private Button BtnFastForward;
        private Button BtnRewind;
        private Button BtnMute;
        private ContextMenuStrip PopupMenuVideo;
        private ToolStripMenuItem PMFollowPackets;
        private ToolStripSeparator PMN1;
        private ToolStripMenuItem PMSpeed;
        private ToolStripMenuItem PMSpeed1;
        private ToolStripSeparator PMSpeedN1;
        private ToolStripMenuItem PMSpeed2;
        private ToolStripMenuItem PMSpeed4;
        private ToolStripMenuItem PMSpeed8;
        private ToolStripSeparator PMSpeedN2;
        private ToolStripMenuItem PMSpeedBy2;
        private ToolStripMenuItem PMSpeedBy4;
        private OpenFileDialog OpenVideoFileDialog;
        private ToolStripMenuItem PMKeepOnTop;
        private ToolStripSeparator PMN2;
        private ToolStripMenuItem PMSync;
        private Button BtnOpenVideoURI;
        private Timer MarqueeTimer;
    }
}