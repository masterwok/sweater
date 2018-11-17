using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sweater.Core.Constants;
using Sweater.Core.Indexers.Contracts;
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

        private IEnumerable<IIndexer> GetIndexersForQuery(Indexer indexer)
            => indexer == Indexer.All
                // Get all indexer instances
                ? Enum.GetValues(typeof(Indexer))
                    .Cast<Indexer>()
                    .Where(i => i != Indexer.All)
                    .Select(_getIndexer)
                // Get single indexer instance
                : new[] {_getIndexer(indexer)};

        public async Task<IEnumerable<IndexerResult>> Query(Query query)
        {
            var indexers = GetIndexersForQuery(query.Indexer);


            // TODO: Actually query..
            return new List<IndexerResult>();
        }
    }
}