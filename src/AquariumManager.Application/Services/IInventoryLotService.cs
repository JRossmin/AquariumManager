using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IInventoryLotService
{
    Task<InventoryLotDto> CreateLotAsync(CreateInventoryLotDto dto);
    Task<InventoryLotDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesAsync(int speciesId);
    Task RegisterMortalityAsync(RegisterMortalityDto dto);

    Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesAsync(int speciesId);
    
}
