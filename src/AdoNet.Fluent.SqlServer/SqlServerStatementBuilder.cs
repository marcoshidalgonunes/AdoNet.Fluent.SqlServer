using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;

namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Class for bulding SQL Server statement execution.
/// </summary>
/// <param name="configuration"><see cref="IConfiguration"/> containing connection string of database.</param>
/// <param name="connectionName">Name of element containing connection string.</param>
public sealed class SqlServerStatementBuilder(IConfiguration configuration, string connectionName) : DataObjectBuilder<SqlServerStatement>(configuration, connectionName, ConnectionMode.Normal), ISqlServerStatementBuilder
{
    protected override SqlServerStatement Build(string? connectionString, ConnectionMode mode)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return new SqlServerStatement(this.GetConnectionString(connectionString, mode), mode);
    }

    /// <summary>
    /// Sets use of MARS (Multiple Active Resultsets) in statement execution.
    /// </summary>
    /// <returns>Type of <see cref="SqlServerStatementBuilder"/> to execute in SQL Server.</returns>
    public ISqlServerStatementBuilder WithMARS()
    {
        ConnectionMode = ConnectionMode.MultipleResultsets;
        return this;
    }
}
