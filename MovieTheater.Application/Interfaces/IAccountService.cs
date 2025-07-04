
using MovieTheater.Application.DTOs;

namespace MovieTheater.Application.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, string? Error)> LoginAsync(LoginRequestDto dto);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterRequestDto dto);
    }
}