using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace MovieTheater.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieService(IMovieRepository movieRepository, IHttpClientFactory httpClientFactory)
        {
            _movieRepository = movieRepository;
            _httpClientFactory = httpClientFactory;
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

        public async Task<List<MovieMainDto>> GetNowShowingAsync(string day)
        {
            var date = DateTime.SpecifyKind(
                DateTime.ParseExact(
                    day + "." + DateTime.UtcNow.Year,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture),
                DateTimeKind.Utc);

            var sessions = await _movieRepository.GetNowShowingSessionsAsync(date);
            return sessions
                .GroupBy(s => s.MovieId)
                .Select(g =>
                {
                    var mv = g.First().Movie;
                    return new MovieMainDto
                    {
                        Id = mv.Id,
                        Title = mv.Title,
                        ThumbnailUrl = mv.ThumbnailUrl,
                        Genre = mv.Genres.FirstOrDefault()?.Genre.Name,
                        AgeRating = mv.AgeRating.Label,
                        Duration = mv.Duration,
                        Sessions = g.Select(s => s.StartTime.ToString("HH:mm")).ToList()
                    };
                })
                .ToList();
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
                DirectorName = m.DirectorName,
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
                Genres = movie.Genres.ToList(),
                Actors = movie.Actors.ToList(),
                DirectorName = movie.DirectorName,
                DirectorDetailsUrl = movie.DirectorDetailsUrl,
                ReleaseDate = movie.ReleaseDate
            };
        }


        public async Task<List<TmdbUpcomingMovie>> GetUpcomingMovies()
        {
            var client = _httpClientFactory.CreateClient("tmdbApi");
            var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");
            var language = "uk-UA";

            var genreResponse = await client.GetAsync($"genre/movie/list?api_key={apiKey}&language={language}");
            var genreJson = await genreResponse.Content.ReadAsStringAsync();
            var genreResult = JsonSerializer.Deserialize<TmdbGenreResponse>(genreJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var genreMap = genreResult?.Genres.ToDictionary(g => g.Id, g => g.Name);

            var response = await client.GetAsync($"movie/upcoming?api_key={apiKey}&language={language}&page=1");
            
            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var tmdbResponse = JsonSerializer.Deserialize<TmdbUpcomingMoviesResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var movies = tmdbResponse?.Results ?? new();

            foreach (var movie in movies)
            {
                movie.GenreNames = string.Join(", ",
                    movie.GenreIds.Select(id => genreMap.TryGetValue(id, out var name) ? name : ""));
            }

            return movies;
        }
    }
}