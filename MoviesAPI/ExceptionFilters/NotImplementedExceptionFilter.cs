using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace MoviesAPI.ExceptionFilters
{
    public class NotImplementedExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.Exception is NotImplementedException notImplementedException)
            {
                context.Result = context.Result = new ObjectResult(notImplementedException.Message)
                {
                    StatusCode = (int)HttpStatusCode.NotImplemented
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
