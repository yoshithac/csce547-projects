using Microsoft.EntityFrameworkCore;
using CarDealership.Data.Entities;

namespace CarDealership.Data.Context;

public class DealershipDbContext : DbContext
{
    public DealershipDbContext(DbContextOptions<DealershipDbContext> options)
        : base(options)
    {
    }

    public DbSet<Dealership> Dealerships => Set<Dealership>();
    public DbSet<Salesperson> Salespeople => Set<Salesperson>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Salesperson>()
            .Property(s => s.CommissionRate)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Price)
            .HasPrecision(12, 2);
    }
}