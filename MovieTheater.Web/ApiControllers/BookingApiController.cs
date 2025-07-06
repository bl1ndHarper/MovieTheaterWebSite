using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;
using MovieTheater.Application.DTOs;
using MovieTheater.Web.Infrastructure;
using System.Security.Claims;

namespace MovieTheater.Web.ApiControllers
{
    public class BookingApiController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ISessionService _sessionService;

        public BookingApiController(
            IBookingService bookingService,
            ISessionService sessionService)
        {
            _bookingService = bookingService;
            _sessionService = sessionService;
        }

        [HttpPost("api/sessions/{sessionId:long}/bookings")]
        public async Task<IActionResult> BookSeat(long sessionId, [FromBody] BookingDto dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.SeatLabel))
                return ApiProblem.Bad("Bad request", "SeatLabel required");

            if (!User.Identity?.IsAuthenticated ?? true)
                return ApiProblem.Unauthorized("Unauthorized");

            dto.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            dto.SessionId = sessionId;

            var (ok, err, result) = await _bookingService.CreateAsync(dto);
            return ok
                ? Created($"/api/bookings/{result!.Id}", result)
                : err switch
                {
                    "session_not_found" => ApiProblem.NotFound("Session not found", $"id={sessionId}"),
                    "seat_invalid" => ApiProblem.Bad("Seat not in session", dto.SeatLabel),
                    "seat_taken" => ApiProblem.Conflict("Seat already booked", dto.SeatLabel),
                    _ => ApiProblem.Internal()
                };
        }

        [HttpGet("/api/bookings/{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var b = await _bookingService.GetAsync(id);
            return b == null
                ? ApiProblem.NotFound("Booking not found", $"id = {id}")
                : Ok(b);
        }

        [HttpDelete("/api/bookings/{id:long}")]
        public async Task<IActionResult> Cancel(long id)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return ApiProblem.Unauthorized("Unauthorized");

            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return await _bookingService.CancelAsync(id, userId)
                ? NoContent()
                : ApiProblem.Forbidden("Cannot cancel booking",
                                       "Booking not yours or not found");
        }
    }
}