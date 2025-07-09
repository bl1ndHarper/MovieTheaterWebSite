using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Application.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using MovieTheater.Web.Infrastructure;
using System.Globalization;
using System.Text.Json;


namespace MovieTheater.Web.ApiControllers
{
    [Route("")]
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

        [HttpGet("api/movies/{id:long}")]
        public async Task<IActionResult> GetMovie(long id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            return movie == null
                ? ApiProblem.NotFound("Movie not found", $"Movie id = {id}")
                : Ok(movie);
        }

        [HttpPut("api/movies/{id:long}")]
        public async Task<IActionResult> UpdateMovie(long id, MovieUpdateDto dto)
        {
            if (!ModelState.IsValid) return ApiProblem.Bad("Bad request", "Invalid movie data");

            return await _movieService.UpdateMovieAsync(id, dto)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"id = {id}");
        }

        [HttpPatch("api/movies/{id:long}")]
        public async Task<IActionResult> PatchMovie(long id, JsonPatchDocument<MovieUpdateDto> patch)
        {
            if (patch == null) return ApiProblem.Bad("Bad request", "Patch document required");

            return await _movieService.PatchMovieAsync(id, patch)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"id = {id}");
        }

        [HttpDelete("api/movies/{id:long}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            return await _movieService.DeleteMovieAsync(id)
                ? NoContent()
                : ApiProblem.NotFound("Movie not found", $"Movie id = {id}");
        }

        [HttpGet("api/movies/latest/{count:int}")]
        public async Task<IActionResult> GetLatestMovies(int count)
        {
            if (count <= 0) return Infrastructure.ApiProblem.Bad("Invalid count", "Count must be positive");

            var movies = await _movieService.GetLatestMoviesAsync(count);
            return Ok(movies);
        }

        [HttpGet("api/movies/now-showing/{day}")]
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

        [HttpGet("api/admin/movies/details/{tmdbId}")]
        public async Task<IActionResult> GetMovieDetailsFromTmdb(int tmdbId)
        {
            var movie = await _movieService.GetMovieDetailsFromApiAsync(tmdbId);
            return movie == null
                ? ApiProblem.NotFound("Not found", $"Movie with TMDB id = {tmdbId} not found")
                : Ok(movie);
        }

        [HttpGet("api/admin/movies/search")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            var results = await _movieService.SearchMoviesAsync(query);
            return Ok(results);
        }

        [HttpPost("api/admin/movies/save")]
        public async Task<IActionResult> SaveMovie([FromBody] JsonElement raw)
        {
            var genreNames = new List<string>();
            if (raw.TryGetProperty("genres", out var genresProp) && genresProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in genresProp.EnumerateArray())
                {
                    if (item.TryGetProperty("genre", out var genreObj) &&
                        genreObj.TryGetProperty("name", out var nameProp))
                    {
                        var name = nameProp.GetString();
                        if (!string.IsNullOrWhiteSpace(name))
                            genreNames.Add(name);
                    }
                }
            }

            using var doc = JsonDocument.Parse(raw.GetRawText());
            var root = doc.RootElement;

            var filteredProperties = root.EnumerateObject()
                .Where(p => p.Name != "genres")
                .ToDictionary(p => p.Name, p => p.Value);

            using var filteredDoc = JsonDocument.Parse(JsonSerializer.Serialize(filteredProperties));
            var dto = JsonSerializer.Deserialize<MovieSaveDto>(filteredDoc.RootElement.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
                return BadRequest("Невалідні дані");

            dto.Genres = genreNames;

            var result = await _movieService.SaveMovieFromDtoAsync(dto);

            return result ? Ok() : BadRequest("Фільм уже існує або сталася помилка при збереженні");
        }
    }
}
