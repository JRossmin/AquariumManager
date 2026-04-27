using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AquariumDbContext _context;

    public SaleRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        _context.Sales.Add(sale);
    }

  
}
