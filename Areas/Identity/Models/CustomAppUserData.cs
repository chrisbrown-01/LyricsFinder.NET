using Microsoft.AspNetCore.Identity;

namespace LyricsFinder.NET.Areas.Identity.Models
{
    public class CustomAppUserData : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }

        public bool IsAdmin { get; set; }

        public byte[]? ProfilePicture { get; set; }
    }
}
