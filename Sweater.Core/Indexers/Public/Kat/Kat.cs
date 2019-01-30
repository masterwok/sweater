using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Kat.Models;
using Sweater.Core.Models;
using Sweater.Core.Utils;

namespace Sweater.Core.Indexers.Public.Kat
{
    /// <inheritdoc />
    /// <summary>
    /// This indexer implementation scrapes KickassTorrents.
    /// </summary>
    public class Kat : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.Kat.ToString();

        private const string PageNumberXPath = "//*[@id=\"wrapperInner\"]/div[2]/table/tbody/tr/td[1]/div[2]/a";
        private const string PageSkipTextValue = ">>";

        private const string TorrentRowXPath = "//tr[@class='even' or @class='odd']";
        private const string TorrentRowLinkXPath = ".//a[@class='cellMainLink']";
        private const string TorrentMagnetXPath = "//a[@title='Magnet link']";
        private const string TorrentRowSizeXPath = "td[2]";
        private const string TorrentRowSeedXPath = "td[5]";
        private const string TorrentRowLeechXPath = "td[6]";

        private readonly Settings _settings;

        public override string Tag => ConfigName;

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public Kat(
            IHttpClient httpClient
            , Settings settings
        ) : base(httpClient) => _settings = settings;

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var documentNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 0
            );

            var firstPage = await ParseTorrents(_settings.BaseUrl, documentNode);
            var lastPageIndex = GetLastPageIndex(documentNode);
            var pageRange = PagingUtil.GetPageRange(lastPageIndex, _settings.MaxPages);

            if (pageRange == null)
            {
                return firstPage;
            }

            var torrents = new List<Torrent>(firstPage);

            torrents.AddRange((await Task.WhenAll(pageRange.Select(async page =>
                {
                    var response = await GetHtmlDocument(
                        _settings.BaseUrl
                        , query.QueryString
                        , page
                    );

                    return await ParseTorrents(_settings.BaseUrl, response);
                })
            )).SelectMany(i => i));

            return torrents;
        }

        private static int GetLastPageIndex(HtmlNode rootNode)
        {
            var buttons = rootNode.SelectNodes(PageNumberXPath);

            // No buttons mean there's no pages to parse
            if (buttons == null || buttons.Count == 0)
            {
                return 0;
            }

            var lastButtonText = buttons
                .Last()
                .InnerText;

            // Last button was double chevron => drop chevron button to get last page
            if (lastButtonText.Equals(PageSkipTextValue))
            {
                buttons.RemoveAt(buttons.Count - 1);
            }

            return int.Parse(buttons.Last().InnerText);
        }

        private async Task<Torrent[]> ParseTorrents(
            string baseUrl
            , HtmlNode documentNode
        )
        {
            var torrentRows = documentNode.SelectNodes(TorrentRowXPath);

            return await Task.WhenAll(torrentRows.Select(async n => await ParseTorrent(baseUrl, n)));
        }

        private async Task<Torrent> ParseTorrent(
            string baseUrl
            , HtmlNode torrentRowNode
        )
        {
            var mainLink = torrentRowNode?.SelectSingleNode(TorrentRowLinkXPath);

            var magnetUri = await GetMagnetUri(
                baseUrl
                , mainLink.GetAttributeValue("href", null)
            );

            return new Torrent
            {
                MagnetUri = magnetUri,
                Name = mainLink.InnerText.Trim(),
                Leechers = int.Parse(torrentRowNode.SelectSingleNode(TorrentRowLeechXPath).InnerText.Trim()),
                Seeders = int.Parse(torrentRowNode.SelectSingleNode(TorrentRowSeedXPath).InnerText.Trim()),
                Size = ParseUtil.GetBytes(torrentRowNode.SelectSingleNode(TorrentRowSizeXPath).InnerText.Trim()),
//                UploadedOn = string.Empty
            };
        }

        private async Task<string> GetMagnetUri(string baseUrl, string href)
        {
            var response = await HttpClient.GetStringAsync($"{baseUrl}{href}");

            return response
                .ToHtmlDocument()
                .DocumentNode
                .SelectSingleNode(TorrentMagnetXPath)
                .GetAttributeValue("href", null);
        }

        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int page
        )
        {
            // https://kat.am/usearch/hackers%201995/?sortby=seeders&sort=desc
            var response = await HttpClient.GetStringAsync(
                $"{baseUrl}/usearch/{queryString}/{page}/?sortby=seeders&sort=desc"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}