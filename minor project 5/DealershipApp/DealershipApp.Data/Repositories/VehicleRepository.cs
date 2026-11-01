using System.Data.Common;
using DealershipApp.Data.Entities;

namespace DealershipApp.Data.Repositories;

/// <summary>
/// Full CRUD for the dbo.Vehicle table.
/// [Year] is bracketed because YEAR is also the name of a SQL Server function.
/// </summary>
public class VehicleRepository : RepositoryBase, IVehicleRepository
{
    private const string SelectAllSql = @"
SELECT VehicleId, DealershipId, Vin, Make, Model, [Year], Price
FROM dbo.Vehicle
ORDER BY VehicleId;";

    private const string SelectByIdSql = @"
SELECT VehicleId, DealershipId, Vin, Make, Model, [Year], Price
FROM dbo.Vehicle
WHERE VehicleId = @VehicleId;";

    private const string SelectByDealershipSql = @"
SELECT VehicleId, DealershipId, Vin, Make, Model, [Year], Price
FROM dbo.Vehicle
WHERE DealershipId = @DealershipId
ORDER BY VehicleId;";

    private const string InsertSql = @"
INSERT INTO dbo.Vehicle (DealershipId, Vin, Make, Model, [Year], Price)
OUTPUT INSERTED.VehicleId
VALUES (@DealershipId, @Vin, @Make, @Model, @Year, @Price);";

    private const string UpdateSql = @"
UPDATE dbo.Vehicle
SET DealershipId = @DealershipId,
    Vin = @Vin,
    Make = @Make,
    Model = @Model,
    [Year] = @Year,
    Price = @Price
WHERE VehicleId = @VehicleId;";

    private const string DeleteSql = @"
DELETE FROM dbo.Vehicle
WHERE VehicleId = @VehicleId;";

    public VehicleRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public Task<List<Vehicle>> GetAllAsync() =>
        QueryAsync(SelectAllSql, Map);

    public Task<Vehicle?> GetByIdAsync(int id) =>
        QuerySingleAsync(SelectByIdSql, Map, ("@VehicleId", id));

    public Task<List<Vehicle>> GetByDealershipIdAsync(int dealershipId) =>
        QueryAsync(SelectByDealershipSql, Map, ("@DealershipId", dealershipId));

    public async Task<int> InsertAsync(Vehicle vehicle)
    {
        var newId = await ExecuteScalarAsync(InsertSql,
            ("@DealershipId", vehicle.DealershipId),
            ("@Vin", vehicle.Vin),
            ("@Make", vehicle.Make),
            ("@Model", vehicle.Model),
            ("@Year", vehicle.Year),
            ("@Price", vehicle.Price));

        return Convert.ToInt32(newId);
    }

    public async Task<bool> UpdateAsync(Vehicle vehicle)
    {
        var rowsAffected = await ExecuteNonQueryAsync(UpdateSql,
            ("@VehicleId", vehicle.VehicleId),
            ("@DealershipId", vehicle.DealershipId),
            ("@Vin", vehicle.Vin),
            ("@Make", vehicle.Make),
            ("@Model", vehicle.Model),
            ("@Year", vehicle.Year),
            ("@Price", vehicle.Price));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await ExecuteNonQueryAsync(DeleteSql, ("@VehicleId", id));
        return rowsAffected > 0;
    }

    /// <summary>Copies one database row into a Vehicle object.</summary>
    private static Vehicle Map(DbDataReader reader) => new()
    {
        VehicleId = reader.GetInt32(reader.GetOrdinal("VehicleId")),
        DealershipId = reader.GetInt32(reader.GetOrdinal("DealershipId")),
        Vin = reader.GetString(reader.GetOrdinal("Vin")),
        Make = reader.GetString(reader.GetOrdinal("Make")),
        Model = reader.GetString(reader.GetOrdinal("Model")),
        Year = reader.GetInt32(reader.GetOrdinal("Year")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
    };
}
