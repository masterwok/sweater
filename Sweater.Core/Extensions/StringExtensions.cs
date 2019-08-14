using System;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// This class provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string to an Html Agility Pack HtmlDocument.
        /// </summary>
        /// <param name="html">A string containing HTML.</param>
        /// <returns>The converted HtmlDocument instance.</returns>
        public static HtmlDocument ToHtmlDocument(this string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        /// <summary>
        /// Parse the info hash contained within a magnet URI string.
        /// </summary>
        /// <param name="magnetUri">A magnet URI string (magnet://...).</param>
        /// <returns>The info hash of the magnet URI.</returns>
        public static string ParseInfoHash(this string magnetUri)
        {
            var match = Regex.Match(magnetUri, @"btih:([\w\d]*)");

            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Attempt to convert the value of the string to an integer.
        /// </summary>
        /// <param name="value">The string value to attempt to convert.</param>
        /// <param name="def">The default value should the conversion fail.</param>
        /// <returns>If successful, the converted string value as an integer. Else the default value.</returns>
        public static int TryToInt(
            this string value
            , int def = 0
        ) => int.TryParse(value, out var result)
            ? result
            : def;

        /// <summary>
        /// Attempt to parse the string value to an exact DateTime format.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="format">The format string.</param>
        /// <param name="cultureInfo">The culture to use when converting parsing the date.</param>
        /// <returns></returns>
        public static DateTime? TryParseExact(
            this string value
            , string format
            , CultureInfo cultureInfo = null
        )
        {
            try
            {
                return DateTime.ParseExact(
                    value
                    , format
                    , cultureInfo ?? CultureInfo.InvariantCulture
                );
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}