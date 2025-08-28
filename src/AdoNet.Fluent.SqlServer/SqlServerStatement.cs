namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Class for operations on SQL Server databases.
/// </summary>
public sealed class SqlServerStatement : SqlServerDataObject
{
    internal SqlServerStatement(string connectionString, ConnectionMode mode) 
        : base(connectionString, mode) { }
}
