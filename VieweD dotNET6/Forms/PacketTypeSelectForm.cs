namespace VieweD.Forms
{
    public partial class PacketTypeSelectForm : Form
    {
        public PacketTypeSelectForm()
        {
            InitializeComponent();
        }

        private void BtnIn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void BtnOut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
