namespace VieweD.Engine.Common
{
    public class SearchParameters
    {
        public string FileFormat { get; set; }
        public bool SearchIncoming { get; set; }
        public bool SearchOutgoing { get; set; }
        public bool SearchByPacketId { get; set; }
        public ushort SearchPacketId { get; set; }
        public bool SearchByPacketLevel { get; set; }
        public byte SearchPacketLevel { get; set; }
        public bool SearchBySync { get; set; }
        public ushort SearchSync { get; set; }
        public bool SearchByByte { get; set; }
        public byte SearchByte { get; set; }
        public bool SearchByUInt16 { get; set; }
        public ushort SearchUInt16 { get; set; }
        public bool SearchByUInt24 { get; set; }
        public uint SearchUInt24 { get; set; }
        public bool SearchByUInt32 { get; set; }
        public uint SearchUInt32 { get; set; }
        public bool SearchByParsedData { get; set; }
        public string SearchParsedFieldName { get; set; }
        public string SearchParsedFieldValue { get; set; }

        public bool HasSearchForData()
        {
            if (SearchByByte) return true;
            if (SearchByUInt16) return true;
            if (SearchByUInt24) return true;
            if (SearchByUInt32) return true;
            if (SearchByParsedData) return true;
            return false;
        }

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
}