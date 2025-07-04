using Microsoft.AspNetCore.Mvc;
using MovieTheater.Web.Models;
using System.Diagnostics;

namespace MovieTheater.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UpdateSession(int id)
        { 
            return View("UpdateSession");
        }

        public async Task<IActionResult> AddSession()
        { 
            return View("AddSession");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}