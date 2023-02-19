using System.Text;
using System.Xml;
using VieweD.Helpers.System;

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
        RuleGroups.Clear();
        if (AllRulesGroups != null)
            for (var i = 0; i < AllRulesGroups.Count; i++)
            {
                var ng = new RulesGroup(this, AllRulesGroups.Item(i), (byte)i);
                RuleGroups.Add(ng.StreamId, ng);
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

            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
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
            MessageBox.Show($"Failed to save rules: {fileName}\r\n{ex.Message}");
            return false;
        }
    }

    public virtual PacketRule? GetPacketRule(PacketDataDirection pdd, byte streamId, byte level, ushort packetId)
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
    public virtual PacketRule? CreateNewPacketRule(RulesGroup ruleGroup, PacketDataDirection pdd, byte streamId, byte level, ushort packetId, string description, XmlNode node)
    {
        //if (!RuleGroups.TryGetValue(streamId, out var ruleGroup))
        //    return null;
        //var key = (uint)((streamId * 0x01000000) + (level * 0x10000) + packetId);
        //var level0Key = (uint)((streamId * 0x01000000) + packetId);

        return new PacketRule(ruleGroup, streamId, level, packetId, description, node);
    }

}