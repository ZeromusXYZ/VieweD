using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using VieweD.engine.common;
using VieweD.Helpers.System;
using VieweD.Properties; // This needs to be made dynamic

namespace VieweD.Forms
{
    public partial class MainForm : Form
    {
        public static MainForm? Instance { get; private set; }
        public bool ShowDebugInfo { get; set; }
        private string AppTitle { get; set; } = string.Empty;

        private const string InfoGridHeader = "     |  0  1  2  3   4  5  6  7   8  9  A  B   C  D  E  F    | 0123456789ABCDEF\n" +
                                              "-----+----------------------------------------------------  -+------------------\n";

        public List<string> AllTempFiles { get; set; } = new ();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Instance = this;
            AppTitle = Text;
            MMVersion.Text = string.Format(Resources.MenuVersion, Assembly.GetExecutingAssembly().GetName().Version);
            // TCProjects.TabPages.Clear();

            // Load Settings
            PacketColors.UpdateColorsFromSettings();
            DgvParsed.Font = Settings.Default.GridViewFont;
            MiFieldDebug.Checked = ShowDebugInfo;

            _ = EngineManager.Instance;

            // Load the welcome text
            try
            {
                var rtfFile = Path.Combine(Application.StartupPath, "data", "welcome.rtf");
                RichTextWelcome.LoadFile(rtfFile);
            }
            catch
            {
                TPWelcome.Dispose();
            }

            // Update Status Bar
            UpdateStatusBar(null);
            UpdateStatusBarProgress(0, 0, null, null);
        }

        private void MMFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateStatusBar(ViewedProjectTab? projectTab)
        {
            if (projectTab != null)
            {
                // TODO: Insert loaded engine name(s)
                StatusBarEngineName.Text = projectTab.InputParser?.Name ?? Resources.StatusNoEngine;
                StatusBarProject.Text =
                    string.Format(Resources.StatusProjectFolder, projectTab.ProjectFolder);
                var t = AppTitle;
                if (projectTab.OpenedLogFile != string.Empty)
                {
                    t += " - " + projectTab.OpenedLogFile;
                    if (projectTab.IsDirty)
                        t += " *";
                }

                Text = t;
            }
            else
            {
                StatusBarEngineName.Text = Resources.StatusNoEngine;
                StatusBarProject.Text = Resources.StatusProjectFolderNotLoaded;
                Text = AppTitle;
            }
        }

        public void UpdateStatusBarProgress(int position, int maxValue, string? title, Color? color)
        {
            StatusBarProgressBar.Minimum = 0;
            StatusBarProgressBar.Maximum = maxValue;
            StatusBarProgressBar.Value = position;
            StatusBarProgressBar.Visible = position != maxValue;
            StatusBar.Update();
            LoadingForm.OnProgress(position, maxValue, title, color);
        }

        public void OnProjectDataChanged(ViewedProjectTab projectTab)
        {
            if (TCProjects.SelectedTab == projectTab)
                UpdateStatusBar(projectTab);
        }

        private void MMFileOpen_Click(object sender, EventArgs e)
        {
            if (openProjectFileDialog.ShowDialog() == DialogResult.OK)
                OpenFile(openProjectFileDialog.FileName);
        }

