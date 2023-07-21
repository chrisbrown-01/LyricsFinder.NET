using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Models
{
    public class UserFavouriteSongs
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public int SongId { get; set; }
    }
}
