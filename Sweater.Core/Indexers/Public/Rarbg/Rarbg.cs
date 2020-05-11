using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.Rarbg.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Indexers.Public.Rarbg
{
    /// <inheritdoc />
    /// <summary>
    /// This Rarbg indexer uses the torrent Restful API provided by Rarbg. Documentation surrounding
    /// the API can be found at: https://torrentapi.org/apidocs_v2.txt
    /// </summary>
    public class Rarbg : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.Rarbg.ToString();

        // Request rate limiting is set to 1 request / 2 seconds.
        private static readonly int RequestDelayMs = 2100;

        private readonly Settings _settings;

        public Rarbg(
            IHttpClient httpClient
            , Settings settings
        ) : base(httpClient)
        {
            _settings = settings;

            // Torrent API requires a browser user-agent.
            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        private string ApiEndpoint => $"{_settings.BaseUrl}/pubapi_v2.php";

        public override string Tag => ConfigName;

        public override async Task<IEnumerable<Torrent>> Query(Query query, CancellationToken token)
        {
            var appId = _settings.AppId;

            var tokenResponse = await GetToken(appId);

            await Task.Delay(RequestDelayMs, token);

            // https://torrentapi.org/pubapi_v2.php?mode=search&search_string=Blade%20Runner&app_id=test&sort=seeders&format=json_extended&limit=100&token=avtns5qpmk
            var requestUri = QueryHelpers.AddQueryString(
                ApiEndpoint,
                new Dictionary<string, string>
                {
                    {"mode", "search"},
                    {"app_id", appId},
                    {"token", tokenResponse.Token},
                    {"search_string", query.QueryString},
                    {"limit", "100"},
                    {"sort", "seeders"},
                    {"format", "json_extended"}
                }
            );

            var response = await HttpClient.GetAsync(requestUri, token);

            var responseString = await response
                .Content
                .ReadAsStringAsync();

            var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(responseString);

            return queryResponse
                       .TorrentResults
                       ?.Select(r => new Torrent
                       {
                           Name = r.Title,
                           MagnetUri = r.Download,
                           Size = r.Size,
                           Seeders = r.Seeders,
                           Leechers = r.Leechers,
                           UploadedOn = r.Pubdate
                       })
                   ?? new Torrent[0];
        }

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
    }
}