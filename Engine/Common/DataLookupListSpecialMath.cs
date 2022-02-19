using System;

namespace VieweD.Engine.Common
{
    public class DataLookupListSpecialMath : DataLookupList
    {
        public string EvalString { get; set; }
        
        public static double EvalDouble(String expression)
        {
            using (var table = new System.Data.DataTable())
            {
                return Convert.ToDouble(table.Compute(expression, string.Empty));
            }
        }

        public static UInt64 EvalUInt64(String expression)
        {
            using (var table = new System.Data.DataTable())
            {
                return Convert.ToUInt64(table.Compute(expression, string.Empty));
            }
        }

        public override string GetValue(UInt64 id)
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