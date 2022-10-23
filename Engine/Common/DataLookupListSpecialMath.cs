using System;

namespace VieweD.Engine.Common
{
    public class DataLookupListSpecialMath : DataLookupList
    {
        public string EvalString { get; set; }
        
        public static double EvalDouble(string expression)
        {
            using (var table = new System.Data.DataTable())
            {
                return Convert.ToDouble(table.Compute(expression, string.Empty));
            }
        }

        public static ulong EvalUInt64(string expression)
        {
            using (var table = new System.Data.DataTable())
            {
                return Convert.ToUInt64(table.Compute(expression, string.Empty));
            }
        }

        public override string GetValue(ulong id)
        {
            try
            {
                var s = EvalString.Replace("?", id.ToString());
                return EvalUInt64(s).ToString();
            }
            catch
            {
                return "MATH-ERROR" ;
            }
        }
    }
}