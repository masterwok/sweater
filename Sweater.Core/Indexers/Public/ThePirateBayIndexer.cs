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
        public static readonly string Tag = "thepiratebay";

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string Foo { get; set; }
        }

        public ThePirateBayIndexer(IWebClient webClient,
            Func<string, IConfigurationSection> readConfigSection
        ) : base(webClient)
        {
            var settings = readConfigSection(Tag).Get<Settings>();
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