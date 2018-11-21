using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Rarbg.Models;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public.Rarbg
{
    public class Rarbg : BaseIndexer
    {
        private Settings _settings;

        private ILogger<Rarbg> _logger;

        public Rarbg(
            IHttpClient httpClient
            , ILogger<Rarbg> logger
        ) : base(httpClient)
        {
            _logger = logger;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        public override string Tag => Indexer.Rarbg.ToString();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var tokenResponse = await GetToken(_settings.AppId);

            return null;
        }

        private string ApiEndpoint => $"{_settings.BaseUrl}/pubapi_v2.php";

        private async Task<TokenResponse> GetToken(string appId)
        {
            var requestUri = QueryHelpers.AddQueryString(
                ApiEndpoint,
                new Dictionary<string, string>
                {
                    {"get_token", "get_token"},
                    {"app_id", appId}
                }
            );

            var response = await HttpClient.GetStringAsync(requestUri);

            return JsonConvert.DeserializeObject<TokenResponse>(response);
        }

        // Seems like torrentapi is needed?
        // Docs: https://torrentapi.org/apidocs_v2.txt

        // Get token:
        // https://torrentapi.org/pubapi_v2.php?get_token=get_token&app_id=test

        // Search, sort seeders, json extended, limit
        // https://torrentapi.org/pubapi_v2.php?mode=search&search_string=Blade%20Runner&app_id=test&sort=seeders&format=json_extended&limit=100&token=avtns5qpmk

        // Need to set web client delay

        private async Task<HtmlNode> GetHtmlDocument(
            string baseUrl
            , string queryString
            , int page
        )
        {
            var url = $"{baseUrl}/torrents.php?search={queryString}&page={page}&order=seeders&by=DESC";
            var response = await HttpClient.GetStringAsync(
//                https://rarbg.to/torrents.php?search=hackers+1995&order=seeders&by=DESC
                $"{baseUrl}/torrents.php?search={queryString}&page={page}&order=seeders&by=DESC"
            );

            return response
                .ToHtmlDocument()
                .DocumentNode;
        }
    }
}