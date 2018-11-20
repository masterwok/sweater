using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class ThePirateBayIndexer : BaseIndexer
    {
        private static readonly string TorrentRowXPath = "//*[@id='searchResult']/tr";
        private static readonly string TorrentNameXPath = "td[2]/div/a";
        private static readonly string MagnetUriXPath = "td[2]/a[1]";
        private static readonly string SeedersXPath = "td[3]";
        private static readonly string LeechersXPath = "td[4]";
        private static readonly string DetailsXpath = "td[2]/font";
        private static readonly Regex InfoTextRegex = new Regex(@"Uploaded\s*([\d\W]*),\s*Size\s*(.*),");

        private readonly ILogger<ThePirateBayIndexer> _logger;

        private Settings _settings;

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string BaseUrl { get; set; }
        }

        public override string Tag => Indexer.ThePirateBay.ToString();

        public ThePirateBayIndexer(
            IHttpClient httpClient
            , ILogger<ThePirateBayIndexer> logger
        ) : base(httpClient) => _logger = logger;

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login() => Task.FromResult(true);

        public override Task Logout() => Task.FromResult(true);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var rootNode = await GetHtmlDocument(
                _settings.BaseUrl
                , query.QueryString
                , 1
            );

            var torrents = ParseTorrentEntries(rootNode);

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

        private IEnumerable<Torrent> ParseTorrentEntries(HtmlNode rootNode)
        {
            var nodes = rootNode.SelectNodes(TorrentRowXPath);

            nodes.RemoveAt(nodes.Count - 1);

            var dev = TryParseRow(nodes.First());

            return nodes.Select(TryParseRow);
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
                    UploadedOn = ParseUploadedOn(infoText)
                };
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to parse row", exception);
                return null;
            }
        }

        private static string ParseUploadedOn(string infoText)
        {
            var text = InfoTextRegex.Match(infoText)
                .Groups[1]
                .Value;

            return text.Contains(':')
                ? $"{text.Substring(0, text.IndexOf(' '))}-{DateTime.Now.Year}"
                : text.Replace(' ', '-');
        }
    }
}