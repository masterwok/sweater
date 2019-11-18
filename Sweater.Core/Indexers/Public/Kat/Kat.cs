using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Kat.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Indexers.Public.Kat
{
    public class Kat : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.Kat.ToString();

        private const string XPathTorrentRow = @"//table[contains(@class, 'torrents_table')]/tbody/tr";

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

        public override Task Login() => Task.CompletedTask;

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var torrents = new List<Torrent>();

            var initialQueryResultsNode = await FetchQueryResults(
                _settings.BaseUrl
                , query.QueryString
                , query.PageIndex + 1
            );

            torrents.AddRange(ParseTorrents(initialQueryResultsNode));

            return torrents;
        }

        private IEnumerable<Torrent> ParseTorrents(HtmlNode rootNode)
        {
            var torrentRows = rootNode.SelectNodes(XPathTorrentRow);

            return new Torrent[0];
        }

        public override Task Logout() => Task.CompletedTask;

        private async Task<HtmlNode> FetchQueryResults(
            string baseUrl
            , string queryString
            , int page
        )
        {
            var response = await HttpClient.GetStringAsync(
                $"{baseUrl}/katsearch/page/{page}/{queryString}"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}