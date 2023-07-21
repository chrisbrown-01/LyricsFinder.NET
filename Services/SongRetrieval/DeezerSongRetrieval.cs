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
                DeezerId = songInfo.id,
                SongDuration = songInfo.duration,
                ArtistArtLink = songInfo.artist.picture_xl, // TODO: safe to place image links in page?
                AlbumArtLink = songInfo.album.cover_xl,
                Lyrics = lyrics,
                LyricsSet = true,
                CreatedBy = song.CreatedBy,
                EditedBy = song.EditedBy
            };
        }

        private async Task<SongContents> GetDeezerSongInfoAsync(string title, string artist) // TODO: check for null and return null?
        {
            // TODO: change to name/artist inputs
            // https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'
            // https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'&limit=1

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync("https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'&limit=1");
            var responseObject = JsonSerializer.Deserialize<DeezerApiSongContents.Rootobject>(response);
            return responseObject.data[0];
        }

        private async Task<string> GetLyricsAsync(string title, string artist) // TODO: implement string interpolation
        {
            var errorResponse = "Error - lyrics could not be found.";
            string pattern = "https://songmeanings.com/songs/view/\\d+";
            var httpClient = _httpClientFactory.CreateClient();

            //var response = await httpClient.GetStringAsync("https://songmeanings.com/query/?query=master of puppets+metallica&type=all");
            //var response = await httpClient.GetStringAsync("https://songmeanings.com/query/?query=born in the usa bruce springsteen&type=all");
            //var response = await httpClient.GetStringAsync("https://songmeanings.com/query/?query=holy wars megadeth&type=all");
            var songQueryResponse = await httpClient.GetStringAsync("https://songmeanings.com/query/?query=walk pantera&type=all");
            //var response = await httpClient.GetStringAsync("https://songmeanings.com/query/?query=asfsdswtr34 dsdfsdf9ws&type=all");       

            var matches = Regex.Matches(songQueryResponse, pattern).FirstOrDefault();

            if (matches is null) return errorResponse;

            var songLink = matches.ToString();

            var songLinkResponse = await httpClient.GetAsync(songLink);

            if (songLinkResponse.StatusCode != System.Net.HttpStatusCode.MovedPermanently) return errorResponse;

            var songLyricsResponse = await httpClient.GetStringAsync(songLinkResponse.Headers.Location);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(songLyricsResponse);

            var lyricBox = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'holder') and contains(@class, 'lyric-box')]");

            if (lyricBox is null) return errorResponse;

            string lyrics = lyricBox.InnerText.TrimStart();
            lyrics = lyrics.Replace("Edit Lyrics", "");
            lyrics = lyrics.TrimEnd();

            return lyrics;
        }
    }
}
