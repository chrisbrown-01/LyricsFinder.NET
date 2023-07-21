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

            switch (sortOrder)
            {
                case "id_desc":
                    modifiedList = modifiedList.OrderByDescending(s => s.Id);
                    break;
                case "id_asc":
                    modifiedList = modifiedList.OrderBy(s => s.Id);
                    break;
                case "name_desc":
                    modifiedList = modifiedList.OrderByDescending(s => s.Name);
                    break;
                case "name_asc":
                    modifiedList = modifiedList.OrderBy(s => s.Name);
                    break;
                case "artist_desc":
                    modifiedList = modifiedList.OrderByDescending(s => s.Artist);
                    break;
                case "artist_asc":
                    modifiedList = modifiedList.OrderBy(s => s.Artist);
                    break;
                case "date_desc":
                    modifiedList = modifiedList.OrderByDescending(s => s.QueryDate);
                    break;
                case "date_asc":
                    modifiedList = modifiedList.OrderBy(s => s.QueryDate);
                    break;
                default:
                    modifiedList = modifiedList.OrderBy(s => s.Id);
                    break;
            }

            return modifiedList;
        }
    }
}
