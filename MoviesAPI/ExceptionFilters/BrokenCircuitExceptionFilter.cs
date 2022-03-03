using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Polly.CircuitBreaker;
using System.Net;

namespace MoviesAPI.ExceptionFilters
{
    public class BrokenCircuitExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.Exception is BrokenCircuitException)
            {
                context.Result = context.Result = new ObjectResult("Dependant service unavailable.")
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
