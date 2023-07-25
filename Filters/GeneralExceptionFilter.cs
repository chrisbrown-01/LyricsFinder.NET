using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LyricsFinder.NET.Filters
{
    public class GeneralExceptionFilter : IExceptionFilter
    {
        // If logging, could perform all logging actions in here rather than inside controller methods
        public void OnException(ExceptionContext context)
        {
            //context.Result = new StatusCodeResult(500);
            context.Result = new ObjectResult(new { Message = "Unexpected server error encountered" }) { StatusCode = 500 };
            return;
        }
    }
}