using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Kat.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.Kat
{
    public class Kat : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.Kat.ToString();

        private const string XPathTorrentRow = @"//table[contains(@class, 'torrents_table')]/tbody/tr";
        private const string XPathTorrentTitle = @".//a[contains(@class, 'torrents_table__torrent_title')]";
        private const string XPathMagnetUri = @".//i[contains(@class, 'kf__magnet')]";
        private const string XPathUploadedOn = @".//td[@data-title='Age']";
        private const string XPathLeech = @".//td[@data-title='Leech']";
        private const string XPathSeed = @".//td[@data-title='Seed']";
        private const string XPathSize = @".//td[@data-title='Size']";

        private const string UploadedOnDateFormat = "yyyy-MM-dd";

        private ILogService<Kat> _logService;
        private readonly Settings _settings;

        public Kat(
            IHttpClient httpClient
            , ILogService<Kat> logService
            , Settings settings
        ) : base(httpClient)
        {
            _logService = logService;
            _settings = settings;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        public override string Tag => ConfigName;

        public override async Task<IEnumerable<Torrent>> Query(Query query, CancellationToken token)
        {
            var torrents = new List<Torrent>();

            var initialQueryResultsNode = await FetchQueryResults(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
                , token
            );

            torrents.AddRange(ParseTorrents(initialQueryResultsNode));

            // Always fetch up to the max pages as paging is broken on Kat and done client side (results with more 
            // seeders are being returned on the second page than the first). Requesting more pages than wanted will
            // just result in empty pages. The downside being wasted bandwidth.
            var maxPages = _settings.MaxPages;
            var pageRange = PagingUtil.GetPageRange(maxPages, maxPages);

            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    try
                    {
                        var response = await FetchQueryResults(
                            _settings.BaseUrl
                            , query.QueryString
                            , page
                            , token
                        );

                        return ParseTorrents(response);
                    }
                    catch (Exception exception)
                    {
                        _logService.LogError("Failed to fetch page", exception);

                        return new Torrent[0];
                    }
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private static IEnumerable<Torrent> ParseTorrents(HtmlNode rootNode)
        {
            var torrentRows = rootNode.SelectNodes(XPathTorrentRow);

            return torrentRows
                       ?.Select(torrentRow => new Torrent
                       {
                           Name = torrentRow
                               .SelectSingleNode(XPathTorrentTitle)
                               ?.InnerText
                               ?.Trim(),
                           MagnetUri = torrentRow
                               .SelectSingleNode(XPathMagnetUri)
                               ?.ParentNode
                               ?.GetAttributeValue("href", string.Empty),
                           Size = ParseUtil.GetBytes(
                               torrentRow
                                   .SelectSingleNode(XPathSize)
                                   ?.InnerText
                           ),
                           Leechers = int.Parse(
                               torrentRow
                                   .SelectSingleNode(XPathLeech)
                                   ?.InnerText
                               ?? "0"
                           ),
                           Seeders = int.Parse(
                               torrentRow
                                   .SelectSingleNode(XPathSeed)
                                   ?.InnerText
                               ?? "0"
                           ),
                           UploadedOn = torrentRow
                               .SelectSingleNode(XPathUploadedOn)
                               ?.InnerText
                               ?.TryParseExact(UploadedOnDateFormat)
                       })
                   ?? new Torrent[0];
        }

        private async Task<HtmlNode> FetchQueryResults(
            string baseUrl
            , string queryString
            , int page
            , CancellationToken token
        )
        {
            var response = await HttpClient.GetAsync(
                $"{baseUrl}/katsearch/page/{page}/{queryString}"
                , token
            );

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}