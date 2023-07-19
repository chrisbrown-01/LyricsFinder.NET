using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Services;
using LyricsFinder.NET.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LyricsFinder.NET.Controllers
{
    public class SpotifyUserInputController : Controller
    {
        private readonly ISongDbRepo _db;
        //private readonly UserManager<CustomAppUserData> _userManager;
        private readonly ILogger<SpotifyUserInputController> _logger;
        private readonly IMemoryCache _cache;

        public SpotifyUserInputController(ISongDbRepo db,
            //UserManager<CustomAppUserData> userManager,
            ILogger<SpotifyUserInputController> logger,
            IMemoryCache memoryCache)
        {
            _db = db;
            //_userManager = userManager;
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
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            IEnumerable<Song> spotifySearchList = _db.GetAllSongsInDb();

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
        //[Authorize]
        //public async Task<ActionResult> IndexFavourites(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        //{
        //    CustomAppUserData loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

        //    // Find all songs that user has favourited from UserFavouriteSongs db. Foreign key forces inclusion of corresponding SpotifyUserInput song object
        //    var usersFavouriteSongs = _db.GetFavouriteSongDb().Where(x => x.UserId == loggedInUser.Id).Include(s => s.SpotifyUserInput);

        //    List<Song> spotifySearchListFavs = new List<SpotifyUserInput>();

        //    // Add each of the SpotifyUserInput objects from the favourite songs db query result to the spotifySearchListFavs list
        //    foreach (var song in usersFavouriteSongs)
        //    {
        //        spotifySearchListFavs.Add(song.SpotifyUserInput);
        //    }

        //    PaginatedList<Song> paginatedList = CreatePaginatedList(sortOrder, currentFilter, searchString, pageNumber, spotifySearchListFavs);

        //    return View(paginatedList);
        //}


        private PaginatedList<Song> CreatePaginatedList(string sortOrder, string currentFilter, string searchString, int? pageNumber, IEnumerable<Song> spotifySearchList)
        {
            // filter/sort code taken from https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
            // and https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

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

            if (!String.IsNullOrEmpty(searchString))
            {
                spotifySearchList = spotifySearchList.Where(s =>
                    s.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
                    s.Artist.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
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

            int pageSize = 10;

            PaginatedList<Song> paginatedList = PaginatedList<Song>.Create(spotifySearchList, pageNumber ?? 1, pageSize);
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

            //CustomAppUserData loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            //song.CreatedBy = loggedInUser.Id;

            _db.AddSongToDb(song);

            _logger.LogInformation($"Song created: Name=\"{song.Name}\"    Artist=\"{song.Artist}\"");

            try
            {
                song = await RetrieveLyricsAndSongInfo.ScrapeSongInfoFromWebAsync(song);

                _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
                return View("Error", song);
            }

            return RedirectToAction("Index", "SongInfo", new { id = song.Id });
        }


        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
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

            //CustomAppUserData loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            //song.EditedBy = loggedInUser.Id;

            _db.UpdateSongInDb(song);
            _cache.Remove(song.Id);

            _logger.LogInformation("Song edited: {@Song}", song);

            try
            {
                song = await RetrieveLyricsAndSongInfo.ScrapeSongInfoFromWebAsync(song);

                _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
                return View("Error", song);
            }

            return RedirectToAction("Index", "SongInfo", new { id = song.Id });
        }


        /// <summary>
        /// Delete song from database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
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
                //CustomAppUserData loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                var song = await _db.GetDbSongByIdAsync(id);

                if (song == null) return NotFound();

                var logDeletedSong = song;

                _db.DeleteSongFromDb(song);

                //_logger.LogInformation("Song deleted: {@DeletedSong} by user {@User}", song, loggedInUser);

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
