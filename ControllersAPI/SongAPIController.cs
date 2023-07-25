using AutoMapper;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Filters;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Models.DTOs;
using LyricsFinder.NET.Services.SongRetrieval;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LyricsFinder.NET.ControllersAPI
{
    [Route("api/songs")]
    [ApiController]
    [ResponseCache(Duration = 60)]
    [ServiceFilter(typeof(CheckSongIdFilter))]
    public class SongAPIController : ControllerBase
    {
        private readonly ISongDbRepo _db;
        private readonly IMapper _mapper;
        private readonly ISongRetrieval _songRetriever;
        private readonly UserManager<CustomAppUserData> _userManager;

        public SongAPIController(
            ISongDbRepo db,
            ISongRetrieval songRetriever,
            IMapper mapper,
            UserManager<CustomAppUserData> userManager)
        {
            _db = db;
            _songRetriever = songRetriever;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("create")]
        public async Task<ActionResult<SongReadDTO>> CreateSongAsync(SongCreateOrEditDTO createSongDTO)
        {
            var song = _mapper.Map<Song>(createSongDTO);

            if (_db.IsSongDuplicate(song)) return BadRequest("Song already exists in database.");

            // TODO: replace all try catches in this controller with filter
            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");

            var loggedInUser = await _userManager.FindByEmailAsync(email);

            var newSong = new Song()
            {
                Name = song.Name,
                Artist = song.Artist,
                QueryDate = DateTime.Now,
                CreatedBy = loggedInUser!.Id,
            };

            newSong = await _songRetriever.RetrieveSongContentsAsync(newSong);
            await _db.AddSongAsync(newSong);

            var songDto = _mapper.Map<SongReadDTO>(newSong);

            return CreatedAtRoute(nameof(GetSongByIdAsync), new { songDto.Id }, songDto);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSongAsync(int id)
        {
            var song = await _db.GetSongByIdAsync(id);

            await _db.DeleteSongAsync(song!);

            return NoContent();
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again. Return edited song via API.
        /// </summary>
        /// <param name="editSongDTO"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("edit/{id}")]
        public async Task<ActionResult<SongReadDTO>> EditSongAsync(int id, SongCreateOrEditDTO editSongDTO)
        {
            var editedSong = await _db.GetSongByIdAsync(id);

            if (_db.IsSongDuplicate(_mapper.Map<Song>(editSongDTO))) return BadRequest("Song already exists in database.");

            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");

            var loggedInUser = await _userManager.FindByEmailAsync(email);

            editedSong!.Name = editSongDTO.Name;
            editedSong.Artist = editSongDTO.Artist;
            editedSong.QueryDate = DateTime.Now;
            editedSong.EditedBy = loggedInUser!.Id;

            editedSong = await _songRetriever.RetrieveSongContentsAsync(editedSong);
            await _db.UpdateSongAsync(editedSong);

            var responseSongDto = _mapper.Map<SongReadDTO>(editedSong);

            return Ok(responseSongDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongReadDTO>>> GetAllSongsAsync()
        {
            var songs = await _db.GetAllSongsAsync();

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        [HttpOptions]
        public IActionResult GetApiOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, PATCH, DELETE");
            return NoContent();
        }

        [HttpGet("{id}", Name = "GetSongByIdAsync")]
        public async Task<ActionResult<SongReadDTO>> GetSongByIdAsync(int id)
        {
            var song = await _db.GetSongByIdAsync(id);

            return Ok(_mapper.Map<SongReadDTO>(song));
        }

        [HttpGet("artistName/{artistName}")]
        public async Task<ActionResult<IEnumerable<SongReadDTO>>> GetSongsByArtistAsync(string artistName)
        {
            var songs = await _db.GetSongsByArtistAsync(artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        /// <summary>
        /// Query database by song name AND artist name, return result via API
        /// </summary>
        /// <param name="songName"></param>
        /// <param name="artistName"></param>
        /// <returns></returns>
        [HttpGet("songNameArtistName/{songName}/{artistName}")]
        public async Task<ActionResult<IEnumerable<SongReadDTO>>> GetSongsBySongArtistAsync(string songName, string artistName)
        {
            var songs = await _db.GetSongsBySongNameArtistAsync(songName, artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        [HttpGet("songName/{songName}")]
        public async Task<ActionResult<IEnumerable<SongReadDTO>>> GetSongsBySongNameAsync(string songName)
        {
            var songs = await _db.GetSongsByNameAsync(songName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        /* Sample patch request body:
         [{
         "op":"replace",
         "path":"/SongDuration",
         "value":1
      },
      {
         "op":"replace",
         "path":"/ArtistArtLink",
         "value":"https://en.wikipedia.org/wiki/File:Felis_silvestris_silvestris_small_gradual_decrease_of_quality.png"
      },
      {
       "op":"replace",
         "path":"/Lyrics",
         "value":"abc123"
      }]
        */

        /// <summary>
        /// Update song details and lyrics via patch document.
        /// </summary>
        /// <param name="id">Database song id</param>
        /// <param name="patchDoc">Json patch document with "op, path, value" parameters specified</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("update/{id}")]
        public async Task<ActionResult<SongReadDTO>> PartialUpdateSongInfoAsync(
            int id,
            [FromBody] JsonPatchDocument<SongUpdateDTO> patchDoc)
        {
            var song = await _db.GetSongByIdAsync(id);

            var songToUpdate = _mapper.Map<SongUpdateDTO>(song);

            patchDoc.ApplyTo(songToUpdate, ModelState);

            if (!TryValidateModel(songToUpdate)) return ValidationProblem(ModelState);

            song = _mapper.Map(songToUpdate, song);

            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            var loggedInUser = await _userManager.FindByEmailAsync(email);

            song!.QueryDate = DateTime.Now;
            song.EditedBy = loggedInUser!.Id;

            await _db.UpdateSongAsync(song);

            var songDTO = _mapper.Map<SongReadDTO>(song);

            return Ok(songDTO);
        }
    }
}