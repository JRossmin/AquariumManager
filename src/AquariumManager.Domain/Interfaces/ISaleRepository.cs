using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id);
    Task<IReadOnlyList<Sale>> GetAllAsync();
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(Sale sale);
}
