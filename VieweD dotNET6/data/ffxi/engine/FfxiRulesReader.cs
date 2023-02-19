using System.Xml;
using VieweD.engine.common;

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
}