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
        public async Task<IActionResult> Index()
        {
            var vm = new HomePageViewModel
            {
                LatestMovies = await GetLatestMovies(6),
                AllSessions = await GetNowShowing()
            };
            return View(vm);
        }


        // Private methods
        private Task<List<MovieMainDto>> GetLatestMovies(int count) =>
            _movieService.GetLatestMoviesAsync(count);

        private Task<List<MovieMainDto>> GetNowShowing() =>
            _movieService.GetNowShowingAsync(DateTime.UtcNow.Date);


        // API endpoints
        [HttpGet("api/movies/latest/{count}")]
        public async Task<IActionResult> ApiLatestMovies(int count) =>
            Ok(await GetLatestMovies(count));

        [HttpGet("api/movies/now-showing")]
        public async Task<IActionResult> ApiNowShowing() =>
            Ok(await GetNowShowing());


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
