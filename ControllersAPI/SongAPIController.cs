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
using Microsoft.AspNetCore.Mvc;

namespace LyricsFinder.NET.ControllersAPI
{
    [Route("api/songs")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "60SecondsDuration")]
    public class SongAPIController : ControllerBase
    {
        private readonly ISongDbRepo _db;
        private readonly ISongRetrieval _songRetriever;
        private readonly IMapper _mapper;
        private readonly UserManager<CustomAppUserData> _userManager;
        private readonly ILogger<SongAPIController> _logger;

        public SongAPIController(
            ISongDbRepo db,
            ISongRetrieval songRetriever,
            IMapper mapper,
            UserManager<CustomAppUserData> userManager,
            ILogger<SongAPIController> logger)
        {
            _db = db;
            _songRetriever = songRetriever;
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        // GET api/songs
        [HttpGet]
        [HttpHead] // TODO: how to use this?
        //[ResponseCache(Duration = 60)]
        public ActionResult<IEnumerable<SongReadDTO>> GetAllSongs()
        {
            _logger.LogInformation("All songs info requested via API");

            var songs = _db.GetAllSongsInDb();

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        // GET api/songs/{id}
        [HttpGet("{id}", Name = "GetSongById")]
        public ActionResult<SongReadDTO> GetSongById(int id)
        {
            _logger.LogInformation($"Song with id:{id} requested via API");

            var song = _db.GetSongById(id);

            if (song == null) return NotFound();

            return Ok(_mapper.Map<SongReadDTO>(song));
        }

        // GET api/songs/songName/{songName}
        [HttpGet("songName/{songName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsBySongName(string songName)
        {
            _logger.LogInformation($"Songs with name \"{songName}\" requested via API");

            var songs = _db.GetSongsByName(songName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        // GET api/songs/artistName/{artistName}
        [HttpGet("artistName/{artistName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsByArtist(string artistName)
        {
            _logger.LogInformation($"Songs with artist \"{artistName}\" requested via API");

            var songs = _db.GetSongsByArtist(artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        /// <summary>
        /// Query database by song name AND artist name, return result via API
        /// </summary>
        /// <param name="songName"></param>
        /// <param name="artistName"></param>
        /// <returns></returns>
        // GET api/songs/songNameArtistName/{songName}/{artistName}
        [HttpGet("songNameArtistName/{songName}/{artistName}")]
        public ActionResult<IEnumerable<SongReadDTO>> GetSongsBySongArtist(string songName, string artistName)
        {
            _logger.LogInformation($"Songs with name \"{songName}\" and artist \"{artistName}\" requested via API");

            var songs = _db.GetSongsBySongNameArtist(songName, artistName);

            return Ok(_mapper.Map<IEnumerable<SongReadDTO>>(songs));
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("createSong")]
        public async Task<ActionResult<SongReadDTO>> CreateSongAsync(SongCreateDTO createSongDTO)
        {
            //var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            //if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            //var loggedInUser = await _userManager.FindByEmailAsync(email);

            var song = _mapper.Map<Song>(createSongDTO); // TODO: song id 0?

            if (_db.IsSongDuplicate(song)) return BadRequest("Song already exists in database.");

            //_logger.LogInformation("Song creation request received via API. Song info: {@Song}. User info: {@User}", createSongDTO, loggedInUser);

            song.QueryDate = DateTime.Now;
            //song.CreatedBy = loggedInUser.Id;

            // TODO: *****consolidate CRUD actions with normal controller and leave all exception handling for filters?
            try
            {
                await _db.AddSongToDb(song);
            }
            catch (Exception ex) // TODO: replace all try catches in this controller with filter
            {
                _logger.LogError("API song creation request could not be added to database: {@Exception}", ex.Message);
                return StatusCode(500);
            }

            try
            {
                song = await _songRetriever.RetrieveSongContentsAsync(song);
                await _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
            }

            var songDto = _mapper.Map<SongReadDTO>(song);

            return CreatedAtRoute(nameof(GetSongById), new { songDto.Id }, songDto);
        }

        /// <summary>
        /// Edit existing database song object and retrieve song info & lyrics again. Return edited song via API.
        /// </summary>
        /// <param name="editSongDTO"></param>
        /// <returns></returns>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("editSong")]
        public async Task<ActionResult<SongReadDTO>> EditSongAsync(SongEditDTO editSongDTO)
        {
            var song = _db.GetSongById(editSongDTO.Id);

            if (song == null) return NotFound();

            if (_db.IsSongDuplicate(_mapper.Map<Song>(editSongDTO))) return BadRequest("Song already exists in database.");

            //var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            //if (email == null) return StatusCode(500, "Authenticated user could not be identified");
            //var loggedInUser = await _userManager.FindByEmailAsync(email);

            //_logger.LogInformation("Song edit request received via API. Song info: {@Song}. User info: {@User}", editSongDTO, loggedInUser);

            song.Name = editSongDTO.Name;
            song.Artist = editSongDTO.Artist;
            song.QueryDate = DateTime.Now;
            //song.EditedBy = loggedInUser.Id;

            try
            {
                await _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogError("API song edit request could not be added to database: {@Exception}", ex.Message);
                return StatusCode(500, "Song could not be edited.");
            }

            try
            {
                song = await _songRetriever.RetrieveSongContentsAsync(song);
                await _db.UpdateSongInDb(song);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning("Could not retrieve lyrics for song: {@Song}. Exception: \"{@Exception}\"", song, ex.Message);
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("updateSong/{id}")]
        public async Task<ActionResult<SongReadDTO>> PartialUpdateSongInfoAsync(
            int id, 
            [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<SongUpdateDTO> patchDoc)
        {
            var song = _db.GetSongById(id);

            if (song == null) return NotFound();

            var songToUpdate = _mapper.Map<SongUpdateDTO>(song);

            patchDoc.ApplyTo(songToUpdate, ModelState);

            if (!TryValidateModel(songToUpdate)) return ValidationProblem(ModelState);

            song = _mapper.Map(songToUpdate, song);

            //var email = HttpContext.User.Claims.FirstOrDefault()?.Value;
            //if (email == null) return StatusCode(500, "Authenticated user could not be identified"); // TODO: change to 403
            //var loggedInUser = await _userManager.FindByEmailAsync(email);

            //_logger.LogInformation("Song information partial edit request received via API. Song id: {@id}. Song info: {@Song}. User info: {@User}", id, patchDoc, loggedInUser);

            song.QueryDate = DateTime.Now;
            //song.EditedBy = loggedInUser.Id;

            try
            {
                await _db.UpdateSongInDb(song);
            }
            catch (Exception ex)
            {
                _logger.LogError("API song partial edit request could not be added to database: {@Exception}", ex.Message);
                return StatusCode(500, "Song could not be updated.");
            }

            var songDTO = _mapper.Map<SongReadDTO>(song);

            return Ok(songDTO);
        }
    }
}
