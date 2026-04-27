public class CreateSaleDto
{
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateSaleItemDto> Items { get; set; } = new();
}