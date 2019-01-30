using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;

namespace Jackett.Common.Utils
{
    public static class ParseUtil
    {
        private static readonly Regex InvalidXmlChars =
            new Regex(
                @"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
                RegexOptions.Compiled);

        private static readonly Regex ImdbId = new Regex(@"^(?:tt)?(\d{1,7})$", RegexOptions.Compiled);

        public static string NormalizeSpace(string s)
        {
            return s.Trim();
        }

        public static string NormalizeMultiSpaces(string s)
        {
            return new Regex(@"\s+").Replace(NormalizeSpace(s), " ");
            ;
        }

        public static string NormalizeNumber(string s)
        {
            var normalized = NormalizeSpace(s);
            normalized = normalized.Replace("-", "0");
            normalized = normalized.Replace(",", "");
            return normalized;
        }

        public static string RemoveInvalidXmlChars(string text)
        {
            return string.IsNullOrEmpty(text) ? "" : InvalidXmlChars.Replace(text, "");
        }

        public static double CoerceDouble(string str)
        {
            return double.Parse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static float CoerceFloat(string str)
        {
            return float.Parse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static int CoerceInt(string str)
        {
            return int.Parse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static long CoerceLong(string str)
        {
            return long.Parse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static bool TryCoerceDouble(string str, out double result)
        {
            return double.TryParse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryCoerceFloat(string str, out float result)
        {
            return float.TryParse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryCoerceInt(string str, out int result)
        {
            return int.TryParse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryCoerceLong(string str, out long result)
        {
            return long.TryParse(NormalizeNumber(str), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static string GetArgumentFromQueryString(string url, string argument)
        {
            if (url == null || argument == null)
                return null;
            var qsStr = url.Split(new[] {'?'}, 2)[1];
            qsStr = qsStr.Split(new[] {'#'}, 2)[0];
            var qs = QueryHelpers.ParseQuery(qsStr);
            return qs[argument].FirstOrDefault();
        }

        public static long? GetLongFromString(string str)
        {
            if (str == null)
                return null;
            var IdRegEx = new Regex(@"(\d+)", RegexOptions.Compiled);
            var IdMatch = IdRegEx.Match(str);
            if (!IdMatch.Success)
                return null;
            var Id = IdMatch.Groups[1].Value;
            return CoerceLong(Id);
        }

        public static int? GetImdbID(string imdbstr)
        {
            if (imdbstr == null)
                return null;
            var match = ImdbId.Match(imdbstr);
            if (!match.Success)
                return null;

            return int.Parse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static string GetFullImdbID(string imdbstr)
        {
            var imdbid = GetImdbID(imdbstr);
            if (imdbid == null)
                return null;

            return "tt" + ((int) imdbid).ToString("D7");
        }

        public static long GetBytes(string str)
        {
            var valStr = new string(str.Where(c => char.IsDigit(c) || c == '.').ToArray());
            var unit = new string(str.Where(char.IsLetter).ToArray());
            var val = CoerceFloat(valStr);
            return GetBytes(unit, val);
        }

        public static long GetBytes(string unit, float value)
        {
            unit = unit
                .Replace("i", "")
                .ToLowerInvariant();

            if (unit.Contains("kb"))
            {
                return BytesFromKB(value);
            }

            if (unit.Contains("mb"))
            {
                return BytesFromMB(value);
            }

            if (unit.Contains("gb"))
            {
                return BytesFromGB(value);
            }

            if (unit.Contains("tb"))
            {
                return BytesFromTB(value);
            }

            return (long) value;
        }

        private static long BytesFromTB(float tb) => BytesFromGB(tb * 1024f);

        private static long BytesFromGB(float gb) => BytesFromMB(gb * 1024f);

        private static long BytesFromMB(float mb) => BytesFromKB(mb * 1024f);

        private static long BytesFromKB(float kb) => (long) (kb * 1024f);
    }
}