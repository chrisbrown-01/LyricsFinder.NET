using DocumentFormat.OpenXml.Spreadsheet;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Helpers;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Services;
using LyricsFinder.NET.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LyricsFinder.NET.Controllers
{
    // SpotifyUserInputController
    public class SongManagerController : Controller
    {
        private readonly ISongDbRepo _db;
        private readonly UserManager<CustomAppUserData> _userManager;
        private readonly ILogger<SongManagerController> _logger;
        private readonly IMemoryCache _cache;

        public SongManagerController(ISongDbRepo db,
            UserManager<CustomAppUserData> userManager,
            ILogger<SongManagerController> logger,
            IMemoryCache memoryCache)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Display all songs in database. Sort/filter according to parameters
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult Index(
            string sortOrder, 
            string currentFilter, 
            string searchString, 
            int? pageNumber)
        {
            var spotifySearchList = _db.GetAllSongsInDb();

            PaginatedList<Song> paginatedList = CreatePaginatedList(sortOrder, currentFilter, searchString, pageNumber, spotifySearchList);

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
        public async Task<ActionResult> IndexFavourites(
            string sortOrder, 
            string currentFilter, 
            string searchString, 
            int? pageNumber)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

            var spotifySearchListFavs = _db.GetUserFavSongs(loggedInUser.Id);

            PaginatedList<Song> paginatedList = CreatePaginatedList(sortOrder, currentFilter, searchString, pageNumber, spotifySearchListFavs);

            return View(paginatedList);
        }

        private PaginatedList<Song> CreatePaginatedList(
            string sortOrder, 
            string currentFilter, 
            string searchString, 
            int? pageNumber, 
            IEnumerable<Song> spotifySearchList)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.ArtistSortParm = sortOrder == "artist_asc" ? "artist_desc" : "artist_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            spotifySearchList = SortingHelpers.SortSongsList(spotifySearchList, searchString, sortOrder);

            int pageSize = 10;

            var paginatedList = PaginatedList<Song>.Create(spotifySearchList, pageNumber ?? 1, pageSize);
            return paginatedList;
        }


        /// <summary>
        /// Create song object and add to database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
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
        public async Task<ActionResult> Create(Song song)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Create song post request did not pass ModelState validation. Object passed in had following details: {@Exception}", song);
                return View(song);
            }

            if (_db.IsSongDuplicate(song)) return View("DuplicateFound");

            song.QueryDate = DateTime.Now;

            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            song.CreatedBy = loggedInUser.Id;

            await _db.AddSongToDb(song);

            _logger.LogInformation($"Song created: Name=\"{song.Name}\"    Artist=\"{song.Artist}\"");

            try
            {
                song = await RetrieveLyricsAndSongInfo.ScrapeSongInfoFromWebAsync(song);

                await _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
                return View("Error", song);
            }

            return RedirectToAction("Index", "SongContents", new { id = song.Id });
        }


        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var song = await _db.GetDbSongByIdAsync(id);

            if (song == null) return NotFound();

            return View(song);
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Song song)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Create song post request did not pass ModelState validation. Object passed in had following details: {@Exception}", song);
                return View(song);
            }

            if (_db.IsSongDuplicate(song)) return View("DuplicateFound");

            song.QueryDate = DateTime.Now;

            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            song.EditedBy = loggedInUser.Id;

            await _db.UpdateSongInDb(song);
            _cache.Remove(song.Id);

            _logger.LogInformation("Song edited: {@Song}", song);

            try
            {
                song = await RetrieveLyricsAndSongInfo.ScrapeSongInfoFromWebAsync(song);

                await _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
                return View("Error", song);
            }

            return RedirectToAction("Index", "SongContents", new { id = song.Id });
        }


        /// <summary>
        /// Delete song from database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var song = await _db.GetDbSongByIdAsync(id);

            if (song == null) return NotFound();

            return View(song);
        }

        /// <summary>
        /// Delete song from database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePost(int id)
        {
            // TODO: replace try-catch with filter?
            try
            {
                var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                var song = await _db.GetDbSongByIdAsync(id);

                if (song == null) return NotFound();

                await _db.DeleteSongFromDb(song);

                _logger.LogInformation("Song deleted: {@DeletedSong} by user {@User}", song, loggedInUser);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when attempting to delete song: {@Exception}", ex.Message);
                return StatusCode(500);
            }
        }
    }
}
