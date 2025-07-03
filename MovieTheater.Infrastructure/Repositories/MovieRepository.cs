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
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetLatestMoviesAsync(int count)
        {
            return await _context.Movies
                .Include(m => m.AgeRating)
                .Include(m => m.Genres)
                    .ThenInclude(g => g.Genre)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Session>> GetNowShowingSessionsAsync(DateTime dateUtc)
        {
            var start = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);

            return await _context.Sessions
                .Include(s => s.Movie).ThenInclude(m => m.AgeRating)
                .Include(s => s.Movie).ThenInclude(m => m.Genres).ThenInclude(g => g.Genre)
                .Include(s => s.Hall)
                .Where(s => s.StartTime >= start && s.StartTime < end)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieWithDetailsByIdAsync(long id)
        {
            return await _context.Movies
                .Include(m => m.Genres).ThenInclude(g => g.Genre)
                .Include(m => m.AgeRating)
                .Include(m => m.Actors).ThenInclude(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<Movie?> GetByIdAsync(long id) =>
            _context.Movies.FindAsync(id).AsTask();

        public Task AddAsync(Movie movie) =>
            _context.Movies.AddAsync(movie).AsTask();

        public void Remove(Movie movie) =>
            _context.Movies.Remove(movie);

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
