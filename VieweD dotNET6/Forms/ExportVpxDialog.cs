using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VieweD.Forms
{
    public partial class ExportVpxDialog : Form
    {
        public bool ExportBytes { get; private set; } = true;
        public bool ExportParsed { get; private set; } = true;

        public ExportVpxDialog()
        {
            InitializeComponent();
        }

        private void RbExportSettings_CheckedChanged(object sender, EventArgs e)
        {
            ExportBytes = RbBytes.Checked || RbBoth.Checked;
            ExportParsed = RbParsed.Checked || RbBoth.Checked;
        }
    }
}
