using LyricsFinder.NET.Validators;
using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models.DTOs
{
    public class SongUpdateDTO
    {
        public int? DeezerId { get; set; }

        public int? SongDuration { get; set; }

        [Url]
        [ValidateImageFile]
        [MaxLength(400, ErrorMessage = "Max input length is 400 characters.")]
        public string? ArtistArtLink { get; set; }

        [Url]
        [ValidateImageFile]
        [MaxLength(400, ErrorMessage = "Max input length is 400 characters.")]
        public string? AlbumArtLink { get; set; }

        [MaxLength(8000, ErrorMessage = "Max input length is 8000 characters.")]
        public string? Lyrics { get; set; }
    }
}