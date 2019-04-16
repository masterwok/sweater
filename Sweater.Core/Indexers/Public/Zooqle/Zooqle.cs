using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Zoogle.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.Zooqle
{
    // TODO: Implement Paging
    public class Zooqle : BaseIndexer
    {
        private readonly ILogService<Zooqle> _logger;
        private readonly Settings _settings;

        public static readonly string ConfigName = Indexer.Zooqle.ToString();

        private const string XPathTorrentTableRowItems = @"//table[contains(@class, 'table-torrents')]/tr";
        private const string XPathTorrentDetailsTorrentName = @"//h4[contains(@id, 'torname')]";
        private const string XPathTorrentDetailsMagnetHref = "//*[@id='dlPanel']/div[2]/ul/li[2]/a";
        private const string XPathTorrentDetailsSeedersDiv = "//*[@id='torinfo']/h6[1]/div/div[1]";
        private const string XPathTorrentDetailsLeechersDiv = "//*[@id='torinfo']/h6[1]/div/div[2]";
        private const string XPathTorrentUploadedOnText = "//*[@id='torinfo']/h6[1]/text()[2]";
        private const string XPathTorrentSizeText = "//*[@id='torinfo']/h6[1]/text()[1]";

        private static readonly Regex RegexUploadedOn = new Regex(@"^(\w{3})\s{1}(\d{1,}),\s{1}(\d{1,5})");

        private static readonly Dictionary<string, int> MonthMapping = new Dictionary<string, int>
        {
            {"Jan", 1},
            {"Feb", 2},
            {"Mar", 3},
            {"Apr", 4},
            {"May", 5},
            {"Jun", 6},
            {"Jul", 7},
            {"Aug", 8},
            {"Sep", 9},
            {"Oct", 10},
            {"Nov", 11},
            {"Dec", 12}
        };

        public Zooqle(
            IHttpClient httpClient
            , ILogService<Zooqle> logger
            , Settings settings
        ) : base(httpClient)
        {
            _logger = logger;
            _settings = settings;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        public override string Tag => ConfigName;
        
        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var rootNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 0
            );

            var nodeTorrentTable = rootNode.SelectNodes(XPathTorrentTableRowItems);
            
            var torrentParseTasks = nodeTorrentTable
                .Select(async node => await TryParseTorrentDetailsForRow(node));

            var results = (await Task.WhenAll(torrentParseTasks))
                .OfType<Torrent>();

            return results;
        }

        //TODO: Add paging to request...
        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int page
        ) => (await HttpClient.GetStringAsync($"{baseUrl}/search?q={queryString}&s=ns&v=t&sd=d"))
            ?.ToHtmlDocument()
            .DocumentNode;

        private async Task<Torrent> TryParseTorrentDetailsForRow(HtmlNode torrentRowNode)
        {
            try
            {
                var parsedHrefTorrentDetails = ParseTorrentDetailsLink(torrentRowNode);
                var urlDetails = $"{_settings.BaseUrl}{parsedHrefTorrentDetails}#files";

                var torrentDetailsHtmlDocumentNode = await GetTorrentDetailsPageHtmlDocument(urlDetails);

                return torrentDetailsHtmlDocumentNode == null
                    ? null
                    : ParseTorrentDetailsHtmlDocumentNode(torrentDetailsHtmlDocumentNode);
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to parse row", exception);
                
                return null;
            }
        }
        

        private static Torrent ParseTorrentDetailsHtmlDocumentNode(
            HtmlNode torrentDetailsDocumentNode
        ) => new Torrent
        {
            Leechers = ParseNumberFromHtmlNode(torrentDetailsDocumentNode, XPathTorrentDetailsLeechersDiv),
            MagnetUri = ParseMagnetUri(torrentDetailsDocumentNode),
            Name = ParseTorrentName(torrentDetailsDocumentNode),
            Seeders = ParseNumberFromHtmlNode(torrentDetailsDocumentNode, XPathTorrentDetailsSeedersDiv),
            Size = ParseSize(torrentDetailsDocumentNode),
            UploadedOn = ParseUploadedOn(torrentDetailsDocumentNode)
        };

        private static long ParseSize(HtmlNode torrentDetailsDocumentNode)
        {
            var textSize = torrentDetailsDocumentNode
                               .SelectSingleNode(XPathTorrentSizeText)
                               ?.InnerText
                           ?? string.Empty;

            return ParseUtil.GetBytes(textSize);
        }


        private static DateTime? ParseUploadedOn(HtmlNode torrentDetailsDocumentNode)
        {
            var textShortDate = torrentDetailsDocumentNode
                                    .SelectSingleNode(XPathTorrentUploadedOnText)
                                    ?.InnerText
                                ?? string.Empty;

            var dateRegexMatches = RegexUploadedOn.Matches(textShortDate);

            if (dateRegexMatches.Count <= 0)
            {
                return null;
            }

            var dateMatchGroups = dateRegexMatches[0].Groups;

            if (dateMatchGroups.Count != 4)
            {
                return null;
            }

            return new DateTime(
                int.Parse(dateMatchGroups[3].Value)
                , MonthMapping[dateMatchGroups[1].Value]
                , int.Parse(dateMatchGroups[2].Value)
            );
        }

        private static int ParseNumberFromHtmlNode(HtmlNode node, string xPath)
        {
            var textSeeders = node
                .SelectSingleNode(xPath)
                ?.InnerText;

            int.TryParse(textSeeders, out var result);

            return result;
        }

        private static string ParseMagnetUri(
            HtmlNode torrentDetailsDocumentNode
        ) => torrentDetailsDocumentNode
            .SelectSingleNode(XPathTorrentDetailsMagnetHref)
            ?.GetAttributeValue("href", null);

        private static string ParseTorrentName(HtmlNode torrentDetailsDocumentNode)
        {
            var nodeTorrentName = torrentDetailsDocumentNode.SelectSingleNode(
                XPathTorrentDetailsTorrentName
            );

            // Remove span that adds ".torrent" extension.
            nodeTorrentName
                ?.Descendants("span")
                ?.FirstOrDefault()
                ?.Remove();

            return nodeTorrentName?.InnerText;
        }

        private async Task<HtmlNode> GetTorrentDetailsPageHtmlDocument(
            string urlDetails
        ) => (await HttpClient.GetStringAsync(urlDetails))
            ?.ToHtmlDocument()
            .DocumentNode;

        private static string ParseTorrentDetailsLink(
            HtmlNode torrentRowNode
        ) => torrentRowNode
            .SelectSingleNode("td[2]/a")
            ?.GetAttributeValue("href", string.Empty);
        
    }
}