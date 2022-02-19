using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Plugins
{
    public class TestSettingsTab : EngineSettingsTab
    {
        public TestSettingsTab(TabControl parent) : base(parent)
        {
            panel.Controls.Add(new Label() { Parent = panel, Text = "No additional settings for Test Plugin", Left = 8, Top = 20, AutoSize = true});
        }
        
    }
}