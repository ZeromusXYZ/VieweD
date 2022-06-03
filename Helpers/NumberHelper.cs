using System;

namespace VieweD.Helpers
{
    public static class NumericExtensions
    {
        public static double DegToRad(this double val)
        {
            return (Math.PI / 180f) * val;
        }

        public static float DegToRad(this float val)
        {
            return (float)(Math.PI / 180f) * val;
        }

        public static double RadToDeg(this double val)
        {
            return val / Math.PI * 180f;
        }

        public static float RadToDeg(this float val)
        {
            return (float)(val / Math.PI * 180f);
        }
    }
}