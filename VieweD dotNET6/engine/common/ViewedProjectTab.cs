using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using VieweD.Forms;
using VieweD.Helpers.System;
using VieweD.engine.serialize;
using VieweD.Properties;
using System.Media;
using System.Windows.Forms;
using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Ionic.BZip2;
using System.Net;

namespace VieweD.engine.common;

public class ViewedProjectTab : TabPage
{
    private string _projectFile;

    // Visual Components
    internal FlickerFreeListBox PacketsListBox { get; } = new();

    // Project Settings
    /// <summary>
    /// Directory of this project (derived from ProjectFile
    /// </summary>
    public string ProjectFolder =>
        string.IsNullOrWhiteSpace(ProjectFile) ? "" : Path.GetDirectoryName(ProjectFile) ?? "";

    /// <summary>
    /// Name of this project (derived from ProjectFile)
    /// </summary>
    public string ProjectName => string.IsNullOrWhiteSpace(ProjectFile)
        ? "VieweD Project"
        : Path.GetFileNameWithoutExtension(ProjectFile);

    public string ProjectFile
    {
        get => _projectFile;
        set
        {
            if (value != _projectFile)
                IsDirty = true;
            _projectFile = value;
            Settings.ProjectFile = value;
        }
    }

    /// <summary>
    /// Project Settings
    /// </summary>
    public ProjectSettings Settings { get; set; }

    /// <summary>
    /// List of Data Packets
    /// </summary>
    public List<BasePacketData> LoadedPacketList { get; set; }

    /// <summary>
    /// Returns if project data has unsaved changes
    /// </summary>
    public bool IsDirty { get; set; }

    /// <summary>
    /// Current SyncId of selected Packet
    /// </summary>
    private int CurrentSyncId { get; set; }

    public BaseInputReader? InputReader { get; set; }
    public BaseParser? InputParser { get; set; }
    public DataLookups DataLookup { get; set; }
    public string TimeStampFormat { internal get; set; }
    public bool RequiresSubPacketCreation { get; set; }

    /// <summary>
    /// Mapping to use to convert a target port number to a StreamId used by the parsers
    /// </summary>
    public Dictionary<ushort, (byte, string, string)> PortToStreamIdMapping { get; } = new();

    public PacketListFilter Filter { get; set; }

    #region popup_menu_items

    // Popup Menu Controls
    private ContextMenuStrip PmPl { get; }
    private ToolStripMenuItem PmPlShowPacketName { get; }
    private ToolStripSeparator PmPls1 { get; }
    private ToolStripMenuItem PmPlShowOnly { get; }
    private ToolStripMenuItem PmPlHideThis { get; }
    private ToolStripSeparator PmPls2 { get; }
    private ToolStripMenuItem PmPlShowOutOnly { get; }
    private ToolStripMenuItem PmPlShowInOnly { get; }
    private ToolStripSeparator PmPls3 { get; }
    private ToolStripMenuItem PmPlResetFilters { get; }
    private ToolStripSeparator PmPls4 { get; }
    private ToolStripMenuItem PmPlEditParser { get; }
    private ToolStripMenuItem PmPlExportPacket { get; }

    #endregion

    /// <summary>
    /// Set to true if a old format project file was loaded
    /// </summary>
    public bool RequestUpdatedProjectFileName { get; set; }

    public SearchParameters SearchParameters { get; set; }
    public List<string> AllFieldNames { get; set; }
    public VideoForm? Video { get; set; }
    public RulesEditorForm? CurrentEditor { get; set; }
    public GameViewForm? GameView { get; set; }
    public string DecryptionKeyName { get; set; } = string.Empty;

    public ViewedProjectTab()
    {
        TimeStampFormat = "HH:mm:ss";
        _projectFile = string.Empty;
        Settings = new ProjectSettings();

        Filter = new PacketListFilter();
        Filter.Clear();

        Settings.Tags = new List<string>();
        Settings.Tags.Clear();

        AllFieldNames = new List<string>();
        SearchParameters = new SearchParameters();

        #region CreatePacketListBox

        // Set ListBox Position
        PacketsListBox.Parent = this;
        PacketsListBox.Location = new Point(0, 0);
        PacketsListBox.Size = new Size(this.Width, this.Height);
        PacketsListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        ReloadPacketListColorsFromSettings();
        PacketsListBox.DrawMode = DrawMode.OwnerDrawFixed;

        PacketsListBox.DrawItem += PacketsListBox_DrawItem;
        PacketsListBox.SelectedIndexChanged += PacketsListBox_SelectedIndexChanged;
        PacketsListBox.DoubleClick += PacketsListBox_DoubleClick;

        #endregion

        #region CreatePopupMenu

        // Create Popup Menu
        PmPl = new ContextMenuStrip();
        PmPl.Opening += PmPL_Opening;

        PacketsListBox.ContextMenuStrip = PmPl;

        PmPlShowPacketName = new ToolStripMenuItem("Show packet name");
        PmPl.Items.Add(PmPlShowPacketName);

        PmPls1 = new ToolStripSeparator();
        PmPl.Items.Add(PmPls1);

        PmPlShowOnly = new ToolStripMenuItem("Show this type only");
        PmPlShowOnly.Click += PmPLShowOnly_Click;
        PmPlShowOnly.Image = Resources.view_visible_16;
        PmPl.Items.Add(PmPlShowOnly);

        PmPlHideThis = new ToolStripMenuItem("Hide this type");
        PmPlHideThis.Click += PmPLHideThis_Click;
        PmPlHideThis.Image = Resources.view_hidden_16;
        PmPl.Items.Add(PmPlHideThis);

        PmPls2 = new ToolStripSeparator();
        PmPl.Items.Add(PmPls2);

        PmPlShowOutOnly = new ToolStripMenuItem("Show only Outgoing");
        PmPlShowOutOnly.Click += PmPLShowOutgoingOnly_Click;
        PmPlShowOutOnly.Image = Resources.mini_out_ticon;
        PmPl.Items.Add(PmPlShowOutOnly);

        PmPlShowInOnly = new ToolStripMenuItem("Show only Incoming");
        PmPlShowInOnly.Click += PmPLShowIncomingOnly_Click;
        PmPlShowInOnly.Image = Resources.mini_in_ticon;
        PmPl.Items.Add(PmPlShowInOnly);

        PmPls3 = new ToolStripSeparator();
        PmPl.Items.Add(PmPls3);

        PmPlResetFilters = new ToolStripMenuItem("Reset all filters");
        PmPlResetFilters.Click += PmPLResetFilter_Click;
        PmPlResetFilters.Image = Resources.view_close_16;
        PmPl.Items.Add(PmPlResetFilters);

        PmPls4 = new ToolStripSeparator();
        PmPl.Items.Add(PmPls4);

        PmPlEditParser = new ToolStripMenuItem("Edit this parser");
        PmPlEditParser.Click += PmPLEditParser_Click;
        PmPlEditParser.Image = Resources.document_properties_16;
        PmPl.Items.Add(PmPlEditParser);

        PmPlExportPacket = new ToolStripMenuItem("Export Packet");
        PmPlExportPacket.Click += PmPLExport_Click;
        PmPlExportPacket.Image = Resources.document_save_as_16;
        PmPl.Items.Add(PmPlExportPacket);

        PacketsListBox.ContextMenuStrip = PmPl;

        #endregion

        // Initialize Empty Project
        ProjectFile = "project.pvd"; // Path.Combine(Directory.GetCurrentDirectory(), "project.pvd");
        Settings.LogFile = string.Empty;
        CurrentSyncId = -1;
        LoadedPacketList = new List<BasePacketData>();
        RequiresSubPacketCreation = false;

        // Load Static Lookups
        DataLookup = new DataLookups();

        ReIndexLoadedPackets();
        PopulateListBox();
        IsDirty = false;
        OnProjectDataChanged();
    }

