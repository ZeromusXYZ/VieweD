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
    public partial class PacketTypeSelectForm : Form
    {
        public PacketTypeSelectForm()
        {
            InitializeComponent();
        }

        private void btnIN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnOUT_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
