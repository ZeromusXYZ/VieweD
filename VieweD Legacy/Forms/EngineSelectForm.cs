using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD.Forms
{
    public partial class EngineSelectForm : Form
    {
        private class ComboBoxFileListValues
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string EngineId { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string DisplayName { get; set; }
        }

        private string SelectedEngineId { get; set; }

        private EngineSelectForm()
        {
            InitializeComponent();
        }

        private void FillForm(bool appendOnly)
        {
            SelectedEngineId = null;
            var list = new List<ComboBoxFileListValues>();
            foreach (var engine in Engines.AllEngines)
            {
                if (appendOnly && !engine.CanAppend(null))
                    continue;
                list.Add(new ComboBoxFileListValues { EngineId = engine.EngineId, DisplayName = engine.EngineName });
            }

            cbEnginesList.DisplayMember = "DisplayName";
            cbEnginesList.ValueMember = "EngineId";
            cbEnginesList.DataSource = list;
            cbEnginesList.SelectedIndex = cbEnginesList.Items.Count - 1;
        }

        /// <summary>
        /// Opens a form to select a engine from all registered engines
        /// </summary>
        /// <param name="appendOnly">If true, only lists engines that support appending of data</param>
        /// <returns>Returns selected engine, or null if nothing was selected</returns>
        public static EngineBase SelectEngine(bool appendOnly)
        {
            using (var thisForm = new EngineSelectForm())
            {
                thisForm.FillForm(appendOnly);

                if (thisForm.cbEnginesList.Items.Count <= 0)
                {
                    MessageBox.Show(@"No engines installed");
                    return null;
                }
                if (thisForm.cbEnginesList.Items.Count == 1)
                    thisForm.SelectedEngineId = (thisForm.cbEnginesList.Items[0] as ComboBoxFileListValues)?.EngineId ?? "null" ; 

                if ((thisForm.cbEnginesList.Items.Count == 1) || (thisForm.ShowDialog() == DialogResult.OK))
                {
                    foreach (var engine in Engines.AllEngines)
                    {
                        if (engine.EngineId == thisForm.SelectedEngineId)
                        {
                            var selected = Activator.CreateInstance(engine.GetType()) as EngineBase;
                            selected?.Init();
                            return selected;
                        }
                    }
                }
            }
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedEngineId = cbEnginesList?.SelectedValue.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
