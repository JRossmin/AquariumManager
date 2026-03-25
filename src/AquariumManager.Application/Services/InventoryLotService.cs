using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class InventoryLotService : IInventoryLotService
{
    private readonly IInventoryLotRepository _lotRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISupplierRepository _supplierRepository;

    public InventoryLotService(
        IInventoryLotRepository lotRepository,
        ISpeciesRepository speciesRepository,
        ISupplierRepository supplierRepository)
    {
        _lotRepository = lotRepository;
        _speciesRepository = speciesRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<InventoryLotDto> CreateLotAsync(CreateInventoryLotDto dto)
    {
        var species = await _speciesRepository.GetByIdAsync(dto.SpeciesId)
                      ?? throw new InvalidOperationException("Species not found.");

        Supplier? supplier = null;
        if (dto.SupplierId.HasValue)
        {
            supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId.Value)
                       ?? throw new InvalidOperationException("Supplier not found.");
        }

        var lot = new InventoryLot(
            speciesId: dto.SpeciesId,
            arrivalDate: dto.ArrivalDate,
            initialQuantity: dto.InitialQuantity,
            deadOnArrival: dto.DeadOnArrival,
            unitCost: dto.UnitCost,
            supplierId: dto.SupplierId,
            batchNumber: dto.BatchNumber,
            notes: dto.Notes
        );

        await _lotRepository.AddAsync(lot);

        return MapToDto(lot, species, supplier);
    }

    public async Task<InventoryLotDto?> GetByIdAsync(int id)
    {
        var lot = await _lotRepository.GetByIdAsync(id);
        if (lot is null) return null;

        var species = lot.Species;
        var supplier = lot.Supplier;

        return MapToDto(lot, species, supplier);
    }

    public async Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesAsync(int speciesId)
    {
        var lots = await _lotRepository.GetBySpeciesAsync(speciesId);

        return lots
            .Select(lot => MapToDto(lot, lot.Species, lot.Supplier))
            .ToList();
    }

    public async Task RegisterMortalityAsync(RegisterMortalityDto dto)
    {
        var lot = await _lotRepository.GetByIdAsync(dto.InventoryLotId)
                  ?? throw new InvalidOperationException("Inventory lot not found.");

        lot.RegisterMortality(dto.Date, dto.Quantity, dto.Cause, dto.Notes);

        await _lotRepository.UpdateAsync(lot);
    }

    //To get the current stock of a species, we can sum the current stock of all lots of that species.
    public async Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesAsync(int speciesId)
    {
        var species = await _speciesRepository.GetByIdAsync(speciesId);
        if (species is null) return null;

        var lots = await _lotRepository.GetBySpeciesAsync(speciesId);
        
       var currentStock = lots.Sum(l =>
        (l.InitialQuantity - l.DeadOnArrival /* - l.MortalityAfterArrival si luego lo agregas */));

       return new BiologicalStockDto
    {
        SpeciesId = species.Id,
        CommonName = species.CommonName,
        CurrentBiologicalStock = currentStock
    };
    }

    private static InventoryLotDto MapToDto(InventoryLot lot, Species species, Supplier? supplier)
    {
        return new InventoryLotDto
        {
            Id = lot.Id,
            SpeciesId = lot.SpeciesId,
            SpeciesCommonName = species.CommonName,
            ArrivalDate = lot.ArrivalDate,
            InitialQuantity = lot.InitialQuantity,
            DeadOnArrival = lot.DeadOnArrival,
            TotalMortality = lot.GetTotalDeaths(),
            CurrentStock = lot.GetCurrentStock(),
            UnitCost = lot.UnitCost,
            SupplierId = lot.SupplierId,
            SupplierName = supplier?.Name,
            BatchNumber = lot.BatchNumber,
            Notes = lot.Notes
        };
    }

}
