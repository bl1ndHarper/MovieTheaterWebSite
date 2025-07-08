using MovieTheater.Infrastructure.Entities;

public interface IHallRepository
{
    Task<IEnumerable<Hall>> GetAllHallsWithSectorsAsync();
    Task<HallSector?> GetSectorByIdAsync(long sectorId);
    Task AddSectorAsync(HallSector sector);
    Task SaveAsync();
    Task<Hall?> GetHallWithSectorsAndSeatsAsync(long hallId);
    Task DeleteSectorAsync(HallSector sector);
}
