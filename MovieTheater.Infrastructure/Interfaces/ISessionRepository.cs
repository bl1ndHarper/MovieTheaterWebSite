using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Interfaces
{
    public interface ISessionRepository
    {
        Task<List<Session>> GetMovieSessionsByDateAsync(long movieId, DateTime date);
        Task<List<DateTime>> GetAvailableSessionDatesAsync(long movieId);
        Task<SessionSeat> GetSessionSeatByLabelAsync(long sessionId, string label);
        Task<Session?> GetByIdAsync(long id);
        Task AddAsync(Session session);
        void Remove(Session session);
        Task AddSessionsAsync(List<Session> sessions);
        Task<bool> SessionExistsAsync(DateTime startTime, int hallId, int movieId);
        Task SaveAsync();
    }
}
