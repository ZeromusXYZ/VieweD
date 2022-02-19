using System;
using System.Windows.Forms;

namespace VieweD.Engine.Common
{
    public class EngineSettingsTab : TabPage
    {
        protected GroupBox panel;

        public EngineSettingsTab(TabControl parent)
        {
            parent.TabPages.Add(this);
            Padding = new Padding(5);
            panel = new GroupBox();
            Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
            panel.Text = " Plugin Settings ";
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