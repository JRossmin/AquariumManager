
using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactInfo = s.ContactInfo,
                Phone = s.Phone,
                Email = s.Email,
                Notes = s.Notes
            })
            .ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier is null) return null;

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactInfo = supplier.ContactInfo,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Notes = supplier.Notes
        };
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
         var supplier = new Supplier(dto.Name, dto.ContactInfo, dto.Phone, dto.Email, dto.Notes);

        await _supplierRepository.AddAsync(supplier);

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactInfo = supplier.ContactInfo,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Notes = supplier.Notes
        };
    }

    public async Task<OperationResult> UpdateAsync(int id, CreateSupplierDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier is null) return OperationResult.Fail($"Proveedor con Id {id} no encontrado.");

        supplier.UpdateContact(dto.Name, dto.Phone, dto.Email, dto.ContactInfo, dto.Notes);
        await _supplierRepository.UpdateAsync(supplier);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier is null) return OperationResult.Fail($"Proveedor con Id {id} no encontrado.");

        await _supplierRepository.DeleteAsync(id);

        return OperationResult.Ok();
    }
}