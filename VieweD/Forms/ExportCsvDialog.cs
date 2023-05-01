using System;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CsvHelper.Configuration;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class ExportCsvDialog : Form
    {
        public List<string> AllFieldNames { get; set; } = new();
        private ViewedProjectTab? ParentProject { get; set; }
        private List<string> DefaultIgnoredFields { get; set; } = new();
        public ExportCsvDialog()
        {
            InitializeComponent();
        }

        public void LoadFromProject(ViewedProjectTab project)
        {
            ParentProject = project;
            AllFieldNames.Clear();
            DefaultIgnoredFields.Clear();
            if (ParentProject == null)
                return;
            foreach (var item in ParentProject.PacketsListBox.Items)
            {
                if (item is not BasePacketData data)
                    continue;

                foreach (var parsedField in data.ParsedData)
                {
                    // ignore "Unparsed" text
                    if ((parsedField.FieldName.Equals(Resources.UnParsedFieldName, StringComparison.InvariantCultureIgnoreCase))
                        &&
                        (string.IsNullOrWhiteSpace(parsedField.DisplayedByteOffset)))
                        continue;

                    // skip rule comments
                    if (parsedField.FieldName.Equals("#comment", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    if (!parsedField.HasValue)
                        DefaultIgnoredFields.Add(parsedField.FieldName);
                    
                    // Generate field names list
                    if (!AllFieldNames.Contains(parsedField.FieldName))
                        AllFieldNames.Add(parsedField.FieldName);
                }
            }

            SelectedFieldsListBox.Items.Clear();
            foreach (var fieldName in AllFieldNames)
                SelectedFieldsListBox.Items.Add(fieldName);

            AutoSelectFields();
        }

        public void AutoSelectFields()
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
            {
                var fieldName = (SelectedFieldsListBox.Items[i] as string) ?? string.Empty;
                var doShow = true;
                if (fieldName.StartsWith("??_"))
                    doShow = false;
                else
                if (DefaultIgnoredFields.Contains(fieldName))
                    doShow = false;
                SelectedFieldsListBox.SetItemChecked(i, doShow);
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
                SelectedFieldsListBox.SetItemChecked(i,true);
        }

        private void BtnUnselectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
                SelectedFieldsListBox.SetItemChecked(i, false);
        }

        private void BtnDefaultSelection_Click(object sender, EventArgs e)
        {
            AutoSelectFields();
        }

        private void BtnSelectQuestionMarks_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
            {
                var fieldName = (SelectedFieldsListBox.Items[i] as string) ?? string.Empty;
                if (fieldName.Contains('?'))
                    SelectedFieldsListBox.SetItemChecked(i, true);
            }

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (DelimiterSemicolon.Checked)
                Settings.Default.CSVExportDelimiter = ";";
            else if (DelimiterTab.Checked)
                Settings.Default.CSVExportDelimiter = "\t";
            else
                Settings.Default.CSVExportDelimiter = ",";
            Settings.Default.CSVExportIncludeTime = CbIncludeTimeStamp.Checked;
            Settings.Default.Save();

            DialogResult = DialogResult.OK;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (ParentProject == null)
                return;

            if (ExportFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var selectedFieldNames = SelectedFieldsListBox.CheckedItems;
                var useDelimiter = ",";
                if (DelimiterSemicolon.Checked)
                    useDelimiter = ";";
                if (DelimiterTab.Checked)
                    useDelimiter = "\t";

                using var writer = new StreamWriter(ExportFileDialog.FileName);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    NewLine = Environment.NewLine,
                    Delimiter = useDelimiter,
                    InjectionOptions = InjectionOptions.Escape,
                };
                using var csv = new CsvWriter(writer, config);

                // Write headers
                if (CbIncludeTimeStamp.Checked)
                    csv.WriteField(@"TimeStamp");
                foreach (var fieldName in selectedFieldNames)
                    csv.WriteField(fieldName);
                csv.NextRecord();

                foreach (var pItem in ParentProject.PacketsListBox.Items)
                {
                    if (pItem is not BasePacketData data)
                        continue;

                    if (CbIncludeTimeStamp.Checked)
                        csv.WriteField(data.TimeStamp.ToString(ParentProject.TimeStampFormat, CultureInfo.InvariantCulture));

                    foreach (var fieldName in selectedFieldNames)
                    {
                        var field = data.GetFirstParsedFieldByName(fieldName as string ?? string.Empty);
                        if (field == null)
                        {
                            csv.WriteField(string.Empty);
                            continue;
                        }

                        csv.WriteField(field.FieldValue);
                    }

                    csv.NextRecord();
                }

                csv.Flush();
                MessageBox.Show($"Exported as {ExportFileDialog.FileName}", Resources.ExportDataTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Resources.SaveCancelled, MessageBoxButtons.OK);
            }

        }

        private void ExportCsvDialog_Load(object sender, EventArgs e)
        {
            DelimiterComma.Checked = (Settings.Default.CSVExportDelimiter == ",");
            DelimiterSemicolon.Checked = (Settings.Default.CSVExportDelimiter == ";");
            DelimiterTab.Checked = (Settings.Default.CSVExportDelimiter == "\t");
            CbIncludeTimeStamp.Checked = Settings.Default.CSVExportIncludeTime;
        }
    }
}
