using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;

namespace AdoNet.Fluent.SqlServer;

public sealed class SqlServerStatementBuilder(IConfiguration configuration, string connectionName) : DataObjectBuilder<SqlServerStatement>(configuration, connectionName, ConnectionMode.Normal), ISqlServerStatementBuilder
{
    protected override SqlServerStatement Build(string? connectionString, ConnectionMode mode)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return new SqlServerStatement(this.GetConnectionString(connectionString, mode), mode);
    }

    public ISqlServerStatementBuilder WithMARS()
    {
        ConnectionMode = ConnectionMode.MultipleResultsets;
        return this;
    }
}
