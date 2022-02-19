using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class RulesReader
    {
        protected XmlDocument _doc;
        protected XmlNodeList _allRulesGroups;
        protected XmlNodeList _allTemplates;
        protected Dictionary<byte, RulesGroup> RuleGroups = new Dictionary<byte, RulesGroup>();
        public Dictionary<uint, PacketRule> C2S = new Dictionary<uint, PacketRule>(); // out: client to server
        public Dictionary<uint, PacketRule> S2C = new Dictionary<uint, PacketRule>(); // In : server to client
        public Dictionary<string, XmlNode> Templates = new Dictionary<string, XmlNode>();
        public string LoadedRulesFileName { get; protected set; }

        public PacketTabPage parentTab;
        protected ZlibCodec decompressor = new ZlibCodec(Ionic.Zlib.CompressionMode.Decompress);


        protected RulesReader(PacketTabPage parent)
        {
            parentTab = parent;
        }

        protected virtual bool LoadRulesFromXmlString(string xmlData)
        {
            LoadedRulesFileName = "";
            // Open XML file
            _doc = new XmlDocument();
            _doc.Load(new StringReader(xmlData));
            
            // Read and Save template nodes in a List
            _allTemplates = _doc.SelectNodes("/root/templates/template");
            Templates.Clear();
            if (_allTemplates != null)
                for (var i = 0; i < _allTemplates.Count; i++)
                {
                    var node = _allTemplates.Item(i);
                    var name = XmlHelper.GetAttributeString(XmlHelper.ReadNodeAttributes(node), "name");
                    Templates.Add(name, node);
                }

            // Locate rule sections
            _allRulesGroups = _doc.SelectNodes("/root/rule");
            RuleGroups.Clear();
            if (_allRulesGroups != null)
                for (var i = 0; i < _allRulesGroups.Count; i++)
                {
                    var ng = new RulesGroup(this, _allRulesGroups.Item(i), (byte)i);
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
                File.Copy(fileName, fileName + ".bak");
                using (XmlTextWriter writer = new XmlTextWriter(fileName, null))
                {
                    writer.Formatting = Formatting.Indented;
                    _doc.Save(writer);
                }
                File.Delete(fileName + ".bak");
                return true;
            }
            catch (Exception x)
            {
                MessageBox.Show("Failed to save rules: " + fileName + "\r\n" + x.Message);
                return false;
            }
        }

        public virtual PacketRule GetPacketRule(PacketLogTypes pt, byte streamId, byte level, UInt16 packetId)
        {
            uint key = (uint)((streamId * 0x01000000) + (level * 0x10000) + packetId);
            uint l0key = (uint)((streamId * 0x01000000) + packetId);
            switch (pt)
            {
                case PacketLogTypes.Incoming:
                    if (S2C.TryGetValue(key, out var inP))
                        return inP;
                    if (S2C.TryGetValue(l0key, out var inP0))
                        return inP0;
                    break;
                case PacketLogTypes.Outgoing:
                    if (C2S.TryGetValue(key, out var outP))
                        return outP;
                    if (C2S.TryGetValue(l0key, out var outP0))
                        return outP0;
                    break;
            }
            return null;
        }
        
    }
}