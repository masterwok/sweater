using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

            // Synchronously parse each page because last page index is unknown.
            var firstPageIndex = query.PageIndex + 1;
            var lastPageIndex = firstPageIndex + (_settings.MaxPages - 1);

            for (var currentPageIndex = firstPageIndex; currentPageIndex <= lastPageIndex; currentPageIndex++)
            {
                var queryResultNode = await GetHtmlDocument(
                    _settings.BaseUrl
                    , query.QueryString
                    , currentPageIndex
                );

                torrents.AddRange(ParseTorrents(queryResultNode));

                if (!HasNextPage(queryResultNode))
                {
                    break;
                }
            }

            return torrents;
        }

        private const string XPathNextPageChevron = @"//ul[contains(@class, 'pagination')]/li";

        private bool HasNextPage(HtmlNode initialPageNode)
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