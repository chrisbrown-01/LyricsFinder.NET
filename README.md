# LyricsFinder.NET

.NET Workflow - [![.NET](https://github.com/chrisbrown-01/LyricsFinder.NET/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/chrisbrown-01/LyricsFinder.NET/actions/workflows/dotnet.yml)

CodeQL Workflow - [![CodeQL](https://github.com/chrisbrown-01/LyricsFinder.NET/actions/workflows/github-code-scanning/codeql/badge.svg?branch=master)](https://github.com/chrisbrown-01/LyricsFinder.NET/actions/workflows/github-code-scanning/codeql)

LyricsFinder.NET is an ASP.NET Core MVC app and the first major .NET project I created when I first started learning C# in late 2021. Users are able to enter a song name and artist to the database, and the app will automatically retrieve and store lyrics, previews & information of the song. The app supports full CRUD (create/read/update/delete) functionality, and all information is stored in a SQL Server database. Unauthenticated users can view songs in the database but must login to create/update/delete anything.

![LyricsFinder.NET Gif](/Documentation/screen-recording.gif)

### Overview and Key Features
- ASP.NET Core MVC that utilizes CRUD, Entity Framework Core, default Microsoft Identity (user registration/authentication/authorization), dependency injection
- Web API that supports CRUD via HTTP GET/POST/DELETE/PATCH/OPTIONS requests, JWT authentication, response caching/compression, rate limiting, Swagger UI
- Webpage design with Razor syntax and Bootstrap
- Add & remove songs to user-specific favourites list, views to display favourited songs only
- User role management, deletion if authorized as admin or moderator
- Webpage table result sorting, search term filtering
- Excel file exporting
- Utilizes Automapper, HttpClientFactory, HtmlAgilityPack, *Mailkit email service (disabled in Docker image)*
- Fake song database generator using Bogus Nuget package. This was used for development purposes before switching over to SQL Server. Simply modify Program.cs to use BogusSongDbRepo for the ISongDbRepo dependency injection.

### How To Run This Project
Ensure you have Docker Desktop installed on your computer. Download the [docker-compose.yml](docker-compose.yml), and in whichever folder the file is located, open a terminal and execute command `docker compose up -d`. Navigate to `http://localhost:8080/` to use the app.

### Other Comments
- This was a project I first completed in 2021 but reworked to use proper dependency injection and other best practices. The Views were designed before I knew any Javascript so at the time I completely relied on Bootstrap and page redirections. Therefore some of the page redirections and Bootstrap modals (for adding/removing songs to favourites, hiding song info, etc.) can act slightly odd
- For a real production app I would implement much more stringent checks on the image links that can be updated to display the artist and album artwork. Or completely remove the feature. I left it in for demonstrative purposes but I am aware this can be a security concern
- Originally the lyrics were retrieved using Selenium webscraper but I reworked this into a way quicker/reliable solution using HTTP requests and parsing of the HTML responses. Lyrics are retrieved from songmeanings.com
- All other song information, images and previews are retrieved via API calls to the Deezer public API
- The login/register pages can support OAuth logins to Google, Facebook or Microsoft if you change the environment variable to "Development" and include service credentials to appsettings.json or secrets.json
- The SQL Server image utilized in the Docker Compose file is pre-seeded with a few songs so due to some oddities with Entity Framework Core or the ApplicationDbContext file, new song IDs begin at 1001 instead of 5
 

