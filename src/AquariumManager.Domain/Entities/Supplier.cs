public class Supplier
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? ContactInfo { get; private set; } = default!;
    
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Notes { get; private set; } = default!;

    public ICollection<InventoryLot> InventoryLots { get; private set; } = new List<InventoryLot>();

    public Supplier() { }

    public Supplier(string name, string? contactInfo = null, string? phone = null, string? email = null, string? notes = null)
    {
        Name = name;
        ContactInfo = contactInfo;
        Phone = phone ?? string.Empty;
        Email = email ?? string.Empty;
        Notes = notes ?? string.Empty;
    }

    public void UpdateContact(string name, string phone, string email, string? contactInfo, string? notes)
    {
        Name = name;
        Phone = phone;
        Email = email;  
        ContactInfo = contactInfo;

        Notes = notes ?? string.Empty;
    }
}