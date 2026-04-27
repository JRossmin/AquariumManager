using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: api/Reports/stock
    [HttpGet("stock")]
    public async Task<IActionResult> GetStockReport()
    {
        var report = await _reportService.GetStockReportAsync();
        return Ok(report);
    }

    // GET: api/Reports/mortality?startDate=X&endDate=Y&speciesId=Z
    [HttpGet("mortality")]
    public async Task<IActionResult> GetMortalityReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? speciesId = null)
    {
        var report = await _reportService.GetMortalityReportAsync(startDate, endDate, speciesId);
        return Ok(report);
    }

    // GET: api/Reports/sales?startDate=X&endDate=Y
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest("startDate must be before endDate.");

        var report = await _reportService.GetSalesReportAsync(startDate, endDate);
        return Ok(report);
    }

    // GET: api/Reports/valuation
    [HttpGet("valuation")]
    public async Task<IActionResult> GetInventoryValuation()
    {
        var report = await _reportService.GetInventoryValuationAsync();
        return Ok(report);
    }
}
