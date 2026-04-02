using AquariumManager.Domain.Entities;

public class InventoryLot
{
    public int Id { get; private set; }
    public int SpeciesId { get; private set; }
    public Species Species { get; private set; } = null!;

    public DateTime ArrivalDate { get; private set; }
    public int InitialQuantity { get; private set; }
    public int DeadOnArrival { get; private set; }
    public decimal UnitCost { get; private set; }
    public int? SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }
    public string? BatchNumber { get; private set; }
    public string? Notes { get; private set; }
    
    public int TotalMortality { get; set;} //bajas despues de la llegada.
    public ICollection<MortalityRecord> MortalityRecords { get; private set; } = new List<MortalityRecord>();

    private InventoryLot() { }

    public InventoryLot(
        int speciesId,
        DateTime arrivalDate,
        int initialQuantity,
        int deadOnArrival,
        decimal unitCost,
        int? supplierId = null,
        string? batchNumber = null,
        string? notes = null)
    {
        if (initialQuantity <= 0)
            throw new ArgumentException("Initial quantity must be greater than zero.", nameof(initialQuantity));

        if (deadOnArrival < 0 || deadOnArrival > initialQuantity)
            throw new ArgumentException("Dead on arrival must be between 0 and initial quantity.", nameof(deadOnArrival));

        SpeciesId = speciesId;
        ArrivalDate = arrivalDate;
        InitialQuantity = initialQuantity;
        DeadOnArrival = deadOnArrival;
        UnitCost = unitCost;
        SupplierId = supplierId;
        BatchNumber = batchNumber;
        Notes = notes;
    }

    public int GetTotalDeaths() => DeadOnArrival + MortalityRecords.Sum(m => m.Quantity);

    public int GetCurrentStock() => InitialQuantity - GetTotalDeaths();

    public MortalityRecord RegisterMortality(DateTime date, int quantity, string? cause = null, string? notes = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (quantity > GetCurrentStock())
            throw new InvalidOperationException("Cannot register mortality greater than current stock.");

        var record = new MortalityRecord(Id, date, quantity, cause, notes);
        MortalityRecords.Add(record);
        return record;
    }
}
