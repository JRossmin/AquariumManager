using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ISpeciesService
{
    Task<SpeciesDto> CreateAsync(CreateSpeciesDto dto);
    Task<SpeciesDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<SpeciesDto>> GetAllAsync();
    Task UpdateAsync(int id, UpdateSpeciesDto dto);
    Task DeleteAsync(int id);
}
