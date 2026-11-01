namespace DealershipApp.Api.Models;

/// <summary>
/// Service model: the dealership data the client receives and sends.
/// Send Id = 0 to add a new dealership.
/// </summary>
public class DealershipModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}
