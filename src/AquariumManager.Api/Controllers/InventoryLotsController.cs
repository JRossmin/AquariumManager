using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryLotsController : ControllerBase
{
    private readonly IInventoryLotService _inventoryLotService;

    public InventoryLotsController(IInventoryLotService inventoryLotService)
    {
        _inventoryLotService = inventoryLotService;
    }

    // GET: api/InventoryLots/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryLotDto>> GetById(int id)
    {
        var lot = await _inventoryLotService.GetByIdAsync(id);
        if (lot is null)
            return NotFound();

        return Ok(lot);
    }

    // GET: api/InventoryLots/by-species/3
    [HttpGet("by-species/{speciesId:int}")]
    public async Task<ActionResult<IEnumerable<InventoryLotDto>>> GetBySpecies(int speciesId)
    {
        var lots = await _inventoryLotService.GetBySpeciesAsync(speciesId);
        return Ok(lots);
    }

    // POST: api/InventoryLots
    [HttpPost]
    public async Task<ActionResult<InventoryLotDto>> Create(CreateInventoryLotDto dto)
    {
        
        if(dto.InitialQuantity <= 0)
            return BadRequest("Cantidad inicial debe ser mayor a cero.");
        if(dto.DeadOnArrival < 0)
            return BadRequest("Decesos al llegar no debe ser negativa.");
        if(dto.DeadOnArrival > dto.InitialQuantity)
            return BadRequest("Decesos al llegar no puede exceder la cantidad inicial.");
        
        if(dto.UnitCost <= 0)
            return BadRequest("Costo unitario debe ser mayor a cero."); 
        var created = await _inventoryLotService.CreateLotAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // POST: api/InventoryLots/register-mortality
    [HttpPost("register-mortality")]
    public async Task<IActionResult> RegisterMortality(RegisterMortalityDto dto)
    {
        await _inventoryLotService.RegisterMortalityAsync(dto);
        return NoContent();
    }
}
