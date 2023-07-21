using LyricsFinder.NET.Models;

namespace LyricsFinder.NET.Services.SongRetrieval
{
    public interface ISongRetrieval
    {
        Task<Song> RetrieveSongContentsAsync(Song song);
    }
}