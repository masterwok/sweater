using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Sweater.Core.Extensions
{
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

    }
}