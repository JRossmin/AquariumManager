namespace AquariumManager.Application.DTOs;

public class CatalogItemDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public int CurrentBiologicalStock { get; set; }
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}
