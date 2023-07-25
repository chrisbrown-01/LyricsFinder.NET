using DocumentFormat.OpenXml.Spreadsheet;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Data
{
    // ***Secrets/key vaults should be used in practice but will simply hard-code credentials for demonstration.***
    public class ApplicationDbContext : IdentityDbContext<CustomAppUserData>
    {
        private const string AdminRoleId = "fab4fac1-c546-41de-aebc-a14da6895711";

        private const string AdminUserId = "aab9c560-3441-40d1-b479-9bb75990ac08";

        private const string BasicRoleId = "c7d013f0-5201-4317-abd8-c846f91b9946";

        private const string ModeratorRoleId = "c7b013f0-5201-4317-abd8-c211f91b7330";

        private const string ModeratorUserId = "00023983-9f16-4a6c-91b8-940283954fc6";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }

        public DbSet<UserFavouriteSongs> FavouritedSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // source: https://www.c-sharpcorner.com/article/seed-data-in-net-core-identity/
            SeedRoles(builder);
            SeedAdmin(builder);
            SeedModerator(builder);
            SeedSongs(builder);
            SeedFavouriteSongs(builder);
        }

        private static void SeedSongs(ModelBuilder builder)
        {
            var song1 = new Song()
            {
                Id = 1,
                Name = "Walk",
                Artist = "Pantera",
                AlbumArtLink = "https://e-cdns-images.dzcdn.net/images/cover/ba6c7c231c21930465a4f51a41e3d5a5/1000x1000-000000-80-0-0.jpg",
                ArtistArtLink = "https://e-cdns-images.dzcdn.net/images/artist/ff5cd034a5414120343b40977109c169/1000x1000-000000-80-0-0.jpg",
                CreatedBy = "aab9c560-3441-40d1-b479-9bb75990ac08",
                DeezerId = 662879,
                EditedBy = null,
                Lyrics = "Can't you see I'm easily bothered by persistence?\r\nOne step from lashing out at you\r\nYou want in, to get under my skin and call yourself a friend\r\nI've got more friends like you, what do I do?\r\n\r\nIs there no standard anymore?\r\nWhat it takes, who I am, where I've been, belong\r\nYou can't be something you're not\r\nBe yourself, by yourself, stay away from me\r\nA lesson learned in life\r\nKnown from the dawn of time\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\n\r\nRun your mouth when I'm not around, it's easy to achieve\r\nYou cry to weak friends that sympathize\r\nCan you hear the violins playing your song?\r\nThose same friends tell me your every word\r\n\r\nIs there no standard anymore?\r\nWhat it takes, who I am, where I've been, belong\r\nYou can't be something you're not\r\nBe yourself, by yourself, stay away from me\r\nA lesson learned in life\r\nKnown from the dawn of time\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk, are you talkin' to me?\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\nNo way, punk\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk, are you talkin' to me?\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\nWalk on home, boy",
                LyricsSet = true,
                QueryDate = DateTime.Now,
                SongDuration = 315
            };

            var song2 = new Song()
            {
                Id = 2,
                Name = "Peace Sells",
                Artist = "Megadeth",
                AlbumArtLink = "https://e-cdns-images.dzcdn.net/images/cover/451129645169db3500114c5a15dfc19c/1000x1000-000000-80-0-0.jpg",
                ArtistArtLink = "https://e-cdns-images.dzcdn.net/images/artist/cff12b4ef973a62d886d69bf056941ec/1000x1000-000000-80-0-0.jpg",
                CreatedBy = "aab9c560-3441-40d1-b479-9bb75990ac08",
                DeezerId = 12565751,
                EditedBy = null,
                Lyrics = "What do you mean, \"I don't believe in God\"?\r\nI talk to him everyday\r\nWhat do you mean, \"I don't support your system\"?\r\nI go to court when I have to\r\nWhat do you mean, \"I can't get to work on time\"?\r\nGot nothing better to do\r\nAnd, what do you mean, \"I don't pay my bills\"?\r\nWhy do you think I'm broke? Huh?\r\n\r\nIf there's a new way\r\nOh, I'll be the first in line\r\nBut it better work this time\r\n\r\nWhat do you mean, \"I hurt your feelings\"?\r\nI didn't know you had any feelings\r\nWhat do you mean, \"I ain't kind\"?\r\nJust not your kind\r\n\r\nWhat do you mean, \"I couldn't be the President\r\nOf the United States of America\"?\r\nTell me something, it's still \"We the people\" right?\r\n\r\nIf there's a new way\r\nOh, I'll be the first in line\r\nBut it better work this time\r\n\r\nCan you put a price on peace?\r\n\r\nPeace\r\nPeace sells\r\nPeace\r\nPeace sells\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\n(No, no no no no)\r\n(Peace sells)\r\n\r\n(Peace sells, aah)",
                LyricsSet = true,
                QueryDate = DateTime.Now,
                SongDuration = 244
            };

            var song3 = new Song()
            {
                Id = 3,
                Name = "Poker Face",
                Artist = "Lady Gaga",
                AlbumArtLink = "https://e-cdns-images.dzcdn.net/images/cover/fad7de079aa103d60ec1e2d1582c2281/1000x1000-000000-80-0-0.jpg",
                ArtistArtLink = "https://e-cdns-images.dzcdn.net/images/artist/83110025016968b8882a1c92a3284a6b/1000x1000-000000-80-0-0.jpg",
                CreatedBy = "aab9c560-3441-40d1-b479-9bb75990ac08",
                DeezerId = 734508762,
                EditedBy = null,
                Lyrics = "(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n\r\nI wanna hold 'em like they do in Texas, please\r\nFold 'em, let 'em hit me, raise it, baby, stay with me (I love it)\r\nLove game intuition, play the cards with spades to start\r\nAnd after he's been hooked, I'll play the one that's on his heart\r\n\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\n\r\nI wanna roll with him, a hard pair we will be\r\nA little gamblin' is fun when you're with me (I love it)\r\nRussian roulette is not the same without a gun\r\nAnd baby, when it's love, if it's not rough, it isn't fun (fun)\r\n\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n\r\nI won't tell you that I love you, kiss or hug you\r\n'Cause I'm bluffin' with my muffin\r\nI'm not lyin', I'm just stunnin' with my love-glue-gunnin'\r\nJust like a chick in the casino\r\nTake your bank before I pay you out\r\nI promise this, promise this\r\nCheck this hand 'cause I'm marvelous\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, p-p-poker face\r\nP-p-p-poker face, f-f-fuck her face (she's got me like nobody)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)",
                LyricsSet = true,
                QueryDate = DateTime.Now,
                SongDuration = 237
            };

            builder.Entity<Song>().HasData(song1, song2, song3);
        }

        private static void SeedFavouriteSongs(ModelBuilder builder)
        {
            var favSong = new UserFavouriteSongs()
            {
                Id = 1,
                SongId = 1,
                UserId = AdminUserId
            };

            builder.Entity<UserFavouriteSongs>().HasData(favSong);
        }

        private static void SeedAdmin(ModelBuilder builder)
        {
            // Create default admin user
            var admin = new CustomAppUserData()
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

            var passwordHasher = new PasswordHasher<CustomAppUserData>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

            // Seed admin in Users table
            builder.Entity<CustomAppUserData>().HasData(admin);

            // Seed admin in User Roles table
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = AdminRoleId, UserId = admin.Id }
                );
        }

        private static void SeedModerator(ModelBuilder builder)
        {
            // create default moderator
            var moderator = new CustomAppUserData()
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

            var passwordHasher = new PasswordHasher<CustomAppUserData>();
            moderator.PasswordHash = passwordHasher.HashPassword(moderator, "Moderator123!");

            // seed moderator in Users table
            builder.Entity<CustomAppUserData>().HasData(moderator);

            // seed moderator in User Roles table
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = ModeratorRoleId, UserId = moderator.Id }
                );
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = AdminRoleId, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() { Id = ModeratorRoleId, Name = "Moderator", ConcurrencyStamp = "2", NormalizedName = "MODERATOR" },
                new IdentityRole() { Id = BasicRoleId, Name = "Basic", ConcurrencyStamp = "3", NormalizedName = "BASIC" }
                );
        }
    }
}