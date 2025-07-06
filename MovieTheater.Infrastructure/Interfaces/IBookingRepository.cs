using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetAsync(long id);
        Task AddAsync(Booking booking);
        Task<bool> SeatAlreadyBookedAsync(long sessionSeatId);
        Task SaveAsync();
        void Remove(Booking booking);
    }
}
