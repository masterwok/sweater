using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
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
        private const string XPathTorrentName = "td[2]/a";
        private const string XPathMagnetUri = "td[3]/a[2]";
        private const string XPathSeeders = "td[6]";
        private const string XPathLeechers = "td[7]";
        private const string XPathSize = "td[4]";
        private const string XPathUploadedOn = "td[5]";
        private const string UploadedOnDateFormat = "yyyy-MM-dd HH:mm";
        private const string XPathPageInfo = @"//div[contains(@class, 'pagination-page-info')]";

        public static readonly string ConfigName = Indexer.Nyaa.ToString();

        private static readonly Regex RegexPaginationNumbers = new Regex(@"(\d+)");

        private ILogService<Nyaa> _logService;
        private readonly Settings _settings;

        public Nyaa(
            IHttpClient httpClient
            , ILogService<Nyaa> logService
            , Settings settings
        ) : base(httpClient)
        {
            _logService = logService;
            _settings = settings;
        }

        public override string Tag => ConfigName;

        public override async Task<IEnumerable<Torrent>> Query(Query query, CancellationToken token)
        {
            var torrents = new List<Torrent>();

            // Fetch the initial page of results.
            var documentIntiailPage = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
                , token
            );

            // Parse and add the initial torrent results.
            torrents.AddRange(ParseTorrents(documentIntiailPage));

            // Get the remaining page range to parse.
            var pageRange = PagingUtil.GetPageRange(
                ParseLastPageIndex(documentIntiailPage)
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
                        , token
                    );

                    return ParseTorrents(response);
                })
            )).SelectMany(i => i));

            return torrents;
        }

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

        private static IEnumerable<Torrent> ParseTorrents(HtmlNode pageNode)
        {
            var torrentTableRows = pageNode.SelectNodes(XPathTorrentRow);

            return torrentTableRows
                       ?.Select(ParseTorrentFromRowNode)
                   ?? new Torrent[0];
        }

        private static Torrent ParseTorrentFromRowNode(
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
                       .SelectNodes(XPathTorrentName)
                       ?.LastOrDefault()
                       ?.InnerText
                       ?.Trim()
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
            , CancellationToken token
        )
        {
            var response = await HttpClient.GetAsync(
                $"{baseUrl}/?f=0&c=0_0&q={queryString}&s=seeders&o=desc&p={page}"
                , token
            );

            var responseString = await response
                .Content
                .ReadAsStringAsync();

            return responseString
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}