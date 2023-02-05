using System;
using System.Diagnostics;
using System.Xml;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class RulesGroup
    {
        public XmlNode RootNode;
        public XmlNode S2C;
        public XmlNode C2S;
        public RulesReader Parent { get; protected set; }

        /// <summary>
        /// Only used if the capture has multiple connections inside one log file to identify which connection this is supposed to be for 
        /// </summary>
        public byte StreamId { get; protected set; }

        /// <summary>
        /// Port of the associated port
        /// </summary>
        public ushort Port { get; protected set; }

        public string Decryptor { get; protected set; }

        public RulesGroup(RulesReader reader, XmlNode root,byte expectedStreamId = 0)
        {
            Parent = reader;
            StreamId = expectedStreamId;
            RootNode = root;
            for (var i = 0; i < RootNode.ChildNodes.Count; i++)
            {
                var section = RootNode.ChildNodes.Item(i);
                var attributes = XmlHelper.ReadNodeAttributes(section);
                switch (section?.Name.ToLower())
                {
                    case "server":
                        Port = Convert.ToUInt16(XmlHelper.GetAttributeInt(attributes, "port"));
                        Parent?.ParentTab?.Engine?.RegisterPort(Port);
                        StreamId = Parent?.ParentTab?.Engine?.GetExpectedStreamIdByPort(Port, 0) ?? StreamId;
                        break;
                    case "decryptor":
                        Decryptor = XmlHelper.GetAttributeString(attributes, "name");
                        break;
                    case "s2c":
                        S2C = section;
                        break;
                    case "c2s":
                        C2S = section;
                        break;
                    case "#comment":
                    case "#whitespace":
                        // Ignore comments
                        break;
                    default:
                        throw new Exception("Unknown section found inside rule");
                }

            }

            // S2C
            if (S2C != null)
            {
                for (var i = 0; i < S2C.ChildNodes.Count; i++)
                {
                    var pNode = S2C.ChildNodes.Item(i);
                    if (pNode?.Name.ToLower() == "packet")
                    {
                        var attributes = XmlHelper.ReadNodeAttributes(pNode);
                        var pType = Convert.ToUInt16(XmlHelper.GetAttributeInt(attributes, "type"));
                        var level = Convert.ToByte(XmlHelper.GetAttributeInt(attributes, "level"));
                        var description = XmlHelper.GetAttributeString(attributes, "desc");
                        var packetRule = new PacketRule(this, StreamId, level, pType, description, pNode);

                        if (Parent.S2C.ContainsKey(packetRule.LookupKey))
                        {
                            Parent.S2C.Remove(packetRule.LookupKey);
                            Debug.WriteLine("Duplicate S2C Key 0x" + packetRule.LookupKey.ToString("X"));
                        }

                        Parent.S2C.Add(packetRule.LookupKey, packetRule);

                        var dataLookupList = Parent.ParentTab.Engine.DataLookups.NLUOrCreate("in");
                        if (dataLookupList.Data.TryGetValue(packetRule.LookupKey, out var dataLookupEntry))
                        {
                            dataLookupEntry.Val = packetRule.Name;
                        }
                        else
                        {
                            dataLookupEntry = new DataLookupEntry() { Id = packetRule.LookupKey, Val = packetRule.Name };
                            dataLookupList.Data.Add(dataLookupEntry.Id, dataLookupEntry);
                        }
                    }
                }
            }

            // C2S
            if (C2S != null)
            {
                for (var i = 0; i < C2S.ChildNodes.Count; i++)
                {
                    var pNode = C2S.ChildNodes.Item(i);
                    if (pNode?.Name.ToLower() == "packet")
                    {
                        var attributes = XmlHelper.ReadNodeAttributes(pNode);
                        ushort pType;
                        try
                        {
                            pType = Convert.ToUInt16(XmlHelper.GetAttributeInt(attributes, "type"));
                        }
                        catch
                        {
                            continue;
                        }

                        var level = Convert.ToByte(XmlHelper.GetAttributeInt(attributes, "level"));
                        var description = XmlHelper.GetAttributeString(attributes, "desc");
                        var packetRule = reader.ParentTab.Engine.CreatePacketRule(this, StreamId, level, pType, description, pNode);
                        if (Parent.C2S.ContainsKey(packetRule.LookupKey))
                        {
                            Parent.C2S.Remove(packetRule.LookupKey);
                            Debug.WriteLine("Duplicate C2S Key 0x" + packetRule.LookupKey.ToString("X"));
                        }

                        Parent.C2S.Add(packetRule.LookupKey, packetRule);

                        var dll = Parent.ParentTab.Engine.DataLookups.NLUOrCreate("out");
                        if (dll.Data.TryGetValue(packetRule.LookupKey, out var dataLookupEntry))
                        {
                            dataLookupEntry.Val = packetRule.Name;
                        }
                        else
                        {
                            dataLookupEntry = new DataLookupEntry() { Id = packetRule.LookupKey, Val = packetRule.Name };
                            dll.Data.Add(dataLookupEntry.Id, dataLookupEntry);
                        }
                    }
                }
            }
        }
    }
}