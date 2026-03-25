using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class InventoryLotRepository : IInventoryLotRepository
{
    private readonly AquariumDbContext _context;

    public InventoryLotRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryLot?> GetByIdAsync(int id)
    {
        return await _context.InventoryLots
            .Include(l => l.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IReadOnlyList<InventoryLot>> GetBySpeciesAsync(int speciesId)
    {
        return await _context.InventoryLots
            .Include(l => l.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.SpeciesId == speciesId)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesAsync(int speciesId)
    {
        // Importante: GetCurrentStock() es método de dominio; EF no puede traducirlo a SQL,
        // por eso primero traemos los lotes y luego filtramos en memoria.
        var lots = await _context.InventoryLots
            .Include(l => l.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.SpeciesId == speciesId)
            .OrderBy(l => l.ArrivalDate)
            .ToListAsync();

        return lots
            .Where(l => l.GetCurrentStock() > 0)
            .ToList();
    }

    public async Task AddAsync(InventoryLot lot)
    {
        _context.InventoryLots.Add(lot);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryLot lot)
    {
        _context.InventoryLots.Update(lot);
        await _context.SaveChangesAsync();
    }
}
