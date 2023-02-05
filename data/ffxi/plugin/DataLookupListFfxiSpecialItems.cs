using System.Collections.Generic;
using VieweD.Engine.Common;

namespace VieweD.Engine.FFXI
{
    public class DataLookupListFfxiSpecialItems : DataLookupList
    {
        public Dictionary<uint, FFXI_Item> Items = new Dictionary<uint, FFXI_Item>();

        public void UpdateData()
        {
            if (Items.Count <= 0)
                return;

            Data.Clear();
            foreach (var i in Items)
            {
                var dle = new DataLookupEntry
                {
                    Id = i.Value.Id,
                    Val = i.Value.Name,
                    Extra = i.Value.Description
                };
                Data.Add(dle.Id, dle);
            }
        }

        public override string GetValue(ulong id)
        {
            try
            {
                if (Items.TryGetValue((uint)id, out var i))
                    return i.Name;
                return "<item not found: " + id.ToString() + ">";
            }
            catch
            {
                return "<exception on item: " + id.ToString() + ">";
            }
        }
    }
}