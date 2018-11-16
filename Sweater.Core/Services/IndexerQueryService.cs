using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Services
{
    /// <summary>
    /// An IIndexerQueryService implementation. This service should be injected as transient so a
    /// new instance is created for each request. Failure to do so will result
    /// </summary>
    public class IndexerQueryService : IIndexerQueryService
    {
        private readonly Func<Indexer, IIndexer> _getIndexer;
        private readonly QueryConfig _queryConfig;

        public IndexerQueryService(
            Func<Indexer, IIndexer> getIndexer
            , QueryConfig queryConfig
        )
        {
            _getIndexer = getIndexer;
            _queryConfig = queryConfig;
        }

        public Task<IEnumerable<string>> GetIndexerTags() =>
            Task.FromResult(Enum.GetNames(typeof(Indexer)).AsEnumerable());

        public async Task<IEnumerable<IndexerResult>> Query(Query query)
        {
            var indexer = _getIndexer(Indexer.ThePirateBay);

            // TODO: Actually query..
            return new List<IndexerResult>
            {
                new IndexerResult(),
                new IndexerResult(),
                new IndexerResult(),
                new IndexerResult()
            };
        }

        public async Task<IndexerResult> QueryByIndexer(
            string tag
            , string queryString
        ) => throw new NotImplementedException();
    }
}