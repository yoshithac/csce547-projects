using DealershipApp.Data.Entities;

namespace DealershipApp.Api.Models;

/// <summary>
/// Converts between data objects (one per table) and service models (what the client sees).
/// </summary>
public static class ModelMapper
{
    public static DealershipModel ToModel(Dealership dealership) => new()
    {
        Id = dealership.DealershipId,
        Name = dealership.Name,
        City = dealership.City
    };

    public static Dealership ToEntity(DealershipModel model) => new()
    {
        DealershipId = model.Id,
        Name = model.Name,
        City = model.City
    };

    public static SalespersonModel ToModel(Salesperson salesperson, string? dealershipName) => new()
    {
        Id = salesperson.SalespersonId,
        DealershipId = salesperson.DealershipId,
        DealershipName = dealershipName,
        FullName = salesperson.FullName,
        Email = salesperson.Email
    };

    public static Salesperson ToEntity(SalespersonModel model) => new()
    {
        SalespersonId = model.Id,
        DealershipId = model.DealershipId,
        FullName = model.FullName,
        Email = model.Email
    };

    public static VehicleModel ToModel(Vehicle vehicle, string? dealershipName) => new()
    {
        Id = vehicle.VehicleId,
        DealershipId = vehicle.DealershipId,
        DealershipName = dealershipName,
        Vin = vehicle.Vin,
        Make = vehicle.Make,
        Model = vehicle.Model,
        Year = vehicle.Year,
        Price = vehicle.Price
    };

    public static Vehicle ToEntity(VehicleModel model) => new()
    {
        VehicleId = model.Id,
        DealershipId = model.DealershipId,
        Vin = model.Vin,
        Make = model.Make,
        Model = model.Model,
        Year = model.Year,
        Price = model.Price
    };
}
