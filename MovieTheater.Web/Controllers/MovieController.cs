using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Application.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace MovieTheater.Web.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreateDto dto)
        {
            var created = await _movieService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetMovie), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(long id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            return movie == null ? NotFound() : Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(long id, [FromBody] MovieUpdateDto dto)
        {
            var updated = await _movieService.UpdateMovieAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchMovie(long id, [FromBody] JsonPatchDocument<MovieUpdateDto> patch)
        {
            var result = await _movieService.PatchMovieAsync(id, patch);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
