using System.Globalization;
using VieweD.engine.common;

namespace VieweD.data.ffxi.engine;

public class FfxiPacketeerInputReader : BaseInputReader
{
    public override string Name => "FFXI Packeteer";
    public override string Description => "Supports .txt files containing Final Fantasy XI capture data made by Packeteer for Ashita";
    public override string DataFolder => "ffxi";

    private StreamReader? _reader { get; set; } = null;

    // Source: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=netframework-4.7.2#System_DateTime_Parse_System_String_System_IFormatProvider_System_Globalization_DateTimeStyles_
    // Assume a date and time string formatted for the fr-FR culture is the local 
    // time and convert it to UTC.
    // dateString = "2008-03-01 10:00";
    public static CultureInfo
        CultureForDateTimeParse =
            CultureInfo.CreateSpecificCulture("fr-FR"); // French seems to best match for what we need here

    public static DateTimeStyles StylesForDateTimeParse = DateTimeStyles.AssumeLocal;

    public FfxiPacketeerInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".txt");
    }

    public FfxiPacketeerInputReader() : base()
    {
        ExpectedFileExtensions.Add(".txt");
    }

    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new FfxiPacketeerInputReader(parentProject);
    }


    public override bool Open(Stream source)
    {
        try
        {
            _reader = new StreamReader(source);
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
        if (_reader == null)
            return -1;

        var packetCounter = 0;
        try
        {
            var allText = _reader.ReadToEnd().Replace("\r", "").Split('\n').ToList();

            ParentProject?.OnInputProgressUpdate(this, 0, allText.Count);

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
                ParentProject?.OnInputProgressUpdate(this, progressCounter, allText.Count);

                string sLower = s.ToLower().Trim(' ').Replace("\r", "");
                if (sLower != string.Empty && packetData == null)
                {
                    hasHadDataHeader = true;

                    if (ParentProject == null)
                        return -1;

                    // Begin building a new packet
                    packetData = new BasePacketData(ParentProject);
                    packetData.HeaderText = s;
                    packetData.OriginalHeaderText = s;

                    if (sLower.IndexOf("[s->c]", StringComparison.InvariantCulture) >= 0)
                    {
                        packetData.PacketDataDirection = PacketDataDirection.Incoming;
                        isUndefinedPacketDirection = false;
                    }
                    else if (sLower.IndexOf("[c->s]", StringComparison.InvariantCulture) >= 0)
                    {
                        packetData.PacketDataDirection = PacketDataDirection.Outgoing;
                        isUndefinedPacketDirection = false;
                    }
                    else if (sLower.IndexOf("npc id:", StringComparison.InvariantCulture) >= 0)
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

                        if (ParentProject.PacketDataDirectionDialog(packetData, out var newDirection))
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
                        if (sLower.IndexOf("--------------------------------", StringComparison.InvariantCulture) >= 0)
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

            ParentProject?.OnInputProgressUpdate(this, allText.Count, allText.Count);

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

    /// <summary>
    /// Add a formatted hex text line as data (packeteer specific)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public int AddRawPacketeerLineAsBytes(string s, BasePacketData packetData)
    {
        /* Example:
        //           1         2         3         4         5         6         7         8         9
        // 0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        // [C->S] Id: 001A | HeaderSize: 28
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
            packetData.PacketId = 0xFFFF;
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

        packetData.VirtualTimeStamp = packetData.TimeStamp;

        packetData.BuildHeaderText();

        return true;
    }
}