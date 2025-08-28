namespace AdoNet.Fluent.SqlServer;

public interface ISqlServerStatementBuilder : IDataObjectBuilder<SqlServerStatement>
{
    ISqlServerStatementBuilder WithMARS();
}
