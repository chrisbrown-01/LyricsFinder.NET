using LyricsFinder.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data.Repositories
{
    public class SqlSongDbRepo : ISongDbRepo
    {
        private readonly ApplicationDbContext _db;

        public SqlSongDbRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddFavSongAsync(int songId, string userId)
        {
            if (await _db.FavouritedSongs.AnyAsync(s => s.SongId == songId && s.UserId == userId)) return;

            var favSong = new UserFavouriteSongs()
            {
                UserId = userId,
                SongId = songId
            };

            await _db.FavouritedSongs.AddAsync(favSong);
            await _db.SaveChangesAsync();
        }

        public async Task AddSongAsync(Song song)
        {
            await _db.Songs.AddAsync(song);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSongAsync(Song song)
        {
            _db.Songs.Remove(song);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            var songs = await _db.Songs.AsNoTracking().ToListAsync();
            return songs;
        }

        public async Task<IEnumerable<Song>> GetFavSongsAsync(string userId)
        {
            var favSongs = await _db.FavouritedSongs
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Join(_db.Songs.AsNoTracking(), fav => fav.SongId, song => song.Id, (fav, song) => song)
                .ToListAsync();

            return favSongs;
        }

        //public async Task<Song?> GetSongByIdAsync(int id)
        //{
        // Note this includes change tracking and will cause exceptions if a new
        // Song object is created with the same Id (ie. anything with a Key attribute)
        //    return await _db.Songs.FindAsync(id);
        //}

        public async Task<Song?> GetSongByIdAsync(int id)
        {
            return await _db.Songs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Song>> GetSongsByArtistAsync(string artistName)
        {
            return await _db.Songs
                .AsNoTracking()
                .Where(s => s.Artist.ToLower() == artistName.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Song>> GetSongsByNameAsync(string songName)
        {
            return await _db.Songs
                .AsNoTracking()
                .Where(s =>
                s.Name.ToLower() == songName.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Song>> GetSongsBySongNameArtistAsync(string songName, string artistName)
        {
            return await _db.Songs
                .AsNoTracking()
                .Where(s =>
                s.Name.ToLower() == songName.ToLower() &&
                s.Artist.ToLower() == artistName.ToLower())
                .ToListAsync();
        }

        public bool IsSongDuplicate(Song song)
        {
            if (_db.Songs.Any(
                s =>
                s.Name.ToLower() == song.Name.ToLower() &&
                s.Artist.ToLower() == song.Artist.ToLower()))
                return true;

            return false;
        }

        public async Task RemoveFavSongAsync(int songId, string userId)
        {
            var favSongs = await _db.FavouritedSongs
                .Where(x => x.SongId == songId && x.UserId == userId)
                .ToListAsync();

            _db.FavouritedSongs.RemoveRange(favSongs);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateSongAsync(Song song)
        {
            _db.Songs.Update(song);
            await _db.SaveChangesAsync();
        }
    }
}