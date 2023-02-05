using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SQLitePCL;
using VieweD.Engine.Common;
using System.Net.Sockets;

namespace VieweD.Engine.pcapraw
{
    // This class is only instantiated using Activator.CreateInstance()
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    // ReSharper disable once InconsistentNaming
    public class EnginePCapRaw : EngineBase
    {
        public const string ThisEngineId = "pcapraw";
        public override string EngineId { get; } = ThisEngineId;
        public override string EngineName { get; } = "PCAP Raw";
        public override bool HasRulesFile { get; } = true;
        public override bool HasDecrypt { get; } = false;
        public override bool AllowedPacketSyncSearch { get; } = false;
        public override bool AllowedPacketLevelSearch { get; } = false;
        public override ushort PacketIdMaximum { get; } = 0xFFFF;

        private string _assumedLocalIp = string.Empty;

        private PacketList _currentPakList;

        public EnginePCapRaw()
        {
            ParentTab = null;
            InitEngine();
        }

        public EnginePCapRaw(PacketTabPage parent)
        {
            ParentTab = parent;
            InitEngine();
        }

        private void InitEngine()
        {
            FileExtensions = new Dictionary<string, string>
            {
                { ".pcap", "PCAP File" },
                { ".pcapng", "PCAPng File" }
            };

            EditorDataTypes.Clear();
            EditorDataTypes.Add("uint16");
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
                case 1:
                    text = "Checking Pretty Captures And Packets, please wait";
                    break;
                default:
                    text = "Populating Listbox, please wait ...";
                    break;
            }
        }

        public override void Init()
        {
            base.Init();
        }

