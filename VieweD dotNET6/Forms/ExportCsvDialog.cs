using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class ExportCsvDialog : Form
    {
        public List<string> AllFieldNames { get; set; } = new();
        private ViewedProjectTab? ParentProject { get; set; }
        public ExportCsvDialog()
        {
            InitializeComponent();
        }

        public void LoadFromProject(ViewedProjectTab project)
        {
            ParentProject = project;
            AllFieldNames.Clear();
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
                if (i == 1)
                    doShow = false;
                else if (fieldName.StartsWith("??_"))
                    doShow = false;
                SelectedFieldsListBox.SetItemChecked(i, doShow);
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
                SelectedFieldsListBox.SetItemChecked(i, (i != 1));
        }

        private void BtnUnselectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFieldsListBox.Items.Count; i++)
                SelectedFieldsListBox.SetItemChecked(i, i == 0); // always keep the Packet ID selected
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
                if ((i == 0) || fieldName.Contains('?'))
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

                //selectedFieldNames.CopyTo(csv.HeaderRecord, 0);

                foreach (var pItem in ParentProject.PacketsListBox.Items)
                {
                    if (pItem is not BasePacketData data)
                        continue;

                    for (var i = 0; i < selectedFieldNames.Count; i++)
                    {
                        var field = data.GetFirstParsedFieldByName(selectedFieldNames[i] as string ?? string.Empty);
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
        }
    }
}
