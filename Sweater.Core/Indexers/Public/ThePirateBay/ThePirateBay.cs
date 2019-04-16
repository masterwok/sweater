using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.ThePirateBay.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.ThePirateBay
{
    public class ThePirateBay : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.ThePirateBay.ToString();

        private static readonly Regex InfoTextRegex = new Regex(@"Uploaded\s[Today|\s]*([\d\W]*),\s*Size\s*(.*),");
        private static readonly Regex RegexMonthDayTime = new Regex(@"(\d{2})-(\d{2}) (\d{2}):(\d{2})$");
        private static readonly Regex RegexMonthDayYear = new Regex(@"(\d{2})-(\d{2}) (\d{4})$");
        private static readonly Regex RegexTodayTime = new Regex(@"(\d{2}):(\d{2})$");

        private const string TorrentRowXPath = "//*[@id='searchResult']/tr";
        private const string TorrentNameXPath = "td[2]/div/a";
        private const string MagnetUriXPath = "td[2]/a[1]";
        private const string SeedersXPath = "td[3]";
        private const string LeechersXPath = "td[4]";
        private const string DetailsXpath = "td[2]/font";

        private readonly ILogService<ThePirateBay> _logger;

        private readonly Settings _settings;

        public override string Tag => ConfigName;

        public ThePirateBay(
            IHttpClient httpClient
            , ILogService<ThePirateBay> logger
            , Settings settings
        ) : base(httpClient)
        {
            _logger = logger;
            _settings = settings;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        public override Task Login() => Task.FromResult(true);

        public override Task Logout() => Task.FromResult(true);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var rootNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 0
            );

            var torrentNodes = rootNode.SelectNodes(TorrentRowXPath);
            var firstPage = ParseTorrentEntries(torrentNodes);
            var torrents = new List<Torrent>(firstPage);

            if (torrents.Count == 0)
            {
                return new Torrent[0];
            }

            var lastPageIndex = GetLastPageIndex(torrentNodes.Last());
            var pageRange = PagingUtil.GetPageRange(
                lastPageIndex
                , _settings.MaxPages
                , true
            );

            if (pageRange == null)
            {
                return torrents;
            }

            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    var response = await GetHtmlDocument(
                        _settings.BaseUrl
                        , query.QueryString
                        , page
                    );

                    return ParseTorrentEntries(response.SelectNodes(TorrentRowXPath));
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int page
        )
        {
            var response = await HttpClient.GetStringAsync(
                $"{baseUrl}/search/{queryString}/{page}/99"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }

        private IEnumerable<Torrent> ParseTorrentEntries(
            HtmlNodeCollection nodes
        ) => nodes?.Where(n => !ShouldSkipRow(n))
                 .Select(TryParseRow)
                 .Where(t => t != null)
             ?? new Torrent[0];

        private int GetLastPageIndex(HtmlNode lastTableNode)
        {
            if (!IsPaginationRow(lastTableNode))
            {
                return 0;
            }

            try
            {
                return lastTableNode
                    .Descendants()
                    .Where(d => d.Name == "a")
                    .Where(d => d.Descendants().All(n => n.Name != "img"))
                    .Select(d => d.Attributes["href"]?.Value)
                    .Where(d => d != null)
                    .Select(h => Regex.Match(h, @"/(\d+)/(\d+)/\d+$").Groups[1].Value)
                    .Select(int.Parse)
                    .Last();
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to read last page index", exception);
                return 0;
            }
        }

        private static bool IsPaginationRow(HtmlNode node)
            => node.Descendants()
                   .FirstOrDefault(d => d.Attributes["colspan"] != null) != null;

        private static bool ShouldSkipRow(HtmlNode node)
        {
            // Skip row containing paging information.
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (IsPaginationRow(node))
            {
                return true;
            }

            // Add more row skip rules here if required in the future..

            return false;
        }

        /// <summary>
        /// Decode the given string and replace whitespace characters with spaces. This is done when
        /// parsing the info/details text of a torrent entry as it's encoded and has %A0 as
        /// whitespace characters.
        /// </summary>
        private static string DecodeAndFixWhitespace(string infoText) => Regex.Replace(
            HttpUtility.HtmlDecode(infoText ?? string.Empty),
            @"\s",
            " "
        );

        private Torrent TryParseRow(HtmlNode torrentNode)
        {
            try
            {
                var infoText = DecodeAndFixWhitespace(
                    torrentNode.SelectSingleNode(DetailsXpath)?.InnerText
                );

                return new Torrent
                {
                    Name = torrentNode.SelectSingleNode(TorrentNameXPath).InnerText,
                    MagnetUri = torrentNode.SelectSingleNode(MagnetUriXPath)?.GetAttributeValue("href", null),
                    Seeders = int.Parse(torrentNode.SelectSingleNode(SeedersXPath).InnerText ?? "0"),
                    Leechers = int.Parse(torrentNode.SelectSingleNode(LeechersXPath).InnerText ?? "0"),
                    UploadedOn = ParseUploadedOn(infoText),
                    Size = ParseSize(infoText)
                };
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to parse row", exception);
                return null;
            }
        }

        private static long ParseSize(string infoText) => ParseUtil.GetBytes(InfoTextRegex
            .Match(infoText)
            .Groups[2]
            .Value);

        private static DateTime? ParseUploadedOn(string infoText)
        {
            var dateText = InfoTextRegex
                .Match(infoText)
                .Groups[1]
                .Value;

            return TryParseMonthDayYearDateFormat(dateText)
                   ?? TryParseMonthDayTime(dateText)
                   ?? TryParseTodayTime(dateText);
        }

        /// <summary>
        /// Attempt to parse a date with "Uploaded 10-09 2014" format.
        /// </summary>
        /// <param name="dateText">The text containing the raw date.</param>
        /// <returns>If parse was successful, a new DateTime instance. Else, null.</returns>
        private static DateTime? TryParseMonthDayYearDateFormat(string dateText)
        {
            var monthDayYearMatches = RegexMonthDayYear.Matches(dateText);

            if (monthDayYearMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = monthDayYearMatches[0].Groups;

            // Must have exactly three groups for this format.
            if (matchGroups.Count != 4)
            {
                return null;
            }

            return new DateTime(
                year: int.Parse(matchGroups[3].Value)
                , month: int.Parse(matchGroups[1].Value)
                , day: int.Parse(matchGroups[2].Value)
            );
        }

        /// <summary>
        /// Attempt to parse a date with "Uploaded 01-23 02:51" format.
        /// </summary>
        /// <param name="dateText">The text containing the raw date.</param>
        /// <returns>If parse was successful, a new DateTime instance. Else, null.</returns>
        private static DateTime? TryParseMonthDayTime(string dateText)
        {
            var monthDayTimeMatches = RegexMonthDayTime.Matches(dateText);

            if (monthDayTimeMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = monthDayTimeMatches[0].Groups;

            // Must have exactly five groups for this format.
            if (matchGroups.Count != 5)
            {
                return null;
            }

            return new DateTime(
                year: DateTime.Now.Year
                , month: int.Parse(matchGroups[1].Value)
                , day: int.Parse(matchGroups[2].Value)
                , hour: int.Parse(matchGroups[3].Value)
                , minute: int.Parse(matchGroups[4].Value)
                , second: 0
            );
        }

        /// <summary>
        /// Attempt to parse a date with "Uploaded Today 02:19" format.
        /// </summary>
        /// <param name="dateText">The text containing the raw date.</param>
        /// <returns>If parse was successful, a new DateTime instance. Else, null.</returns>
        private static DateTime? TryParseTodayTime(string dateText)
        {
            var todayTimeMatches = RegexTodayTime.Matches(dateText);

            if (todayTimeMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = todayTimeMatches[0].Groups;

            // Must have exactly three groups for this format.
            if (matchGroups.Count != 3)
            {
                return null;
            }

            var now = DateTime.Now;

            return new DateTime(
                year: now.Year
                , month: now.Month
                , day: now.Day
                , hour: int.Parse(matchGroups[1].Value)
                , minute: int.Parse(matchGroups[2].Value)
                , second: 0
            );
        }
    }
}