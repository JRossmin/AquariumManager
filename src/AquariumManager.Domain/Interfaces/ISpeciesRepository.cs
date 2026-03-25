using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISpeciesRepository
{
    Task<Species?> GetByIdAsync(int id);
    Task<IReadOnlyList<Species>> GetAllAsync();
    Task<Species> AddAsync(Species species);
    Task UpdateAsync(Species species);
    Task DeleteAsync(int id);
}
