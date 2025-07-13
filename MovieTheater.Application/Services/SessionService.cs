using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using System;
using Microsoft.Extensions.Logging; 
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieTheater.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<SessionService> _logger;


        public SessionService(ISessionRepository sessionRepository, IMovieRepository movieRepository, ILogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _movieRepository = movieRepository;
            _logger = logger;
        }
        public async Task<MovieSessionDto?> GetSessionByIdAsync(long id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null) return null;

            return new MovieSessionDto
            {
                Id = session.Id,
                MovieId = session.MovieId,
                StartTime = session.StartTime,
                Hall = session.Hall,
                SeatsTotal = session.SeatsTotal
            };
        }

        public async Task<SessionSeat?> GetSessionSeatAsync(long sessionId, string label)
        {
            SessionSeat seat = await _sessionRepository.GetSessionSeatByLabelAsync(sessionId, label);
            return seat;
        }

        public async Task<MovieSessionDto> CreateSessionAsync(MovieSessionDto dto)
        {
            var session = new Session
            {
                MovieId = dto.MovieId,
                StartTime = dto.StartTime,
                HallId = dto.Hall.Id,
                SeatsTotal = dto.SeatsTotal
            };

            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveAsync();

            return new MovieSessionDto { Id = session.Id, MovieId = session.MovieId };
        }

        public async Task<bool> CreateSessionsAsync(List<SessionCreateDto> dtos)
        {
             try
            {
                var validSessions = new List<Session>();
                foreach (var dto in dtos)
                {
                    
                    _logger.LogDebug("Checking if session exists: StartTime={StartTime}, HallId={HallId}, MovieId={MovieId}", dto.StartTime, dto.HallId, dto.MovieId);

                    var exists = await _sessionRepository.SessionExistsAsync(dto.StartTime, dto.HallId, dto.MovieId);
                    if (!exists)
                    {
                        validSessions.Add(new Session
                        {
                            StartTime = dto.StartTime,
                            HallId = dto.HallId,
                            MovieId = dto.MovieId
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Skipping duplicate session: StartTime={StartTime}, HallId={HallId}, MovieId={MovieId}", dto.StartTime, dto.HallId, dto.MovieId);
                    }
                }

                if (validSessions.Any())
                {
                    _logger.LogInformation("Attempting to add {Count} new sessions.", validSessions.Count);
                    await _sessionRepository.AddSessionsAsync(validSessions);
                    await _sessionRepository.SaveAsync();
                    _logger.LogInformation("Successfully added and saved sessions.");
                    return true;
                }
                _logger.LogInformation("No new sessions to add.");
                return false;
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to create sessions due to an unhandled exception in SessionService.");
                throw; 
            }
        }

        public async Task<bool> DeleteSessionAsync(long id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null) return false;

            _sessionRepository.Remove(session);
            await _sessionRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateSessionAsync(long id, MovieSessionDto dto)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null) return false;

            session.StartTime = dto.StartTime;
            session.MovieId = dto.MovieId;
            session.StartTime = dto.StartTime;
            session.HallId = dto.Hall.Id;
            session.SeatsTotal = dto.SeatsTotal;

            await _sessionRepository.SaveAsync();
            return true;
        }

        public async Task<List<MovieSessionDto>> CreateSessionsDailyAsync(SessionDailyCreateDto dto)
        {
            var sessions = new List<Session>();
            for (var date = dto.StartDate.Date; date <= dto.EndDate.Date; date = date.AddDays(1))
            {
                var session = new Session
                {
                    MovieId = dto.MovieId,
                    StartTime = date.Add(dto.StartTime.TimeOfDay),
                    HallId = dto.Hall.Id,
                    SeatsTotal = dto.SeatsTotal,
                    Hall = dto.Hall,
                    Movie = await _movieRepository.GetMovieWithDetailsByIdAsync(dto.MovieId),
                };

                sessions.Add(session);
                await _sessionRepository.AddAsync(session);
            }

            await _sessionRepository.SaveAsync();
            return sessions.Select(s => new MovieSessionDto { Id = s.Id, MovieId = s.MovieId }).ToList();
        }

        public async Task<Dictionary<DateTime, List<MovieSessionDto>>> GetMovieSessionsByDayAsync(long movieId, string selectedDate)
        {
            var parsedDate = DateTime.SpecifyKind(
                DateTime.ParseExact(selectedDate + "." + DateTime.Now.Year, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                DateTimeKind.Utc
            );

            var sessions = await _sessionRepository.GetMovieSessionsByDateAsync(movieId, parsedDate);

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
            var rawDates = await _sessionRepository.GetAvailableSessionDatesAsync(movieId);

            return rawDates
                .Select(d => d.ToString("dd.MM"))
                .ToList();
        }

        public async Task<List<SessionSeatDto>> GetSeatsBySessionIdAsync(long sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
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