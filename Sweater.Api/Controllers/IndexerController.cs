using Microsoft.AspNetCore.Mvc;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexerController : ControllerBase
    {
        private readonly IIndexerQueryService _queryService;

        public IndexerController(IIndexerQueryService queryService)
        {
            _queryService = queryService;
        }

        public ActionResult<string> Query(
            string queryString
            , string indexer = "all"
        )
        {
            return $"queryString = {queryString}, indexer = {indexer}";
        }
    }
}