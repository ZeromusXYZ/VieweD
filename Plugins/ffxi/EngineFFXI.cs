using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Data.Sqlite;
using VieweD.Engine.Common;

namespace VieweD.Engine.FFXI
{
    // This class is only instantiated using Activator.CreateInstance()
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    // ReSharper disable once InconsistentNaming
    public class EngineFFXI : EngineBase
    {
        public const string ThisEngineId = "ffxi";
        public override string EngineId { get; } = ThisEngineId;
        public override string EngineName { get; } = "Final Fantasy XI";
        public override bool HasRulesFile { get; } = false ;
        public override bool HasDecrypt { get; } = false;

        #region LookupConstants
        public static readonly string LU_Zones = "zones";
        public static readonly string LU_EquipmentSlots = "equipslot";
        public static readonly string LU_Container = "containers";
        public static readonly string LU_Item = "items";
        public static readonly string LU_ItemModel = "itemmodels";
        public static readonly string LU_Music = "music";
        public static readonly string LU_Job = "jobs";
        public static readonly string LU_Weather = "weather";
        public static readonly string LU_Merit = "merits";
        public static readonly string LU_JobPoint = "jobpoints";
        public static readonly string LU_Spell = "spells";
        public static readonly string LU_WeaponSkill = "weaponskill";
        public static readonly string LU_Ability = "ability";
        public static readonly string LU_ARecast = "abilityrecast";
        public static readonly string LU_PetCommand = "petcommand";
        public static readonly string LU_Trait = "trait";
        public static readonly string LU_Mounts = "mounts";
        public static readonly string LU_RoE = "roe";
        public static readonly string LU_CraftRanks = "craftranks";
        public static readonly string LU_Buffs = "buffs";
        public static readonly string LU_ActionCategory0x028 = "actioncategory0x028";
        public static readonly string LU_ActionCategory = "actioncategory";
        public static readonly string LU_ActionReaction = "actionreaction";
        public static readonly string LU_Dialog = "dialog"; // Key = zoneId * 0x10000 + dialogId
        #endregion

        public static readonly DataLookupListFfxiSpecialItems ItemsListFfxi = new DataLookupListFfxiSpecialItems();
        public static readonly DataLookupListFfxiSpecialDialog DialogsListFfxi = new DataLookupListFfxiSpecialDialog();
        
        private static string ToolImportGame = "FFXI: Import from game" ;
        
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=netframework-4.7.2#System_DateTime_Parse_System_String_System_IFormatProvider_System_Globalization_DateTimeStyles_
        // Assume a date and time string formatted for the fr-FR culture is the local 
        // time and convert it to UTC.
        // dateString = "2008-03-01 10:00";
        public CultureInfo cultureForDateTimeParse = CultureInfo.CreateSpecificCulture("fr-FR"); // French seems to best match for what we need here
        public DateTimeStyles stylesForDateTimeParse = DateTimeStyles.AssumeLocal;
        

        public EngineFFXI()
        {
            ParentTab = null;
            InitEngine();
        }

        public EngineFFXI(PacketTabPage parent)
        {
            ParentTab = parent;
            InitEngine();
        }

        private void InitEngine()
        {
            FileExtensions = new Dictionary<string, string>
            {
                { ".log", "PacketViewer File" },
                { ".txt", "Packeteer File" },
                { ".sqlite", "PacketDB File" }
            };

            ToolNamesList.Add(ToolImportGame);

            EditorDataTypes.Clear();
            EditorDataTypes.Add("uint16");

            EditorDataTypes.Add("uint32");
            EditorDataTypes.Add("int32");
            EditorDataTypes.Add("int16");
            EditorDataTypes.Add("byte");
            EditorDataTypes.Add("float");
            EditorDataTypes.Add("pos");
            EditorDataTypes.Add("dir");
            EditorDataTypes.Add("switchblock");
            EditorDataTypes.Add("showblock");
            EditorDataTypes.Add("info");
            EditorDataTypes.Add("bit");
            EditorDataTypes.Add("bits");
            EditorDataTypes.Add("string");
            EditorDataTypes.Add("data");
            EditorDataTypes.Add("ms");
            EditorDataTypes.Add("frames");
            EditorDataTypes.Add("vanatime");
            EditorDataTypes.Add("ip4");
            EditorDataTypes.Add("linkshellstring");
            EditorDataTypes.Add("inscribestring");
            EditorDataTypes.Add("bitflaglist");
            EditorDataTypes.Add("bitflaglist2");
            EditorDataTypes.Add("combatskill");
            EditorDataTypes.Add("craftskill");
            EditorDataTypes.Add("equipsetitem");
            EditorDataTypes.Add("equipsetitemlist");
            EditorDataTypes.Add("abilityrecastlist");
            EditorDataTypes.Add("blacklistentry");
            EditorDataTypes.Add("meritentries");
            EditorDataTypes.Add("playercheckitems");
            EditorDataTypes.Add("bufficons");
            EditorDataTypes.Add("bufftimers");
            EditorDataTypes.Add("buffs");
            EditorDataTypes.Add("jobpointentries");
            EditorDataTypes.Add("shopitems");
            EditorDataTypes.Add("guildshopitems");
            EditorDataTypes.Add("jobpoints");
            EditorDataTypes.Add("roequest");
            EditorDataTypes.Add("packet-in-0x028");
        }

