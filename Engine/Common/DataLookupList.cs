using System;
using System.Collections.Generic;

namespace VieweD.Engine.Common
{
    public class DataLookupList
    {
        public Dictionary<UInt64, DataLookupEntry> Data = new Dictionary<UInt64, DataLookupEntry>();

        public virtual string GetValue(UInt64 id, string defaultValue = "")
        {
            if (defaultValue == null)
                defaultValue = string.Empty;
                
            var res = GetValue(id);
            if (res == "")
            {
                if (defaultValue == "?")
                    return id.ToString();
                else
                    return defaultValue;
            }
            else
                return res;
        }

        public virtual string GetValue(UInt64 id)
        {
            DataLookupEntry res;
            if (Data.TryGetValue(id, out res))
                return res.Val;
            return "";
        }

        public virtual string GetExtra(UInt64 id)
        {
            DataLookupEntry res;
            if (Data.TryGetValue(id, out res))
                return res.Extra;
            return "";
        }
        
    }
}
