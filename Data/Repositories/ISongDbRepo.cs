using LyricsFinder.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data.Repositories
{
    public interface ISongDbRepo
    {
        Task SaveChangesToDbAsync();

        IEnumerable<Song> GetAllSongsInDb();
        Task AddSongToDb(Song song); // TODO: rename to async

        Task UpdateSongInDb(Song song);

        Task DeleteSongFromDb(Song song);

        Task<Song?> GetDbSongByIdAsync(int id);



        // UserFavouriteSongs table methods
        //IEnumerable<UserFavouriteSongs> GetAllFavouriteSongs(); // TODO: convert to async

        //DbSet<UserFavouriteSongs> GetFavouriteSongDb();

        //IEnumerable<UserFavouriteSongs> GetUserFavouriteSongs(CustomAppUserData loggedInUser);

        //void AddFavSongToDb(UserFavouriteSongs obj);

        //void RemoveFavSongFromDb(UserFavouriteSongs obj);



        // API methods

        DbSet<Song> GetSongDb();
        bool IsSongDuplicate(Song song);

        //SpotifyUserInput GetSongById(int id);

        //IEnumerable<SpotifyUserInput> GetSongsByName(string songName);

        //IEnumerable<SpotifyUserInput> GetSongsByArtist(string artistName);

        //IEnumerable<SpotifyUserInput> GetSongsBySongNameArtist(string songName, string artistName);
    }
}
