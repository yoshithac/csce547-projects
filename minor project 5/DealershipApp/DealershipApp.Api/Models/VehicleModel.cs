using System.Globalization;

namespace DealershipApp.Api.Models;

/// <summary>
/// Service model: the vehicle data the client receives and sends.
/// Send Id = 0 to add a new vehicle.
/// </summary>
public class VehicleModel
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public int Id { get; set; }

    public int DealershipId { get; set; }

    /// <summary>Read-only: filled in by the API so the client can show a name, not just a number.</summary>
    public string? DealershipName { get; set; }

    public string Vin { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public decimal Price { get; set; }

    /// <summary>Ready-to-show text, for example "2021 Honda Accord".</summary>
    public string Description => $"{Year} {Make} {Model}";

    /// <summary>Price formatted as US dollars, for example "$24,500.00".</summary>
    public string PriceDisplay => Price.ToString("C", UsCulture);
}