        public override bool CanAppend(PacketTabPage project)
        {
            return (project == null) || (project.PLLoaded.LoadedLogFileFormat != "PacketDB");
        }

        public override void GetLoadListBoxFlavor(out string text, ref Color color)
        {
            Random rand = new Random();
            switch (rand.Next(100))
            {
                case 0:
                    color = Color.DarkRed;
                    text = "Sacrificing Taru-Taru's ...";
                    break;
                case 1:
                    text = "That's a lot of data ...";
                    break;
                case 2:
                    text = "Burning circles, please wait ...";
                    break;
                case 3:
                    text = "I'm bored ...";
                    break;
                case 4:
                    text = "Camping Shikigami Weapon, come back tomorrow ...";
                    break;
                default:
                    text = "Populating Listbox, please wait ...";
                    break;
            }
        }

        public override void Init()
        {
            base.Init();
            
            if (FFXIHelper.FindPaths())
                LoadDataFromFFXIGameclient(DataLookups);

            // Replace lookups with gamedata if enabled
            if (ItemsListFfxi.Items.Count > 0)
            {
                DataLookups.LookupLists.Remove(LU_Item);
                DataLookups.LookupLists.Add(LU_Item, ItemsListFfxi);
            }
            if (DialogsListFfxi.Data.Count > 0)
            {
                DataLookups.LookupLists.Remove(LU_Dialog);
                DataLookups.LookupLists.Add(LU_Dialog, DialogsListFfxi);
            }
        }

