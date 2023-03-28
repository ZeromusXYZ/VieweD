using System;
using System.Globalization;
using System.IO;
using System.Linq;
using VieweD.engine.common;

namespace VieweD.data.ffxi.engine;

public class FfxiPacketViewerInputReader : BaseInputReader
{
    public override string Name => "FFXI Packet Viewer";
    public override string Description => "Supports .log files containing Final Fantasy XI capture data made by the Packet Viewer plugin for Windower";
    public override string DataFolder => "ffxi";

    private StreamReader? Reader { get; set; }

    // Source: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=netframework-4.7.2#System_DateTime_Parse_System_String_System_IFormatProvider_System_Globalization_DateTimeStyles_
    // Assume a date and time string formatted for the fr-FR culture is the local 
    // time and convert it to UTC.
    // dateString = "2008-03-01 10:00";
    public static readonly CultureInfo
        CultureForDateTimeParse =
            CultureInfo.CreateSpecificCulture("fr-FR"); // French seems to best match for what we need here

    public static readonly DateTimeStyles StylesForDateTimeParse = DateTimeStyles.AssumeLocal;

    public FfxiPacketViewerInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".log");

        InitReader();
    }

    // ReSharper disable once UnusedMember.Global
    public FfxiPacketViewerInputReader()
    {
        ExpectedFileExtensions.Add(".log");
    }

    private void InitReader()
    {
        ParentProject!.PortToStreamIdMapping.Clear();
        ParentProject.RegisterPort(0, "Game", "G"); // 0 - Base Game
    }

    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new FfxiPacketViewerInputReader(parentProject);
    }

    public override bool Open(Stream source)
    {
        try
        {
            Reader = new StreamReader(source);
            // NOTE: PacketViewer uses a text based log that doesn't have any real file header, only packet headers
            return true;
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return false;
        }
    }

    public override int ReadAllData()
    {
        if (Reader == null)
            return -1;

        if (ParentProject == null)
            return -1;

        ParentProject.TimeStampFormat = "HH:mm:ss";

        var packetCounter = 0;
        try
        {
            var allText = Reader.ReadToEnd().Replace("\r", "").Split('\n').ToList();

            ViewedProjectTab.OnInputProgressUpdate(this, 0, allText.Count);

            #region read_file_data_lines

            BasePacketData? packetData = null;
            var hasHadDataHeader = false;
            var pastStartOfDataMarker = false;
            var askForPacketType = true;
            var preferredDirection = PacketDataDirection.Unknown;

            var progressCounter = 0;
            foreach (var s in allText)
            {
                progressCounter++;
                ViewedProjectTab.OnInputProgressUpdate(this, progressCounter, allText.Count);

                string sLower = s.ToLower().Trim(' ').Replace("\r", "");
                if (sLower != string.Empty && packetData == null)
                {
                    hasHadDataHeader = true;
                    pastStartOfDataMarker = false;

                    if (ParentProject == null)
                        return -1;

                    // Begin building a new packet
                    packetData = new BasePacketData(ParentProject)
                    {
                        HeaderText = s,
                        OriginalHeaderText = s
                    };

                    bool isUndefinedPacketDirection;
                    if (sLower.Contains("incoming", StringComparison.InvariantCulture))
                    {
                        packetData.PacketDataDirection = PacketDataDirection.Incoming;
                        isUndefinedPacketDirection = false;
                    }
                    else if (sLower.Contains("outgoing", StringComparison.InvariantCulture))
                    {
                        packetData.PacketDataDirection = PacketDataDirection.Outgoing;
                        isUndefinedPacketDirection = false;
                    }
                    else if (sLower.Contains("npc id:", StringComparison.InvariantCulture))
                    {
                        // This is likely a npc logger log file, assume it's a incoming packet
                        packetData.PacketDataDirection = PacketDataDirection.Incoming;
                        isUndefinedPacketDirection = false;
                    }
                    else
                    {
                        packetData.PacketDataDirection = preferredDirection;
                        isUndefinedPacketDirection = true;
                    }


                    if (
                        // Not a comment or empty line
                        s != "" && !s.StartsWith("--") &&
                        // Unknown packet and we need to know ?
                        isUndefinedPacketDirection && askForPacketType &&
                        packetData.PacketDataDirection == PacketDataDirection.Unknown
                    )
                    {
                        askForPacketType = false;
                        // preferredDirection = PacketDataDirection.Incoming;
                        // isUndefinedPacketDirection = false;

                        packetData.PacketDataDirection = preferredDirection;

                        if (ViewedProjectTab.PacketDataDirectionDialog(packetData, out var newDirection))
                        {
                            packetData.PacketDataDirection = newDirection;
                        }
                        // else
                        // {
                        //    isUndefinedPacketDirection = true;
                        // }
                        
                    }

                    packetData.SourceText.Add(s);
                } // end start new packet
                else if (hasHadDataHeader && sLower != string.Empty && packetData != null)
                {
                    // Add line of data
                    packetData.SourceText.Add(s);

                    // Actual packet data starts at the 3rd line after the header
                    if (pastStartOfDataMarker)
                    {
                        _ = AddRawLineAsBytes(s, packetData);
                    }
                    else
                    {
                        // a reasonable amount of dashes line (32 chars) to mark the start of the data
                        if (sLower.Contains("--------------------------------", StringComparison.InvariantCulture))
                            pastStartOfDataMarker = true;
                    }
                }
                else if (hasHadDataHeader && sLower == string.Empty && packetData != null)
                {
                    // Close this packet and add it to list
                    if (CompileData(packetData))
                    {
                        //packetData.CompileSpecial(packetList);
                        // Set zone after CompileSpecial, this is only needed if not captured by PacketDB
                        //packetData.CapturedZoneId = packetList.CurrentParseZone;

                        /*
                        if (packetList.IsPreParsed)
                        {
                            packetData.PP = packetData.ParentProject.ParentProject.InputParser.GetParser(packetData);

                            if (packetData.PP != null)
                            {
                                packetData.PP.AssignPacket(packetData);
                                packetData.PP.ParseData("-");
                            }
                        }
                        */

                        ParentProject?.OnInputDataRead(this, packetData);
                    }
                    else
                    {
                        // Invalid data
                    }

                    // reset our packet holder
                    packetData = null;
                }
                else if (string.IsNullOrWhiteSpace(sLower) && packetData == null)
                {
                    // Blank line
                }
                else if (sLower.StartsWith("--") && packetData != null)
                {
                    // Comment
                }
                else
                {
                    // ERROR, this should not be possible in a valid file, but just let's ignore it anyway, just in case
                }

            } // end foreach datafile line

            ViewedProjectTab.OnInputProgressUpdate(this, allText.Count, allText.Count);

            #endregion
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return -1;
        }

        return packetCounter;
    }

    /// <summary>
    /// Add a formatted hex text line as data (generic)
    /// </summary>
    /// <param name="line"></param>
    /// <param name="packetData"></param>
    /// <returns></returns>
    private static int AddRawLineAsBytes(string line, BasePacketData packetData)
    {
        // Removes spaces and tabs
        var simpleLine = line.Replace(" ", "").Replace("\t", "");

        // Data should start after the first pipeline symbol
        var dataStartPos = simpleLine.IndexOf("|", StringComparison.InvariantCulture) + 1;
        if (simpleLine.Length < dataStartPos + 32)
        {
            // Data seems too short
            return 0;
        }

        var dataString = simpleLine.Substring(dataStartPos, 32); // max 32 hex digits expect

        var c = 0;
        for (var i = 0; i <= 0xf; i++)
        {
            var h = dataString.Substring(i * 2, 2);
            if (h != "--")
            {
                if (byte.TryParse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                    packetData.ByteData.Add(b);
                c++;
            }
        }

        return c;

        /* Example:
        //        1         2         3         4         5         6         7         8         9
        01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890

        [2018-05-16 18:11:35] Outgoing packet 0x015:
                |  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F      | 0123456789ABCDEF
            -----------------------------------------------------  ----------------------
              0 | 15 10 9E 00 CF 50 A0 C3 04 0E 1C C2 46 BF 33 43    0 | .....P......F.3C
              1 | 00 00 02 00 5D 00 00 00 49 97 B8 69 00 00 00 00    1 | ....]...I..i....
        ...
              5 | 00 00 00 00 -- -- -- -- -- -- -- -- -- -- -- --    5 | ....------------
        */
    }

    public bool CompileData(BasePacketData packetData)
    {
        if (packetData.ByteData.Count < 4)
        {
            packetData.PacketId = uint.MaxValue;
            packetData.PacketDataSize = 0;
            packetData.HeaderText = "Invalid Packet Size < 4";
            return false;
        }

        packetData.PacketId = (ushort)(packetData.GetByteAtPos(0) + (packetData.GetByteAtPos(1) & 0x01) * 0x100);
        packetData.PacketDataSize = (ushort)((packetData.GetByteAtPos(1) & 0xFE) * 2);
        packetData.SyncId = (ushort)(packetData.GetByteAtPos(2) + packetData.GetByteAtPos(3) * 0x100);

        // Try to determine timestamp from header
        var p1 = packetData.OriginalHeaderText.IndexOf('[');
        var p2 = packetData.OriginalHeaderText.IndexOf(']');
        if (p1 >= 0 && p2 >= 0 && p2 > p1)
        {
            var originalTimeString = packetData.OriginalHeaderText.Substring(p1 + 1, p2 - p1 - 1);
            if (originalTimeString.Length > 0)
            {
                try
                {
                    // try quick-parse first
                    packetData.TimeStamp = DateTimeParse(originalTimeString, out var dt)
                        ? dt
                        : DateTime.Parse(originalTimeString, CultureForDateTimeParse, StylesForDateTimeParse);
                }
                catch
                {
                    packetData.TimeStamp = new DateTime(0);
                }
            }
        }

        if (packetData.ParentProject.LoadedPacketList.Count > 0)
        {
            var firstPacketTime = packetData.ParentProject.LoadedPacketList[0].TimeStamp;
            packetData.OffsetFromStart = packetData.TimeStamp - firstPacketTime;
            packetData.VirtualOffsetFromStart = packetData.OffsetFromStart;
        }

        packetData.BuildHeaderText();

        return true;
    }
}