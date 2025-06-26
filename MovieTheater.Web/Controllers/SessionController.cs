using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;

namespace MovieTheater.Web.Controllers
{
    public class SessionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public SessionController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        [Route("Session/{id}")]
        public async Task<IActionResult> Session(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);

            var viewModel = new SessionPageViewModel
            {
                MovieDetails = movie
            };

            ViewData["SessionId"] = id;
            return View("Session", viewModel);
        }
    }
}