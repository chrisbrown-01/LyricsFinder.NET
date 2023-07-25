using LyricsFinder.NET.Data;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LyricsFinder.NET.Filters
{
    public class CheckSongIdFilter : IAsyncActionFilter
    {
        private readonly ISongDbRepo _db;

        public CheckSongIdFilter(ISongDbRepo db)
        {
            _db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("id"))
            {
                int id = (int)context.ActionArguments["id"]!;
                if (id <= 0)
                {
                    context.Result = new BadRequestObjectResult("Id cannot be less than 1");
                    return;
                }

                var song = await _db.GetSongByIdAsync(id);
                if (song == null)
                {
                    context.Result = new NotFoundObjectResult($"Song with id {id} does not exist.");
                    return;
                }
            }

            await next();
        }
    }
}