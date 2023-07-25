using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace LyricsFinder.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly ISongDbRepo _db;

        public HomeController(ISongDbRepo db,
                              IMemoryCache memoryCache)
        {
            _db = db;
            _cache = memoryCache;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Queries favourited song database and returns song id of the most favourited song
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMostPopularSong()
        {
            int mostPopularSongId;

            if (!_cache.TryGetValue("mostPopularSongId", out mostPopularSongId))
            {
                var favSongList = await _db.GetAllFavSongsAsync();

                if (favSongList.Count() < 1) return 0;

                mostPopularSongId = favSongList.GroupBy(song => song.SongId)
                           .OrderByDescending(group => group.Count())
                           .First().Key;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    Size = 1
                };

                _cache.Set("mostPopularSongId", mostPopularSongId, cacheEntryOptions);

                return mostPopularSongId;
            }

            return mostPopularSongId;
        }

        /// <summary>
        /// Selects and returns random song database id
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetRandomSongAsync()
        {
            var songList = await _db.GetAllSongsAsync();
            if (songList.Count() < 1) return 0;
            return Random.Shared.Next(1, songList.Count());
        }

        public async Task<IActionResult> IndexAsync()
        {
            // Not a very good implementation but I want to keep this to demonstrate ViewBag
            ViewBag.randomSongId = await GetRandomSongAsync();

            ViewBag.mostPopularSongId = await GetMostPopularSong();

            return View();
        }
    }
}