using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
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

        public IndexerQueryService(
            ILogService<IndexerQueryService> logger
            , Func<Indexer, IIndexer> getIndexer
        )
        {
            _logger = logger;
            _getIndexer = getIndexer;
        }

        public Task<IList<string>> GetIndexerTags() => Task.FromResult(
            (IList<string>) Enum.GetNames(typeof(Indexer)).ToList()
        );

        private IEnumerable<IIndexer> GetIndexersForQuery(
            ICollection<Indexer> requestedIndexers
        )
        {
            var indexerValues = Enum
                .GetValues(typeof(Indexer))
                .Cast<Indexer>();

            // No indexers provided, use all indexers.
            if (!requestedIndexers.Any())
            {
                return indexerValues.Select(_getIndexer);
            }

            // Subset of indexers requested
            return indexerValues
                .Where(requestedIndexers.Contains)
                .Select(_getIndexer);
        }

        public async Task<IList<TorrentQueryResult>> Query(
            Query query
            , CancellationToken? token = null
        )
        {
            if (token == null)
            {
                using var cts = new CancellationTokenSource();
                token = cts.Token;
            }

            var indexers = GetIndexersForQuery(query.Indexers);

            var indexerResults = await Task.WhenAll(
                indexers.Select(indexer => QueryIndexer(
                    indexer
                    , query
                    , (CancellationToken) token
                )));

            return indexerResults
                .FlattenIndexerResults()
                .SortTorrentQueryResults(
                    query.SortField
                    , query.SortOrder
                );
        }


        private async Task<IndexerResult> QueryIndexer(
            IIndexer indexer
            , Query query
            , CancellationToken token
        )
        {
            var indexerResult = new IndexerResult {Indexer = indexer.Tag};

            try
            {
                var result = await indexer.Query(query, token);

                indexerResult.Torrents = result?.Where(t => t != null)
                                         ?? new Torrent[0];
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError(
                    $"{indexer} timed out during query."
                    , exception
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    $"{indexer} threw an exception during execution."
                    , exception
                );
            }

            return indexerResult;
        }
    }
}