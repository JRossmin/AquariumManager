
using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    // POST: api/Sales
    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleDto dto)
    {
        var result = await _saleService.CreateSaleAsync(dto);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    // GET: api/Sales
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    // GET: api/Sales/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale is null) return NotFound();
        return Ok(sale);
    }

    // GET: api/Sales/by-date-range?startDate=X&endDate=Y
    [HttpGet("by-date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var sales = await _saleService.GetByDateRangeAsync(startDate, endDate);
        return Ok(sales);
    }
}