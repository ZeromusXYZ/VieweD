using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Plugins
{
    public class TestEngine : EngineBase
    {
        public override string EngineId => "Test";
        public override string EngineName => "Test Plugin";

        public override EngineSettingsTab CreateSettingsTab(TabControl parent)
        {
            var newTab = new TestSettingsTab(parent) { Text = EngineName };

            return newTab;
        }
    }
}
