using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sweater.Core.Models;

namespace Sweater.Api.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// This filter will return a 400 bad request JSON response.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class ValidModelStateFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                base.OnResultExecuting(context);
                return;
            }

            context.Result = new JsonResult(
                new ErrorResponse("Invalid model state")
            )
            {
                StatusCode = 400
            };
        }
    }
}