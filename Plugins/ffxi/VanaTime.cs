using System;

namespace VieweD.Engine.FFXI
{
    public enum VanadielDayOfWeek { Firesday = 0, Earthsday = 1, Watersday = 2, Windsday = 3, Iceday = 4, Thundersday = 5, Lightsday = 6, Darksday = 7 }

    public class VanadielTime
    {
        static public int EarthEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        static public int VanaEpoch = 1009810800 ;
        static public DateTime VanadielBirthUTCTime = new DateTime(2002, 6, 23, 15, 0, 0, DateTimeKind.Utc);
        static private long VanadielSecondsSinceYear0 = 27933984000; // => (((898 * 360) + 30) * 24 * 60 * 60);
        //private static readonly DateTime EarthTimeYear0 = new DateTime(VanadielBirthUTCTime.Ticks - ((VanadielSecondsSinceYear0 * 1000) / 25));
        private static TimeZoneInfo defaultServerTimeZone;
        static public TimeZoneInfo DefaultServerTimeZone { get => defaultServerTimeZone; set => defaultServerTimeZone = value; }
        static private int VanadielSecondsInMinute = 60;
        static private int VanadielMinutesInHour = 60;
        static private int VanadielSecondsInHour = VanadielSecondsInMinute * VanadielMinutesInHour ;
        static private int VanadielHoursInDay = 24;
        static private int VanadielSecondsInDay = VanadielSecondsInHour * VanadielHoursInDay ;
        static private int VanadielDaysInMonth = 30;
        static private int VanadielSecondsInMonth = VanadielSecondsInDay * VanadielDaysInMonth ;
        static private int VanadielMonthsInYear = 12;
        static private int VanadielSecondsInYear = VanadielSecondsInMonth * VanadielMonthsInYear ;

        private DateTime localEarthTime;
        private DateTime serverEarthTime;
        private DateTime utcEarthTime;
        private TimeZoneInfo serverTimeZone;
        //private int unixServerTime;
        private int vanaYear;
        private int vanaMonth;
        private int vanaDay;
        private VanadielDayOfWeek vanaDoW;
        private int vanaHour;
        private int vanaMinute;
        private int vanaSecond;
        private long vanaRawTime;

        public DateTime LocalEarthTime
        {
            get => localEarthTime;
            set
            {
                localEarthTime = value;
                serverEarthTime = TimeZoneInfo.ConvertTime(value, serverTimeZone);
                utcEarthTime = TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.Local);
                UpdateVanaTimeFromServerTime();
            }
        }
        public DateTime ServerEarthTime {
            get => serverEarthTime;
            set
            {
                serverEarthTime = value;
                localEarthTime = TimeZoneInfo.ConvertTime(value, serverTimeZone, TimeZoneInfo.Local);
                utcEarthTime = TimeZoneInfo.ConvertTimeToUtc(value, serverTimeZone);
                UpdateVanaTimeFromServerTime();
            }
        }
        public TimeZoneInfo ServerTimeZone { get => serverTimeZone; set => serverTimeZone = value; }
        public int VanaYear { get => vanaYear; private set => vanaYear = value; }
        public int VanaMonth { get => vanaMonth; private set => vanaMonth = value; }
        public int VanaDay { get => vanaDay; private set => vanaDay = value; }
        public VanadielDayOfWeek VanaDoW { private get => vanaDoW; set => vanaDoW = value; }
        public int VanaHour { get => vanaHour; private set => vanaHour = value; }
        public int VanaMinute { get => vanaMinute; private set => vanaMinute = value; }
        public int VanaSecond { get => vanaSecond; private set => vanaSecond = value; }
        public long VanaRawTime { get => vanaRawTime; }

        public VanadielTime()
        {
            InitVanaTime();
            LocalEarthTime = DateTime.Now;
        }

        public VanadielTime(DateTime serverNowTime)
        {
            InitVanaTime();
            LocalEarthTime = serverNowTime;
        }

        static public Int32 DateTimeToUnixTime(DateTime dt)
        {
            return (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        static public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dt = dt.AddSeconds(unixTimeStamp);
            return dt;
        }

        private void InitVanaTime()
        {
            // Initialize server time zone (for first use)
            if (DefaultServerTimeZone == null)
                DefaultServerTimeZone = FindJST();
            ServerTimeZone = DefaultServerTimeZone;
        }

        private TimeZoneInfo FindJST()
        {
            var tzi = TimeZoneInfo.GetSystemTimeZones();
            foreach (var zi in tzi)
            {
                if (zi.Id.ToLower().IndexOf("tokyo") >= 0)
                {
                    return zi;
                }
            }
            // Failed falls back to local time
            return TimeZoneInfo.Local;
        }

        private void UpdateVanaTimeFromServerTime()
        {
            //unixServerTime = DateTimeToUnixTime(serverEarthTime);

            var eMilliSinceVanaEpoch = utcEarthTime.Ticks - VanadielBirthUTCTime.Ticks;
            vanaRawTime = ((long)Math.Round((decimal)eMilliSinceVanaEpoch / 1000 / 10000 * 25)) + VanadielSecondsSinceYear0;

            vanaYear = (int)(vanaRawTime / VanadielSecondsInYear);
            vanaMonth = (int)((vanaRawTime / VanadielSecondsInMonth) % VanadielMonthsInYear)+1;
            vanaDay = (int)((vanaRawTime / VanadielSecondsInDay) % VanadielDaysInMonth)+1;
            vanaDoW = (VanadielDayOfWeek)((vanaRawTime / VanadielSecondsInDay) % 8);
            vanaHour = (int)((vanaRawTime / VanadielSecondsInHour) % VanadielHoursInDay);
            vanaMinute = (int)((vanaRawTime / VanadielSecondsInMinute) % VanadielMinutesInHour);
            vanaSecond = (int)(vanaRawTime % VanadielSecondsInMinute);
        }

        public static string VanadielDayOfWeekByteToString(byte dow)
        {
            switch (dow)
            {
                case 0: return "Firesday";
                case 1: return "Earthsday";
                case 2: return "Watersday";
                case 3: return "Windsday";
                case 4: return "Iceday";
                case 5: return "Thundersday";
                case 6: return "Lightsday";
                case 7: return "Darksday";
                default: return "DoW " + dow.ToString();
            }
        }


        public string YearType()
        {
            if (VanaYear >= 0)
                return "C.E.";
            else
                return "B.C.";
        }

        public override string ToString()
        {
            return VanaDoW.ToString() + "  " + 
                VanaYear.ToString() + "/" + VanaMonth.ToString("00") + "/" + VanaDay.ToString("00") + "  " + 
                VanaHour.ToString("00") + ":" + VanaMinute.ToString("00");
        }

        public void FromVanadielIntTime(int aTime)
        {
            var st = UnixTimeStampToDateTime(aTime + VanaEpoch);
            ServerEarthTime = st;
        }


    }
}
