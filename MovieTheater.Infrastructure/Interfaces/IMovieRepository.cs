using MovieTheater.Infrastructure.Entities;
using MovieTheater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTheater.Domain.DTOs;

namespace MovieTheater.Infrastructure.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetLatestMoviesAsync(int count);
        Task<List<Session>> GetNowShowingSessionsAsync(DateTime date);
        Task<Movie?> GetMovieWithDetailsByIdAsync(long id);
        Task<Movie?> GetByIdAsync(long id);
        Task AddAsync(Movie movie);
        void Remove(Movie movie);
        Task SaveAsync();
        Task SaveMovieFromApiAsync(TmdbMovieDto dto);
        Task<List<Genre>> GetGenresByNamesAsync(List<string> names);
        Task AddGenresAsync(IEnumerable<Genre> genres);
        Task AddMovieGenresAsync(IEnumerable<MovieGenre> movieGenres);
        Task<bool> ExistsByTitleAndYearAsync(string title, int year);

        Task<List<Session>> GetSessionsBeforeAsync(DateTime nowUtc, int daysBack);
        Task<List<Session>> GetSessionsAfterAsync(DateTime nowUtc);
        Task<List<Movie>> GetAllMoviesForListAsync();
    }
}
