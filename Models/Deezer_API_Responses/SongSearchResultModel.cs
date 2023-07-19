namespace LyricsFinder.NET.Models.Deezer_API_Responses
{
    public class SongSearchResultModel
    {
        public int? id { get; set; }
        public bool? readable { get; set; }
        public string? title { get; set; }
        public string? title_short { get; set; }
        public string? link { get; set; }
        public int? duration { get; set; }
        public int? rank { get; set; }
        public bool? explicit_lyrics { get; set; }
        public int? explicit_content_lyrics { get; set; } // TODO: change to boolean?
        public int? explicit_content_cover { get; set; }
        public string? preview { get; set; }
        public string? md5_image { get; set; }
        public Artist? artist { get; set; }
        public Album? album { get; set; }
        public string? type { get; set; }
        public string? title_version { get; set; }
    }
}
