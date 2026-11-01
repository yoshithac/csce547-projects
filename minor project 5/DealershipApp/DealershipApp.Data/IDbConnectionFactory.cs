using System.Data.Common;

namespace DealershipApp.Data;

/// <summary>
/// Creates database connections. The repositories only depend on this interface,
/// so the connection string lives in one place (appsettings.json).
/// </summary>
public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}