        #region editor_stuff
        public override void BuildEditorPopupMenu(ToolStripMenuItem miInsert, ParseEditorForm editor)
        {
            base.BuildEditorPopupMenu(miInsert, editor);

            var basic = editor.AddMenuItem(miInsert.DropDownItems, "Basic Types", "");
            var complex = editor.AddMenuItem(miInsert.DropDownItems, "Complex Types", "");
            var functions = editor.AddMenuItem(miInsert.DropDownItems, "Functions", "");
            var others = editor.AddMenuItem(miInsert.DropDownItems, "Other", "");

            // Basic
            editor.AddMenuItem(basic.DropDownItems, "Byte (8 bit)", "byte%LOOKUP%;%POS%;%NAME%%COMMENT%","byte");
            editor.AddMenuItem(basic.DropDownItems, "Byte (8 bit signed)", "sbyte%LOOKUP%;%POS%;%NAME%%COMMENT%", "sbyte");
            editor.AddMenuItem(basic.DropDownItems, "uint16 (16 bit)", "uint16%LOOKUP%;%POS%;%NAME%%COMMENT%", "uint16");
            editor.AddMenuItem(basic.DropDownItems, "int16  (16 bit signed)", "int16%LOOKUP%;%POS%;%NAME%%COMMENT%", "int16");
            editor.AddMenuItem(basic.DropDownItems, "-", "");
            editor.AddMenuItem(basic.DropDownItems, "Bit/Bool", "bit%LOOKUP%;%POS%:0;%NAME%%COMMENT%", "Bit");
            editor.AddMenuItem(basic.DropDownItems, "Bits", "bits%LOOKUP%;%POS%:0-8;%NAME%%COMMENT%", "Bits Array");
            editor.AddMenuItem(basic.DropDownItems, "-", "");
            editor.AddMenuItem(basic.DropDownItems, "uint32 (32 bit)", "uint32%LOOKUP%;%POS%;%NAME%%COMMENT%", "uint32");
            editor.AddMenuItem(basic.DropDownItems, "int32  (32 bit signed)", "int32%LOOKUP%;%POS%;%NAME%%COMMENT%", "int32");
            editor.AddMenuItem(basic.DropDownItems, "-", "");
            editor.AddMenuItem(basic.DropDownItems, "Float (32 bit)", "float%LOOKUP%;%POS%;%NAME%%COMMENT", "float");

            // Complex -> Strings
            var strings = editor.AddMenuItem(complex.DropDownItems, "Strings", "");
            editor.AddMenuItem(strings.DropDownItems, "String Null Terminated", "string%LOOKUP%;%POS%;%NAME%%COMMENT%", "string");
            editor.AddMenuItem(strings.DropDownItems, "String Fixed Length(16)", "string16%LOOKUP%;%POS%;%NAME%%COMMENT%", "string16");
            editor.AddMenuItem(strings.DropDownItems, "String with Size", "vstring%LOOKUP%;%POS%;%NAME%%COMMENT%", "vstring");
            editor.AddMenuItem(strings.DropDownItems, "-", "");
            editor.AddMenuItem(strings.DropDownItems, "Linkshell", "linkshellstring%LOOKUP%;%POS%;%NAME%%COMMENT%", "linkshellstring");
            editor.AddMenuItem(strings.DropDownItems, "Item Inscription", "inscribestring%LOOKUP%;%POS%;%NAME%%COMMENT%", "inscribestring");

            // Complex
            editor.AddMenuItem(complex.DropDownItems, "Byte Array(16)", "data16%LOOKUP%;%POS%;%NAME%%COMMENT%", "data");
            editor.AddMenuItem(complex.DropDownItems, "Milliseconds", "ms%LOOKUP%;%POS%;%NAME%%COMMENT%", "ms");
            editor.AddMenuItem(complex.DropDownItems, "Frames", "frames%LOOKUP%;%POS%;%NAME%%COMMENT%", "frames");
            editor.AddMenuItem(complex.DropDownItems, "Vana'diel Time", "vanatime%LOOKUP%;%POS%;%NAME%%COMMENT%", "vanatime");
            editor.AddMenuItem(complex.DropDownItems, "IPv4", "ip4%LOOKUP%;%POS%;%NAME%%COMMENT%", "ip4");
            editor.AddMenuItem(complex.DropDownItems, "-", "");
            editor.AddMenuItem(complex.DropDownItems, "Bit-flags Multi-Line", "bitflaglist%LOOKUP%;%POS%:0-64;%NAME%%COMMENT%", "bitflaglist");
            editor.AddMenuItem(complex.DropDownItems, "Bit-flags Single-Line", "bitflaglist2%LOOKUP%;%POS%:0-64;%NAME%%COMMENT%", "bitflaglist2");
            editor.AddMenuItem(complex.DropDownItems, "-", "");
            editor.AddMenuItem(complex.DropDownItems, "Position", "pos%LOOKUP%;%POS%;%NAME%%COMMENT%", "pos");
            editor.AddMenuItem(complex.DropDownItems, "Direction", "dir%LOOKUP%;%POS%;%NAME%%COMMENT%", "dir");

            // Functions
            editor.AddMenuItem(functions.DropDownItems, "Save Lookup", "save;@targetfield;source id;source value");
            editor.AddMenuItem(functions.DropDownItems, "-", "");
            editor.AddMenuItem(functions.DropDownItems, "Switch Block (if)", "switchblock;%POS%;value;blockname");
            editor.AddMenuItem(functions.DropDownItems, "Code Block", "[[blockname]]\n" +
                "rem;your code here\n" +
                "[[]]\n" +
                "\n");
            editor.AddMenuItem(functions.DropDownItems, "Show Block", "showblock;0;;blockname");
            editor.AddMenuItem(functions.DropDownItems, "-", "");
            editor.AddMenuItem(functions.DropDownItems, "Enable Code", "enablecode;1");
            editor.AddMenuItem(functions.DropDownItems, "Disable Code", "enablecode;0");

            // Lookup Tags
            var ltags = editor.AddMenuItem(miInsert.DropDownItems, "Insert Lookup Tag", "");
            foreach (var item in editor.CurrentTab.Engine.DataLookups.LookupLists)
                editor.AddMenuItem(ltags.DropDownItems, item.Key, ":" + item.Key);

            // Others
            editor.AddMenuItem(others.DropDownItems, "Comment", "rem;Comment");
            editor.AddMenuItem(others.DropDownItems, "-", "");
            editor.AddMenuItem(others.DropDownItems, "Combat Skill", "combatskill%LOOKUP%;%POS%;%NAME%%COMMENT%", "combatskill");
            editor.AddMenuItem(others.DropDownItems, "Craft Skill", "craftskill%LOOKUP%;%POS%;%NAME%%COMMENT%", "craftskill");
            editor.AddMenuItem(others.DropDownItems, "Equip Set Item", "equipsetitem%LOOKUP%;%POS%;%NAME%%COMMENT%", "equipsetitem");
            editor.AddMenuItem(others.DropDownItems, "Equip Set Item List", "equipsetitemlist%LOOKUP%;%POS%;%NAME%%COMMENT%", "equipsetitemlist");
            editor.AddMenuItem(others.DropDownItems, "Ability Recast List", "abilityrecastlist%LOOKUP%;%POS%;%NAME%%COMMENT%", "abilityrecastlist");

            editor.AddMenuItem(others.DropDownItems, "Blacklist Entry", "blacklistentry%LOOKUP%;%POS%;%NAME%%COMMENT%", "blacklistentry");
            editor.AddMenuItem(others.DropDownItems, "Merit Entries", "meritentries%LOOKUP%;%POS%:$04;%NAME%%COMMENT%", "meritentries");

            editor.AddMenuItem(others.DropDownItems, "Player Check Items", "playercheckitems%LOOKUP%;%POS%:$08;%NAME%%COMMENT%", "playercheckitems");
            editor.AddMenuItem(others.DropDownItems, "Buffs", "buffs%LOOKUP%;%POS%:32;%NAME%%COMMENT%", "buffs");
            editor.AddMenuItem(others.DropDownItems, "Job Point Entries", "jobpointentries%LOOKUP%;%POS%:20;%NAME%%COMMENT%", "jobpointentries");
            editor.AddMenuItem(others.DropDownItems, "Shop Items", "shopitems%LOOKUP%;%POS%:32;%NAME%%COMMENT%", "shopitems");
            editor.AddMenuItem(others.DropDownItems, "Guild Shop Items", "guildshopitems%LOOKUP%;%POS%;%NAME%%COMMENT%", "guildshopitems");
            editor.AddMenuItem(others.DropDownItems, "Job Points", "jobpoints%LOOKUP%;%POS%;%NAME%%COMMENT%", "jobpoints");
            editor.AddMenuItem(others.DropDownItems, "RoE Quest", "roequest%LOOKUP%;%POS%;%NAME%%COMMENT%", "roequest");
            editor.AddMenuItem(others.DropDownItems, "Dialog Text", "dialogtext%LOOKUP%;%POS%;%NAME%%COMMENT%", "dialogtext");
            editor.AddMenuItem(others.DropDownItems, "Fish Rank Server Message", "fishrankservermessage%LOOKUP%;%POS%;%NAME%%COMMENT%", "fishrankservermessage");
            editor.AddMenuItem(others.DropDownItems, "Packet in-0x028", "packet-in-0x028;0;%NAME%%COMMENT%", "Data");

        }

