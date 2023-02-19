using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using VieweD.Helpers.System;

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

        /// <summary>
        /// Called after the settings form has created all settings tabs
        /// </summary>
        public virtual void OnSettingsLoaded()
        {
            // Do save stuff
        }

        /// <summary>
        /// Called when the defaults button is pressed on the program settings form
        /// </summary>
        public virtual void OnSettingsResetDefaults()
        {
            // Re-initialize to default
        }

    }
}