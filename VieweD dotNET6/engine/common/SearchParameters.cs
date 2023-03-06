namespace VieweD.engine.common;

public class SearchParameters
{
    public string FileFormat { get; set; } = string.Empty;
    public bool SearchIncoming { get; set; } = false;
    public bool SearchOutgoing { get; set; } = false;
    public bool SearchByPacketId { get; set; } = false;
    public ushort SearchPacketId { get; set; } = 0;
    public bool SearchByPacketLevel { get; set; } = false;
    public byte SearchPacketLevel { get; set; } = 0;
    public bool SearchBySync { get; set; } = false;
    public ushort SearchSync { get; set; } = 0;
    public bool SearchByByte { get; set; } = false;
    public byte SearchByte { get; set; } = 0;
    public bool SearchByUInt16 { get; set; } = false;
    public ushort SearchUInt16 { get; set; } = 0;
    public bool SearchByUInt24 { get; set; } = false;
    public uint SearchUInt24 { get; set; } = 0;
    public bool SearchByUInt32 { get; set; } = false;
    public uint SearchUInt32 { get; set; } = 0;
    public bool SearchByParsedData { get; set; } = false;
    public string SearchParsedFieldName { get; set; } = string.Empty;
    public string SearchParsedFieldValue { get; set; } = string.Empty;

    public bool HasSearchForData => SearchByByte || SearchByUInt16 || SearchByUInt24 || SearchByUInt32 || SearchByParsedData;
    public bool HasSearchForRaw => SearchByByte || SearchByUInt16 || SearchByUInt24 || SearchByUInt32;


    public void ClearValidSearchFlags()
    {
        // FileFormat = "Unknown";
        SearchIncoming = false;
        SearchOutgoing = false;
        SearchByPacketId = false;
        SearchByPacketLevel = false;
        SearchBySync = false;
        SearchByByte = false;
        SearchByUInt16 = false;
        SearchByUInt24 = false;
        SearchByUInt32 = false;
        SearchByParsedData = false;
    }

    public void CopyFrom(SearchParameters p)
    {
        FileFormat = p.FileFormat;
        SearchIncoming = p.SearchIncoming;
        SearchOutgoing = p.SearchOutgoing;
        SearchByPacketId = p.SearchByPacketId;
        SearchPacketId = p.SearchPacketId;
        SearchByPacketLevel = p.SearchByPacketLevel;
        SearchPacketLevel = p.SearchPacketLevel;
        SearchBySync = p.SearchBySync;
        SearchSync = p.SearchSync;
        SearchByByte = p.SearchByByte;
        SearchByte = p.SearchByte;
        SearchByUInt16 = p.SearchByUInt16;
        SearchUInt16 = p.SearchUInt16;
        SearchByUInt24 = p.SearchByUInt24;
        SearchUInt24 = p.SearchUInt24;
        SearchByUInt32 = p.SearchByUInt32;
        SearchUInt32 = p.SearchUInt32;
        SearchByParsedData = p.SearchByParsedData;
        SearchParsedFieldName = p.SearchParsedFieldName;
        SearchParsedFieldValue = p.SearchParsedFieldValue;
    }
}