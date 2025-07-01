using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;
using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.Controllers
{
    [Route("")]
    public class SessionController : Controller
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IMovieService _movieService;
        private readonly ISessionService _sessionService;

        public SessionController(
            ILogger<SessionController> logger,
            IMovieService movieService,
            ISessionService sessionService)
        {
            _logger = logger;
            _movieService = movieService;
            _sessionService = sessionService;
        }

        // Index (MVC)
        [HttpGet("Movies/{movieId}/Sessions")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(long movieId, string day)
        {
            var selectedDate = string.IsNullOrEmpty(day)
                ? DateTime.Now.ToLocalTime().ToString("dd.MM")
                : day;

            var viewModel = new SessionPageViewModel
            {
                MovieDetails = await GetMovieDetails(movieId),
                Sessions = await GetSessionsByDay(movieId, selectedDate),
                AvailableDates = await GetAvailableDates(movieId),
                SelectedDate = selectedDate
            };

            ViewData["SessionId"] = movieId;
            return View("Index", viewModel);
        }


        // API endpoints
        [HttpGet("api/movies/{movieId}/dates")]
        public async Task<IActionResult> ApiAvailableDates(long movieId)
        {
            var dates = await GetAvailableDates(movieId);
            return Ok(dates);
        }
        
        [HttpGet("api/movies/{movieId}/sessions/{day}")]
        public async Task<IActionResult> ApiSessionsByDay(long movieId, string day)
        {
            var sessions = await GetSessionsByDay(movieId, day);
            return Ok(sessions);
        }

        [HttpGet("api/sessions/{sessionId}/seats")]
        public async Task<IActionResult> ApiSeats(long sessionId)
        {
            var seats = await _sessionService.GetSeatsBySessionIdAsync(sessionId);
            return Ok(seats);
        }

        [HttpGet("api/sessions/{id}")]
        public async Task<IActionResult> GetSessionById(long id)
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpPost("api/sessions")]
        public async Task<IActionResult> CreateSession([FromBody] MovieSessionDto dto)
        {
            var created = await _sessionService.CreateSessionAsync(dto);
            return Created("", created);
        }

        [HttpPost("api/sessions/daily")]
        public async Task<IActionResult> CreateDailySessions([FromBody] SessionDailyCreateDto dto)
        {
            var results = new List<object>();
            for (var date = dto.StartDate.Date; date <= dto.EndDate.Date; date = date.AddDays(1))
            {
                var dailySession = new MovieSessionDto
                {
                    MovieId = dto.MovieId,
                    StartTime = date.Add(dto.StartTime.TimeOfDay),
                    Hall = dto.Hall,
                    SeatsTotal = dto.SeatsTotal
                };
                var created = await _sessionService.CreateSessionAsync(dailySession);
                results.Add(created);
            }

            return Ok(results);
        }

        [HttpDelete("api/sessions/{id}")]
        public async Task<IActionResult> DeleteSession(long id)
        {
            var deleted = await _sessionService.DeleteSessionAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPut("api/sessions/{id}")]
        public async Task<IActionResult> UpdateSession(long id, [FromBody] MovieSessionDto dto)
        {
            var updated = await _sessionService.UpdateSessionAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }


        // MVC-compatible JSON
        [HttpGet("Sessions/{sessionId}/Seats")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetSeats(long sessionId)
        {
            var seats = await _sessionService.GetSeatsBySessionIdAsync(sessionId);
            return Json(seats);
        }


        // Private methods
        private Task<Dictionary<DateTime, List<MovieSessionDto>>> GetSessionsByDay(long movieId, string day) =>
            _sessionService.GetMovieSessionsByDayAsync(movieId, day);

        private Task<List<string>> GetAvailableDates(long movieId) =>
            _sessionService.GetAvailableSessionDatesAsync(movieId);

        private Task<MovieDto> GetMovieDetails(long movieId) =>
            _movieService.GetMovieByIdAsync(movieId)!;
    }
}