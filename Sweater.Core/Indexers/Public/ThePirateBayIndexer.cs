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
        private readonly Settings _settings;

        // ReSharper disable once ClassNeverInstantiated.Local
        public sealed class Settings
        {
            public string Foo { get; set; }
        }

        public ThePirateBayIndexer(
            IWebClient webClient
            , Settings settings
        ) : base(webClient)
        {
            _settings = settings;
        }

        public override Task<bool> Login()
        {
            throw new System.NotImplementedException();
        }

        public override Task<IndexerResult> Query(string queryString)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}