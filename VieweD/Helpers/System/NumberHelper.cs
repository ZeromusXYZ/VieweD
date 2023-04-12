using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace VieweD.Helpers.System
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

        public static string ToHex(this byte val, int digits = 2)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToBinary(this byte val, int bits = 8)
        {
            var res = string.Empty;
            var startMask = 0x01 << (bits - 1);

            for (var bitMask = startMask; bitMask >= 0x01; bitMask >>= 1)
                res += ((val & bitMask) != 0 ? "1" : "0");

            return "0b" + res;
        }

        public static string ToHex(this sbyte val, int digits = 2)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this short val, int digits = 4)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this ushort val, int digits = 4)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this int val, int digits = 8)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this uint val, int digits = 8)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this long val, int digits = 16)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string ToHex(this ulong val, int digits = 16)
        {
            return "0x" + val.ToString("X" + digits);
        }

        public static string AsMilliseconds(this uint val)
        {
            var r = val % 1000;
            var v = val / 1000;
            var res = r.ToString("0000") + "ms";
            if (v <= 0)
                return res;

            r = v % 60;
            v /= 60;
            res = r.ToString("00") + "s " + res;

            if (v <= 0)
                return res;

            r = v % 60;
            v /= 60;
            res = r.ToString("00") + "m " + res;

            if (v <= 0)
                return res;

            r = v % 24;
            v /= 24;
            res = r.ToString("00") + "h " + res;

            if (v > 0)
                res = v + "d " + res;

            return res;
        }
    }

    public static class NumberHelper
    {
        /// <summary>
        /// Parse a string as a int (int32) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParse(string field, out int res)
        {
            bool result;

            // Handle notation for forced positive and negative values
            var isNegative = field.StartsWith("-");

            // Remove the sign from the start of the string
            if (isNegative)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                result = int.TryParse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                result = int.TryParse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in ????h notation (classic C)
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                result = int.TryParse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            {
                result = int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out res);
            }

            // Re-apply negative sign if required
            if (isNegative)
                res *= -1;

            return result;
        }

        /// <summary>
        /// Parse a string as a long (int64) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParse(string field, out long res)
        {
            bool result;

            // Handle notation for forced positive and negative values
            var isNegative = field.StartsWith("-");

            // Remove the sign from the start of the string
            if (isNegative)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                result = long.TryParse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                result = long.TryParse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in ????h notation (classic C)
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                result = long.TryParse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            {
                // Handle it as a regular long
                result = long.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out res);
            }

            // Re-apply negative sign if required
            if (isNegative)
                res *= -1;

            return result;
        }

        /// <summary>
        /// Parse a string as a ulong (uint64) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParse(string field, out ulong res)
        {
            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                return ulong.TryParse(field.Substring(2, field.Length - 2),
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                return ulong.TryParse(field.Substring(1, field.Length - 1),
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Handle Hex numbers in ????h notation (classic C)
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                return ulong.TryParse(field.Substring(0, field.Length - 1),
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            return ulong.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out res);
        }

        public static string BytesToHexString(byte[] bytes, string spacer = "")
        {
            var res = string.Empty;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (res != string.Empty)
                    res += spacer;
                res += bytes[i].ToString("X2");
            }

            return res;
        }

        public static List<byte> HexStringToBytes(string text)
        {
            var res = new List<byte>();
            var currentByte = string.Empty;
            foreach (var c in text.ToUpper())
            {
                if (((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'F')))
                    currentByte += c;
                if ((currentByte.Length > 1) && byte.TryParse(currentByte, NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out var b))
                {
                    res.Add(b);
                    currentByte = string.Empty;
                }
            }
            return res;
        }

        public static string ByteToBits(byte b)
        {
            var res = "";

            for (var i = 1; i < 256; i <<= 1)
            {
                if (i == 16)
                    res = " " + res;

                if ((b & i) != 0)
                {
                    res = "1" + res;
                }
                else
                {
                    res = "0" + res;
                }
            }

            return res;
        }
    }
}