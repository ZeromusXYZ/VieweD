using System;
using System.Drawing;
using System.Windows.Forms;

namespace VieweD.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        public static LoadingForm? Instance { get; private set; }
        private DateTime _showThresholdTime = DateTime.MinValue;
        private DateTime _unFreezeThresholdTime = DateTime.MinValue;

        public static void OnProgress(int position, int maxValue, string? title, Color? color, bool forceShow = false)
        {
            if ((Instance == null) && (position <= 0) && (maxValue <= 0))
                return;

            if (Instance == null)
            {
                Instance = new LoadingForm();
                Instance._showThresholdTime = DateTime.UtcNow.AddMilliseconds(1500);
                Instance._unFreezeThresholdTime = DateTime.UtcNow.AddSeconds(7);
                if (title != null)
                    Instance.Text = title;
                if (color != null)
                    Instance.BackColor = (Color)color;
            }

            if (forceShow)
            {
                Instance._showThresholdTime = DateTime.UtcNow.AddSeconds(-2000);
                Instance._unFreezeThresholdTime = DateTime.UtcNow.AddSeconds(-2000);
            }

            if ((Instance.Visible == false) && (DateTime.UtcNow >= Instance._showThresholdTime))
            {
                Instance.Show();
                Instance.BringToFront();

                MainForm.Instance?.CenterMyForm(Instance);
            }

            Instance.Bar.Maximum = maxValue;
            Instance.Bar.Minimum = 0;
            Instance.Bar.Value = position;

            if (DateTime.UtcNow >= Instance._unFreezeThresholdTime)
            {
                Instance._unFreezeThresholdTime = DateTime.UtcNow.AddSeconds(7);
                Application.DoEvents();
            }

            if (position >= maxValue)
            {
                // Done loading, free the form
                Instance.Close();
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}
