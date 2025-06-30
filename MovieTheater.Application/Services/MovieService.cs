using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovieMainDto>> GetNowShowingAsync(DateTime date)
        {
            var sessions = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Include(s => s.Movie.AgeRating)
                .Include(s => s.Movie.Genres)
                    .ThenInclude(mg => mg.Genre)
                .ToListAsync();

            var grouped = sessions.GroupBy(s => s.MovieId);

            return grouped.Select(group =>
            {
                var movie = group.First().Movie;
                return new MovieMainDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ThumbnailUrl = movie.ThumbnailUrl,
                    Genre = movie.Genres.FirstOrDefault()?.Genre.Name,
                    AgeRating = movie.AgeRating.Label,
                    Duration = movie.Duration,
                    Sessions = group.Select(s => s.StartTime.ToString("HH:mm")).ToList()
                };
            }).ToList();
        }

        public async Task<List<MovieMainDto>> GetLatestMoviesAsync(int count)
        {
            var movies = await _context.Movies
                .Include(m => m.AgeRating)
                .Include(m => m.Genres).ThenInclude(mg => mg.Genre)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(count)
                .ToListAsync();

            return movies.Select(m => new MovieMainDto
            {
                Id = m.Id,
                Title = m.Title,
                ThumbnailUrl = m.ThumbnailUrl,
                Genre = m.Genres.FirstOrDefault()?.Genre.Name,
                AgeRating = m.AgeRating.Label,
                Duration = m.Duration,
                ImdbRating = m.ImdbRating,
                Sessions = new List<string>()
            }).ToList();
        }

        public async Task<MovieDto?> GetMovieByIdAsync(long id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genres).ThenInclude(mg => mg.Genre)
                .Include(m => m.AgeRating)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is null) return null;

            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Duration = movie.Duration,
                ImdbRating = movie.ImdbRating,
                ThumbnailUrl = movie.ThumbnailUrl,
                AgeRatingLabel = movie.AgeRating.Label,
                MinAgeRating = movie.AgeRating.MinAge,
                ReleaseYear = movie.ReleaseDate.Year,
                Description = movie.Description,
                MainGenre = movie.Genres.FirstOrDefault()?.Genre,
                Genres = movie.Genres.ToList()
            };
        }
    }
}