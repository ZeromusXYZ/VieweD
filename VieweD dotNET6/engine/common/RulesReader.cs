using System.Text;
using System.Xml;
using VieweD.Forms;
using VieweD.Helpers.System;
using VieweD.Properties;

namespace VieweD.engine.common;

public class RulesReader
{
    public XmlDocument? XmlDoc { get; set; }
    public XmlNodeList? AllRulesGroups { get; set; }
    public XmlNodeList? AllTemplates { get; set; }
    public Dictionary<byte, RulesGroup> RuleGroups { get; set; } = new Dictionary<byte, RulesGroup>();
    public Dictionary<uint, PacketRule> C2S { get; set; } = new Dictionary<uint, PacketRule>(); // out: client to server
    public Dictionary<uint, PacketRule> S2C { get; set; } = new Dictionary<uint, PacketRule>(); // In : server to client
    public Dictionary<string, XmlNode> Templates { get; set; } = new Dictionary<string, XmlNode>();
    public string LoadedRulesFileName { get; protected set; } = string.Empty;
    public ViewedProjectTab ParentProject { get; private set; }
    // protected ZlibCodec DecompressionHandler { get; set; } = new ZlibCodec(Ionic.Zlib.CompressionMode.Decompress);
    public string ExpectedClientVersion { get; set; } = string.Empty;
    public bool UsesCompressionLevels { get; internal set; }
    public bool UsesMultipleStreams { get; private set; }

    public RulesReader(ViewedProjectTab parent)
    {
        ParentProject = parent;
    }

    protected virtual bool LoadRulesFromXmlString(string xmlData)
    {
        LoadedRulesFileName = "";
        // Open XML file
        XmlDoc = new XmlDocument();
        XmlDoc.Load(new StringReader(xmlData));

        var versionNode = XmlDoc.SelectSingleNode("/root/version");
        if (versionNode != null)
        {
            var client = XmlHelper.GetAttributeString(XmlHelper.ReadNodeAttributes(versionNode), "client");
            ExpectedClientVersion = client;
        }
        
        // Read and Save template nodes in a List
        AllTemplates = XmlDoc.SelectNodes("/root/templates/template");
        Templates.Clear();
        if (AllTemplates != null)
            for (var i = 0; i < AllTemplates.Count; i++)
            {
                var node = AllTemplates.Item(i);
                if (node == null)
                    continue;
                var name = XmlHelper.GetAttributeString(XmlHelper.ReadNodeAttributes(node), "name");
                Templates.Add(name, node);
            }

        // Locate rule sections
        AllRulesGroups = XmlDoc.SelectNodes("/root/rule");
        S2C.Clear();
        C2S.Clear();

        if (AllRulesGroups != null)
        {
            UsesMultipleStreams = AllRulesGroups.Count > 1;
            RuleGroups.Clear();
            if (AllRulesGroups != null)
                for (var i = 0; i < AllRulesGroups.Count; i++)
                {
                    var ng = new RulesGroup(this, AllRulesGroups.Item(i), (byte)i);
                    RuleGroups.Add(ng.StreamId, ng);
                }
        }
        else
        {
            UsesMultipleStreams = false;
        }

        return true;
    }

    public virtual bool LoadRulesFromFile(string fileName)
    {
        if (!File.Exists(fileName))
            return false;
        var xmlData = File.ReadAllText(fileName, Encoding.UTF8);
        var res = LoadRulesFromXmlString(xmlData);
        if (res)
            LoadedRulesFileName = fileName;
        return res;
    }

    public virtual bool SaveRulesFile(string fileName)
    {
        try
        {
            var doBackup = !File.Exists(fileName + ".bak");
            if (doBackup)
                File.Copy(fileName, fileName + ".bak");

            using (XmlTextWriter writer = new (fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                XmlDoc?.Save(writer);
            }

            if (doBackup)
                File.Delete(fileName + ".bak");
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format(Resources.SaveRulesFileFailed, fileName, ex.Message));
            return false;
        }
    }

