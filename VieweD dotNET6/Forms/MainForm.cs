using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
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

        public List<string> AllTempFiles { get; set; } = new();

        public BasePacketData? CurrentPacketData;

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
            DgvParsed.Font = Settings.Default.FieldViewFont;
            MiFieldDebug.Checked = ShowDebugInfo;

            _ = EngineManager.Instance;

            if (EngineManager.PluginErrors.Count > 0)
                MessageBox.Show(string.Join('\n', EngineManager.PluginErrors), Resources.PluginCompileError, MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Load the welcome text
            try
            {
                var rtfFile = Path.Combine(Application.StartupPath, "data", "welcome.rtf");
                if (File.Exists(rtfFile))
                    RichTextWelcome.LoadFile(rtfFile);
                else
                    RichTextWelcome.Text = Resources.WelcomeFileNotFound;
            }
            catch
            {
                TPWelcome.Dispose();
            }

            // Update Status Bar
            UpdateStatusBar(null);
            UpdateStatusBarProgress(1, 1, null, null);
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
                if (projectTab.Settings.LogFile != string.Empty)
                {
                    t += " - " + projectTab.Settings.LogFile;
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
            // Ensure max and position values are within valid range
            if (maxValue < 1)
                maxValue = 1;
            position = Math.Clamp(position, 0, maxValue);
            StatusBarProgressBar.Minimum = 0;
            StatusBarProgressBar.Maximum = maxValue;
            StatusBarProgressBar.Value = position;
            StatusBarProgressBar.Visible = position != maxValue;
            StatusBar.Update();
            LoadingForm.OnProgress(position, maxValue, title, color);
            var waiting = position < maxValue;
            Application.UseWaitCursor = waiting;
            Cursor = waiting ? Cursors.WaitCursor : Cursors.Default;
        }

        public void OnProjectDataChanged(ViewedProjectTab projectTab)
        {
            if (TCProjects.SelectedTab == projectTab)
                UpdateStatusBar(projectTab);
        }

        private void MMFileOpen_Click(object sender, EventArgs e)
        {
            if (OpenProjectFileDialog.ShowDialog() == DialogResult.OK)
                OpenFile(OpenProjectFileDialog.FileName);
        }

        /// <summary>
        /// Tries to open a project file, if aFileName is a log file instead, it will create a new project instead with suggested input reader and parser
        /// </summary>
        /// <param name="aFileName"></param>
        private void OpenFile(string aFileName)
        {
            var logFileName = aFileName;
            var rulesFileName = string.Empty;
            var lastTimeOffset = TimeSpan.Zero;

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

            // Check if the expected project folder exists
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
                project.RequestUpdatedProjectFileName = true;
            }

            if ((expectedProjectFileName != string.Empty) && File.Exists(expectedProjectFileName))
            {
                // Try loading settings from the project file
                project.ProjectFile = expectedProjectFileName;
                var projectSetting = project.LoadProjectSettingsFile(expectedProjectFileName);
                if (projectSetting == null)
                {
                    MessageBox.Show(string.Format(Resources.UnableToOpenProject, expectedProjectFileName), Resources.ProjectReadingError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    project.CloseProject(true);
                    return;
                }

                project.Settings = projectSetting;

                logFileName = projectSetting.LogFile;
                //project.Settings.ProjectUrl = projectSetting.ProjectUrl;
                project.DecryptionKeyName = projectSetting.DecryptionName;
                project.InputReader = EngineManager.Instance.GetInputReaderByName(projectSetting.InputReader, project);

                project.InputParser = EngineManager.Instance.GetParserByName(projectSetting.Parser, project);

                rulesFileName = projectSetting.RulesFile;
                //project.Settings.Tags = projectSetting.Tags;
                //project.Settings.VideoSettings.VideoFile = projectSetting.VideoSettings.VideoFile;
                //project.Settings.VideoSettings.VideoUrl = projectSetting.VideoSettings.VideoUrl;
                //project.Settings.VideoSettings.VideoOffset = projectSetting.VideoSettings.VideoOffset;

                project.Filter.CopyFrom(projectSetting.Filter);
                project.SearchParameters.CopyFrom(projectSetting.Search);
                lastTimeOffset = projectSetting.LastTimeOffset;
            }
            else
            {
                project.RequestUpdatedProjectFileName = true;
            }

            if (string.IsNullOrWhiteSpace(project.ProjectFile))
            {
                project.ProjectFile = string.IsNullOrWhiteSpace(expectedProjectFileName) ?
                    Path.Combine(expectedProjectFolder, Path.GetFileNameWithoutExtension(expectedProjectFolder) + ".pvd") :
                    expectedProjectFileName;
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
                project.CloseProject(true);
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
                    project.CloseProject(true);
                    return;
                }

                project.InputParser.ParentProject = project;

                if ((rulesFileName == string.Empty) || (!File.Exists(rulesFileName)))
                    rulesFileName = RulesSelectDialog.SelectRulesFile(rulesFileName, project);

                if (!project.InputParser.OpenRulesFile(rulesFileName))
                {
                    MessageBox.Show(string.Format(Resources.MissingRulesFileForProject, OpenProjectFileDialog.FileName));
                    project.CloseProject(true);
                    return;
                }

                if (project.RequiresSubPacketCreation)
                    project.InputParser.ExpandSubPackets();
                project.InputParser.ParseAllData(true);
                project.ReIndexLoadedPackets();
                project.PopulateListBox();
                project.GotoPacketTimeOffset(lastTimeOffset);
                project.IsDirty = project.RequestUpdatedProjectFileName;
                project.OnProjectDataChanged();

                if ((Settings.Default.AutoLoadVideo > 0) && !string.IsNullOrWhiteSpace(project.Settings.VideoSettings.VideoFile) &&
                    (File.Exists(project.Settings.VideoSettings.VideoFile)))
                {
                    project.OpenVideoForm();
                }
            }
            else
            {
                MessageBox.Show(string.Format(Resources.UnableToOpenFile, OpenProjectFileDialog.FileName));
                project.CloseProject(true);
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
                CurrentPacketData = null;
                return;
            }

            CurrentPacketData = packetData;

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
                        row = new DataGridViewRow();
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.DisplayedByteOffset });
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.FieldName });
                        row.Cells.Add(new DataGridViewTextBoxCell() { Value = parsedField.FieldValue });
                        DgvParsed.Rows.Add(row);
                    }
                    else
                    {
                        row = DgvParsed.Rows[y];
                    }
                    row.Height = (int)Math.Ceiling(DgvParsed.Font.GetHeight()) + 4;
                    var foreColor = parsedField.FieldColor;
                    var defaultForeColor = (y % 2) == 0 ? DgvParsed.DefaultCellStyle.ForeColor : DgvParsed.AlternatingRowsDefaultCellStyle.ForeColor;
                    var backColor = (y % 2) == 0 ? DgvParsed.DefaultCellStyle.BackColor : DgvParsed.AlternatingRowsDefaultCellStyle.BackColor;


                    if (parsedField.MatchSearch(packetData.ParentProject.SearchParameters, packetData))
                    {
                        foreColor = Color.Black;
                        defaultForeColor = foreColor;
                        backColor = Color.Yellow;
                    }

                    row.Cells[0].Value = parsedField.DisplayedByteOffset;
                    row.Cells[0].Style.ForeColor = foreColor;
                    row.Cells[0].Style.BackColor = backColor;

                    row.Cells[1].Value = nestString + parsedField.FieldName;
                    row.Cells[1].Style.ForeColor = foreColor;
                    row.Cells[1].Style.BackColor = backColor;

                    row.Cells[2].Value = parsedField.FieldValue;
                    row.Cells[2].Style.ForeColor = defaultForeColor;
                    row.Cells[2].Style.BackColor = backColor;
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
                //DgvParsed.Rows.Clear();
                DgvParsed.Tag = packetData;
                DgvParsed.Enabled = false;
                DgvParsed.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.R * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.G * 0.95),
                    (int)Math.Round(DgvParsed.DefaultCellStyle.BackColor.B * 0.95));

                var y = 0;
                var rule = packetData.ParentProject.InputParser?.Rules?.GetPacketRule(packetData);
                if (rule != null)
                {
                    rule.Build();
                    rule.RunRule(packetData);
                    foreach (var localVar in rule.LocalVars)
                    {
                        DataGridViewRow? row;
                        if (y >= DgvParsed.RowCount)
                        {
                            row = new DataGridViewRow();
                            row.Cells.Add(new DataGridViewTextBoxCell() { Value = "" });
                            row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Key });
                            row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Value });
                            DgvParsed.Rows.Add(row);
                        }
                        else
                        {
                            row = DgvParsed.Rows[y];
                        }
                        row.Height = DgvParsed.Font.Height + 4;
                        var baseCell = new DataGridViewTextBoxCell() { Value = "" };
                        Color cellColor;

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

                        row.Cells[0] = baseCell;
                        row.Cells[1].Value = localVar.Key;
                        row.Cells[1].Style = new DataGridViewCellStyle(baseCell.Style) { ForeColor = cellColor };
                        row.Cells[2].Value = localVar.Value;
                        row.Cells[2].Style = new DataGridViewCellStyle(baseCell.Style) { ForeColor = cellColor };

                        // row.Cells.Add(baseCell);
                        // row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Key, Style = new DataGridViewCellStyle(baseCell.Style) { ForeColor = cellColor } });
                        // row.Cells.Add(new DataGridViewTextBoxCell() { Value = localVar.Value, Style = new DataGridViewCellStyle(baseCell.Style) { ForeColor = cellColor } });
                        // DgvParsed.Rows.Add(row);
                        y++;
                    }
                }

                while (DgvParsed.RowCount > y)
                    DgvParsed.Rows.RemoveAt(DgvParsed.Rows.Count - 1);

                DgvParsed.Enabled = true;
                DgvParsed.ResumeLayout();

                if (oldFocus)
                    DgvParsed.Focus();
            }
            #endregion

            var headerTextSize = TextRenderer.MeasureText(InfoGridHeader, Settings.Default.RawViewFont);
            LayoutRawAndSuggested.ColumnStyles[0].Width = headerTextSize.Width + 30;

            #region RawData
            PacketDataToRichText(packetData, RichTextData);
            #endregion

            SuggestionListBox.Items.Clear();
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        private static void PacketDataToRichText(BasePacketData packetData, RichTextBox richText)
        {
            richText.Tag = packetData;
            richText.Font = Settings.Default.RawViewFont;
            var fontSizeTag = (int)Math.Floor(Settings.Default.RawViewFont.Size * 2f);
            var rtf = string.Empty;
            List<Color> colorTable = new();
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
                    rtfHead += "\\red" + col.R + "\\green" + col.G + "\\blue" + col.B + ";";

                rtfHead += "}";
                rtfHead += "{\\*\\generator VieweD}";
                rtfHead += "\\viewkind4\\uc1\\pard\\cf1\\highlight2\\f0\\fs" + fontSizeTag + " ";

                return rtfHead;
            }

            void SetColorBasic()
            {
                SetRtfColor(richText.ForeColor, richText.BackColor);
            }

            void SetColorGrid()
            {
                SetRtfColor(Color.DarkGray, richText.BackColor);
            }

            void SetColorSelect()
            {
                SetRtfColor(Color.Yellow, Color.DarkBlue);
            }

            void SetColorHighlight()
            {
                SetRtfColor(Color.SaddleBrown, Color.Yellow);
            }

            void SetColorNotSelect(int fieldIndex, bool forChars)
            {
                if ((hasSelectedFields) || forChars)
                    SetRtfColor(BasePacketData.GetDataColor(fieldIndex), richText.BackColor);
                else
                    SetRtfColor(richText.BackColor, BasePacketData.GetDataColor(fieldIndex));
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
                    {
                        SetColorSelect();
                    }
                    else
                    {
                        SetColorNotSelect(n, true);
                    }

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

            richText.SuspendLayout();
            richText.ForeColor = SystemColors.WindowText;
            richText.BackColor = SystemColors.Window;
            // richText.Clear();

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
                        var isSearchHighlighted = false;
                        if (packetData.ParentProject.SearchParameters.HasSearchForData)
                        {
                            if (packetData.ParentProject.SearchParameters.SearchByByte &&
                                packetData.GetByteAtPos(thisByteIndex) ==
                                packetData.ParentProject.SearchParameters.SearchByte)
                            {
                                isSearchHighlighted = true;
                            }
                            else if (packetData.ParentProject.SearchParameters.SearchByUInt16 &&
                                     (
                                         (packetData.GetUInt16AtPos(thisByteIndex) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt16) ||
                                         (packetData.GetUInt16AtPos(thisByteIndex - 1) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt16)
                                     )
                                    )
                            {
                                isSearchHighlighted = true;
                            }
                            else if (packetData.ParentProject.SearchParameters.SearchByUInt24 &&
                                     (
                                         (packetData.GetUInt24AtPos(thisByteIndex) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt24) ||
                                         (packetData.GetUInt24AtPos(thisByteIndex - 1) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt24) ||
                                         (packetData.GetUInt24AtPos(thisByteIndex - 2) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt24)
                                     )
                                    )
                            {
                                isSearchHighlighted = true;
                            }
                            else if (packetData.ParentProject.SearchParameters.SearchByUInt32 &&
                                     (
                                         (packetData.GetUInt32AtPos(thisByteIndex) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt32) ||
                                         (packetData.GetUInt32AtPos(thisByteIndex - 1) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt32) ||
                                         (packetData.GetUInt32AtPos(thisByteIndex - 2) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt32) ||
                                         (packetData.GetUInt32AtPos(thisByteIndex - 3) ==
                                          packetData.ParentProject.SearchParameters.SearchUInt32)
                                     )
                                    )
                            {
                                isSearchHighlighted = true;
                            }
                        }

                        if (hasSelectedFields)
                        {
                            // Change how things are displayed when there is at least one field selected
                            if (thisByteField?.IsSelected ?? false)
                            {
                                // Is selected field
                                if (isSearchHighlighted)
                                {
                                    SetColorHighlight();
                                }
                                else
                                {
                                    SetColorSelect();
                                }

                                if (moveCursor)
                                {
                                    moveCursor = false;
                                    //endCursor = i + i2;
                                }
                            }
                            else
                            {
                                // we have non-selected field
                                if (isSearchHighlighted)
                                    SetColorHighlight();
                                else
                                    SetColorNotSelect(thisByteFieldIndex, false);
                            }
                        }
                        else
                        {
                            // No fields selected
                            if (isSearchHighlighted)
                                SetColorHighlight();
                            else
                                SetColorNotSelect(thisByteFieldIndex, false);
                        }

                        rtf += packetData.GetByteAtPos(thisByteIndex).ToString("X2");
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
                // richText.AppendText("\r\n");
            }
            rtf += "}\n";
            richText.WordWrap = false;
            richText.Rtf = BuildHeaderWithColorTable() + rtf;
            richText.Refresh();
            richText.ResumeLayout();


            /*
            if ((endCursor >= 0) && (cbOriginalData.Checked == false))
            {
                var line = (endCursor / 16) + 2;
                var linePos = endCursor % 16;
                var rawPos = richText.GetFirstCharIndexFromLine(line) + 7 + (linePos * 3);
                if (linePos > 3)
                    rawPos++;
                if (linePos > 7)
                    rawPos++;
                if (linePos > 11)
                    rawPos++;
                richText.SelectionStart = rawPos;
                richText.SelectionLength = 0;
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
                project.CloseProject(false);
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
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            SaveProject(project, ModifierKeys.HasFlag(Keys.Shift));
        }

        public void SaveProject(ViewedProjectTab project, bool forceSaveAs)
        {
            if (string.IsNullOrWhiteSpace(project.ProjectFile))
            {
                MessageBox.Show(Resources.CannotSaveVirtualProject,
                    Resources.SaveProject, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if ((project.RequestUpdatedProjectFileName) || (!File.Exists(project.ProjectFile)) || forceSaveAs)
            {
                if (project.RequestUpdatedProjectFileName)
                {
                    var dir = Helper.MakeProjectDirectoryFromLogFileName(project.Settings.LogFile);
                    project.ProjectFile = Path.Combine(dir, Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar)) + ".pvd");
                    SaveProjectFileDialog.FileName = Path.ChangeExtension(project.ProjectFile, ".pvd");
                    MessageBox.Show(
                        Resources.LoadedOldProjectTypeAndNeedSave,
                        Resources.SaveProject, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SaveProjectFileDialog.FileName = project.ProjectFile;
                }
                SaveProjectFileDialog.InitialDirectory = Path.GetDirectoryName(SaveProjectFileDialog.FileName);

                if (SaveProjectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    project.ProjectFile = SaveProjectFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show(Resources.SaveCancelled, Resources.SaveProject, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            project.SaveProjectSettingsFile(project.ProjectFile, project.ProjectFolder);
            UpdateStatusBar(project);
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
                        e.Graphics.DrawImage(tabIcon, tabRect.Left + (tabRect.Width / 2) - (tabIcon.Width / 2), tabRect.Bottom - tabIcon.Height - 4);
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
                            project.CloseProject(false);
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

        private void MMSearchEditFilter_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is ViewedProjectTab project)
            {
                using var filterForm = new FilterForm();
                CenterMyForm(filterForm);

                filterForm.ParentProject = project;
                filterForm.Filter.CopyFrom(project.Filter);
                filterForm.Filter.MarkAsDimmed = project.Filter.MarkAsDimmed;
                filterForm.LoadLocalFromFilter();

                var res = filterForm.ShowDialog();
                if (res is not (DialogResult.OK or DialogResult.Yes))
                    return;

                filterForm.SaveLocalToFilter();
                project.Filter.CopyFrom(filterForm.Filter);
                project.Filter.MarkAsDimmed = (res == DialogResult.Yes);
                project.IsDirty = true;

                project.PopulateListBox((project.PacketsListBox.SelectedItem as BasePacketData)?.ThisIndex ?? -1);
                project.CenterListBox();
            }
        }

        private void MMSearchFilterMenu_DropDownOpening(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem menu)
                return;

            // remove all but the first two items (reset + line)
            while (menu.DropDownItems.Count > 2)
                menu.DropDownItems.RemoveAt(menu.DropDownItems.Count - 1);

            // check if we got a project going
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            // Get filter files
            var filterFolder = Path.Combine(Application.StartupPath, "data", project.InputReader?.DataFolder ?? "base", "filter");
            if (!Directory.Exists(filterFolder))
                return;
            var menuTag = (menu.Tag as string) ?? "";
            var files = Directory.GetFiles(filterFolder, "*.pfl", SearchOption.AllDirectories);
            foreach (var fileName in files)
            {
                var newItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fileName))
                {
                    Tag = fileName,
                };

                if (menuTag == "ap")
                    newItem.Click += MMSearchFilterApplyFile_Click;
                if (menuTag == "hl")
                    newItem.Click += MMSearchFilterHighlightApplyFile_Click;
                menu.DropDownItems.Add(newItem);
            }
        }

        private void DoMenuFilter(object? sender, bool doHighLight)
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            var fileName = menuItem.Tag as string;
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                // Reset
                project.Filter.Clear();
            else
                // Filter with file
                project.Filter.LoadFromFile(fileName);

            project.Filter.MarkAsDimmed = doHighLight;
            project.PopulateListBox((project.PacketsListBox.SelectedItem as BasePacketData)?.ThisIndex ?? -1);
            project.CenterListBox();
        }

        private void MMSearchFilterApplyFile_Click(object? sender, EventArgs e)
        {
            DoMenuFilter(sender, false);
        }

        private void MMSearchFilterHighlightApplyFile_Click(object? sender, EventArgs e)
        {
            DoMenuFilter(sender, true);
        }

        private void MMFileSettings_Click(object sender, EventArgs e)
        {
            using var settings = new ProgramSettingsForm();
            if (settings.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.Save();
                PacketColors.UpdateColorsFromSettings();
                DgvParsed.Font = Settings.Default.FieldViewFont;

                // Apply packet list font to open tabs
                foreach (var page in TCProjects.TabPages)
                    if (page is ViewedProjectTab tp)
                        tp.ReloadPacketListColorsFromSettings();
            }
        }

        private void MMSearchFind_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            using var search = new SearchForm();
            search.ParentProject = project;
            search.SearchParameters.CopyFrom(project.SearchParameters);
            var res = search.ShowDialog();
            if (res == DialogResult.OK)
            {
                // Find Next
                project.SearchParameters.CopyFrom(search.SearchParameters);
                project.IsDirty = true;
                if (search.SearchParameters.SearchIncoming || search.SearchParameters.SearchOutgoing)
                    FindNext(project);
            }
            else
            if (res == DialogResult.Retry)
            {
                // New Tab
                project.SearchParameters.CopyFrom(search.SearchParameters);
                project.IsDirty = true;
                if (search.SearchParameters.SearchIncoming || search.SearchParameters.SearchOutgoing)
                    FindAsNewTab(project);
            }
        }

        private static void FindNext(ViewedProjectTab project)
        {
            if (project.PacketsListBox.Items.Count <= 0)
            {
                MessageBox.Show(Resources.NothingToSearch, Resources.Search, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var startIndex = project.PacketsListBox.SelectedIndex;
            if ((startIndex < 0) && (startIndex >= project.PacketsListBox.Items.Count))
                startIndex = -1;

            var i = startIndex + 1;
            for (var c = 0; c < project.PacketsListBox.Items.Count - 1; c++)
            {
                if (i >= project.PacketsListBox.Items.Count)
                    i = 0;
                if ((project.PacketsListBox.Items[i] is BasePacketData packetData) && packetData.MatchesSearch(project.SearchParameters))
                {
                    // Select index
                    project.PacketsListBox.SelectedIndex = i;
                    // Move to center
                    var iHeight = project.PacketsListBox.ItemHeight;
                    if (iHeight <= 0)
                        iHeight = 8;
                    var iCount = project.PacketsListBox.Size.Height / iHeight;
                    var tPos = i - (iCount / 2);
                    if (tPos < 0)
                        tPos = 0;
                    project.PacketsListBox.TopIndex = tPos;
                    project.PacketsListBox.Focus();
                    // We're done
                    return;
                }
                i++;
            }
            MessageBox.Show(Resources.NothingFound, Resources.Search, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FindAsNewTab(ViewedProjectTab project)
        {
            if ((project.PacketsListBox.Items.Count <= 0))
            {
                MessageBox.Show(Resources.NothingToSearch, Resources.SearchAsNewTab, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var newProject = new ViewedProjectTab
            {
                ImageIndex = 4,
                ProjectFile = string.Empty,
                Text = @"*" + project.Text,
            };
            newProject.InputReader = project.InputReader?.CreateNew(newProject);
            newProject.InputParser = project.InputParser?.CreateNew(newProject);

            var count = newProject.SearchFrom(project, project.SearchParameters);

            if (count <= 0)
            {
                MessageBox.Show(Resources.NothingFound, Resources.SearchAsNewTab, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                newProject.ReIndexLoadedPackets();
                newProject.PopulateListBox();
                TCProjects.TabPages.Add(newProject);
                TCProjects.SelectedTab = newProject;
                UpdateStatusBar(newProject);
            }
        }

        private void MMSearchFindNext_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if ((project.SearchParameters.SearchIncoming == false) &&
                (project.SearchParameters.SearchOutgoing == false))
            {
                MMSearchFind_Click(sender, e);
                return;
            }

            FindNext(project);
        }

        private void MMProjectVideo_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
            {
                var lonePlayer = new VideoForm();
                lonePlayer.Show();
                lonePlayer.BringToFront();
                return;
            }

            project.OpenVideoForm();
        }

        private void RichTextData_SelectionChanged(object sender, EventArgs e)
        {
            var lookupResults = new List<string>();

            void AddLabel(string val)
            {
                SuggestionListBox.Items.Add(val);
            }

            // Used for uint24 that doesn't support negative values
            void AddLabelUnsigned(string typeName, int hexWidth, ulong val, bool showBase)
            {
                var sVal = typeName + ": " + val.ToHex(hexWidth) + " - " + val;
                if (showBase)
                    AddLabel(sVal);

                foreach (var ll in CurrentPacketData.ParentProject.DataLookup.LookupLists)
                {
                    if (ll.Value.Data.TryGetValue(val, out var v))
                        if (v.Id > 0)
                        {
                            var s = ll.Key + "(0x" + val.ToString("X") + ") => " + v.Val;
                            if (!lookupResults.Contains(s))
                                lookupResults.Add(s);
                        }
                }
            }

            // Displays both signed and it's unsigned equivalent if they are different
            void AddLabelSigned(string typeName, int hexWidth, long val, bool showBase)
            {
                var h = val.ToString("X" + hexWidth);
                if (h.Length >= hexWidth)
                    h = h.Substring(h.Length - hexWidth, hexWidth);
                else
                    h = h.PadLeft(hexWidth, 'F');
                if (!long.TryParse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var positiveVal))
                    positiveVal = val;

                string sVal;
                if (positiveVal != val)
                    sVal = typeName + @": 0x" + h + @" - " + positiveVal + @" (" + val + @")";
                else
                    sVal = typeName + @": 0x" + h + @" - " + val;

                if (showBase)
                    AddLabel(sVal);

                // Only lookups for negative values on signed
                if (val < 0)
                {
                    foreach (var ll in CurrentPacketData.ParentProject.DataLookup.LookupLists)
                    {
                        // ReSharper disable once IntVariableOverflowInUncheckedContext
                        if (ll.Value.Data.TryGetValue((ulong)val, out var v))
                            if (v.Id > 0)
                            {
                                var s = ll.Key + "(" + val + ") => " + v.Val;
                                if (!lookupResults.Contains(s))
                                    lookupResults.Add(s);
                            }
                    }
                }

            }

            if (CurrentPacketData == null)
            {
                SuggestionListBox.Items.Clear();
                return;
            }

            var firstPos = RichTextData.SelectionStart;
            var line = RichTextData.GetLineFromCharIndex(firstPos);
            var lineFirst = RichTextData.GetFirstCharIndexFromLine(line);
            var linePos = firstPos - lineFirst;

            var rawPos = -1;
            var maxPos = 0;

            // Only calculate position if there is data
            if (CurrentPacketData.ByteData.Count > 0)
            {
                maxPos = CurrentPacketData.ByteData.Count;


                rawPos = ((line - 2) * 16);
                if ((linePos >= 6) && (linePos < 58)) // normal hex view location
                {
                    var off = linePos - 6;
                    var blockNumber = off / 12;
                    off -= blockNumber;
                    var p = off / 3;
                    if (p < 0)
                        p = 0;
                    if (linePos == 57)
                        p++;
                    rawPos += p;
                }
                else if ((linePos >= 63) && (linePos < 80)) // string hex view
                {
                    var off = linePos - 63;
                    rawPos += off;
                }

            }

            SuggestionListBox.Items.Clear();
            // var cursorPosText = "(" + line + "," + linePos + ")";
            string cursorPosText;
            if ((rawPos >= 0) && (rawPos < maxPos))
            {
                cursorPosText = "Cursor: " + rawPos.ToHex(2);// + " "+ cursorPosText;
                var sizeLeft = CurrentPacketData.ByteData.Count - rawPos;

                if (sizeLeft >= 2)
                {
                    //AddLabelUnsigned("uint16",4,CurrentPP.PD.GetUInt16AtPos(rawPos));
                    var n = CurrentPacketData.GetInt16AtPos(rawPos);
                    if (n != 0)
                        AddLabelSigned("int16 ", 4, n, PMSuggestIntergers.Checked);
                }
                if (sizeLeft >= 3)
                {
                    var n = CurrentPacketData.GetUInt24AtPos(rawPos);
                    if (n != 0)
                        AddLabelUnsigned("uint24", 6, n, PMSuggestIntergers.Checked);
                }
                if (sizeLeft >= 4)
                {
                    //AddLabelUnsigned("uint32", 8, CurrentPP.PD.GetUInt32AtPos(rawPos));
                    var i = CurrentPacketData.GetInt32AtPos(rawPos);
                    if (i != 0)
                        AddLabelSigned("int32 ", 8, i, PMSuggestIntergers.Checked);
                    var f = CurrentPacketData.GetFloatAtPos(rawPos);
                    if ((f != 0f) && PMSuggestFloats.Checked)
                        AddLabel("float : " + f.ToString(CultureInfo.InvariantCulture));
                    var dt = CurrentPacketData.GetTimeStampAtPos(rawPos);
                    if ((i != 0) && PMSuggestDateTime.Checked)
                        AddLabel("datetime: " + dt);
                }
                if (sizeLeft >= 8)
                {
                    //AddLabelUnsigned("uint64", 16, CurrentPP.PD.GetUInt64AtPos(rawPos));
                    var i = CurrentPacketData.GetInt64AtPos(rawPos);
                    if (i != 0)
                        AddLabelSigned("int64 ", 16, i, PMSuggestIntergers.Checked);
                    var d = CurrentPacketData.GetDoubleAtPos(rawPos);
                    if ((d != 0.0) && PMSuggestFloats.Checked)
                        AddLabel("double: " + d.ToString(CultureInfo.InvariantCulture));
                }

                foreach (var lr in lookupResults)
                {
                    if (lr.StartsWith("@"))
                    {
                        if (PMSuggestCustomLookup.Checked)
                            AddLabel(lr);
                    }
                    else if (PMSuggestLookup.Checked)
                        AddLabel(lr);
                }
            }
            else
            {
                // cursorPosText = "Pos: " + cursorPosText;
                cursorPosText = "N/A";
            }

            SuggestionListBox.Items.Insert(0, cursorPosText);
        }

        private void MMProjectGameData_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is ViewedProjectTab project)
            {
                project.GameView ??= new GameViewForm(project);
                project.GameView.Show();
                project.GameView.BringToFront();
            }
        }

        private void HandleCommandLine()
        {
            var cmdLine = CommandLineParser.SplitCommandLineIntoArguments(Environment.CommandLine, true);
            var c = 0;
            foreach (var arg in cmdLine)
            {
                c++;
                if (c == 1)
                    continue;
                if (File.Exists(arg))
                    OpenFile(arg);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            HandleCommandLine();
        }

        private void MMToolsExportCSV_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;
            using var exportDlg = new ExportCsvDialog();
            exportDlg.LoadFromProject(project);
            exportDlg.ShowDialog();
        }

        private void MMToolsEditTemplates_DropDownOpening(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if (sender is not ToolStripMenuItem menuItem)
                return;

            menuItem.DropDownItems.Clear();
            if ((project.InputParser == null) || (project.InputParser.Rules == null))
                return;

            foreach (var rulesTemplate in project.InputParser.Rules.Templates)
                menuItem.DropDownItems.Add(rulesTemplate.Key).Tag = rulesTemplate.Key;

            menuItem.DropDownItems.Add("-");
            menuItem.DropDownItems.Add("<create new>").Tag = "???";
        }

        private void MMToolsEditTemplates_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if (e.ClickedItem.Tag is not string templateName)
                return;

            if (templateName == "???")
            {
                templateName = InputBoxDialog.InputTextBox(string.Empty, "New Template", "Template name:");
            }

            if (templateName != string.Empty)
                project.EditTemplate(templateName);
        }

        private void MMToolExportData_DropDownOpening(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if (sender is not ToolStripMenuItem menuItem)
                return;

            menuItem.DropDownItems.Clear();
            if ((project.InputParser == null) || (project.InputParser.Rules == null))
                return;

            foreach (var exportDataTool in project.InputParser.Rules.ExportDataTools)
                menuItem.DropDownItems.Add(exportDataTool.Key).Tag = exportDataTool.Key;
        }

        private void MMToolExportData_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Clicked menu
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if (e.ClickedItem.Tag is not string exportName)
                return;

            if (exportName != string.Empty)
                project.RunExportDataTool(exportName);
        }

        private void MMToolsExportParsed_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            if (project.ExportParsedDataAsXml(Path.Combine(project.ProjectFolder, "export.vpx"), true))
                MessageBox.Show($"Saved Xml");
            else
                MessageBox.Show($"Failed Xml");

            /*
            if (project.ExportParsedDataAsCsv(Path.Combine(project.ProjectFolder, "export.vpc")))
                MessageBox.Show($"Saved CSV");
            else
                MessageBox.Show($"Failed CSV");
            */
        }

        private void MMProjectCopySelectedPackets_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            var list = new List<BasePacketData>();
            foreach (var selectedItem in project.PacketsListBox.SelectedItems)
            {
                if (selectedItem is BasePacketData packetData)
                    list.Add(packetData);
            }
            var s = project.ExportPacketsAsXmlString(list, true, true, true);
            list.Clear();
            ClipboardHelper.SetClipboard(s);
        }

        private void MMFileExportVpx_Click(object sender, EventArgs e)
        {
            if (TCProjects.SelectedTab is not ViewedProjectTab project)
                return;

            ExportSaveFileDialog.InitialDirectory = project.ProjectFolder;
            ExportSaveFileDialog.FileName = project.ProjectName + ".vpx";
            if (ExportSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!project.ExportParsedDataAsXml(ExportSaveFileDialog.FileName, true))
                    MessageBox.Show($"Failed to export!");
                else
                    SystemSounds.Asterisk.Play();
            }
        }

        private void MMFileImportVpx_Click(object sender, EventArgs e)
        {
            if (ImportParsedDataFileDialog.ShowDialog() != DialogResult.OK)
                return;

            var project = new ViewedProjectTab();
            if (project.ImportFromVpxFile(ImportParsedDataFileDialog.FileName))
            {
                project.Text = Helper.MakeTabName(ImportParsedDataFileDialog.FileName);
                TCProjects.TabPages.Add(project);
                if (TCProjects.ImageList != null)
                    project.ImageIndex = 1; // viewed icon
                TCProjects.SelectedTab = project;
            }
            else
            {
                project.CloseProject(true);
            }
        }
    }
}