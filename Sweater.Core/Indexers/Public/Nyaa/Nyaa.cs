using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Nyaa.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.Nyaa
{
    public sealed class Nyaa : BaseIndexer
    {
        private const string XPathTorrentRow = @"//table[contains(@class, 'torrent-list')]/tbody/tr";
        private const string XPathTorrentName = "td[2]/a[2]";
        private const string XPathMagnetUri = "td[3]/a[2]";
        private const string XPathSeeders = "td[6]";
        private const string XPathLeechers = "td[7]";
        private const string XPathSize = "td[4]";
        private const string XPathUploadedOn = "td[5]";
        private const string UploadedOnDateFormat = "yyyy-MM-dd HH:mm";
        private const string XPathNextPageChevron = @"//ul[contains(@class, 'pagination')]/li";
        private const string XPathPageInfo = @"//div[contains(@class, 'pagination-page-info')]";

        public static readonly string ConfigName = Indexer.Nyaa.ToString();

        private readonly IHttpClient _httpClient;
        private readonly ILogService<Nyaa> _logService;
        private readonly Settings _settings;

        public Nyaa(
            IHttpClient httpClient
            , ILogService<Nyaa> logService
            , Settings settings
        ) : base(httpClient)
        {
            _httpClient = httpClient;
            _logService = logService;
            _settings = settings;
        }

        public override string Tag => ConfigName;

        public override Task Login() => Task.CompletedTask;

        public override Task Logout() => Task.CompletedTask;

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var torrents = new List<Torrent>();

            // Fetch the initial page of results.
            var initialPageIndex = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
            );

            // Parse and add the initial torrent results.
            torrents.AddRange(ParseTorrents(initialPageIndex));

            // Get the remaining page range to parse.
            var pageRange = PagingUtil.GetPageRange(
                ParseLastPageIndex(initialPageIndex)
                , _settings.MaxPages
            );

            // Nothing to parse return initial page results.
            if (pageRange == null)
            {
                return torrents;
            }

            // Fetch and parse all remaining pages concurrently.
            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    var response = await GetHtmlDocument(
                        _settings.BaseUrl
                        , query.QueryString
                        , page
                    );

                    return ParseTorrents(response);
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private static readonly Regex RegexPaginationNumbers = new Regex(@"(\d+)");

        private static int ParseLastPageIndex(HtmlNode initialPageNode)
        {
            try
            {
                var pageInfoNode = initialPageNode.SelectSingleNode(XPathPageInfo);
                var paginationText = pageInfoNode
                    ?.InnerText
                    ?.Split('\n')[0];

                var matches = RegexPaginationNumbers.Matches(paginationText);
                var pageSize = matches[1].Value.TryToInt();
                var resultCount = matches[2].Value.TryToInt();

                return (resultCount + pageSize - 1) / pageSize;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static bool HasNextPage(HtmlNode initialPageNode)
        {
            var chevronNode = initialPageNode
                .SelectNodes(XPathNextPageChevron)
                ?.LastOrDefault();

            return chevronNode != null
                   && !chevronNode.HasClass("disabled");
        }

        private IEnumerable<Torrent> ParseTorrents(HtmlNode pageNode)
        {
            var torrentTableRows = pageNode.SelectNodes(XPathTorrentRow);

            return torrentTableRows
                .Select(ParseTorrentFromRowNode);
        }

        private Torrent ParseTorrentFromRowNode(
            HtmlNode rowNode
        ) => new Torrent
        {
            Seeders = rowNode
                          .SelectSingleNode(XPathSeeders)
                          ?.InnerText
                          ?.TryToInt()
                      ?? 0,
            Leechers = rowNode
                           .SelectSingleNode(XPathLeechers)
                           ?.InnerText
                           ?.TryToInt()
                       ?? 0,
            MagnetUri = rowNode
                            .SelectSingleNode(XPathMagnetUri)
                            ?.GetAttributeValue("href", string.Empty)
                        ?? string.Empty,
            Name = rowNode
                       .SelectSingleNode(XPathTorrentName)
                       ?.GetAttributeValue("title", string.Empty)
                   ?? string.Empty,
            Size = ParseUtil.GetBytes(rowNode
                .SelectSingleNode(XPathSize)
                ?.InnerText),
            UploadedOn = rowNode
                .SelectSingleNode(XPathUploadedOn)
                ?.InnerText
                ?.TryParseExact(UploadedOnDateFormat)
        };

        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int page
        )
        {
            var response = await HttpClient.GetStringAsync(
                $"{baseUrl}/?f=0&c=0_0&q={queryString}&s=seeders&o=desc&p={page}"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}