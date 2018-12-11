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
        private readonly ILogService<IndexerQueryService> _logger;
        private readonly Func<Indexer, IIndexer> _getIndexer;
        private readonly QueryConfig _queryConfig;

        public IndexerQueryService(
            ILogService<IndexerQueryService> logger
            , Func<Indexer, IIndexer> getIndexer
            , QueryConfig queryConfig
        )
        {
            _logger = logger;
            _getIndexer = getIndexer;
            _queryConfig = queryConfig;
        }

        public Task<IList<string>> GetIndexerTags() => Task.FromResult(
            (IList<string>) Enum.GetNames(typeof(Indexer)).ToList()
        );

        private IEnumerable<IIndexer> GetIndexersForQuery(Indexer indexer)
            => indexer == Indexer.All
                // Get all indexer instances
                ? Enum.GetValues(typeof(Indexer))
                    .Cast<Indexer>()
                    .Where(i => i != Indexer.All)
                    .Select(_getIndexer)
                // Get single indexer instance
                : new[] {_getIndexer(indexer)};

        public async Task<IList<IndexerResult>> Query(Query query)
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
                _logger.LogError(
                    $"{indexer} threw an exception during execution."
                    , exception
                );
            }

            return result;
        }
    }
}