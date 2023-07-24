using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models.DTOs
{
    public class SongCreateOrEditDTO
    {
        [Required]
        [MaxLength(150, ErrorMessage = "Max input length is 150 characters.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Max input length is 150 characters.")]
        public string Artist { get; set; }
    }
}