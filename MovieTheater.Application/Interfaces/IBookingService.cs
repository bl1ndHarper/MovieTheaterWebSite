using MovieTheater.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Interfaces
{
    public interface IBookingService
    {
        Task<(bool ok, string? error, BookingDetailsDto? result)> CreateAsync(BookingDto dto);
        Task<BookingDetailsDto?> GetAsync(long id);
        Task<bool> CancelAsync(long id, long userId);
    }
}
