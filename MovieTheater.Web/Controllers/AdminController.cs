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

        public AdminController(ILogger<AdminController> logger, IHallService hallService)
        {
            _logger = logger;
            _hallService = hallService;
        }

        public async Task<IActionResult> Index()
        {
            var halls = await _hallService.GetAllHallsWithSectorsAsync();
            return View(new AdminPanelViewModel { Halls = halls });
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
