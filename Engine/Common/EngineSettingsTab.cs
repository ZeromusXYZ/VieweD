using System.Windows.Forms;

namespace VieweD.Engine.Common
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class EngineSettingsTab : TabPage
    {
        protected GroupBox Panel;

        public EngineSettingsTab(TabControl parent)
        {
            parent.TabPages.Add(this);
            Padding = new Padding(5);
            Panel = new GroupBox();
            Controls.Add(Panel);
            Panel.Dock = DockStyle.Fill;
            Panel.Text = @" Plugin Settings ";
        }

        /// <summary>
        /// Called when the player presses OK/Save on the program settings dialog
        /// </summary>
        public virtual void OnSettingsTabSave()
        {
            // Do save stuff
        }
    }
}