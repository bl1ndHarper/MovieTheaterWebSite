using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Enums;
using MovieTheater.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISessionService _sessionService;
        public BookingService(IBookingRepository bookingRepository, ISessionService sessionService)
        {
            _bookingRepository = bookingRepository; _sessionService = sessionService;
        }

        public async Task<(bool ok, string? error, BookingDetailsDto? result)> CreateAsync(BookingDto dto)
        {
            if (await _sessionService.GetSessionByIdAsync(dto.SessionId) is null)
                return (false, "session_not_found", null);

            var seat = await _sessionService.GetSessionSeatAsync(dto.SessionId, dto.SeatLabel);
            if (seat == null)
                return (false, "seat_invalid", null);

            if (seat.Status is SeatStatus.Reserved or SeatStatus.Unavailable)
                return (false, "seat_taken", null);

            seat.Status = SeatStatus.Reserved;

            var booking = new Booking
            {
                UserId = dto.UserId,
                ScreeningId = dto.SessionId,
                SessionSeatId = seat.Id,
                Status = BookingStatus.Booked,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveAsync();

            return (true, null, new BookingDetailsDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                SessionId = booking.ScreeningId,
                SessionSeatId = booking.SessionSeatId,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            });
        }

        public async Task<BookingDetailsDto?> GetAsync(long id)
        {
            var b = await _bookingRepository.GetAsync(id);
            return b is null ? null : new BookingDetailsDto
            {
                Id = b.Id,
                UserId = b.UserId,
                SessionId = b.ScreeningId,
                SessionSeatId = b.SessionSeatId,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            };
        }

        public async Task<bool> CancelAsync(long id, long userId)
        {
            var b = await _bookingRepository.GetAsync(id);
            if (b == null || b.UserId != userId) return false;

            b.Status = BookingStatus.Cancelled;
            await _bookingRepository.SaveAsync();
            return true;
        }
    }
}
