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
        Task<MovieSessionDto> GetSessionByIdAsync(long id);
        Task<MovieSessionDto> CreateSessionAsync(MovieSessionDto dto);
        Task<bool> DeleteSessionAsync(long id);
        Task<bool> UpdateSessionAsync(long id, MovieSessionDto dto);
        Task<List<MovieSessionDto>> CreateSessionsDailyAsync(SessionDailyCreateDto dto);
        Task<Dictionary<DateTime, List<MovieSessionDto>>> GetMovieSessionsByDayAsync(long movieId, string selectedDate);
        Task<List<string>> GetAvailableSessionDatesAsync(long movieId);
        Task<List<SessionSeatDto>> GetSeatsBySessionIdAsync(long sessionId);
    }
}
