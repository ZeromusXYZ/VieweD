using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Engine.FFXI
{
    public class FFXIPacketParser : PacketParser
    {
        static private string[] CompasDirectionNames = new string[16] { "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N", "NNE", "NE", "ENE" };

        public FFXIPacketParser(UInt16 aPacketID, PacketLogTypes aPacketLogType) : base()
        {
            var filename = string.Empty;
            switch (aPacketLogType)
            {
                case PacketLogTypes.Incoming:
                    filename = Path.Combine(Application.StartupPath, "data", EngineFFXI.ThisEngineId, "parse", "in-0x" + aPacketID.ToString("X3") + ".txt");
                    break;
                case PacketLogTypes.Outgoing:
                    filename = Path.Combine(Application.StartupPath, "data", EngineFFXI.ThisEngineId, "parse", "out-0x" + aPacketID.ToString("X3") + ".txt");
                    break;
                default:
                    // Really shouldn't be used, but added for completion
                    filename = Path.Combine(Application.StartupPath, "data", EngineFFXI.ThisEngineId, "parse", "unk-0x" + aPacketID.ToString("X3") + ".txt");
                    break;
            }

            if (File.Exists(filename))
            {
                PD = null;
                ThisPacketID = aPacketID;
                ThisPacketLogType = aPacketLogType;
                RawParseData = File.ReadLines(filename).ToList();
            }
        }

        private int Parse_Packet_In_0x028(PacketData PD, ref ushort DataFieldIndex)
        {
            int BitOffset = 0;

            void AddField(ref ushort AddDataFieldIndex, int BitSize, string name, string data, UInt64 dataUInt64 = 0)
            {
                AddDataFieldEx((BitOffset / 8), (BitSize / 8), ref AddDataFieldIndex);
                var posStr = "0x" + (BitOffset / 8).ToString("X") + ":" + (BitOffset % 8).ToString();
                if (BitSize > 1)
                    posStr += "-" + BitSize.ToString();
                AddParseLineToView(AddDataFieldIndex, posStr, GetDataColor(AddDataFieldIndex), name, data, dataUInt64);
                MarkParsed((BitOffset / 8), (BitSize / 8), AddDataFieldIndex);
                BitOffset += BitSize;
            }

            int bytesUsed = 0;

            if ((PD.PacketLogType != PacketLogTypes.Incoming) || (PD.PacketId != 0x028))
            {
                AddParseLineToView(0x00, "Error", Color.Red, "Invalid", "Can only use this field type on a Incoming 0x028 packet");
                return 0;
            }
            var pSize = PD.GetByteAtPos(0x04);
            var pActor = PD.GetUInt32AtPos(0x05);
            var pTargetCount = PD.GetBitsAtPos(0x09, 0, 10);

            // First group contains info about the size instead of actual data ?
            // The bit offset is a pain to work with however >.>

            var pActionCategory = PD.GetBitsAtPos(0x0A, 2, 4);
            var pActionID = PD.GetBitsAtPos(0x0A, 6, 16);
            var pUnknown1 = PD.GetBitsAtPos(0x0C, 6, 16);
            var pRecast = PD.GetBitsAtPos(0x0E, 6, 32);

            AddDataFieldEx(0x04, 1, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x04", GetDataColor(DataFieldIndex), "Info Size", pSize.ToString(), pSize);
            MarkParsed(0x04, 1, DataFieldIndex);

            AddDataFieldEx(0x05, 4, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x05", GetDataColor(DataFieldIndex), "Actor", PD.Parent._parentTab.Engine.DataLookups.NLU("@actors").GetValue(pActor) + " <= 0x" + pActor.ToString("X8") + " - " + pActor.ToString(), pActor);
            MarkParsed(0x05, 4, DataFieldIndex);

            AddDataFieldEx(0x09, 1, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x09", GetDataColor(DataFieldIndex), "Target Count", pTargetCount.ToString(), (UInt64)pTargetCount);
            MarkParsed(0x09, 1, DataFieldIndex);

            AddDataFieldEx(0x0A, 1, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x0A:2-4", GetDataColor(DataFieldIndex), "Action Category", pActionCategory.ToString() + " => " + PD.Parent._parentTab.Engine.DataLookups.NLU(EngineFFXI.LU_ActionCategory0x028).GetValue((UInt64)pActionCategory), (UInt64)pActionCategory);
            MarkParsed(0x0A, 1, DataFieldIndex);

            AddDataFieldEx(0x0B, 1, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x0A:6-16", GetDataColor(DataFieldIndex), "Action ID", pActionID.ToString(), (UInt64)pActionID);
            MarkParsed(0x0B, 1, DataFieldIndex);

            AddDataFieldEx(0x0C, 2, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x0C:6-16", GetDataColor(DataFieldIndex), "_unknown1", pUnknown1.ToString(), (UInt64)pUnknown1);
            MarkParsed(0x0C, 2, DataFieldIndex);

            AddDataFieldEx(0x0E, 5, ref DataFieldIndex);
            AddParseLineToView(DataFieldIndex, "0x0E:6-32", GetDataColor(DataFieldIndex), "Recast", pRecast.ToString(), (UInt64)pRecast);
            MarkParsed(0x0E, 5, DataFieldIndex);

            int FirstTargetBitOffset = 150; // (0x012:6)
            int LastBit = PD.RawBytes.Count * 8;
            BitOffset = FirstTargetBitOffset;
            int pTargetCountLoopCounter = 0;

            while ((BitOffset < LastBit) && (pTargetCountLoopCounter < pTargetCount))
            {
                pTargetCountLoopCounter++;

                var pActionTargetID = PD.GetBitsAtBitPos(BitOffset, 32);
                AddField(ref DataFieldIndex, 32, "#" + pTargetCountLoopCounter + " : Target ID", PD.Parent._parentTab.Engine.DataLookups.NLU("@actors").GetValue((UInt64)pActionTargetID) + " <= 0x" + pActionTargetID.ToString("X8") + " - " + pActionTargetID.ToString());

                var pActionTargetIDSize = PD.GetBitsAtBitPos(BitOffset, 4);
                AddField(ref DataFieldIndex, 4, "#" + pTargetCountLoopCounter + " : Effect Count", pActionTargetIDSize.ToString());

                int tTargetEffectLoopCounter = 0;

                while ((BitOffset < LastBit) && (tTargetEffectLoopCounter < pActionTargetIDSize))
                {
                    tTargetEffectLoopCounter++;

                    var thisLoopName = "  #" + pTargetCountLoopCounter + " " + tTargetEffectLoopCounter + "/" + pActionTargetIDSize.ToString() + " ";

                    var tReaction = PD.GetBitsAtBitPos(BitOffset, 5);
                    AddField(ref DataFieldIndex, 5, thisLoopName + "Reaction", "0x" + tReaction.ToString("X") + " => " + BitFlagsToString(EngineFFXI.LU_ActionReaction, (UInt64)tReaction, " + "));

                    var tAnimation = PD.GetBitsAtBitPos(BitOffset, 12);
                    AddField(ref DataFieldIndex, 12, thisLoopName + "Animation", "0x" + tAnimation.ToString("X3") + " - " + tAnimation.ToString());

                    var tSpecialEffect = PD.GetBitsAtBitPos(BitOffset, 7);
                    AddField(ref DataFieldIndex, 7, thisLoopName + "SpecialEffect", "0x" + tSpecialEffect.ToString("X2") + " - " + tSpecialEffect.ToString());

                    var tKnockBack = PD.GetBitsAtBitPos(BitOffset, 3);
                    AddField(ref DataFieldIndex, 3, thisLoopName + "KnockBack", tKnockBack.ToString());

                    var tParam = PD.GetBitsAtBitPos(BitOffset, 17);
                    AddField(ref DataFieldIndex, 17, thisLoopName + "Param", "0x" + tParam.ToString("X5") + " - " + tParam.ToString());

                    var tMessageID = PD.GetBitsAtBitPos(BitOffset, 10);
                    AddField(ref DataFieldIndex, 10, thisLoopName + "MessageID", "0x" + tMessageID.ToString("X3") + " - " + tMessageID.ToString());

                    var tUnknown = PD.GetBitsAtBitPos(BitOffset, 31);
                    AddField(ref DataFieldIndex, 31, thisLoopName + "??? 31 Bits", "0x" + tUnknown.ToString("X8") + " - " + tUnknown.ToString());

                    var hasAdditionalEffect = PD.GetBitAtPos(BitOffset / 8, BitOffset % 8);
                    AddField(ref DataFieldIndex, 1, thisLoopName + "Has Additional Effect", hasAdditionalEffect.ToString());

                    if (hasAdditionalEffect)
                    {
                        var tAdditionalEffect = PD.GetBitsAtBitPos(BitOffset, 10);
                        AddField(ref DataFieldIndex, 10, "  " + thisLoopName + "Added Effect", "0x" + tAdditionalEffect.ToString("X3") + " - " + tAdditionalEffect.ToString());

                        var tAddEffectParam = PD.GetBitsAtBitPos(BitOffset, 17);
                        AddField(ref DataFieldIndex, 17, "  " + thisLoopName + "Effect Param", "0x" + tAddEffectParam.ToString("X5") + " - " + tAddEffectParam.ToString());

                        var tAddEffectMessage = PD.GetBitsAtBitPos(BitOffset, 10);
                        AddField(ref DataFieldIndex, 10, "  " + thisLoopName + "Effect MsgID", "0x" + tAddEffectMessage.ToString("X3") + " - " + tAddEffectMessage.ToString());
                    }

                    var hasSpikesEffect = PD.GetBitAtPos(BitOffset / 8, BitOffset % 8);
                    AddField(ref DataFieldIndex, 1, thisLoopName + "Has Spikes Effect", hasSpikesEffect.ToString());

                    if (hasSpikesEffect)
                    {
                        var tSpikesEffect = PD.GetBitsAtBitPos(BitOffset, 10);
                        AddField(ref DataFieldIndex, 10, "  " + thisLoopName + "Spike Effect", "0x" + tSpikesEffect.ToString("X3") + " - " + tSpikesEffect.ToString());

                        var tSpikesParam = PD.GetBitsAtBitPos(BitOffset, 14);
                        AddField(ref DataFieldIndex, 14, "  " + thisLoopName + "Spike Param", "0x" + tSpikesParam.ToString("X4") + " - " + tSpikesParam.ToString());

                        var tSpikesMessage = PD.GetBitsAtBitPos(BitOffset, 10);
                        AddField(ref DataFieldIndex, 10, "  " + thisLoopName + "Spike MsgID", "0x" + tSpikesMessage.ToString("X3") + " - " + tSpikesMessage.ToString());
                    }

                }
            }
            if ((BitOffset % 8) > 0)
                bytesUsed = (BitOffset / 8) + 1;
            else
                bytesUsed = (BitOffset / 8);

            return bytesUsed;
        }

        public override void ParseData(string ActiveSwitchBlock)
        {
            ushort DataFieldIndex = 0; // header is considered 0

            void AddDataField(int StartPos, int FieldByteSize)
            {
                AddDataFieldEx(StartPos, FieldByteSize, ref DataFieldIndex);
            }

            ParsedView.Clear();
            var ffxiEngine = PD.Parent._parentTab.Engine;
            var ffxiLookup = ffxiEngine.DataLookups;

            // Fixed Header Info, Always 4 bytes, always listed
            switch (PD.PacketLogType)
            {
                case PacketLogTypes.Outgoing:
                    AddParseLineToView(0xFF,
                        "0x00", GetDataColor(0),
                        "PacketID",
                        "OUT 0x" + PD.PacketId.ToString("X") + " - ", ffxiLookup.PacketTypeToString(PacketLogTypes.Outgoing, PD.PacketId), PD.PacketId);
                    break;
                case PacketLogTypes.Incoming:
                    AddParseLineToView(0xFF,
                        "0x00", GetDataColor(0),
                        "PacketID",
                        "IN 0x" + PD.PacketId.ToString("X") + " - " + ffxiLookup.PacketTypeToString(PacketLogTypes.Incoming,PD.PacketId), PD.PacketId);
                    break;
                default:
                    AddParseLineToView(0xFF,
                        "0x00", GetDataColor(0),
                        "PacketID",
                        "??? 0x" + PD.PacketId.ToString("X"), PD.PacketId);
                    break;
            }
            AddParseLineToView(0xFF,
                "0x00", GetDataColor(0),
                "Size",
                PD.PacketDataSize.ToString() + " (0x" + PD.PacketDataSize.ToString("X2") + ")", PD.PacketDataSize);
            AddParseLineToView(0xFF,
                "0x00", GetDataColor(0),
                "Sync",
                PD.PacketSync.ToString() + " (0x" + PD.PacketSync.ToString("X4") + ")", PD.PacketSync);
            // Marked as FF for fixed format
            ParsedBytes[0] = 0xFF;
            ParsedBytes[1] = 0xFF;
            ParsedBytes[2] = 0xFF;
            ParsedBytes[3] = 0xFF;
            int DataCursor = 0;
            int DataSubCursor = 0;
            bool enableCode = true; // This is mostly used to enable or disable the logging of switchblocks

            string CurrentlyParsedSwitchBlock = "";
            SwitchBlocks.Clear();
            // Parse the parse file
            //foreach(string rawline in RawParseData)
            bool AllowAutoSwitchBlock = ((ActiveSwitchBlock == string.Empty) || (ActiveSwitchBlock == "-"));
            LastSwitchedBlock = "";
            for (int parseLineNumber = 1; parseLineNumber <= RawParseData.Count; parseLineNumber++)
            {
                // We want a  type;offset[;name[;description]] format

                string line = RawParseData[parseLineNumber - 1];
                // Cut out comments
                if (line.IndexOf("//") >= 0)
                    line = line.Substring(0, line.IndexOf("//"));
                line = line.Trim(' ');

                // Skip blank lines
                if (line == string.Empty)
                    continue;

                // Handle Switch Blocks
                if (line.StartsWith("[[") && line.EndsWith("]]"))
                {
                    // This is a switch block statement
                    var thisSwitchName = line.Substring(2, line.Length - 4).Trim(' ');
                    if (thisSwitchName != string.Empty)
                    {
                        if (enableCode)
                            SwitchBlocks.Add(thisSwitchName);
                    }
                    else
                    {
                        AllowAutoSwitchBlock = true;
                    }
                    CurrentlyParsedSwitchBlock = thisSwitchName;
                    continue;
                }
                if ((CurrentlyParsedSwitchBlock != "") && (CurrentlyParsedSwitchBlock.ToLower() != ActiveSwitchBlock.ToLower()))
                    continue;

                string[] fields = line.Split(';');
                if (fields.Count() < 2)
                {
                    // Need at least 2 fields to be a valid line
                    AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Need at least 2 fields");
                    continue;
                }
                string typeField = fields[0].ToLower().Trim(' ');
                string posField = fields[1].ToLower().Trim(' ');
                string nameField = "";
                string descriptionField = "";
                string lookupField = "";
                string lookupOffsetEvalString = "";
                int lookupFieldOffset = 0;
                if (fields.Length > 2)
                {
                    nameField = fields[2].TrimEnd(' ');
                }
                else
                {
                    nameField = typeField + " (@" + posField + ")";
                }
                if (fields.Length > 3)
                {
                    descriptionField = fields[3];
                }

                if (typeField == "file")
                    continue;
                if (typeField == "rem")
                    continue;
                if (typeField == "comment")
                    continue;
                if (typeField == "enablecode")
                {
                    if (int.TryParse(posField, out var enableCodeVal))
                        enableCode = (enableCodeVal != 0);
                    continue;
                }
                if (typeField == string.Empty)
                    continue;

                if (typeField == "save")
                {
                    // Saves a previously parsed value into a (temporary) custom lookup table
                    // Lookup table is program-wide, so can span multiple logs at the same time

                    if (fields.Count() < 4)
                    {
                        AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Not enough fields for save function, requires 3 parameters");
                        continue;
                    }

                    var baseVal = GetParsedBaseValue(fields[2].ToLower());
                    var parseVal = GetParsedValue(fields[3].ToLower());
                    ffxiLookup.RegisterCustomLookup(fields[1], baseVal, parseVal);
                    continue;
                }

                // Parse type lookup (if present)
                var lookupFieldSplitPos = typeField.IndexOf(":");
                if (lookupFieldSplitPos > 0)
                {
                    var lookupFields = typeField.Split(':');
                    typeField = lookupFields[0];
                    lookupField = lookupFields[1];
                    if (lookupFields.Length > 2)
                    {
                        lookupOffsetEvalString = lookupFields[2];
                        if (DataLookups.TryFieldParse(lookupOffsetEvalString, out int lookupoffsetres))
                            lookupFieldOffset = lookupoffsetres;
                    }
                }

                // Parse posField
                int Offset = 0;
                int SubOffset = 0;
                int SubOffsetRange = 0;
                bool hasSubOffset = false;
                bool hasSubOffsetRange = false;
                int posSplitPos = posField.IndexOf(":");
                if (posSplitPos > 0)
                {
                    // Has sub offset
                    var offsetStr = posField.Substring(0, posSplitPos);
                    var subOffsetStr = posField.Substring(posSplitPos + 1, posField.Length - posSplitPos - 1);
                    hasSubOffset = true;

                    // Get normal offset for this
                    if (!DataLookups.TryFieldParse(offsetStr, out Offset))
                    {
                        Offset = 0;
                        AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Invalid Offset Value in: " + posField + " ("+ offsetStr + ")");
                        continue;
                    }

                    // Check for bit range
                    int rangeSplitPos = subOffsetStr.IndexOf("-");
                    if (rangeSplitPos > 0)
                    {
                        // We have a range
                        var subOffsetBitStr = subOffsetStr.Substring(0, rangeSplitPos);
                        var subOffsetRangeStr = subOffsetStr.Substring(rangeSplitPos + 1, subOffsetStr.Length - rangeSplitPos - 1);
                        hasSubOffsetRange = true;

                        if (!DataLookups.TryFieldParse(subOffsetBitStr, out SubOffset))
                        {
                            Offset = 0;
                            AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Invalid SubOffset Value in: " + posField + " ("+ subOffsetBitStr + ")");
                            continue;
                        }

                        if (!DataLookups.TryFieldParse(subOffsetRangeStr, out SubOffsetRange))
                        {
                            Offset = 0;
                            AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Invalid SubOffsetRange Value in: " + posField + " ("+ subOffsetRangeStr + ")");
                            continue;
                        }
                    }
                    else
                    {
                        // doesn't have a range
                        if (!DataLookups.TryFieldParse(subOffsetStr, out SubOffset))
                        {
                            Offset = 0;
                            AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Invalid SubOffset Value in: " + posField + " ("+ subOffsetStr + ")");
                            continue;
                        }
                    }
                }
                else
                {
                    // Normal single offset value
                    if (!DataLookups.TryFieldParse(posField, out Offset))
                    {
                        Offset = 0;
                        AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "Parse Error", "Invalid Offset Value in: " + posField);
                        continue;
                    }
                }

                DataCursor = Offset;
                DataSubCursor = SubOffset;
                // Rebuild Pos string
                posField = "0x" + Offset.ToString("X2");
                if (hasSubOffset)
                {
                    posField += ":" + SubOffset.ToString();
                    if (hasSubOffsetRange)
                        posField += "-" + SubOffsetRange.ToString();
                }

                // Begin parse
                if (typeField == "switchblock")
                {
                    if (AllowAutoSwitchBlock)
                    {
                        // switchblock;checkpos;checkval;blockname
                        // Compares value at Offset, if posVal matches a value in the description field, activate blockname as current block

                        int posVal = 0;
                        if ((SubOffset != 0) || (SubOffsetRange != 0))
                            posVal = (int)PD.GetBitsAtPos(Offset, SubOffset, SubOffsetRange);
                        else
                            posVal = (int)PD.GetByteAtPos(Offset);
                        // Switchval seems valid, next compare it
                        if (ValueInStringList(posVal, descriptionField))
                        {
                            ActiveSwitchBlock = nameField;
                            AllowAutoSwitchBlock = false;

                            // Debug Info
                            if (enableCode)
                                AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "SwitchBlock", "Activate Block: " + ActiveSwitchBlock);

                            LastSwitchedBlock = ActiveSwitchBlock;
                            if (PreParsedSwitchBlock == "-")
                                PreParsedSwitchBlock = ActiveSwitchBlock;
                            continue;
                        }
                    }
                }
                else
                if (typeField == "showblock")
                {
                    // showblock;0;;blockname
                    // Forcefull enable a switchblock if none is set yet
                    if (AllowAutoSwitchBlock)
                    {

                        ActiveSwitchBlock = nameField;
                        AllowAutoSwitchBlock = false;

                        // Debug Info
                        if (enableCode)
                            AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), Color.Red, "ShowBlock", "Activate Block: " + ActiveSwitchBlock);

                        LastSwitchedBlock = ActiveSwitchBlock;
                        if (PreParsedSwitchBlock == "-")
                            PreParsedSwitchBlock = ActiveSwitchBlock;
                        continue;
                    }
                }
                else
                if (typeField == "info")
                {
                    // Info line for the view
                    if (Offset < 4)
                        AddParseLineToView(0xFF, "Info", GetDataColor(0xFF), nameField, descriptionField);
                    else
                    {
                        // Only mark it as parsed if a specific offset is provided
                        AddDataField(Offset, 1);
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, descriptionField);
                        MarkParsed(Offset, 1, DataFieldIndex);
                    }
                }
                else
                if ((typeField == "byte") || (typeField == "uint8"))
                {
                    var d = PD.GetByteAtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)(d), lookupOffsetEvalString);
                    AddDataField(Offset, 1);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " - 0x" + d.ToString("X2") + " - " + ByteToBits(d) + " - '" + (char)d + "'", descriptionField);
                    MarkParsed(Offset, 1, DataFieldIndex);
                }
                else
                if ((typeField == "sbyte") || (typeField == "int8"))
                {
                    var d = PD.GetSByteAtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)unchecked((byte)d), lookupOffsetEvalString);
                    AddDataField(Offset, 1);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " - 0x" + d.ToString("X2") + " - " + ByteToBits(unchecked((byte)d)) + " - '" + (char)d + "'", descriptionField, (UInt64)unchecked((byte)d));
                    MarkParsed(Offset, 1, DataFieldIndex);
                }
                else
                if ((typeField == "bit") || (typeField == "bool"))
                {
                    var d = PD.GetBitAtPos(Offset, SubOffset);
                    AddDataField(Offset, 1);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d.ToString(), descriptionField, (UInt64)(d ? 1 : 0));
                    MarkParsed(Offset, 1, DataFieldIndex);
                }
                else
                if (typeField == "bits")
                {
                    var d = PD.GetBitsAtPos(Offset, SubOffset, SubOffsetRange);
                    var l = Lookup(lookupField, (UInt64)(d), lookupOffsetEvalString);
                    var firstbit = (Offset * 8) + SubOffset;
                    var lastbit = firstbit + SubOffsetRange;
                    var firstbyte = firstbit / 8;
                    var lastbyte = lastbit / 8;
                    var bytesize = lastbyte - firstbyte + 1;
                    AddDataField(firstbyte, bytesize);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + "0x" + d.ToString("X") + " - " + d.ToString(""), descriptionField, (UInt64)d);
                    MarkParsed(firstbyte, bytesize, DataFieldIndex);
                }
                else
                if (typeField == "uint16")
                {
                    var d = PD.GetUInt16AtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)(d), lookupOffsetEvalString);
                    AddDataField(Offset, 2);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " (0x" + d.ToString("X4") + ")", descriptionField);
                    MarkParsed(Offset, 2, DataFieldIndex);
                }
                else
                if (typeField == "int16")
                {
                    var d = PD.GetInt16AtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)unchecked((UInt16)d), lookupOffsetEvalString);
                    AddDataField(Offset, 2);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " (0x" + d.ToString("X4") + ")", descriptionField, (UInt64)unchecked((UInt16)d));
                    MarkParsed(Offset, 2, DataFieldIndex);
                }
                else
                if (typeField == "uint32")
                {
                    var d = PD.GetUInt32AtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)(d), lookupOffsetEvalString);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " (0x" + d.ToString("X8") + ")", descriptionField, d);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "int32")
                {
                    var d = PD.GetInt32AtPos(Offset);
                    var l = Lookup(lookupField, (UInt64)unchecked((UInt32)d), lookupOffsetEvalString);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString() + " (0x" + d.ToString("X8") + ")", descriptionField, (UInt64)unchecked((UInt32)d));
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "float")
                {
                    var d = PD.GetFloatAtPos(Offset);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d.ToString(), descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "pos")
                {
                    var x = PD.GetFloatAtPos(Offset);
                    var y = PD.GetFloatAtPos(Offset + 4);
                    var z = PD.GetFloatAtPos(Offset + 8);
                    AddDataField(Offset, 12);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "X:" + x.ToString("F") + "  Y:" + y.ToString("F") + "  Z:" + z.ToString("F"), descriptionField);
                    MarkParsed(Offset, 12, DataFieldIndex);
                }
                else
                if (typeField == "dir")
                {
                    var d = PD.GetByteAtPos(Offset);
                    var dir = ByteToRotation(d);
                    AddDataField(Offset, 1);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, dir, descriptionField);
                    MarkParsed(Offset, 1, DataFieldIndex);
                }
                else
                if (typeField.StartsWith("string"))
                {
                    var sizeStr = typeField.Substring(6, typeField.Length - 6);
                    int size = -1;
                    if ((sizeStr == string.Empty) || (!int.TryParse(sizeStr, out size)))
                    {
                        size = -1;
                    }
                    var d = PD.GetStringAtPos(Offset, size);
                    string dHex = "";
                    if (d == string.Empty)
                    {
                        d = "NULL";
                        dHex = string.Empty;
                    }
                    else
                    if (Engines.ShowStringHexData)
                    {
                        foreach (char c in d)
                        {
                            if (dHex != string.Empty)
                                dHex += " ";
                            dHex += ((byte)c).ToString("X2");
                        }
                        dHex = " (" + dHex + ")";
                    }
                    else
                    {
                        dHex = string.Empty;
                    }

                    AddDataField(Offset, size);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d + dHex, descriptionField);
                    if (size > 1)
                    {
                        MarkParsed(Offset, size, DataFieldIndex);
                    }
                    else
                    {
                        MarkParsed(Offset, d.Length, DataFieldIndex);
                    }
                }
                else
                if (typeField == "vstring")
                {
                    var stringSize = PD.GetUInt16AtPos(Offset);
                    var stringdata = PD.GetStringAtPos(Offset + 2, stringSize);
                    AddDataField(Offset, stringSize + 2);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, stringdata, descriptionField);
                    MarkParsed(Offset, stringSize + 2, DataFieldIndex);
                }
                else
                if (typeField == "ffstring")
                {
                    var stringBytes = new List<byte>();
                    var bPos = Offset;
                    while ((bPos < PD.RawBytes.Count) && (PD.RawBytes[bPos] != 0x00))
                    {
                        stringBytes.Add(PD.GetByteAtPos(bPos));
                        bPos++;
                    }
                    var stringSize = stringBytes.Count;
                    var stringdata = "";
                    stringdata = FFXIEncoding.E.GetString(stringBytes.ToArray());
                    AddDataField(Offset, stringSize);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, stringdata, descriptionField);
                    MarkParsed(Offset, stringSize, DataFieldIndex);
                }
                else
                if (typeField.StartsWith("data"))
                {
                    var sizeStr = typeField.Substring(4, typeField.Length - 4);
                    int size = 1;
                    if (!int.TryParse(sizeStr, out size))
                    {
                        size = 1;
                    }
                    var d = PD.GetDataAtPos(Offset, size);
                    AddDataField(Offset, size);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d, descriptionField);
                    MarkParsed(Offset, size, DataFieldIndex);
                }
                else
                if (typeField == "ms")
                {
                    var d = PD.GetUInt32AtPos(Offset);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, MSToString(d) + " (" + d.ToString() + ")", descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "frames")
                {
                    var d = PD.GetUInt32AtPos(Offset);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, FramesToString(d) + " (" + d.ToString() + ")", descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "vanatime")
                {
                    var d = PD.GetUInt32AtPos(Offset);
                    AddDataField(Offset, 4);
                    var vt = new VanadielTime();
                    vt.FromVanadielIntTime((int)d);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, vt.LocalEarthTime.ToString("yyyy-MM-dd HH:mm:ss") + " (" + d.ToString() + ") => " + vt.ToString(), descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "ip4")
                {
                    var d = PD.GetIP4AtPos(Offset);
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d + " <= " + PD.GetUInt32AtPos(Offset), descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "linkshellstring")
                {
                    var d = PD.GetPackedString16AtPos(Offset, String6BitEncodeKeys.Linkshell);
                    AddDataField(Offset, 16);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d, descriptionField);
                    MarkParsed(Offset, 16, DataFieldIndex);
                }
                else
                if (typeField == "inscribestring")
                {
                    var d = PD.GetPackedString16AtPos(Offset, String6BitEncodeKeys.Item);
                    AddDataField(Offset, 16);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d, descriptionField);
                    MarkParsed(Offset, 16, DataFieldIndex);
                }
                else
                if (typeField == "bitflaglist")
                {
                    // Default to 64 bits if nothing provided
                    if (SubOffsetRange <= 0)
                        SubOffsetRange = 64;
                    AddDataField(Offset, SubOffsetRange / 8);
                    int foundCount = 0;
                    int posByte = Offset;
                    int posBit = SubOffset;
                    int bitNumber = 0;
                    while (posByte + (posBit / 8) < PD.RawBytes.Count)
                    {
                        bool thisBit = PD.GetBitAtPos(posByte, posBit);
                        if (thisBit)
                        {
                            foundCount++;
                            string d = "Bit" + bitNumber.ToString();
                            if (lookupField != string.Empty)
                            {
                                d += " => " + ffxiLookup.NLU(lookupField).GetValue((UInt64)(bitNumber + lookupFieldOffset));
                            }
                            AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField + " - Bit" + bitNumber.ToString(), d, descriptionField);
                        }

                        bitNumber++;
                        posBit++;

                        if (bitNumber >= SubOffsetRange)
                            break;

                        while (posBit >= 8)
                        {
                            posBit -= 8;
                            posByte++;
                        }
                    }
                    if (foundCount <= 0)
                    {
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "No bits are set", descriptionField);
                    }
                    MarkParsed(Offset, SubOffsetRange / 8, DataFieldIndex);
                }
                else
                if (typeField == "bitflaglist2")
                {
                    // Default to 16 bits if nothing provided
                    if (SubOffsetRange <= 0)
                        SubOffsetRange = 16;
                    AddDataField(Offset, SubOffsetRange / 8);
                    int foundCount = 0;
                    int posByte = Offset;
                    int posBit = SubOffset;
                    int bitNumber = 0;
                    string d2 = string.Empty;
                    while (posByte + (posBit / 8) < PD.RawBytes.Count)
                    {
                        bool thisBit = PD.GetBitAtPos(posByte, posBit);
                        if (thisBit)
                        {
                            foundCount++;
                            string d = "Bit" + bitNumber.ToString();
                            if (lookupField != string.Empty)
                            {
                                d = ffxiLookup.NLU(lookupField).GetValue((UInt64)(bitNumber + lookupFieldOffset));
                            }
                            if (d.Trim(' ') == "")
                                d = "Bit" + bitNumber.ToString();
                            if (d2 != string.Empty)
                                d2 += ", ";
                            d2 += d;
                        }

                        bitNumber++;
                        posBit++;

                        if (bitNumber >= SubOffsetRange)
                            break;

                        while (posBit >= 8)
                        {
                            posBit -= 8;
                            posByte++;
                        }
                    }
                    if (foundCount <= 0)
                    {
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "No bits are set", descriptionField);
                    }
                    else
                    {
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d2, descriptionField);
                    }
                    MarkParsed(Offset, SubOffsetRange / 8, DataFieldIndex);
                }
                else
                if (typeField == "combatskill")
                {
                    var d = PD.GetUInt16AtPos(Offset);
                    var cappedString = "";
                    var skilllevel = (d & 0x7FFF);
                    AddDataField(Offset, 2);
                    if ((d & 0x8000) != 0)
                        cappedString = " (Capped)";
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, skilllevel.ToString() + cappedString + " <= 0x" + d.ToString("X4"), descriptionField);
                    MarkParsed(Offset, 2, DataFieldIndex);
                }
                else
                if (typeField == "craftskill")
                {
                    var d = PD.GetUInt16AtPos(Offset);
                    var cappedString = "";
                    var craftlevel = ((d >> 5) & 0x03FF);
                    var craftrank = (d & 0x001F);
                    AddDataField(Offset, 2);
                    if ((d & 0x8000) != 0)
                        cappedString = " (Capped)";
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "Level: " + craftlevel.ToString() + " Rank: " + craftrank.ToString() + " " + ffxiLookup.NLU(EngineFFXI.LU_CraftRanks).GetValue((UInt64)craftrank) + cappedString + " <= 0x" + d.ToString("X4"), descriptionField);
                    MarkParsed(Offset, 2, DataFieldIndex);
                }
                else
                if (typeField == "equipsetitem")
                {
                    AddDataField(Offset, 4);
                    string d = "";
                    if (PD.GetBitAtPos(Offset, 0))
                        d += "Active ";
                    if (PD.GetBitAtPos(Offset, 1))
                        d += "Bit1Set? ";
                    var bagid = PD.GetBitsAtPos(Offset, 2, 6);
                    var invindex = PD.GetByteAtPos(Offset + 1);
                    var item = PD.GetUInt32AtPos(Offset + 2);
                    d += ffxiLookup.NLU(EngineFFXI.LU_Container).GetValue((UInt64)bagid) + " ";
                    d += "InvIndex: " + invindex.ToString() + " ";
                    d += "Item: " + ffxiLookup.NLU(EngineFFXI.LU_Item).GetValue((UInt64)item);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "0x" + PD.GetUInt32AtPos(Offset).ToString("X4") + " => " + d, descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "equipsetitemlist")
                {
                    // Special field, SubOffset is used a position for the count byte
                    var counter = PD.GetByteAtPos(SubOffset);
                    // Otherwise works exactly the same as a loop of "equipsetitem"

                    AddDataField(Offset, counter * 4);
                    var c = 0;
                    for (int off = 0; (off < PD.RawBytes.Count - 4) && (c < counter); off += 4)
                    {
                        string d = "";
                        if (PD.GetBitAtPos(off + Offset, 0))
                            d += "Active ";
                        if (PD.GetBitAtPos(off + Offset, 1))
                            d += "Bit1Set? ";
                        var bagid = PD.GetBitsAtPos(off + Offset, 2, 6);
                        var invindex = PD.GetByteAtPos(off + Offset + 1);
                        var item = PD.GetUInt32AtPos(off + Offset + 2);
                        d += ffxiLookup.NLU(EngineFFXI.LU_Container).GetValue((UInt64)bagid) + " ";
                        d += "InvIndex: " + invindex.ToString() + " ";
                        d += "Item: " + ffxiLookup.NLU(EngineFFXI.LU_Item).GetValue((UInt64)item);
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), "0x" + PD.GetUInt32AtPos(off + Offset).ToString("X4") + " => " + d, descriptionField);
                        c++;
                    }
                    MarkParsed(Offset, counter * 4, DataFieldIndex);
                }
                else
                if (typeField == "abilityrecastlist")
                {
                    string d = "";
                    int off = Offset;
                    int counter = SubOffset;
                    for (int c = 0; c < counter; c++)
                    {
                        if (off > PD.RawBytes.Count - 8)
                            break;

                        var aID = PD.GetByteAtPos(off + 3);
                        var aDur = PD.GetUInt16AtPos(off);

                        d = "";
                        d += "ID: 0x" + aID.ToString("X2") + " (" + ffxiLookup.NLU(EngineFFXI.LU_ARecast).GetValue(aID) + ") ";
                        d += "Duration: " + aDur.ToString() + " - ";
                        d += "byte@2: 0x" + PD.GetByteAtPos(off + 2).ToString("X2") + " ";
                        d += "uint32@4: 0x" + PD.GetUInt32AtPos(off + 4).ToString("X8") + " ";

                        // Only show used recast timers
                        if ((aID != 0) || (c == 0))
                        {
                            AddDataField(off, 8);
                            AddParseLineToView(DataFieldIndex, "0x" + off.ToString("X2"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), d, descriptionField);
                            MarkParsed(off, 8, DataFieldIndex);
                        }
                        off += 8;
                    }
                }
                else
                if (typeField == "blacklistentry")
                {
                    var pID = PD.GetUInt32AtPos(Offset);
                    var pName = PD.GetStringAtPos(Offset + 4);
                    AddDataField(Offset, 20);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, "ID: 0x" + pID.ToString("X8") + " => " + pName, descriptionField);
                    MarkParsed(Offset, 20, DataFieldIndex);
                }
                else
                if (typeField == "meritentries")
                {
                    // Special field, SubOffset is used a position for the count byte
                    var counter = PD.GetByteAtPos(SubOffset);
                    // Otherwise works exactly the same as a loop of "equipsetitem"

                    AddDataField(Offset, counter * 4);
                    var c = 0;
                    for (int off = 0; (off < PD.RawBytes.Count - 4) && (c < counter); off += 4)
                    {
                        var meritID = PD.GetUInt16AtPos(off + Offset);
                        var meritNextCost = PD.GetByteAtPos(off + Offset + 2);
                        var meritValue = PD.GetUInt32AtPos(off + Offset + 3);
                        string d = "";
                        d += "ID: 0x" + meritID.ToString("X4") + " (" + ffxiLookup.NLU(EngineFFXI.LU_Merit).GetValue((UInt64)meritID) + ") - ";
                        d += "Next Cost: " + meritNextCost.ToString() + " - ";
                        d += "Value: " + meritValue.ToString();
                        AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), d, descriptionField);
                        c++;
                    }
                    MarkParsed(Offset, counter * 4, DataFieldIndex);
                }
                else
                if (typeField == "playercheckitems")
                {
                    // SubOffset is the adress for the counter to use, defaults to previous byte
                    int counter;
                    if (SubOffset < 1)
                        counter = PD.GetByteAtPos(Offset - 1);
                    else
                        counter = PD.GetByteAtPos(SubOffset);

                    var n = Offset;
                    var c = 0;
                    while ((n <= (PD.RawBytes.Count - 28)) && (c < counter))
                    {
                        c++;
                        UInt16 idVal = PD.GetUInt16AtPos(n);
                        UInt16 slotVal = PD.GetUInt16AtPos(n + 2);
                        byte unkVal = PD.GetByteAtPos(n + 3);
                        string idStr = "0x" + idVal.ToString("X4") + " => " + ffxiLookup.NLU(EngineFFXI.LU_Item).GetValue(idVal);
                        string slotStr = "0x" + slotVal.ToString("X4") + " => " + ffxiLookup.NLU(EngineFFXI.LU_EquipmentSlots).GetValue(slotVal);
                        string unkStr = "0x" + unkVal.ToString("X4") + " (" + unkVal.ToString() + ")";
                        string extdataStr = PD.GetDataAtPos(n + 4, 24);

                        AddDataField(n, 28);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " ID", idStr, descriptionField);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " Slot", slotStr, descriptionField);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " ???", unkStr, descriptionField);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " ExtData", extdataStr, descriptionField);
                        MarkParsed(n, 28, DataFieldIndex);
                        n += 28;
                    }

                }
                else
                if (typeField == "bufficons")
                {
                    // SubOffset is counter to use, defaults to 1
                    int counter;
                    if (SubOffset < 1)
                        counter = 1;
                    else
                        counter = SubOffset;

                    var n = Offset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 2) && (c < counter))
                    {
                        c++;
                        UInt16 iconVal = PD.GetUInt16AtPos(n);
                        string iconStr = "0x" + iconVal.ToString("X4") + " => " + ffxiLookup.NLU(EngineFFXI.LU_Buffs).GetValue(iconVal);

                        AddDataField(n, 2);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), iconStr, descriptionField);
                        MarkParsed(n, 2, DataFieldIndex);
                        n += 2;
                    }

                }
                else
                if (typeField == "bufftimers")
                {
                    // SubOffset is counter to use, defaults to 1
                    int counter;
                    if (SubOffset < 1)
                        counter = 1;
                    else
                        counter = SubOffset;

                    var n = Offset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 4) && (c < counter))
                    {
                        c++;
                        Int32 timerVal = PD.GetInt32AtPos(n);
                        string timerStr;
                        if (timerVal == 0)
                            timerStr = "0x" + timerVal.ToString("X8") + " => Not defined";
                        else
                        if (timerVal == 0x7FFFFFFF)
                            timerStr = "0x" + timerVal.ToString("X8") + " => Always";
                        else
                            timerStr = "0x" + timerVal.ToString("X8") + " => " + MSToString((uint)timerVal);

                        AddDataField(n, 4);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), timerStr, descriptionField);
                        MarkParsed(n, 4, DataFieldIndex);
                        n += 4;
                    }

                }
                else
                if (typeField == "buffs")
                {
                    // SubOffset is counter to use, defaults to 1
                    // SubOffsetRange is the starting offset for the timers, defaults to right after the icons
                    int counter;
                    int timerOffset;
                    if (SubOffset < 1)
                        counter = 1;
                    else
                        counter = SubOffset;

                    if (SubOffsetRange < 1)
                        timerOffset = Offset + (counter * 2);
                    else
                        timerOffset = SubOffsetRange;

                    var n = Offset;
                    var t = timerOffset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 2) && (t <= PD.RawBytes.Count - 4) && (c < counter))
                    {
                        c++;

                        UInt16 iconVal = PD.GetUInt16AtPos(n);
                        string iconStr = "0x" + iconVal.ToString("X4") + " => " + ffxiLookup.NLU(EngineFFXI.LU_Buffs).GetValue(iconVal);

                        AddDataField(n, 2);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " Icon", iconStr, descriptionField);
                        MarkParsed(n, 2, DataFieldIndex);

                        Int32 timerVal = PD.GetInt32AtPos(t);
                        string timerStr;
                        if (timerVal == 0)
                            timerStr = "0x" + timerVal.ToString("X8") + " => Not defined";
                        else
                        if (timerVal == 0x7FFFFFFF)
                            timerStr = "0x" + timerVal.ToString("X8") + " => Always";
                        else
                            timerStr = "0x" + timerVal.ToString("X8") + " => " + MSToString((uint)timerVal);

                        // AddDataField(t, 4);
                        AddParseLineToView(DataFieldIndex, "0x" + t.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString() + " Timer", timerStr, descriptionField);
                        MarkParsed(t, 4, DataFieldIndex);
                        n += 2;
                        t += 4;
                    }

                }
                else
                if (typeField == "jobpointentries")
                {
                    // SubOffset is counter to use, defaults to "until end of packet"
                    int counter;
                    if (SubOffset < 1)
                        counter = (PD.RawBytes.Count - Offset) / 4;
                    else
                        counter = SubOffset;

                    var n = Offset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 4) && (c < counter))
                    {
                        c++;
                        string d = "";
                        UInt16 jpVal = PD.GetUInt16AtPos(n);
                        UInt16 jp2Val = PD.GetUInt16AtPos(n + 2);
                        var jpUnkVal = PD.GetBitsAtPos(n + 2, 0, 10);
                        var jpLevelVal = PD.GetBitsAtPos(n + 3, 2, 6);

                        d += "ID: 0x" + jpVal.ToString("X4") + " => " + ffxiLookup.NLU(EngineFFXI.LU_JobPoint).GetValue(jpVal) + " + ";
                        d += "Para: 0x" + jp2Val.ToString("X4") + " => ";
                        d += "Level: " + jpLevelVal.ToString() + " - ";
                        d += "Next?: " + jpUnkVal.ToString() + " (0x" + jpUnkVal.ToString("X") + ")";

                        AddDataField(n, 4);
                        if ((jpVal != 0) && (jp2Val != 0))
                            AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), d, descriptionField);
                        MarkParsed(n, 4, DataFieldIndex);
                        n += 4;
                    }

                }
                else
                if (typeField == "shopitems")
                {
                    // SubOffset is counter to use, defaults to "until end of packet"
                    int counter;
                    if (SubOffset < 1)
                        counter = (PD.RawBytes.Count - Offset) / 12;
                    else
                        counter = SubOffset;

                    var n = Offset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 12) && (c < counter))
                    {
                        c++;
                        string d = "";
                        UInt32 gilVal = PD.GetUInt32AtPos(n);
                        UInt16 itemVal = PD.GetUInt16AtPos(n + 4);
                        byte slotVal = PD.GetByteAtPos(n + 6);
                        byte unkVal = PD.GetByteAtPos(n + 7);
                        UInt16 skillVal = PD.GetUInt16AtPos(n + 8);
                        UInt16 rankVal = PD.GetUInt16AtPos(n + 10);

                        d += "Item: 0x" + itemVal.ToString("X4") + "  ";
                        d += "Gil: " + gilVal.ToString().PadLeft(7) + "  ";
                        d += "Skill: 0x" + skillVal.ToString("X4") + "  ";
                        d += "Rank: 0x" + rankVal.ToString("X4") + "  ";
                        d += "Slot: " + slotVal.ToString().PadRight(2) + "  ";
                        d += "???: 0x" + unkVal.ToString("X2");
                        d += " => " + ffxiLookup.NLU(EngineFFXI.LU_Item).GetValue(itemVal);

                        AddDataField(n, 12);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), d, descriptionField);
                        MarkParsed(n, 12, DataFieldIndex);
                        n += 12;
                    }

                }
                else
                if (typeField == "guildshopitems")
                {
                    int counter = 30;
                    var n = Offset;
                    var c = 0;
                    while ((n <= PD.RawBytes.Count - 8) && (c < counter))
                    {
                        c++;
                        string d = "";
                        UInt32 gilVal = PD.GetUInt32AtPos(n + 4);
                        UInt16 itemVal = PD.GetUInt16AtPos(n);
                        byte stockVal = PD.GetByteAtPos(n + 2);
                        byte stockmaxVal = PD.GetByteAtPos(n + 3);

                        d += "Item: 0x" + itemVal.ToString("X4") + "  ";
                        d += "Gil: " + gilVal.ToString().PadLeft(7) + "  ";
                        d += "Stock: " + stockVal.ToString() + " / " + stockmaxVal.ToString();
                        d += " => " + ffxiLookup.NLU(EngineFFXI.LU_Item).GetValue(itemVal);

                        AddDataField(n, 8);
                        AddParseLineToView(DataFieldIndex, "0x" + n.ToString("X"), GetDataColor(DataFieldIndex), nameField + " #" + c.ToString(), d, descriptionField);
                        MarkParsed(n, 8, DataFieldIndex);
                        n += 8;
                    }

                }
                else
                if (typeField == "jobpoints")
                {
                    var cpVal = PD.GetUInt16AtPos(Offset);
                    var jpVal = PD.GetUInt16AtPos(Offset + 2);
                    var spentVal = PD.GetUInt16AtPos(Offset + 4);
                    string d = "";
                    d += cpVal.ToString() + " CP  ";
                    d += jpVal.ToString() + " JP  ";
                    d += spentVal.ToString() + " Spent JP  ";

                    AddDataField(Offset, 6);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d, descriptionField);
                    MarkParsed(Offset, 6, DataFieldIndex);
                }
                else
                if (typeField == "roequest")
                {
                    var idVal = PD.GetBitsAtPos(Offset, 0, 12);
                    var progressVal = PD.GetBitsAtPos(Offset + 1, 4, 20);
                    var maxVal = PD.Parent._parentTab.Engine.DataLookups.NLU(EngineFFXI.LU_RoE).GetExtra((UInt64)idVal);
                    if (maxVal == "")
                        maxVal = "???";
                    string d = "";
                    d += "ID; 0x" + idVal.ToString("X3") + " => " + PD.Parent._parentTab.Engine.DataLookups.NLU(EngineFFXI.LU_RoE).GetValue((UInt64)idVal) + "  ";
                    d += "Progress; " + progressVal.ToString() + " / " + maxVal;

                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, d, descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);
                }
                else
                if (typeField == "dialogtext")
                {
                    var d = PD.GetUInt16AtPos(Offset);
                    var l = "0x" + d.ToString("X4") + " => " + EngineFFXI.DialogsListFfxi.GetValue((ushort)PD.CapturedZoneId, d);
                    AddDataField(Offset, 2);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField, l + d.ToString(), descriptionField);
                    MarkParsed(Offset, 2, DataFieldIndex);
                }
                else
                if (typeField == "fishrankservermessage")
                {
                    var fishrankentrycount = PD.GetUInt32AtPos(Offset);
                    var l = fishrankentrycount.ToString() + " entries in this block";
                    AddDataField(Offset, 4);
                    AddParseLineToView(DataFieldIndex, posField, GetDataColor(DataFieldIndex), nameField + " count", l, descriptionField);
                    MarkParsed(Offset, 4, DataFieldIndex);

                    // struct fish_ranking_listing {
                    //     uint8 name[16];     // 00:15
                    //     uint8 mjob;         // 16
                    //     uint8 sjob;         // 17
                    //     uint8 mlevel;       // 18
                    //     uint8 slevel;       // 19
                    //     uint8 race;         // 20
                    //     uint8 padding;      // 21
                    //     uint8 allegience;   // 22
                    //     uint8 fishrank;     // 23
                    //     uint32 score;       // 24:27
                    //     uint32 submittime;  // 28:31
                    //     uint8 contestrank;  // 32
                    //     uint8 resultcount;  // 33
                    //     uint8 dataset_a;    // 34
                    //     uint8 dataset_b;    // 35
                    // };

                    // grab message length from before this offset
                    var fishrankentrycountforloop = (PD.GetUInt32AtPos(Offset - 4) / 36);
                    fishrankentrycountforloop++; // add one for self data

                    for (int i = 0; i < fishrankentrycountforloop; i++)
                    {
                        int thisoffset = Offset + 4 + (i * 36);
                        var fishline = string.Empty;
                        // check mainjob to see if this is a valid entry
                        if (PD.GetByteAtPos(thisoffset + 16) == 0)
                        {
                            fishline = "none";
                        }
                        else
                        {
                            fishline += "Rank: " + PD.GetByteAtPos(thisoffset + 32).ToString().PadLeft(3);
                            fishline += "/" + PD.GetByteAtPos(thisoffset + 33).ToString().PadLeft(3);
                            fishline += " ";
                            fishline += "Score: " + PD.GetUInt32AtPos(thisoffset + 24).ToString().PadLeft(4);
                            fishline += " ";
                            fishline += "SubmitTime: 0x" + PD.GetUInt32AtPos(thisoffset + 28).ToString("X8");
                            fishline += " - ";
                            var job = ffxiLookup.NLU(EngineFFXI.LU_Job).GetValue(PD.GetByteAtPos(thisoffset + 16)) + PD.GetByteAtPos(thisoffset + 18).ToString("00");
                            if (PD.GetByteAtPos(thisoffset + 17) > 0)
                            {
                                job += "/";
                                job += ffxiLookup.NLU(EngineFFXI.LU_Job).GetValue(PD.GetByteAtPos(thisoffset + 17));
                                job += PD.GetByteAtPos(thisoffset + 19).ToString("00");
                            }
                            fishline += job.PadRight(11);
                            fishline += " ";
                            fishline += ffxiLookup.NLU("raceandgender").GetValue(PD.GetByteAtPos(thisoffset + 20), "?").PadRight(10); // race
                            fishline += " ";
                            fishline += ffxiLookup.NLU("nations").GetValue(PD.GetByteAtPos(thisoffset + 22), "Unknown").PadRight(10); // Allegience
                            fishline += "(" + PD.GetByteAtPos(thisoffset + 23).ToString() + ")";
                            fishline += " - ";
                            fishline += PD.GetStringAtPos(thisoffset, 16).PadRight(16);
                            fishline += " - ";
                            fishline += "Data-A: 0x" + PD.GetByteAtPos(thisoffset + 34).ToString("X2");
                            fishline += " ";
                            fishline += "Data-B: 0x" + PD.GetByteAtPos(thisoffset + 35).ToString("X2");
                        }

                        AddDataField(thisoffset, 36);
                        var fishFieldName = nameField + " " + (i + 1).ToString() + "/" + fishrankentrycountforloop.ToString();
                        if (i == 0)
                            fishFieldName = nameField + " self";
                        AddParseLineToView(DataFieldIndex, "0x" + thisoffset.ToString("X2"), GetDataColor(DataFieldIndex), fishFieldName.ToString(), fishline, descriptionField);
                        MarkParsed(thisoffset, 36, DataFieldIndex);
                    }

                }
                else
                if (typeField == "packet-in-0x028")
                {
                    // This packet is too complex to do the normal way (for now)
                    var bytesfor0x028 = Parse_Packet_In_0x028(PD, ref DataFieldIndex);
                    // Mark any byts that didn't get marked as parsed, as parsed
                    for (int i = 0; i < bytesfor0x028; i++)
                    {
                        if ((i < ParsedBytes.Count) && (ParsedBytes[i] == 0))
                            ParsedBytes[i] = 0xFF;
                    }
                }
                else
                {
                    // Unknown field type
                    AddDataField(Offset, 0);
                    AddParseLineToView(0xFF, "L " + parseLineNumber.ToString(), GetDataColor(DataFieldIndex), "Parse Error", "Unknown Field Type: " + typeField);
                }

            }

            ParseUnusedData(ref DataFieldIndex);

            //ToGridView(DGV);
        }

        protected string ByteToRotation(byte b)
        {
            int i = (b * 360) / 256;
            double rads = ((Math.PI * 2) / 360 * i);
            return CompasDirectionNames[(i / 16) % 16] + " (" + b.ToString() + " - 0x" + b.ToString("X2") + " ≈ " + i.ToString() + "° = " + rads.ToString("F") + " rad)";
        }
    }

}
