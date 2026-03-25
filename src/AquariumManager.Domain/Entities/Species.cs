namespace AquariumManager.Domain.Entities;

public class Species
{
    public int Id { get; private set; }

    public string CommonName { get; private set; } = string.Empty;
    public string ScientificName { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Variety { get; private set; } = string.Empty;
    public decimal? MinPH { get; private set; }
    public decimal? MaxPH { get; private set; }
    public decimal? MinTemperature { get; private set; }
    public decimal? MaxTemperature { get; private set; }
    public string? CompatibilityNotes { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public ICollection<InventoryItem> InventoryItems { get; private set; } = new List<InventoryItem>();
    public ICollection<InventoryLot> InventoryLots { get; private set; } = new List<InventoryLot>();

    private Species() { }

    public Species(
        string commonName,
        string scientificName,
        string type,
        string variety,
        decimal? minPH,
        decimal? maxPH,
        decimal? minTemperature,
        decimal? maxTemperature,
        string? compatibilityNotes,
        string category,
        string? notes = null)
    {
        CommonName = commonName;
        ScientificName = scientificName;
        Type = type;
        Variety = variety;
        MinPH = minPH;
        MaxPH = maxPH;
        MinTemperature = minTemperature;
        MaxTemperature = maxTemperature;
        CompatibilityNotes = compatibilityNotes;
        Category = category;
        Notes = notes;
    }

    public void UpdateInfo(
        string commonName,
        string scientificName,
        string type,
        string variety,
        decimal? minPH,
        decimal? maxPH,
        decimal? minTemperature,
        decimal? maxTemperature,
        string? compatibilityNotes,
        string category,
        string? notes)
    {
        CommonName = commonName;
        ScientificName = scientificName;
        Type = type;
        Variety = variety;
        MinPH = minPH;
        MaxPH = maxPH;
        MinTemperature = minTemperature;
        MaxTemperature = maxTemperature;
        CompatibilityNotes = compatibilityNotes;
        Category = category;
        Notes = notes;
    }
}
