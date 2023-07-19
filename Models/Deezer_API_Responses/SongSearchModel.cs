namespace LyricsFinder.NET.Models.Deezer_API_Responses
{
    public class SongSearchModel
    {
        public List<SongSearchResultModel> data { get; set; }
        public int total { get; set; }
        public string next { get; set; }
    }
}
