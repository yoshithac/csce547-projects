namespace DealershipApp.Data.Entities;

/// <summary>
/// Data object that matches one row of the dbo.Vehicle table.
/// </summary>
public class Vehicle
{
    /// <summary>VehicleId INT IDENTITY(1,1) PRIMARY KEY</summary>
    public int VehicleId { get; set; }

    /// <summary>DealershipId INT NOT NULL, foreign key to Dealership (ON DELETE CASCADE)</summary>
    public int DealershipId { get; set; }

    /// <summary>Vin NVARCHAR(50) NOT NULL</summary>
    public string Vin { get; set; } = string.Empty;

    /// <summary>Make NVARCHAR(50) NOT NULL</summary>
    public string Make { get; set; } = string.Empty;

    /// <summary>Model NVARCHAR(50) NOT NULL</summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>Year INT NOT NULL</summary>
    public int Year { get; set; }

    /// <summary>Price DECIMAL(10,2) NOT NULL</summary>
    public decimal Price { get; set; }
}
