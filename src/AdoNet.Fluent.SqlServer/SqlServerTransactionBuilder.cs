using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;

namespace AdoNet.Fluent.SqlServer;

public sealed class SqlServerTransactionBuilder(IConfiguration configuration, string connectionName) : DataObjectBuilder<SqlServerTransaction>(configuration, connectionName, ConnectionMode.Transational), ISqlServerTransactionBuilder
{
    protected override SqlServerTransaction Build(string? connectionString, ConnectionMode mode)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return new SqlServerTransaction(this.GetConnectionString(connectionString, mode));
    }
}
