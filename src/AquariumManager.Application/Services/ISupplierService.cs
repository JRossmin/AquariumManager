using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetAllAsync();
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
    Task<OperationResult> UpdateAsync(int id, CreateSupplierDto dto);
    Task<OperationResult> DeleteAsync(int id);
}
