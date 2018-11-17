using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Extensions;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class LeetX : BaseIndexer
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string BaseUrl { get; set; }
        }

        private Settings _settings;

        public LeetX(IHttpClient httpClient) : base(httpClient)
            => Expression.Empty();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override async Task<IndexerResult> Query(Query query)
        {
            var requestUrl = CreateRequestUrl(
                _settings.BaseUrl
                , query.QueryString
                , 1
            );

            var response = await HttpClient.GetStringAsync(requestUrl);

            await ParseResponse(response);

            return new IndexerResult();
        }

        private static string CreateRequestUrl(
            string baseUrl
            , string queryString
            , int page
        ) => $"{baseUrl}/search/{queryString}/{page}/";

        private const string PaginationXPath = "/html/body/main/div/div/div/div[3]/div[2]/ul/li";
        private const string TorrentRowXPath = "/html/body/main/div/div/div/div[3]/div[1]/table/tbody/tr";
        private const string MagnetXPath = "/html/body/main/div/div/div/div[2]/div[1]/ul[1]/li[1]/a";
        private const string TorrentNameXPath = "td[1]/a[2]";
        private const string SeedersXPath = "td[2]";
        private const string LeechersXPath = "td[3]";
        private const string UploadedOnXPath = "td[4]";
        private const string SizeXPath = "td[5]";

        private async Task ParseResponse(string response)
        {
            var rootNode = response.ToHtmlDocument().DocumentNode;

            var lastPageIndex = GetLastPageIndex(rootNode);

//            var torrentRows = rootNode.SelectNodes(TorrentRowXPath);
//
//            var torrents = await Task.WhenAll(torrentRows.Select(ParseTorrentRow));
        }

        private static int GetLastPageIndex(HtmlNode rootNode)
        {
            var pageListItems = rootNode.SelectNodes(PaginationXPath);

            switch (pageListItems.Count)
            {
                case 0:
                    return 0;
                case 2:
                    return 2;
                default: return ParseLastPageButton(pageListItems.LastOrDefault());
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