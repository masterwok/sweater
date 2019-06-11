using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Kat.Models;
using Sweater.Core.Models;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.Kat
{
    /// <inheritdoc />
    /// <summary>
    /// This indexer implementation scrapes KickassTorrents.
    /// </summary>
    public class Kat : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.Kat.ToString();

        private static readonly Regex DateFormatRegex = new Regex(@"(\d+)(minute|hour|day|month|year)s?$");

        private const string PageNumberXPath = "//*[@id=\"wrapperInner\"]/div[2]/table/tbody/tr/td[1]/div[2]/a";
        private const string PageSkipTextValue = ">>";

        private const string TorrentRowXPath = "//tr[@class='even' or @class='odd']";
        private const string TorrentRowLinkXPath = ".//a[@class='cellMainLink']";
        private const string TorrentMagnetXPath = "//a[@title='Magnet link']";
        private const string TorrentRowSizeXPath = "td[2]";
        private const string TorrentRowUploadedOnXPath = "td[4]";
        private const string TorrentRowSeedXPath = "td[5]";
        private const string TorrentRowLeechXPath = "td[6]";

        private readonly Settings _settings;

        public override string Tag => ConfigName;

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public Kat(
            IHttpClient httpClient
            , Settings settings
        ) : base(httpClient)
        {
            _settings = settings;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        /// <summary>
        /// Attempt to parse results for the single result case. This is a special case where only one result is
        /// returned. When this happens, including paging will break the scraper.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>If single results case, the single result. Else, an empty list.</returns>
        private async Task<IList<Torrent>> ParseSinglePageCase(Query query)
        {
            var documentNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
            );

            var isLastPage = GetLastPageIndex(documentNode) == 0;

            return isLastPage
                ? await ParseTorrents(_settings.BaseUrl, documentNode)
                : new Torrent[0];
        }

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            // Check if there is only one page of results as including page when there's only one page of results will
            // break the scraper.
            var singlePageCaseResults = await ParseSinglePageCase(query);

            // Results implies that there was only one page of results.
            if (singlePageCaseResults.Count > 0)
            {
                return singlePageCaseResults;
            }

            // Continue parsing as normal...
            var documentNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 0
            );

            var firstPage = await ParseTorrents(_settings.BaseUrl, documentNode);
            var lastPageIndex = GetLastPageIndex(documentNode);
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

                    return await ParseTorrents(_settings.BaseUrl, response);
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private static int GetLastPageIndex(HtmlNode rootNode)
        {
            var buttons = rootNode.SelectNodes(PageNumberXPath);

            // No buttons mean there's no pages to parse
            if (buttons == null || buttons.Count == 0)
            {
                return 0;
            }

            var lastButtonText = buttons
                .Last()
                .InnerText;

            // Last button was double chevron => drop chevron button to get last page
            if (lastButtonText.Equals(PageSkipTextValue))
            {
                buttons.RemoveAt(buttons.Count - 1);
            }

            return int.Parse(buttons.Last().InnerText);
        }

        private async Task<Torrent[]> ParseTorrents(
            string baseUrl
            , HtmlNode documentNode
        )
        {
            var torrentRows = documentNode.SelectNodes(TorrentRowXPath);

            return await Task.WhenAll(torrentRows.Select(async n => await ParseTorrent(baseUrl, n)));
        }

        private async Task<Torrent> ParseTorrent(
            string baseUrl
            , HtmlNode torrentRowNode
        )
        {
            var mainLink = torrentRowNode?.SelectSingleNode(TorrentRowLinkXPath);

            var magnetUri = await GetMagnetUri(
                baseUrl
                , mainLink.GetAttributeValue("href", null)
            );

            return new Torrent
            {
                MagnetUri = magnetUri,
                Name = mainLink.InnerText.Trim(),
                Leechers = int.Parse(torrentRowNode.SelectSingleNode(TorrentRowLeechXPath).InnerText.Trim()),
                Seeders = int.Parse(torrentRowNode.SelectSingleNode(TorrentRowSeedXPath).InnerText.Trim()),
                Size = ParseUtil.GetBytes(torrentRowNode.SelectSingleNode(TorrentRowSizeXPath).InnerText.Trim()),
                UploadedOn = ParseUploadedOn(torrentRowNode
                    .SelectSingleNode(TorrentRowUploadedOnXPath)
                    .InnerText
                    .Trim()
                )
            };
        }

        private static DateTime? ParseUploadedOn(string dateText)
        {
            Debug.WriteLine($"Date Text: {dateText}");

            var dateMatches = DateFormatRegex.Matches(dateText);

            if (dateMatches.Count <= 0)
            {
                return null;
            }

            var matchGroups = dateMatches[0].Groups;

            // Must have exactly three groups for this format.
            if (matchGroups.Count != 3)
            {
                return null;
            }

            var value = int.Parse(matchGroups[1].Value);
            var unit = matchGroups[2].Value;
            var now = DateTime.Now;

            switch (unit)
            {
                case "month":
                    return now.AddMonths(-value);
                case "year":
                    return now.AddYears(-value);
                case "day":
                    return now.AddDays(-value);
                case "hour":
                    return now.AddHours(-value);
                case "minute":
                    return now.AddMinutes(-value);
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected age unit: {unit}");
            }
        }

        private async Task<string> GetMagnetUri(string baseUrl, string href)
        {
            var response = await HttpClient.GetStringAsync($"{baseUrl}{href}");

            return response
                .ToHtmlDocument()
                .DocumentNode
                .SelectSingleNode(TorrentMagnetXPath)
                .GetAttributeValue("href", null);
        }

        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int? page = null
        )
        {
            // TODO: Fix broken single entry query result
            // It seems to be the page number that breaks in when there's only 1 page
            // https://kat.am/usearch/True.Detective.S03E04.The.Hour.and.the.Day.720p.AMZN.WEB-DL.DDP5.1.H.264-NTb[eztv].mkv/0/?sortby=seeders&sort=desc
            var url = page == null
                ? $"{baseUrl}/usearch/{queryString}/?sortby=seeders&sort=desc"
                : $"{baseUrl}/usearch/{queryString}/{page}/?sortby=seeders&sort=desc";

            // https://kat.am/usearch/hackers%201995/?sortby=seeders&sort=desc
            var response = await HttpClient.GetStringAsync(url);

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}