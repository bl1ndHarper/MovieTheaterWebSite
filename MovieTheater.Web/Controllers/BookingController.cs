using System.Security.Claims;
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

        [HttpPost("/Booking/Cancel/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Cancel(long id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userId, out var uid)) return Unauthorized();

            var result = await _bookingService.CancelAsync(id, uid);
            return result ? Ok() : BadRequest();
        }
    }
}
