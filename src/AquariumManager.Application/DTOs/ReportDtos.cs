namespace AquariumManager.Application.DTOs;

// Stock Report
public class StockReportDto
{
    public List<StockReportItemDto> Items { get; set; } = new();
    public int TotalSpecies { get; set; }
    public int TotalStock { get; set; }
}

public class StockReportItemDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public decimal TotalCostValue { get; set; }
    public List<LotBreakdownDto> Lots { get; set; } = new();
}

public class LotBreakdownDto
{
    public int LotId { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentStock { get; set; }
    public string? SupplierName { get; set; }
    public decimal UnitCost { get; set; }
}

// Mortality Report
public class MortalityReportDto
{
    public List<MortalitySummaryDto> Summaries { get; set; } = new();
    public int TotalDeaths { get; set; }
    public int TotalSold { get; set; }
    public int TotalOtherCauses { get; set; }
}

public class MortalitySummaryDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public int TotalDeaths { get; set; }
    public int Sold { get; set; }
    public int OtherCauses { get; set; }
    public List<MortalityRecordDto> Records { get; set; } = new();
}

public class MortalityRecordDto
{
    public int RecordId { get; set; }
    public int LotId { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public string? Cause { get; set; }
    public string? Notes { get; set; }
}

// Sales Report
public class SalesReportDto
{
    public List<SalesSummaryDto> Sales { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public int TotalItemsSold { get; set; }
    public List<TopSpeciesDto> TopSpecies { get; set; } = new();
}

public class SalesSummaryDto
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public class TopSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

// Inventory Valuation
public class InventoryValuationDto
{
    public decimal TotalCostValue { get; set; }
    public int TotalUnitsInStock { get; set; }
    public int TotalLots { get; set; }
    public decimal AverageUnitCost { get; set; }
    public List<ValuationByCategoryDto> ByCategory { get; set; } = new();
}

public class ValuationByCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int UnitsInStock { get; set; }
    public decimal TotalCostValue { get; set; }
    public decimal AverageUnitCost { get; set; }
}
