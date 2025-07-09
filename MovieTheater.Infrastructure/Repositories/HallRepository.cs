using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Data;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;

namespace MovieTheater.Infrastructure.Repositories
{
    public class HallRepository : IHallRepository
    {
        private readonly ApplicationDbContext _context;

        public HallRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hall>> GetAllHallsWithSectorsAsync()
        {
            return await _context.Halls
                .Include(h => h.Sectors.OrderBy(s => s.Id))
                .ToListAsync();
        }

        public async Task<Hall?> GetHallWithSectorsAndSeatsAsync(long hallId)
        {
            return await _context.Halls
                .Include(h => h.Sectors)
                    .ThenInclude(s => s.Seats)
                .FirstOrDefaultAsync(h => h.Id == hallId);
        }

        public async Task<HallSector?> GetSectorByIdAsync(long sectorId)
        {
            return await _context.HallSectors
                .FirstOrDefaultAsync(s => s.Id == sectorId);
        }

        public async Task AddSectorAsync(HallSector sector)
        {
            await _context.HallSectors.AddAsync(sector);
        }

        public async Task DeleteSectorAsync(HallSector sector)
        {
            _context.HallSectors.Remove(sector);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
