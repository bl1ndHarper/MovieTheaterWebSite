using Microsoft.EntityFrameworkCore;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Application.Services
{
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;

        public HallService(IHallRepository hallRepository)
        {
            _hallRepository = hallRepository;
        }

        public async Task<IEnumerable<Hall>> GetAllHallsWithSectorsAsync()
        {
            return await _hallRepository.GetAllHallsWithSectorsAsync();
        }

        public async Task<bool> UpdateSectorPriceAsync(long sectorId, decimal newPrice)
        {
            var sector = await _hallRepository.GetSectorByIdAsync(sectorId);
            if (sector == null)
                return false;

            sector.SeatPrice = decimal.Round(newPrice, 2);
            await _hallRepository.SaveAsync();

            return true;
        }

        public async Task AddSectorAsync(long hallId, string name, decimal price)
        {
            var newSector = new HallSector
            {
                HallId = hallId,
                Name = name,
                SeatPrice = decimal.Round(price, 2)
            };

            await _hallRepository.AddSectorAsync(newSector);
            await _hallRepository.SaveAsync();
        }

        public async Task<DeleteSectorResult> DeleteSectorAsync(long hallId, long sectorId)
        {
            var hall = await _hallRepository.GetHallWithSectorsAndSeatsAsync(hallId);
            if (hall == null)
                return DeleteSectorResult.NotFound;

            var sectorToDelete = hall.Sectors.FirstOrDefault(s => s.Id == sectorId);
            if (sectorToDelete == null)
                return DeleteSectorResult.NotFound;

            if (hall.Sectors.Count <= 1)
                return DeleteSectorResult.OnlyOneSectorLeft;

            var alternativeSector = hall.Sectors.FirstOrDefault(s => s.Id != sectorId);
            if (alternativeSector == null)
                return DeleteSectorResult.ReassignFailed;

            try
            {
                foreach (var seat in sectorToDelete.Seats)
                {
                    seat.SectorId = alternativeSector.Id;
                }

                await _hallRepository.DeleteSectorAsync(sectorToDelete);
                await _hallRepository.SaveAsync();

                return DeleteSectorResult.Success;
            }
            catch
            {
                return DeleteSectorResult.ReassignFailed;
            }
        }
    }

    public enum DeleteSectorResult
    {
        Success,
        NotFound,
        OnlyOneSectorLeft,
        ReassignFailed
    }
}
