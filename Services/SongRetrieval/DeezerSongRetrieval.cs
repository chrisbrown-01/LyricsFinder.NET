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

        public async Task<Song> RetrieveSongContentsAsync(Song song)
        {
            var getSongInfoTask = GetDeezerSongInfoAsync(song.Name, song.Artist);
            var getLyricsTask = GetLyricsAsync(song.Name, song.Artist);

            await Task.WhenAll(getSongInfoTask, getLyricsTask);

            var songInfo = getSongInfoTask.Result;
            var lyrics = getLyricsTask.Result;

            return new Song()
            {
                Id = song.Id,
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = song.QueryDate,
                DeezerId = songInfo.Id,
                SongDuration = songInfo.SongDuration,
                ArtistArtLink = songInfo.ArtistArtLink,
                AlbumArtLink = songInfo.AlbumArtLink,
                Lyrics = lyrics,
                LyricsSet = true,
                CreatedBy = song.CreatedBy,
                EditedBy = song.EditedBy
            };
        }

        private async Task<DeezerSongInfo> GetDeezerSongInfoAsync(string songName, string artist)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetStringAsync($"https://api.deezer.com/search?q=artist:'{artist}' track:'{songName}'&limit=1");
                var responseObject = JsonSerializer.Deserialize<DeezerApiSongContents.DeezerApiResponse>(response);

                if (responseObject?.data == null || responseObject.data.Length == 0) throw new Exception("Deezer API did not return any data.");

                var responseData = responseObject.data[0];

                return new DeezerSongInfo()
                {
                    Id = responseData.id,
                    SongDuration = responseData.duration,
                    ArtistArtLink = responseData!.artist!.picture_xl,
                    AlbumArtLink = responseData!.album!.cover_xl
                };
            }
            catch
            {
                return new DeezerSongInfo();
            }
        }

        private async Task<string> GetLyricsAsync(string songName, string artist)
        {
            string pattern = "https://songmeanings.com/songs/view/\\d+";
            using var httpClient = _httpClientFactory.CreateClient();

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