namespace AquariumManager.Domain.Entities;

public class InventoryItem
{
    public int Id { get; private set; }

    public int SpeciesId { get; private set; }
    public Species Species { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal CostPrice { get; private set; }
    public decimal SalePrice { get; private set; }

    private InventoryItem() { }

    public InventoryItem(int speciesId, int quantity, decimal costPrice, decimal salePrice)
    {
        SpeciesId = speciesId;
        Quantity = quantity;
        CostPrice = costPrice;
        SalePrice = salePrice;
    }
}
