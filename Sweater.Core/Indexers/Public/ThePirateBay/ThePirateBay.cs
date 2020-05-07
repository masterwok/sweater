using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Public.ThePirateBay.Models;
using Sweater.Core.Models;
using Sweater.Core.Utils;
using Settings = Sweater.Core.Indexers.Public.ThePirateBay.Models.Settings;

namespace Sweater.Core.Indexers.Public.ThePirateBay
{
    public class ThePirateBay : BaseIndexer
    {
        public static readonly string ConfigName = Indexer.ThePirateBay.ToString();

        private const string KeyInfoHash = "{info_hash}";

        private static readonly string MagnetUri =
            $"magnet:?xt=urn:btih:{KeyInfoHash}&dn=Carey%20M.%20Tribe%20of%20Hackers." +
            "%20Cybersecurity%20Advice...2019&tr=udp%3A%2F%2Ftracker.coppersurfer.tk" +
            "%3A6969%2Fannounce&tr=udp%3A%2F%2F9.rarbg.to%3A2920%2Fannounce&tr=udp%3" +
            "A%2F%2Ftracker.opentrackr.org%3A1337&tr=udp%3A%2F%2Ftracker.internetwar" +
            "riors.net%3A1337%2Fannounce&tr=udp%3A%2F%2Ftracker.leechers-paradise.or" +
            "g%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969%2Fann" +
            "ounce&tr=udp%3A%2F%2Ftracker.pirateparty.gr%3A6969%2Fannounce&tr=udp%3A" +
            "%2F%2Ftracker.cyberia.is%3A6969%2Fannounce";

        private readonly Settings _settings;

        public override string Tag => ConfigName;

        public ThePirateBay(
            IHttpClient httpClient
            , Settings settings
        ) : base(httpClient)
        {
            _settings = settings;

            HttpClient.SetDefaultUserAgent(UserAgent.Chrome);
        }

        public override Task Login() => Task.FromResult(true);

        public override Task Logout() => Task.FromResult(true);

        public override async Task<IEnumerable<Torrent>> Query(Query query)
        {
            var responseString = await HttpClient.GetStringAsync(
                $"{_settings.BaseUrl}/q.php?q={query.QueryString}&cat=0"
            );

            var items = JsonConvert.DeserializeObject<List<QueryResponseItem>>(responseString);

            return items.Select(n => new Torrent
            {
                Leechers = (int) n.Leechers,
                MagnetUri = MagnetUri.Replace(KeyInfoHash, n.InfoHash),
                Name = n.Name,
                Seeders = (int) n.Seeders,
                Size = n.Size,
                UploadedOn = DateTimeOffset.FromUnixTimeSeconds(n.Added).Date
            });
        }
    }
}