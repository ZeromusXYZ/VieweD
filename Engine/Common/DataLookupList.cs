using System;
using System.Collections.Generic;

namespace VieweD.Engine.Common
{
    public class DataLookupList
    {
        public Dictionary<ulong, DataLookupEntry> Data { get; set; } = new Dictionary<ulong, DataLookupEntry>();

        public virtual string GetValue(ulong id, string defaultValue)
        {
            if (defaultValue == null)
                defaultValue = string.Empty;
                
            var res = GetValue(id);
            if (res == "")
            {
                return defaultValue == "?" ? id.ToString() : defaultValue;
            }
            else
                return res;
        }

        public virtual string GetValue(ulong id)
        {
            return Data.TryGetValue(id, out var res) ? res.Val : "";
        }

        public virtual string GetExtra(ulong id)
        {
            return Data.TryGetValue(id, out var res) ? res.Extra : "";
        }
        
    }
}
