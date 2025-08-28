using AdoNet.Fluent;
using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal sealed class CRUDService(ISqlServerStatementBuilder builder) : ICRUDService<Department>
{
    private readonly ISqlServerStatementBuilder _builder = builder;

    private const string InsertDepartment = @"
        INSERT INTO HumanResources.Department
            ([Name], GroupName, ModifiedDate)
        OUTPUT INSERTED.DepartmentID
        VALUES 
            (@Name, @GroupName, @ModifiedDate)
    ";

    private const string DeleteDepartment = @"
        DELETE FROM HumanResources.Department
        WHERE DepartmentID = @DepartmentID
    ";

    private const string SelectDepartment = @"
        SELECT
            @Name = [Name],
            @GroupName= GroupName,
            @ModifiedDate = ModifiedDate
        FROM HumanResources.Department
        WHERE DepartmentID = @DepartmentID
    ";

    private const string UpdateDepartment = @"
        UPDATE HumanResources.Department SET
            [Name] = @Name,
            GroupName = @GroupName,
            ModifiedDate = @ModifiedDate
        WHERE DepartmentID = @DepartmentID
    ";

    public int Create(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(InsertDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate);

        int? id = _statement.ScalarInt32();

        return id ?? 0;
    }

    public async Task<int> CreateAsync(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(InsertDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate);

        int? id = await _statement.ScalarInt32Async();

        return id ?? 0;
    }

    public void Delete(int id)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(DeleteDepartment)
            .AddInParameter("DepartmentID", id);

        _statement.Execute();
    }

    public async Task DeleteAsync(int id)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(DeleteDepartment)
            .AddInParameter("DepartmentID", id);

        await _statement.ExecuteAsync();
    }

    public Department Read(int id)
    {
        Department department = new()
        { 
            Id = id
        };

        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(SelectDepartment)
            .AddInParameter("DepartmentID", id)
            .AddOutParameter("Name", 50)
            .AddOutParameter("GroupName", 50)
            .AddOutParameter("ModifiedDate", NumericType.DateTime);

        _statement.Execute();

        department.Name = _statement.GetString("Name");
        department.GroupName = _statement.GetString("GroupName");
        department.ModifiedDate = _statement.GetDateTime("ModifiedDate");

        return department;
    }

    public async Task<Department> ReadAsync(int id)
    {
        Department department = new()
        {
            Id = id
        };

        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(SelectDepartment)
            .AddInParameter("DepartmentID", id)
            .AddOutParameter("Name", 50)
            .AddOutParameter("GroupName", 50)
            .AddOutParameter("ModifiedDate", NumericType.DateTime);

        await _statement.ExecuteAsync();

        department.Name = _statement.GetString("Name");
        department.GroupName = _statement.GetString("GroupName");
        department.ModifiedDate = _statement.GetDateTime("ModifiedDate");

        return department;
    }

    public void Update(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(UpdateDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate)
            .AddInParameter("DepartmentID", department.Id);

        _statement.Execute();
    }

    public async Task UpdateAsync(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(UpdateDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate)
            .AddInParameter("DepartmentID", department.Id);

        await _statement.ExecuteAsync();
    }
}
