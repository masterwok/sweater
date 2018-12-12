using System.Collections.Generic;
using System.Linq;
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

        public IndexerController(IIndexerQueryService queryService)
            => _queryService = queryService;

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
        /// <param name="pageIndex">The page index to fetch (default: 0).</param>
        /// <param name="pageSize">The maximum number of items per page (default: 10).</param>
        /// <returns>A TorrentQueryResult instance.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<PaginatedResponse<TorrentQueryResult>> Query(
            [FromQuery] Query query
            , int pageIndex = 0
            , int pageSize = 10
        )
        {
            var results = await _queryService.Query(query);

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