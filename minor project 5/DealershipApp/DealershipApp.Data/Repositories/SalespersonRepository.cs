using System.Data.Common;
using DealershipApp.Data.Entities;

namespace DealershipApp.Data.Repositories;

/// <summary>
/// Full CRUD for the dbo.Salesperson table.
/// </summary>
public class SalespersonRepository : RepositoryBase, ISalespersonRepository
{
    private const string SelectAllSql = @"
SELECT SalespersonId, DealershipId, FullName, Email
FROM dbo.Salesperson
ORDER BY SalespersonId;";

    private const string SelectByIdSql = @"
SELECT SalespersonId, DealershipId, FullName, Email
FROM dbo.Salesperson
WHERE SalespersonId = @SalespersonId;";

    private const string SelectByDealershipSql = @"
SELECT SalespersonId, DealershipId, FullName, Email
FROM dbo.Salesperson
WHERE DealershipId = @DealershipId
ORDER BY SalespersonId;";

    private const string InsertSql = @"
INSERT INTO dbo.Salesperson (DealershipId, FullName, Email)
OUTPUT INSERTED.SalespersonId
VALUES (@DealershipId, @FullName, @Email);";

    private const string UpdateSql = @"
UPDATE dbo.Salesperson
SET DealershipId = @DealershipId,
    FullName = @FullName,
    Email = @Email
WHERE SalespersonId = @SalespersonId;";

    private const string DeleteSql = @"
DELETE FROM dbo.Salesperson
WHERE SalespersonId = @SalespersonId;";

    public SalespersonRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public Task<List<Salesperson>> GetAllAsync() =>
        QueryAsync(SelectAllSql, Map);

    public Task<Salesperson?> GetByIdAsync(int id) =>
        QuerySingleAsync(SelectByIdSql, Map, ("@SalespersonId", id));

    public Task<List<Salesperson>> GetByDealershipIdAsync(int dealershipId) =>
        QueryAsync(SelectByDealershipSql, Map, ("@DealershipId", dealershipId));

    public async Task<int> InsertAsync(Salesperson salesperson)
    {
        var newId = await ExecuteScalarAsync(InsertSql,
            ("@DealershipId", salesperson.DealershipId),
            ("@FullName", salesperson.FullName),
            ("@Email", salesperson.Email));

        return Convert.ToInt32(newId);
    }

    public async Task<bool> UpdateAsync(Salesperson salesperson)
    {
        var rowsAffected = await ExecuteNonQueryAsync(UpdateSql,
            ("@SalespersonId", salesperson.SalespersonId),
            ("@DealershipId", salesperson.DealershipId),
            ("@FullName", salesperson.FullName),
            ("@Email", salesperson.Email));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await ExecuteNonQueryAsync(DeleteSql, ("@SalespersonId", id));
        return rowsAffected > 0;
    }

    /// <summary>Copies one database row into a Salesperson object.</summary>
    private static Salesperson Map(DbDataReader reader) => new()
    {
        SalespersonId = reader.GetInt32(reader.GetOrdinal("SalespersonId")),
        DealershipId = reader.GetInt32(reader.GetOrdinal("DealershipId")),
        FullName = reader.GetString(reader.GetOrdinal("FullName")),
        Email = reader.GetString(reader.GetOrdinal("Email"))
    };
}
