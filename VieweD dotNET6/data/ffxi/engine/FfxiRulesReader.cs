using System.Xml;
using Microsoft.CodeAnalysis.FlowAnalysis;
using VieweD.engine.common;
using VieweD.Forms;
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
    public override PacketRule CreateNewPacketRule(RulesGroup ruleGroup, PacketDataDirection pdd, byte streamId, byte level, ushort packetId, string description, XmlNode node)
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

    public override void BuildEditorPopupMenu(ContextMenuStrip miInsert, RulesEditorForm editor)
    {
        base.BuildEditorPopupMenu(miInsert, editor);

        var ffxiMenu = editor.AddMenuItem(miInsert.Items, "FFXI Specific", "");

        editor.AddMenuItem(ffxiMenu!.DropDownItems, "Position (12 byte)", "<data type=\"pos\" name=\"|Position|\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "Direction (1 byte)", "<data type=\"dir\" name=\"|Direction|\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "Vana'diel Time (4 byte)", "<data type=\"vanatime\" name=\"|Time|\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "-", "");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "Combat Skill (2 byte)", "<data type=\"combatskill\" name=\"|SkillName|\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "Job Points (3 byte)", "<data type=\"jobpoints\" name=\"|JobName|\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "Buffs (32x6 byte)", "<data type=\"buffs\" name=\"|Buffs|\" arg=\"32\" />");
        editor.AddMenuItem(ffxiMenu.DropDownItems, "RoE Quest (4 byte)", "<data type=\"roequest\" name=\"|Quest|\" />");
    }
}