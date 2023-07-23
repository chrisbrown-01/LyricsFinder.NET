using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace LyricsFinder.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISongDbRepo _db;
        private readonly IMemoryCache _cache;

        public HomeController(ISongDbRepo db,
                              IMemoryCache memoryCache)
        {
            _db = db;
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            ViewBag.randomSongId = GetRandomSong();

            ViewBag.mostPopularSongId = GetMostPopularSong();

            return View();
        }

        /// <summary>
        /// Selects and returns random song database id
        /// </summary>
        /// <returns></returns>
        public int GetRandomSong()
        {
            return Random.Shared.Next(1, _db.GetAllSongsInDb().Count());
        }

        /// <summary>
        /// Queries favourited song database and returns song id of the most favourited song
        /// </summary>
        /// <returns></returns>
        public int GetMostPopularSong()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}