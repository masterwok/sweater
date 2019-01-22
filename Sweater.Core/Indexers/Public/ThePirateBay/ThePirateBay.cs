using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.ThePirateBay.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.ThePirateBay
{
    public class ThePirateBay : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.ThePirateBay.ToString();

        private static readonly string TorrentRowXPath = "//*[@id='searchResult']/tr";
        private static readonly string TorrentNameXPath = "td[2]/div/a";
        private static readonly string MagnetUriXPath = "td[2]/a[1]";
        private static readonly string SeedersXPath = "td[3]";
        private static readonly string LeechersXPath = "td[4]";
        private static readonly string DetailsXpath = "td[2]/font";
        private static readonly Regex InfoTextRegex = new Regex(@"Uploaded\s*([\d\W]*),\s*Size\s*(.*),");

        private readonly ILogService<ThePirateBay> _logger;

        private readonly Settings _settings;

        public override string Tag => ConfigName;

        public ThePirateBay(
            IHttpClient httpClient
            , ILogService<ThePirateBay> logger
            , Settings settings
        ) : base(httpClient)
        {
            _logger = logger;
            _settings = settings;
        }

        public override Task Login() => Task.FromResult(true);

        public override Task Logout() => Task.FromResult(true);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var rootNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 0
            );

            var torrentNodes = rootNode.SelectNodes(TorrentRowXPath);
            var firstPage = ParseTorrentEntries(torrentNodes);
            var torrents = new List<Torrent>(firstPage);
            var lastPageIndex = GetLastPageIndex(torrentNodes.Last());
            var pageRange = PagingUtil.GetPageRange(
                lastPageIndex
                , _settings.MaxPages
                , true
            );

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

                    return ParseTorrentEntries(response.SelectNodes(TorrentRowXPath));
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
                $"{baseUrl}/s/?q={queryString}&page={page}&orderby=99"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }

        private IEnumerable<Torrent> ParseTorrentEntries(HtmlNodeCollection nodes) => nodes
            .Where(n => !ShouldSkipRow(n))
            .Select(TryParseRow)
            .Where(t => t != null);

        private int GetLastPageIndex(HtmlNode lastTableNode)
        {
            if (!IsPaginationRow(lastTableNode))
            {
                return 0;
            }

            try
            {
                return lastTableNode
                    .Descendants()
                    .Where(d => d.Name == "a")
                    .Where(d => d.Descendants().All(n => n.Name != "img"))
                    .Select(d => d.Attributes["href"]?.Value)
                    .Where(d => d != null)
                    .Select(h => Regex.Match(h, @"/(\d+)/(\d+)/\d+$").Groups[1].Value)
                    .Select(int.Parse)
                    .Last();
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to read last page index", exception);
                return 0;
            }
        }

        private static bool IsPaginationRow(HtmlNode node)
            => node.Descendants()
                   .FirstOrDefault(d => d.Attributes["colspan"] != null) != null;

        private static bool ShouldSkipRow(HtmlNode node)
        {
            // Skip row containing paging information.
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (IsPaginationRow(node))
            {
                return true;
            }

            // Add more row skip rules here if required in the future..

            return false;
        }

        /// <summary>
        /// Decode the given string and replace whitespace characters with spaces. This is done when
        /// parsing the info/details text of a torrent entry as it's encoded and has %A0 as
        /// whitespace characters.
        /// </summary>
        private static string DecodeAndFixWhitespace(string infoText) => Regex.Replace(
            HttpUtility.HtmlDecode(infoText ?? string.Empty),
            @"\s",
            " "
        );

        private Torrent TryParseRow(HtmlNode torrentNode)
        {
            try
            {
                var infoText = DecodeAndFixWhitespace(
                    torrentNode.SelectSingleNode(DetailsXpath)?.InnerText
                );
                return new Torrent
                {
                    Name = torrentNode.SelectSingleNode(TorrentNameXPath).InnerText,
                    MagnetUri = torrentNode.SelectSingleNode(MagnetUriXPath)?.GetAttributeValue("href", null),
                    Seeders = int.Parse(torrentNode.SelectSingleNode(SeedersXPath).InnerText ?? "0"),
                    Leechers = int.Parse(torrentNode.SelectSingleNode(LeechersXPath).InnerText ?? "0"),
                    UploadedOn = ParseUploadedOn(infoText),
                    Size = ParseSize(infoText)
                };
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to parse row", exception);
                return null;
            }
        }

        private static string ParseSize(string infoText) => InfoTextRegex
            .Match(infoText)
            .Groups[2]
            .Value;

        private static string ParseUploadedOn(string infoText)
        {
            var text = InfoTextRegex
                .Match(infoText)
                .Groups[1]
                .Value;
            return text.Contains(':')
                ? $"{text.Substring(0, text.IndexOf(' '))}-{DateTime.Now.Year}"
                : text.Replace(' ', '-');
        }
    }
}