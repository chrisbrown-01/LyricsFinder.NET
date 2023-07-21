using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LyricsFinder.NET.Validators;

namespace LyricsFinder.NET.Models
{
    // TODO: formerly SpotifyUserInput
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // TODO: how to handle non-null validity?

        [Required]
        public string Artist { get; set; }

        [DisplayName("Query Date")]
        public DateTime QueryDate { get; set; }

        [DisplayName("Deezer Id")]
        public int? DeezerId { get; set; }

        [DisplayName("Song Duration (seconds)")]
        public int? SongDuration { get; set; }

        [Url]
        [ValidateImageFile]
        [DisplayName("Artist Art")]
        public string? ArtistArtLink { get; set; }

        [Url]
        [ValidateImageFile]
        [DisplayName("Album Art")]
        public string? AlbumArtLink { get; set; }

        public string? Lyrics { get; set; }

        public bool? LyricsSet { get; set; }

        public string Slug => Name?.Replace(' ', '-').ToLower() + '-' + Artist?.Replace(' ', '-').ToLower();

        [DisplayName("Created By (UserId)")]
        public string? CreatedBy { get; set; }

        [DisplayName("Edited By (UserId)")]
        public string? EditedBy { get; set; }
    }
}
