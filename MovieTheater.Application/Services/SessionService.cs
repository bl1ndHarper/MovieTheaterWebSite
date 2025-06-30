using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<DateTime, List<MovieSessionDto>>> GetMovieSessionsByDayAsync(long movieId, string selectedDate)
        {
            var parsedDate = DateTime.SpecifyKind(
                DateTime.ParseExact(selectedDate + "." + DateTime.Now.Year, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                DateTimeKind.Utc
            );
            var nextDay = parsedDate.AddDays(1);

            var sessions = await _context.Sessions
                .Where(s => s.MovieId == movieId &&
                            s.StartTime >= parsedDate &&
                            s.StartTime < nextDay)
                .Include(s => s.Hall)
                .ToListAsync();

            return sessions
                .GroupBy(s => DateOnly.FromDateTime(s.StartTime))
                .ToDictionary(
                    g => g.Key.ToDateTime(TimeOnly.MinValue),
                    g => g.Select(s => new MovieSessionDto
                    {
                        Id = s.Id,
                        MovieId = s.MovieId,
                        Hall = s.Hall,
                        StartTime = s.StartTime,
                        SeatsTotal = s.SeatsTotal
                    }).ToList()
                );
        }

        public async Task<List<string>> GetAvailableSessionDatesAsync(long movieId)
        {
            var rawDates = await _context.Sessions
                .Where(s => s.MovieId == movieId)
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return rawDates
                .Select(d => d.ToString("dd.MM"))
                .ToList();
        }

        public async Task<List<SessionSeatDto>> GetSeatsBySessionIdAsync(long sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.SessionSeats)
                    .ThenInclude(ss => ss.HallSeat)
                        .ThenInclude(hs => hs.Sector)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return new List<SessionSeatDto>();

            return session.SessionSeats.Select(ss => new SessionSeatDto
            {
                Label = ss.HallSeat.Label,
                SectorName = ss.HallSeat.Sector.Name,
                Price = ss.HallSeat.Sector.SeatPrice,
                Status = ss.Status
            }).ToList();
        }
    }
}