using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data.Repositories
{
    public interface ISongDbRepo
    {
        Task<IEnumerable<Song>> GetAllSongsAsync();
        Task<IEnumerable<Song>> GetFavSongsAsync(string userId);
        Task AddSongAsync(Song song); 

        Task UpdateSongAsync(Song song);

        Task DeleteSongAsync(Song song);

        Task<Song?> GetSongByIdAsync(int id);

        bool IsSongDuplicate(Song song);

        Task<IEnumerable<Song>> GetSongsByNameAsync(string songName);

        Task<IEnumerable<Song>> GetSongsByArtistAsync(string artistName);

        Task<IEnumerable<Song>> GetSongsBySongNameArtistAsync(string songName, string artistName);
        Task RemoveFavSongAsync(int songId, string userId);
        Task AddFavSongAsync(int songId, string userId);
    }
}
