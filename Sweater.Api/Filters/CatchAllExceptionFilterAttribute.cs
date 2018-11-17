using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sweater.Core.Models;

namespace Sweater.Api.Filters
{
    /// <summary>
    /// This filter will return a 400 bad request JSON response when an exception
    /// occurs within a controller action.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CatchAllExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(
                new ErrorResponse("An unexpected error occured while processing the request.")
            )
            {
                StatusCode = 400
            };
        }
    }
}