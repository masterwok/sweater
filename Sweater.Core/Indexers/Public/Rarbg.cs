using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class Rarbg : BaseIndexer
    {
        private Settings _settings;

        private ILogger<Rarbg> _logger;

        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private sealed class Settings
        {
            public string BaseUrl { get; set; }
            public int MaxPages { get; set; }
        }

        public Rarbg(
            IHttpClient httpClient
            , ILogger<Rarbg> logger
        ) : base(httpClient) => _logger = logger;

        public override string Tag => Indexer.Rarbg.ToString();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override Task<IEnumerable<Torrent>> Query(Query query)
        {
            throw new System.NotImplementedException();
        }
    }
}