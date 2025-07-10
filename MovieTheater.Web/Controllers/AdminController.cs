using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;

namespace MovieTheater.Web.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IHallService _hallService;
        private readonly IMovieService _movieService;

        public AdminController(ILogger<AdminController> logger, IHallService hallService, IMovieService movieService)
        {
            _logger = logger;
            _hallService = hallService;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(string? day)
        {   
            var selectedDate = string.IsNullOrEmpty(day)
                ? DateTime.UtcNow.ToString("dd.MM")
                : day;

            var model = new AdminHomePageViewModel
            {
                SelectedDate = selectedDate,
                Halls = await _hallService.GetAllHallsWithSectorsAsync()
            };

            if (day == "past" || day == "future")
            {
                model.MoviesGroupedByDate = await _movieService.GetGroupedByDateAsync(day, daysBack: 7);
                model.Genres = model.MoviesGroupedByDate.SelectMany(g => g.Movies.Select(m => m.Genre)).Distinct().ToList();
                model.Ratings = model.MoviesGroupedByDate.SelectMany(g => g.Movies.Select(m => m.AgeRating)).Distinct().ToList();
            }
            else
            {
                model.MoviesByDay = await _movieService.GetNowShowingAsync(selectedDate);
                model.Genres = model.MoviesByDay.Select(m => m.Genre).Distinct().ToList();
                model.Ratings = model.MoviesByDay.Select(m => m.AgeRating).Distinct().ToList();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSectorPrice(long sectorId, decimal price)
        {
            await _hallService.UpdateSectorPriceAsync(sectorId, price);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AddSession()
        { 
            return View("AddSession");
        }

        [HttpPost]
        public async Task<IActionResult> AddSector(long hallId, string name, decimal seatPrice)
        {
            await _hallService.AddSectorAsync(hallId, name, seatPrice);
            return RedirectToAction("Index");
        }
    }
}
