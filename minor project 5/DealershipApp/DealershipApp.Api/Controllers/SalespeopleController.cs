using DealershipApp.Api.Models;
using DealershipApp.Business;
using Microsoft.AspNetCore.Mvc;

namespace DealershipApp.Api.Controllers;

/// <summary>
/// Service endpoints for the Salesperson table.
/// </summary>
[ApiController]
[Route("api/salespeople")]
[Produces("application/json")]
public class SalespeopleController : ApiControllerBase
{
    private readonly SalespersonManager _salespeople;
    private readonly DealershipManager _dealerships;

    public SalespeopleController(SalespersonManager salespeople, DealershipManager dealerships)
    {
        _salespeople = salespeople;
        _dealerships = dealerships;
    }

    /// <summary>
    /// GET api/salespeople — every salesperson.
    /// GET api/salespeople?dealershipId=5 — only the salespeople at dealership 5.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SalespersonModel>>> GetAll([FromQuery] int? dealershipId)
    {
        var salespeople = dealershipId is null
            ? await _salespeople.GetAllAsync()
            : await _salespeople.GetByDealershipAsync(dealershipId.Value);

        var dealershipNames = await _dealerships.GetNameLookupAsync();
        return salespeople
            .Select(s => ModelMapper.ToModel(s, dealershipNames.GetValueOrDefault(s.DealershipId)))
            .ToList();
    }

    /// <summary>GET api/salespeople/5 — one salesperson.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalespersonModel>> GetById(int id)
    {
        var model = await LoadModelAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return model;
    }

    /// <summary>
    /// POST api/salespeople — save a salesperson.
    /// Id = 0 inserts (201 Created); any other Id updates (200 OK).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalespersonModel>> Save(SalespersonModel model)
    {
        var result = await _salespeople.SaveAsync(ModelMapper.ToEntity(model));
        return await SaveResponseAsync(result, LoadModelAsync);
    }

    /// <summary>PUT api/salespeople/5 — update one salesperson (optional; POST can also update).</summary>
    [HttpPut("{id:int:min(1)}")]
    public Task<ActionResult<SalespersonModel>> Update(int id, SalespersonModel model)
    {
        model.Id = id;
        return Save(model);
    }

    /// <summary>DELETE api/salespeople/5 — delete one salesperson.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _salespeople.DeleteAsync(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    private async Task<SalespersonModel?> LoadModelAsync(int id)
    {
        var salesperson = await _salespeople.GetByIdAsync(id);
        if (salesperson is null)
        {
            return null;
        }

        var dealership = await _dealerships.GetByIdAsync(salesperson.DealershipId);
        return ModelMapper.ToModel(salesperson, dealership?.Name);
    }
}
