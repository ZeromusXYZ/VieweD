using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using VieweD.Forms;
using VieweD.Helpers.System;
using VieweD.engine.serialize;
using VieweD.Properties;
using System.Media;

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
    public string ProjectFolder => string.IsNullOrWhiteSpace(ProjectFile) ? "" : Path.GetDirectoryName(ProjectFile) ?? "";
    /// <summary>
    /// Name of this project (derived from ProjectFile)
    /// </summary>
    public string ProjectName => string.IsNullOrWhiteSpace(ProjectFile) ? "VieweD Project.pvd" : Path.GetFileNameWithoutExtension(ProjectFile);

    public string ProjectFile
    {
        get => _projectFile;
        set
        {
            if (value != _projectFile)
                IsDirty = true;
            _projectFile = value; 
            
        }
    }

    /// <summary>
    /// Actual file that has been loaded
    /// </summary>
    public string OpenedLogFile { get; set; }

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

    /// <summary>
    /// Mapping to use to convert a target port number to a StreamId used by the parsers
    /// </summary>
    public Dictionary<ushort, (byte, string)> PortToStreamIdMapping { get; private set; } = new ();
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

    public List<string> Tags { get; set; }
    /// <summary>
    /// Set to true if a old format project file was loaded
    /// </summary>
    public bool RequestUpdatedProjectFileName { get; set; }

    public SearchParameters SearchParameters { get; set; }
    public List<string> AllFieldNames { get; set; }
    public ProjectVideoSettings VideoSettings { get; set; } = new();
    public VideoForm? Video { get; set; }
    public RulesEditorForm? CurrentEditor { get; set; }
    public GameViewForm? GameView { get; set; }

    public ViewedProjectTab()
    {
        TimeStampFormat = "HH:mm:ss";
        _projectFile = string.Empty;

        Filter = new PacketListFilter();
        Filter.Clear();

        Tags = new List<string>();
        Tags.Clear();

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
        OpenedLogFile = string.Empty;
        CurrentSyncId = -1;
        LoadedPacketList = new List<BasePacketData>();

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

        var packetKey = new PacketFilterListEntry(packetData.PacketId, packetData.CompressionLevel, packetData.StreamId);

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

        var packetKey = new PacketFilterListEntry(packetData.PacketId, packetData.CompressionLevel, packetData.StreamId);

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
        PmPlExportPacket.Enabled = false;// packetData != null;
        
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
                var lookupName = PacketFilterListEntry.AsString(thisRule.PacketId, thisRule.Level, thisRule.StreamId);
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

                rule = InputParser?.Rules?.CreateNewUserPacketRule(this, packetData.PacketDataDirection,
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

                if ((LoadedPacketList[i].OffsetFromStart == lastSameOffset) && (i < LoadedPacketList.Count-1))
                {
                    sameTimeCount++;
                }
                else
                {
                    if ((sameTimeCount > 0) && (i > lastSameTimeIndex + 1))
                    {
                        sameTimeCount++;
                        var timeSpanDelta = (LoadedPacketList[i].OffsetFromStart - lastSameOffset) / (double)sameTimeCount;
                        // Only update virtual times if there is a noticeable time difference
                        if (timeSpanDelta.TotalMilliseconds > 1)
                        {
                            for (var n = 1; n <= sameTimeCount-1; n++)
                            {
                                LoadedPacketList[lastSameTimeIndex + n].VirtualOffsetFromStart =
                                    lastSameOffset + (timeSpanDelta * (double)n);
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
        Cursor = Cursors.WaitCursor;
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
        Cursor = Cursors.Default;
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
            Video?.UpdateVideoPositionFromProject(pd.VirtualOffsetFromStart + VideoSettings.VideoOffset);
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
                else
                if (barOn)
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
                else
                if (barOn)
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
                else
                if (barOn)
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
            textCol = Color.FromArgb(textCol.A, (backCol.R + textCol.R) / 2, (backCol.G + textCol.G) / 2, (backCol.B + textCol.B) / 2);

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
        // s = pd.VirtualTimeStamp.ToString() + "." + pd.VirtualTimeStamp.Millisecond.ToString("0000");

        var icon1 = new Rectangle(e.Bounds.Left, e.Bounds.Top + ((e.Bounds.Height - Resources.mini_unk_icon.Height) / 2), Resources.mini_unk_icon.Width, Resources.mini_unk_icon.Height);
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
            textBounds = new Rectangle(e.Bounds.Left + (icon1.Width * 2), e.Bounds.Top, e.Bounds.Width - (icon1.Width * 2), e.Bounds.Height);
        }
        else
        {
            textBounds = new Rectangle(e.Bounds.Left + (icon1.Width), e.Bounds.Top, e.Bounds.Width - (icon1.Width), e.Bounds.Height);
        }

        switch (PacketColors.PacketListStyle)
        {
            case 1:
                // Colored arrows
                if (pd.PacketDataDirection == PacketDataDirection.Incoming)
                {
                    e.Graphics.DrawImage(Resources.mini_in_icon, icon1);
                }
                else
                if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
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
                else
                if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
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
                else
                if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
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
            e.Graphics.FillRectangle(barBrush, new Rectangle(e.Bounds.Right - barSize, e.Bounds.Top, barSize, e.Bounds.Height));
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
        // TODO: Do actual calculations
        if ((Video == null) || (Video.MPlayer == null))
            return false;

        var videoPos = timeOffset - VideoSettings.VideoOffset;
        return ((videoPos >= TimeSpan.Zero) && (videoPos.TotalMilliseconds < Video.MPlayer.Length));
    }

    /// <summary>
    /// Returns if this project has at least one valid video segment attached
    /// </summary>
    /// <returns></returns>
    private bool HasVideoAttached()
    {
        // TODO: Do actual check
        return true;
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

    public void OnInputProgressUpdate(BaseInputReader inputReader, int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue,null,null);
    }
    public void OnPopulateProgressUpdate(int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue, Resources.PopulateListBox, null);
    }

    public void OnParseProgressUpdate(BaseParser parser, int position, int maxValue)
    {
        // NOTE: it's possible to manipulate the value based on the reader
        MainForm.Instance?.UpdateStatusBarProgress(position, maxValue, Resources.ParsePackets, null);
    }

    /// <summary>
    /// Get the rules' stream id and name based on a port number
    /// </summary>
    /// <param name="port"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public (byte, string) GetExpectedStreamIdByPort(ushort port, byte defaultValue)
    {
        return PortToStreamIdMapping.TryGetValue(port, out var id) ? id : (defaultValue, "S"+defaultValue);
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

    /// <summary>
    /// Makes sure that a port is a valid option, adds it at the end if not yet registered
    /// </summary>
    /// <param name="port"></param>
    /// <param name="streamName"></param>
    public void RegisterPort(ushort port, string streamName)
    {
        if (!PortToStreamIdMapping.TryGetValue(port, out _))
            PortToStreamIdMapping.Add(port, ((byte)PortToStreamIdMapping.Count, streamName));
    }

    /// <summary>
    /// Opens the project settings dialog
    /// </summary>
    /// <returns>True if OK or Save was pressed</returns>
    public bool OpenSettings()
    {
        using var settings = new ProjectSettingsDialog();
        settings.AssignProject(this);
        return (settings.ShowDialog() == DialogResult.OK);
    }

    public bool PacketDataDirectionDialog(BasePacketData packetData, out PacketDataDirection selectedDirection)
    {
        // TODO: Make new Dialog Form
        selectedDirection = packetData.PacketDataDirection;
        return false;
        // Ask for type
        /*
        var askDlgRes = DialogResult.Cancel;
        using (PacketTypeSelectForm askDlg = new PacketTypeSelectForm())
        {
            askDlg.lHeaderData.Text = s;
            askDlgRes = askDlg.ShowDialog();
        }

        if (askDlgRes == DialogResult.Yes)
        {
            preferredDirection = PacketDataDirection.Incoming;
            isUndefinedPacketDirection = false;
            packetData.PacketDataDirection = preferredDirection;
        }
        else
        if (askDlgRes == DialogResult.No)
        {
            preferredDirection = PacketDataDirection.Outgoing;
            isUndefinedPacketDirection = false;
            packetData.PacketDataDirection = preferredDirection;
        }
        */
    }

    public bool SaveProjectSettingsFile(string fileName, string projectFolder)
    {
        try
        {
            var settings = new ProjectSettings
            {
                ProjectFile = Helper.MakeRelative(projectFolder, ProjectFile),
                LogFile = Helper.MakeRelative(projectFolder, OpenedLogFile),
                InputReader = InputReader?.Name ?? "",
                Parser = InputParser?.Name ?? "",
                RulesFile = Helper.MakeRelative(projectFolder, InputParser?.Rules?.LoadedRulesFileName ?? ""),
                VideoSettings =
                {
                    VideoFile = Helper.MakeRelative(projectFolder, VideoSettings.VideoFile),
                    VideoUrl = VideoSettings.VideoUrl,
                    VideoOffset = VideoSettings.VideoOffset,
                },
                Tags = Tags,
                LastTimeOffset = (PacketsListBox.SelectedItem as BasePacketData)?.VirtualOffsetFromStart ?? TimeSpan.Zero,
            };
            settings.Filter.CopyFrom(Filter);
            settings.Search.CopyFrom(SearchParameters);

            var writer = new System.Xml.Serialization.XmlSerializer(typeof(ProjectSettings));

            using var fileStream = File.Create(fileName);

            writer.Serialize(fileStream, settings);
            fileStream.Close();
            RequestUpdatedProjectFileName = false;
            IsDirty = false;
            return true;
        }
        catch
        {
            //
        }
        return false;
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
                else
                if (fieldType == "rules")
                {
                    res.RulesFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                }
                else
                if (fieldType == "video")
                {
                    res.VideoSettings.VideoFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                }
                else
                if (fieldType == "youtube")
                {
                    res.VideoSettings.VideoUrl = fieldVal;
                }
                else
                if (fieldType == "offset")
                {
                    if (NumberHelper.TryFieldParse(fieldVal, out int n))
                        res.VideoSettings.VideoOffset = TimeSpan.FromMilliseconds(n);
                }
                else
                if (fieldType == "packedsource")
                {
                    res.ProjectUrl = fieldVal;
                }
                else
                if (fieldType == "tags")
                {
                    res.Tags = ProjectSettingsDialog.TagsToList(fieldVal);
                }
                else
                if (fieldType == "decrypt")
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
            if (MessageBox.Show("Save changes to project: " + Text + " ?", Resources.SaveProject,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                MainForm.Instance?.SaveProject(this, ModifierKeys.HasFlag(Keys.Shift));
        }

        Video?.Close();
        Dispose();
    }

    public void ReloadPacketListColorsFromSettings()
    {
        PacketsListBox.Font = Settings.Default.PacketListFont;
        PacketsListBox.ItemHeight = (int)Math.Ceiling(PacketsListBox.Font.GetHeight());
        PacketsListBox.Dock = DockStyle.Fill;
        PacketsListBox.Refresh();
    }

    public int SearchFrom(ViewedProjectTab project, SearchParameters search)
    {
        if ((project.InputParser == null) || (project.InputParser.Rules == null) || (File.Exists(project.InputParser.Rules.LoadedRulesFileName) == false))
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
        var offset = TimeSpan.FromMilliseconds(videoPosition).Add(VideoSettings.VideoOffset);
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
        if (Video == null)
        {
            Video = new VideoForm();
            Video.ParentProject = this;
        }

        _ = Video.OpenVideoFromProject();
        Video.Show();
        Video.BringToFront();
    }
}