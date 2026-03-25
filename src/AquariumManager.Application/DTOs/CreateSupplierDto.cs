namespace AquariumManager.Application.DTOs
{
    public class CreateSupplierDto
    {
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Notes { get; set; } = default!;
    }
}