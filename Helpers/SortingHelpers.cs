using LyricsFinder.NET.Models;

namespace LyricsFinder.NET.Helpers
{
    public class SortingHelpers
    {
        public static IEnumerable<Song> SortSongsList(IEnumerable<Song> list, string filter, string sortOrder)
        {
            // filter/sort code taken from https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
            // and https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

            var modifiedList = list.AsEnumerable();

            if (!String.IsNullOrEmpty(filter))
            {
                modifiedList = modifiedList.Where(s =>
                    s.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                    s.Artist.Contains(filter, StringComparison.CurrentCultureIgnoreCase));
            }

            modifiedList = sortOrder switch
            {
                "id_desc" => modifiedList.OrderByDescending(s => s.Id),
                "id_asc" => modifiedList.OrderBy(s => s.Id),
                "name_desc" => modifiedList.OrderByDescending(s => s.Name),
                "name_asc" => modifiedList.OrderBy(s => s.Name),
                "artist_desc" => modifiedList.OrderByDescending(s => s.Artist),
                "artist_asc" => modifiedList.OrderBy(s => s.Artist),
                "date_desc" => modifiedList.OrderByDescending(s => s.QueryDate),
                "date_asc" => modifiedList.OrderBy(s => s.QueryDate),
                _ => modifiedList.OrderBy(s => s.Id),
            };
            return modifiedList;
        }
    }
}