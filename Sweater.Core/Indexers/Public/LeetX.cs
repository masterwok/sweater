using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class LeetX : BaseIndexer
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Settings
        {
            public string BaseUrl { get; set; }
        }

        private Settings _settings;

        public LeetX(IHttpClient httpClient) : base(httpClient)
            => Expression.Empty();

        public override BaseIndexer Configure(IConfiguration configuration)
        {
            _settings = configuration.Get<Settings>();

            return this;
        }

        public override Task Login() => Task.FromResult(0);

        public override Task Logout() => Task.FromResult(0);

        public override Task<IndexerResult> Query(Query query)
        {
            var requestUrl = CreateRequestUrl(
                _settings.BaseUrl
                , query.QueryString
                , 1
            );

            return Task.FromResult(new IndexerResult());
        }

        private static string CreateRequestUrl(
            string baseUrl
            , string queryString
            , int page
        ) => $"{baseUrl}/search/{queryString}/{page}/";
    }
}