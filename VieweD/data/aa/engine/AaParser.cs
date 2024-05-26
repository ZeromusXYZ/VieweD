using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.data.aa.engine;

public class AaParser : BaseParser
{
    public override string Name => "ArcheAge";
    public override string Description => "Parses ArcheAge packet data";
    public override string DefaultRulesFile => "aa.xml";

    public override int PacketIdMinimum => 1;
    public override int PacketIdMaximum => 0x1FF;
    public override bool AllowSyncSearch => true;
    public override int PacketCompressionLevelMaximum => 5;

    public AaParser(ViewedProjectTab parentProject) : base(parentProject)
    {
        // Supported Readers
        SupportedReaders.Add("ArcheAge D-file Reader");
        // Override the rules reader with the AA one for custom fields
        Rules = new AaRulesReader(parentProject);
    }

    public AaParser()
    {
        // Supported Readers
        SupportedReaders.Add("ArcheAge D-file Reader");
    }

    public override BaseParser CreateNew(ViewedProjectTab parentProject)
    {
        return new AaParser(parentProject);
    }

    public override bool ParsePacketData(BasePacketData packetData, bool initialLoading)
    {
        packetData.ParsedData.Clear();
        // In your own parser, replace this check with whatever 
        // if (packetData is not BasePacketData data)
        //     return false;
        var data = packetData; // and then remove this one

        if (packetData.DoNotParse)
        {
            packetData.AddParsedError("", "Unparsed", "Parsing for this packet is disabled!", 0);
            var d = packetData.GetDataAtPos(0, packetData.ByteData.Count);
            packetData.AddParsedField(false, 0, packetData.ByteData.Count, 0.ToHex(2), "Byte data", d, 0);
            d = packetData.GetStringAtPos(0);
            packetData.AddParsedField(false, 0, packetData.ByteData.Count, 0.ToHex(2), "String data", d, 0);
            data.BuildHeaderText();
            return true;
        }

        if (data.ByteData.Count < 4)
        {
            data.PacketId = 0xFFFF;
            data.CompressionLevel = 0;
            data.PacketDataSize = 0;
            data.HeaderText = "Invalid Packet Size < 4";
            return false;
        }

        data.Cursor = 0;

        // Do actual parsing, you can overwrite packetData values here if you want
        var rule = Rules?.GetPacketRule(data);
        data.ParsedPacketName = rule?.Name ?? string.Empty;
        data.BuildHeaderText();
        rule?.Build();
        rule?.RunRule(data);

        // Add unparsed data
        data.AddUnparsedFields();
        return true;
    }

    public override void ExpandSubPackets()
    {
        if (ParentProject == null) // Requires a project
            return;
        if (Rules == null) // requires rules to be loaded
            return;
        if (ParentProject.InputReader is not AaBaseInputReader reader)
            return; // needs a D-file reader

        // Un-mark requirement here, so if things fail, it won't get parsed again
        ParentProject.RequiresSubPacketCreation = false;

        ViewedProjectTab.OnExpandProgressUpdate(this, 0, ParentProject.LoadedPacketList.Count);

        for (int i = ParentProject.LoadedPacketList.Count - 1; i >= 0; i--)
        {
            var parentPacketData = ParentProject.LoadedPacketList[i];
            if (parentPacketData.UnParseSubPacketCount <= 1)
                continue;

            // First, do a regular parse of the main packet
            if (Rules.GetPacketRule(parentPacketData) is not AaPacketRule firstRule)
                return; // If it can't parse, abort

            var previousData = parentPacketData;
            parentPacketData.Cursor = 0;

            parentPacketData.ParsedPacketName = firstRule.Name;
            firstRule.Build();
            firstRule.RunRule(parentPacketData);
            parentPacketData.UnParseSubPacketCount--;

            var originalExpandedStartPos = parentPacketData.Cursor;

            //while (parentPacketData.Cursor < parentPacketData.ByteData.Count - 6)
            while (parentPacketData.UnParseSubPacketCount > 0)
            {
                var subData = new BasePacketData(parentPacketData.ParentProject);

                var startPos = parentPacketData.Cursor;
                subData.ParentPacket = parentPacketData;
                subData.OriginalHeaderText = parentPacketData.OriginalHeaderText;
                subData.SourceIp = parentPacketData.SourceIp;
                subData.SourcePort = parentPacketData.SourcePort;
                subData.SourceMac = parentPacketData.SourceMac;
                subData.DestinationIp = parentPacketData.DestinationIp;
                subData.DestinationPort = parentPacketData.DestinationPort;
                subData.DestinationMac = parentPacketData.DestinationMac;
                subData.TimeStamp = parentPacketData.TimeStamp;
                subData.SyncId = parentPacketData.SyncId;
                subData.PacketDataDirection = parentPacketData.PacketDataDirection;
                subData.CompressionLevel = 0;
                subData.PacketId = parentPacketData.PacketId;
                subData.ByteData.AddRange(parentPacketData.ByteData.GetRange(0, 4));
                var readCount = parentPacketData.ByteData.Count - startPos;
                if (readCount > 0)
                {
                    subData.ByteData.AddRange(parentPacketData.ByteData.GetRange(startPos, readCount));
                }
                else
                {
                    previousData.MarkedAsInvalid = true;
                    return;
                }
                //if (parentPacketData.CompressionLevel == 4) 
                subData.PacketId = subData.GetUInt16AtPos(6);
                subData.Cursor = 0;

                if (reader.CompileData(subData))
                {
                    // ParentProject.LoadedPacketList.Insert(data.ThisIndex, pde);
                    ParentProject.LoadedPacketList.Insert(i + parentPacketData.SubPackets.Count + 1, subData);

                    subData.BuildHeaderText();
                    if (Rules.GetPacketRule(subData) is AaPacketRule subRule)
                    {
                        subData.ParsedPacketName = subRule.Name;
                        subRule.Build();
                        subRule.RunRule(subData);
                        var pdeEndPos = subData.Cursor;

                        _ = subData.GetUInt16AtPos(subData.Cursor); // This is the repeat counter on the first packet

                        // Remove unwanted bytes from sub-packet
                        if (subData.ByteData.Count > pdeEndPos)
                        {
                            subData.ByteData.RemoveRange(pdeEndPos, subData.ByteData.Count - pdeEndPos);
                            subData.IsTruncatedByParser = true;
                        }
                        else
                        {
                            // Perfect fit
                        }

                        // Advance original packet cursor to expected position
                        parentPacketData.Cursor += subData.ByteData.Count - 4;
                        parentPacketData.SubPackets.Add(subData);
                        parentPacketData.UnParseSubPacketCount--;
                    }
                    else
                    {
                        // Invalid/Unknown data in sub-packet, likely the last packet was not parsed completely/correctly
                        subData.MarkedAsInvalid = true;
                        previousData.MarkedAsInvalid = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
                

                previousData = subData;
                ViewedProjectTab.OnExpandProgressUpdate(this, ParentProject.LoadedPacketList.Count - i, ParentProject.LoadedPacketList.Count);
            }

            parentPacketData.ByteData.RemoveRange(originalExpandedStartPos, parentPacketData.ByteData.Count - originalExpandedStartPos);
            parentPacketData.IsTruncatedByParser = true;

        }
        ViewedProjectTab.OnExpandProgressUpdate(this, 1, 1);
    }
}