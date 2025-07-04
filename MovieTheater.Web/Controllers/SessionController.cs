using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;
using MovieTheater.Application.DTOs;
using MovieTheater.Web.Infrastructure;

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
                MovieDetails = await _movieService.GetMovieByIdAsync(movieId),
                Sessions = await _sessionService.GetMovieSessionsByDayAsync(movieId, day),
                AvailableDates = await _sessionService.GetAvailableSessionDatesAsync(movieId),
                SelectedDate = selectedDate
            };

            ViewData["SessionId"] = movieId;
            return View("Index", viewModel);
        }


        // MVC-compatible JSON
        [HttpGet("Sessions/{sessionId}/Seats")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetSeats(long sessionId)
        {
            var seats = await _sessionService.GetSeatsBySessionIdAsync(sessionId);
            return Json(seats);
        }
    }
}