namespace LyricsFinder.NET.Areas.Identity.Models
{
    public class ManageUserRolesView
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleNormalizedName { get; set; }
        public bool Selected { get; set; }
    }
}