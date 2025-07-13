using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Session?> GetByIdAsync(long id)
        {
            return await _context.Sessions
                .Include(s => s.SessionSeats)
                    .ThenInclude(ss => ss.HallSeat)
                        .ThenInclude(hs => hs.Sector)
                .Include(s => s.Movie)
                .Include(s => s.SessionSeats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Session>> GetMovieSessionsByDateAsync(long movieId, DateTime date)
        {
            var nextDay = date.Date.AddDays(1);

            return await _context.Sessions
                .Where(s => s.MovieId == movieId &&
                            s.StartTime >= date &&
                            s.StartTime < nextDay)
                .Include(s => s.Hall)
                .ToListAsync();
        }

        public async Task<List<DateTime>> GetAvailableSessionDatesAsync(long movieId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Sessions
                .Where(s => s.MovieId == movieId && s.StartTime.Date >= today)
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<SessionSeat?> GetSessionSeatByLabelAsync(long sessionId, string label)
        {
            var seat = await _context.SessionSeats
                .Include(s => s.HallSeat).ThenInclude(hs => hs.Sector)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.HallSeat.Label == label);
            return seat;
        }

        public Task AddAsync(Session session) =>
            _context.Sessions.AddAsync(session).AsTask();

        public async Task AddSessionsAsync(List<Session> sessions)
        {
        await _context.Sessions.AddRangeAsync(sessions);
        }

        public async Task<bool> SessionExistsAsync(DateTime startTime, int hallId, int movieId)
        {
            return await _context.Sessions.AnyAsync(s =>
                s.StartTime == startTime &&
                s.HallId == hallId &&
                s.MovieId == movieId);
        }

        public void Remove(Session session) =>
            _context.Sessions.Remove(session);

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
