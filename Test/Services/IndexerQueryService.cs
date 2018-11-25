using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IndexerQueryService> _logger;
        private readonly Func<Indexer, IIndexer> _getIndexer;
        private readonly QueryConfig _queryConfig;

        public IndexerQueryService(
            ILogger<IndexerQueryService> logger
            , Func<Indexer, IIndexer> getIndexer
            , QueryConfig queryConfig
        )
        {
            _logger = logger;
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

            return await Task.WhenAll(indexers.Select(
                indexer => QueryIndexer(indexer, query)
            ));
        }

        private async Task<IndexerResult> QueryIndexer(
            IIndexer indexer
            , Query query
        )
        {
            var result = new IndexerResult
            {
                Indexer = indexer.Tag
            };

            try
            {
                await indexer.Login();

                result.Torrents = await indexer.Query(query);

                await indexer.Logout();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"{indexer} threw an exception during execution.");
            }

            return result;
        }
    }
}