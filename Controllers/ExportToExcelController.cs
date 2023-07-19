using ClosedXML.Excel;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Controllers
{
    public class ExportToExcelController : Controller
    {
        private readonly ISongDbRepo _db;
        private readonly UserManager<CustomAppUserData> _userManager;

        // TODO: rework to reduce code re-use
        public ExportToExcelController(ISongDbRepo db, UserManager<CustomAppUserData> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Exports entire song database to Excel file according to search/sort parameters
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <returns></returns>
        public ActionResult ExportToExcel(string sortOrder, string currentFilter)
        {
            var spotifySearchList = _db.GetAllSongsInDb();

            // filter/sort code taken from https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
            // and https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

            if (!String.IsNullOrEmpty(currentFilter))
            {
                spotifySearchList = spotifySearchList.Where(s => s.Name.Contains(currentFilter, StringComparison.CurrentCultureIgnoreCase)
                                                            || s.Artist.Contains(currentFilter, StringComparison.CurrentCultureIgnoreCase)
                                                            );
            }

            switch (sortOrder)
            {
                case "id_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Id);
                    break;
                case "id_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Id);
                    break;
                case "name_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Name);
                    break;
                case "name_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Name);
                    break;
                case "artist_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Artist);
                    break;
                case "artist_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Artist);
                    break;
                case "date_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.QueryDate);
                    break;
                case "date_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.QueryDate);
                    break;
                default:
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Id);
                    break;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Song Database");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Song Name";
                worksheet.Cell(currentRow, 3).Value = "Artist";
                worksheet.Cell(currentRow, 4).Value = "Query Date";
                worksheet.Cell(currentRow, 5).Value = "Deezer Id";
                worksheet.Cell(currentRow, 6).Value = "Song Duration";
                worksheet.Cell(currentRow, 7).Value = "Artist Art";
                worksheet.Cell(currentRow, 8).Value = "Album Art";
                worksheet.Cell(currentRow, 9).Value = "Lyrics";

                foreach (var song in spotifySearchList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = song.Id;
                    worksheet.Cell(currentRow, 2).Value = song.Name;
                    worksheet.Cell(currentRow, 3).Value = song.Artist;
                    worksheet.Cell(currentRow, 4).Value = song.QueryDate;
                    worksheet.Cell(currentRow, 5).Value = song.DeezerId;
                    worksheet.Cell(currentRow, 6).Value = song.SongDuration;
                    worksheet.Cell(currentRow, 7).Value = song.ArtistArtLink;
                    worksheet.Cell(currentRow, 8).Value = song.AlbumArtLink;
                    worksheet.Cell(currentRow, 9).Value = song.Lyrics;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "song_database.xlsx");
                }
            }
        }

        /// <summary>
        /// Exports user's favourited song list database to Excel file according to search/sort parameters
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> ExportFavouritesToExcelAsync(string sortOrder, string currentFilter)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

            // Find all songs that user has favourited from UserFavouriteSongs db. Foreign key forces inclusion of corresponding SpotifyUserInput song object
            var usersFavouriteSongs = _db.GetFavouriteSongDb().Where(x => x.UserId == loggedInUser.Id).Include(s => s.SpotifyUserInput);

            var spotifySearchListFavs = new List<Song>();

            // Add each of the SpotifyUserInput objects from the favourite songs db query result to the spotifySearchListFavs list
            foreach (var song in usersFavouriteSongs)
            {
                spotifySearchListFavs.Add(song.SpotifyUserInput);
            }

            // Populate table with the users favourite SpotifyUserInput song objects
            var spotifySearchList = spotifySearchListFavs.AsEnumerable();

            // filter/sort code taken from https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
            // and https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

            if (!String.IsNullOrEmpty(currentFilter))
            {
                spotifySearchList = spotifySearchList.Where(s => s.Name.Contains(currentFilter, StringComparison.CurrentCultureIgnoreCase)
                                                            || s.Artist.Contains(currentFilter, StringComparison.CurrentCultureIgnoreCase)
                                                            );
            }

            switch (sortOrder)
            {
                case "id_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Id);
                    break;
                case "id_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Id);
                    break;
                case "name_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Name);
                    break;
                case "name_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Name);
                    break;
                case "artist_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.Artist);
                    break;
                case "artist_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Artist);
                    break;
                case "date_desc":
                    spotifySearchList = spotifySearchList.OrderByDescending(s => s.QueryDate);
                    break;
                case "date_asc":
                    spotifySearchList = spotifySearchList.OrderBy(s => s.QueryDate);
                    break;
                default:
                    spotifySearchList = spotifySearchList.OrderBy(s => s.Id);
                    break;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Song Database");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Song Name";
                worksheet.Cell(currentRow, 3).Value = "Artist";
                worksheet.Cell(currentRow, 4).Value = "Query Date";
                worksheet.Cell(currentRow, 5).Value = "Deezer Id";
                worksheet.Cell(currentRow, 6).Value = "Song Duration";
                worksheet.Cell(currentRow, 7).Value = "Artist Art";
                worksheet.Cell(currentRow, 8).Value = "Album Art";
                worksheet.Cell(currentRow, 9).Value = "Lyrics";

                foreach (var song in spotifySearchList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = song.Id;
                    worksheet.Cell(currentRow, 2).Value = song.Name;
                    worksheet.Cell(currentRow, 3).Value = song.Artist;
                    worksheet.Cell(currentRow, 4).Value = song.QueryDate;
                    worksheet.Cell(currentRow, 5).Value = song.DeezerId;
                    worksheet.Cell(currentRow, 6).Value = song.SongDuration;
                    worksheet.Cell(currentRow, 7).Value = song.ArtistArtLink;
                    worksheet.Cell(currentRow, 8).Value = song.AlbumArtLink;
                    worksheet.Cell(currentRow, 9).Value = song.Lyrics;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "favourite_songs_database.xlsx");
                }
            }
        }
    }
}
