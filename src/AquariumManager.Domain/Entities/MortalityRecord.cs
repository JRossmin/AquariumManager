public class MortalityRecord
{
    public int Id { get; private set; }
    public int InventoryLotId { get; private set; }
    public InventoryLot InventoryLot { get; private set; } = null!;

    public DateTime Date { get; private set; }
    public int Quantity { get; private set; }
    public string? Cause { get; private set; }
    public string? Notes { get; private set; }

    private MortalityRecord() { }

    public MortalityRecord(int inventoryLotId, DateTime date, int quantity, string? cause, string? notes)
    {
        InventoryLotId = inventoryLotId;
        Date = date;
        Quantity = quantity;
        Cause = cause;
        Notes = notes;
    }
}