    private void PmPLExport_Click(object? sender, EventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void PmPLEditParser_Click(object? sender, EventArgs e)
    {
        EditCurrentPacketRule();
    }

    private void PmPLResetFilter_Click(object? sender, EventArgs e)
    {
        var packetData = GetSelectedPacket();

        // Clear filter
        Filter.MarkAsDimmed = false;
        Filter.Clear();

        PopulateListBox(packetData?.ThisIndex ?? -1);
        CenterListBox();
    }

    internal void CenterListBox()
    {
        // Move to center
        var iHeight = PacketsListBox.ItemHeight;
        if (iHeight <= 0)
            iHeight = 8;
        var iCount = PacketsListBox.Size.Height / iHeight;
        var tPos = PacketsListBox.SelectedIndex - (iCount / 2);
        if (tPos < 0)
            tPos = 0;
        PacketsListBox.TopIndex = tPos;
    }

    private void PmPLShowIncomingOnly_Click(object? sender, EventArgs e)
    {
        var hasShift = ModifierKeys.HasFlag(Keys.Shift);
        var packetData = GetSelectedPacket();
        if (packetData == null)
            return;

        if ((Filter.FilterInType == FilterType.AllowNone) || (Filter.FilterInType == FilterType.HidePackets))
        {
            Filter.FilterInType = FilterType.Off;
        }

        Filter.FilterOutType = FilterType.AllowNone;

        Filter.MarkAsDimmed = hasShift;
        IsDirty = true;
        PopulateListBox(packetData.ThisIndex);
        CenterListBox();
    }

    private void PmPLShowOutgoingOnly_Click(object? sender, EventArgs e)
    {
        var hasShift = ModifierKeys.HasFlag(Keys.Shift);
        var packetData = GetSelectedPacket();
        if (packetData == null)
            return;

        if ((Filter.FilterOutType == FilterType.AllowNone) || (Filter.FilterOutType == FilterType.HidePackets))
        {
            Filter.FilterOutType = FilterType.Off;
        }

        Filter.FilterInType = FilterType.AllowNone;

        Filter.MarkAsDimmed = hasShift;
        IsDirty = true;
        PopulateListBox(packetData.ThisIndex);
        CenterListBox();
    }

    private void PmPLHideThis_Click(object? sender, EventArgs e)
    {
        var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
        var packetData = GetSelectedPacket();
        if (packetData == null)
            return;

        var packetKey =
            new PacketFilterListEntry(packetData.PacketId, packetData.CompressionLevel, packetData.StreamId);

        switch (packetData.PacketDataDirection)
        {
            case PacketDataDirection.Incoming:
                if (Filter.FilterInType != FilterType.HidePackets)
                {
                    Filter.FilterInType = FilterType.HidePackets;
                    Filter.FilterInList.Clear();
                }

                Filter.FilterInList.Add(packetKey);
                break;
            case PacketDataDirection.Outgoing:
                if (Filter.FilterOutType != FilterType.HidePackets)
                {
                    Filter.FilterOutType = FilterType.HidePackets;
                    Filter.FilterOutList.Clear();
                }

                Filter.FilterOutList.Add(packetKey);
                break;
            default:
                return;
        }

        Filter.MarkAsDimmed = hasShift;
        IsDirty = true;
        PopulateListBox(packetData.ThisIndex);
        CenterListBox();
    }

    private void PmPLShowOnly_Click(object? sender, EventArgs e)
    {
        var hasShift = ModifierKeys.HasFlag(Keys.Shift);
        var packetData = GetSelectedPacket();
        if (packetData == null)
            return;

        var packetKey =
            new PacketFilterListEntry(packetData.PacketId, packetData.CompressionLevel, packetData.StreamId);

        switch (packetData.PacketDataDirection)
        {
            case PacketDataDirection.Incoming:
                Filter.Clear();
                Filter.FilterInType = FilterType.ShowPackets;
                Filter.FilterInList.Add(packetKey);
                Filter.FilterOutType = FilterType.AllowNone;
                break;
            case PacketDataDirection.Outgoing:
                Filter.Clear();
                Filter.FilterOutType = FilterType.ShowPackets;
                Filter.FilterOutList.Add(packetKey);
                Filter.FilterInType = FilterType.AllowNone;
                break;
            default:
                return;
        }

        Filter.MarkAsDimmed = hasShift;
        IsDirty = true;
        PopulateListBox(packetData.ThisIndex);
        CenterListBox();
    }

    private void PmPL_Opening(object? sender, CancelEventArgs e)
    {
        var packetData = GetSelectedPacket();
        PmPlShowPacketName.Enabled = packetData != null;
        PmPls1.Enabled = packetData != null;
        PmPlShowOnly.Enabled = packetData != null;
        PmPlHideThis.Enabled = packetData != null;
        PmPls2.Enabled = packetData != null;
        PmPlShowOutOnly.Enabled = packetData != null;
        PmPlShowInOnly.Enabled = packetData != null;
        PmPls3.Enabled = packetData != null;
        PmPlResetFilters.Enabled = (Filter.FilterInType != FilterType.Off) || (Filter.FilterOutType != FilterType.Off);
        PmPls4.Enabled = packetData != null;
        PmPlEditParser.Enabled = packetData != null;
        PmPlExportPacket.Enabled = false; // packetData != null;

        if (packetData == null)
        {
            PmPlShowPacketName.Text = Resources.NothingSelected;
            //e.Cancel = true;
            //return;
        }

        if (packetData != null)
        {
            var thisRule = packetData.ParentProject.InputParser?.Rules?.GetPacketRule(packetData);
            if (thisRule != null)
            {
                // var lookupName = PacketFilterListEntry.AsString(thisRule.PacketId, thisRule.Level, thisRule.StreamId);
                var lookupName = thisRule.PacketId.ToHex(3) +
                                 (InputParser?.PacketCompressionLevelMaximum > 0 ? " L" + thisRule.Level : "") +
                                 (PortToStreamIdMapping.Count > 1 ? " " + GetStreamIdName(thisRule.StreamId) : "");
                PmPlShowPacketName.Text = lookupName + @" - " + thisRule.Name; // lookupKey.ToString("X8");
                PmPlEditParser.Tag = thisRule;
                if (packetData.PacketDataDirection != PacketDataDirection.Unknown)
                {
                    PmPlEditParser.Text = string.Format(Resources.PopupEditRule, lookupName, thisRule.Name);
                    PmPlEditParser.Visible = true;
                }
                else
                {
                    PmPlEditParser.Text = Resources.PopupUnknownDirection;
                    PmPlEditParser.Visible = false;
                }
            }
            else
            {
                PmPlShowPacketName.Text = Resources.PopupNoRuleAssigned;

                var packetFilterEntry = new PacketFilterListEntry(packetData.PacketId, 0, 0);
                if (packetData.PacketDataDirection != PacketDataDirection.Unknown)
                {
                    PmPlEditParser.Text = string.Format(Resources.PopupCreateRule, packetFilterEntry);
                    PmPlEditParser.Visible = true;
                    PmPlEditParser.Tag = packetFilterEntry;
                }
                else
                {
                    PmPlEditParser.Tag = null;
                    PmPlEditParser.Text = Resources.PopupNothingToEdit;
                    PmPlEditParser.Visible = false;
                }
            }

            PmPlShowOnly.Enabled = (packetData.PacketDataDirection != PacketDataDirection.Unknown);
            PmPlHideThis.Enabled = (packetData.PacketDataDirection != PacketDataDirection.Unknown);
        }
    }

    private BasePacketData? GetSelectedPacket()
    {
        if (PacketsListBox.SelectedItem is BasePacketData data)
            return data;
        return null;
    }

    private void PacketsListBox_DoubleClick(object? sender, EventArgs e)
    {
        EditCurrentPacketRule();
    }

    private void EditCurrentPacketRule()
    {
        if (CurrentEditor != null)
        {
            CurrentEditor.BringToFront();
            CurrentEditor.Focus();
            SystemSounds.Exclamation.Play();
            return;
        }

        var packetData = GetSelectedPacket();
        if (packetData != null)
        {
            var rule = InputParser?.Rules?.GetPacketRule(packetData);

            if (rule == null)
            {
                if (MessageBox.Show(Resources.NoRuleAttachedToThisPacketCreateNewRule, Resources.EditRulesFile,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) != DialogResult.Yes)
                    return;

                var newName = $"0x{packetData.PacketId:X3} - {packetData.PacketDataDirection}";
                var expectedName = string.Empty;
                if (packetData.PacketDataDirection == PacketDataDirection.Incoming)
                    expectedName = DataLookup.NLU(DataLookups.LuPacketIn).GetValue(packetData.PacketId, string.Empty);
                if (packetData.PacketDataDirection == PacketDataDirection.Outgoing)
                    expectedName = DataLookup.NLU(DataLookups.LuPacketOut).GetValue(packetData.PacketId, string.Empty);

                if (expectedName != string.Empty)
                    newName = expectedName;

                rule = InputParser?.Rules?.CreateNewUserPacketRule(packetData.PacketDataDirection,
                    new PacketFilterListEntry(packetData.PacketId, packetData.CompressionLevel, packetData.StreamId),
                    newName) ?? null;
            }

            //rule?.Build();
            //rule?.RunRule(data);
            if (rule != null)
            {
                CurrentEditor = RulesEditorForm.OpenRuleEditor(rule, packetData);
                CurrentEditor.ParentProject = this;
            }
            else
            {
                MessageBox.Show(Resources.NoRuleLinkedToThisPacket, Resources.NoRuleFound,
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
    }

    public void EditTemplate(string templateName)
    {
        if (CurrentEditor != null)
        {
            CurrentEditor.BringToFront();
            CurrentEditor.Focus();
            SystemSounds.Exclamation.Play();
            return;
        }

        var reader = InputParser?.Rules;

        if (reader == null)
            return;

        if (!reader.Templates.TryGetValue(templateName, out var templateNode))
        {
            var templatesRoot = reader.XmlDoc?.SelectSingleNode("/root/templates");
            if (templatesRoot == null)
                return;

            templateNode = XmlHelper.CreateNewXmlElementNode(templatesRoot, "template");
            // templateNode = reader.XmlDoc?.CreateNode(XmlNodeType.Element, "template", null);
            if (templateNode == null)
                return;

            XmlHelper.SetAttribute(templateNode, "name", templateName);
            // templatesRoot.AppendChild(templateNode);
            reader.Templates.Add(templateName, templateNode);
            // Update the AllTemplates node
            reader.AllTemplates = reader.XmlDoc?.SelectNodes("/root/templates/template");
        }

        CurrentEditor = RulesEditorForm.OpenTemplateEditor(templateNode, this);
    }

    public void OnProjectDataChanged()
    {
        MainForm.Instance?.OnProjectDataChanged(this);
    }

    /// <summary>
    /// Recalculates all ThisIndex and TimeOffset values for all items in the LoadedPacketList
    /// </summary>
    public void ReIndexLoadedPackets()
    {
        var startTime = LoadedPacketList.Count > 0 ? LoadedPacketList[0].TimeStamp : DateTime.MinValue;
        var lastSameOffset = TimeSpan.Zero;
        var lastSameTimeIndex = 0;
        var sameTimeCount = -1;
        lock (LoadedPacketList)
        {
            for (var i = 0; i < LoadedPacketList.Count; i++)
            {
                LoadedPacketList[i].ThisIndex = i;
                LoadedPacketList[i].OffsetFromStart = LoadedPacketList[i].TimeStamp - startTime;
                LoadedPacketList[i].VirtualOffsetFromStart = LoadedPacketList[i].OffsetFromStart;

                if ((LoadedPacketList[i].OffsetFromStart == lastSameOffset) && (i < LoadedPacketList.Count - 1))
                {
                    sameTimeCount++;
                }
                else
                {
                    if ((sameTimeCount > 0) && (i > lastSameTimeIndex + 1))
                    {
                        sameTimeCount++;
                        var timeSpanDelta = (LoadedPacketList[i].OffsetFromStart - lastSameOffset) / sameTimeCount;
                        // Only update virtual times if there is a noticeable time difference
                        if (timeSpanDelta.TotalMilliseconds > 1)
                        {
                            for (var n = 1; n <= sameTimeCount - 1; n++)
                            {
                                LoadedPacketList[lastSameTimeIndex + n].VirtualOffsetFromStart =
                                    lastSameOffset + (timeSpanDelta * n);
                            }
                        }
                    }

                    lastSameOffset = LoadedPacketList[i].OffsetFromStart;
                    lastSameTimeIndex = i;
                    sameTimeCount = 0;
                }
            }
        }
    }

    /// <summary>
    /// Updates the PacketListBox with the currently visible PacketData
    /// </summary>
    public void PopulateListBox(int selectIndex = -1)
    {
        OnPopulateProgressUpdate(0, LoadedPacketList.Count);
        lock (LoadedPacketList)
        {
            PacketsListBox.BeginUpdate();
            PacketsListBox.Items.Clear();
            for (var i = 0; i < LoadedPacketList.Count; i++)
            {
                var packetData = LoadedPacketList[i];

                packetData.ApplyFilter(Filter);
                // packetData.ApplySearch(SearchParameters);

                if (packetData.IsVisible == false)
                    continue;

                PacketsListBox.Items.Add(packetData);

                if ((i % 50) == 0)
                    OnPopulateProgressUpdate(i, LoadedPacketList.Count);

                // Select the specified index if needed (or the closest after it)
                if ((selectIndex < 0) || (selectIndex > i))
                    continue;

                selectIndex = -1;
                PacketsListBox.SelectedItem = packetData;
            }

            PacketsListBox.EndUpdate();
            OnPopulateProgressUpdate(1, 1);
        }
    }

    /// <summary>
    /// SelectedIndexChanged event for the PacketListBox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PacketsListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        PacketsListBox.Invalidate();
        if (PacketsListBox.Items[PacketsListBox.SelectedIndex] is BasePacketData pd)
        {
            CurrentSyncId = pd.SyncId;
            MainForm.Instance?.ShowPacketData(pd);
            Video?.UpdateVideoPositionFromProject(pd.VirtualOffsetFromStart + Settings.VideoSettings.VideoOffset);
        }
        else
        {
            MainForm.Instance?.ShowPacketData(null);
        }
    }

    /// <summary>
    /// DrawItem event for the PacketListBox
    /// This is gives the Packet List it's flair
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PacketsListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (sender is not ListBox lb)
            return;

        // Check if we are drawing inside out list bounds, if not, just draw the background
        if ((e.Index < 0) || (e.Index >= PacketsListBox.Items.Count))
        {
            e.DrawBackground();
            return;
        }

        // Check if the attached object of the list box item to be drawn is a BasePacketData object
        // If not, just draw the background
        if (PacketsListBox.Items[e.Index] is not BasePacketData pd)
        {
            // Draw the background of the ListBox control for each item that isn't a valid one.
            e.DrawBackground();
            return;
        }

        var barOn = CurrentSyncId == pd.SyncId;
        var isSelected = (e.Index == lb.SelectedIndex);
        Color textCol;
        Color backCol;
        Color barCol;

        // Determine the color of the brush to draw each item based on the index of the item to draw.
        var packetDirectionForColors = pd.PacketDataDirection;
        // If a packet is marked as invalid, always draw it the same as a unknown packet color
        if (pd.MarkedAsInvalid)
            packetDirectionForColors = PacketDataDirection.Unknown;

        switch (packetDirectionForColors)
        {
            case PacketDataDirection.Incoming:
                textCol = PacketColors.ColorFontIn;
                if (isSelected)
                {
                    backCol = PacketColors.ColorSelectIn;
                    textCol = PacketColors.ColorSelectedFontIn;
                }
                else if (barOn)
                    backCol = PacketColors.ColorSyncIn;
                else
                    backCol = PacketColors.ColorBackIn;

                barCol = PacketColors.ColorBarIn;
                break;
            case PacketDataDirection.Outgoing:
                textCol = PacketColors.ColorFontOut;
                if (isSelected)
                {
                    backCol = PacketColors.ColorSelectOut;
                    textCol = PacketColors.ColorSelectedFontOut;
                }
                else if (barOn)
                    backCol = PacketColors.ColorSyncOut;
                else
                    backCol = PacketColors.ColorBackOut;

                barCol = PacketColors.ColorBarOut;
                break;
            case PacketDataDirection.Unknown:
            default:
                textCol = PacketColors.ColorFontUnknown;
                if (isSelected)
                {
                    backCol = PacketColors.ColorSelectUnknown;
                    textCol = PacketColors.ColorSelectedFontUnknown;
                }
                else if (barOn)
                    backCol = PacketColors.ColorSyncUnknown;
                else
                    backCol = PacketColors.ColorBackUnknown;

                barCol = PacketColors.ColorBarUnknown;
                break;
        }

        // Change colors if the packet is "dimmed"
        if (pd.MarkedAsDimmed)
        {
            // Grab the background color's luminance to determine if it's considered a light or a dark color
            Colorspace.RgbToHls(backCol, out _, out var l, out _);
            var isDark = l < 0.5;

            // We use two different ways to adjust the background color based on if it's a light or dark color
            if (isDark)
                backCol = Color.FromArgb(backCol.A, backCol.R / 2, backCol.G / 2, backCol.B / 2);
            else
                backCol = Color.FromArgb(
                    backCol.A,
                    backCol.R > 128 ? 255 : backCol.R * 2,
                    backCol.G > 128 ? 255 : backCol.G * 2,
                    backCol.B > 128 ? 255 : backCol.B * 2);

            // Text color is always a RGB average of the foreground and background colors
            textCol = Color.FromArgb(textCol.A, (backCol.R + textCol.R) / 2, (backCol.G + textCol.G) / 2,
                (backCol.B + textCol.B) / 2);

            // NOTE: We currently don't change the bar color as it looks unusually weird if in the same sync
            // barCol = Color.FromArgb(barCol.A, barCol.R / 4, barCol.G / 4, barCol.B / 4);
        }

        // Create the brushes we need based on the selected colors
        Brush textBrush = new SolidBrush(textCol);
        Brush backBrush = new SolidBrush(backCol);
        Brush barBrush = new SolidBrush(barCol);

        // Draw the background of the ListBox control for each item.
        e.Graphics.FillRectangle(backBrush, e.Bounds);

        // Header text
        var s = lb.Items[e.Index].ToString();

        var icon1 = new Rectangle(e.Bounds.Left,
            e.Bounds.Top + ((e.Bounds.Height - Resources.mini_unk_icon.Height) / 2), Resources.mini_unk_icon.Width,
            Resources.mini_unk_icon.Height);
        var icon2 = icon1 with { X = icon1.Left + icon1.Width, Y = icon1.Top };

        // Draw the video strip icon if this packet is within a video segment
        if (IsInVideoTimeRange(pd.VirtualOffsetFromStart))
        {
            e.Graphics.DrawImage(Resources.mini_video_icon, icon2);
        }

        // Change the text location based on the number of icons that we display
        Rectangle textBounds;
        if (HasVideoAttached())
        {
            textBounds = new Rectangle(e.Bounds.Left + (icon1.Width * 2), e.Bounds.Top,
                e.Bounds.Width - (icon1.Width * 2), e.Bounds.Height);
        }
        else
        {
            textBounds = new Rectangle(e.Bounds.Left + (icon1.Width), e.Bounds.Top, e.Bounds.Width - (icon1.Width),
                e.Bounds.Height);
        }

        switch (PacketColors.PacketListStyle)
        {
            case 1:
                // Colored arrows
                if (pd.PacketDataDirection == PacketDataDirection.Incoming)
                {
                    e.Graphics.DrawImage(Resources.mini_in_icon, icon1);
                }
                else if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
                {
                    e.Graphics.DrawImage(Resources.mini_out_icon, icon1);
                }
                else
                {
                    e.Graphics.DrawImage(Resources.mini_unk_icon, icon1);
                }

                break;
            case 2:
                // transparent arrows
                if (pd.PacketDataDirection == PacketDataDirection.Incoming)
                {
                    e.Graphics.DrawImage(Resources.mini_in_ticon, icon1);
                }
                else if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
                {
                    e.Graphics.DrawImage(Resources.mini_out_ticon, icon1);
                }
                else
                {
                    e.Graphics.DrawImage(Resources.mini_unk_ticon, icon1);
                }

                break;
            default:
                // No icons, just text
                textBounds = e.Bounds;
                if (pd.PacketDataDirection == PacketDataDirection.Incoming)
                {
                    s = "<= " + s;
                }
                else if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
                {
                    s = "=> " + s;
                }
                else
                {
                    s = "?? " + s;
                }

                break;
        }

        // Draw the current item text based on the current Font and the custom brush settings.
        e.Graphics.DrawString(s, e.Font!, textBrush, textBounds, StringFormat.GenericDefault);

        // Check if the Sync Bar needs to be drawn
        if (barOn)
        {
            var barSize = 8;
            // If it's also the selected item, double the width
            if (isSelected)
                barSize = 16;
            e.Graphics.FillRectangle(barBrush,
                new Rectangle(e.Bounds.Right - barSize, e.Bounds.Top, barSize, e.Bounds.Height));
        }

        // If the ListBox has focus, draw a focus rectangle around the selected item.
        e.DrawFocusRectangle();
    }

    /// <summary>
    /// Returns true if given timeOffset falls instead a video section
    /// </summary>
    /// <param name="timeOffset"></param>
    /// <returns></returns>
    private bool IsInVideoTimeRange(TimeSpan timeOffset)
    {
        if ((Video == null) || (Video.MPlayer == null))
            return false;

        var videoPos = timeOffset - Settings.VideoSettings.VideoOffset;
        return ((videoPos >= TimeSpan.Zero) && (videoPos.TotalMilliseconds < Video.MPlayer.Length));
    }

    /// <summary>
    /// Returns if this project has at least one valid video segment attached
    /// </summary>
    /// <returns></returns>
    private bool HasVideoAttached()
    {
        return (Video?.MPlayer?.Length ?? 0) > 0;
    }

    /// <summary>
    /// Called when source has been opened and is ready to start adding data packets
    /// </summary>
    /// <param name="inputReader"></param>
    public virtual void OnInputSourceOpened(BaseInputReader inputReader)
    {
        lock (LoadedPacketList)
        {
            LoadedPacketList.Clear();
        }
    }

    /// <summary>
    /// Called when the InputReader failed to read or compile the source
    /// </summary>
    /// <param name="inputReader"></param>
    /// <param name="errorMessage"></param>
    public virtual void OnInputError(BaseInputReader inputReader, string errorMessage)
    {
        // Do nothing
    }

    /// <summary>
    /// Called when the input reader has finished reading the source, or if forcefully closed
    /// </summary>
    /// <param name="inputReader"></param>
    public virtual void OnInputSourceClosing(BaseInputReader inputReader)
    {
        ReIndexLoadedPackets();
        PopulateListBox(0);
    }

    /// <summary>
    /// Called when the input reader has parsed a new piece of packet data
    /// </summary>
    /// <param name="inputReader"></param>
    /// <param name="packetData"></param>
    public virtual void OnInputDataRead(BaseInputReader inputReader, BasePacketData packetData)
    {
        lock (LoadedPacketList)
        {
            LoadedPacketList.Add(packetData);
        }
    }

    public static void OnInputProgressUpdate(BaseInputReader? inputReader, int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        var title = inputReader != null ? "Reading with " + inputReader.Name : "Reading input data";
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue, title, null);
    }

    public static void OnPopulateProgressUpdate(int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue, Resources.PopulateListBox, null);
    }

