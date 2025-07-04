using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Application.Services;
using MovieTheater.Web.Models;
using MovieTheater.Web.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using MovieTheater.Application.DTOs;
using Microsoft.AspNetCore.JsonPatch.Internal;
using System.Globalization;

namespace MovieTheater.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
      

        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;

            
        }

        // Index (MVC)
        public async Task<IActionResult> Index(string? day)
        {
            var selectedDate = string.IsNullOrEmpty(day)
                ? DateTime.UtcNow.ToString("dd.MM")
                : day;

            var parsedDate = DateTime.SpecifyKind(
                DateTime.ParseExact(
                    selectedDate + "." + DateTime.UtcNow.Year,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture),
                DateTimeKind.Utc);

            var moviesByDay = await GetNowShowing(parsedDate);

            var genres = moviesByDay
                .Select(m => m.Genre)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            var ratings = moviesByDay
                .Select(m => m.AgeRating)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            var vm = new HomePageViewModel
            {
                SelectedDate = selectedDate,
                LatestMovies = await GetLatestMovies(6),
                MoviesByDay = moviesByDay,

                Genres = genres,
                Ratings = ratings
            };
            return View(vm);
        }
        [HttpGet("/Upcoming")]
        public Task<IActionResult> Upcoming()
        { 
            return Task.FromResult<IActionResult>(View());
        }


        // Private methods
        private Task<List<MovieMainDto>> GetLatestMovies(int count) =>
            _movieService.GetLatestMoviesAsync(count);

        private Task<List<MovieMainDto>> GetNowShowing(string day)
        {
            var parsedDate = DateTime.SpecifyKind(
                DateTime.ParseExact(
                    day + "." + DateTime.UtcNow.Year,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture),
                DateTimeKind.Utc);

            return _movieService.GetNowShowingAsync(parsedDate);
        }


        // API endpoints
        [HttpGet("api/movies/latest/{count}")]
        public async Task<IActionResult> ApiLatestMovies(int count) =>
            Ok(await GetLatestMovies(count));

        [HttpGet("api/movies/now-showing/{day}")]
        public async Task<IActionResult> ApiNowShowing(string day) =>
            Ok(await GetNowShowing(day));


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
