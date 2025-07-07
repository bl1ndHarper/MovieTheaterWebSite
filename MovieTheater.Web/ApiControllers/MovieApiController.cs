using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Application.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using MovieTheater.Web.Infrastructure;
using System.Globalization;


namespace MovieTheater.Web.ApiControllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieApiController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieApiController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreateDto dto)
        {
            if (!ModelState.IsValid) return ApiProblem.Bad("Bad request", "Invalid movie data");

            var created = await _movieService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetMovie), new { id = created.Id }, created);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetMovie(long id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            return movie == null
                ? ApiProblem.NotFound("Movie not found", $"Movie id = {id}")
                : Ok(movie);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateMovie(long id, MovieUpdateDto dto)
        {
            if (!ModelState.IsValid) return ApiProblem.Bad("Bad request", "Invalid movie data");

            return await _movieService.UpdateMovieAsync(id, dto)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"id = {id}");
        }

        [HttpPatch("{id:long}")]
        public async Task<IActionResult> PatchMovie(long id, JsonPatchDocument<MovieUpdateDto> patch)
        {
            if (patch == null) return ApiProblem.Bad("Bad request", "Patch document required");

            return await _movieService.PatchMovieAsync(id, patch)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"id = {id}");
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            return await _movieService.DeleteMovieAsync(id)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"Movie id = {id}");
        }

        [HttpGet("latest/{count:int}")]
        public async Task<IActionResult> GetLatestMovies(int count)
        {
            if (count <= 0) return Infrastructure.ApiProblem.Bad("Invalid count", "Count must be positive");

            var movies = await _movieService.GetLatestMoviesAsync(count);
            return Ok(movies);
        }

        
        

        
        [HttpGet("now-showing/{day}")]
        public async Task<IActionResult> GetNowShowingMovies(string day)
        {
            if (!DateTime.TryParseExact(
                    day + "." + DateTime.UtcNow.Year,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
                return Infrastructure.ApiProblem.Bad("Bad date", $"'{day}' is not dd.MM");

            var movies = await _movieService.GetNowShowingAsync(day);
            return Ok(movies);
        }
    }
}