        /// <summary>
        /// Tries to open a project file, if aFileName is a log file instead, it will create a new project instead with suggested input reader and parser
        /// </summary>
        /// <param name="aFileName"></param>
        private void OpenFile(string aFileName)
        {
            var logFileName = aFileName;
            var rulesFileName = string.Empty;

            var project = new ViewedProjectTab();
            TCProjects.TabPages.Add(project);
            if (TCProjects.ImageList != null)
                project.ImageIndex = 1; // viewed icon
            TCProjects.SelectedTab = project;

            // First check if the opened file is a existing project file
            var expectedProjectFileName = string.Empty;
            string expectedProjectFolder;
            var ext = Path.GetExtension(logFileName).ToLower();

            // If the existing file is a project file, try using that
            if (File.Exists(logFileName) && (ext is ".pvd" or ".pvlv"))
            {
                expectedProjectFolder = Path.GetDirectoryName(logFileName) ?? string.Empty;
                expectedProjectFileName = logFileName;
            }
            else
            {
                // Check if there exists a project file by finding where the project folder would be,
                expectedProjectFolder = Helper.MakeProjectDirectoryFromLogFileName(logFileName);
            }

            if (Directory.Exists(expectedProjectFolder))
            {
                var projectFiles = Directory.GetFiles(expectedProjectFolder, "*.pvd", SearchOption.TopDirectoryOnly).ToList();
                projectFiles.AddRange(Directory.GetFiles(expectedProjectFolder, "*.pvlv", SearchOption.TopDirectoryOnly).ToList());

                // If there is exactly 1 project file found, use it
                if (projectFiles.Count == 1)
                    expectedProjectFileName = projectFiles[0];
            }
            else
            {
                expectedProjectFileName = string.Empty;
            }

            if ((expectedProjectFileName != string.Empty) && File.Exists(expectedProjectFileName))
            {
                // Try loading settings from the project file
                project.ProjectFile = expectedProjectFileName;
                var projectSetting = project.LoadProjectSettingsFile(expectedProjectFileName);
                if (projectSetting == null)
                {
                    MessageBox.Show(string.Format(Resources.UnableToOpenProject, expectedProjectFileName), Resources.ProjectReadingError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    project.CloseProject();
                    return;
                }

                logFileName = projectSetting.LogFile;
                project.InputReader = EngineManager.Instance.GetInputReaderByName(projectSetting.InputReaderName, project);

                project.InputParser = EngineManager.Instance.GetParserByName(projectSetting.ParserName, project);

                rulesFileName = projectSetting.RulesFile;
            }
            
            if (string.IsNullOrWhiteSpace(project.ProjectFile))
            {
                project.ProjectFile = string.IsNullOrWhiteSpace(expectedProjectFileName) ? 
                    Path.Combine(expectedProjectFolder, Path.GetFileNameWithoutExtension(expectedProjectFolder) + ".pvd") : 
                    expectedProjectFileName ;
            }

            project.Text = Helper.MakeTabName(expectedProjectFileName);

            project.InputReader ??= EngineManager.Instance.GetExpectedInputReaderForFile(project, logFileName);

            if (project.InputReader == null)
            {
                project.InputReader = InputReaderDialog.SelectInputReader(project, logFileName);
                if (project.InputReader != null)
                    project.InputReader.ParentProject = project;
            }

            if (project.InputReader == null)
            {
                MessageBox.Show(Resources.UnableToFindInputReader, Resources.InputReaderError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                project.CloseProject();
                return;
            }

            // Load the lookup data after we know our reader (so we can name packets before parsing)
            _ = project.DataLookup.LoadLookups(project.InputReader.DataFolder, true);

            if (project.InputReader.OpenFile(logFileName))
            {
                project.InputReader.ReadAllData();

                project.InputParser ??= ParserDialog.SelectParser(project, project.InputReader);

                if (project.InputParser == null)
                {
                    MessageBox.Show(Resources.UnableToFindParser, Resources.ParserError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    project.CloseProject();
                    return;
                }

                project.InputParser.ParentProject = project;

                if ((rulesFileName == string.Empty) && (!File.Exists(rulesFileName)))
                    rulesFileName = Path.Combine(Application.StartupPath, "data", project.InputReader.DataFolder, "rules", project.InputParser.DefaultRulesFile);

                project.InputParser.OpenRulesFile(rulesFileName);
                project.InputParser.ParseAllData(true);
                project.ReIndexLoadedPackets();
                project.PopulateListBox();
                project.IsDirty = false;
                project.OnProjectDataChanged();
            }
            else
            {
                MessageBox.Show(string.Format(Resources.UnableToOpenFile, openProjectFileDialog.FileName));
                project.CloseProject();
            }
        }

        private const string DepthSpacerVertical = "⁞";
        private const string DepthSpacerHorizontalSingle = "── ";
        private const string DepthSpacerHorizontalTop = "┌─ ";
        private const string DepthSpacerHorizontalMiddle = "├─ ";
        private const string DepthSpacerHorizontalBottom = "└─ ";

        /// <summary>
        /// Generates a string that can be used as a fake tree view for the Field Grid
        /// </summary>
        /// <param name="thisField"></param>
        /// <param name="previousField"></param>
        /// <param name="nextField"></param>
        /// <returns></returns>
        private static string GetNestedString(ParsedField thisField, ParsedField? previousField, ParsedField? nextField)
        {
            var res = string.Empty;

            if (thisField.NestingDepth > 0)
            {
                var prevFieldIsParent = (previousField != null) && (previousField.NestingDepth == thisField.NestingDepth - 1);
                var prevFieldIsSibling = (previousField != null) && (previousField.NestingDepth == thisField.NestingDepth);
                var nextFieldIsSibling = (nextField != null) && (nextField.NestingDepth == thisField.NestingDepth);

                if (prevFieldIsParent && nextFieldIsSibling)
                {
                    // new depth, with at least one next field
                    res = DepthSpacerHorizontalTop;
                }
                else if (prevFieldIsParent)
                {
                    // new depth, with no next field
                    res = DepthSpacerHorizontalSingle;
                }
                else if (prevFieldIsSibling && nextFieldIsSibling)
                {
                    // same depth, with at least one prev and one next field
                    res = DepthSpacerHorizontalMiddle;
                }
                else if (prevFieldIsSibling)
                {
                    // same depth, with at least one prev but no next field
                    res = DepthSpacerHorizontalBottom;
                }

                for (var n = 1; n < thisField.NestingDepth; n++)
                {
                    res = DepthSpacerVertical + res;
                }
            }
            return res;
        }

        /// <summary>
        /// Show packetData in the Field Grid and Raw Data View
        /// </summary>
        /// <param name="packetData"></param>
        public void ShowPacketData(BasePacketData? packetData)
        {
            if (packetData == null)
            {
                DgvParsed.Rows.Clear();
                DgvParsed.Tag = null;
                RichTextData.Clear();
                RichTextData.Tag = null;
                return;
            }

            #region FieldGridView
            if (MiFieldFields.Checked)
            {
                var oldFocus = DgvParsed.Focused;

                DgvParsed.SuspendLayout();
                DgvParsed.Tag = packetData;
                DgvParsed.Enabled = false;
                DgvParsed.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.R * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.G * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.B * 0.95));

                var y = 0;
                for (var i = 0; i < packetData.ParsedData.Count; i++)
                {
                    var parsedField = packetData.ParsedData[i];
                    var previousField = ((i - 1) >= 0) ? packetData.ParsedData[i - 1] : null;
                    var nextField = ((i + 1) < packetData.ParsedData.Count) ? packetData.ParsedData[i + 1] : null;

                    var nestString = GetNestedString(parsedField, previousField, nextField);

                    DataGridViewRow? row;
                    if (y >= DgvParsed.RowCount)
                    {
                        #pragma warning disable IDE0017 // Simplify object initialization
                        row = new DataGridViewRow();
                        #pragma warning restore IDE0017 // Simplify object initialization
                        row.Height = DgvParsed.Font.Height + 4;
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.DisplayedByteOffset });
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.FieldName });
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.FieldValue });
                        DgvParsed.Rows.Add(row);
                    }
                    else
                    {
                        row = DgvParsed.Rows[y];
                    }

                    row.Cells[0].Value = parsedField.DisplayedByteOffset;
                    row.Cells[0].Style.ForeColor = parsedField.FieldColor;

                    row.Cells[1].Value = nestString + parsedField.FieldName;
                    row.Cells[1].Style.ForeColor = parsedField.FieldColor;

                    row.Cells[2].Value = parsedField.FieldValue;
                    // row.Cells[2].Style.ForeColor = parsedField.FieldColor;

                    row.Selected = parsedField.IsSelected;

                    y++;
                }

                while (DgvParsed.RowCount > y)
                    DgvParsed.Rows.RemoveAt(DgvParsed.Rows.Count - 1);

                DgvParsed.Enabled = true;
                DgvParsed.ResumeLayout();

                if (oldFocus)
                    DgvParsed.Focus();
            }
            #endregion

            #region LocalVarView
            if (MIFieldLocalVars.Checked)
            {
                var oldFocus = DgvParsed.Focused;

                DgvParsed.SuspendLayout();
                DgvParsed.Rows.Clear();
                DgvParsed.Tag = packetData;
                DgvParsed.Enabled = false;
                DgvParsed.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.R * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.G * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.B * 0.95));

                var rule = packetData.ParentProject.InputParser?.Rules?.GetPacketRule(packetData);
                if (rule != null)
                {
                    rule.Build();
                    rule.RunRule(packetData);
                    foreach (var localVar in rule.LocalVars)
                    {
#pragma warning disable IDE0017 // Simplify object initialization
                        var row = new DataGridViewRow();
#pragma warning restore IDE0017 // Simplify object initialization
                        row.Height = DgvParsed.Font.Height + 4;
                        var baseCell = new DataGridViewTextBoxCell() { Value = "" };
                        var cellColor = baseCell.Style.ForeColor;

                        if ((localVar.Key == "p_type") || (localVar.Key == "p_size"))
                            cellColor = Color.DarkOrange;
                        else
                        if (NumberHelper.TryFieldParse(localVar.Value, out int _))
                            cellColor = Color.Blue;
                        else
                        if (NumberHelper.TryFieldParse(localVar.Value, out long _))
                            cellColor = Color.DarkBlue;
                        else
                        if (NumberHelper.TryFieldParse(localVar.Value, out long _))
                            cellColor = Color.Navy;
                        else if (double.TryParse(localVar.Value, out _))
                            cellColor = Color.Green;
                        else if ((localVar.Value == "NULL") || (string.IsNullOrWhiteSpace(localVar.Value)))
                            cellColor = Color.Red;
                        else
                            cellColor = Color.Fuchsia;

                        row.Cells.Add(baseCell);
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Key, Style = new DataGridViewCellStyle(baseCell.Style) {ForeColor = cellColor } });
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Value, Style = new DataGridViewCellStyle(baseCell.Style) { ForeColor = cellColor } });
                        DgvParsed.Rows.Add(row);
                    }
                }

                DgvParsed.Enabled = true;
                DgvParsed.ResumeLayout();

                if (oldFocus)
                    DgvParsed.Focus();
            }
            #endregion

            #region RawData
            PacketDataToRichText(packetData, RichTextData);
            #endregion
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        private static void PacketDataToRichText(BasePacketData packetData, RichTextBox rt)
        {
            rt.Tag = packetData;
            var rtInfo = rt;
            var rtf = string.Empty;
            List<Color> colorTable = new ();
            var lastForegroundColorIndex = -1;
            var lastBackgroundColorIndex = -1;
            var hasSelectedFields = packetData.HasSelectedFields(); // cache this

            int GetRtfColor(Color col)
            {
                var p = colorTable.IndexOf(col);
                if (p < 0)
                {
                    p = colorTable.Count;
                    colorTable.Add(col);
                }
                return p + 1;
            }

            void SetRtfColor(Color foregroundColor, Color backgroundColor)
            {
                var f = GetRtfColor(foregroundColor);
                var b = GetRtfColor(backgroundColor);
                //rtf += "\\cf" + f.ToString() + "\\highlight" + b.ToString();
                if ((f == lastForegroundColorIndex) && (b == lastBackgroundColorIndex))
                    return;
                if (f != lastForegroundColorIndex)
                    rtf += "\\cf" + f.ToString();
                if (b != lastBackgroundColorIndex)
                    rtf += "\\highlight" + b.ToString();
                rtf += " ";
                lastForegroundColorIndex = f;
                lastBackgroundColorIndex = b;
            }

            string BuildHeaderWithColorTable()
            {
                var rtfHead = string.Empty;

                rtfHead += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 " + Settings.Default.RawViewFont.Name + ";}}";
                rtfHead += "{\\colortbl;";

                foreach (var col in colorTable)
                    rtfHead += "\\red" + col.R.ToString() + "\\green" + col.G.ToString() + "\\blue" + col.B.ToString() + ";";
                
                rtfHead += "}";
                rtfHead += "\\viewkind4\\uc1\\pard\\cf1\\highlight2\\f0\\fs18 ";
                
                return rtfHead;
            }

            void SetColorBasic()
            {
                SetRtfColor(rtInfo.ForeColor, rtInfo.BackColor);
            }

            void SetColorGrid()
            {
                SetRtfColor(Color.DarkGray, rtInfo.BackColor);
            }

            void SetColorSelect()
            {
                SetRtfColor(Color.Yellow, Color.DarkBlue);
            }

            void SetColorNotSelect(int fieldIndex, bool forChars)
            {
                if ((hasSelectedFields) || forChars)
                    SetRtfColor(packetData.GetDataColor(fieldIndex), rtInfo.BackColor);
                else
                    SetRtfColor(rtInfo.BackColor, packetData.GetDataColor(fieldIndex));
            }

            void AddCharsOnTheRight(int startIndex)
            {
                SetColorGrid();
                rtf += "  | ";

                for (var c = 0; (c < 0x10) && ((startIndex + c) < packetData.ByteData.Count); c++)
                {
                    var thisByteIndex = startIndex + c;

                    var thisByteField = packetData.GetParsedFieldByByteIndex(thisByteIndex, true);
                    var n = thisByteField != null ? packetData.ParsedData.IndexOf(thisByteField) : 0;

                    if (thisByteField?.IsSelected ?? false)
                        SetColorSelect();
                    else
                        SetColorNotSelect(n, true);

                    char ch = (char)packetData.GetByteAtPos(startIndex + c);
                    if (ch == 92)
                        rtf += "\\\\";
                    else
                    if (ch == 64)
                        rtf += "\\@";
                    else
                    if (ch == 123)
                        rtf += "\\{";
                    else
                    if (ch == 125)
                        rtf += "\\}";
                    else
                    if ((ch < 32) || (ch >= 128))
                        rtf += '.';
                    else
                        rtf += ch.ToString();
                }
            }

            rtInfo.SuspendLayout();
            rtInfo.ForeColor = SystemColors.WindowText;
            rtInfo.BackColor = SystemColors.Window;
            // rtInfo.Clear();

            // The main grid header
            SetColorGrid();
            rtf += InfoGridHeader.Replace("\n", "\\par\n");


            var addCharCount = 0;
            var moveCursor = true;
            //var endCursor = -1;
            //ParsedField? lastParsedField;
            
            for (var i = 0; i < packetData.ByteData.Count; i += 0x10) // note this is +16, not +1
            {
                // Hex offset of the line
                SetColorGrid();
                rtf += i.ToString("X").PadLeft(4, ' ') + " | ";

                // Draw the line
                for (var i2 = 0; i2 < 0x10; i2++)
                {
                    var thisByteIndex = i + i2;
                    var thisByteField = packetData.GetParsedFieldByByteIndex(thisByteIndex, true);
                    var thisByteFieldIndex = thisByteField != null ? packetData.ParsedData.IndexOf(thisByteField) : -1;

                    // Still in range?
                    if ((thisByteIndex) < packetData.ByteData.Count)
                    {
                        if (hasSelectedFields)
                        {
                            // Change how things are displayed when there is at least one field selected
                            if (thisByteField?.IsSelected ?? false)
                            {
                                // Is selected field
                                SetColorSelect();
                                if (moveCursor)
                                {
                                    moveCursor = false;
                                    //endCursor = i + i2;
                                }
                            }
                            else
                            {
                                // we have non-selected field
                                SetColorNotSelect(thisByteFieldIndex, false);
                            }
                        }
                        else
                        {
                            // No fields selected
                            SetColorNotSelect(thisByteFieldIndex, false);
                        }
                        rtf += packetData.GetByteAtPos(i + i2).ToString("X2");
                        addCharCount++;
                    }
                    else
                    {
                        SetColorGrid();
                        rtf += "  ";
                    }

                    // Check if next byte is the same field, if no, change the color to the next field
                    var nextByteIndex = thisByteIndex + 1;
                    if ((thisByteIndex + 1) < packetData.ByteData.Count)
                    {
                        //var nextByte = packetData.ByteData[nextByteIndex];

                        var nextByteField = packetData.GetParsedFieldByByteIndex(nextByteIndex, true);
                        var nextByteFieldIndex = nextByteField != null ? packetData.ParsedData.IndexOf(nextByteField) : -1;

                        if (thisByteFieldIndex != nextByteFieldIndex)
                            SetColorBasic();
                    }
                    else
                    {
                        SetColorGrid();
                    }

                    // Add space after the Byte
                    rtf += " ";

                    // If the last byte of a quartet, add a extra space
                    if ((i2 % 0x4) == 0x3)
                    {
                        rtf += " ";
                    }

                    //lastParsedField = thisByteField;
                }

                if (addCharCount > 0)
                {
                    AddCharsOnTheRight(i);
                    addCharCount = 0;
                }
                rtf += "\\par\n";
                // rtInfo.AppendText("\r\n");
            }
            rtf += "}\n";
            rtInfo.WordWrap = false;
            rtInfo.Rtf = BuildHeaderWithColorTable() + rtf;
            rtInfo.Refresh();
            rtInfo.ResumeLayout();

            /*
            if ((endCursor >= 0) && (cbOriginalData.Checked == false))
            {
                var line = (endCursor / 16) + 2;
                var linePos = endCursor % 16;
                var rawPos = rtInfo.GetFirstCharIndexFromLine(line) + 7 + (linePos * 3);
                if (linePos > 3)
                    rawPos++;
                if (linePos > 7)
                    rawPos++;
                if (linePos > 11)
                    rawPos++;
                rtInfo.SelectionStart = rawPos;
                rtInfo.SelectionLength = 0;
            }
            */
        }

        private void DgvParsed_SelectionChanged(object sender, EventArgs e)
        {
            if ((sender is not DataGridView { Tag: BasePacketData packetData } dataGridView))
                return;

            // check if parsed field count actually matches the row count of the grid
            if (dataGridView.RowCount != packetData.ParsedData.Count)
                return;

            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                var dgvRow = dataGridView.Rows[i];
                packetData.ParsedData[i].IsSelected = dgvRow.Selected;
            }

            PacketDataToRichText(packetData, RichTextData);
        }

        private void TCProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatusBar(TCProjects.SelectedTab as ViewedProjectTab);
            if (TCProjects.SelectedTab is ViewedProjectTab project)
            {
                if (project.PacketsListBox.SelectedItem is BasePacketData packetData)
                {
                    ShowPacketData(packetData);
                }
                else
                {
                    ShowPacketData(null);
                }
            }
            else
            {
                ShowPacketData(null);
            }
        }

        private void MMProjectSettings_Click(object sender, EventArgs e)
        {
            (TCProjects.SelectedTab as ViewedProjectTab)?.OpenSettings();
        }

        private void MMProjectClose_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is ViewedProjectTab project)
            {
                //TCProjects.TabPages.Remove(project);
                project.CloseProject();
            }
            else
            {
                if (TCProjects.SelectedTab != null)
                    TCProjects.TabPages.Remove(TCProjects.SelectedTab);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Remove temp files when closing
            foreach (var tempFile in AllTempFiles)
            {
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch
                {
                    // Ignore
                }
            }
        }

        private void TPWelcomeBtnClose_Click(object sender, EventArgs e)
        {
            TPWelcome?.Dispose();
        }

        private void MMProjectSave_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is ViewedProjectTab project)
                project.SaveProjectSettingsFile(project.ProjectFile, project.ProjectFolder);
        }

        private void TCProjects_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender is not TabControl tabControl)
                return;
            // Source: https://social.technet.microsoft.com/wiki/contents/articles/50957.c-winform-tabcontrol-with-add-and-close-button.aspx
            // Adapted to using resources and without the add button
            try
            {
                var tabPage = tabControl.TabPages[e.Index];
                var tabRect = tabControl.GetTabRect(e.Index);
                //tabRect.Inflate(-2, -2);
                var closeImage = tabControl.ImageList?.Images[0] ?? Resources.close;
                var tabIcon = tabControl.ImageList?.Images[tabPage.ImageIndex];

                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    // for tabs at the top/bottom
                    e.Graphics.DrawImage(closeImage,
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
                    TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                        tabRect, tabPage.ForeColor, TextFormatFlags.Left);
                }
                else if (tabControl.Alignment == TabAlignment.Left)
                {
                    // for tabs to the left
                    e.Graphics.DrawImage(closeImage,
                        tabRect.Left + (tabRect.Width - closeImage.Width) / 2,
                        tabRect.Top);

                    if (tabIcon != null)
                    {
                        e.Graphics.DrawImage(tabIcon, tabRect.Left + (tabRect.Width / 2) - (tabIcon.Width / 2) , tabRect.Bottom - tabIcon.Height - 4);
                        tabRect = new Rectangle(tabRect.Left, tabRect.Top, tabRect.Width, tabRect.Height - tabIcon.Height - 4);
                    }

                    var tSize = e.Graphics.MeasureString(tabPage.Text, tabPage.Font);
                    e.Graphics.TranslateTransform(tabRect.Left + tabRect.Width, tabRect.Bottom);
                    e.Graphics.RotateTransform(-90);
                    var textBrush = new SolidBrush(tabPage.ForeColor);
                    e.Graphics.DrawString(tabPage.Text, tabPage.Font, textBrush, 0, -tabRect.Width - (tSize.Height / -4), StringFormat.GenericDefault);
                }
                else
                {
                    // If you want it on the right as well, you code it >.>
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void TCProjects_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Process MouseDown event only till (tabControl.TabPages.Count - 1) excluding the last TabPage
            if (sender is not TabControl tabControl)
                return;

            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                var tabRect = tabControl.GetTabRect(i);
                tabRect.Inflate(-2, -2);

                var inFront = tabControl.ImageList?.Images.Count > 1;
                var iconImage = inFront ? tabControl.ImageList?.Images[0] : Resources.close;

                if (iconImage == null)
                    return;

                Rectangle imageRect;
                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    if (inFront)
                    {
                        imageRect = new Rectangle(
                            tabRect.Left,
                            tabRect.Top + (tabRect.Height - iconImage.Height) / 2,
                            iconImage.Width,
                            iconImage.Height);
                    }
                    else
                    {
                        imageRect = new Rectangle(
                            (tabRect.Right - iconImage.Width),
                            tabRect.Top + (tabRect.Height - iconImage.Height) / 2,
                            iconImage.Width,
                            iconImage.Height);
                    }
                }
                else
                {
                    if (inFront)
                    {
                        imageRect = new Rectangle(
                            tabRect.Left + (tabRect.Width - iconImage.Width) / 2,
                            tabRect.Bottom - iconImage.Height - 2,
                            iconImage.Width,
                            iconImage.Height);
                    }
                    else
                    {
                        imageRect = new Rectangle(
                            tabRect.Left + (tabRect.Width - iconImage.Width) / 2,
                            tabRect.Top,
                            iconImage.Width,
                            iconImage.Height);
                    }

                }

                if (imageRect.Contains(e.Location))
                {
                    var tabText = tabControl.TabPages[i]?.Text ?? "Tab";
                    var thisTab = tabControl.TabPages[i];
                    if (MessageBox.Show(string.Format(Resources.CloseFile, tabText),
                            Resources.ConfirmClose,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (thisTab is ViewedProjectTab project)
                            project.CloseProject();
                        else
                            thisTab.Dispose();
                    }
                    return;
                }
            }
        }

        private void MMLinksOpen_Click(object sender, EventArgs e)
        {
            if ((sender is ToolStripMenuItem menuItem) && 
                (menuItem.Tag is string url) &&
                (string.IsNullOrWhiteSpace(url) == false))
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = url
                    };
                    Process.Start(psi);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format(Resources.FailedToOpenUrl, url, exception.Message), Resources.FailedToOpen, 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void MiFieldFields_Click(object sender, EventArgs e)
        {
            MiFieldFields.Checked = true;
            MIFieldLocalVars.Checked = false;
            DgvParsed.Rows.Clear();
            TCProjects_SelectedIndexChanged(TCProjects.SelectedTab, e);
        }

        private void MIFieldLocalVars_Click(object sender, EventArgs e)
        {
            MiFieldFields.Checked = false;
            MIFieldLocalVars.Checked = true;
            DgvParsed.Rows.Clear();
            TCProjects_SelectedIndexChanged(TCProjects.SelectedTab, e);
        }

        private void MiFieldView_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MiFieldDebug_Click(object sender, EventArgs e)
        {
            // Show debug view for this packet
            ShowDebugInfo = true;
            MiFieldFields.Checked = true;
            MIFieldLocalVars.Checked = false;
            DgvParsed.Rows.Clear();
            if ((TCProjects?.SelectedTab is ViewedProjectTab project) && (project.PacketsListBox.SelectedItem is BasePacketData packetData))
            {
                var rule = project.InputParser?.Rules?.GetPacketRule(packetData);
                if (rule != null)
                {
                    rule.Build();
                    rule.RunRule(packetData);
                    packetData.AddUnparsedFields();
                }
            }
            TCProjects_SelectedIndexChanged(TCProjects!.SelectedTab!, e);
            ShowDebugInfo = false;
        }

        public void CenterMyForm(Form form)
        {
            var centerX = Left + (Width / 2);
            var centerY = Top + (Height / 2);
            form.Left = centerX - (form.Width / 2);
            form.Top = centerY - (form.Height / 2);
        }
    }
}