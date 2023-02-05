using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Engine.pcapraw
{
    public class PCapRawSettingsTab : EngineSettingsTab
    {
        public PCapRawSettingsTab(TabControl parent) : base(parent)
        {
            Panel.Controls.Add(new Label() { Parent = Panel, Text = "No additional settings for PCAP Raw", Left = 8, Top = 20, AutoSize = true});
        }
    }
}