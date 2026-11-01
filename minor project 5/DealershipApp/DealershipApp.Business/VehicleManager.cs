using System.Text.RegularExpressions;
using DealershipApp.Data.Entities;
using DealershipApp.Data.Repositories;

namespace DealershipApp.Business;

/// <summary>
/// Business methods for vehicles: one Save for insert and update,
/// VIN clean-up and checks, sensible year and price ranges, and a check that the dealership exists.
/// </summary>
public class VehicleManager
{
    /// <summary>Vehicles made since 1981 have a 17-character VIN that never uses I, O or Q.</summary>
    private static readonly Regex VinPattern = new("^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.Compiled);

    /// <summary>Largest value a DECIMAL(10,2) column can hold.</summary>
    private const decimal MaxPrice = 99_999_999.99m;

    private const int OldestYear = 1900;

    private readonly IVehicleRepository _vehicles;
    private readonly IDealershipRepository _dealerships;

    public VehicleManager(IVehicleRepository vehicles, IDealershipRepository dealerships)
    {
        _vehicles = vehicles;
        _dealerships = dealerships;
    }

    public Task<List<Vehicle>> GetAllAsync() => _vehicles.GetAllAsync();

    public Task<Vehicle?> GetByIdAsync(int id) => _vehicles.GetByIdAsync(id);

    /// <summary>Every vehicle on one dealership's lot.</summary>
    public Task<List<Vehicle>> GetByDealershipAsync(int dealershipId) =>
        _vehicles.GetByDealershipIdAsync(dealershipId);

    /// <summary>
    /// Inserts the vehicle when VehicleId is 0, otherwise updates it.
    /// The VIN is trimmed and upper-cased, and the price is rounded to cents.
    /// </summary>
    public async Task<SaveResult> SaveAsync(Vehicle vehicle)
    {
        vehicle.Vin = vehicle.Vin?.Trim().ToUpperInvariant() ?? string.Empty;
        vehicle.Make = vehicle.Make?.Trim() ?? string.Empty;
        vehicle.Model = vehicle.Model?.Trim() ?? string.Empty;
        vehicle.Price = decimal.Round(vehicle.Price, 2, MidpointRounding.AwayFromZero);

        var errors = new ValidationErrors();
        if (vehicle.VehicleId < 0)
        {
            errors.Add(nameof(Vehicle.VehicleId), "VehicleId cannot be negative. Use 0 to add a new vehicle.");
        }

        errors.RequireText(nameof(Vehicle.Vin), vehicle.Vin, 50);
        if (vehicle.Vin.Length > 0 && !VinPattern.IsMatch(vehicle.Vin))
        {
            errors.Add(nameof(Vehicle.Vin), "Vin must be 17 letters or digits, and cannot contain I, O or Q.");
        }

        errors.RequireText(nameof(Vehicle.Make), vehicle.Make, 50);
        errors.RequireText(nameof(Vehicle.Model), vehicle.Model, 50);

        var newestYear = DateTime.Today.Year + 1;
        if (vehicle.Year < OldestYear || vehicle.Year > newestYear)
        {
            errors.Add(nameof(Vehicle.Year), $"Year must be between {OldestYear} and {newestYear}.");
        }

        if (vehicle.Price < 0 || vehicle.Price > MaxPrice)
        {
            errors.Add(nameof(Vehicle.Price), $"Price must be between 0 and {MaxPrice:N2}.");
        }

        if (await _dealerships.GetByIdAsync(vehicle.DealershipId) is null)
        {
            errors.Add(nameof(Vehicle.DealershipId), $"Dealership {vehicle.DealershipId} does not exist.");
        }

        if (errors.Any)
        {
            return SaveResult.Invalid(errors.ToDictionary());
        }

        if (vehicle.VehicleId == 0)
        {
            var newId = await _vehicles.InsertAsync(vehicle);
            return SaveResult.Inserted(newId);
        }

        var updated = await _vehicles.UpdateAsync(vehicle);
        return updated
            ? SaveResult.Updated(vehicle.VehicleId)
            : SaveResult.NotFound(vehicle.VehicleId);
    }

    /// <summary>Deletes a vehicle. Returns false if it doesn't exist.</summary>
    public Task<bool> DeleteAsync(int id) => _vehicles.DeleteAsync(id);
}
