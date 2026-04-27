using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IReportService
{
    Task<StockReportDto> GetStockReportAsync();
    Task<MortalityReportDto> GetMortalityReportAsync(DateTime? startDate = null, DateTime? endDate = null, int? speciesId = null);
    Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate, int page, int pageSize);
    Task<InventoryValuationDto> GetInventoryValuationAsync();
}
