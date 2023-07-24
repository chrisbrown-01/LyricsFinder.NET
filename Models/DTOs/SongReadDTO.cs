﻿namespace LyricsFinder.NET.Models.DTOs
{
    public class SongReadDTO
    {
        public string? AlbumArtLink { get; set; }
        public required string Artist { get; set; }
        public string? ArtistArtLink { get; set; }
        public int? DeezerId { get; set; }
        public int Id { get; set; }

        public string? Lyrics { get; set; }
        public required string Name { get; set; }
        public DateTime QueryDate { get; set; }
        public int? SongDuration { get; set; }
    }
}