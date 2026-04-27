using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;

namespace AquariumManager.Application.Services;

public interface IInventoryLotService
{
    Task<InventoryLotDto> CreateLotAsync(CreateInventoryLotDto dto);
    Task<InventoryLotDto?> GetByIdAsync(int id);
    Task<InventoryLot?> GetLotEntityByIdAsync(int id);
    Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesAsync(int speciesId);
    Task RegisterMortalityAsync(RegisterMortalityDto dto);

    Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesAsync(int speciesId);
    Task<IReadOnlyList<InventoryLotDto>> GetAllAsync();
}
