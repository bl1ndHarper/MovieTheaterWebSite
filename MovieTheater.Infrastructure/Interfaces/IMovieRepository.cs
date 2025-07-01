using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
