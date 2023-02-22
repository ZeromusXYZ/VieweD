using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VieweD.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        public static LoadingForm? Instance { get; private set; } = null;
        private DateTime _showThresholdTime = DateTime.MinValue;
        private DateTime _unFreezeThresholdTime = DateTime.MinValue;

        public static void OnProgress(int position, int maxValue, string? title, Color? color)
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

            if (position == maxValue)
            {
                // Done loading, free the form
                Instance.Close();
                Instance.Dispose();
                Instance = null;
            }
        }
    }

    
}
