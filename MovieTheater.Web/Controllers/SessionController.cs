using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;

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

        [HttpGet("Movies/{movieId}/Sessions")]
        public async Task<IActionResult> Index([FromRoute]long movieId, string day)
        {
            var selectedDate = string.IsNullOrEmpty(day)
                ? DateTime.Now.ToLocalTime().ToString("dd.MM")
                : day;

            var sessionsByDay = await _sessionService.GetMovieSessionsByDayAsync(movieId, selectedDate);
            var allAvailableDates = await _sessionService.GetAvailableSessionDatesAsync(movieId);

            var viewModel = new SessionPageViewModel
            {
                MovieDetails = await _movieService.GetMovieByIdAsync(movieId),
                Sessions = sessionsByDay,
                AvailableDates = allAvailableDates,
                SelectedDate = selectedDate
            };

            ViewData["SessionId"] = movieId;
            return View("Index", viewModel);
        }

        [HttpGet("Sessions/{sessionId}/Seats")]
        public async Task<IActionResult> GetSeats(long sessionId)
        {
            var seats = await _sessionService.GetSeatsBySessionIdAsync(sessionId);
            return Json(seats);
        }
    }
}