using Microsoft.AspNetCore.JsonPatch;
using MovieTheater.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.Interfaces
{
    public interface IMovieService
    {
        Task<MovieMainDto> CreateMovieAsync(MovieCreateDto dto);
        Task<bool> UpdateMovieAsync(long id, MovieUpdateDto dto);
        Task<bool> PatchMovieAsync(long id, JsonPatchDocument<MovieUpdateDto> patch);
        Task<bool> DeleteMovieAsync(long id);
        Task<List<MovieMainDto>> GetNowShowingAsync(DateTime date);
        Task<List<MovieMainDto>> GetLatestMoviesAsync(int count);
        Task<MovieDto?> GetMovieByIdAsync(long id);
    }
}
