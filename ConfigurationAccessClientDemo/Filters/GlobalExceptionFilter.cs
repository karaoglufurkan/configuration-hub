using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoClient.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var errorResponse = new
            {
                message = "Unexpected error",
                detail = context.Exception.Message
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }
}