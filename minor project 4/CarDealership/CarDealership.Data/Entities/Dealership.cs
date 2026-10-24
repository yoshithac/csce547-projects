namespace CarDealership.Data.Entities;

public class Dealership
{
    public int DealershipId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public ICollection<Salesperson> Salespeople { get; set; } = new List<Salesperson>();
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}