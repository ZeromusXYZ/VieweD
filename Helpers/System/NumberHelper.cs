using System;
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
                result = int.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
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
                result = long.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
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

            return ulong.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
        }
    }
}