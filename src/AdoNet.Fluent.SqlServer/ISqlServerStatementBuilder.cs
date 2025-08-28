namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Interface for bulding SQL Server statement execution.
/// </summary>
public interface ISqlServerStatementBuilder : IDataObjectBuilder<SqlServerStatement>
{
    /// <summary>
    /// Sets use of MARS (Multiple Active Resultsets) in statement execution.
    /// </summary>
    /// <returns>Type of <see cref="IDataObjectBuilder<SqlServerStatement>"/> to execute in SQL Server.</returns>
    ISqlServerStatementBuilder WithMARS();
}
