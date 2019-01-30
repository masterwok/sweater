using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.LeetX.Models;
using Sweater.Core.Models;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.LeetX
{
    public class LeetX : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.LeetX.ToString();

        private const string PaginationXPath = "/html/body/main/div/div/div/div[3]/div[2]/ul/li";
        private const string TorrentRowXPath = "/html/body/main/div/div/div/div[3]/div[1]/table/tbody/tr";
        private const string MagnetXPath = "/html/body/main/div/div/div/div[2]/div[1]/ul[1]/li[1]/a";
        private const string TorrentNameXPath = "td[1]/a[2]";
        private const string SeedersXPath = "td[2]";
        private const string LeechersXPath = "td[3]";
        private const string UploadedOnXPath = "td[4]";
        private const string SizeXPath = "td[5]";

        private static readonly Dictionary<string, int> _monthMapping = new Dictionary<string, int>
        {
            {"Jan", 1},
            {"Feb", 2},
            {"Mar", 3},
            {"Apr", 4},
            {"May", 5},
            {"Jun", 6},
            {"Jul", 7},
            {"Aug", 8},
            {"Sep", 9},
            {"Oct", 10},
            {"Nov", 11},
            {"Dec", 12}
        };

        public override string Tag => ConfigName;

        private readonly Settings _settings;

        public LeetX(
            IHttpClient httpClient
            , Settings settings
        ) : base(httpClient) => _settings = settings;

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var rootNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 1
            );

            var firstPage = await ParseResponse(rootNode);

            if (!firstPage.Any())
            {
                return new Torrent[0];
            }

            var lastPageIndex = GetLastPageIndex(rootNode);
            var pageRange = PagingUtil.GetPageRange(lastPageIndex, _settings.MaxPages);

            if (pageRange == null)
            {
                return firstPage;
            }

            var torrents = new List<Torrent>(firstPage);

            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    var response = await GetHtmlDocument(
                        _settings.BaseUrl
                        , query.QueryString
                        , page
                    );

                    return await ParseResponse(response);
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
                $"{baseUrl}/search/{queryString}/{page}/"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }

        private async Task<IList<Torrent>> ParseResponse(HtmlNode rootNode)
        {
            if (rootNode == null)
            {
                return new Torrent[0];
            }

            var tasks = rootNode
                .SelectNodes(TorrentRowXPath)
                ?.Select(ParseTorrentRow);

            if (tasks?.Any() != true)
            {
                return new List<Torrent>();
            }

            var results = await Task.WhenAll(tasks);

            return results
                .Where(t => t != null)
                .ToList();
        }

        private static int GetLastPageIndex(HtmlNode rootNode)
        {
            var pageListItems = rootNode.SelectNodes(PaginationXPath);

            switch (pageListItems?.Count ?? 0)
            {
                case 0:
                    return 0;
                case 2:
                    return 2;
                default: return ParseLastPageButton(pageListItems?.LastOrDefault());
            }
        }

        private static int ParseLastPageButton(HtmlNode lastButtonNode)
        {
            var lastButtonHref = lastButtonNode
                ?.FirstChild
                .GetAttributeValue("href", null);

            if (lastButtonHref == null)
            {
                return 0;
            }

            var match = Regex.Match(lastButtonHref, @"/(\d+)/$");

            return match.Success
                ? int.Parse(match.Groups[1].Value)
                : 0;
        }

        private async Task<string> GetInfoHash(string href)
        {
            var response = await HttpClient.GetStringAsync($"{_settings.BaseUrl}{href}");

            var documentNode = response.ToHtmlDocument().DocumentNode;
            return documentNode.SelectSingleNode(MagnetXPath)?.GetAttributeValue("href", null);
        }

        private async Task<Torrent> ParseTorrentRow(HtmlNode torrentNode)
        {
            if (torrentNode == null)
            {
                return null;
            }

            var torrentHrefNode = torrentNode.SelectSingleNode(TorrentNameXPath);

            return new Torrent
            {
                Name = torrentHrefNode?.InnerText,
                Seeders = int.Parse(torrentNode.SelectSingleNode(SeedersXPath)?.InnerText ?? "0"),
                Leechers = int.Parse(torrentNode.SelectSingleNode(LeechersXPath)?.InnerText ?? "0"),
                UploadedOn = ParseDateTime(torrentNode.SelectSingleNode(UploadedOnXPath)?.InnerText ?? string.Empty),
                Size = GetSizeText(torrentNode),
                MagnetUri = await GetInfoHash(torrentHrefNode?.GetAttributeValue("href", null))
            };
        }

        private static readonly Regex _regexMonthDateYear = new Regex(@"(\w{3}). (\d+)\w{2,} '(\d{2})$");

        // TODO: Parse: "Nov. 20th '12"
        /// <summary>
        /// Attempt to parse a date with "Uploaded Today 02:19" format.
        /// </summary>
        /// <param name="dateText">The text containing the raw date.</param>
        /// <returns>If parse was successful, a new DateTime instance. Else, null.</returns>
        private static DateTime? TryParseMonthDateYear(string dateText)
        {
            var todayTimeMatches = _regexMonthDateYear.Matches(dateText);

            if (todayTimeMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = todayTimeMatches[0].Groups;

            // Must have exactly four groups for this format.
            if (matchGroups.Count != 4)
            {
                return null;
            }

            var yearText = matchGroups[3].Value;
            var yearFormattedText = yearText.Length == 2
                // '18 format
                ? $"20{yearText}"
                // Possible four character format
                : yearText;

            return new DateTime(
                year: int.Parse(yearFormattedText)
                , month: _monthMapping[matchGroups[1].Value]
                , day: int.Parse(matchGroups[2].Value)
            );
        }

        // TODO: Parse: "2am Jan. 16th"
        private static readonly Regex _timeMonthDayRegex = new Regex(@"(\d+)(\w{2}) (\w{3}). (\d+)\w{2}$");

        private static DateTime? TryParseTimeMonthDayDate(string dateText)
        {
            var todayTimeMatches = _timeMonthDayRegex.Matches(dateText);

            if (todayTimeMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = todayTimeMatches[0].Groups;

            // Must have exactly four groups for this format.
            if (matchGroups.Count != 5)
            {
                return null;
            }

            var hourRawValue = int.Parse(matchGroups[1].Value);
            var timeOfDay = matchGroups[2].Value;

            var hour = timeOfDay.ToLowerInvariant().Equals("am")
                ? hourRawValue
                : 12 + hourRawValue;

            return new DateTime(
                year: DateTime.Now.Year
                , month: _monthMapping[matchGroups[3].Value]
                , day: int.Parse(matchGroups[4].Value)
                , hour: hour
                , minute: 0
                , second: 0
            );
        }

        private static readonly Regex RegexTodayTime = new Regex(@"(\d+):(\d{2})(\w{2})$");

        /// <summary>
        /// Attempt to parse a date with "2:19pm" format.
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
            if (matchGroups.Count != 4)
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

        private static DateTime? ParseDateTime(
            string dateText
        ) => TryParseMonthDateYear(dateText)
             ?? TryParseTimeMonthDayDate(dateText)
             ?? TryParseTodayTime(dateText);

        private static long GetSizeText(HtmlNode torrentNode)
        {
            var sizeNode = torrentNode.SelectSingleNode(SizeXPath);
            sizeNode?.SelectNodes("span")?.ToList().ForEach(node => node.Remove());
            return ParseUtil.GetBytes(sizeNode?.InnerText);
        }
    }
}