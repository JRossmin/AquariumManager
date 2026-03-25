using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IInventoryLotRepository
{
    Task<InventoryLot?> GetByIdAsync(int id);
    Task<IReadOnlyList<InventoryLot>> GetBySpeciesAsync(int speciesId);
    Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesAsync(int speciesId);
    Task AddAsync(InventoryLot lot);
    Task UpdateAsync(InventoryLot lot);
}
