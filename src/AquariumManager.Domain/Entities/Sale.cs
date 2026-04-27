namespace AquariumManager.Domain.Entities;
public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}

