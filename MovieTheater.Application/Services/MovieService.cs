using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<MovieMainDto> CreateMovieAsync(MovieCreateDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                Duration = dto.Duration,
                TrailerUrl = dto.TrailerUrl,
                ThumbnailUrl = dto.ThumbnailUrl,
                ReleaseDate = dto.ReleaseDate,
                ImdbRating = dto.ImdbRating,
                ActivityStatus = dto.ActivityStatus,
                AgeRatingId = dto.AgeRatingId
            };

            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveAsync();

            return new MovieMainDto { Id = movie.Id, Title = movie.Title };
        }

        public async Task<bool> UpdateMovieAsync(long id, MovieUpdateDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return false;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.Duration = dto.Duration;
            movie.TrailerUrl = dto.TrailerUrl;
            movie.ThumbnailUrl = dto.ThumbnailUrl;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.ImdbRating = dto.ImdbRating;
            movie.ActivityStatus = dto.ActivityStatus;
            movie.AgeRatingId = dto.AgeRatingId;

            await _movieRepository.SaveAsync();
            return true;
        }

        public async Task<bool> PatchMovieAsync(long id, JsonPatchDocument<MovieUpdateDto> patch)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return false;

            var dto = new MovieUpdateDto
            {
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                TrailerUrl = movie.TrailerUrl,
                ThumbnailUrl = movie.ThumbnailUrl,
                ReleaseDate = movie.ReleaseDate,
                ImdbRating = movie.ImdbRating,
                ActivityStatus = movie.ActivityStatus,
                AgeRatingId = movie.AgeRatingId
            };

            patch.ApplyTo(dto);

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.Duration = dto.Duration;
            movie.TrailerUrl = dto.TrailerUrl;
            movie.ThumbnailUrl = dto.ThumbnailUrl;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.ImdbRating = dto.ImdbRating;
            movie.ActivityStatus = dto.ActivityStatus;
            movie.AgeRatingId = dto.AgeRatingId;

            await _movieRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteMovieAsync(long id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return false;

            _movieRepository.Remove(movie);
            await _movieRepository.SaveAsync();
            return true;
        }

        public async Task<List<MovieMainDto>> GetNowShowingAsync(DateTime date)
        {
            var sessions = await _movieRepository.GetNowShowingSessionsAsync(date);

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
            var movies = await _movieRepository.GetLatestMoviesAsync(count);

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
        var movie = await _movieRepository.GetMovieWithDetailsByIdAsync(id);
        if (movie == null) return null;

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