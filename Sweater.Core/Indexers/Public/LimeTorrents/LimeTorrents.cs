using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        public override async Task<IEnumerable<Torrent>> Query(Query query, CancellationToken token)
        {
            var torrents = new List<Torrent>();

            var initialQueryResultsNode = await FetchQueryResults(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
                , token
            );

            var pageRange = GetPageRange(initialQueryResultsNode);

            torrents.AddRange(await ParseTorrents(initialQueryResultsNode));

            // Nothing to parse return initial page results.
            if (pageRange == null)
            {
                return torrents;
            }

            // Fetch and parse all remaining pages concurrently.
            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    var response = await FetchQueryResults(
                        _settings.BaseUrl
                        , query.QueryString
                        , page
                        , token
                    );

                    return ParseTorrents(response);
                })
            )).SelectMany(i => i?.Result));

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
                            UploadedOn = ParseUploadedOn(
                                torrentRow
                                    .SelectSingleNode("td[2]")
                                    ?.InnerText
                            )
                        };
                    }
                    catch (Exception ex)
                    {
                        _logService.LogError("Failed to parse torrent entry.", ex);

                        return null;
                    }
                });

            return await Task.WhenAll(torrents);
        }

        private static DateTime? ParseUploadedOn(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
            {
                return null;
            }

            var prefix = dateText
                .Split('-')
                .FirstOrDefault()
                ?.Replace("+", null)
                .Trim();

            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }

            var today = DateTime.Today;

            var monthsRegex = Regex.Match(
                prefix
                , @"(\d+).*month"
                , RegexOptions.IgnoreCase
            );

            if (monthsRegex.Success)
            {
                return today.AddMonths(-monthsRegex.Groups[1].Value.TryToInt());
            }

            var yearRegex = Regex.Match(
                prefix
                , @"(\d+).*year"
                , RegexOptions.IgnoreCase
            );

            if (yearRegex.Success)
            {
                return today.AddYears(-yearRegex.Groups[1].Value.TryToInt());
            }

            var dayRegex = Regex.Match(
                prefix
                , @"(\d+).*day"
                , RegexOptions.IgnoreCase
            );

            return dayRegex.Success
                ? today.AddDays(-dayRegex.Groups[1].Value.TryToInt())
                : today;
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
            , CancellationToken token
        )
        {
            var response = await HttpClient.GetAsync(
                $"{baseUrl}/search/all/{queryString}/seeds/1/"
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