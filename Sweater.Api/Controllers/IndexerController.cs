using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexerController : ControllerBase
    {
        private readonly IIndexerQueryService _queryService;

        public IndexerController(
            IIndexerQueryService queryService
        )
        {
            _queryService = queryService;
        }

        /// <summary>
        /// Get the tags of all of the implemented indexers.
        /// </summary>
        /// <returns>A string array of indexer tags.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<string>> Tags()
            => await _queryService.GetIndexerTags();

        /// <summary>
        /// Query the indexers.
        /// </summary>
        /// <param name="query">The query to pass to the indexers.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<IndexerResult>> Query(
            [FromBody] Query query
        ) => await _queryService.Query(query);
    }
}