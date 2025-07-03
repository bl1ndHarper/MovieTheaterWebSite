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

            var vm = new HomePageViewModel
            {
                SelectedDate = selectedDate,
                LatestMovies = await GetLatestMovies(6),
                MoviesByDay = await GetNowShowing(parsedDate)
            };
            return View(vm);
        }
        [HttpGet("/Upcoming")]
        public async Task<IActionResult> Upcoming()
        { 
            return View();
        }


        // Private methods
        private Task<List<MovieMainDto>> GetLatestMovies(int count) =>
            _movieService.GetLatestMoviesAsync(count);

        private Task<List<MovieMainDto>> GetNowShowing(DateTime parsedDate) =>
            _movieService.GetNowShowingAsync(parsedDate);


        // API endpoints
        [HttpGet("api/movies/latest/{count}")]
        public async Task<IActionResult> ApiLatestMovies(int count) =>
            Ok(await GetLatestMovies(count));

        [HttpGet("api/movies/now-showing")]
//        public async Task<IActionResult> ApiNowShowing() =>
//            Ok(await GetNowShowing(parsedDate));


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
