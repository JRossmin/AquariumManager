
using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IInventoryLotService _inventoryLotService;
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(
        ISaleRepository saleRepository,
        ISpeciesRepository speciesRepository,
        IInventoryLotService inventoryLotService,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _speciesRepository = speciesRepository;
        _inventoryLotService = inventoryLotService;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<SaleDto>> CreateSaleAsync(CreateSaleDto saleDto)
    {
        // 1. Validaciones generales
        if (saleDto.Items == null || saleDto.Items.Count == 0)
            return OperationResult<SaleDto>.Fail("La venta debe tener al menos un item.");

        foreach (var item in saleDto.Items)
        {
            if (item.SpeciesId <= 0)
                return OperationResult<SaleDto>.Fail("Cada item debe tener un SpeciesId válido.");

            if (item.Quantity <= 0)
                return OperationResult<SaleDto>.Fail("La cantidad debe ser mayor que 0.");

            if (item.UnitPrice <= 0)
                return OperationResult<SaleDto>.Fail("El precio unitario debe ser mayor que 0.");
        }

        // 2. Validar que las especies existan
        foreach (var item in saleDto.Items)
        {
            var species = await _speciesRepository.GetByIdAsync(item.SpeciesId);
            if (species is null)
                return OperationResult<SaleDto>.Fail($"La especie con Id {item.SpeciesId} no existe.");
        }

        // 3. Validar stock biológico suficiente por especie
        foreach (var item in saleDto.Items)
        {
            var stockDto = await _inventoryLotService.GetBiologicalStockDtoBySpeciesAsync(item.SpeciesId);
            var available = stockDto?.CurrentBiologicalStock ?? 0;

            if (available < item.Quantity)
            {
                return OperationResult<SaleDto>.Fail(
                    $"No hay stock suficiente para la especie {item.SpeciesId}. " +
                    $"Disponible: {available}, solicitado: {item.Quantity}.");
            }
        }

        // 4. Crear y persistir la venta dentro de una transacción
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var sale = new Sale
            {
                Date = saleDto.Date,
                CustomerName = saleDto.CustomerName
            };

            foreach (var itemDto in saleDto.Items)
            {
                var species = await _speciesRepository.GetByIdAsync(itemDto.SpeciesId)!;
                var remainingToSell = itemDto.Quantity;

                // Crear SaleItem y descontar stock de los lotes más antiguos primero
                var saleItem = new SaleItem
                {
                    SpeciesId = itemDto.SpeciesId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                };
                sale.Items.Add(saleItem);

                // Descontar stock usando FIFO (lotes más antiguos primero)
                var openLots = (await _inventoryLotService.GetBySpeciesAsync(itemDto.SpeciesId))
                    .Where(l => l.CurrentStock > 0)
                    .OrderBy(l => l.ArrivalDate)
                    .ToList();

                foreach (var lotDto in openLots)
                {
                    if (remainingToSell <= 0) break;

                    var toDeduct = Math.Min(remainingToSell, lotDto.CurrentStock);

                    // Register mortality to deduct stock
                    if (toDeduct > 0)
                    {
                        await _inventoryLotService.RegisterMortalityAsync(
                            new RegisterMortalityDto
                            {
                                InventoryLotId = lotDto.Id,
                                Date = saleDto.Date,
                                Quantity = toDeduct,
                                Cause = "Sold"
                            });
                        remainingToSell -= toDeduct;
                    }
                }
            }

            await _saleRepository.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return OperationResult<SaleDto>.Ok(MapToDto(sale));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return OperationResult<SaleDto>.Fail($"Error al crear la venta: {ex.Message}");
        }
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        return sale is null ? null : MapToDto(sale);
    }

    public async Task<IReadOnlyList<SaleDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
        return sales.Select(MapToDto).ToList();
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            Date = sale.Date,
            CustomerName = sale.CustomerName,
            TotalAmount = sale.Items.Sum(i => i.Quantity * i.UnitPrice),
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                SpeciesId = i.SpeciesId,
                SpeciesCommonName = i.Species?.CommonName ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}