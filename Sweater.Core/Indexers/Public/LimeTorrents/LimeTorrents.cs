using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.LimeTorrents.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.LimeTorrents
{
    public sealed class LimeTorrents : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.LimeTorrents.ToString();

//        private static readonly Regex RegexPaginationNumbers = new Regex(@"(\d+)");

        private ILogService<LimeTorrents> _logService;
        private readonly Settings _settings;

        public LimeTorrents(
            IHttpClient httpClient
            , ILogService<LimeTorrents> logService
            , Settings settings
        ) : base(httpClient)
        {
            _logService = logService;
            _settings = settings;
        }

        public override string Tag => ConfigName;

        public override Task Login() => Task.CompletedTask;

        public override Task Logout() => Task.CompletedTask;

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var torrents = new List<Torrent>();

            var initialQueryResultsNode = await FetchQueryResults(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
            );

            var pageRange = GetPageRange(initialQueryResultsNode);

            torrents.AddRange(await ParseTorrents(initialQueryResultsNode));

            return torrents;
        }

        private async Task<IEnumerable<Torrent>> ParseTorrents(HtmlNode node)
        {
            var torrents = node
                .SelectNodes(@"//table[contains(@class, 'table2')]/tr")
                .Skip(1)
                .Select(async torrentRow =>
                {
                    try
                    {
                        var detailsUrl = torrentRow
                            .SelectSingleNode("td[1]/div[1]/a[2]")
                            .GetAttributeValue("href", string.Empty);

                        return new Torrent
                        {
                            Leechers = torrentRow
                                           .SelectSingleNode("td[5]")
                                           ?.InnerText
                                           ?.Replace(",", null)
                                           .TryToInt()
                                       ?? 0,
                            MagnetUri = await GetMagnetUri(_settings.BaseUrl, detailsUrl),
                            Name = torrentRow
                                .SelectSingleNode("td[1]")
                                ?.InnerText,
                            Seeders = torrentRow
                                          .SelectSingleNode("td[4]")
                                          ?.InnerText
                                          ?.Replace(",", null)
                                          .TryToInt()
                                      ?? 0,
                            Size = ParseUtil.GetBytes(
                                torrentRow
                                    .SelectSingleNode("td[3]")
                                    ?.InnerText
                            ),
                            UploadedOn = DateTime.Now
                        };
                    }
                    catch (Exception ex)
                    {
                        _logService.LogError("Failed to parse torrent entry.", ex);

                        return null;
                    }
                })
                .ToList();

            return await Task.WhenAll(torrents);
        }

        private async Task<string> GetMagnetUri(string baseUrl, string detailsUrl)
        {
            var response = await HttpClient.GetStringAsync($"{baseUrl}/{detailsUrl}");

            return response
                .ToHtmlDocument()
                .DocumentNode
                .SelectSingleNode("//*[@id='content']/div[1]/div[1]/div/div[4]/div/p/a")
                .GetAttributeValue("href", string.Empty);
        }

        private IEnumerable<int> GetPageRange(HtmlNode node)
        {
            var searchStatNode = node.SelectSingleNode(@"//div[contains(@class, 'search_stat')]");

            if (searchStatNode == null)
            {
                return new int[0];
            }

            var lastPageIndex = searchStatNode
                                    .SelectNodes("a")
                                    .LastOrDefault(
                                        n => !n.InnerText.Contains("Next")
                                             && !n.InnerText.Contains("Previous")
                                    )
                                    ?.InnerText
                                    ?.TryToInt(1)
                                ?? 1;

            return PagingUtil.GetPageRange(
                lastPageIndex
                , _settings.MaxPages
            );
        }


        private async Task<HtmlNode> FetchQueryResults(
            string baseUrl
            , string queryString
            , int page
        )
        {
            var response = await HttpClient.GetStringAsync(
                $"{baseUrl}/search/all/{queryString}/seeds/1/"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}