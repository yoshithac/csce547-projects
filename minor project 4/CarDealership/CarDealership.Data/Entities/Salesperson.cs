namespace CarDealership.Data.Entities;

public class Salesperson
{
    public int SalespersonId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }

    public int DealershipId { get; set; }
    public Dealership? Dealership { get; set; }
}