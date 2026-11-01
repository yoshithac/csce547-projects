namespace DealershipApp.Data.Entities;

/// <summary>
/// Data object that matches one row of the dbo.Salesperson table.
/// </summary>
public class Salesperson
{
    /// <summary>SalespersonId INT IDENTITY(1,1) PRIMARY KEY</summary>
    public int SalespersonId { get; set; }

    /// <summary>DealershipId INT NOT NULL, foreign key to Dealership (ON DELETE CASCADE)</summary>
    public int DealershipId { get; set; }

    /// <summary>FullName NVARCHAR(100) NOT NULL</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email NVARCHAR(100) NOT NULL</summary>
    public string Email { get; set; } = string.Empty;
}
