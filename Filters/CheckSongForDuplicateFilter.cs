using LyricsFinder.NET.Data;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using LyricsFinder.NET.Data.Repositories;

namespace LyricsFinder.NET.Filters
{
    public class CheckSongForDuplicateFilter : Attribute, IAsyncActionFilter
    {
        private readonly ISongDbRepo _db;

        public CheckSongForDuplicateFilter(ISongDbRepo db)
        {
            _db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var song = (Song)context.ActionArguments["song"]!;
            if (_db.IsSongDuplicate(song))
            {
                context.Result = new ViewResult { ViewName = "~/Views/SongManager/DuplicateFound.cshtml" };
                return;
            }

            await next();
        }
    }
}