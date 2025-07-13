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
            try
            {
                var movie = await _movieService.GetMovieDetailsFromApiAsync(tmdbId);
                return movie == null
                    ? ApiProblem.NotFound("Not found", $"Movie with TMDB id = {tmdbId} not found or lacks essential data")
                    : Ok(movie);
            }
            catch (Exception ex)
            {
                return ApiProblem.Internal($"Failed to get movie details: {ex.Message}");
            }
        }

        [HttpGet("api/admin/movies/search")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return ApiProblem.Bad("Missing query", "Search query is empty");

            try
            {
                var results = await _movieService.SearchMoviesAsync(query);
                if (results == null || results.Count == 0)
                    return ApiProblem.NotFound("No results", $"No movies found for '{query}'");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return ApiProblem.Internal($"Failed to search movies: {ex.Message}");
            }
        }

        [HttpPost("api/admin/movies/save")]
        public async Task<IActionResult> SaveMovie([FromBody] JsonElement raw)
        {
            try
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
                    return ApiProblem.Bad("Invalid data", "MovieSaveDto could not be deserialized");

                dto.Genres = genreNames;

                var result = await _movieService.SaveMovieFromDtoAsync(dto);
                return result ? Ok() : ApiProblem.Conflict("Duplicate movie or failed save", "The movie may already exist or save failed");
            }
            catch (JsonException ex)
            {
                return ApiProblem.Bad("Invalid JSON", ex.Message);
            }
            catch (Exception ex)
            {
                return ApiProblem.Internal($"Error while saving movie: {ex.Message}");
            }
        }

        [HttpGet("api/movies/all")] 
        public async Task<IActionResult> GetAllLocalMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync(); 
              
                return Ok(movies); 
            }
            catch (Exception ex)
            {
               
                return ApiProblem.Internal($"Failed to retrieve movies: {ex.Message}");
            }
        }
    }
}
