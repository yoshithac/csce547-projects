using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DealershipApp.Data;

/// <summary>
/// Creates SQL Server connections from the connection string in appsettings.json.
/// </summary>
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public DbConnection CreateConnection() => new SqlConnection(_connectionString);
}
