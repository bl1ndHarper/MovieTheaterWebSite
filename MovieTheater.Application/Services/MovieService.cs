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
using System.Net.Http.Json;
using MovieTheater.Infrastructure.Enums;
using MovieTheater.Domain.DTOs;

namespace MovieTheater.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

        public MovieService(
            IMovieRepository movieRepository,
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext dbContext)
        {
            _movieRepository = movieRepository;
            _httpClient = httpClientFactory.CreateClient("tmdbApi");
            _dbContext = dbContext;
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
            List<Session> sessions;

            var date = DateTime.SpecifyKind(
                DateTime.ParseExact(
                    day + "." + DateTime.UtcNow.Year,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture),
                DateTimeKind.Utc);

            sessions = await _movieRepository.GetNowShowingSessionsAsync(date);

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
                        Sessions = g.Select(s => s.StartTime.ToLocalTime().ToString("HH:mm")).ToList()
                    };
                })
                .ToList();
        }

        public async Task<List<MovieSessionsByDateDto>> GetGroupedByDateAsync(string mode, int daysBack = 7)
        {
            var now = DateTime.UtcNow;
            List<Session> sessions;

            if (mode == "past")
                sessions = await _movieRepository.GetSessionsBeforeAsync(now, daysBack);
            else if (mode == "future")
                sessions = await _movieRepository.GetSessionsAfterAsync(now);
            else
                return new(); // fallback

            var groupedByDate = sessions
                .GroupBy(s => s.StartTime.Date)
                .OrderBy(g => mode == "past" ? -g.Key.Ticks : g.Key.Ticks)
                .ToList();

            var result = new List<MovieSessionsByDateDto>();

            foreach (var group in groupedByDate)
            {
                var movies = group
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
                            Sessions = g
                                .OrderBy(s => s.StartTime)
                                .Select(s => s.StartTime.ToLocalTime().ToString("HH:mm"))
                                .ToList()
                        };
                    })
                    .ToList();

                result.Add(new MovieSessionsByDateDto
                {
                    Date = group.Key,
                    Movies = movies
                });
            }

            return result;
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
            var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");
            var language = "uk-UA";

            var genreResponse = await _httpClient.GetAsync($"genre/movie/list?api_key={apiKey}&language={language}");
            var genreJson = await genreResponse.Content.ReadAsStringAsync();
            var genreResult = JsonSerializer.Deserialize<TmdbGenreResponse>(genreJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var genreMap = genreResult?.Genres.ToDictionary(g => g.Id, g => g.Name);

            var response = await _httpClient.GetAsync($"movie/upcoming?api_key={apiKey}&language={language}&page=1");

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

        public async Task<List<MovieSearchResultDto>> SearchMoviesAsync(string query)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&language=uk-UA";
            var res = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);

            return res?.Results
                .Select(r =>
                {
                    string pattern = "yyyy-MM-dd";
                    string year = !string.IsNullOrWhiteSpace(r.Release_Date)
                        ? DateTime.ParseExact(r.Release_Date, "yyyy-MM-dd", null).Year.ToString()
                        : "Невідомо"; // або "", або null

                    return new MovieSearchResultDto
                    {
                        TmdbId = r.Id,
                        Title = r.Title,
                        ReleaseDate = year
                    };
                })
                .ToList() ?? new();
        }

        public async Task<MoviePreviewDto?> GetMovieDetailsFromApiAsync(int tmdbId)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={_apiKey}&language=uk-UA&append_to_response=credits");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var movieDto = JsonSerializer.Deserialize<TmdbMovieDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (movieDto == null)
                return null;

            var director = movieDto.Credits?.Crew?.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Невідомо";
            var actors = movieDto.Credits?.Cast?
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .Take(5)
                    .Select(c => c.Name)
                    .ToList() ?? new();

            return new MoviePreviewDto
            {
                Title = movieDto.Title,
                Description = movieDto.Overview,
                ReleaseDate = DateTime.TryParse(movieDto.Release_Date, out var rd) ? rd : DateTime.MinValue,
                ImdbRating = (decimal)Math.Round(movieDto.Vote_Average, 1),
                Duration = (short?)movieDto.Runtime,
                ThumbnailUrl = $"https://image.tmdb.org/t/p/w500{movieDto.Poster_Path}",
                DirectorName = director,
                ActorNames = actors,
                Genres = movieDto.Genres.Select(g => new MovieGenre
                {
                    Genre = new Genre { Name = g.Name }
                }).ToList()
            };
        }

        public async Task<bool> AddMovieFromApiAsync(int tmdbId)
        {
            var movieDto = await GetMovieDetailsFromApiAsync(tmdbId);
            if (movieDto == null)
                return false;

            var alreadyExists = await _movieRepository.ExistsByTitleAndYearAsync(movieDto.Title, movieDto.ReleaseDate.Year);
            if (alreadyExists)
                return false;

            TmdbMovieDto tmdbDto = new()
            {
                Title = movieDto.Title,
                Overview = movieDto.Description,
                Poster_Path = movieDto.ThumbnailUrl.Replace("https://image.tmdb.org/t/p/w500", ""),
                Release_Date = movieDto.ReleaseDate.ToString("yyyy-MM-dd"),
                Vote_Average = (double)movieDto.ImdbRating,
                Runtime = movieDto.Duration ?? 0,
                Adult = movieDto.AgeRatingLabel == "NC-17",
                Genres = movieDto.Genres.Select(g => new TmdbGenreDto { Name = g.Genre.Name }).ToList()
            };

            await _movieRepository.SaveMovieFromApiAsync(tmdbDto);
            return true;
        }

        public async Task<bool> SaveMovieFromDtoAsync(MovieSaveDto dto)
        {
            if (await _movieRepository.ExistsByTitleAndYearAsync(dto.Title, dto.ReleaseDate.Year))
                return false;

            var existingGenres = await _movieRepository.GetGenresByNamesAsync(dto.Genres);

            var newGenres = dto.Genres
                .Where(name => !existingGenres.Any(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(name => new Genre { Name = name })
                .ToList();

            if (newGenres.Any())
            {
                await _movieRepository.AddGenresAsync(newGenres);
                await _movieRepository.SaveAsync();
                existingGenres.AddRange(newGenres);
            }

            var ratingLabel = dto.Adult ? "NC-17" : "PG-13";
            var rating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.Label == ratingLabel)
                ?? await _dbContext.Ratings.FirstOrDefaultAsync(r => r.Label == "PG-13");

            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl,
                ReleaseDate = DateTime.SpecifyKind(dto.ReleaseDate, DateTimeKind.Utc),
                ImdbRating = dto.ImdbRating,
                Duration = dto.Duration,
                ActivityStatus = ActivityStatus.Upcoming,
                AgeRating = rating,
                DirectorName = dto.DirectorName,
                DirectorDetailsUrl = dto.DirectorDetailsUrl
            };

            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveAsync();

            var genreLinks = existingGenres.Select(g => new MovieGenre
            {
                MovieId = movie.Id,
                GenreId = g.Id
            }).ToList();

            await _movieRepository.AddMovieGenresAsync(genreLinks);
            await _movieRepository.SaveAsync();

            return true;
        }
        
        public async Task<List<MovieListItemDto>> GetAllMoviesAsync() 
        {
         
            var moviesFromDb = await _movieRepository.GetAllMoviesForListAsync();
            var movieListItemDtos = moviesFromDb
                .Select(movie => new MovieListItemDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ReleaseYear = movie.ReleaseDate.Year 
                })
                .ToList();

            return movieListItemDtos;
        }
        
        
    }
}