        public override string EditorReplaceString(string source, string posField, string nameField, string lookupField, string commentField)
        {
            source = source.Replace("%NAME%", !string.IsNullOrWhiteSpace(nameField) ? nameField : "");

            source = source.Replace("%POS%", !string.IsNullOrWhiteSpace(posField) ? posField : "0x04");

            source = !string.IsNullOrWhiteSpace(lookupField) ? source.Replace("%LOOKUP%", ":" + lookupField) : source.Replace("%LOOKUP%", "");

            source = source.Replace("%COMMENT%", !string.IsNullOrWhiteSpace(commentField) ? commentField : "");

            return source;
        }
        #endregion

        #region compilers
        public override bool CompileData(PacketData packetData, string packetLogFileFormats)
        {
            if (packetData.RawBytes.Count < 4)
            {
                packetData.PacketId = 0xFFFF;
                packetData.PacketDataSize = 0;
                packetData.HeaderText = "Invalid Packet Size < 4";
                return false;
            }
            packetData.PacketId = (UInt16)(packetData.GetByteAtPos(0) + ((packetData.GetByteAtPos(1) & 0x01) * 0x100));
            packetData.PacketDataSize = (UInt16)((packetData.GetByteAtPos(1) & 0xFE) * 2);
            packetData.PacketSync = (UInt16)(packetData.GetByteAtPos(2) + (packetData.GetByteAtPos(3) * 0x100));
            var ts = "";
            if (packetData.TimeStamp.Ticks > 0)
                ts = packetData.TimeStamp.ToString("HH:mm:ss");
            if (packetLogFileFormats == "AshitaPacketeer")
            {
                // Packeteer doesn't have time info (yet)
                packetData.TimeStamp = new DateTime(0);
                packetData.OriginalTimeString = "0000-00-00 00:00";
            }
            else
            if (packetLogFileFormats == "WindowerPacketViewer")
            {
                // Try to determine timestamp from header
                var p1 = packetData.OriginalHeaderText.IndexOf('[');
                var p2 = packetData.OriginalHeaderText.IndexOf(']');
                if ((p1 >= 0) && (p2 >= 0) && (p2 > p1))
                {
                    packetData.OriginalTimeString = packetData.OriginalHeaderText.Substring(p1 + 1, p2 - p1 - 1);
                    if (packetData.OriginalTimeString.Length > 0)
                    {
                        try
                        {
                            // try quick-parse first
                            packetData.TimeStamp = packetData.DateTimeParse(packetData.OriginalTimeString, out var dt) ? dt : DateTime.Parse(packetData.OriginalTimeString, cultureForDateTimeParse, stylesForDateTimeParse);
                            ts = packetData.TimeStamp.ToString("HH:mm:ss");
                        }
                        catch (FormatException)
                        {
                            packetData.TimeStamp = new DateTime(0);
                            ts = "";
                            packetData.OriginalTimeString = "0000-00-00 00:00";
                        }
                    }
                }
            }
            packetData.VirtualTimeStamp = packetData.TimeStamp;
            if (packetData.TimeStamp.Ticks == 0)
                ts = "";

            var s = "";
            switch (packetData.PacketLogType)
            {
                case PacketLogTypes.Outgoing:
                    s = "OUT ";
                    break;
                case PacketLogTypes.Incoming:
                    s = "IN  ";
                    break;
                default:
                    s = "??? ";
                    break;
            }
            s = ts + "  " + s + "0x" + packetData.PacketId.ToString("X3") + " - ";
            packetData.HeaderText = s + packetData.Parent._parentTab.Engine.DataLookups.PacketTypeToString(packetData.PacketLogType, packetData.PacketId);
            return true;
        }

