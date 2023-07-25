using LyricsFinder.NET.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models
{
    // In future, should have constructor and Id set property as private
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Artist { get; set; }

        [Url]
        [ValidateImageFile]
        [DisplayName("Album Art")]
        public string? AlbumArtLink { get; set; }

        [Url]
        [ValidateImageFile]
        [DisplayName("Artist Art")]
        public string? ArtistArtLink { get; set; }

        [DisplayName("Created By (UserId)")]
        public string? CreatedBy { get; set; }

        [DisplayName("Deezer Id")]
        public int? DeezerId { get; set; }

        [DisplayName("Edited By (UserId)")]
        public string? EditedBy { get; set; }

        public string? Lyrics { get; set; }

        public bool? LyricsSet { get; set; }

        [DisplayName("Query Date")]
        public DateTime QueryDate { get; set; }

        public string Slug => Name?.Replace(' ', '-').ToLower() + '-' + Artist?.Replace(' ', '-').ToLower();

        [DisplayName("Song Duration (seconds)")]
        public int? SongDuration { get; set; }
    }
}