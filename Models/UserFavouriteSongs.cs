using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models
{
    public class UserFavouriteSongs
    {
        [Key]
        public int Id { get; set; }

        public required int SongId { get; set; }
        public required string UserId { get; set; }
    }
}