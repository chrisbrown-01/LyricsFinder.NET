namespace LyricsFinder.NET.Models
{
    public class DeezerApiSongContents
    {
        public class DeezerApiResponse
        {
            public AllDeezerSongInfo[]? data { get; set; }
            public int? total { get; set; }
            public string? next { get; set; }
        }

        public class DeezerSongInfo
        {
            public int? Id { get; set; }
            public int? SongDuration { get; set; }
            public string? ArtistArtLink { get; set; } // TODO: safe to place image links in page?
            public string? AlbumArtLink { get; set; }

        }

        public class AllDeezerSongInfo
        {
            public int? id { get; set; }
            public bool? readable { get; set; }
            public string? title { get; set; }
            public string? title_short { get; set; }
            public string? title_version { get; set; }
            public string? link { get; set; }
            public int? duration { get; set; }
            public int? rank { get; set; }
            public bool? explicit_lyrics { get; set; }
            public int? explicit_content_lyrics { get; set; }
            public int? explicit_content_cover { get; set; }
            public string? preview { get; set; }
            public string? md5_image { get; set; }
            public DeezerArtist? artist { get; set; }
            public DeezerAlbum? album { get; set; }
            public string? type { get; set; }
        }

        public class DeezerArtist
        {
            public int? id { get; set; }
            public string? name { get; set; }
            public string? link { get; set; }
            public string? picture { get; set; }
            public string? picture_small { get; set; }
            public string? picture_medium { get; set; }
            public string? picture_big { get; set; }
            public string? picture_xl { get; set; }
            public string? tracklist { get; set; }
            public string? type { get; set; }
        }

        public class DeezerAlbum
        {
            public int? id { get; set; }
            public string? title { get; set; }
            public string? cover { get; set; }
            public string? cover_small { get; set; }
            public string? cover_medium { get; set; }
            public string? cover_big { get; set; }
            public string? cover_xl { get; set; }
            public string? md5_image { get; set; }
            public string? tracklist { get; set; }
            public string? type { get; set; }
        }

    }
}
