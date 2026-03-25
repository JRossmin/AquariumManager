using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly AquariumDbContext _context;

    public SpeciesRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Species?> GetByIdAsync(int id)
    {
        return await _context.Species
            .Include(s => s.InventoryItems) // legacy
            .Include(s => s.InventoryLots)  // nuevo modelo
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Species>> GetAllAsync()
    {
        return await _context.Species
            .OrderBy(s => s.CommonName)
            .ToListAsync();
    }

    public async Task<Species> AddAsync(Species species)
    {
        _context.Species.Add(species);
        await _context.SaveChangesAsync();
        return species;
    }

    public async Task UpdateAsync(Species species)
    {
        _context.Species.Update(species);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var species = await _context.Species.FindAsync(id);
        if (species is null)
        {
            return;
        }

        _context.Species.Remove(species);
        await _context.SaveChangesAsync();
    }
}
