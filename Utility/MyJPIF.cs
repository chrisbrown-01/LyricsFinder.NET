using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LyricsFinder.NET.Utility
{
    // Required for API song update (PATCH request) method.
    // See docs here: https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0
    public static class MyJPIF
    {
        public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
