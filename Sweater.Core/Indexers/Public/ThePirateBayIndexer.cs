using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class ThePirateBayIndexer : BaseIndexer
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string Foo { get; set; }
        }

        private Settings _settings;

        public ThePirateBayIndexer(IHttpClient httpClient) : base(httpClient)
            => Expression.Empty();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login()
        {
            return Task.FromResult(true);
        }

        public override Task<IndexerResult> Query(Query query)
        {
            return Task.FromResult(new IndexerResult());
        }

        public override Task Logout()
        {
            return Task.FromResult(true);
        }
    }
}