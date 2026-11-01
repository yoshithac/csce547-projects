namespace DealershipApp.Api.Models;

/// <summary>
/// Service model: the salesperson data the client receives and sends.
/// Send Id = 0 to add a new salesperson.
/// </summary>
public class SalespersonModel
{
    public int Id { get; set; }

    public int DealershipId { get; set; }

    /// <summary>Read-only: filled in by the API so the client can show a name, not just a number.</summary>
    public string? DealershipName { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
