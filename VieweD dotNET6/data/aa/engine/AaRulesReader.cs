using System.Xml;
using VieweD.Forms;
using VieweD.engine.common;
using VieweD.Helpers.System;
using System.Windows.Forms;

namespace VieweD.data.aa.engine;

public class AaRulesReader : RulesReader
{
    public override string DefaultDataParseElementName => "chunk";

    public AaRulesReader(ViewedProjectTab parent) : base(parent)
    {
    }

    /// <summary>
    /// Returns AA style PacketRule
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
        return new AaPacketRule(ruleGroup, streamId, level, packetId, description, node);
    }

    public override void ParsePacketHeader(BasePacketData packetData)
    {
        base.ParsePacketHeader(packetData);

        var streamName = packetData.ParentProject.GetStreamIdName(packetData.StreamId);
        //packetData.CompressionLevel = 0;
        //packetData.PacketDataSize = 0;
        //packetData.PacketId = 0;
        //packetData.SyncId = 0;

        packetData.AddParsedError("Header", "Data", $"{streamName} - Level {packetData.CompressionLevel} - Size {packetData.PacketDataSize}", 0);
        packetData.AddParsedError("Header", "PacketID", $"{packetData.PacketDataDirection} {packetData.PacketId.ToHex(3)} - {packetData.GetPacketName()}", 0);
    }

    public override void BuildEditorPopupMenu(ContextMenuStrip miInsert, RulesEditorForm editor)
    {
        base.BuildEditorPopupMenu(miInsert, editor);

        editor.AddMenuItem(miInsert.Items, "-", "");
        var aaMenu = editor.AddMenuItem(miInsert.Items, "ArcheAge Specific", "");

        editor.AddMenuItem(aaMenu!.DropDownItems, "pish (?? bytes)", "<chunk type=\"pish\" name=\"|pish|\" count=\"4\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "-", "");
        editor.AddMenuItem(aaMenu.DropDownItems, "float X (3 byte)", "<chunk type=\"bcx\" name=\"|x|\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "float Y (3 byte)", "<chunk type=\"bcy\" name=\"|y|\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "float Z (3 byte)", "<chunk type=\"bcz\" name=\"|z|\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "-", "");
        editor.AddMenuItem(aaMenu.DropDownItems, "uint64 shifted X (8 byte)", "<chunk type=\"qx\" name=\"|x|\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "uint64 shifted Y (8 byte)", "<chunk type=\"qy\" name=\"|y|\" />");
        editor.AddMenuItem(aaMenu.DropDownItems, "uint64 shifted Z (8 byte)", "<chunk type=\"qz\" name=\"|z|\" />");
    }
}