using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.data.ffxi.engine;

public class FfxiParser : BaseParser
{
    public override string Name => "Final Fantasy XI";
    public override string Description => "Parses Final Fantasy XI packet data";
    public override string DefaultRulesFile => "ffxi.xml";

    public override int PacketIdMinimum => 1;
    public override int PacketIdMaximum => 0x1FF;
    public override bool AllowSyncSearch => true;

    public FfxiParser(ViewedProjectTab parentProject) : base(parentProject)
    {
        // Supported Readers
        SupportedReaders.Add("FFXI Packeteer");
        SupportedReaders.Add("FFXI Packet Viewer");
        // Override the rules reader with the FFXI one for custom fields
        Rules = new FfxiRulesReader(parentProject);
    }

    public FfxiParser() : base()
    {
        // Supported Readers
        SupportedReaders.Add("FFXI Packeteer");
        SupportedReaders.Add("FFXI Packet Viewer");
    }

    public override BaseParser CreateNew(ViewedProjectTab parentProject)
    {
        return new FfxiParser(parentProject);
    }

    public override bool ParsePacketData(BasePacketData packetData, bool initialLoading)
    {
        packetData.ParsedData.Clear();
        // In your own parser, replace this check with whatever 
        if (packetData is not BasePacketData data)
            return false;

        if (data.ByteData.Count < 4)
        {
            data.PacketId = 0x01FF; // FFXI's max theoretical packet ID
            data.PacketDataSize = 0;
            data.HeaderText = "Invalid Packet Size < 4";
            return false;
        }
        data.PacketId = (ushort)(data.GetByteAtPos(0) + (data.GetByteAtPos(1) & 0x01) * 0x100);
        data.PacketDataSize = (ushort)((data.GetByteAtPos(1) & 0xFE) * 2);
        data.SyncId = (ushort)(data.GetByteAtPos(2) + data.GetByteAtPos(3) * 0x100);

        data.AddParsedField(true, 0, 1, "0x00", "PacketID", data.PacketId.ToHex(3) + " - " + data.GetPacketName(), 0);
        data.AddParsedField(true, 1, 1, "0x01", "PacketSize", data.PacketDataSize.ToString(), 0);
        data.AddParsedField(true, 2, 3, "0x02", "SyncID", data.SyncId.ToHex(4), 0);

        // All FFXI packet actual data start at position 4 (4 byte packet header)
        data.Cursor = 4;

        // Do actual parsing, you can overwrite packetData values here if you want
        var rule = Rules?.GetPacketRule(data);
        rule?.Build();
        rule?.RunRule(data);

        // Add unparsed data
        data.AddUnparsedFields();
        return true;
    }
}