        #region editor_stuff
        public override void BuildEditorPopupMenu(ToolStripMenuItem miInsert, ParseEditorForm editor)
        {
            base.BuildEditorPopupMenu(miInsert, editor);

            var basic = editor.AddMenuItem(miInsert.DropDownItems, "Basic Types", "");
            var complex = editor.AddMenuItem(miInsert.DropDownItems, "Complex Types", "");
            var funcs = editor.AddMenuItem(miInsert.DropDownItems, "Functions", "");
            var others = editor.AddMenuItem(miInsert.DropDownItems, "Other", "");

            // Basic
            editor.AddMenuItem(basic.DropDownItems, "byte (8  bit)", "<data type=\"byte\" %NAME%%LOOKUP%/>", "Byte");
            editor.AddMenuItem(basic.DropDownItems, "short (16 bit)", "<data type=\"ushort\" %NAME%%LOOKUP%/>", "UInt16 (ushort)");
            editor.AddMenuItem(basic.DropDownItems, "short (16 bit signed)", "<data type=\"short\" %NAME%%LOOKUP%/>", "Int16 (short)");
            editor.AddMenuItem(basic.DropDownItems, "-", "");
            editor.AddMenuItem(basic.DropDownItems, "uint (32 bit)", "<data type=\"uint\" %NAME%%LOOKUP%/>", "UInt32 (uint)");
            editor.AddMenuItem(basic.DropDownItems, "int (32 bit signed)", "<data type=\"int\" %NAME%%LOOKUP%/>", "Int32 (int)");
            editor.AddMenuItem(basic.DropDownItems, "uint64 (64 bit)", "<data type=\"q\" %NAME%%LOOKUP%/>", "UInt64 (ulong)");
            editor.AddMenuItem(basic.DropDownItems, "int64 (64 bit signed)", "<data type=\"int64\" %NAME%%LOOKUP%/>", "Int64 (long)");
            editor.AddMenuItem(basic.DropDownItems, "-", "");
            editor.AddMenuItem(basic.DropDownItems, "float (32 bit)", "<data type=\"f\" %NAME%%LOOKUP%/>", "Float");
            editor.AddMenuItem(basic.DropDownItems, "double (64 bit)", "<data type=\"double\" %NAME%%LOOKUP%/>", "Double");

            var strings = editor.AddMenuItem(complex.DropDownItems, "Strings", "");
            editor.AddMenuItem(strings.DropDownItems, "String with Size (ASCII)", "<data type=\"zs\" %NAME%/>", "Size+ASCII");
            editor.AddMenuItem(strings.DropDownItems, "String (ASCII)", "<data type=\"s\" arg=\"0\" %NAME%/>", "ASCII");
            editor.AddMenuItem(strings.DropDownItems, "String with Size (Unicode)", "<data type=\"zu\" %NAME%/>", "Size+Unicode");
            editor.AddMenuItem(strings.DropDownItems, "String (Unicode)", "<data type=\"u\" arg=\"0\" %NAME%/>", "Unicode");
            editor.AddMenuItem(strings.DropDownItems, "String with Size (UTF8)", "<data type=\"zu8\" %NAME%/>", "Size+UTF8");
            editor.AddMenuItem(strings.DropDownItems, "String (UTF8)", "<data type=\"t\" arg=\"0\" %NAME%/>", "UTF8");

            editor.AddMenuItem(complex.DropDownItems, "-", "");
            editor.AddMenuItem(complex.DropDownItems, "Milliseconds (UInt32)", "<data type=\"ms\" %NAME%/>", "MS");
            editor.AddMenuItem(complex.DropDownItems, "Half-Float (16 bit)", "<data type=\"half\" %NAME%%LOOKUP%/>", "Half-Float");
            editor.AddMenuItem(complex.DropDownItems, "Byte Array", "<data type=\"a\" %NAME% arg=\"16\"/>", "Array (a)");

            // Functions
            var ifs = editor.AddMenuItem(funcs.DropDownItems, "Compare", "");
            editor.AddMenuItem(ifs.DropDownItems, "If Equals (a == b)", "<ifeq arg1=\"#a\" arg2=\"#b\">\n<!-- your code -->\n</ifeq>");
            editor.AddMenuItem(ifs.DropDownItems, "If Not Equals (a != b)", "<ifneq arg1=\"#a\" arg2=\"#b\">\n<!-- your code -->\n</ifneq>");
            editor.AddMenuItem(ifs.DropDownItems, "If Less Than (a < b)", "<iflt arg1=\"#a\" arg2=\"#b\">\n<!-- your code -->\n</iflt>");
            editor.AddMenuItem(ifs.DropDownItems, "If Greater Than (a > b)", "<ifgt arg1=\"#a\" arg2=\"#b\">\n<!-- your code -->\n</ifgt>");
            editor.AddMenuItem(ifs.DropDownItems, "If Zero (a == 0)", "<ifz arg=\"#a\">\n<!-- your code -->\n</ifz>");
            editor.AddMenuItem(ifs.DropDownItems, "If Not Zero (a != 0)", "<ifnz arg=\"#a\">\n<!-- your code -->\n</ifnz>");

            var maths = editor.AddMenuItem(funcs.DropDownItems, "Math", "");
            editor.AddMenuItem(maths.DropDownItems, "Add (c = a + b)", "<add dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "Subtract (c = a - b)", "<sub dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "Multiply (c = a * b)", "<mul dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "-", "");
            editor.AddMenuItem(maths.DropDownItems, "Move/Copy/Assign (c = a)", "<mov dst=\"c\" val=\"#a\" />");
            editor.AddMenuItem(maths.DropDownItems, "-", "");
            editor.AddMenuItem(maths.DropDownItems, "Shift-Left (c = a << b)", "<shl dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "Shift-Right (c = a >> b)", "<shr dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "-", "");
            editor.AddMenuItem(maths.DropDownItems, "And (c = a && b)", "<and dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
            editor.AddMenuItem(maths.DropDownItems, "Or (c = a || b)", "<or dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");

            var loops = editor.AddMenuItem(funcs.DropDownItems, "Loops", "");
            editor.AddMenuItem(loops.DropDownItems, "Loop", "<loop>\n<!-- your code here, do not forget to add a break -->\n</loop>");
            editor.AddMenuItem(loops.DropDownItems, "Break", "<break />");
            editor.AddMenuItem(loops.DropDownItems, "Continue", "<continue />");
            editor.AddMenuItem(loops.DropDownItems, "-", "");
            editor.AddMenuItem(loops.DropDownItems, "Loop with counter", "<!-- Loop to 10 -->\n" +
                "<mov dst=\"c\" val=\"0\" />\n" +
                "<loop>\n" +
                "\t<add dst=\"c\" arg1=\"#c\" arg2=\"1\" />\n" +
                "\t<ifgt arg1=\"#c\" arg2=\"10\">\n" +
                "\t\t<break />\n" +
                "\t</ifgt>\n" +
                "\t<!-- your code here -->\n" +
                "</loop>");

            editor.AddMenuItem(funcs.DropDownItems, "Save Lookup", "<lookup save=\"NewSavedField\" source=\"sourceIdFieldName\" val=\"valueToSafeFieldName\" />");
            editor.AddMenuItem(funcs.DropDownItems, "Save Lookup with Value-Lookup", "<lookup save=\"NewSavedField\" source=\"sourceIdFieldName\" val=\"valueToSafeFieldName\" altlookup=\"referenceLookupFieldForValue\" />");

            // Lookup tags
            var ltags = editor.AddMenuItem(miInsert.DropDownItems, "Insert Lookup Tag", "");
            foreach (var item in editor.CurrentTab.Engine.DataLookups.LookupLists)
                editor.AddMenuItem(ltags.DropDownItems, item.Key, "lookup=\"" + item.Key + "\" ");


            // Templates in rule file
            if (editor.LoadedRule != null)
            {
                var tpls = editor.AddMenuItem(miInsert.DropDownItems, "Templates", "");
                foreach (var t in editor.LoadedRule.Parent.Parent.Templates)
                    editor.AddMenuItem(tpls.DropDownItems, t.Key, "<template name=\"" + t.Key + "\" />");
            }

            editor.AddMenuItem(others.DropDownItems, "Echo", "<echo arg=\"#a\" />");
        }

        public override string EditorReplaceString(string source, string posField, string nameField, string lookupField, string commentField)
        {
            // posfield is not used for AA rules
            if (!string.IsNullOrWhiteSpace(nameField))
                source = source.Replace("%NAME%", "name=\"" + nameField + "\" ");
            else
                source = source.Replace("%NAME%", "");

            if (!string.IsNullOrWhiteSpace(lookupField))
                source = source.Replace("%LOOKUP%", "lookup=\"" + lookupField + "\" ");
            else
                source = source.Replace("%LOOKUP%", "");

            if (!string.IsNullOrWhiteSpace(commentField))
                source = source.Replace("%COMMENT%", "comment=\"" + commentField + "\" ");
            else
                source = source.Replace("%COMMENT%", "");

            return source;
        }
        #endregion

        #region compilers
        public override bool CompileData(PacketData packetData, string packetLogFileFormats)
        {
            return base.CompileData(packetData, packetLogFileFormats);
        }

        private string CreateHeaderText(PacketData packetData, string appendText)
        {
            var ts = "";
            if (packetData.TimeStamp.Ticks > 0)
                ts = packetData.TimeStamp.ToString("HH:mm:ss");

            if (packetData.TimeStamp.Ticks == 0)
                ts = "";

            string s;
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

            s = ts + "  " + s;
            if (!string.IsNullOrWhiteSpace(appendText))
                s += appendText;
            return s;
        }

        public override void CompileSpecial(PacketData packetData, PacketList packetList)
        {
            base.CompileSpecial(packetData, packetList);
        }

        public override PacketParser GetParser(PacketData packetData)
        {
            var rule = packetData.Parent.Rules.GetPacketRule(packetData.PacketLogType, packetData.StreamId, packetData.PacketLevel, packetData.PacketId);
            return new PCapRawPacketParser(rule);
        }
        #endregion
        
        #region settings
        public override EngineSettingsTab CreateSettingsTab(TabControl parent)
        {
            var newTab = new PCapRawSettingsTab(parent) { Text = EngineName };

            return newTab;
        }
        #endregion

        #region loaders

        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var pd = new PacketData(_currentPakList);

            var rawPacket = e.GetPacket();
            if (rawPacket == null)
                return;

            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            if (packet == null) 
                return;

            var ethernetPacket = packet.Extract<EthernetPacket>();

            // We're only interested in Ethernet base captures
            if (ethernetPacket == null)
                return;

            var tcpPacket = packet.Extract<TcpPacket>();
            var udpPacket = packet.Extract<UdpPacket>();

            if ((tcpPacket == null) && (udpPacket == null))
                return;

            if ((tcpPacket != null) && (tcpPacket.PayloadData.Length > 0))
            {
                pd.RawBytes.AddRange(tcpPacket.PayloadData);
            }
            else
            if ((udpPacket != null) && (udpPacket.PayloadData.Length > 0))
            {
                pd.RawBytes.AddRange(udpPacket.PayloadData);
            }
            else
            {
                // Neither TCP or UDP packet/data, so probably not for us
                return;
            }
            pd.RawText.Add(packet.PrintHex());

            // pd.RawBytes.AddRange(thePacket.PayloadData.ToArray());
            pd.HeaderText = "#" + _currentPakList.PacketDataList.Count;
            pd.OriginalHeaderText = pd.HeaderText;
            pd.PacketSync = (ushort)(_currentPakList.Count % 0x10000);
            pd.TimeStamp = rawPacket.Timeval.Date;


            // ---------------------

            // Get IPvX packet data
            var ipv4Packet = packet.Extract<IPv4Packet>();
            var ipv6Packet = packet.Extract<IPv6Packet>();

            if ((ipv4Packet == null) && (ipv6Packet == null))
                return; // No IPv4 or IPv6 data, I don't know how to handle this

            var sourcePort = tcpPacket?.SourcePort ?? (udpPacket?.SourcePort ?? 0);
            var destinationPort = tcpPacket?.DestinationPort ?? (udpPacket?.DestinationPort ?? 0);

            pd.PacketId = 0;
            pd.PacketDataSize = (uint)pd.RawBytes.Count;
            pd.PacketSync = 0xFFFF;
            pd.TimeStamp = e.Header.Timeval.Date;

            var appendText = "";
            if (tcpPacket != null)
                appendText += "TCP ";
            if (udpPacket != null)
                appendText += "UDP ";

            var sourceIp = "";
            var destIp = "";
            if (ipv4Packet != null)
            {
                appendText += "IPv4 " +
                             ipv4Packet.SourceAddress + ":" + sourcePort + " => " +
                             ipv4Packet.DestinationAddress + ":" + destinationPort;
                sourceIp = ipv4Packet.SourceAddress.ToString();
                destIp = ipv4Packet.DestinationAddress.ToString();
                pd.PacketSync = ipv4Packet.Id;
            }
            if (ipv6Packet != null)
            {
                appendText += "IPv6 " +
                             ipv6Packet.SourceAddress + " : " + sourcePort + " => " +
                             ipv6Packet.DestinationAddress + " : " + destinationPort;
                sourceIp = ipv6Packet.SourceAddress.ToString();
                destIp = ipv6Packet.DestinationAddress.ToString();
                pd.PacketSync = (ushort)(ipv6Packet.FlowLabel % 0x10000); // truncate it, should be good enough
            }

            // Guess which machine is our "local/client" side
            //var fromMac = ethernetPacket.SourceHardwareAddress.ToString();
            //var toMac = ethernetPacket.DestinationHardwareAddress.ToString();
            if (string.IsNullOrWhiteSpace(_assumedLocalIp))
            {
                if (PortToStreamIdMapping.TryGetValue(sourcePort, out _))
                {
                    // If source port is in the list of known ports, assume it's the server, instead of the client
                    _assumedLocalIp = destIp;
                }
                else
                {
                    _assumedLocalIp = sourceIp;
                }
            }

            PacketLogTypes packetLogType;
            if (_assumedLocalIp == sourceIp)
                packetLogType = PacketLogTypes.Outgoing;
            else if (_assumedLocalIp == destIp)
                packetLogType = PacketLogTypes.Incoming;
            else
                packetLogType = PacketLogTypes.Unknown;

            pd.PacketLogType = packetLogType;
            var relatedPort = packetLogType == PacketLogTypes.Outgoing ? destinationPort : sourcePort;
            pd.StreamId = ParentTab.Engine?.GetExpectedStreamIdByPort(relatedPort, pd.StreamId) ?? pd.StreamId;


            // Grab name from rules (if any)
            if (pd.Parent?.Rules != null)
                appendText += " " + pd.Parent?.Rules?.GetPacketRule(pd.PacketLogType, pd.StreamId, pd.PacketLevel, pd.PacketId)?.Name ?? "";

            pd.HeaderText = CreateHeaderText(pd, appendText);

            _currentPakList.PacketDataList.Add(pd);
        }

        public override bool LoadFromStream(PacketList packetList, Stream fileStream, string sourceFileName, string rulesFileName, string decryptVersion)
        {
            packetList.IsPreParsed = Engines.PreParseData;
            _assumedLocalIp = string.Empty;

            using (var loadForm = new LoadingForm(MainForm.ThisMainForm))
            {
                try
                {
                    loadForm.Text = "Loading pcap file";
                    loadForm.Show();
                    loadForm.pb.Minimum = 0;
                    loadForm.pb.Maximum = (int)fileStream.Length;
                    loadForm.pb.Step = 1;

                    var pCapRules = CreateRulesReader(packetList.ParentTab);
                    if (pCapRules == null)
                        throw new Exception("Error creating PCapRulesReader");
                    packetList.Rules = pCapRules;

                    packetList.Rules = pCapRules;
                    var streamIdMapping = new Dictionary<byte, byte>();
                    if (!packetList.Rules.LoadRulesFromFile(rulesFileName))
                    {
                        MessageBox.Show("Failed to load rules file, or no rules found: " + rulesFileName,
                            "Error loading Rules", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // return false;
                    }

                    _currentPakList = packetList;
                    using (var device = new CaptureFileReaderDevice(sourceFileName))
                    {
                        device.Open();
                        device.OnPacketArrival += Device_OnPacketArrival;
                        device.Capture();
                    }
                    _currentPakList = null;

                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(ex.Message);
                    return false;
                }
            }

        }
        #endregion
        public override void RunTool(PacketTabPage currentTabPage, string toolName)
        {
            base.RunTool(currentTabPage, toolName);
        }

        public override RulesReader CreateRulesReader(PacketTabPage parent)
        {
            return new RulesReader(parent);
        }
    }
    
}
