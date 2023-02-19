namespace VieweD
{
    partial class VideoLinkForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoLinkForm));
            this.media = new Vlc.DotNet.Forms.VlcControl();
            this.btnOpen = new System.Windows.Forms.Button();
            this.IL = new System.Windows.Forms.ImageList(this.components);
            this.openVideoDlg = new System.Windows.Forms.OpenFileDialog();
            this.btnPlay = new System.Windows.Forms.Button();
            this.tb = new System.Windows.Forms.TrackBar();
            this.lYouTube = new System.Windows.Forms.Label();
            this.eYoutubeURL = new System.Windows.Forms.TextBox();
            this.btnStreamYT = new System.Windows.Forms.Button();
            this.cbStayOnTop = new System.Windows.Forms.CheckBox();
            this.cbFollowPacketList = new System.Windows.Forms.CheckBox();
            this.btnSetOffset = new System.Windows.Forms.Button();
            this.lWarningLabel = new System.Windows.Forms.Label();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnPrevFrame = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.Button();
            this.closeFixTimer = new System.Windows.Forms.Timer(this.components);
            this.packetUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.buttonsToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.tbPlaybackSpeed = new System.Windows.Forms.TrackBar();
            this.lVideoCurrentTime = new System.Windows.Forms.Label();
            this.lVideoMaxTime = new System.Windows.Forms.Label();
            this.lVideoSpeed = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.media)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPlaybackSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // media
            // 
            this.media.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.media.BackColor = System.Drawing.Color.DimGray;
            this.media.ForeColor = System.Drawing.Color.Black;
            this.media.Location = new System.Drawing.Point(12, 39);
            this.media.Name = "media";
            this.media.Size = new System.Drawing.Size(560, 271);
            this.media.Spu = -1;
            this.media.TabIndex = 0;
            this.media.Text = "Video";
            this.media.VlcLibDirectory = null;
            this.media.VlcMediaplayerOptions = null;
            this.media.VlcLibDirectoryNeeded += new System.EventHandler<Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs>(this.Media_VlcLibDirectoryNeeded);
            this.media.LengthChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs>(this.Media_LengthChanged);
            this.media.MediaChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerMediaChangedEventArgs>(this.Media_MediaChanged);
            this.media.Paused += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs>(this.Media_Paused);
            this.media.Playing += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs>(this.Media_Playing);
            this.media.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.Media_PositionChanged);
            this.media.Stopped += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs>(this.Media_Stopped);
            // 
            // btnOpen
            // 
            this.btnOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpen.ImageIndex = 3;
            this.btnOpen.ImageList = this.IL;
            this.btnOpen.Location = new System.Drawing.Point(12, 10);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(89, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open Local";
            this.btnOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // IL
            // 
            this.IL.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL.ImageStream")));
            this.IL.TransparentColor = System.Drawing.Color.Transparent;
            this.IL.Images.SetKeyName(0, "Fairytale_player_play.png");
            this.IL.Images.SetKeyName(1, "Fairytale_player_pause.png");
            this.IL.Images.SetKeyName(2, "Fairytale_player_stop.png");
            this.IL.Images.SetKeyName(3, "Fairytale_player_eject.png");
            this.IL.Images.SetKeyName(4, "Fairytale_player_end.png");
            this.IL.Images.SetKeyName(5, "Fairytale_player_fwd.png");
            this.IL.Images.SetKeyName(6, "Fairytale_player_rev.png");
            this.IL.Images.SetKeyName(7, "Fairytale_player_start.png");
            this.IL.Images.SetKeyName(8, "Fairytale_player_rec.png");
            this.IL.Images.SetKeyName(9, "Fairytale_apply.png");
            this.IL.Images.SetKeyName(10, "Fairytale_history.png");
            this.IL.Images.SetKeyName(11, "Fairytale_button_cancel.png");
            // 
            // openVideoDlg
            // 
            this.openVideoDlg.Filter = "Video files|*.avi;*.mp4;*.mpg;*.mpeg;*.ts;*.mkv;*.mov|All files|*.*";
            this.openVideoDlg.Title = "Open Video File";
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlay.ImageIndex = 0;
            this.btnPlay.ImageList = this.IL;
            this.btnPlay.Location = new System.Drawing.Point(12, 367);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(48, 32);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonsToolTips.SetToolTip(this.btnPlay, "Play");
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // tb
            // 
            this.tb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb.LargeChange = 60000;
            this.tb.Location = new System.Drawing.Point(82, 316);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(420, 45);
            this.tb.SmallChange = 5000;
            this.tb.TabIndex = 4;
            this.tb.TickFrequency = 10000;
            this.tb.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tb.Scroll += new System.EventHandler(this.Tb_Scroll);
            // 
            // lYouTube
            // 
            this.lYouTube.AutoSize = true;
            this.lYouTube.Location = new System.Drawing.Point(282, 15);
            this.lYouTube.Name = "lYouTube";
            this.lYouTube.Size = new System.Drawing.Size(75, 13);
            this.lYouTube.TabIndex = 7;
            this.lYouTube.Text = "Youtube URL:";
            // 
            // eYoutubeURL
            // 
            this.eYoutubeURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eYoutubeURL.Location = new System.Drawing.Point(363, 12);
            this.eYoutubeURL.Name = "eYoutubeURL";
            this.eYoutubeURL.Size = new System.Drawing.Size(146, 20);
            this.eYoutubeURL.TabIndex = 8;
            // 
            // btnStreamYT
            // 
            this.btnStreamYT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStreamYT.Location = new System.Drawing.Point(515, 10);
            this.btnStreamYT.Name = "btnStreamYT";
            this.btnStreamYT.Size = new System.Drawing.Size(57, 23);
            this.btnStreamYT.TabIndex = 9;
            this.btnStreamYT.Text = "Stream";
            this.btnStreamYT.UseVisualStyleBackColor = true;
            this.btnStreamYT.Click += new System.EventHandler(this.BtnTestYT_Click);
            // 
            // cbStayOnTop
            // 
            this.cbStayOnTop.AutoSize = true;
            this.cbStayOnTop.Checked = true;
            this.cbStayOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStayOnTop.Location = new System.Drawing.Point(196, 14);
            this.cbStayOnTop.Name = "cbStayOnTop";
            this.cbStayOnTop.Size = new System.Drawing.Size(80, 17);
            this.cbStayOnTop.TabIndex = 10;
            this.cbStayOnTop.Text = "Stay on top";
            this.cbStayOnTop.UseVisualStyleBackColor = true;
            this.cbStayOnTop.CheckedChanged += new System.EventHandler(this.CbStayOnTop_CheckedChanged);
            // 
            // cbFollowPacketList
            // 
            this.cbFollowPacketList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFollowPacketList.AutoSize = true;
            this.cbFollowPacketList.Checked = true;
            this.cbFollowPacketList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFollowPacketList.Location = new System.Drawing.Point(145, 376);
            this.cbFollowPacketList.Name = "cbFollowPacketList";
            this.cbFollowPacketList.Size = new System.Drawing.Size(98, 17);
            this.cbFollowPacketList.TabIndex = 11;
            this.cbFollowPacketList.Text = "Follow Packets";
            this.cbFollowPacketList.UseVisualStyleBackColor = true;
            // 
            // btnSetOffset
            // 
            this.btnSetOffset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSetOffset.ImageIndex = 10;
            this.btnSetOffset.ImageList = this.IL;
            this.btnSetOffset.Location = new System.Drawing.Point(107, 10);
            this.btnSetOffset.Name = "btnSetOffset";
            this.btnSetOffset.Size = new System.Drawing.Size(83, 23);
            this.btnSetOffset.TabIndex = 12;
            this.btnSetOffset.Text = "Set Offset";
            this.btnSetOffset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSetOffset.UseVisualStyleBackColor = true;
            this.btnSetOffset.Click += new System.EventHandler(this.BtnSetOffset_Click);
            // 
            // lWarningLabel
            // 
            this.lWarningLabel.AutoSize = true;
            this.lWarningLabel.BackColor = System.Drawing.Color.Maroon;
            this.lWarningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lWarningLabel.ForeColor = System.Drawing.Color.Yellow;
            this.lWarningLabel.Location = new System.Drawing.Point(19, 48);
            this.lWarningLabel.Name = "lWarningLabel";
            this.lWarningLabel.Size = new System.Drawing.Size(122, 20);
            this.lWarningLabel.TabIndex = 13;
            this.lWarningLabel.Text = "Nothing Loaded";
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNextFrame.ImageIndex = 4;
            this.btnNextFrame.ImageList = this.IL;
            this.btnNextFrame.Location = new System.Drawing.Point(107, 367);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(32, 32);
            this.btnNextFrame.TabIndex = 14;
            this.btnNextFrame.Text = " ";
            this.buttonsToolTips.SetToolTip(this.btnNextFrame, "Forward one frame");
            this.btnNextFrame.UseVisualStyleBackColor = true;
            this.btnNextFrame.Click += new System.EventHandler(this.BtnNextFrame_Click);
            // 
            // btnPrevFrame
            // 
            this.btnPrevFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevFrame.ImageIndex = 6;
            this.btnPrevFrame.ImageList = this.IL;
            this.btnPrevFrame.Location = new System.Drawing.Point(69, 367);
            this.btnPrevFrame.Name = "btnPrevFrame";
            this.btnPrevFrame.Size = new System.Drawing.Size(32, 32);
            this.btnPrevFrame.TabIndex = 15;
            this.btnPrevFrame.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonsToolTips.SetToolTip(this.btnPrevFrame, "Rewind 1 second");
            this.btnPrevFrame.UseVisualStyleBackColor = true;
            this.btnPrevFrame.Click += new System.EventHandler(this.BtnPrevFrame_Click);
            // 
            // btnMute
            // 
            this.btnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMute.Font = new System.Drawing.Font("Webdings", 7F);
            this.btnMute.Location = new System.Drawing.Point(540, 367);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(32, 32);
            this.btnMute.TabIndex = 16;
            this.btnMute.Text = "X¯";
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.BtnMute_Click);
            // 
            // closeFixTimer
            // 
            this.closeFixTimer.Tick += new System.EventHandler(this.CloseFixTimer_Tick);
            // 
            // packetUpdateTimer
            // 
            this.packetUpdateTimer.Interval = 50;
            // 
            // tbPlaybackSpeed
            // 
            this.tbPlaybackSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPlaybackSpeed.Location = new System.Drawing.Point(435, 367);
            this.tbPlaybackSpeed.Maximum = 4;
            this.tbPlaybackSpeed.Minimum = -4;
            this.tbPlaybackSpeed.Name = "tbPlaybackSpeed";
            this.tbPlaybackSpeed.Size = new System.Drawing.Size(67, 45);
            this.tbPlaybackSpeed.TabIndex = 19;
            this.buttonsToolTips.SetToolTip(this.tbPlaybackSpeed, "Playback Speed");
            this.tbPlaybackSpeed.ValueChanged += new System.EventHandler(this.tbPlaybackSpeed_ValueChanged);
            // 
            // lVideoCurrentTime
            // 
            this.lVideoCurrentTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lVideoCurrentTime.Location = new System.Drawing.Point(12, 326);
            this.lVideoCurrentTime.Name = "lVideoCurrentTime";
            this.lVideoCurrentTime.Size = new System.Drawing.Size(64, 18);
            this.lVideoCurrentTime.TabIndex = 17;
            this.lVideoCurrentTime.Text = "Time";
            this.lVideoCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lVideoMaxTime
            // 
            this.lVideoMaxTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lVideoMaxTime.Location = new System.Drawing.Point(508, 326);
            this.lVideoMaxTime.Name = "lVideoMaxTime";
            this.lVideoMaxTime.Size = new System.Drawing.Size(64, 18);
            this.lVideoMaxTime.TabIndex = 18;
            this.lVideoMaxTime.Text = "Time";
            this.lVideoMaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lVideoSpeed
            // 
            this.lVideoSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lVideoSpeed.Location = new System.Drawing.Point(365, 374);
            this.lVideoSpeed.Name = "lVideoSpeed";
            this.lVideoSpeed.Size = new System.Drawing.Size(64, 18);
            this.lVideoSpeed.TabIndex = 20;
            this.lVideoSpeed.Text = "Speed";
            this.lVideoSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VideoLinkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this.lVideoSpeed);
            this.Controls.Add(this.tbPlaybackSpeed);
            this.Controls.Add(this.lVideoMaxTime);
            this.Controls.Add(this.lVideoCurrentTime);
            this.Controls.Add(this.btnMute);
            this.Controls.Add(this.btnPrevFrame);
            this.Controls.Add(this.btnNextFrame);
            this.Controls.Add(this.lWarningLabel);
            this.Controls.Add(this.btnSetOffset);
            this.Controls.Add(this.cbFollowPacketList);
            this.Controls.Add(this.cbStayOnTop);
            this.Controls.Add(this.btnStreamYT);
            this.Controls.Add(this.eYoutubeURL);
            this.Controls.Add(this.lYouTube);
            this.Controls.Add(this.tb);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.media);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VideoLinkForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VideoLinkForm";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.VideoLinkForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VideoLinkForm_FormClosing);
            this.Load += new System.EventHandler(this.VideoLinkForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.media)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPlaybackSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog openVideoDlg;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.TrackBar tb;
        public Vlc.DotNet.Forms.VlcControl media;
        private System.Windows.Forms.Label lYouTube;
        private System.Windows.Forms.TextBox eYoutubeURL;
        private System.Windows.Forms.Button btnStreamYT;
        private System.Windows.Forms.CheckBox cbStayOnTop;
        public System.Windows.Forms.CheckBox cbFollowPacketList;
        private System.Windows.Forms.Button btnSetOffset;
        private System.Windows.Forms.Label lWarningLabel;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Button btnPrevFrame;
        private System.Windows.Forms.Button btnMute;
        private System.Windows.Forms.Timer closeFixTimer;
        private System.Windows.Forms.ImageList IL;
        private System.Windows.Forms.Timer packetUpdateTimer;
        private System.Windows.Forms.ToolTip buttonsToolTips;
        private System.Windows.Forms.Label lVideoCurrentTime;
        private System.Windows.Forms.Label lVideoMaxTime;
        private System.Windows.Forms.TrackBar tbPlaybackSpeed;
        private System.Windows.Forms.Label lVideoSpeed;
    }
}