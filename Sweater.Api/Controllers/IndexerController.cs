using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sweater.Api.Models;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexerController : ControllerBase
    {
        private readonly IIndexerQueryService _queryService;
        private readonly long _queryTimeoutMs;

        public IndexerController(
            IIndexerQueryService queryService
            , QueryConfig queryConfig
        )
        {
            _queryService = queryService;
            _queryTimeoutMs = queryConfig.QueryTimeoutMs;
        }

        /// <summary>
        /// Get the tags of all of the implemented indexers.
        /// </summary>
        /// <returns>A string array of indexer tags.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<string>> Tags() => await _queryService.GetIndexerTags();

        /// <summary>
        /// Query the indexers.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A TorrentQueryResult instance.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<PaginatedResponse<TorrentQueryResult>> Query(Query query)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_queryTimeoutMs));
            
            var results = await _queryService.Query(query, cts.Token);
            
            var pageIndex = query.PageIndex;
            var pageSize = query.PageSize;

            return new PaginatedResponse<TorrentQueryResult>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemCount = results.Count,
                Items = results
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToList()
            };
        }
    }
}