using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Sweater.Core.Models;

namespace Sweater.Api.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// This filter will return a 400 bad request JSON response when an exception
    /// occurs within a controller action.
    /// </summary>
    public sealed class CatchAllExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<CatchAllExceptionFilter> _logger;

        public CatchAllExceptionFilter(
            ILogger<CatchAllExceptionFilter> logger
        ) => _logger = logger;

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An uncaught exception occured.");

            context.Result = new JsonResult(
                new ErrorResponse("An unexpected error occured while processing the request.")
            )
            {
                StatusCode = 400
            };
        }
    }
}