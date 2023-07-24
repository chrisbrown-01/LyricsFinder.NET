using LyricsFinder.NET.Models;

namespace LyricsFinder.NET.Data.Repositories
{
    public interface ISongDbRepo
    {
        Task AddFavSongAsync(int songId, string userId);

        Task AddSongAsync(Song song);

        Task DeleteSongAsync(Song song);

        Task<IEnumerable<Song>> GetAllSongsAsync();

        Task<IEnumerable<Song>> GetFavSongsAsync(string userId);

        Task<Song?> GetSongByIdAsync(int id);

        Task<IEnumerable<Song>> GetSongsByArtistAsync(string artistName);

        Task<IEnumerable<Song>> GetSongsByNameAsync(string songName);

        Task<IEnumerable<Song>> GetSongsBySongNameArtistAsync(string songName, string artistName);

        bool IsSongDuplicate(Song song);

        Task RemoveFavSongAsync(int songId, string userId);

        Task UpdateSongAsync(Song song);
    }
}