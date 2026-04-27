using AquariumManager.Application.DTOs;
using AquariumManager.Application.Common;

namespace AquariumManager.Application.Services;

public interface ISaleService
{
    Task<OperationResult<SaleDto>> CreateSaleAsync(CreateSaleDto dto);
    Task<SaleDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<SaleDto>> GetAllAsync();
    Task<IReadOnlyList<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}