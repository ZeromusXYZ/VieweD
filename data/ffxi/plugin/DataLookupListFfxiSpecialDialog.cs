using System;
using System.Collections.Generic;
using VieweD.Engine.Common;

namespace VieweD.Engine.FFXI
{
    public class DataLookupListFfxiSpecialDialog : DataLookupList
    {
        public bool EnableCache = true;
        private Dictionary<uint, FFXI_DialogTableEntry> dialogsCache = new Dictionary<uint, FFXI_DialogTableEntry>();

        public void UpdateData()
        {
            if (dialogsCache.Count <= 0)
                return;

            Data.Clear();
            foreach (var i in dialogsCache)
            {
                var dle = new DataLookupEntry
                {
                    Id = i.Value.Id,
                    Val = i.Value.Text,
                    Extra = string.Empty
                };
                Data.Add(dle.Id, dle);
            }
        }

        public override string GetValue(ulong id)
        {
            try
            {
                FFXI_DialogTableEntry i;
                if (dialogsCache.TryGetValue((uint)id, out i))
                {
                    return i.Text;
                }
                else
                {
                    ushort zone = (ushort)(id / 0x10000);
                    if (MakeZoneDialogCache(zone))
                    {
                        // try again if we loaded something
                        if (dialogsCache.TryGetValue((uint)id, out i))
                        {
                            return i.Text;
                        }
                    }
                }
                return "<dialog not found: 0x" + id.ToString("X8") + ">";
            }
            catch
            {
                return "<exception on dialog: 0x" + id.ToString("X8") + ">";
            }
        }

        public string GetValue(ushort zoneId, ushort dialogId)
        {
            return GetValue((UInt64)(dialogId + (zoneId * 0x10000)));
        }

        private bool MakeZoneDialogCache(ushort zoneId)
        {
            if (FFXIHelper.FFXI_FTable.Count <= 0)
                return false;
            return FFXIHelper.FFXI_LoadDialogsFromDats(ref dialogsCache, zoneId);
        }
    }
}