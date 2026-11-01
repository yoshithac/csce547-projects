using DealershipApp.Api.Models;
using DealershipApp.Business;
using Microsoft.AspNetCore.Mvc;

namespace DealershipApp.Api.Controllers;

/// <summary>
/// Service endpoints for the Dealership table.
/// </summary>
[ApiController]
[Route("api/dealerships")]
[Produces("application/json")]
public class DealershipsController : ApiControllerBase
{
    private readonly DealershipManager _dealerships;
    private readonly SalespersonManager _salespeople;
    private readonly VehicleManager _vehicles;

    public DealershipsController(DealershipManager dealerships, SalespersonManager salespeople, VehicleManager vehicles)
    {
        _dealerships = dealerships;
        _salespeople = salespeople;
        _vehicles = vehicles;
    }

    /// <summary>GET api/dealerships — every dealership.</summary>
    [HttpGet]
    public async Task<ActionResult<List<DealershipModel>>> GetAll()
    {
        var dealerships = await _dealerships.GetAllAsync();
        return dealerships.Select(d => ModelMapper.ToModel(d)).ToList();
    }

    /// <summary>GET api/dealerships/5 — one dealership.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DealershipModel>> GetById(int id)
    {
        var dealership = await _dealerships.GetByIdAsync(id);
        if (dealership is null)
        {
            return NotFound();
        }

        return ModelMapper.ToModel(dealership);
    }

    /// <summary>GET api/dealerships/5/salespeople — everyone who works at one dealership.</summary>
    [HttpGet("{id:int}/salespeople")]
    public async Task<ActionResult<List<SalespersonModel>>> GetSalespeople(int id)
    {
        var dealership = await _dealerships.GetByIdAsync(id);
        if (dealership is null)
        {
            return NotFound();
        }

        var salespeople = await _salespeople.GetByDealershipAsync(id);
        return salespeople.Select(s => ModelMapper.ToModel(s, dealership.Name)).ToList();
    }

    /// <summary>GET api/dealerships/5/vehicles — every vehicle on one dealership's lot.</summary>
    [HttpGet("{id:int}/vehicles")]
    public async Task<ActionResult<List<VehicleModel>>> GetVehicles(int id)
    {
        var dealership = await _dealerships.GetByIdAsync(id);
        if (dealership is null)
        {
            return NotFound();
        }

        var vehicles = await _vehicles.GetByDealershipAsync(id);
        return vehicles.Select(v => ModelMapper.ToModel(v, dealership.Name)).ToList();
    }

    /// <summary>
    /// POST api/dealerships — save a dealership.
    /// Id = 0 inserts a new dealership (201 Created); any other Id updates that dealership (200 OK).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DealershipModel>> Save(DealershipModel model)
    {
        var result = await _dealerships.SaveAsync(ModelMapper.ToEntity(model));
        return await SaveResponseAsync(result, LoadModelAsync);
    }

    /// <summary>PUT api/dealerships/5 — update one dealership (optional; POST can also update).</summary>
    [HttpPut("{id:int:min(1)}")]
    public Task<ActionResult<DealershipModel>> Update(int id, DealershipModel model)
    {
        model.Id = id;
        return Save(model);
    }

    /// <summary>
    /// DELETE api/dealerships/5 — delete a dealership.
    /// Its salespeople and vehicles are deleted too (ON DELETE CASCADE).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _dealerships.DeleteAsync(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    private async Task<DealershipModel?> LoadModelAsync(int id)
    {
        var dealership = await _dealerships.GetByIdAsync(id);
        return dealership is null ? null : ModelMapper.ToModel(dealership);
    }
}
