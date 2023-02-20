using System.Xml;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.data.ffxi.engine;

public class FfxiRulesReader : RulesReader
{
    public FfxiRulesReader(ViewedProjectTab parent) : base(parent)
    {
    }

    /// <summary>
    /// Returns FFXI style PacketRule
    /// </summary>
    /// <param name="ruleGroup"></param>
    /// <param name="pdd"></param>
    /// <param name="streamId"></param>
    /// <param name="level"></param>
    /// <param name="packetId"></param>
    /// <param name="description"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public override PacketRule? CreateNewPacketRule(RulesGroup ruleGroup, PacketDataDirection pdd, byte streamId, byte level, ushort packetId, string description, XmlNode node)
    {
        //if (!RuleGroups.TryGetValue(streamId, out var ruleGroup))
        //    return null;

        return new FfxiPacketRule(ruleGroup, streamId, level, packetId, description, node);
    }

    public override void ParsePacketHeader(BasePacketData packetData)
    {
        base.ParsePacketHeader(packetData);

        packetData.PacketId = (ushort)(packetData.GetByteAtPos(0) + (packetData.GetByteAtPos(1) & 0x01) * 0x100);
        packetData.PacketDataSize = (ushort)((packetData.GetByteAtPos(1) & 0xFE) * 2);
        packetData.SyncId = (ushort)(packetData.GetByteAtPos(2) + packetData.GetByteAtPos(3) * 0x100);

        packetData.AddParsedField(true, 0, 1, "0x00", "PacketID", packetData.PacketId.ToHex(3) + " - " + packetData.GetPacketName(), 0);
        packetData.AddParsedField(true, 1, 1, "0x01", "PacketSize", packetData.PacketDataSize.ToString(), 0);
        packetData.AddParsedField(true, 2, 3, "0x02", "SyncID", packetData.SyncId.ToHex(4), 0);
    }
}