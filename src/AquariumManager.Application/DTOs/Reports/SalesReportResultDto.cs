public class SalesReportResultDto : PagedResult<SalesReportItemDto>
{
    
    public decimal TotalAmountInRange { get; set; }
    public int TotalSalesInRange { get; set; }
}