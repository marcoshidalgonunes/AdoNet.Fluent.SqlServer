# AdoNet.Fluent.SqlServer
ADO.Net [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) Library for [SQL Server](https://www.microsoft.com/en-us/sql-server) DBMS.

## Purpose
This library offers another abstraction layer to Ado.Net, as [LINQ](https://learn.microsoft.com/en-us/dotnet/standard/linq/) does. It is aimed to deliver a higher performance than [Entity Framework](https://learn.microsoft.com/en-us/ef/ef6/) because combines the ease of fluent notation with direct call to [ADO.Net](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/) classes.

As pure ADO.Net, the AdoNet.Fluent.SqlServer demands a knowledge of SQL Server, including SQL commands. The benefit is a code easier to be developed, suitable to be used with AI helpers such as GitHub Copilot, in order to achieve a good development productivity along with better performance.

AdoNet.Fluent.SqlServer fits in development of containerized microservices that use relational databases and requires high throughput.

## Available Classes

- **SqlServerStatement**  
  Encapsulates ADO.Net functionality, abstracting complexities such as open and close connections, using Fluent Notation.

- **SqlServerStatementBuilder**  
  Builder for `SqlServerStatement` instances using Fluent Notation.

- **SqlServerTransaction**  
  Encapsulates ADO.Net functionality, abstracting complexities such as open and close connections, for [transaction](https://en.wikipedia.org/wiki/Database_transaction) operations using Fluent Notation.

- **SqlServerTransactionBuilder**  
  Builder for `SqlServerTransaction` instances using Fluent Notation.

## Getting Started

Install it via NuGet:

```script
dotnet add package AdoNet.Fluent.SqlServer
```

The installation will include `AdoNet.Fluent` as transient dependency, which is the core library for AdoNet.Fluent.SqlServer.

## Usage

Usage examples target [AdventureWorks](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms) sample database that shows SQL Server features. AdoNet.Fluent.SqlServer supports all Ado.Net capabilities, such as [MARS](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql/multiple-active-result-sets-mars).

The examples show how to use both synchronous and asynchronous methods of AdoNet.Fluent.SqlServer.

### Setup

To setup builders for statements and transactions, the JSON settings for application should hava a connection string configured for SQL Server. The example below is a minimal configuration for AdventureWorks runnning in a [LocalDb](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver17) instance:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AdventureWorks;Integrated Security=True;Persist Security Info=False"
  }
```

To inject builders in repository classes that uses statements and transactions, in the application initialization (typically `Program.cs` file) uses a code like the example below (which is for a .Net API application):

```csharp
using AdoNet.Fluent.SqlServer;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var services = builder.Services;

services.AddSingleton<ISqlServerStatementBuilder>(new SqlServerStatementBuilder(config, "DefaultConnection"));
services.AddSingleton<ISqlServerTransactionBuilder>(new SqlServerTransactionBuilder(config, "DefaultConnection"));
```

### Basic CRUD Operations

The example below is a Repository that performs CRUD operations in AdventureWorks database, using `SqlServerStatement` for them.

```csharp
using AdoNet.Fluent.SqlServer;

public sealed class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? GroupName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public List<Shift>? Shifts { get; set; }
}

public sealed class CRUDRepository(ISqlServerStatementBuilder builder)
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

        int? id = _statement
            .SetSql(InsertDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate)
            .ScalarInt32();

        return id ?? 0;
    }

    public async Task<int> CreateAsync(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        int? id = await _statement
            .SetSql(InsertDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate)
            .ScalarInt32Async();

        return id ?? 0;
    }

    public void Delete(int id)
    {
        using SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(DeleteDepartment)
            .AddInParameter("DepartmentID", id)
            .Execute();
    }

    public async Task DeleteAsync(int id)
    {
        using SqlServerStatement _statement = _builder.Build();

        await _statement
            .SetSql(DeleteDepartment)
            .AddInParameter("DepartmentID", id)
            .ExecuteAsync();
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
            .AddOutParameter("ModifiedDate", NumericType.DateTime)
            .Execute();

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

        await _statement
            .SetSql(SelectDepartment)
            .AddInParameter("DepartmentID", id)
            .AddOutParameter("Name", 50)
            .AddOutParameter("GroupName", 50)
            .AddOutParameter("ModifiedDate", NumericType.DateTime)
            .ExecuteAsync();

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
            .AddInParameter("DepartmentID", department.Id)
            .Execute();
    }

    public async Task UpdateAsync(Department department)
    {
        using SqlServerStatement _statement = _builder.Build();

        await _statement
            .SetSql(UpdateDepartment)
            .AddInParameter("Name", department.Name, 50)
            .AddInParameter("GroupName", department.GroupName, 50)
            .AddInParameter("ModifiedDate", department.ModifiedDate)
            .AddInParameter("DepartmentID", department.Id)
            .ExecuteAsync();
    }
}

```

### Read many lines

For better performance, AdoNet.Fluent.SqlServer uses `SqlDataReader` under the hood, which is [DataReader](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/retrieving-data-using-a-datareader) implementation for SQL Server.
The example below shows a repository class that implements read multiple lines:

```csharp
using System.Data;
using System.Data.Common;
using AdoNet.Fluent;

public class Hierarchy
{
    public int Id { get; set; }

    public required int RecursionLevel { get; set; }

    public required string OrganizationNode { get; set; }

    public required string ManagerFirstName { get; set; }

    public required string ManagerLastName { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}

public sealed class ReadRepository(ISqlServerStatementBuilder builder)
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

```
