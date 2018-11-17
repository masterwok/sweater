using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
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

        public override string Tag => Indexer.ThePirateBay.ToString();

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

        public override Task<IEnumerable<Torrent>> Query(Query query)
        {
            return null;
        }

        public override Task Logout()
        {
            return Task.FromResult(true);
        }
    }
}