using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;

namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Class for building SQL Server transaction execution.
/// </summary>
/// <param name="configuration"><see cref="IConfiguration"/> containing connection string of database.</param>
/// <param name="connectionName">Name of element containing connection string.</param>
public sealed class SqlServerTransactionBuilder(IConfiguration configuration, string connectionName) : DataObjectBuilder<SqlServerTransaction>(configuration, connectionName, ConnectionMode.Transational), ISqlServerTransactionBuilder
{
    /// <summary>
    /// Builds statement to execute in SQL Server.
    /// </summary>
    /// <param name="connectionString">SQL Server connection string.</param>
    /// <param name="mode"><see cref="ConnectionMode"/> of connection.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="connectionString"/> is null.</exception>
    protected override SqlServerTransaction Build(string? connectionString, ConnectionMode mode)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return new SqlServerTransaction(this.GetConnectionString(connectionString, mode));
    }
}
