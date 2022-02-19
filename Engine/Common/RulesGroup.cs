using System;
using System.Diagnostics;
using System.Xml;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class RulesGroup
    {
        protected XmlNode RootNode;
        protected XmlNode S2C;
        protected XmlNode C2S;
        public RulesReader Parent { get; protected set; }

        /// <summary>
        /// Only used if the capture has multiple connections inside one log file to identify which connection this is supposed to be for 
        /// </summary>
        public byte StreamId { get; protected set; }
        /// <summary>
        /// Port of the associated port
        /// </summary>
        public UInt16 Port { get; protected set; }
        public string Decryptor { get; protected set; }

        public RulesGroup(RulesReader reader, XmlNode root,byte expectedStreamId = 0)
        {
            Parent = reader;
            StreamId = expectedStreamId;
            RootNode = root;
            for (int i = 0; i < RootNode.ChildNodes.Count; i++)
            {
                var section = RootNode.ChildNodes.Item(i);
                var attribs = XmlHelper.ReadNodeAttributes(section);
                switch (section?.Name.ToLower())
                {
                    case "server":
                        // Ignored
                        break;
                    case "decryptor":
                        Decryptor = XmlHelper.GetAttributeString(attribs, "name");
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
                for (int i = 0; i < S2C.ChildNodes.Count; i++)
                {
                    var pNode = S2C.ChildNodes.Item(i);
                    if (pNode.Name.ToLower() == "packet")
                    {
                        var attribs = XmlHelper.ReadNodeAttributes(pNode);
                        var type = Convert.ToUInt16(XmlHelper.GetAttributeInt(attribs, "type"));
                        var level = Convert.ToByte(XmlHelper.GetAttributeInt(attribs, "level"));
                        var desc = XmlHelper.GetAttributeString(attribs, "desc");
                        var pr = new PacketRule(this, this.StreamId, level, type, desc, pNode);
                        if (this.Parent.S2C.ContainsKey(pr.LookupKey))
                        {
                            this.Parent.S2C.Remove(pr.LookupKey);
                            Debug.WriteLine("Duplicate S2C Key 0x" + pr.LookupKey.ToString("X"));
                        }

                        this.Parent.S2C.Add(pr.LookupKey, pr);
                        var dll = this.Parent.parentTab.Engine.DataLookups.NLUOrCreate("in");
                        if (dll.Data.TryGetValue(pr.LookupKey, out var dle))
                        {
                            dle.Val = pr.Name;
                        }
                        else
                        {
                            dle = new DataLookupEntry() { Id = pr.LookupKey, Val = pr.Name };
                            dll.Data.Add(dle.Id, dle);
                        }
                    }
                }
            }

            // C2S
            if (C2S != null)
            {
                for (int i = 0; i < C2S.ChildNodes.Count; i++)
                {
                    var pNode = C2S.ChildNodes.Item(i);
                    if (pNode.Name.ToLower() == "packet")
                    {
                        var attribs = XmlHelper.ReadNodeAttributes(pNode);
                        UInt16 type = 0xFFFF;
                        try
                        {
                            type = Convert.ToUInt16(XmlHelper.GetAttributeInt(attribs, "type"));
                        }
                        catch
                        {
                            continue;
                        }

                        var level = Convert.ToByte(XmlHelper.GetAttributeInt(attribs, "level"));
                        var desc = XmlHelper.GetAttributeString(attribs, "desc");
                        var pr = reader.parentTab.Engine.CreatePacketRule(this, this.StreamId, level, type, desc, pNode);
                        if (this.Parent.C2S.ContainsKey(pr.LookupKey))
                        {
                            this.Parent.C2S.Remove(pr.LookupKey);
                            Debug.WriteLine("Duplicate C2S Key 0x" + pr.LookupKey.ToString("X"));
                        }

                        this.Parent.C2S.Add(pr.LookupKey, pr);
                        var dll = this.Parent.parentTab.Engine.DataLookups.NLUOrCreate("out");
                        if (dll.Data.TryGetValue(pr.LookupKey, out var dle))
                        {
                            dle.Val = pr.Name;
                        }
                        else
                        {
                            dle = new DataLookupEntry() { Id = pr.LookupKey, Val = pr.Name };
                            dll.Data.Add(dle.Id, dle);
                        }
                    }
                }
            }
            
        }
    }

}