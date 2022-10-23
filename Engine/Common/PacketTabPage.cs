using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms ;
using System.ComponentModel;
using System.Drawing;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class PacketTabPage: TabPage
    {
        private MainForm OwnerMainForm { get; }
        // ReSharper disable once InconsistentNaming
        public PacketList PLLoaded { get; set; } // File Loaded
        // ReSharper disable once InconsistentNaming
        public PacketList PL { get; set; } // Filtered File Data Displayed

        public uint CurrentSync { get; set; }
        public string LoadedLogFile { get; set; }
        public string LoadedRulesFile { get; set; }
        public VideoLinkForm VideoLink { get; set; }
        public string ProjectFolder { get; set; }
        public string ProjectFile { get; set; }
        public string ProjectTags { get; set; }
        public string LinkVideoFileName { get; set; }
        public string LinkYoutubeUrl { get; set; }
        public string LinkPacketsDownloadUrl { get; set; }
        public TimeSpan LinkVideoOffset { get; set; }
        public string DecryptVersion { get; set; } = "_None_";

        public FlickerFreeListBox LbPackets { get; }

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

        // Engine Handler
        public EngineBase Engine { get; set; }

        public PacketTabPage(MainForm mainForm)
        {
            OwnerMainForm = mainForm;

            Engine = new EngineBase(this); // Default NULL Handler

            // Create base controls
            PLLoaded = new PacketList(this);
            PL = new PacketList(this);
            LbPackets = new FlickerFreeListBox();
            VideoLink = null;
            ProjectFolder = string.Empty;
            LinkVideoFileName = string.Empty;
            LinkYoutubeUrl = string.Empty;
            LinkPacketsDownloadUrl = string.Empty;
            ProjectFile = string.Empty;

            // Set ListBox Position
            LbPackets.Parent = this;
            LbPackets.Location = new Point(0, 0);
            LbPackets.Size = new Size(this.Width, this.Height);
            LbPackets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LbPackets.Dock = DockStyle.Fill;
            LbPackets.Font = Properties.Settings.Default.PacketListFont ; // Add fixed sized font (to override the tab page itself)
            LbPackets.ItemHeight = (int)Math.Ceiling(LbPackets.Font.GetHeight());
            //lbPackets.Font = new Font("Consolas", 9); // Add fixed sized font (to override the tab page itself)
            LbPackets.DrawMode = DrawMode.OwnerDrawFixed;

            LbPackets.DrawItem += ListboxPackets_DrawItem;

            // Add the SelectedIndexChanged for this from MainForm/process creating it, as it's too complex to handle internally
            // lbPackets.SelectedIndexChanged += new System.EventHandler(this.lbPackets_SelectedIndexChanged); 

            // Title to use on main program as "Filename"
            LoadedLogFile = "?Packets";
            ProjectTags = string.Empty;

            // Create Popup Menu
            PmPl = new ContextMenuStrip();
            PmPl.Opening += PmPL_Opening;

            LbPackets.ContextMenuStrip = PmPl;

            PmPlShowPacketName = new ToolStripMenuItem("Show packet name");
            PmPl.Items.Add(PmPlShowPacketName);

            PmPls1 = new ToolStripSeparator();
            PmPl.Items.Add(PmPls1);

            PmPlShowOnly = new ToolStripMenuItem("Show this type only");
            PmPlShowOnly.Click += PmPLShowOnly_Click;
            PmPl.Items.Add(PmPlShowOnly);

            PmPlHideThis = new ToolStripMenuItem("Hide this type");
            PmPlHideThis.Click += PmPLHideThis_Click;
            PmPl.Items.Add(PmPlHideThis);

            PmPls2 = new ToolStripSeparator();
            PmPl.Items.Add(PmPls2);

            PmPlShowOutOnly = new ToolStripMenuItem("Show only Outgoing");
            PmPlShowOutOnly.Click += PmPLShowOutgoingOnly_Click;
            PmPl.Items.Add(PmPlShowOutOnly);

            PmPlShowInOnly = new ToolStripMenuItem("Show only Incoming");
            PmPlShowInOnly.Click += PmPLShowIncomingOnly_Click;
            PmPl.Items.Add(PmPlShowInOnly);

            PmPls3 = new ToolStripSeparator();
            PmPl.Items.Add(PmPls3);

            PmPlResetFilters = new ToolStripMenuItem("Reset all filters");
            PmPlResetFilters.Click += PmPLResetFilter_Click;
            PmPl.Items.Add(PmPlResetFilters);

            PmPls4 = new ToolStripSeparator();
            PmPl.Items.Add(PmPls4);

            PmPlEditParser = new ToolStripMenuItem("Edit this parser");
            PmPlEditParser.Click += PmPLEditParser_Click;
            PmPl.Items.Add(PmPlEditParser);

            PmPlExportPacket = new ToolStripMenuItem("Export Packet");
            PmPlExportPacket.Click += PmPLExport_Click;
            PmPl.Items.Add(PmPlExportPacket);

            // Init misc stuff
            CurrentSync = 0xFFFFFFFF;
        }

        public PacketData GetSelectedPacket()
        {
            if ((LbPackets.SelectedIndex < 0) || (LbPackets.SelectedIndex >= PL.Count))
                return null;
            return PL.GetPacket(LbPackets.SelectedIndex);
        }

        private void ListboxPackets_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ListBox lb))
                return;
            
            PacketData pd;
            if ((lb.Parent is PacketTabPage tp) && (e.Index >= 0) && (e.Index < tp.PL.Count))
            {
                pd = tp.PL.GetPacket(e.Index);
            }
            else
            {
                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                return;
            }

            var barOn = (tp.CurrentSync == pd.PacketSync);
            var isSelected = (e.Index == lb.SelectedIndex);
            Color textCol;
            Color backCol;
            Color barCol;

            // Determine the color of the brush to draw each item based 
            // on the index of the item to draw.
            var pltForBackGround = pd.PacketLogType;
            if (pd.MarkedAsInvalid)
                pltForBackGround = PacketLogTypes.Unknown;
            switch (pltForBackGround)
            {
                case PacketLogTypes.Incoming:
                    textCol = PacketColors.ColFontIN;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectIN;
                        textCol = PacketColors.ColSelectedFontIN;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncIN;
                    else
                        backCol = PacketColors.ColBackIN;
                    barCol = PacketColors.ColBarIN;
                    break;
                case PacketLogTypes.Outgoing:
                    textCol = PacketColors.ColFontOUT;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectOUT;
                        textCol = PacketColors.ColSelectedFontOUT;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncOUT;
                    else
                        backCol = PacketColors.ColBackOUT;
                    barCol = PacketColors.ColBarOUT;
                    break;
                default:
                    textCol = PacketColors.ColFontUNK;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectUNK;
                        textCol = PacketColors.ColSelectedFontUNK;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncUNK;
                    else
                        backCol = PacketColors.ColBackUNK;
                    barCol = PacketColors.ColBarUNK;
                    break;
            }

            if (pd.MarkedAsDimmed)
            {
                Colorspace.RgbToHls(backCol, out _, out var l, out _);

                var isDark = l < 0.5 ;

                if (isDark)
                    backCol = Color.FromArgb(backCol.A, backCol.R / 2, backCol.G / 2, backCol.B / 2);
                else
                    backCol = Color.FromArgb(
                        backCol.A,
                        backCol.R > 128 ? 255 : backCol.R * 2,
                        backCol.G > 128 ? 255 : backCol.G * 2,
                        backCol.B > 128 ? 255 : backCol.B * 2);

                textCol = Color.FromArgb(textCol.A, (backCol.R + textCol.R) / 2, (backCol.G + textCol.G) / 2, (backCol.B + textCol.B) / 2);
                // barCol = Color.FromArgb(barCol.A, barCol.R / 4, barCol.G / 4, barCol.B / 4);
            }

            // Define the colors of our brushes.
            Brush textBrush = new SolidBrush(textCol);
            Brush backBrush = new SolidBrush(backCol);
            Brush barBrush = new SolidBrush(barCol);

            // Draw the background of the ListBox control for each item.
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            
            // header text
            var s = lb.Items[e.Index].ToString();
            //s = pd.VirtualTimeStamp.ToString() + "." + pd.VirtualTimeStamp.Millisecond.ToString("0000");

            var icon1 = new Rectangle(e.Bounds.Left, e.Bounds.Top + ((e.Bounds.Height - Properties.Resources.mini_unk_icon.Height) / 2), Properties.Resources.mini_unk_icon.Width, Properties.Resources.mini_unk_icon.Height);
            var icon2 = new Rectangle(icon1.Left + icon1.Width, icon1.Top, icon1.Width, icon1.Height);

            Rectangle textBounds;
            if ((tp.VideoLink != null) && (tp.VideoLink.IsInTimeRange(pd.VirtualTimeStamp)))
            {
                e.Graphics.DrawImage(Properties.Resources.mini_video_icon, icon2);
            }

            if ((tp.LinkVideoFileName != string.Empty) || (tp.LinkYoutubeUrl != string.Empty))
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
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_in_icon, icon1);
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_out_icon, icon1);
                    }
                    else
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_unk_icon, icon1);
                    }
                    break;
                case 2:
                    // transparent arrows
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_in_ticon, icon1);
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_out_ticon, icon1);
                    }
                    else
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_unk_ticon, icon1);
                    }
                    break;
                default:
                    textBounds = e.Bounds ;
                    // No icons, just text
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        s = "<= " + s;
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        s = "=> " + s;
                    }
                    else
                    {
                        s = "?? " + s;
                    }
                    break;
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(s, e.Font, textBrush, textBounds, StringFormat.GenericDefault);

            if (barOn)
            {
                var barSize = 8;
                if (isSelected)
                    barSize = 16;
                e.Graphics.FillRectangle(barBrush, new Rectangle(e.Bounds.Right - barSize, e.Bounds.Top, barSize, e.Bounds.Height));
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        public void CenterListBox()
        {
            // Move to center
            var iHeight = LbPackets.ItemHeight;
            if (iHeight <= 0)
                iHeight = 8;
            var iCount = LbPackets.Size.Height / iHeight;
            var tPos = LbPackets.SelectedIndex - (iCount / 2);
            if (tPos < 0)
                tPos = 0;
            LbPackets.TopIndex = tPos;
        }

        public void FillListBox(uint gotToLastSync = 0)
        {
            var gotoIndex = -1;
            Application.UseWaitCursor = true;
            using (LoadingForm loadForm = new LoadingForm(OwnerMainForm))
            {
                try
                {
                    var c = loadForm.BackColor;
                    Engine.GetLoadListBoxFlavor(out var t, ref c);
                    loadForm.Text = t ;
                    loadForm.BackColor = c;

                    loadForm.Show();
                    loadForm.pb.Minimum = 0;
                    loadForm.pb.Maximum = PL.Count;
                    LbPackets.BeginUpdate();
                    LbPackets.Items.Clear();
                    for (int i = 0; i < PL.Count; i++)
                    {
                        PacketData pd = PL.GetPacket(i);
                        LbPackets.Items.Add(pd.HeaderText);
                        if ((gotoIndex < 0) && (gotToLastSync > 0) && (pd.PacketSync >= gotToLastSync))
                        {
                            gotoIndex = LbPackets.Items.Count - 1 ;
                        }
                        loadForm.pb.Value = i;
                        if ((i % 50) == 0)
                        {
                            loadForm.pb.Refresh();
                            loadForm.BringToFront();
                            Application.DoEvents();
                        }
                    }
                    LbPackets.EndUpdate();
                    if (gotoIndex >= 0)
                    {
                        LbPackets.SelectedIndex = gotoIndex;
                    }
                    loadForm.Hide();
                }
                catch
                {
                    // ignored
                }
            }
            Application.UseWaitCursor = false;
        }


        private void PmPL_Opening(object sender, CancelEventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
            {
                e.Cancel = true;
                return;
            }
            if (pd.Parent.ParentTab.Engine.HasRulesFile)
            {
                var r = pd.Parent.Rules.GetPacketRule(pd.PacketLogType, pd.StreamId, pd.PacketLevel, pd.PacketId);
                if (r != null)
                {
                    // var lookupKey = r.LookupKey + (((ulong)pd.PacketLogType) * 0x0100000000);
                    var lookupName = PacketFilterListEntry.AsString(r.PacketId, r.Level, r.StreamId);
                    PmPlShowPacketName.Text = lookupName + @" - " + r.Name; // lookupKey.ToString("X8");
                    PmPlEditParser.Tag = r ;
                    if (pd.PacketLogType != PacketLogTypes.Unknown)
                    {
                        PmPlEditParser.Text = @"Edit " + lookupName + @" => " + r.Name;
                        //PmPlEditParser.Text = @"Edit " + lookupKey.ToString("X8") + @" => " + r.Name;
                        PmPlEditParser.Visible = true;
                    }
                    else
                    {
                        PmPlEditParser.Text = @"Unknown direction, can't edit";
                        PmPlEditParser.Visible = false;
                    }
                }
                else
                {
                    PmPlShowPacketName.Text = @"No rule assigned";
                    PmPlEditParser.Tag = null;
                    PmPlEditParser.Text = @"Nothing to edit";
                    PmPlEditParser.Visible = false;
                }

            }
            else
            {
                PmPlShowPacketName.Text = pd.PacketLogType.ToString() + @" - 0x" + pd.PacketId.ToString("X3");
                string parserFileName;
                switch (pd.PacketLogType)
                {
                    case PacketLogTypes.Incoming:
                        parserFileName = Path.Combine("data", pd.Parent.ParentTab.Engine.EngineId, "parse", "in-0x" + pd.PacketId.ToString("X3") + ".txt");
                        PmPlEditParser.Text = @"Edit " + parserFileName;
                        PmPlEditParser.Visible = true;
                        break;
                    case PacketLogTypes.Outgoing:
                        parserFileName = Path.Combine("data", pd.Parent.ParentTab.Engine.EngineId, "parse", "out-0x" + pd.PacketId.ToString("X3") + ".txt");
                        PmPlEditParser.Text = @"Edit " + parserFileName;
                        PmPlEditParser.Visible = true;
                        break;
                    default:
                        parserFileName = "";
                        PmPlEditParser.Text = "";
                        PmPlEditParser.Visible = false;
                        break;
                }
                PmPlEditParser.Tag = parserFileName;
            }

            PmPlShowOnly.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);
            PmPlHideThis.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);
        }

        private void PmPLEditParser_Click(object sender, EventArgs e)
        {
            if (PmPlEditParser.Tag == null)
                return;
            if (PmPlEditParser.Tag is string fName)
                OwnerMainForm.OpenBasicParseEditor(fName);
            if (PmPlEditParser.Tag is PacketRule pr)
                OwnerMainForm.OpenXmlRulesParseEditor(pr);
        }

        private void PmPLShowOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);

            switch (pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    PL.Filter.Clear();
                    PL.Filter.FilterInType = FilterType.ShowPackets;
                    PL.Filter.FilterInList.Add(packetKey);
                    PL.Filter.FilterOutType = FilterType.AllowNone;
                    break;
                case PacketLogTypes.Outgoing:
                    PL.Filter.Clear();
                    PL.Filter.FilterOutType = FilterType.ShowPackets;
                    PL.Filter.FilterOutList.Add(packetKey);
                    PL.Filter.FilterInType = FilterType.AllowNone;
                    break;
                default:
                    return;
            }
            var lastSync = CurrentSync;
            if (hasShift)
                PL.HighlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLHideThis_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);

            switch (pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    if (PL.Filter.FilterInType != FilterType.HidePackets)
                    {
                        PL.Filter.FilterInType = FilterType.HidePackets;
                        PL.Filter.FilterInList.Clear();
                    }

                    PL.Filter.FilterInList.Add(packetKey);
                    break;
                case PacketLogTypes.Outgoing:
                    if (PL.Filter.FilterOutType != FilterType.HidePackets)
                    {
                        PL.Filter.FilterOutType = FilterType.HidePackets;
                        PL.Filter.FilterOutList.Clear();
                    }
                    PL.Filter.FilterOutList.Add(packetKey);
                    break;
                default:
                    return;
            }
            var lastSync = CurrentSync;
            if (hasShift)
                PL.HighlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowIncomingOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterInType == FilterType.AllowNone) || (PL.Filter.FilterInType == FilterType.HidePackets))
            {
                PL.Filter.FilterInType = FilterType.Off;
            }
            PL.Filter.FilterOutType = FilterType.AllowNone;

            var lastSync = CurrentSync;
            if (hasShift)
                PL.HighlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowOutgoingOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterOutType == FilterType.AllowNone) || (PL.Filter.FilterOutType == FilterType.HidePackets))
            {
                PL.Filter.FilterOutType = FilterType.Off;
            }
            PL.Filter.FilterInType = FilterType.AllowNone;

            var lastSync = CurrentSync;
            if (hasShift)
                PL.HighlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLResetFilter_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            PL.Filter.Clear();
            var lastSync = CurrentSync;
            PL.CopyFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLExport_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            string exportName = "";
            switch(pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    exportName += "i";
                    break;
                case PacketLogTypes.Outgoing:
                    exportName += "o";
                    break;
                default:
                    exportName += "u";
                    break;
            }
            exportName += pd.PacketLevel.ToString() + "-" + pd.PacketId.ToString("X4");

            using (var saveDlg = new SaveFileDialog())
            {
                saveDlg.FileName = exportName;
                saveDlg.CheckPathExists = true;
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;
                exportName = saveDlg.FileName;
            }

            try
            {
                File.WriteAllBytes(exportName,pd.RawBytes.ToArray());
            }
            catch (Exception x)
            {
                MessageBox.Show($"Error saving raw packet {exportName}\r\nException: {x.Message}");
            }

        }

        public void ClearProjectFile()
        {
            LoadedLogFile = string.Empty;
            LinkVideoFileName = string.Empty;
            LinkVideoOffset = TimeSpan.Zero;
            LinkYoutubeUrl = string.Empty;
            LinkPacketsDownloadUrl = string.Empty;
            LoadedRulesFile = string.Empty;
        }

        public bool LoadProjectFile(Stream projectStream)
        {
            ClearProjectFile();
            try
            {
                var sl = new List<string>();
                using (var ps = new StreamReader(projectStream))
                {
                    while(!ps.EndOfStream)
                    {
                        sl.Add(ps.ReadLine());
                    }
                }
                    

                foreach (string s in sl)
                {
                    var fields = s.Split(';');
                    if (fields.Length < 2)
                        continue;
                    var fieldType = fields[0].ToLower();
                    var fieldVal = fields[1];
                    if (fieldType == "packetlog")
                    {
                        LoadedLogFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "rules")
                    {
                        LoadedRulesFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "video")
                    {
                        LinkVideoFileName = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "youtube")
                    {
                        LinkYoutubeUrl = fieldVal;
                    }
                    else
                    if (fieldType == "offset")
                    {
                        if (DataLookups.TryFieldParse(fieldVal, out int n))
                            LinkVideoOffset = TimeSpan.FromMilliseconds(n);
                    }
                    else
                    if (fieldType == "packedsource")
                    {
                        LinkPacketsDownloadUrl = fieldVal;
                    }
                    else
                    if (fieldType == "tags")
                    {
                        ProjectTags = fieldVal;
                    }
                    else
                    if (fieldType == "decrypt")
                    {
                        DecryptVersion = fieldVal;
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
                ClearProjectFile();
                return false;
            }
            return true;
        }

        public bool LoadProjectFile(string aProjectFile)
        {
            ClearProjectFile();
            ProjectFolder = Path.GetDirectoryName(aProjectFile)?.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var res = false;
            try
            {
                if (File.Exists(aProjectFile))
                {
                    using (var projectStream = new FileStream(aProjectFile, FileMode.Open, FileAccess.Read))
                    {
                        res = LoadProjectFile(projectStream);
                    }
                }
            }
            catch
            {
                ClearProjectFile();
                res = false;
            }
            if (res)
                ProjectFile = aProjectFile;
            return res ;
        }

        public bool LoadProjectFileFromLogFile(string fromLogFile)
        {
            if (!string.IsNullOrEmpty(fromLogFile))
                ProjectFolder = Helper.MakeProjectDirectoryFromLogFileName(fromLogFile);

            var aProjectFile = ProjectFolder + Path.GetFileName(ProjectFolder.TrimEnd('\\')) + ".pvd";

            return LoadProjectFile(aProjectFile);
        }

        public bool SaveProjectFile()
        {
            if ((ProjectFolder == null) || (ProjectFolder == string.Empty))
                return false;

            // Generate Filename if needed
            if ((ProjectFile == null) || (ProjectFile == string.Empty))
            {
                var partialProjectFileName = Path.GetFileName(ProjectFolder.TrimEnd('\\'));
                // don't create in a drive root directory
                if (partialProjectFileName == string.Empty)
                    return false;
                ProjectFile = ProjectFolder + partialProjectFileName + ".pvd";
            }

            if ( (Properties.Settings.Default.AskCreateNewProjectFile) && (!File.Exists(ProjectFile)) )
            {
                if (MessageBox.Show($"Do you want to save project settings as a new project file ?\r\n{ProjectFile}", 
                        @"Create Project File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    ProjectFile = string.Empty;
                    return false;
                }
            }

            var relVideo = string.Empty;
            if (!string.IsNullOrEmpty(LinkVideoFileName))
                relVideo = Helper.MakeRelative(ProjectFolder, LinkVideoFileName);

            var relLogFile = string.Empty;
            if (!string.IsNullOrEmpty(LoadedLogFile))
                relLogFile = Helper.MakeRelative(ProjectFolder, LoadedLogFile);

            var relRuleFile = string.Empty;
            if (!string.IsNullOrEmpty(LoadedRulesFile))
                relRuleFile = Helper.MakeRelative(ProjectFolder, LoadedRulesFile);

            try
            {
                var pin = string.Empty;
                foreach(var n in PLLoaded.ContainsPacketsIn)
                {
                    if (pin != string.Empty)
                        pin += ",";
                    pin += n.ToString("X3");
                }

                var pout = string.Empty;
                foreach (var n in PLLoaded.ContainsPacketsOut)
                {
                    if (pout != string.Empty)
                        pout += ",";
                    pout += n.ToString("X3");
                }

                var sl = new List<string>
                {
                    "rem;VieweD Project File",
                    "packetlog;" + relLogFile,
                    "rules;" + relRuleFile,
                    "decrypt;" + DecryptVersion,
                    "video;" + relVideo,
                    "tags;" + ProjectTags,
                    "packedsource;" + LinkPacketsDownloadUrl,
                    "youtube;" + LinkYoutubeUrl,
                    "offset;" + LinkVideoOffset.TotalMilliseconds,
                    "pin;" + pin,
                    "pout;" + pout
                };

                File.WriteAllLines(ProjectFile, sl);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
