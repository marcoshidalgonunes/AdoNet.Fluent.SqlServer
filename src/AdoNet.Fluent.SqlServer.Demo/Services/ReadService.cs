using System.Data;
using System.Data.Common;
using AdoNet.Fluent;
using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal sealed class ReadService(ISqlServerStatementBuilder builder) : IReadService<Hierarchy>
{
    private readonly IDataObjectBuilder<SqlServerStatement> _builder = builder;

    private readonly List<Hierarchy> _hierarchies = [];

    private int ordId, ordRecursionLevel, ordOrganizationNode, ordManagerFirstName, ordManagerLastName, ordFirstName, ordLastName;

    private const string StoredProcedure = "uspGetManagerEmployees";

    private void Fill(DbDataReader reader)
    {
        _hierarchies.Add(new Hierarchy {
            RecursionLevel = reader.GetInt32(ordRecursionLevel),
            OrganizationNode = reader.GetString(ordOrganizationNode),
            ManagerFirstName = reader.GetString(ordManagerFirstName),
            ManagerLastName = reader.GetString(ordManagerLastName),
            Id = reader.GetInt32(ordId),
            FirstName = reader.GetString(ordFirstName),
            LastName = reader.GetString(ordLastName)
        });
    }

    private void SetOrdinal(IDataRecord reader)
    {
        ordRecursionLevel = reader.GetOrdinal("RecursionLevel");
        ordOrganizationNode = reader.GetOrdinal("OrganizationNode");
        ordManagerFirstName = reader.GetOrdinal("ManagerFirstName");
        ordManagerLastName = reader.GetOrdinal("ManagerLastName");
        ordId = reader.GetOrdinal("BusinessEntityID");
        ordFirstName = reader.GetOrdinal("FirstName");
        ordLastName = reader.GetOrdinal("LastName");
    }

    #region IReadService<Hierarchy> members

    public List<Hierarchy> Read(int id)
    {
        using SqlServerStatement statement = _builder.Build();

        statement
            .SetStoredProcedure(StoredProcedure)
            .AddInParameter("BusinessEntityID", id)
            .Read(SetOrdinal, Fill);

        return _hierarchies;
    }

    public async Task<List<Hierarchy>> ReadAsync(int id)
    {
        using SqlServerStatement statement = _builder.Build();

        await statement
            .SetStoredProcedure(StoredProcedure)
            .AddInParameter("BusinessEntityID", id)
            .ReadAsync(SetOrdinal, Fill);
        
        return _hierarchies;
    }

    #endregion
}