    public static void OnParseProgressUpdate(BaseParser parser, int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue,
            string.Format(Resources.ParsePackets, parser.Name), null);
    }

    public static void OnExpandProgressUpdate(BaseParser parser, int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue,
            string.Format(Resources.ExpandingPackets, parser.Name), null);
    }

    /// <summary>
    /// Get the rules' stream id and name based on a port number
    /// </summary>
    /// <param name="port"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public (byte, string, string) GetExpectedStreamIdByPort(ushort port, byte defaultValue)
    {
        return PortToStreamIdMapping.TryGetValue(port, out var id) ? id : (defaultValue, "S" + defaultValue, "?");
    }

    public string GetStreamIdName(byte id)
    {
        foreach (var valueTuple in PortToStreamIdMapping)
        {
            if (valueTuple.Value.Item1 == id)
                return valueTuple.Value.Item2;
        }

        return "S" + id;
    }

    public byte GetStreamIdByName(string name)
    {
        foreach (var valueTuple in PortToStreamIdMapping)
        {
            if (valueTuple.Value.Item2 == name)
                return valueTuple.Value.Item1;
        }
        return 0;
    }

    public string GetStreamIdShortName(byte id)
    {
        foreach (var valueTuple in PortToStreamIdMapping)
        {
            if (valueTuple.Value.Item1 == id)
                return valueTuple.Value.Item3;
        }

        return "?";
    }

    public ushort GetStreamIdPort(byte id)
    {
        foreach (var valueTuple in PortToStreamIdMapping)
        {
            if (valueTuple.Value.Item1 == id)
                return valueTuple.Key;
        }

        return 0;
    }


    /// <summary>
    /// Makes sure that a port is a valid option, adds it at the end if not yet registered
    /// </summary>
    /// <param name="port"></param>
    /// <param name="streamName"></param>
    /// <param name="shortName"></param>
    public void RegisterPort(ushort port, string streamName, string shortName)
    {
        if (!PortToStreamIdMapping.TryGetValue(port, out _))
            PortToStreamIdMapping.Add(port, ((byte)PortToStreamIdMapping.Count, streamName, shortName));
    }

    /// <summary>
    /// Opens the project settings dialog
    /// </summary>
    /// <returns>True if OK or Save was pressed</returns>
    public bool OpenSettings()
    {
        using var settings = new ProjectSettingsDialog();
        settings.AssignProject(this);
        var oldRulesFile = InputParser?.Rules?.LoadedRulesFileName ?? "";
        var dialogOk = (settings.ShowDialog() == DialogResult.OK);
        if (dialogOk)
        {
            Settings.Tags = settings.GetTagsList();
            Settings.VideoSettings.VideoUrl = settings.TextVideoURL.Text;
            Settings.ProjectUrl = settings.TextProjectURL.Text;
            Settings.Description = settings.TextDescription.Text;
            var newRulesFile = (settings.CbRules.SelectedValue as string);
            if ((oldRulesFile != newRulesFile) && (newRulesFile != null) && File.Exists(newRulesFile) &&
                (InputParser != null))
            {
                MessageBox.Show(Resources.RulesChangedReparsingProject, Resources.SaveProject, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                InputParser.OpenRulesFile(newRulesFile);
                InputParser.ParseAllData(true);
                ReIndexLoadedPackets();
                PopulateListBox();
            }

            IsDirty = true;
        }

        return dialogOk;
    }

    public static bool PacketDataDirectionDialog(BasePacketData packetData, out PacketDataDirection selectedDirection)
    {
        selectedDirection = packetData.PacketDataDirection;
        var isUndefinedPacketDirection = true;
        //return false;
        // Ask for type

        using var askDlg = new PacketTypeSelectForm();
        askDlg.HeaderDataLabel.Text = packetData.HeaderText;
        var askDlgRes = askDlg.ShowDialog();

        if (askDlgRes == DialogResult.Yes)
        {
            selectedDirection = PacketDataDirection.Incoming;
            isUndefinedPacketDirection = false;
            //packetData.PacketDataDirection = selectedDirection;
        }
        else if (askDlgRes == DialogResult.No)
        {
            selectedDirection = PacketDataDirection.Outgoing;
            isUndefinedPacketDirection = false;
            //packetData.PacketDataDirection = selectedDirection;
        }
        else
        {
            selectedDirection = PacketDataDirection.Unknown;
        }

        return !isUndefinedPacketDirection;
    }

    public bool SaveProjectSettingsFile(string fileName, string projectFolder)
    {
        // temporary save file paths so they can be returned after making relative
        var pFile = Settings.ProjectFile;
        var log = Settings.LogFile;
        var video = Settings.VideoSettings.VideoFile;
        var rFile = Helper.MakeRelative(ProjectFolder, InputParser?.Rules?.LoadedRulesFileName ?? "");
        var res = false;
        try
        {
            Settings.ProjectFile = Helper.MakeRelative(ProjectFolder, Settings.ProjectFile);
            Settings.LogFile = Helper.MakeRelative(ProjectFolder, Settings.LogFile);
            Settings.VideoSettings.VideoFile = Helper.MakeRelative(ProjectFolder, Settings.VideoSettings.VideoFile);
            Settings.InputReader = InputReader?.Name ?? "";
            Settings.Parser = InputParser?.Name ?? "";
            Settings.RulesFile = rFile;
            Settings.Filter.CopyFrom(Filter);
            Settings.Search.CopyFrom(SearchParameters);

            var writer = new System.Xml.Serialization.XmlSerializer(typeof(ProjectSettings));

            using var fileStream = File.Create(fileName);

            writer.Serialize(fileStream, Settings);
            fileStream.Close();
            RequestUpdatedProjectFileName = false;
            IsDirty = false;
            res = true;
        }
        catch
        {
            //
        }

        Settings.ProjectFile = pFile;
        Settings.LogFile = log;
        Settings.VideoSettings.VideoFile = video;
        return res;
    }

    private ProjectSettings? LoadLegacyProjectSettings(List<string> sl)
    {
        var res = new ProjectSettings();
        try
        {
            foreach (string s in sl)
            {
                var fields = s.Split(';');
                if (fields.Length < 2)
                    continue;
                var fieldType = fields[0].ToLower();
                var fieldVal = fields[1];
                if (fieldType == "packetlog")
                {
                    res.LogFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                }
                else if (fieldType == "rules")
                {
                    res.RulesFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                }
                else if (fieldType == "video")
                {
                    res.VideoSettings.VideoFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                }
                else if (fieldType == "youtube")
                {
                    res.VideoSettings.VideoUrl = fieldVal;
                }
                else if (fieldType == "offset")
                {
                    if (NumberHelper.TryFieldParse(fieldVal, out int n))
                        res.VideoSettings.VideoOffset = TimeSpan.FromMilliseconds(n);
                }
                else if (fieldType == "packedsource")
                {
                    res.ProjectUrl = fieldVal;
                }
                else if (fieldType == "tags")
                {
                    res.Tags = ProjectSettingsDialog.TagsToList(fieldVal);
                }
                else if (fieldType == "decrypt")
                {
                    res.DecryptionName = fieldVal;
                }
                /*
                else
                if (fieldType == "pin")
                {
                    // not used for reading inside this program
                }
                else
                if (fieldType == "pout")
                {
                    // not used for reading inside this program
                }
                else
                {
                    continue;
                }
                */
            }
        }
        catch
        {
            return null;
        }

        return res;
    }

    public ProjectSettings? LoadProjectSettingsFile(string fileName)
    {
        RequestUpdatedProjectFileName = false;
        try
        {
            var testText = File.ReadAllText(fileName);
            var oldStyle = testText.Contains("packetlog;");
            var newStyle = testText.Contains("<?xml ");

            if (newStyle)
            {
                var reader = new System.Xml.Serialization.XmlSerializer(typeof(ProjectSettings));
                using var file = File.OpenRead(fileName);
                var res = reader.Deserialize(file) as ProjectSettings;
                file.Close();
                if (res != null)
                {
                    var pFolder = Path.GetDirectoryName(fileName) ?? "";
                    res.ProjectFile = Helper.TryMakeFullPath(pFolder, res.ProjectFile);
                    res.LogFile = Helper.TryMakeFullPath(pFolder, res.LogFile);
                    res.RulesFile = Helper.TryMakeFullPath(pFolder, res.RulesFile);
                    res.VideoSettings.VideoFile = Helper.TryMakeFullPath(pFolder, res.VideoSettings.VideoFile);
                }

                return res;
            }

            if (oldStyle)
            {
                var lines = File.ReadAllLines(fileName).ToList();
                var res = LoadLegacyProjectSettings(lines);
                if (res != null)
                {
                    RequestUpdatedProjectFileName = true;
                    // We managed to load a old format, backup the file
                    try
                    {
                        var oldName = fileName + ".old";
                        if (!File.Exists(oldName))
                            File.Copy(fileName, oldName);
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                return res;
            }
        }
        catch
        {
            // Don't show a error, just return null
        }

        return null;
    }

    public void CloseProject(bool skipSave)
    {
        if (IsDirty && (skipSave == false))
        {
            if (MessageBox.Show(string.Format(Resources.SaveChangesToProject, Text), Resources.SaveProject,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                MainForm.Instance?.SaveProject(this, ModifierKeys.HasFlag(Keys.Shift));
        }

        Video?.Close();
        Dispose();
    }

    public void ReloadPacketListColorsFromSettings()
    {
        PacketsListBox.Font = Properties.Settings.Default.PacketListFont;
        PacketsListBox.ItemHeight = (int)Math.Ceiling(PacketsListBox.Font.GetHeight());
        PacketsListBox.Dock = DockStyle.Fill;
        PacketsListBox.Refresh();
    }

    public int SearchFrom(ViewedProjectTab project, SearchParameters search)
    {
        if ((project.InputParser == null) || (project.InputParser.Rules == null) ||
            (File.Exists(project.InputParser.Rules.LoadedRulesFileName) == false))
            return 0;

        int c = 0;
        LoadedPacketList.Clear();
        AllFieldNames.Clear();
        InputParser?.OpenRulesFile(project.InputParser.Rules.LoadedRulesFileName);
        //XorKey = original.XorKey;
        //AesKey = original.AesKey;
        foreach (var pd in project.LoadedPacketList.Where(pd => pd.MatchesSearch(search)))
        {
            LoadedPacketList.Add(pd);
            c++;
        }

        return c;
    }

    public void GotoVideoOffset(int videoPosition)
    {
        var offset = TimeSpan.FromMilliseconds(videoPosition).Add(Settings.VideoSettings.VideoOffset);
        if (PacketsListBox.Items.Count > 1)
        {
            if ((PacketsListBox.Items[0] is BasePacketData data0) && (offset < data0.OffsetFromStart))
                offset = data0.OffsetFromStart;
            if ((PacketsListBox.Items[^1] is BasePacketData dataLast) && (offset > dataLast.OffsetFromStart))
                offset = dataLast.OffsetFromStart;
        }

        foreach (var lbItem in PacketsListBox.Items)
        {
            if (lbItem is not BasePacketData data)
                continue;
            if (data.OffsetFromStart >= offset)
            {
                PacketsListBox.SelectedItem = lbItem;
                CenterListBox();
                return;
            }
        }
    }

    public void GotoPacketTimeOffset(TimeSpan offset)
    {
        var o = offset.Add(TimeSpan.FromMilliseconds(-5));
        foreach (var lbItem in PacketsListBox.Items)
        {
            if (lbItem is not BasePacketData data)
                continue;
            if (data.VirtualOffsetFromStart >= o)
            {
                PacketsListBox.SelectedItem = lbItem;
                CenterListBox();
                return;
            }
        }
    }

    public void OpenVideoForm()
    {
        Video ??= new VideoForm
        {
            ParentProject = this,
        };

        _ = Video.OpenVideoFromProject();
        Video.Show();
        Video.BringToFront();
    }

    public void RunExportDataTool(string exportName)
    {
        if ((InputParser == null) || (InputParser.Rules == null))
            return;

        if (!InputParser.Rules.ExportDataTools.TryGetValue(exportName, out var exportTool))
            return;

        InputParser.Rules.CurrentExportDataTool = exportTool;
        InputParser.Rules.CurrentExportCount = 0;
        var targetFile = Path.Combine(ProjectFolder, exportTool.FileName);
        if (File.Exists(targetFile))
        {
            var res = MessageBox.Show(string.Format(Resources.AppendToCurrentFile, targetFile),
                Resources.OverwriteFileTitle,
                MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Yes)
            {
                // Good to go
            }
            else if (res == DialogResult.No)
            {
                // No = Create a new file, so delete the old one
                File.Delete(targetFile);
            }
            else
            {
                // Cancelled
                return;
            }
        }


        // Re-run parser with tool enabled
        File.AppendAllText(targetFile, exportTool.Header);
        InputParser.ParseAllData(false);
        File.AppendAllText(targetFile, exportTool.Footer);

        // Disable again
        InputParser.Rules.CurrentExportDataTool = null;

        // Show export report
        MessageBox.Show(
            InputParser.Rules.CurrentExportCount > 0
                ? string.Format(Resources.ExportedXItems, InputParser.Rules.CurrentExportCount)
                : Resources.NoDataExported, Resources.ExportDataTitle, MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    }

    /// <summary>
    /// Returns a list of FullFilterKeys of each used 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public List<ulong> GetAllUsedPacketsByDirection(PacketDataDirection direction)
    {
        var res = new List<ulong>();
        foreach (var basePacketData in LoadedPacketList)
        {
            if (basePacketData.PacketDataDirection != direction)
                continue;

            var key = PacketFilterListEntry.EncodeFilterKey(basePacketData.PacketId, basePacketData.CompressionLevel,
                basePacketData.StreamId);
            if (!res.Contains(key))
                res.Add(key);
        }

        return res;
    }

    public List<BasePacketData> GetVisiblePacketList()
    {
        var list = new List<BasePacketData>();
        foreach (var selectedItem in PacketsListBox.Items)
        {
            if (selectedItem is BasePacketData packetData)
                list.Add(packetData);
        }

        return list;
    }

    public string ExportPacketsAsXmlString(List<BasePacketData> packets, bool includeRawData, bool includeParsedData,
        bool separateLines)
    {
        var xmlDoc = ExportPacketsAsXml(packets, includeRawData, includeParsedData);

        return !separateLines ? xmlDoc.OuterXml : xmlDoc.OuterXml.Replace("\r", "").Replace("><", ">\n<");
    }

    public XmlDocument ExportPacketsAsXml(List<BasePacketData> packets, bool includeRawData, bool includeParsedData)
    {
        var xmlDoc = new XmlDocument();
        var listNode = xmlDoc.CreateNode(XmlNodeType.Element, "packetlist", null);
        xmlDoc.AppendChild(listNode);
        MainForm.Instance?.UpdateStatusBarProgress(0, packets.Count, "Export Packets", null);
        var progress = 0;
        foreach (var basePacketData in packets)
        {
            // var packetNode = xmlDoc.CreateNode(XmlNodeType.Element, "packetdata", null);
            var packetNode = XmlHelper.CreateNewXmlElementNode(listNode, "pd"); // packet data
            if (packetNode == null)
                break; // Error

            XmlHelper.SetAttribute(packetNode, "d", basePacketData.PacketDataDirection == PacketDataDirection.Incoming ? "I" : basePacketData.PacketDataDirection == PacketDataDirection.Outgoing ? "O" : "?");
            XmlHelper.SetAttribute(packetNode, "s", GetStreamIdName(basePacketData.StreamId));
            XmlHelper.SetAttribute(packetNode, "n", basePacketData.GetPacketName());
            if (basePacketData.CompressionLevel > 0)
                XmlHelper.SetAttribute(packetNode, "l", basePacketData.CompressionLevel.ToString());
            XmlHelper.SetAttribute(packetNode, "p", basePacketData.PacketId.ToHex(3));
            XmlHelper.SetAttribute(packetNode, "t", basePacketData.TimeStamp.Ticks.ToString());
            if (basePacketData.SyncId > 0)
                XmlHelper.SetAttribute(packetNode, "c", basePacketData.SyncId.ToString());

            if (includeRawData)
            {
                var dataNode = XmlHelper.CreateNewXmlElementNode(packetNode, "d");
                if (dataNode != null)
                {
                    dataNode.InnerText = basePacketData.GetDataAtPos(0, basePacketData.ByteData.Count).Trim();
                }
            }

            if (includeParsedData)
            {
                foreach (var parsedField in basePacketData.ParsedData)
                {
                    if ((parsedField.DisplayedByteOffset == "") &&
                        (parsedField.FieldName == Resources.UnParsedFieldName) &&
                        (parsedField.FieldValue == Resources.UnParsedFieldDescription))
                        break;

                    var parseNode = XmlHelper.CreateNewXmlElementNode(packetNode, "p");
                    if (parseNode == null)
                        break; // Error

                    XmlHelper.SetAttribute(parseNode, "p", parsedField.DisplayedByteOffset);
                    XmlHelper.SetAttribute(parseNode, "n", parsedField.FieldName);
                    XmlHelper.SetAttribute(parseNode, "v", parsedField.FieldValue);
                }
            }

            progress++;
            MainForm.Instance?.UpdateStatusBarProgress(progress, packets.Count, "Export Packets", null);
        }

        MainForm.Instance?.UpdateStatusBarProgress(packets.Count, packets.Count, "Export Packets", null);
        return xmlDoc;
    }

    public bool ExportParsedDataAsXml(string fileName, bool compressed)
    {
        try
        {
            var list = GetVisiblePacketList();
            if (compressed)
            {
                var xmlDoc = ExportPacketsAsXml(list, true, true);
                using var fileStream = new FileStream(fileName, FileMode.Create);
                fileStream.Write(new byte[] { 0x56, 0x50, 0x58, 0 });
                using var bStream = new BZip2OutputStream(fileStream, false);
                LoadingForm.OnProgress(0, 100, "Saving export", null, true);
                xmlDoc.Save(bStream);
            }
            else
            {
                var xml = ExportPacketsAsXmlString(list, true, true, false);
                LoadingForm.OnProgress(0, 100, "Saving export", null, true);
                File.WriteAllText(fileName, xml);
            }
            LoadingForm.OnProgress(100, 100, "Saving export", null, true);
        }
        catch
        {
            return false;
        }

        return true;
    }

    /*
    public bool ExportParsedDataAsCsv(string fileName)
    {
        try
        {
            using var writer = new StreamWriter(fileName);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = ";",
                InjectionOptions = InjectionOptions.Escape,
            };
            using var csv = new CsvWriter(writer, config);

            foreach (var basePacketData in LoadedPacketList)
            {
                csv.WriteField("");
                csv.WriteField("");
                csv.WriteField("");
                csv.NextRecord();
                foreach (var parsedField in basePacketData.ParsedData)
                {
                    if ((parsedField.DisplayedByteOffset == "") &&
                        (parsedField.FieldName == Resources.UnParsedFieldName) &&
                        (parsedField.FieldValue == Resources.UnParsedFieldDescription))
                        break;

                    csv.WriteField(parsedField.DisplayedByteOffset);
                    csv.WriteField(parsedField.FieldName);
                    csv.WriteField(parsedField.FieldValue);
                    csv.NextRecord();
                }
                
            }
            csv.Flush();
        }
        catch
        {
            return false;
        }

        return true;
    }
    */

    public bool ImportFromVpxFile(string fileName)
    {
        try
        {
            using var fileStream = File.OpenRead(fileName);
            var header = new byte[4];
            if (fileStream.Read(header, 0, header.Length) < 4)
                throw new Exception("file is too small");
            //0x56, 0x50, 0x58, 0
            if ((header[0] != 0x56)|| (header[1] != 0x50) || (header[2] != 0x58))
                throw new Exception("Invalid header");
            if (header[3] != 0)
                throw new Exception("Unsupported file version");

            using var bStream = new BZip2InputStream(fileStream);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(bStream);

            ushort streamCounter = 1;
            var xmlPackets = xmlDoc.SelectNodes("/packetlist/pd");
            OnInputProgressUpdate(null, 0, 1);
            if (xmlPackets != null)
            {
                OnInputProgressUpdate(null, 0, xmlPackets.Count);
                for (var x = 0; x < xmlPackets.Count; x++)
                {
                    var xmlPacket = xmlPackets[x];
                    if (xmlPacket == null)
                        continue;

                    var packetAttributes = XmlHelper.ReadNodeAttributes(xmlPacket);

                    var packetStreamName = XmlHelper.GetAttributeString(packetAttributes, "s");
                    // for now register dummy ports where stream Id = Port
                    var packetStreamId = GetStreamIdByName(packetStreamName);
                    if (packetStreamId == 0)
                    {
                        RegisterPort(streamCounter, packetStreamName, packetStreamName[..1]);
                        packetStreamId = GetStreamIdByName(packetStreamName);
                    }

                    var packetLevel = (byte)XmlHelper.GetAttributeInt(packetAttributes, "l");
                    var packetId = (uint)XmlHelper.GetAttributeInt(packetAttributes, "p");
                    var packetTicks = XmlHelper.GetAttributeInt(packetAttributes, "t");
                    var packetName = XmlHelper.GetAttributeString(packetAttributes, "n");
                    var packetDirection = XmlHelper.GetAttributeString(packetAttributes, "d");
                    var packetSync = (int)XmlHelper.GetAttributeInt(packetAttributes, "c");

                    var packet = new BasePacketData(this);
                    packet.SourcePort = GetStreamIdPort(packetStreamId);
                    packet.PacketDataDirection = packetDirection == "I"
                        ? PacketDataDirection.Incoming
                        : packetDirection == "O"
                            ? PacketDataDirection.Outgoing
                            : PacketDataDirection.Unknown;
                    packet.PacketId = packetId;
                    packet.CompressionLevel = packetLevel;
                    packet.ParsedPacketName = packetName;
                    packet.TimeStamp = DateTime.UtcNow.AddTicks(packetTicks);
                    packet.SyncId = packetSync;
                    packet.DoNotParse = true;

                    packet.BuildHeaderText();

                    // TODO: Read Raw Data
                    // TODO: Read Parsed Data

                    LoadedPacketList.Add(packet);
                    if ((x % 20) == 0)
                        OnInputProgressUpdate(null, x, xmlPackets.Count);
                }
            }
            OnInputProgressUpdate(null, 1, 1);
            ReIndexLoadedPackets();
            PopulateListBox();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
}