namespace CarDealership.Data.Entities;

public class Vehicle
{
    public int VehicleId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Mileage { get; set; }

    public int DealershipId { get; set; }
    public Dealership? Dealership { get; set; }
}