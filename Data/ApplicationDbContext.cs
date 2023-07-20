using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace LyricsFinder.NET.Data
{
    // ***Secrets/key vaults should be used in practice but will simply hard-code credentials for demonstration.***
    public class ApplicationDbContext : IdentityDbContext<CustomAppUserData>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Song> SongDatabase { get; set; }

        public DbSet<UserFavouriteSongs> UserFavouriteSongs { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // source: https://www.c-sharpcorner.com/article/seed-data-in-net-core-identity/
            this.SeedRoles(builder);
            this.SeedAdmin(builder);
            this.SeedModerator(builder);
        }

        private const string AdminRoleId = "fab4fac1-c546-41de-aebc-a14da6895711";
        private const string ModeratorRoleId = "c7b013f0-5201-4317-abd8-c211f91b7330";
        private const string BasicRoleId = "c7d013f0-5201-4317-abd8-c846f91b9946";
        
        private const string AdminUserId = "aab9c560-3441-40d1-b479-9bb75990ac08";
        private const string ModeratorUserId = "00023983-9f16-4a6c-91b8-940283954fc6";

        private void SeedAdmin(ModelBuilder builder)
        {
            // Create default admin user
            CustomAppUserData admin = new CustomAppUserData()
            {
                Id = AdminUserId,
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                NormalizedEmail = "admin@admin.com".ToUpper(),
                NormalizedUserName = "admin@admin.com".ToUpper(),
                LockoutEnabled = false,
                Name = "Admin",
                DOB = DateTime.Today,
                IsAdmin = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ProfilePicture = null
            };

            PasswordHasher<CustomAppUserData> passwordHasher = new PasswordHasher<CustomAppUserData>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

            // Seed admin in Users table
            builder.Entity<CustomAppUserData>().HasData(admin);

            // Seed admin in User Roles table
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = AdminRoleId, UserId = admin.Id }
                );
        }

        private void SeedModerator(ModelBuilder builder)
        {
            // create default moderator
            CustomAppUserData moderator = new CustomAppUserData()
            {
                Id = ModeratorUserId,
                UserName = "moderator@mod.com",
                Email = "moderator@mod.com",
                NormalizedEmail = "moderator@mod.com".ToUpper(),
                NormalizedUserName = "moderator@mod.com".ToUpper(),
                LockoutEnabled = false,
                Name = "Moderator",
                DOB = DateTime.Today,
                IsAdmin = false,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ProfilePicture = null
            };


            PasswordHasher<CustomAppUserData> passwordHasher = new PasswordHasher<CustomAppUserData>();
            moderator.PasswordHash = passwordHasher.HashPassword(moderator, "Moderator123!");

            // seed moderator in Users table
            builder.Entity<CustomAppUserData>().HasData(moderator);

            // seed moderator in User Roles table
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = ModeratorRoleId, UserId = moderator.Id }
                );
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = AdminRoleId, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() { Id = ModeratorRoleId, Name = "Moderator", ConcurrencyStamp = "2", NormalizedName = "MODERATOR" },
                new IdentityRole() { Id = BasicRoleId, Name = "Basic", ConcurrencyStamp = "3", NormalizedName = "BASIC" }
                );
        }
    }
}