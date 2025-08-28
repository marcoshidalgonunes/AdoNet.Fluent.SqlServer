namespace AdoNet.Fluent.SqlServer;

public sealed class SqlServerStatement : SqlServerDataObject
{
    internal SqlServerStatement(string connectionString, ConnectionMode mode) 
        : base(connectionString, mode) { }
}
