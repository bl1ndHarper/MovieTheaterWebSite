using MovieTheater.Application.Services;
using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Application.Interfaces
{
    public interface IHallService
    {
        Task<IEnumerable<Hall>> GetAllHallsWithSectorsAsync();
        Task<bool> UpdateSectorPriceAsync(long sectorId, decimal newPrice);
        Task<DeleteSectorResult> DeleteSectorAsync(long hallId, long sectorId);
        Task AddSectorAsync(long hallId, string name, decimal price);
    }
}
