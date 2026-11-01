using System.Data.Common;
using DealershipApp.Data.Entities;

namespace DealershipApp.Data.Repositories;

/// <summary>
/// Full CRUD for the dbo.Dealership table.
/// Note: deleting a dealership also deletes its salespeople and vehicles,
/// because both foreign keys are declared ON DELETE CASCADE in the database.
/// </summary>
public class DealershipRepository : RepositoryBase, IDealershipRepository
{
    private const string SelectAllSql = @"
SELECT DealershipId, Name, City
FROM dbo.Dealership
ORDER BY DealershipId;";

    private const string SelectByIdSql = @"
SELECT DealershipId, Name, City
FROM dbo.Dealership
WHERE DealershipId = @DealershipId;";

    private const string InsertSql = @"
INSERT INTO dbo.Dealership (Name, City)
OUTPUT INSERTED.DealershipId
VALUES (@Name, @City);";

    private const string UpdateSql = @"
UPDATE dbo.Dealership
SET Name = @Name,
    City = @City
WHERE DealershipId = @DealershipId;";

    private const string DeleteSql = @"
DELETE FROM dbo.Dealership
WHERE DealershipId = @DealershipId;";

    public DealershipRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public Task<List<Dealership>> GetAllAsync() =>
        QueryAsync(SelectAllSql, Map);

    public Task<Dealership?> GetByIdAsync(int id) =>
        QuerySingleAsync(SelectByIdSql, Map, ("@DealershipId", id));

    public async Task<int> InsertAsync(Dealership dealership)
    {
        var newId = await ExecuteScalarAsync(InsertSql,
            ("@Name", dealership.Name),
            ("@City", dealership.City));

        return Convert.ToInt32(newId);
    }

    public async Task<bool> UpdateAsync(Dealership dealership)
    {
        var rowsAffected = await ExecuteNonQueryAsync(UpdateSql,
            ("@DealershipId", dealership.DealershipId),
            ("@Name", dealership.Name),
            ("@City", dealership.City));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await ExecuteNonQueryAsync(DeleteSql, ("@DealershipId", id));
        return rowsAffected > 0;
    }

    /// <summary>Copies one database row into a Dealership object.</summary>
    private static Dealership Map(DbDataReader reader) => new()
    {
        DealershipId = reader.GetInt32(reader.GetOrdinal("DealershipId")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        City = reader.GetString(reader.GetOrdinal("City"))
    };
}
