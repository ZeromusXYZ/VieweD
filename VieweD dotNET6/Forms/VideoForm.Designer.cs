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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoForm));
            button1 = new Button();
            VideoViewPort = new LibVLCSharp.WinForms.VideoView();
            BtnPlay = new Button();
            ((System.ComponentModel.ISupportInitialize)VideoViewPort).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(12, 295);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // VideoViewPort
            // 
            VideoViewPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewPort.BackColor = Color.Black;
            VideoViewPort.Location = new Point(12, 12);
            VideoViewPort.MediaPlayer = null;
            VideoViewPort.Name = "VideoViewPort";
            VideoViewPort.Size = new Size(554, 277);
            VideoViewPort.TabIndex = 1;
            VideoViewPort.Text = "videoView1";
            // 
            // BtnPlay
            // 
            BtnPlay.Location = new Point(93, 295);
            BtnPlay.Name = "BtnPlay";
            BtnPlay.Size = new Size(75, 23);
            BtnPlay.TabIndex = 2;
            BtnPlay.Text = "Play";
            BtnPlay.UseVisualStyleBackColor = true;
            BtnPlay.Click += BtnPlay_Click;
            // 
            // VideoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(578, 330);
            Controls.Add(BtnPlay);
            Controls.Add(VideoViewPort);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "VideoForm";
            Text = "Video";
            Deactivate += VideoForm_Deactivate;
            FormClosed += VideoForm_FormClosed;
            ((System.ComponentModel.ISupportInitialize)VideoViewPort).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private LibVLCSharp.WinForms.VideoView VideoViewPort;
        private Button BtnPlay;
    }
}