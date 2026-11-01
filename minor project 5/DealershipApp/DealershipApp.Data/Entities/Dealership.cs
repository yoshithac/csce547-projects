namespace DealershipApp.Data.Entities;

/// <summary>
/// Data object that matches one row of the dbo.Dealership table.
/// </summary>
public class Dealership
{
    /// <summary>DealershipId INT IDENTITY(1,1) PRIMARY KEY</summary>
    public int DealershipId { get; set; }

    /// <summary>Name NVARCHAR(100) NOT NULL</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>City NVARCHAR(50) NOT NULL</summary>
    public string City { get; set; } = string.Empty;
}
