using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sweater.Core.Constants;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;
using Sweater.Core.Services;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Services
{
    /// <summary>
    /// This is implementation of IIndexerQueryService uses IMemoryCache to cache results. Cache
    /// configuration is provided through a QueryConfig instance.
    /// </summary>
    public class CachedIndexerQueryService : IIndexerQueryService
    {
        private readonly IIndexerQueryService _queryService;
        private readonly QueryConfig _queryConfig;
        private readonly IMemoryCache _memoryCache;

        public CachedIndexerQueryService(
            ILogger<IndexerQueryService> logger
            , Func<Indexer, IIndexer> getIndexer
            , QueryConfig queryConfig
            , IMemoryCache memoryCache
        )
        {
            _queryService = new IndexerQueryService(
                logger
                , getIndexer
                , queryConfig
            );

            _queryConfig = queryConfig;
            _memoryCache = memoryCache;
        }

        public async Task<IList<string>> GetIndexerTags()
            => await _queryService.GetIndexerTags();

        public async Task<IList<IndexerResult>> Query(Query query)
        {
            if (!_queryConfig.IsCacheEnabled)
            {
                return await _queryService.Query(query);
            }

            return await _memoryCache.GetOrCreateAsync(
                $"{query.Indexer}_{query.QueryString}".ToLower()
                , async entry =>
                {
                    entry.SetSlidingExpiration(
                        TimeSpan.FromMilliseconds(_queryConfig.CacheTimeSpanMs)
                    );

                    return (await _queryService.Query(query)).ToList();
                });
        }
    }
}