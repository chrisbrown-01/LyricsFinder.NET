using Bogus.DataSets;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Models.Deezer_API_Responses;
using System.Text.Json;
using static LyricsFinder.NET.Models.DeezerApiSongContents;

namespace LyricsFinder.NET.Services
{
    // TODO: rework into DI service
    public static class RetrieveLyricsAndSongInfo
    {
        /// <summary>
        /// Calls Deezer API using song details supplied by user during SongManager Create action
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="artist">Artist name</param>
        /// <returns>Returns first element returned from Deezer API call</returns>
        private static async Task<SongContents> GetDeezerSongInfoAsync(string title, string artist) // TODO: return null, create interface for SongContents and properties?
        {
            // TODO: change to name/artist inputs
            // https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'
            // https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'&limit=1

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync("https://api.deezer.com/search?q=artist:'metallica' track:'master of puppets'&limit=1");
                var responseObject = JsonSerializer.Deserialize<DeezerApiSongContents.Rootobject>(response);
                return responseObject.data[0];
            }

            //var testSong = new SongSearchResultModel()
            //{
            //    id = 1,
            //    readable = true,
            //    title = "Title1",
            //    title_short = title,
            //    link = "Link1",
            //    duration = 100,
            //    rank = 1,
            //    explicit_lyrics = false,
            //    explicit_content_lyrics = 0,
            //    explicit_content_cover = 0,
            //    preview = "Preview",
            //    md5_image = "md5_image",
            //    artist = new Artist()
            //    {
            //        id = 123,
            //        name = "My Name",
            //        link = "https://www.example.com",
            //        share = "https://www.example.com/share",
            //        picture = "https://www.example.com/picture.jpg",
            //        picture_small = "https://www.example.com/picture_small.jpg",
            //        picture_medium = "https://www.example.com/picture_medium.jpg",
            //        picture_big = "https://www.example.com/picture_big.jpg",
            //        picture_xl = "https://www.example.com/picture_xl.jpg",
            //        radio = true,
            //        tracklist = "https://www.example.com/tracklist",
            //        type = "My Type"
            //    },
            //    album = new Album()
            //    {
            //        id = 456,
            //        title = "My Album",
            //        link = "https://www.example.com/album",
            //        cover = "https://www.example.com/cover.jpg",
            //        cover_small = "https://www.example.com/cover_small.jpg",
            //        cover_medium = "https://www.example.com/cover_medium.jpg",
            //        cover_big = "https://www.example.com/cover_big.jpg",
            //        cover_xl = "https://www.example.com/cover_xl.jpg",
            //        md5_image = "d41d8cd98f00b204e9800998ecf8427e",
            //        release_date = "2023-07-18",
            //        tracklist = "https://www.example.com/tracklist",
            //        type = "Album",
            //    },
            //    type = "type",
            //    title_version = "title_version"
            //};

            //return testSong;
        }

        /// <summary>
        /// Uses Chrome Selenium webscraper to retrieve song lyrics from songmeanings.com
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="artist">Artist name</param>
        /// <returns>Lyrics in string format</returns>
        private static string GetLyrics(string title, string artist)
        {
            return "Test lyrics response, change this later.";

            //try
            //{

            //    // TODO: use "using" statement to see if it will close chrome connections upon completion
            //    var chromeOptions = new ChromeOptions();
            //    chromeOptions.AddArguments("headless");

            //    IWebDriver driver = new ChromeDriver(chromeOptions);

            //    driver.Navigate().GoToUrl("http://www.google.com/");

            //    var searchText = driver.FindElement(By.ClassName("gLFyf"));

            //    searchText.SendKeys($"site:songmeanings.com {title} {artist}");
            //    searchText.SendKeys(Keys.Enter);

            //    //IWebElement elements = driver.FindElement(By.PartialLinkText("SongMeanings"));
            //    //elements.Click();

            //    var searchResults = driver.FindElements(By.ClassName("yuRUbf"));
            //    string songMeaningsUrl = searchResults[0].FindElement(By.TagName("a")).GetAttribute("href");

            //    driver.Close();
            //    driver.Quit();

            //    chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
            //    IWebDriver driver2 = new ChromeDriver(chromeOptions);

            //    driver2.Navigate().GoToUrl(songMeaningsUrl);

            //    // seems if the page doesn't have enough time to load, function will randomly incorrectly throw exception
            //    // Thread.Sleep(250);

            //    searchText = driver2.FindElement(By.ClassName("lyric-box"));
            //    //searchText = driver2.FindElement(By.ClassName("songtext"));
            //    string lyrics = searchText.Text;
            //    lyrics = lyrics.Substring(0, lyrics.Length - 15); // trim "Edit Lyrics" appended from website

            //    driver2.Quit();

            //    return lyrics;
            //}
            //catch (Exception)
            //{
            //    return "Error - lyrics could not be found.";
            //}
        }

        // TODO: add in summary info for all new methods also
        /// <summary>
        /// Finds Deezer song information & lyrics
        /// </summary>
        /// <param name="song">Song database object</param>
        /// <returns>Updates song database object with newly found Deezer info and lyrics</returns>
        public static async Task<Song> ScrapeSongInfoFromWebAsync(Song song)
        {       
            var getSongInfo = GetDeezerSongInfoAsync(song.Name, song.Artist);

            // TODO: make async with http client request
            var lyrics = GetLyrics(song.Name, song.Artist);

            song.Lyrics = lyrics;

            var songInfo = await getSongInfo;

            song.DeezerId = songInfo.id;
            song.SongDuration = songInfo.duration;
            song.ArtistArtLink = songInfo.artist.picture_xl;
            song.AlbumArtLink = songInfo.album.cover_xl;

            song.LyricsSet = true;

            return song;
        }
    }
}
