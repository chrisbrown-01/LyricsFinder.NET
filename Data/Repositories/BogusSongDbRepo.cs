using Bogus;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data.Repositories
{
    public class BogusSongDbRepo : ISongDbRepo
    {
        // TODO: lightshot backups list
        private readonly Faker<Song> _songFaker;
        private readonly List<Song> _songsTable;
        private readonly List<UserFavouriteSongs> _favouritesTable;
        private static int _favouritesTableIdCounter;

        public BogusSongDbRepo()
        {
            _songFaker = new Faker<Song>()
                .RuleFor(s => s.Id, (f, s) => f.IndexFaker + 1)
                .RuleFor(s => s.Name, f => f.Random.Word())
                .RuleFor(s => s.Artist, f => f.Name.FullName())
                .RuleFor(s => s.QueryDate, f => f.Date.Recent())
                .RuleFor(s => s.DeezerId, f => f.Random.Int())
                .RuleFor(s => s.SongDuration, f => f.Random.Int(60, 300))
                .RuleFor(s => s.ArtistArtLink, f => f.Image.PicsumUrl())
                .RuleFor(s => s.AlbumArtLink, f => f.Image.PicsumUrl())
                .RuleFor(s => s.Lyrics, f => string.Join("\n", f.Lorem.Paragraphs()))
                .RuleFor(s => s.LyricsSet, f => f.Random.Bool())
                .RuleFor(s => s.CreatedBy, f => f.Random.Uuid().ToString())
                .RuleFor(s => s.EditedBy, f => f.Random.Uuid().ToString());

            _songsTable = _songFaker.Generate(16);

            _favouritesTable = new();

            _favouritesTableIdCounter = 0;
        }

        public bool IsSongDuplicate(Song song)
        {
            if (_songsTable.Any(s => s.Name == song.Name && s.Artist == song.Artist)) return true;
            return false;
        }

        public async Task AddSongToDb(Song song)
        {
            song.Id = _songsTable.Count() + 1;
            _songsTable.Add(song);
            await Task.CompletedTask;
        }

        public async Task DeleteSongFromDb(Song song)
        {
            _songsTable.Remove(song);
            await Task.CompletedTask;
        }

        // TODO: rename to GetAllSongs()
        public IEnumerable<Song> GetAllSongsInDb()
        {
            return _songsTable;
        }

        // TODO: rename to remove db in method name, consolidate with API method
        public Task<Song?> GetDbSongByIdAsync(int songId)
        {
            var song = _songsTable.Where(s => s.Id == songId).FirstOrDefault();
            return Task.FromResult(song);
        }

        public async Task UpdateSongInDb(Song song)
        {
            var songIndex = _songsTable.FindIndex(s => s.Id == song.Id);
            _songsTable[songIndex] = song;
            await Task.CompletedTask;
        }

        public IEnumerable<UserFavouriteSongs> GetAllFavouriteSongs()
        {
            return _favouritesTable;
        }

        // TODO: perform empty check inside repo methods and return null if they are empty
        public IEnumerable<UserFavouriteSongs> GetUserFavouriteSongsIds(CustomAppUserData loggedInUser)
        {
            // return _db.UserFavouriteSongs.Where(x => x.UserId == loggedInUser.Id);

            return _favouritesTable.Where(x => x.UserId == loggedInUser.Id);
        }

        public void AddFavSongToDb(UserFavouriteSongs favSong) // TODO: change to async
        {
            favSong.Id = _favouritesTable.Count() + 1;
            _favouritesTable.Add(favSong);
            //await Task.CompletedTask;
        }

        public async Task AddFavSongToDb(int songId, string userId) // TODO: rename
        {
            if (_favouritesTable.Any(s => s.SongId == songId && s.UserId == userId)) return;

            var favSong = new UserFavouriteSongs()
            {
                Id = ++_favouritesTableIdCounter,
                UserId = userId,
                SongId = songId
            };

            _favouritesTable.Add(favSong);
            await Task.CompletedTask;
        }

        public async Task RemoveFavSongFromDb(int songId, string userId) // TODO: rename to removeSongFromFavs
        {
            _favouritesTable.RemoveAll(x => x.SongId == songId && x.UserId == userId);
            await Task.CompletedTask;
        }

        public Song? GetSongById(int songId)
        {
            return _songsTable.FirstOrDefault(s => s.Id == songId);
        }

        public IEnumerable<Song> GetSongsByName(string songName)
        {
            return _songsTable.Where(s => s.Name == songName);
        }

        public IEnumerable<Song> GetSongsByArtist(string artistName)
        {
            return _songsTable.Where(s => s.Artist == artistName);
        }

        public IEnumerable<Song> GetSongsBySongNameArtist(string songName, string artistName)
        {
            return _songsTable.Where(s => s.Name == songName && s.Artist == artistName);
        }

        public IEnumerable<Song> GetUserFavSongs(string userId)
        {
            var userFavSongIds = _favouritesTable.Where(u => u.UserId == userId).Select(s => s.SongId);

            return _songsTable.Where(s => userFavSongIds.Contains(s.Id));
        }

        public void RemoveFavSongFromDb(UserFavouriteSongs obj)
        {
            throw new NotImplementedException();
        }
    }
}
