using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Interfaces;
using MovieTheater.Application.Services;

namespace MovieTheater.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService) => _bookingService = bookingService;

        [HttpGet("{id:long}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Details(long id)
        {
            var b = await _bookingService.GetAsync(id);
            return b == null ? NotFound() : View(b);
        }
    }
}
