using Bogus;
using LyricsFinder.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data.Repositories
{
    public class BogusSongDbRepo : ISongDbRepo
    {
        private static int _favouritesTableIdCounter;
        private static int _songsTableIdCounter;
        private readonly List<UserFavouriteSongs> _favouritesTable;
        private readonly Faker<Song> _songFaker;
        private readonly List<Song> _songsTable;

        public BogusSongDbRepo()
        {
            _songFaker = new Faker<Song>()
                .RuleFor(s => s.Id, (f, s) => f.IndexFaker + 1)
                .RuleFor(s => s.Name, f => f.Random.Word())
                .RuleFor(s => s.Artist, f => f.Name.FullName())
                .RuleFor(s => s.QueryDate, f => f.Date.Recent())
                .RuleFor(s => s.DeezerId, f => f.Random.Int())
                .RuleFor(s => s.SongDuration, f => f.Random.Int(60, 300))
                //.RuleFor(s => s.ArtistArtLink, f => f.Image.PicsumUrl())
                //.RuleFor(s => s.AlbumArtLink, f => f.Image.PicsumUrl())
                .RuleFor(s => s.ArtistArtLink, f => null)
                .RuleFor(s => s.AlbumArtLink, f => null)
                .RuleFor(s => s.Lyrics, f => string.Join("\n", f.Lorem.Paragraphs()))
                .RuleFor(s => s.LyricsSet, f => f.Random.Bool())
                .RuleFor(s => s.CreatedBy, f => f.Random.Uuid().ToString())
                .RuleFor(s => s.EditedBy, f => f.Random.Uuid().ToString());

            _songsTable = _songFaker.Generate(16);
            _songsTableIdCounter = _songsTable.Count();

            _favouritesTable = new();
            _favouritesTableIdCounter = 0;
        }

        public async Task AddFavSongAsync(int songId, string userId)
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

        public async Task AddSongAsync(Song song)
        {
            song.Id = ++_songsTableIdCounter;
            _songsTable.Add(song);
            await Task.CompletedTask;
        }

        public async Task DeleteSongAsync(Song song)
        {
            _songsTable.Remove(song);
            await Task.CompletedTask;
        }

        public Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return Task.FromResult(_songsTable.AsEnumerable());
        }

        public Task<IEnumerable<Song>> GetFavSongsAsync(string userId)
        {
            var userFavSongIds = _favouritesTable.Where(u => u.UserId == userId).Select(s => s.SongId);

            return Task.FromResult(
                _songsTable.Where(s => userFavSongIds.Contains(s.Id)));
        }

        public Task<Song?> GetSongByIdAsync(int songId)
        {
            var song = _songsTable.FirstOrDefault(s => s.Id == songId);
            return Task.FromResult(song);
        }

        public Task<IEnumerable<Song>> GetSongsByArtistAsync(string artistName)
        {
            return Task.FromResult(
                _songsTable.Where(s => s.Artist.ToLower() == artistName.ToLower()));
        }

        public Task<IEnumerable<Song>> GetSongsByNameAsync(string songName)
        {
            return Task.FromResult(
                _songsTable.Where(s => s.Name.ToLower() == songName.ToLower()));
        }

        public Task<IEnumerable<Song>> GetSongsBySongNameArtistAsync(string songName, string artistName)
        {
            return Task.FromResult(
                _songsTable.Where(s =>
                    s.Name.ToLower() == songName.ToLower() &&
                    s.Artist.ToLower() == artistName.ToLower()));
        }

        public bool IsSongDuplicate(Song song)
        {
            if (_songsTable.Any(
                s =>
                s.Name.ToLower() == song.Name.ToLower() &&
                s.Artist.ToLower() == song.Artist.ToLower()))
                return true;

            return false;
        }

        public async Task RemoveFavSongAsync(int songId, string userId)
        {
            _favouritesTable.RemoveAll(x => x.SongId == songId && x.UserId == userId);
            await Task.CompletedTask;
        }

        public async Task UpdateSongAsync(Song song)
        {
            var songIndex = _songsTable.FindIndex(s => s.Id == song.Id);
            _songsTable[songIndex] = song;
            await Task.CompletedTask;
        }
    }
}