        public override void CompileSpecial(PacketData packetData, PacketList packetList)
        {
            try
            {
                switch (packetData.PacketLogType)
                {
                    case PacketLogTypes.Incoming:
                        switch (packetData.PacketId)
                        {
                            case 0x00a:
                                CompileSpecialized.In0x00a(packetData, packetList);
                                break;
                            case 0x032:
                                CompileSpecialized.In0x032(packetData, packetList);
                                break;
                            case 0x034:
                                CompileSpecialized.In0x034(packetData, packetList);
                                break;
                            default:
                                break;
                        }

                        break;
                    case PacketLogTypes.Outgoing:
                        switch (packetData.PacketId)
                        {
                            default:
                                break;
                        }

                        break;
                }
            }
            catch
            {
                // ignored
            }
        }

        public override PacketParser GetParser(PacketData packetData)
        {
            return new FFXIPacketParser(packetData.PacketId, packetData.PacketLogType);
        }
        #endregion
        
        #region settings
        public override EngineSettingsTab CreateSettingsTab(TabControl parent)
        {
            var newTab = new FFXISettingsTab(parent) { Text = EngineName };
                        
            if (FFXIHelper.FindPaths())
            {
                newTab.eFFXIPath.Text = FFXIHelper.FFXI_InstallationPath;
                newTab.lFFXIFileCount.Text = FFXIHelper.FFXI_FTable.Count.ToString() + @" / " + FFXIHelper.FFXI_FTable.Count.ToString();
            }
            else
            {
                newTab.eFFXIPath.Text = @"<not installed>";
                newTab.lFFXIFileCount.Text = @"no files";
            }            

            return newTab;
        }
        #endregion

