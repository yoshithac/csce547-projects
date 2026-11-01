using System.Data.Common;

namespace DealershipApp.Data.Repositories;

/// <summary>
/// Shared ADO.NET plumbing: open a connection, build a parameterized command, run it.
/// Every value goes in as a parameter (@Name), never pasted into the SQL text,
/// which protects against SQL injection.
/// </summary>
public abstract class RepositoryBase
{
    private readonly IDbConnectionFactory _connectionFactory;

    protected RepositoryBase(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>Runs a SELECT and turns each row into an object with <paramref name="map"/>.</summary>
    protected async Task<List<T>> QueryAsync<T>(
        string sql,
        Func<DbDataReader, T> map,
        params (string Name, object? Value)[] parameters)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = CreateCommand(connection, sql, parameters);
        await using var reader = await command.ExecuteReaderAsync();

        var results = new List<T>();
        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    /// <summary>Runs a SELECT that returns at most one row.</summary>
    protected async Task<T?> QuerySingleAsync<T>(
        string sql,
        Func<DbDataReader, T> map,
        params (string Name, object? Value)[] parameters) where T : class
    {
        var rows = await QueryAsync(sql, map, parameters);
        return rows.FirstOrDefault();
    }

    /// <summary>Runs an INSERT, UPDATE or DELETE and returns the number of rows affected.</summary>
    protected async Task<int> ExecuteNonQueryAsync(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = CreateCommand(connection, sql, parameters);
        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>Runs a command and returns the first column of the first row (used to read back new IDs).</summary>
    protected async Task<object?> ExecuteScalarAsync(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = CreateCommand(connection, sql, parameters);
        return await command.ExecuteScalarAsync();
    }

    private static DbCommand CreateCommand(DbConnection connection, string sql, (string Name, object? Value)[] parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var (name, value) in parameters)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        return command;
    }
}
