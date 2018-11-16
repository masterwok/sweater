using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class ThePirateBayIndexer : BaseIndexer
    {
        private Settings _settings;

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string Foo { get; set; }
        }

        public ThePirateBayIndexer(IWebClient webClient) : base(webClient)
            => Expression.Empty();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task<bool> Login()
        {
            throw new NotImplementedException();
        }

        public override Task<IndexerResult> Query(string queryString)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Logout()
        {
            throw new NotImplementedException();
        }
    }
}