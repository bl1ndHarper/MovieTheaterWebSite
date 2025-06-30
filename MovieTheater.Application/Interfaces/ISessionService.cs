using MovieTheater.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Interfaces
{
    public interface ISessionService
    {
        Task<Dictionary<DateTime, List<MovieSessionDto>>> GetMovieSessionsByDayAsync(long movieId, string selectedDate);
        Task<List<string>> GetAvailableSessionDatesAsync(long movieId);
        Task<List<SessionSeatDto>> GetSeatsBySessionIdAsync(long sessionId);
    }
}
