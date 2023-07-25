using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Filters;
using LyricsFinder.NET.Helpers;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Services.SongRetrieval;
using LyricsFinder.NET.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LyricsFinder.NET.Controllers
{
    //[TypeFilter(typeof(GeneralExceptionFilter))]
    [ServiceFilter(typeof(CheckSongIdFilter))]
    public class SongManagerController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly ISongDbRepo _db;
        private readonly ISongRetrieval _songRetriever;
        private readonly UserManager<CustomAppUserData> _userManager;

        public SongManagerController(
            ISongDbRepo db,
            ISongRetrieval songRetriever,
            UserManager<CustomAppUserData> userManager,
            IMemoryCache memoryCache)
        {
            _db = db;
            _songRetriever = songRetriever;
            _userManager = userManager;
            _cache = memoryCache;
        }

        /// <summary>
        /// Create song object and add to database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create song object and add to database
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckSongForDuplicateFilter))]
        public async Task<IActionResult> CreateAsync(Song song)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            var newSong = new Song()
            {
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = DateTime.Now,
                CreatedBy = loggedInUser!.Id,
            };

            newSong = await _songRetriever.RetrieveSongContentsAsync(newSong);
            await _db.AddSongAsync(newSong);

            return RedirectToAction("Index", "SongContents", new { id = newSong.Id });
        }

        /// <summary>
        /// Delete song from database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return View(await _db.GetSongByIdAsync(id));
        }

        /// <summary>
        /// Delete song from database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePostAsync(int id)
        {
            var song = await _db.GetSongByIdAsync(id);
            await _db.DeleteSongAsync(song!);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> EditAsync(int id)
        {
            return View(await _db.GetSongByIdAsync(id));
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckSongForDuplicateFilter))]
        public async Task<IActionResult> EditAsync(Song song)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            var editedSong = new Song()
            {
                Id = song.Id,
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = DateTime.Now,
                CreatedBy = song.CreatedBy,
                EditedBy = loggedInUser!.Id
            };

            editedSong = await _songRetriever.RetrieveSongContentsAsync(editedSong);
            await _db.UpdateSongAsync(editedSong);
            _cache.Remove(song.Id);

            return RedirectToAction("Index", "SongContents", new { id = song.Id });
        }

        /// <summary>
        /// Display all songs in database. Sort/filter according to parameters
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            var songList = await _db.GetAllSongsAsync();

            PaginatedList<Song> paginatedList = CreatePaginatedList(sortOrder, currentFilter, searchString, pageNumber, songList);

            return View(paginatedList);
        }

        /// <summary>
        /// Display all songs in database that user has favourited. Sort/filter according to parameters
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> IndexFavouritesAsync(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            var favSongList = await _db.GetUserFavSongsAsync(loggedInUser!.Id);

            PaginatedList<Song> paginatedList = CreatePaginatedList(sortOrder, currentFilter, searchString, pageNumber, favSongList);

            return View(paginatedList);
        }

        /// <summary>
        /// Created paginated list used for displaying songs.
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <param name="songList"></param>
        /// <returns></returns>
        private PaginatedList<Song> CreatePaginatedList(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            IEnumerable<Song> songList)
        {
            if (searchString != null) pageNumber = 1;
            else searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.ArtistSortParm = sortOrder == "artist_asc" ? "artist_desc" : "artist_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            var sortedSongList = SortingHelpers.SortSongsList(songList, searchString, sortOrder);

            int pageSize = 10;

            var paginatedList = PaginatedList<Song>.Create(sortedSongList, pageNumber ?? 1, pageSize);
            return paginatedList;
        }
    }
}