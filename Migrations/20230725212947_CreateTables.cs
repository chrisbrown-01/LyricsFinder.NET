using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LyricsFinder.NET.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavouritedSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouritedSongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumArtLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArtistArtLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeezerId = table.Column<int>(type: "int", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lyrics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyricsSet = table.Column<bool>(type: "bit", nullable: true),
                    QueryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SongDuration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c7b013f0-5201-4317-abd8-c211f91b7330", "2", "Moderator", "MODERATOR" },
                    { "c7d013f0-5201-4317-abd8-c846f91b9946", "3", "Basic", "BASIC" },
                    { "fab4fac1-c546-41de-aebc-a14da6895711", "1", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DOB", "Email", "EmailConfirmed", "IsAdmin", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "00023983-9f16-4a6c-91b8-940283954fc6", 0, "3aaae154-27bb-49ba-9384-5c49b8bfd7ab", new DateTime(2023, 7, 25, 0, 0, 0, 0, DateTimeKind.Local), "moderator@mod.com", true, false, false, null, "Moderator", "MODERATOR@MOD.COM", "MODERATOR@MOD.COM", "AQAAAAIAAYagAAAAEKgfEwOC7Q4sjlXeVtpEn3tSBRr+B5XlU5LkM3Otyl4r6iE0Ki9+XOnT5XvUPZW9Qw==", null, true, null, "10a7065b-45a5-4962-8df5-a3c923b9c853", false, "moderator@mod.com" },
                    { "aab9c560-3441-40d1-b479-9bb75990ac08", 0, "e7ec9014-046a-459d-a2b2-2271fe379b8c", new DateTime(2023, 7, 25, 0, 0, 0, 0, DateTimeKind.Local), "admin@admin.com", true, true, false, null, "Admin", "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEHAljTSknJjrOnE7A+jTr4p4dHE4Tn14G/zYVlrJkxmINNKAbK3sqwPigjwgwMR4Uw==", null, true, null, "2133eee9-16d1-496b-9699-38b1cac80407", false, "admin@admin.com" }
                });

            migrationBuilder.InsertData(
                table: "FavouritedSongs",
                columns: new[] { "Id", "SongId", "UserId" },
                values: new object[] { 1, 1, "aab9c560-3441-40d1-b479-9bb75990ac08" });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumArtLink", "Artist", "ArtistArtLink", "CreatedBy", "DeezerId", "EditedBy", "Lyrics", "LyricsSet", "Name", "QueryDate", "SongDuration" },
                values: new object[,]
                {
                    { 1, "https://e-cdns-images.dzcdn.net/images/cover/ba6c7c231c21930465a4f51a41e3d5a5/1000x1000-000000-80-0-0.jpg", "Pantera", "https://e-cdns-images.dzcdn.net/images/artist/ff5cd034a5414120343b40977109c169/1000x1000-000000-80-0-0.jpg", "aab9c560-3441-40d1-b479-9bb75990ac08", 662879, null, "Can't you see I'm easily bothered by persistence?\r\nOne step from lashing out at you\r\nYou want in, to get under my skin and call yourself a friend\r\nI've got more friends like you, what do I do?\r\n\r\nIs there no standard anymore?\r\nWhat it takes, who I am, where I've been, belong\r\nYou can't be something you're not\r\nBe yourself, by yourself, stay away from me\r\nA lesson learned in life\r\nKnown from the dawn of time\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\n\r\nRun your mouth when I'm not around, it's easy to achieve\r\nYou cry to weak friends that sympathize\r\nCan you hear the violins playing your song?\r\nThose same friends tell me your every word\r\n\r\nIs there no standard anymore?\r\nWhat it takes, who I am, where I've been, belong\r\nYou can't be something you're not\r\nBe yourself, by yourself, stay away from me\r\nA lesson learned in life\r\nKnown from the dawn of time\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk, are you talkin' to me?\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\nNo way, punk\r\n\r\nRespect, walk, what did you say?\r\nRespect, walk, are you talkin' to me?\r\nRespect, walk, what did you say?\r\nRespect, walk\r\nAre you talkin' to me? Are you talkin' to me?\r\nWalk on home, boy", true, "Walk", new DateTime(2023, 7, 25, 17, 29, 47, 301, DateTimeKind.Local).AddTicks(836), 315 },
                    { 2, "https://e-cdns-images.dzcdn.net/images/cover/451129645169db3500114c5a15dfc19c/1000x1000-000000-80-0-0.jpg", "Megadeth", "https://e-cdns-images.dzcdn.net/images/artist/cff12b4ef973a62d886d69bf056941ec/1000x1000-000000-80-0-0.jpg", "aab9c560-3441-40d1-b479-9bb75990ac08", 12565751, null, "What do you mean, \"I don't believe in God\"?\r\nI talk to him everyday\r\nWhat do you mean, \"I don't support your system\"?\r\nI go to court when I have to\r\nWhat do you mean, \"I can't get to work on time\"?\r\nGot nothing better to do\r\nAnd, what do you mean, \"I don't pay my bills\"?\r\nWhy do you think I'm broke? Huh?\r\n\r\nIf there's a new way\r\nOh, I'll be the first in line\r\nBut it better work this time\r\n\r\nWhat do you mean, \"I hurt your feelings\"?\r\nI didn't know you had any feelings\r\nWhat do you mean, \"I ain't kind\"?\r\nJust not your kind\r\n\r\nWhat do you mean, \"I couldn't be the President\r\nOf the United States of America\"?\r\nTell me something, it's still \"We the people\" right?\r\n\r\nIf there's a new way\r\nOh, I'll be the first in line\r\nBut it better work this time\r\n\r\nCan you put a price on peace?\r\n\r\nPeace\r\nPeace sells\r\nPeace\r\nPeace sells\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\nPeace sells, but who's buying?\r\n(No, no no no no)\r\n(Peace sells)\r\n\r\n(Peace sells, aah)", true, "Peace Sells", new DateTime(2023, 7, 25, 17, 29, 47, 301, DateTimeKind.Local).AddTicks(940), 244 },
                    { 3, "https://e-cdns-images.dzcdn.net/images/cover/fad7de079aa103d60ec1e2d1582c2281/1000x1000-000000-80-0-0.jpg", "Lady Gaga", "https://e-cdns-images.dzcdn.net/images/artist/83110025016968b8882a1c92a3284a6b/1000x1000-000000-80-0-0.jpg", "aab9c560-3441-40d1-b479-9bb75990ac08", 734508762, null, "(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n\r\nI wanna hold 'em like they do in Texas, please\r\nFold 'em, let 'em hit me, raise it, baby, stay with me (I love it)\r\nLove game intuition, play the cards with spades to start\r\nAnd after he's been hooked, I'll play the one that's on his heart\r\n\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\n\r\nI wanna roll with him, a hard pair we will be\r\nA little gamblin' is fun when you're with me (I love it)\r\nRussian roulette is not the same without a gun\r\nAnd baby, when it's love, if it's not rough, it isn't fun (fun)\r\n\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\nOh, whoa, oh, oh\r\nOh, oh-oh\r\nI'll get him hot, show him what I got\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n(Mum-mum-mum-mah)\r\n\r\nI won't tell you that I love you, kiss or hug you\r\n'Cause I'm bluffin' with my muffin\r\nI'm not lyin', I'm just stunnin' with my love-glue-gunnin'\r\nJust like a chick in the casino\r\nTake your bank before I pay you out\r\nI promise this, promise this\r\nCheck this hand 'cause I'm marvelous\r\n\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\nCan't read my, can't read my\r\nNo, he can't read my poker face\r\n(She's got me like nobody)\r\n\r\nP-p-p-poker face, p-p-poker face\r\nP-p-p-poker face, f-f-fuck her face (she's got me like nobody)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)\r\nP-p-p-poker face, f-f-fuck her face (mum-mum-mum-mah)", true, "Poker Face", new DateTime(2023, 7, 25, 17, 29, 47, 301, DateTimeKind.Local).AddTicks(944), 237 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c7b013f0-5201-4317-abd8-c211f91b7330", "00023983-9f16-4a6c-91b8-940283954fc6" },
                    { "fab4fac1-c546-41de-aebc-a14da6895711", "aab9c560-3441-40d1-b479-9bb75990ac08" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FavouritedSongs");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
