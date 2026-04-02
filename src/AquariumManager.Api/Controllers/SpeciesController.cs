using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeciesController : ControllerBase
{
    private readonly ISpeciesService _speciesService;

    public SpeciesController(ISpeciesService speciesService)
    {
        _speciesService = speciesService;
    }

    // GET: api/Species
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpeciesDto>>> GetAll()
    {
        var species = await _speciesService.GetAllAsync();
        return Ok(species);
    }

    // GET: api/Species/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeciesDto>> GetById(int id)
    {
        var species = await _speciesService.GetByIdAsync(id);
        if (species is null)
            return NotFound();

        return Ok(species);
    }

    // POST: api/Species
    [HttpPost]
    public async Task<ActionResult<SpeciesDto>> Create(CreateSpeciesDto dto)
    {
        if(dto.MinPH <= 0 || dto.MaxPH <= 0)
            return BadRequest("Los valores de pH deben ser mayores que cero.");
        
        if(dto.MinPH > dto.MaxPH)
            return BadRequest("El pH mínimo no puede ser mayor que el pH máximo.");
        if(dto.MinTemperature > dto.MaxTemperature)
            return BadRequest("La temperatura mínima no puede ser mayor que la temperatura máxima.");

        var created = await _speciesService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/Species/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSpeciesDto dto)
    {
         var result = await _speciesService.UpdateAsync(id, dto);

    if (!result.Success)
    {
        // Diferenciar si es validación o recurso no encontrado
        if (result.ErrorMessage == "La especie especificada no existe.")
            return NotFound(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }

    return NoContent();
    }
}
