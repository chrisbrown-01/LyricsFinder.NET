﻿using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models.DTOs
{
    public class SongEditDTO
    {
        [Required]
        public required int Id { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Max input length is 150 characters.")]
        public required string Name { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Max input length is 150 characters.")]
        public required string Artist { get; set; }
    }
}
