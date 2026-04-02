using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;


public class CatalogService : ICatalogService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IInventoryLotService _inventoryLotService;

    public CatalogService(
        ISpeciesRepository speciesRepository,
        IInventoryLotService inventoryLotService)
    {
        _speciesRepository = speciesRepository;
        _inventoryLotService = inventoryLotService;
    }

    public async Task<IReadOnlyList<CatalogItemDto>> GetCatalogAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync();

        var result = new List<CatalogItemDto>();

        foreach (var species in speciesList)
        {
            var stock = await _inventoryLotService
                .GetBiologicalStockDtoBySpeciesAsync(species.Id);

            var currentStock = stock?.CurrentBiologicalStock ?? 0;

            // Si quieres solo especies con stock > 0, puedes filtrar aquí.
             if (currentStock <= 0) continue;

            result.Add(new CatalogItemDto
            {
                SpeciesId = species.Id,
                CommonName = species.CommonName,
                Variety = species.Variety,
                CurrentBiologicalStock = currentStock,
                MinPH = species.MinPH,
                MaxPH = species.MaxPH,
                MinTemperature = species.MinTemperature,
                MaxTemperature = species.MaxTemperature,
                ImageUrl = species.ImageUrl
            });
        }

        return result;
    }
}
