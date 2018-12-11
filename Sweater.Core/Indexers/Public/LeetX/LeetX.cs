using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.LeetX.Models;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public.LeetX
{
    public class LeetX : BaseIndexer<Settings>
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
            var torrents = new List<Torrent>(firstPage);
            var lastPageIndex = GetLastPageIndex(rootNode);
            var pageRange = GetPageRange(lastPageIndex, _settings.MaxPages);

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

                    return await ParseResponse(response);
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private static IEnumerable<int> GetPageRange(
            int lastPageIndex
            , int maxPages
        )
        {
            var remainingPageCount = Math.Min(maxPages, lastPageIndex) - 1;

            return remainingPageCount <= 0
                ? null
                : Enumerable
                    .Range(2, remainingPageCount)
                    .ToList();
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

        private async Task<IEnumerable<Torrent>> ParseResponse(HtmlNode rootNode)
            => await Task.WhenAll(
                rootNode
                    .SelectNodes(TorrentRowXPath)
                    .Select(ParseTorrentRow)
            );

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
            var torrentHrefNode = torrentNode.SelectSingleNode(TorrentNameXPath);
            return new Torrent
            {
                Name = torrentHrefNode?.InnerText,
                Seeders = int.Parse(torrentNode.SelectSingleNode(SeedersXPath)?.InnerText ?? "0"),
                Leechers = int.Parse(torrentNode.SelectSingleNode(LeechersXPath)?.InnerText ?? "0"),
                UploadedOn = torrentNode.SelectSingleNode(UploadedOnXPath)?.InnerText,
                Size = GetSizeText(torrentNode),
                MagnetUri = await GetInfoHash(torrentHrefNode?.GetAttributeValue("href", null))
            };
        }

        private static string GetSizeText(HtmlNode torrentNode)
        {
            var sizeNode = torrentNode.SelectSingleNode(SizeXPath);
            sizeNode?.SelectNodes("span")?.ToList().ForEach(node => node.Remove());
            return sizeNode?.InnerText;
        }
    }
}