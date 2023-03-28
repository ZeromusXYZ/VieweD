using System;
using System.Drawing;

namespace VieweD.engine.common;

public class ParsedField
{
    public bool HasValue { get; set; }
    public int StartingByte { get; set; }
    public int EndingByte { get; set; }
    public string DisplayedByteOffset { get; set; }
    public string FieldName { get; set; }
    public string FieldValue { get; set; }
    public int NestingDepth { get; set; }
    public Color FieldColor { get; set; }
    public bool IsSelected { get; set; }

    public ParsedField()
    {
        HasValue = false;
        StartingByte = -1;
        EndingByte = -1;
        DisplayedByteOffset = string.Empty;
        FieldName = string.Empty;
        FieldValue = string.Empty;
        NestingDepth = 0;
        FieldColor = SystemColors.WindowText;
        IsSelected = false;
    }

    public bool MatchSearch(SearchParameters search, BasePacketData data)
    {
        // nothing to search?
        if ((search.SearchOutgoing == false) && (search.SearchIncoming == false))
            return false;

        var matchField = FieldValue.Contains(search.SearchParsedFieldValue, StringComparison.InvariantCultureIgnoreCase);
        // Check if field name is searched
        var matchName = (search.SearchParsedFieldName == "") || FieldName.Contains(search.SearchParsedFieldName, StringComparison.InvariantCultureIgnoreCase);
        var res = matchName;

        // Only field value is searched
        res = res && ((search.SearchParsedFieldValue == "") || matchField);

        // Check related bytes
        if (HasValue && res && search.SearchByByte)
        {
            res = false;
            for (var i = StartingByte; i <= EndingByte; i++)
            {
                if (search.SearchByte == data.GetByteAtPos(i))
                {
                    res = true;
                    break;
                }
            }
        }
        else
        if (HasValue && res && search.SearchByUInt16)
        {
            res = false;
            for (var i = StartingByte; i <= EndingByte-1; i++)
            {
                if (search.SearchUInt16 == data.GetUInt16AtPos(i))
                {
                    res = true;
                    break;
                }
            }
        }
        else
        if (HasValue && res && search.SearchByUInt24)
        {
            res = false;
            for (var i = StartingByte; i <= EndingByte - 2; i++)
            {
                if (search.SearchUInt24 == data.GetUInt24AtPos(i))
                {
                    res = true;
                    break;
                }
            }
        }
        else
        if (HasValue && res && search.SearchByUInt32)
        {
            res = false;
            for (var i = StartingByte; i <= EndingByte - 3; i++)
            {
                if (search.SearchUInt16 == data.GetUInt32AtPos(i))
                {
                    res = true;
                    break;
                }
            }
        }
        else
        {
            res = search.SearchByParsedData && matchName && matchField;
        }

        return res;
    }
}