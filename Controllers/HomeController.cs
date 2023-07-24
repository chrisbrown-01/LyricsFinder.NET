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
            return 1;
            // TODO: seed favourited songs in test database
            //int mostPopularSongId;

            //if (!_cache.TryGetValue("mostPopularSongId", out mostPopularSongId))
            //{
            //    mostPopularSongId = _db.GetAllFavouriteSongs()
            //                           .GroupBy(i => i.SongId)
            //                           .OrderByDescending(grp => grp.Count())
            //                           .Select(grp => grp.Key)
            //                           .First();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            //        Size = 1
            //    };

            //    _cache.Set("mostPopularSongId", mostPopularSongId, cacheEntryOptions);

            //    return mostPopularSongId;
            //}

            //return mostPopularSongId;
        }

        /// <summary>
        /// Selects and returns random song database id
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetRandomSongAsync()
        {
            var songList = await _db.GetAllSongsAsync();
            var songListSize = songList.Count();
            return Random.Shared.Next(1, songListSize);
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.randomSongId = await GetRandomSongAsync();

            ViewBag.mostPopularSongId = await GetMostPopularSong();

            return View();
        }
    }
}