using System;
using System.Globalization;
using System.IO;
using System.Linq;
using VieweD.engine.common;

namespace VieweD.data.ffxi.engine;

// ReSharper disable once UnusedMember.Global
/// <inheritdoc />
public class FfxiPacketeerInputReader : BaseInputReader
{
    public override string Name => "FFXI Packeteer";
    public override string Description => "Supports .txt files containing Final Fantasy XI capture data made by Packeteer for Ashita";
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

    public FfxiPacketeerInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".txt");

        InitReader();
    }

    public FfxiPacketeerInputReader()
    {
        ExpectedFileExtensions.Add(".txt");
    }

    private void InitReader()
    {
        ParentProject!.PortToStreamIdMapping.Clear();
        ParentProject.RegisterPort(0, "Game", "G"); // 0 - Base Game
    }


    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new FfxiPacketeerInputReader(parentProject);
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
            var isUndefinedPacketDirection = false;
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

                    if (ParentProject == null)
                        return -1;

                    // Begin building a new packet
                    packetData = new BasePacketData(ParentProject)
                    {
                        HeaderText = s,
                        OriginalHeaderText = s
                    };

                    if (sLower.Contains("[s->c]", StringComparison.InvariantCulture))
                    {
                        packetData.PacketDataDirection = PacketDataDirection.Incoming;
                        isUndefinedPacketDirection = false;
                    }
                    else if (sLower.Contains("[c->s]", StringComparison.InvariantCulture))
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
                        preferredDirection = PacketDataDirection.Incoming;
                        isUndefinedPacketDirection = false;
                        packetData.PacketDataDirection = preferredDirection;

                        if (ViewedProjectTab.PacketDataDirectionDialog(packetData, out var newDirection))
                        {
                            packetData.PacketDataDirection = newDirection;
                            isUndefinedPacketDirection = false;
                        }
                    }

                    packetData.SourceText.Add(s);
                } // end start new packet
                else 
                if (hasHadDataHeader && sLower != string.Empty && packetData != null)
                {
                    // Add line of data
                    packetData.SourceText.Add(s);

                    if (packetData.SourceText.Count > 1)
                    {
                        _ = AddRawPacketeerLineAsBytes(s, packetData);
                    }
                    else
                    {
                        // a reasonable amount of dashes line (32 chars) to mark the start of the data
                        if (sLower.Contains("--------------------------------", StringComparison.InvariantCulture))
                        {
                        }
                    }
                }
                else 
                if (hasHadDataHeader && sLower == string.Empty && packetData != null)
                {
                    // Close this packet and add it to list
                    if (CompileData(packetData))
                    {
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
    /// Add a formatted hex text line as data (packeteer specific)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="packetData"></param>
    /// <returns></returns>
    public static int AddRawPacketeerLineAsBytes(string s, BasePacketData packetData)
    {
        /* Example:
        //           1         2         3         4         5         6         7         8         9
        // 0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        // [C->S] PacketId: 001A | HeaderSize: 28
        //     1A 0E ED 24 D5 10 10 01 D5 00 00 00 00 00 00 00  ..í$Õ...Õ.......
        */

        if (s.StartsWith("--"))
        {
            // Still a comment, get out of here
            return 0;
        }

        if (s.Length < 51)
        {
            // Doesn't look like a correct format
            return 0;
        }

        var c = 0;
        for (var i = 0; i <= 0xf; i++)
        {
            var h = s.Substring(4 + (i * 3), 2);
            // If this fails, we're probably at the end of the packet
            // Unlike Windower, Packeteer doesn't add dashes for the blanks
            if ((h != "--") && (h != "  ") && (h != " "))
            {
                if (byte.TryParse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                    packetData.ByteData.Add(b);

                c++;
            }
        }
        return c;
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
            // If it's not at least 8 long, don't even bother to try to get the timestamp
            if (originalTimeString.Length > 8)
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
            else
            {
                packetData.TimeStamp = new DateTime(0);
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