using HtmlAgilityPack;
using LyricsFinder.NET.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using static LyricsFinder.NET.Models.DeezerApiSongContents;

namespace LyricsFinder.NET.Services.SongRetrieval
{
    public class DeezerSongRetrieval : ISongRetrieval
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DeezerSongRetrieval(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Song> RetrieveSongContentsAsync(Song song) // TODO: check if any other methods are directly modifying the object that was passed in
        {
            var getSongInfoTask = GetDeezerSongInfoAsync(song.Name, song.Artist);
            var getLyricsTask = GetLyricsAsync(song.Name, song.Artist);

            var songInfo = await getSongInfoTask; // TODO: change to async task upon all
            var lyrics = await getLyricsTask;

            return new Song()
            {
                Id = song.Id, // TODO: is this even passed in? how will this cope with EF Core
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = song.QueryDate,
                DeezerId = songInfo.Id,
                SongDuration = songInfo.SongDuration,
                ArtistArtLink = songInfo.ArtistArtLink, // TODO: safe to place image links in page?
                AlbumArtLink = songInfo.AlbumArtLink,
                Lyrics = lyrics,
                LyricsSet = true,
                CreatedBy = song.CreatedBy,
                EditedBy = song.EditedBy
            };
        }

        private async Task<DeezerSongInfo> GetDeezerSongInfoAsync(string songName, string artist) // TODO: check for null and return null?
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetStringAsync($"https://api.deezer.com/search?q=artist:'{artist}' track:'{songName}'&limit=1");
                var responseObject = JsonSerializer.Deserialize<DeezerApiSongContents.DeezerApiResponse>(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var responseData = responseObject.data[0];
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return new DeezerSongInfo()
                {
                    Id = responseData.id,
                    SongDuration = responseData.duration,
                    ArtistArtLink = responseData!.artist!.picture_xl,
                    AlbumArtLink = responseData!.album!.cover_xl
                };
            }
            catch  // TODO: better null handling from chatgpt?
            {
                return new DeezerSongInfo();
            }
        }

        private async Task<string> GetLyricsAsync(string songName, string artist) // TODO: implement string interpolation
        {
            string pattern = "https://songmeanings.com/songs/view/\\d+";
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var songQueryResponse = await httpClient.GetStringAsync($"https://songmeanings.com/query/?query={songName} {artist}&type=all");

                var matches = Regex.Matches(songQueryResponse, pattern).First();

                var songLink = matches.ToString();

                var songLinkResponse = await httpClient.GetAsync(songLink);

                // if (songLinkResponse.StatusCode != System.Net.HttpStatusCode.MovedPermanently) return errorResponse; // songmeanings.com always forced a redirect to the actual song link

                var songLyricsResponse = await httpClient.GetStringAsync(songLinkResponse.Headers.Location); 

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(songLyricsResponse);

                var lyricBox = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'holder') and contains(@class, 'lyric-box')]");

                string lyrics = lyricBox.InnerText.TrimStart();
                lyrics = lyrics.Replace("Edit Lyrics", "");
                lyrics = lyrics.TrimEnd();

                return lyrics;
            }
            catch
            {
                return "Error - lyrics could not be found."; 
            }
        }
    }
}
