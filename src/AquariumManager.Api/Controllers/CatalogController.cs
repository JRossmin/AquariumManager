using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 

public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    // GET: api/Catalog
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItemDto>>> Get()
    {
        var items = await _catalogService.GetCatalogAsync();
        return Ok(items);
    }
}