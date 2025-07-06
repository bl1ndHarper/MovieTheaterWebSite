using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Enums;
using MovieTheater.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingRepository(ApplicationDbContext context) => _context = context;

        public Task<Booking?> GetAsync(long id) =>
            _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

        public Task<bool> SeatAlreadyBookedAsync(long sessionSeatId) =>
            _context.Bookings.AnyAsync(b => b.SessionSeatId == sessionSeatId &&
                                        b.Status != BookingStatus.Cancelled);

        public async Task AddAsync(Booking b) => await _context.Bookings.AddAsync(b);

        public Task SaveAsync() => _context.SaveChangesAsync();

        public void Remove(Booking b) => _context.Bookings.Remove(b);
    }
}
