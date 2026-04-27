using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class ReportService : IReportService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IInventoryLotRepository _lotRepository;
    private readonly ISaleRepository _saleRepository;

    public ReportService(
        ISpeciesRepository speciesRepository,
        IInventoryLotRepository lotRepository,
        ISaleRepository saleRepository)
    {
        _speciesRepository = speciesRepository;
        _lotRepository = lotRepository;
        _saleRepository = saleRepository;
    }

    public async Task<StockReportDto> GetStockReportAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync();
        var report = new StockReportDto();

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesAsync(species.Id);
            var openLots = lots.Where(l => l.GetCurrentStock() > 0).ToList();

            if (openLots.Count == 0) continue;

            var totalStock = openLots.Sum(l => l.GetCurrentStock());
            var totalCostValue = openLots.Sum(l => l.GetCurrentStock() * l.UnitCost);

            report.Items.Add(new StockReportItemDto
            {
                SpeciesId = species.Id,
                CommonName = species.CommonName,
                ScientificName = species.ScientificName,
                Category = species.Category,
                CurrentStock = totalStock,
                TotalCostValue = totalCostValue,
                Lots = openLots.Select(l => new LotBreakdownDto
                {
                    LotId = l.Id,
                    ArrivalDate = l.ArrivalDate,
                    InitialQuantity = l.InitialQuantity,
                    CurrentStock = l.GetCurrentStock(),
                    SupplierName = l.Supplier?.Name,
                    UnitCost = l.UnitCost
                }).OrderBy(l => l.ArrivalDate).ToList()
            });
        }

        report.TotalSpecies = report.Items.Count;
        report.TotalStock = report.Items.Sum(i => i.CurrentStock);

        return report;
    }

    public async Task<MortalityReportDto> GetMortalityReportAsync(DateTime? startDate = null, DateTime? endDate = null, int? speciesId = null)
    {
        var speciesList = speciesId.HasValue
            ? (await _speciesRepository.GetByIdAsync(speciesId.Value) is { } s ? new[] { s } : Array.Empty<Domain.Entities.Species>())
            : await _speciesRepository.GetAllAsync();

        var report = new MortalityReportDto();

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesAsync(species.Id);
            var allRecords = lots.SelectMany(l => l.MortalityRecords).ToList();

            if (startDate.HasValue)
                allRecords = allRecords.Where(r => r.Date >= startDate.Value).ToList();
            if (endDate.HasValue)
                allRecords = allRecords.Where(r => r.Date <= endDate.Value).ToList();

            if (allRecords.Count == 0) continue;

            var sold = allRecords.Where(r => string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase)).Sum(r => r.Quantity);
            var otherCauses = allRecords.Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase)).Sum(r => r.Quantity);
            var totalDeaths = sold + otherCauses;

            report.Summaries.Add(new MortalitySummaryDto
            {
                SpeciesId = species.Id,
                CommonName = species.CommonName,
                TotalDeaths = totalDeaths,
                Sold = sold,
                OtherCauses = otherCauses,
                Records = allRecords
                    .OrderByDescending(r => r.Date)
                    .Select(r => new MortalityRecordDto
                    {
                        RecordId = r.Id,
                        LotId = r.InventoryLotId,
                        Date = r.Date,
                        Quantity = r.Quantity,
                        Cause = r.Cause,
                        Notes = r.Notes
                    }).ToList()
            });
        }

        report.TotalDeaths = report.Summaries.Sum(s => s.TotalDeaths);
        report.TotalSold = report.Summaries.Sum(s => s.Sold);
        report.TotalOtherCauses = report.Summaries.Sum(s => s.OtherCauses);

        return report;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 50)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 50;
        
        
        var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
        var report = new SalesReportDto();

        report.Sales = sales.Select(s => new SalesSummaryDto
        {
            SaleId = s.Id,
            Date = s.Date,
            CustomerName = s.CustomerName,
            TotalAmount = s.Items.Sum(i => i.Quantity * i.UnitPrice),
            ItemCount = s.Items.Count
        }).OrderByDescending(s => s.Date).ToList();

        report.TotalRevenue = report.Sales.Sum(s => s.TotalAmount);
        report.TotalItemsSold = sales.Sum(s => s.Items.Sum(i => i.Quantity));

        // Top species by quantity sold
        var speciesGroups = sales
            .SelectMany(s => s.Items)
            .GroupBy(i => new { i.SpeciesId, SpeciesName = i.Species?.CommonName ?? "Unknown" })
            .Select(g => new TopSpeciesDto
            {
                CommonName = g.Key.SpeciesName,
                TotalQuantitySold = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(t => t.TotalQuantitySold)
            .Take(10)
            .ToList();

        report.TopSpecies = speciesGroups;

        return report;
    }

    public async Task<InventoryValuationDto> GetInventoryValuationAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync();
        var byCategory = new Dictionary<string, ValuationByCategoryDto>();

        int totalUnits = 0;
        decimal totalCostValue = 0;
        int totalLots = 0;

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesAsync(species.Id);
            var openLots = lots.Where(l => l.GetCurrentStock() > 0).ToList();

            foreach (var lot in openLots)
            {
                var stock = lot.GetCurrentStock();
                var costValue = stock * lot.UnitCost;
                totalUnits += stock;
                totalCostValue += costValue;
                totalLots++;

                if (!byCategory.ContainsKey(species.Category))
                {
                    byCategory[species.Category] = new ValuationByCategoryDto
                    {
                        Category = species.Category,
                        UnitsInStock = 0,
                        TotalCostValue = 0
                    };
                }

                byCategory[species.Category].UnitsInStock += stock;
                byCategory[species.Category].TotalCostValue += costValue;
            }
        }

        foreach (var cat in byCategory.Values)
        {
            cat.AverageUnitCost = cat.UnitsInStock > 0 ? cat.TotalCostValue / cat.UnitsInStock : 0;
        }

        return new InventoryValuationDto
        {
            TotalCostValue = totalCostValue,
            TotalUnitsInStock = totalUnits,
            TotalLots = totalLots,
            AverageUnitCost = totalUnits > 0 ? totalCostValue / totalUnits : 0,
            ByCategory = byCategory.Values.OrderByDescending(c => c.TotalCostValue).ToList()
        };
    }
}
