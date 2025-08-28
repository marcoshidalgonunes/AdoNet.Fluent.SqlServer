using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer.Extensions;

internal static class DataObjectBuilderExtensions
{
    internal static string GetConnectionString<TDataObject>(this DataObjectBuilder<TDataObject> builder, string connectionString, ConnectionMode mode)
        where TDataObject : IDataObject
    {
        if (mode == ConnectionMode.MultipleResultsets)
        {
            SqlConnectionStringBuilder scsb = new(connectionString)
            {
                MultipleActiveResultSets = true // Define que será suportado MARS.
            };

            return scsb.ConnectionString;
        }

        return connectionString;
    }
}
