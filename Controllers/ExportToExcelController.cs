using ClosedXML.Excel;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Helpers;
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

        public ExportToExcelController(
            ISongDbRepo db, 
            UserManager<CustomAppUserData> userManager)
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
        public async Task<ActionResult> ExportToExcelAsync(string sortOrder, string currentFilter)
        {
            var songList = await _db.GetAllSongsAsync();

            return GenerateExcelFile(sortOrder, currentFilter, songList, isExportForFavourites: false);
        }

        private FileContentResult GenerateExcelFile(string sortOrder, string currentFilter, IEnumerable<Song> songList, bool isExportForFavourites)
        {
            string worksheetName;
            string fileName;
            
            if (isExportForFavourites)
            {
                worksheetName = "Favourites";
                fileName = "favourites_database.xlsx";
            }
            else
            {
                worksheetName = "Songs";
                fileName = "song_database.xlsx";
            }

            var sortedSongList = SortingHelpers.SortSongsList(songList, currentFilter, sortOrder);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(worksheetName);
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

            foreach (var song in sortedSongList)
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

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
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
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            var favSongList = await _db.GetFavSongsAsync(loggedInUser!.Id);

            return GenerateExcelFile(sortOrder, currentFilter, favSongList, isExportForFavourites: true);
        }
    }
}
