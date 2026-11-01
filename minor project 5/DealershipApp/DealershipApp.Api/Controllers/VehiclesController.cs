using DealershipApp.Api.Models;
using DealershipApp.Business;
using Microsoft.AspNetCore.Mvc;

namespace DealershipApp.Api.Controllers;

/// <summary>
/// Service endpoints for the Vehicle table.
/// </summary>
[ApiController]
[Route("api/vehicles")]
[Produces("application/json")]
public class VehiclesController : ApiControllerBase
{
    private readonly VehicleManager _vehicles;
    private readonly DealershipManager _dealerships;

    public VehiclesController(VehicleManager vehicles, DealershipManager dealerships)
    {
        _vehicles = vehicles;
        _dealerships = dealerships;
    }

    /// <summary>
    /// GET api/vehicles — every vehicle.
    /// GET api/vehicles?dealershipId=5 — only the vehicles at dealership 5.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<VehicleModel>>> GetAll([FromQuery] int? dealershipId)
    {
        var vehicles = dealershipId is null
            ? await _vehicles.GetAllAsync()
            : await _vehicles.GetByDealershipAsync(dealershipId.Value);

        var dealershipNames = await _dealerships.GetNameLookupAsync();
        return vehicles
            .Select(v => ModelMapper.ToModel(v, dealershipNames.GetValueOrDefault(v.DealershipId)))
            .ToList();
    }

    /// <summary>GET api/vehicles/5 — one vehicle.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleModel>> GetById(int id)
    {
        var model = await LoadModelAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return model;
    }

    /// <summary>
    /// POST api/vehicles — save a vehicle.
    /// Id = 0 inserts (201 Created); any other Id updates (200 OK).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleModel>> Save(VehicleModel model)
    {
        var result = await _vehicles.SaveAsync(ModelMapper.ToEntity(model));
        return await SaveResponseAsync(result, LoadModelAsync);
    }

    /// <summary>PUT api/vehicles/5 — update one vehicle (optional; POST can also update).</summary>
    [HttpPut("{id:int:min(1)}")]
    public Task<ActionResult<VehicleModel>> Update(int id, VehicleModel model)
    {
        model.Id = id;
        return Save(model);
    }

    /// <summary>DELETE api/vehicles/5 — delete one vehicle.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _vehicles.DeleteAsync(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    private async Task<VehicleModel?> LoadModelAsync(int id)
    {
        var vehicle = await _vehicles.GetByIdAsync(id);
        if (vehicle is null)
        {
            return null;
        }

        var dealership = await _dealerships.GetByIdAsync(vehicle.DealershipId);
        return ModelMapper.ToModel(vehicle, dealership?.Name);
    }
}