    public virtual PacketRule? GetPacketRule(PacketDataDirection pdd, byte streamId, byte level, uint packetId)
    {
        var key = (uint)((streamId * 0x01000000) + (level * 0x10000) + packetId);
        var level0Key = (uint)((streamId * 0x01000000) + packetId);
        switch (pdd)
        {
            case PacketDataDirection.Incoming:
                if (S2C.TryGetValue(key, out var inP))
                    return inP;
                if (S2C.TryGetValue(level0Key, out var inP0))
                    return inP0;
                break;
            case PacketDataDirection.Outgoing:
                if (C2S.TryGetValue(key, out var outP))
                    return outP;
                if (C2S.TryGetValue(level0Key, out var outP0))
                    return outP0;
                break;
            case PacketDataDirection.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pdd), pdd, null);
        }
        return null;
    }

    public PacketRule? GetPacketRule(BasePacketData data) => GetPacketRule(data.PacketDataDirection, data.StreamId, data.CompressionLevel, data.PacketId);

    /// <summary>
    /// Override this function to return your own PacketRule descendant if you want to add custom types
    /// </summary>
    /// <param name="ruleGroup"></param>
    /// <param name="pdd"></param>
    /// <param name="streamId"></param>
    /// <param name="level"></param>
    /// <param name="packetId"></param>
    /// <param name="description"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public virtual PacketRule CreateNewPacketRule(RulesGroup ruleGroup, PacketDataDirection pdd, byte streamId, byte level, ushort packetId, string description, XmlNode node)
    {
        //if (!RuleGroups.TryGetValue(streamId, out var ruleGroup))
        //    return null;
        //var key = (uint)((streamId * 0x01000000) + (level * 0x10000) + packetId);
        //var level0Key = (uint)((streamId * 0x01000000) + packetId);

        return new PacketRule(ruleGroup, streamId, level, packetId, description, node);
    }

    public virtual void ParsePacketHeader(BasePacketData packetData)
    {
        // Only reset the cursor if not a specific reader
        packetData.Cursor = 0;
    }

    public virtual void BuildEditorPopupMenu(ContextMenuStrip miInsert, RulesEditorForm editor)
    {
        // Example:
        // var basic = editor.AddMenuItem(miInsert.Items, "Basic Types", "");
        // editor.AddMenuItem(basic!.DropDownItems, "Byte (8 bit)", "<data type=\"byte\" name=\"byte field\" />");

        //--------------------------------------

        // Basic
        var basic = editor.AddMenuItem(miInsert.Items, "Basic Types", "");

        editor.AddMenuItem(basic!.DropDownItems, "byte (8  bit)", "<data type=\"byte\" name=\"|byte|\" />");
        editor.AddMenuItem(basic.DropDownItems, "ushort (16 bit)", "<data type=\"ushort\" name=\"|ushort|\" />");
        editor.AddMenuItem(basic.DropDownItems, "short (16 bit signed)", "<data type=\"short\" name=\"|short|\" />");
        editor.AddMenuItem(basic.DropDownItems, "-", "");
        editor.AddMenuItem(basic.DropDownItems, "uint (32 bit)", "<data type=\"uint\" name=\"|uint|\" />");
        editor.AddMenuItem(basic.DropDownItems, "int (32 bit signed)", "<data type=\"int\" name=\"|int|\" />");
        editor.AddMenuItem(basic.DropDownItems, "ulong (64 bit)", "<data type=\"ulong\" name=\"|ulong|\" />");
        editor.AddMenuItem(basic.DropDownItems, "long (64 bit signed)", "<data type=\"long\" name=\"long|\" />");
        editor.AddMenuItem(basic.DropDownItems, "-", "");
        editor.AddMenuItem(basic.DropDownItems, "float (32 bit)", "<data type=\"float\" name=\"|float|\" />");
        editor.AddMenuItem(basic.DropDownItems, "double (64 bit)", "<data type=\"double\" name=\"|double|\" />");


        var strings = editor.AddMenuItem(miInsert.Items, "Strings", "");
        editor.AddMenuItem(strings!.DropDownItems, "string with size (ASCII)", "<data type=\"zs\" name=\"|ascii size+string|\" />");
        editor.AddMenuItem(strings.DropDownItems, "string (ASCII)", "<data type=\"s\" arg=\"0\" name=\"|ascii string|\" />");
        editor.AddMenuItem(strings.DropDownItems, "string with size (Unicode)", "<data type=\"zu\" name=\"|unicode size+string|\" />");
        editor.AddMenuItem(strings.DropDownItems, "string (Unicode)", "<data type=\"u\" arg=\"0\" name=\"|unicode string|\" />");
        editor.AddMenuItem(strings.DropDownItems, "string with size (UTF8)", "<data type=\"zu8\" name=\"|utf8 size+string|\" />");
        editor.AddMenuItem(strings.DropDownItems, "string (UTF8)", "<data type=\"t\" arg=\"0\" name=\"|utf8 string|\" />");

        // Complex
        var complex = editor.AddMenuItem(miInsert.Items, "Complex Types", "");

        editor.AddMenuItem(complex!.DropDownItems, "bits as value", "<data type=\"bitval\" bits=\"8\" style=\"normal\" name=\"|bit value|\" />");
        editor.AddMenuItem(complex.DropDownItems, "bits (multi-line)", "<data type=\"bits\" bits=\"8\" style=\"normal\" name=\"|bits|\" />");
        editor.AddMenuItem(complex.DropDownItems, "bits (all bits)", "<data type=\"bits\" bits=\"8\" style=\"full\" name=\"|bits|\" />");
        editor.AddMenuItem(complex.DropDownItems, "bits (single-line)", "<data type=\"bits\" bits=\"8\" style=\"compact\" name=\"|bits|\" />");
        editor.AddMenuItem(complex.DropDownItems, "Byte Array", "<data type=\"a\" arg=\"16\" name=\"|array|\" />");
        editor.AddMenuItem(complex.DropDownItems, "-", "");
        editor.AddMenuItem(complex.DropDownItems, "milliseconds (uint32)", "<data type=\"ms\" name=\"|milliseconds|\" />");
        editor.AddMenuItem(complex.DropDownItems, "-", "");
        editor.AddMenuItem(complex.DropDownItems, "int (24 bit)", "<data type=\"uint24\" name=\"|uint24|\" />");
        editor.AddMenuItem(complex.DropDownItems, "half-float (16 bit)", "<data type=\"half\" name=\"|half|\" />");
        editor.AddMenuItem(complex.DropDownItems, "-", "");
        editor.AddMenuItem(complex.DropDownItems, "IPv4 (4 byte)", "<data type=\"ip4\" name=\"|IP|\" />");
        editor.AddMenuItem(complex.DropDownItems, "IPv6 (6 byte)", "<data type=\"ip6\" name=\"|IP|\" />");

        // Functions
        var functions = editor.AddMenuItem(miInsert.Items, "Functions", "");

        var ifs = editor.AddMenuItem(functions!.DropDownItems, "Compare", "");
        editor.AddMenuItem(ifs!.DropDownItems, "if equals (a == b)", "<ifeq arg1=\"#a\" arg2=\"#b\">\n\t<!-- your code -->\n</ifeq>");
        editor.AddMenuItem(ifs.DropDownItems, "if not equals (a != b)", "<ifneq arg1=\"#a\" arg2=\"#b\">\n\t<!-- your code -->\n</ifneq>");
        editor.AddMenuItem(ifs.DropDownItems, "if less than (a < b)", "<iflt arg1=\"#a\" arg2=\"#b\">\n\t<!-- your code -->\n</iflt>");
        editor.AddMenuItem(ifs.DropDownItems, "if greater than (a > b)", "<ifgt arg1=\"#a\" arg2=\"#b\">\n\t<!-- your code -->\n</ifgt>");
        editor.AddMenuItem(ifs.DropDownItems, "if zero (a == 0)", "<ifz arg=\"#a\">\n\t<!-- your code -->\n</ifz>");
        editor.AddMenuItem(ifs.DropDownItems, "if not zero (a != 0)", "<ifnz arg=\"#a\">\n\t<!-- your code -->\n</ifnz>");
        editor.AddMenuItem(ifs.DropDownItems, "else", "<else />");

        var maths = editor.AddMenuItem(functions.DropDownItems, "Math", "");
        editor.AddMenuItem(maths!.DropDownItems, "add (c = a + b)", "<add dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "subtract (c = a - b)", "<sub dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "multiply (c = a * b)", "<mul dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "divide (c = a / b)", "<div dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "-", "");
        editor.AddMenuItem(maths.DropDownItems, "move/copy/assign (c = a)", "<mov dst=\"c\" val=\"#a\" />");
        editor.AddMenuItem(maths.DropDownItems, "-", "");
        editor.AddMenuItem(maths.DropDownItems, "shift-left (c = a << b)", "<shl dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "shift-right (c = a >> b)", "<shr dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "-", "");
        editor.AddMenuItem(maths.DropDownItems, "and (c = a && b)", "<and dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "or (c = a || b)", "<or dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "-", "");
        editor.AddMenuItem(maths.DropDownItems, "float add (c = a + b)", "<addf dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "float subtract (c = a - b)", "<subf dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "float multiply (c = a * b)", "<mulf dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "float divide (c = a / b)", "<divf dst=\"c\" arg1=\"#a\" arg2=\"#b\" />");
        editor.AddMenuItem(maths.DropDownItems, "float copy (c = a)", "<movf dst=\"c\" val=\"#a\" />");

        var loops = editor.AddMenuItem(functions.DropDownItems, "Loops", "");
        editor.AddMenuItem(loops!.DropDownItems, "loop", "<loop max=\"512\">\n<!-- your code here, do not forget to add a break -->\n</loop>");
        editor.AddMenuItem(loops.DropDownItems, "break", "<break />");
        editor.AddMenuItem(loops.DropDownItems, "continue", "<continue />");
        editor.AddMenuItem(loops.DropDownItems, "-", "");
        editor.AddMenuItem(loops.DropDownItems, "loop with counter", "<!-- Loop to 10 example-->\n" +
            "<mov dst=\"c\" val=\"0\" />\n" +
            "<loop>\n" +
            "\t<add dst=\"c\" arg1=\"#c\" arg2=\"1\" />\n" +
            "\t<ifgt arg1=\"#c\" arg2=\"10\">\n" +
            "\t\t<break />\n" +
            "\t</ifgt>\n" +
            "\t<!-- your code here -->\n" +
            "</loop>");

        editor.AddMenuItem(functions.DropDownItems, "save lookup", "<lookup save=\"NewSavedField\" source=\"sourceIdFieldName\" val=\"valueToSafeFieldName\" />");
        editor.AddMenuItem(functions.DropDownItems, "save lookup with value-lookup", "<lookup save=\"NewSavedField\" source=\"sourceIdFieldName\" val=\"valueToSafeFieldName\" altlookup=\"referenceLookupFieldForValue\" />");

        // Lookup tags
        if (editor.PacketData != null)
        {
            var lookupTags = editor.AddMenuItem(miInsert.Items, "Insert Lookup Tag", "");
            foreach (var item in editor.PacketData.ParentProject.DataLookup.LookupLists)
                editor.AddMenuItem(lookupTags!.DropDownItems, item.Key, "lookup=\"" + item.Key + "\" ");
        }

        // Templates in rule file
        var templates = editor.AddMenuItem(miInsert.Items, "Templates", "");
        if (editor.Rule != null)
        {
            foreach (var t in editor.Rule.Parent.Parent.Templates)
                editor.AddMenuItem(templates!.DropDownItems, t.Key, "<template name=\"" + t.Key + "\" />");
        }
        templates!.Enabled = templates.DropDownItems.Count > 0;

        var others = editor.AddMenuItem(miInsert.Items, "Other", "");
        editor.AddMenuItem(others!.DropDownItems, "cursor", "<cursor pos=\"4\" bit=\"0\" />");
        editor.AddMenuItem(others.DropDownItems, "-", "");
        editor.AddMenuItem(others.DropDownItems, "echo", "<echo arg=\"a\" name=\"Info\" />");
        editor.AddMenuItem(others.DropDownItems, "echo block", "<echo arg=\"a\" name=\"Info\" >\n" +
                                                                "\t<!-- your code here -->\n" +
                                                                "</echo>\n");
        editor.AddMenuItem(others.DropDownItems, "-", "");
        editor.AddMenuItem(others.DropDownItems, "comment", "<!--- your comment here -->");
    }

    public PacketRule? CreateNewUserPacketRule(PacketDataDirection packetDataDirection, PacketFilterListEntry key, string newName)
    {
        var verifyPacket = GetPacketRule(packetDataDirection, key.StreamId, key.CompressionLevel, key.PacketId);

        // Already exists, don't create a new one
        if (verifyPacket != null)
            return verifyPacket;

        // Find the related rule group for the given StreamId
        if (!RuleGroups.TryGetValue(key.StreamId, out var ruleGroup))
            return null;

        var packetGroupDirectionNode = packetDataDirection == PacketDataDirection.Incoming ? ruleGroup.S2C :
            packetDataDirection == PacketDataDirection.Outgoing ? ruleGroup.C2S : null;

        var directionSet = packetDataDirection == PacketDataDirection.Incoming ? S2C :
            packetDataDirection == PacketDataDirection.Outgoing ? C2S : null;

        if ((packetGroupDirectionNode == null) || (directionSet == null))
            return null;

        var doc = ruleGroup.RootNode?.OwnerDocument;
        if (doc == null) 
            return null;

        var newXmlElement = doc.CreateElement("packet", packetGroupDirectionNode.NamespaceURI);
        XmlHelper.AddAttribute(newXmlElement, "type", key.PacketId.ToHex(3));
        if (key.CompressionLevel > 0)
        {
            XmlHelper.AddAttribute(newXmlElement, "level", key.CompressionLevel.ToHex());
            UsesCompressionLevels = true;
        }

        XmlHelper.AddAttribute(newXmlElement, "desc", newName);
        XmlHelper.AddAttribute(newXmlElement, "comment", "");

        // var currentWindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last();
        var currentWindowsUser = string.IsNullOrWhiteSpace(Settings.Default.CreditsName) ? Environment.UserName : Settings.Default.CreditsName;
        XmlHelper.AddAttribute(newXmlElement, "credits", currentWindowsUser);

        XmlHelper.AddAttribute(newXmlElement, "app", "VieweD");

        newXmlElement.InnerXml = "<!-- "+newName+" -->"; // GetDefaultNewPacketRuleNodeContents(packetId);

        packetGroupDirectionNode.AppendChild(newXmlElement);

        var newPacketRule = CreateNewPacketRule(ruleGroup, packetDataDirection, key.StreamId, key.CompressionLevel, (ushort)key.PacketId, newName, newXmlElement);

        directionSet.Add(newPacketRule.LookupKey, newPacketRule);

        newPacketRule.Build();

        return newPacketRule;

    }
}