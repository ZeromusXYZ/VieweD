using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VieweD
{
    public partial class LoadingForm : Form
    {
        private Form mainForm = null;
        public static LoadingForm thisLastForm = null;

        public LoadingForm()
        {
            InitializeComponent();
        }

        public LoadingForm(Form aParent)
        {
            InitializeComponent();
            mainForm = aParent;
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            thisLastForm = this;
            if (mainForm != null)
            {
                // CenterToParent();
                Location = new Point(mainForm.Location.X + ((mainForm.Size.Width - Size.Width) / 2), mainForm.Location.Y + ((mainForm.Size.Height - Size.Height) / 2));
            }
        }
    }
}
