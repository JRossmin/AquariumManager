namespace AquariumManager.Application.DTOs;

public class CreateSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class SpeciesDto
{
    public int Id { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