        #region loaders
        private bool LoadFromSqLite3(PacketList packetList, string sqliteFileName)
        {
            packetList.IsPreParsed = Engines.PreParseData;
            int c = 0;
            using (LoadingForm loadForm = new LoadingForm(MainForm.ThisMainForm))
            {
                try
                {
                    loadForm.Text = @"Loading sqlite log file";
                    loadForm.Show();
                    loadForm.pb.Minimum = 0;
                    loadForm.pb.Maximum = 100000;
                    loadForm.pb.Step = 100;

                    using (SqliteConnection sqlConnection = new SqliteConnection("Data Source=" + sqliteFileName))
                    {
                        sqlConnection.Open();

                        const string sql = @"SELECT * FROM `packets` ORDER by PACKET_ID ASC";

                        SqliteCommand command = new SqliteCommand(sql, sqlConnection);
                        SqliteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var pd = new PacketData(packetList);
                            pd.TimeStamp = reader.GetDateTime(reader.GetOrdinal("RECEIVED_DT"));
                            pd.VirtualTimeStamp = pd.TimeStamp;
                            var dir = reader.GetInt16(reader.GetOrdinal("DIRECTION")); // 0 = in ; 1 = out
                            switch (dir)
                            {
                                case 0:
                                    pd.PacketLogType = PacketLogTypes.Incoming;
                                    break;
                                case 1:
                                    pd.PacketLogType = PacketLogTypes.Outgoing;
                                    break;
                                default:
                                    pd.PacketLogType = PacketLogTypes.Unknown;
                                    break;
                            }
                            pd.PacketId = (ushort)reader.GetInt32(reader.GetOrdinal("PACKET_TYPE"));
                            var pData = reader.GetString(reader.GetOrdinal("PACKET_DATA"));
                            pd.RawText.Add(pData);
                            pd.AddRawHexStringDataAsBytes(pData);
                            pd.CapturedZoneId = (ushort)reader.GetInt16(reader.GetOrdinal("ZONE_ID"));

                            pd.OriginalHeaderText = "PACKET_ID " + reader.GetInt64(reader.GetOrdinal("PACKET_ID")) + " , DIR " + dir.ToString() + " , TYPE " + pd.PacketId.ToString();
                            pd.OriginalTimeString = "";

                            if (pd.CompileData("PacketDB"))
                            {
                                pd.CompileSpecial(packetList);

                                if (packetList.IsPreParsed)
                                {
                                    pd.PP = pd.Parent._parentTab.Engine.GetParser(pd);
                                    pd.PP?.AssignPacket(pd);
                                    pd.PP?.ParseData("-");
                                }
                                packetList.PacketDataList.Add(pd);
                                if (pd.PacketLogType == PacketLogTypes.Outgoing)
                                {
                                    if (packetList.ContainsPacketsOut.IndexOf(pd.PacketId) < 0)
                                        packetList.ContainsPacketsOut.Add(pd.PacketId);
                                }
                                else
                                if (pd.PacketLogType == PacketLogTypes.Incoming)
                                {
                                    if (packetList.ContainsPacketsIn.IndexOf(pd.PacketId) < 0)
                                        packetList.ContainsPacketsIn.Add(pd.PacketId);
                                }
                            }

                            c++;
                            if ((c % 100) == 0)
                            {
                                loadForm.pb.PerformStep();
                                if (loadForm.pb.Value >= loadForm.pb.Maximum)
                                    loadForm.pb.Value = loadForm.pb.Minimum;
                                loadForm.pb.Refresh();
                                Application.DoEvents();
                            }
                        }

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show("Exception: " + x.Message, "Exception loading SQLite file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            packetList.LoadedLogFileFormat = "PacketDB";
            if (packetList.PacketDataList.Count > 0)
                packetList.firstPacketTime = packetList.PacketDataList[0].TimeStamp;
            return true;
        }
        
        private bool LoadFromStringList(PacketList packetList, List<string> fileData, string logFileType, PacketLogTypes preferredType)
        {
            packetList.IsPreParsed = Engines.PreParseData;
            // Add dummy blank lines to fix a bug of ignoring last packet if isn't finished by a blank line
            fileData.Add("");

            Application.UseWaitCursor = true;

            using (var loadForm = new LoadingForm(MainForm.ThisMainForm))
            {
                try
                {
                    loadForm.Text = "Loading text log file";
                    loadForm.Show();
                    loadForm.pb.Minimum = 0;
                    loadForm.pb.Maximum = fileData.Count ;
                    loadForm.pb.Step = 1000;

                    PacketData pd = null;
                    var isUndefinedPacketType = true;
                    var askForPacketType = true;
                    var hasHadDataHeader = false;
                    var pastStartOfDataMarker = false;

                    #region read_file_data_lines
                    int c = 0;
                    foreach(string s in fileData)
                    {
                        string sLower = s.ToLower().Trim(' ').Replace("\r","");
                        if ((sLower != string.Empty) && (pd == null))
                        {
                            hasHadDataHeader = true;
                            pastStartOfDataMarker = false;
                            // Begin building a new packet
                            pd = new PacketData(packetList);
                            if (sLower.IndexOf("incoming", StringComparison.InvariantCulture) >= 0)
                            {
                                pd.PacketLogType = PacketLogTypes.Incoming;
                                isUndefinedPacketType = false;
                                logFileType = "WindowerPacketViewer";
                            }
                            else
                            if (sLower.IndexOf("outgoing", StringComparison.InvariantCulture) >= 0)
                            {
                                pd.PacketLogType = PacketLogTypes.Outgoing;
                                isUndefinedPacketType = false;
                                logFileType = "WindowerPacketViewer";
                            }
                            else
                            if (sLower.IndexOf("[s->c]", StringComparison.InvariantCulture) >= 0)
                            {
                                pd.PacketLogType = PacketLogTypes.Incoming;
                                isUndefinedPacketType = false;
                                logFileType = "AshitaPacketeer";
                            }
                            else
                            if (sLower.IndexOf("[c->s]", StringComparison.InvariantCulture) >= 0)
                            {
                                pd.PacketLogType = PacketLogTypes.Outgoing;
                                isUndefinedPacketType = false;
                                logFileType = "AshitaPacketeer";
                            }
                            else
                            if (sLower.IndexOf("npc id:", StringComparison.InvariantCulture) >= 0)
                            {
                                // This is likely a npc logger log file, assume it's a incomming packet
                                pd.PacketLogType = PacketLogTypes.Incoming;
                                isUndefinedPacketType = false;
                                logFileType = "WindowerPacketViewer";
                            }
                            else
                            {
                                pd.PacketLogType = preferredType;
                            }

                            // Save last type to project tab
                            pd.Parent.LoadedLogFileFormat = logFileType;

                            if (
                                // Not a comment or empty line
                                ((s != "") && (!s.StartsWith("--"))) &&
                                // Unknown packet and we need to know ?
                                (isUndefinedPacketType && askForPacketType && (pd.PacketLogType == PacketLogTypes.Unknown))
                               )
                            {
                                askForPacketType = false;
                                // Ask for type
                                var askDlgRes = DialogResult.Cancel;
                                using (PacketTypeSelectForm askDlg = new PacketTypeSelectForm())
                                {
                                    askDlg.lHeaderData.Text = s;
                                    askDlgRes = askDlg.ShowDialog();
                                }

                                if (askDlgRes == DialogResult.Yes)
                                {
                                    preferredType = PacketLogTypes.Incoming ;
                                    isUndefinedPacketType = false;
                                    pd.PacketLogType = preferredType ;
                                }
                                else
                                if (askDlgRes == DialogResult.No)
                                {
                                    preferredType = PacketLogTypes.Outgoing;
                                    isUndefinedPacketType = false;
                                    pd.PacketLogType = preferredType;
                                }
                            }

                            pd.RawText.Add(s);
                            if (logFileType == "Unknown")
                            {
                                // We couldn't identify what type of packet this might be, and we didn't provide a type
                                // Assume the pasted data is just raw hex bytes (as string)
                                pd.HeaderText = "Clipboard";
                                pd.OriginalHeaderText = "Clipboard Data";
                                pd.AddRawHexStringDataAsBytes(s);
                            }
                            else
                            {
                                // Looks like a normal text packet, initialize the header
                                pd.HeaderText = s;
                                pd.OriginalHeaderText = s;

                                // Packeteer doesn't have any "in between" lines, so mark our header as complete
                                if (logFileType == "AshitaPacketeer")
                                    pastStartOfDataMarker = true;
                            }

                        } // end start new packet
                        else
                        if (hasHadDataHeader && (sLower != string.Empty) && (pd != null))
                        {
                            // Add line of data
                            pd.RawText.Add(s);
                            // Actual packet data starts at the 3rd line after the header
                            if ((logFileType != "AshitaPacketeer") && (pastStartOfDataMarker))
                            {
                                pd.AddRawLineAsBytes(s);
                            }
                            else
                            if ((logFileType != "AshitaPacketeer") && (!pastStartOfDataMarker))
                            {
                                // a reasonable amount of dashes line (32 chars) to mark the start of the data
                                if (sLower.IndexOf("--------------------------------", StringComparison.InvariantCulture) >= 0)
                                    pastStartOfDataMarker = true;
                            }
                            else
                            if ((logFileType == "AshitaPacketeer") && (pd.RawText.Count > 1))
                            {
                                pd.AddRawPacketeerLineAsBytes(s);
                            }
                            else
                            if (logFileType == "Unknown")
                            {
                                // Assume the pasted data is just raw hex bytes (as string)
                                pd.AddRawHexStringDataAsBytes(s);
                            }
                        }
                        else
                        if (hasHadDataHeader && (sLower == string.Empty) && (pd != null))
                        {
                            // Close this packet and add it to list
                            if (pd.CompileData(logFileType))
                            {
                                pd.CompileSpecial(packetList);
                                // Set zone after CompileSpecial, this is only needed if not captured by PacketDB
                                pd.CapturedZoneId = packetList.currentParseZone;

                                if (packetList.IsPreParsed)
                                {
                                    pd.PP = pd.Parent._parentTab.Engine.GetParser(pd);

                                    if (pd.PP != null)
                                    {
                                        pd.PP.AssignPacket(pd);
                                        pd.PP.ParseData("-");
                                    }
                                }

                                packetList.PacketDataList.Add(pd);
                                if (pd.PacketLogType == PacketLogTypes.Outgoing)
                                {
                                    if (packetList.ContainsPacketsOut.IndexOf(pd.PacketId) < 0)
                                        packetList.ContainsPacketsOut.Add(pd.PacketId);
                                }
                                else
                                if (pd.PacketLogType == PacketLogTypes.Incoming)
                                {
                                    if (packetList.ContainsPacketsIn.IndexOf(pd.PacketId) < 0)
                                        packetList.ContainsPacketsIn.Add(pd.PacketId);
                                }
                            }
                            else
                            {
                                // Invalid data
                            }
                            // reset our packet holder
                            pd = null;
                        }
                        else
                        if (string.IsNullOrWhiteSpace(sLower) && (pd == null))
                        {
                            // Blank line
                        }
                        else
                        if (sLower.StartsWith("--") && (pd != null))
                        {
                            // Comment
                        }
                        else
                        {
                            // ERROR, this should not be possible in a valid file, but just let's ignore it anyway, just in case
                        }

                        c++;
                        if ((c % 1000) == 0)
                        {
                            loadForm.pb.PerformStep();
                            loadForm.pb.Refresh();
                            loadForm.BringToFront();
                            Application.DoEvents();
                        }
                    } // end foreach datafile line
                    #endregion
                }
                catch
                {
                    Application.UseWaitCursor = false;
                    return false;
                }
            }
            if (packetList.PacketDataList.Count > 0)
                packetList.firstPacketTime = packetList.PacketDataList[0].TimeStamp;
            
            Application.UseWaitCursor = false;
            return true;
        }

        public override bool LoadFromStream(PacketList packetList, Stream fileStream, string sourceFileName, string rulesFileName, string decryptVersion)
        {
            // Check supported FFXI Log Types
            var expectedLogType = packetList.LoadedLogFileFormat;
            if (string.IsNullOrWhiteSpace(expectedLogType))
                expectedLogType = "Unknown";
            if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
                if ((expectedLogType == "Unknown") && (Path.GetExtension(sourceFileName) == ".log"))
                    expectedLogType = "WindowerPacketViewer";
                if ((expectedLogType == "Unknown") && (Path.GetExtension(sourceFileName) == ".txt"))
                    expectedLogType = "AshitaPacketeer";
                if ((expectedLogType == "Unknown") && (Path.GetExtension(sourceFileName) == ".sqlite"))
                    expectedLogType = "PacketDB";
            }
            
            if ((expectedLogType == "WindowerPacketViewer") || (expectedLogType == "AshitaPacketeer") || (expectedLogType == "Clipboard"))
            {
                try
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var allText = reader.ReadToEnd();
                        var sl = allText.Replace("\r", "").Split('\n').ToList();
                        return LoadFromStringList(packetList, sl, expectedLogType, PacketLogTypes.Unknown);
                    }
                }
                catch (Exception x)
                {
                    if (x is PathTooLongException)
                    {
                        MessageBox.Show("This program does not support file paths that are longer than MAX_PATH (260 characters by default)\r\nPlease consider shortening your directory or file names, and try again.", "Name too long", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        MessageBox.Show("Exception:\r\n" + x.Message, "LoadFromFile()", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    return false;
                }
            }
            else
            if (expectedLogType == "PacketDB")
            {
                return LoadFromSqLite3(packetList, sourceFileName);
            }
            else
            {
                return false;
            }
        }
        #endregion
        
        private void AddValuesFromMobList(DataLookupList DLL, ref Dictionary<uint, FFXI_MobListEntry> mobList)
        {
            foreach(var MLE in mobList)
            {
                if (DLL.Data.TryGetValue(MLE.Key, out _))
                    continue;
                DataLookupEntry DLE = new DataLookupEntry();
                DLE.Id = MLE.Value.Id;
                DLE.Val = MLE.Value.Name;
                DLE.Extra = MLE.Value.ExpectedZoneId.ToString();
                DLL.Data.Add(DLE.Id,DLE);
            }
        }        
        
        private void LoadDataFromFFXIGameclient(DataLookups dataLookups)
        {
            if ((Engines.UseGameClientData == false) || (!Directory.Exists(FFXIHelper.FFXI_InstallationPath)))
                return;

            // Items
            FFXIHelper.FFXI_LoadItemsFromDats(ref ItemsListFfxi.Items);
            ItemsListFfxi.UpdateData();

            // Enabled dynamic loading for dialog text
            DialogsListFfxi.EnableCache = true;

            // NPC Names
            var mobList = new Dictionary<uint, FFXI_MobListEntry>();
            mobList.Add(0, new FFXI_MobListEntry()); // Id 0 = "none"
            for (ushort z = 0; z < 0x1FF; z++)
                FFXIHelper.FFXI_LoadMobListForZone(ref mobList, z);
            AddValuesFromMobList(dataLookups.NLUOrCreate("@actors"), ref mobList);
            AddValuesFromMobList(dataLookups.NLUOrCreate("npcname"), ref mobList); // Not sure if we're ever gonna use this, but meh
        }

        public override void RunTool(string toolName)
        {
            if (toolName == ToolImportGame)
                ImportFromGame();
        }

        private void ImportFromGame()
        {
            if (!Directory.Exists(FFXIHelper.FFXI_InstallationPath))
            {
                MessageBox.Show("No FFXI Installation found to extract data from !", "No game client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("You are about to import data from \r\n" +
                FFXIHelper.FFXI_InstallationPath + "\r\n\r\n" +
                "The following lookups will be overwritten:\r\n" +
                "- items.txt",
                "Import game data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using (var loadForm = new LoadingForm(MainForm.ThisMainForm))
            {
                var engine = Engines.GetEngine("ffxi");
                loadForm.Text = "Importing ...";
                loadForm.pb.Hide();
                loadForm.lTextInfo.Show();
                loadForm.Show();

                FFXIHelper.FFXI_LoadItemsFromDats(ref EngineFFXI.ItemsListFfxi.Items);
                EngineFFXI.ItemsListFfxi.UpdateData();
                /*
                var itemFiles = Directory.GetFiles(Properties.Settings.Default.POLUtilsDataFolder, "items-*.xml");
                for(var c = 0; c < itemFiles.Length; c++)
                {
                    loadform.lTextInfo.Text = "Items " + (c + 1).ToString() + "/" + itemFiles.Length.ToString();
                    loadform.Refresh();
                    items.AddRange(SEHelper.ReadItemListFromXML(itemFiles[c]));
                    System.Threading.Thread.Sleep(250);
                }
                */
                loadForm.lTextInfo.Text = "Saving " + EngineFFXI.ItemsListFfxi.Items.Count.ToString() + " items ...";
                loadForm.Refresh();
                // var sorteditems = DataLookups.ItemsList.items.OrderBy(d => d.Value);
                var itemsString = new List<string>();
                itemsString.Add("id;name");
                foreach (var item in EngineFFXI.ItemsListFfxi.Items)
                {
                    if ((item.Value.Id > 0) && (item.Value.Name != string.Empty) && (item.Value.Name != "."))
                    {
                        itemsString.Add(item.Value.Id.ToString() + ";" + item.Value.Name);
                    }
                }

                File.WriteAllLines(Path.Combine(DataLookups.DefaultLookupPath(EngineId), "items.txt"),
                    itemsString);
                System.Threading.Thread.Sleep(500);

                loadForm.lTextInfo.Text = "Reloading lookups ...";
                loadForm.Refresh();
                DataLookups.LoadLookups(EngineId, false);
            }
        }
        
    }
    
}
