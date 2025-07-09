using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using MovieTheater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTheater.Domain.DTOs;
using MovieTheater.Infrastructure.Enums;

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

        public async Task SaveMovieFromApiAsync(TmdbMovieDto dto)
        {
            var genreNames = dto.Genres.Select(g => g.Name).Distinct().ToList();
            var existingGenres = await GetGenresByNamesAsync(genreNames);

            var newGenres = genreNames
                .Where(name => existingGenres.All(g => g.Name != name))
                .Select(name => new Genre { Name = name })
                .ToList();

            if (newGenres.Any())
            {
                await AddGenresAsync(newGenres);
                await SaveAsync();
                existingGenres.AddRange(newGenres);
            }

            var ratingLabel = dto.Adult ? "NC-17" : "PG-13";
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.Label == ratingLabel)
                ?? await _context.Ratings.FirstOrDefaultAsync(r => r.Label == "PG-13");

            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Overview,
                ThumbnailUrl = $"https://image.tmdb.org/t/p/w500{dto.Poster_Path}",
                ReleaseDate = DateTime.TryParse(dto.Release_Date, out var rd) ? rd : DateTime.MinValue,
                ImdbRating = (decimal)Math.Round(dto.Vote_Average, 1),
                Duration = (short?)dto.Runtime,
                ActivityStatus = ActivityStatus.Upcoming,
                AgeRating = rating,
                DirectorName = ""
            };

            await AddAsync(movie);
            await SaveAsync();

            var genresToLink = existingGenres
                .Select(g => new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = g.Id
                }).ToList();

            await AddMovieGenresAsync(genresToLink);
            await SaveAsync();
        }

        public async Task<List<Genre>> GetGenresByNamesAsync(List<string> names) =>
            await _context.Genres.Where(g => names.Contains(g.Name)).ToListAsync();

        public async Task AddGenresAsync(IEnumerable<Genre> genres)
        {
            await _context.Genres.AddRangeAsync(genres);
        }

        public async Task AddMovieGenresAsync(IEnumerable<MovieGenre> movieGenres)
        {
            await _context.MovieGenres.AddRangeAsync(movieGenres);
        }

        public async Task<bool> ExistsByTitleAndYearAsync(string title, int year)
        {
            return await _context.Movies.AnyAsync(m =>
                m.Title == title &&
                m.ReleaseDate.Year == year);
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
