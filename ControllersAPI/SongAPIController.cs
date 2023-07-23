using AutoMapper;
using LyricsFinder.NET.Areas.Identity.Models;
using LyricsFinder.NET.Data.Repositories;
using LyricsFinder.NET.Models;
using LyricsFinder.NET.Models.DTOs;
using LyricsFinder.NET.Services.SongRetrieval;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LyricsFinder.NET.ControllersAPI
{
    [Route("api/songs")]
    [ApiController]
    [ResponseCache(Duration = 60)]
    public class SongAPIController : ControllerBase
    {
        private readonly ISongDbRepo _db;
        private readonly ISongRetrieval _songRetriever;
        private readonly IMapper _mapper;
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

        [HttpGet]
        public ActionResult<IEnumerable<SongReadDTO>> GetAllSongs()
        {
            var songs = _db.GetAllSongsInDb();

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        [HttpGet("{id}", Name = "GetSongById")]
        public ActionResult<SongReadDTO> GetSongById(int id)
        {
            var song = _db.GetSongById(id);

            if (song == null) return NotFound();

            return Ok(_mapper.Map<SongReadDTO>(song));
        }

        [HttpGet("songName/{songName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsBySongName(string songName)
        {
            var songs = _db.GetSongsByName(songName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        [HttpGet("artistName/{artistName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsByArtist(string artistName)
        {
            var songs = _db.GetSongsByArtist(artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        /// <summary>
        /// Query database by song name AND artist name, return result via API
        /// </summary>
        /// <param name="songName"></param>
        /// <param name="artistName"></param>
        /// <returns></returns>
        [HttpGet("songNameArtistName/{songName}/{artistName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsBySongArtist(string songName, string artistName)
        {
            var songs = _db.GetSongsBySongNameArtist(songName, artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("create")]
        public async Task<ActionResult<SongReadDTO>> CreateSongAsync(SongCreateDTO createSongDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            var loggedInUser = await _userManager.FindByEmailAsync(email);

            var song = _mapper.Map<Song>(createSongDTO);

            if (_db.IsSongDuplicate(song)) return BadRequest("Song already exists in database.");

            song.QueryDate = DateTime.Now;
            song.CreatedBy = loggedInUser!.Id;

            // TODO: *****consolidate CRUD actions with normal controller and leave all exception handling for filters?
            await _db.AddSongToDb(song);


            try
            {
                song = await _songRetriever.RetrieveSongContentsAsync(song);
                await _db.UpdateSongInDb(song);
            }
            catch (Exception) // TODO: replace all try catches in this controller with filter
            {
                return StatusCode(500);
            }

            var songDto = _mapper.Map<SongReadDTO>(song);

            return CreatedAtRoute(nameof(GetSongById), new { songDto.Id }, songDto);
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again. Return edited song via API.
        /// </summary>
        /// <param name="editSongDTO"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("edit")]
        public async Task<ActionResult<SongReadDTO>> EditSongAsync(SongEditDTO editSongDTO)
        {
            var song = _db.GetSongById(editSongDTO.Id);

            if (song == null) return NotFound();

            if (_db.IsSongDuplicate(_mapper.Map<Song>(editSongDTO))) return BadRequest("Song already exists in database.");

            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            var loggedInUser = await _userManager.FindByEmailAsync(email);

            song.Name = editSongDTO.Name;
            song.Artist = editSongDTO.Artist;
            song.QueryDate = DateTime.Now;
            song.EditedBy = loggedInUser.Id;

            await _db.UpdateSongInDb(song);


            try
            {
                song = await _songRetriever.RetrieveSongContentsAsync(song);
                await _db.UpdateSongInDb(song);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            var editedSongDTO = _mapper.Map<SongReadDTO>(song);

            return CreatedAtRoute(nameof(GetSongById), new { editedSongDTO.Id }, editedSongDTO);
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
            var song = _db.GetSongById(id);

            if (song == null) return NotFound();

            var songToUpdate = _mapper.Map<SongUpdateDTO>(song);

            patchDoc.ApplyTo(songToUpdate, ModelState);

            if (!TryValidateModel(songToUpdate)) return ValidationProblem(ModelState);

            song = _mapper.Map(songToUpdate, song);

            var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            var loggedInUser = await _userManager.FindByEmailAsync(email);

            song.QueryDate = DateTime.Now;
            song.EditedBy = loggedInUser!.Id;

            await _db.UpdateSongInDb(song);

            var songDTO = _mapper.Map<SongReadDTO>(song);

            return Ok(songDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSongAsync(int id)
        {
            var song = _db.GetSongById(id);

            if (song == null) return NotFound();

            await _db.DeleteSongFromDb(song);

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetApiOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, PATCH, DELETE");
            return NoContent();
        }
    }
}
