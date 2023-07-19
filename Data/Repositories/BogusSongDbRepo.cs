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
        private readonly List<Song> _db;

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

            _db = _songFaker.Generate(16);
        }

        public bool IsSongDuplicate(Song song)
        {
            if (_db.Any(s => s.Name == song.Name && s.Artist == song.Artist)) return true;
            return false;
        }

        public async Task AddSongToDb(Song song)
        {
            song.Id = _db.Count() + 1;
            _db.Add(song);
            await Task.CompletedTask;
        }

        public async Task DeleteSongFromDb(Song song)
        {
            _db.Remove(song);
            await Task.CompletedTask;
        }

        public IEnumerable<Song> GetAllSongsInDb()
        {
            return _db;
        }

        public Task<Song?> GetDbSongByIdAsync(int id)
        {
            var song = _db.Where(s => s.Id == id).FirstOrDefault();
            return Task.FromResult(song);
        }

        public DbSet<Song> GetSongDb()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesToDbAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSongInDb(Song song)
        {
            var songIndex = _db.FindIndex(s => s.Id == song.Id);
            _db[songIndex] = song;
            await Task.CompletedTask;
        }

        public IEnumerable<UserFavouriteSongs> GetAllFavouriteSongs()
        {
            throw new NotImplementedException();
        }

        public DbSet<UserFavouriteSongs> GetFavouriteSongDb()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserFavouriteSongs> GetUserFavouriteSongs(CustomAppUserData loggedInUser)
        {
            throw new NotImplementedException();
        }

        public void AddFavSongToDb(UserFavouriteSongs obj)
        {
            throw new NotImplementedException();
        }

        public void RemoveFavSongFromDb(UserFavouriteSongs obj)
        {
            throw new NotImplementedException();
        }

        public Song GetSongById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Song> GetSongsByName(string songName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Song> GetSongsByArtist(string artistName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Song> GetSongsBySongNameArtist(string songName, string artistName)
        {
            throw new NotImplementedException();
        }
    }
}
