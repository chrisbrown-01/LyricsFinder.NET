namespace LyricsFinder.NET.Areas.Identity.Models
{
    public class UserRolesView
    {
        public DateTime DOB { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public string? UserName { get; set; }
    }
}