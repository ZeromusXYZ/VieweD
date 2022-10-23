using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Engine.FFXI
{
    public class FFXISettingsTab : EngineSettingsTab
    {
        public TextBox eFFXIPath;
        public Label lFFXIFileCount;
        
        public FFXISettingsTab(TabControl parent) : base(parent)
        {
            Panel.Controls.Add(new Label() { Text = "Installation Folder", Left = 8, Top = 20, AutoSize = true});
            eFFXIPath = new TextBox() { Left = 8, Top = 44, Width = this.Width - 100, Anchor = (AnchorStyles.Left & AnchorStyles.Top & AnchorStyles.Right), Enabled = false };
            Panel.Controls.Add(eFFXIPath);
            lFFXIFileCount = new Label() { Left = eFFXIPath.Left + eFFXIPath.Width + 8, Top = eFFXIPath.Top , AutoSize = true, Anchor = (AnchorStyles.Top & AnchorStyles.Right) };
            Panel.Controls.Add(lFFXIFileCount);
        }
        
    }
}