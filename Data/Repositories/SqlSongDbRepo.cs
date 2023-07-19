//using LyricsFinder.NET.Models;
//using Microsoft.EntityFrameworkCore;

//namespace LyricsFinder.NET.Data.Repositories
//{

        // TODO: add SaveChangesToDbAsync() call in each db call method
//    public class SqlSongDbRepo : ISongDbRepo
//    {
//        private readonly ApplicationDbContext _db;

//        public SqlSongDbRepo(ApplicationDbContext db)
//        {
//            _db = db;
//        }

//        //public void AddFavSongToDb(UserFavouriteSongs obj)
//        //{
//        //    _db.UserFavouriteSongs.Add(obj);
//        //}

//        public void AddSongToDb(Song obj)
//        {
//            _db.SongDatabase.Add(obj);
//        }

//        public void DeleteSongFromDb(Song obj)
//        {
//            _db.SongDatabase.Remove(obj);
//        }

//        //public IEnumerable<UserFavouriteSongs> GetAllFavouriteSongs()
//        //{
//        //    return _db.UserFavouriteSongs;
//        //}

//        public IEnumerable<Song> GetAllSongsInDb()
//        {
//            return _db.SongDatabase;
//        }

//        public async Task<Song> GetDbSongByIdAsync(int id)
//        {
//            return await _db.SongDatabase.FindAsync(id);
//        }

//        //public DbSet<UserFavouriteSongs> GetFavouriteSongDb()
//        //{
//        //    return _db.UserFavouriteSongs;
//        //}

//        public Song GetSongById(int id)
//        {
//            return _db.SongDatabase.FirstOrDefault(s => s.Id == id);
//        }

//        public DbSet<Song> GetSongDb()
//        {
//            return _db.SongDatabase;
//        }

//        public IEnumerable<Song> GetSongsByArtist(string artistName)
//        {
//            return _db.SongDatabase.Where(s => s.Artist == artistName);
//        }

//        public IEnumerable<Song> GetSongsByName(string songName)
//        {
//            return _db.SongDatabase.Where(s => s.Name == songName);
//        }

//        public IEnumerable<Song> GetSongsBySongNameArtist(string songName, string artistName)
//        {
//            return _db.SongDatabase.Where(s => s.Name == songName && s.Artist == artistName);
//        }

//        //public IEnumerable<UserFavouriteSongs> GetUserFavouriteSongs(CustomAppUserData loggedInUser)
//        //{
//        //    return _db.UserFavouriteSongs.Where(x => x.UserId == loggedInUser.Id);
//        //}

//        //public void RemoveFavSongFromDb(UserFavouriteSongs obj)
//        //{
//        //    _db.UserFavouriteSongs.Remove(obj);
//        //}

//        public async Task SaveChangesToDbAsync()
//        {
//            await _db.SaveChangesAsync();
//        }

//        // TODO: investigate tracking requirements for DB
//        public void UpdateSongInDb(Song obj)
//        {
//            _db.SongDatabase.Update(obj);
//        }
//    }
//}
