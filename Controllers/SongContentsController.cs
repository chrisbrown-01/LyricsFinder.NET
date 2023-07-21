using LyricsFinder.NET.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Areas.Identity.Models;

namespace LyricsFinder.NET.Controllers
{
    public class SongContentsController : Controller
    {
        private readonly ISongDbRepo _db;
        private readonly UserManager<CustomAppUserData> _userManager;
        private readonly ILogger<SongContentsController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

        public SongContentsController(
            ISongDbRepo db,
            UserManager<CustomAppUserData> userManager,
            ILogger<SongContentsController> logger,
            IEmailSender emailSender,
            IMemoryCache memoryCache)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _cache = memoryCache;

            _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(1),
                Size = 1
            };
        }

        public async Task<ActionResult> Index(int id)
        {
            Song? song;

            if (!_cache.TryGetValue(id, out song))
            {
                song = await _db.GetDbSongByIdAsync(id);

                if (song == null) return NotFound();

                _cache.Set(id, song, _memoryCacheEntryOptions);
            }

            return View(song);
        }

        /// <summary>
        /// Edit song information and manually input lyrics
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> UpdateWrongSongInfo(int id)
        {
            var song = await _db.GetDbSongByIdAsync(id);

            if (song == null) return NotFound();

            return View(song);
        }

        /// <summary>
        /// Edit song information and manually input lyrics
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWrongSongInfo([FromForm] Song song)
        {
            // TODO: replace try-catch with filter
            try
            {
                if (!ModelState.IsValid) return View(song);

                song.QueryDate = DateTime.Now;
                song.LyricsSet = true;

                var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                song.EditedBy = loggedInUser.Id;

                await _db.UpdateSongInDb(song);

                _logger.LogInformation("Song details information edited: {@Song}", song);

                _cache.Remove(song.Id);

                return View("Index", song);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when attempting to edit song details information: {@Exception}", ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Add song to favourite song database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="redirectUrl">Redirect user back to SongContents index page via url</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> AddToFavourites(int id, string redirectUrl)
        {
            if (id <= 0) return BadRequest();

            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

            await _db.AddFavSongToDb(id, loggedInUser.Id);

            TempData["SongAddedToFavourites"] = "Favourite song added";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Remove song from favourite song database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="redirectUrl">Redirect user back to SongContents index page via url</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> RemoveFromFavourites(int id, string redirectUrl)
        {
            if (id <= 0) return BadRequest();

            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

            await _db.RemoveFavSongFromDb(id, loggedInUser.Id);

            TempData["SongRemovedFromFavourites"] = "Favourite song removed";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Send email to site admin to notify that wrong song info has been entered
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="redirectUrl">Redirect user back to SongContents index page via url</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> NotifyWrongSongInfoViaEmailAsync(int id, string redirectUrl)
        {
            if (id <= 0) return BadRequest();

            var loggedInUser = await _userManager.FindByEmailAsync(User.Identity.Name);

            try
            {
                await _emailSender.SendEmailAsync("NOTIFY_SITE_ADMIN",
                                    "Wrong song info report",
                                    $"Wrong song info reported by user with email: {loggedInUser.Email} for song ID: {id}" +
                                    $"<br>" +
                                    $"<br>" +
                                    $"<a href={redirectUrl}>Link to song info page</a>");
            }
            catch (Exception ex)
            {
                _logger.LogError("Wrong song info was reported by user, but email service failed to deliver notification email. Exception: {@Exception}", ex.Message);
                TempData["EmailNotificationServiceFailed"] = "Email notification service failed";
                return Redirect(redirectUrl);
            }

            TempData["WrongSongInfoReported"] = "Wrong song info reported";

            return Redirect(redirectUrl);
        }
    }
}
