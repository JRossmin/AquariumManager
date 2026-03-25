using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SpeciesService : ISpeciesService
{
    private readonly ISpeciesRepository _speciesRepository;

    public SpeciesService(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<SpeciesDto> CreateAsync(CreateSpeciesDto dto)
    {
        var species = new Species(
            dto.CommonName,
            dto.ScientificName,
            dto.Type,
            dto.Variety,
            dto.MinPH,
            dto.MaxPH,
            dto.MinTemperature,
            dto.MaxTemperature,
            dto.CompatibilityNotes,
            dto.Category,
            dto.Notes
        );

        await _speciesRepository.AddAsync(species);

        return MapToDto(species);
    }

    public async Task<SpeciesDto?> GetByIdAsync(int id)
    {
        var species = await _speciesRepository.GetByIdAsync(id);
        return species is null ? null : MapToDto(species);
    }

    public async Task<IReadOnlyList<SpeciesDto>> GetAllAsync()
    {
        var list = await _speciesRepository.GetAllAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task UpdateAsync(int id, UpdateSpeciesDto dto)
    {
        var species = await _speciesRepository.GetByIdAsync(id)
                      ?? throw new InvalidOperationException("Species not found.");

        species.UpdateInfo(
            dto.CommonName,
            dto.ScientificName,
            dto.Type,
            dto.Variety,
            dto.MinPH,
            dto.MaxPH,
            dto.MinTemperature,
            dto.MaxTemperature,
            dto.CompatibilityNotes,
            dto.Category,
            dto.Notes
        );

        await _speciesRepository.UpdateAsync(species);
    }

    public async Task DeleteAsync(int id)
    {
        await _speciesRepository.DeleteAsync(id);
    }

    private static SpeciesDto MapToDto(Species s) => new()
    {
        Id = s.Id,
        CommonName = s.CommonName,
        ScientificName = s.ScientificName,
        Type = s.Type,
        Variety = s.Variety,
        MinPH = s.MinPH,
        MaxPH = s.MaxPH,
        MinTemperature = s.MinTemperature,
        MaxTemperature = s.MaxTemperature,
        CompatibilityNotes = s.CompatibilityNotes,
        Category = s.Category,
        Notes = s.Notes
    };
}
