﻿using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Filters;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LyricsFinder.NET.Controllers
{
    [ServiceFilter(typeof(CheckSongIdFilter))]
    public class SongContentsController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly ISongDbRepo _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<CustomAppUserData> _userManager;

        public SongContentsController(
            ISongDbRepo db,
            UserManager<CustomAppUserData> userManager,
            IEmailSender emailSender,
            IMemoryCache memoryCache)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
            _cache = memoryCache;
        }

        /// <summary>
        /// Add song to favourite song database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="redirectUrl">Redirect user back to SongContents index page via url</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> AddToFavouritesAsync(int id, string redirectUrl)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            await _db.AddFavSongAsync(id, loggedInUser!.Id);

            TempData["SongAddedToFavourites"] = "Favourite song added";

            return Redirect(redirectUrl);
        }

        public async Task<ActionResult> IndexAsync(int id)
        {
            if (!_cache.TryGetValue(id, out Song? song))
            {
                song = await _db.GetSongByIdAsync(id);

                var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                    Size = 1
                };

                _cache.Set(id, song, memoryCacheEntryOptions);
            }

            return View(song);
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
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            await _emailSender.SendEmailAsync("NOTIFY_SITE_ADMIN",
                                "Wrong song info report",
                                $"Wrong song info reported by user with email: {loggedInUser!.Email} for song ID: {id}" +
                                $"<br>" +
                                $"<br>" +
                                $"<a href={redirectUrl}>Link to song info page</a>");

            TempData["WrongSongInfoReported"] = "Wrong song info reported";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Remove song from favourite song database
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="redirectUrl">Redirect user back to SongContents index page via url</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> RemoveFromFavouritesAsync(int id, string redirectUrl)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            await _db.RemoveFavSongAsync(id, loggedInUser!.Id);

            TempData["SongRemovedFromFavourites"] = "Favourite song removed";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Edit song information and manually input lyrics
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> UpdateWrongSongInfoAsync(int id)
        {
            return View(await _db.GetSongByIdAsync(id));
        }

        /// <summary>
        /// Edit song information and manually input lyrics
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWrongSongInfoAsync([FromForm] Song song)
        {
            var loggedInUser = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            var updatedSong = new Song()
            {
                Id = song.Id,
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = DateTime.Now,
                DeezerId = song.DeezerId,
                SongDuration = song.SongDuration,
                ArtistArtLink = song.ArtistArtLink,
                AlbumArtLink = song.AlbumArtLink,
                Lyrics = song.Lyrics,
                LyricsSet = true,
                CreatedBy = song.CreatedBy,
                EditedBy = loggedInUser!.Id
            };

            await _db.UpdateSongAsync(updatedSong);

            _cache.Remove(song.Id);

            return View("Index", updatedSong);
        }
    }
}