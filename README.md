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
using AdoNet.Fluent.SqlServer;

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
    private readonly ISqlServerStatementBuilder _builder = builder;

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
}
```

### Read Multiple Active Result Sets (MARS)

AdoNet.Fluent.SqlServer supports [MARS](https://learn.microsoft.com/en-us/sql/relational-databases/native-client/features/using-multiple-active-result-sets-mars) feature of SQL Server.

It is enabled via `WithMARS()` method of `SqlServerStatementBuilder`, therefore is no need to enable MARS in connection string configuration. 

```csharp
using System.Data;
using System.Data.Common;
using AdoNet.Fluent.SqlServer;

public sealed class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? GroupName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public List<Shift>? Shifts { get; set; }
}

public sealed class Shift
{
    public int Id { get; set; }

    public string? Name { get; set; }
}

public class MARSRepository(ISqlServerStatementBuilder builder)
{
    private readonly ISqlServerStatementBuilder _builder = builder;

    private SqlServerStatement? _statement;

    private readonly List<Department> _departments = [];

    private readonly List<Shift> _shifts = [];

    private int ordDeptId, ordName, ordGroupName, ordModifiedDate, ordShiftId, ordShiftName;

    private const string SelectDepartment = @"
        SELECT 
            DepartmentID,
            [Name],
            GroupName,
            ModifiedDate
        FROM HumanResources.Department 
        WHERE GroupName = @GroupName
    ";

    private const string SelectShift = @"
        SELECT DISTINCT
            S.ShiftID,
            S.[Name]
        FROM HumanResources.[Shift] S 
        INNER JOIN HumanResources.EmployeeDepartmentHistory EDH
        ON S.ShiftID = EDH.DepartmentID
        WHERE EDH.DepartmentID = @DepartmentID
    ";

    private void FillDepartment(DbDataReader reader)
    {
        short departmentId = reader.GetInt16(ordDeptId);
        _shifts.Clear();

        _statement?.SetParameter("DepartmentID", departmentId);
        _statement?.Read(SetOrdinalShift, FillShift);

        _departments.Add(new Department()
        {
            Id = departmentId,
            Name = reader.GetString(ordName),
            GroupName = reader.GetString(ordGroupName),
            ModifiedDate = reader.GetDateTime(ordModifiedDate),
            Shifts = new List<Shift>(_shifts)
        });
    }

    private void FillShift(DbDataReader reader)
    {
        _shifts.Add(new Shift { Id = reader.GetByte(ordShiftId), Name = reader.GetString(ordShiftName) });   
    }

    private void SetOrdinalDepartment(IDataRecord reader)
    {
        ordDeptId = reader.GetOrdinal("DepartmentID");
        ordName = reader.GetOrdinal("Name");
        ordGroupName = reader.GetOrdinal("GroupName");
        ordModifiedDate = reader.GetOrdinal("ModifiedDate");

        _statement?
            .SetSql(SelectShift)
            .AddInParameter("DepartmentID", NumericType.Int16);
    }

    private void SetOrdinalShift(IDataRecord reader)
    {
        ordShiftId = reader.GetOrdinal("ShiftID");
        ordShiftName = reader.GetOrdinal("Name");
    }

    public List<Department> Read(string name)
    {
        using (_statement = _builder.WithMARS().Build())
        {
            _statement
                .SetSql(SelectDepartment)
                .AddInParameter("GroupName", name, 50)
                .Read(SetOrdinalDepartment, FillDepartment);
        }

        return _departments;
    }

    public async Task<List<Department>> ReadAsync(string name)
    {
        using (_statement = _builder.WithMARS().Build())
        {
            await _statement
                .SetSql(SelectDepartment)
                .AddInParameter("GroupName", name, 50)
                .ReadAsync(SetOrdinalDepartment, FillDepartment);
        }

        return _departments;
    }
}
```

### Operations with Binary data

AdoNet.Fluent.SqlServer supports operations with binary data (e.g. media content such as images, audio and video).

> **Note** This example also shows how to get values from queries that bings only one row and column via "Scalar" methods, which are available for other data types.

```csharp
using AdoNet.Fluent.SqlServer;

public class ImageRepository(ISqlServerStatementBuilder builder)
{
    private readonly ISqlServerStatementBuilder _builder = builder;

    private const string SelectImage = "SELECT ThumbNailPhoto FROM Production.ProductPhoto WHERE ProductPhotoID = @Id";

    private const string UpdateImage = "UPDATE Production.ProductPhoto SET ThumbNailPhoto = @Image WHERE ProductPhotoID = @Id";

    public byte[]? Get(int id)
    {
        SqlServerStatement _statement = _builder.Build();

        return _statement
            .SetSql(SelectImage)
            .AddInParameter("Id", id)
            .ScalarBinary();
    }

    public async Task<byte[]?> GetAsync(int id)
    {
        SqlServerStatement _statement = _builder.Build();

        return await _statement
            .SetSql(SelectImage)
            .AddInParameter("Id", id)
            .ScalarBinaryAsync();
    }

    public void Save(int id, byte[] data)
    {
        SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(UpdateImage)
            .AddInParameter("Image", data)
            .AddInParameter("Id", id)
            .Execute();
    }

    public async Task SaveAsync(int id, byte[] data)
    {
        SqlServerStatement _statement = _builder.Build();

        await _statement
            .SetSql(UpdateImage)
            .AddInParameter("Image", data)
            .AddInParameter("Id", id)
            .ExecuteAsync();
    }
}
```

### Operations with XML data

AdoNet.Fluent.SqlServer supports SQL Server features to deal with [XML](https://en.wikipedia.org/wiki/XML) data.

> **Note** This example also shows how to get values from queries that bings only one row and column via "Scalar" methods, which are available for other data types.

```csharp
using System.Data;
using System.Data.Common;
using System.Xml;
using AdoNet.Fluent.SqlServer;

public class StoreSurvey
{
    public int AnnualSales { get; set; }

    public int AnnualRevenue { get; set; }

    public string? BankName { get; set; }

    public string? BusinessType { get; set; }

    public int YearOpened { get; set; } 

    public string? Specialty { get; set; }

    public int SquareFeet { get; set; }

    public string? Brands { get; set; }

    public string? Internet { get; set; }

    public int NumberEmployees { get; set; }
}

public sealed class XMLRepository(ISqlServerStatementBuilder builder)
{
    private StoreSurvey _storeSurvey = new();

    private readonly List<StoreSurvey> _storesSurvey = [];

    private const string FindDemographics = @"
        SELECT Demographics from Sales.Store 
        WHERE BusinessEntityID = @BusinessEntityID
    ";

    private const string SelectDemographics = @"
        SELECT Demographics from Sales.Store 
        WHERE SalesPersonID = @SalesPersonID
    ";

    private const string UpdateDemographics = @"
        UPDATE Sales.Store
            SET Demographics = @Demographics
        WHERE SalesPersonID = @SalesPersonID
    ";

    private int ordDemographics;

    private static XmlNodeReader ConvertToXml(StoreSurvey storeSurvey)
    {
        XmlDocument doc = new();

        XmlElement root = doc.CreateElement("StoreSurvey", "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/StoreSurvey");
        doc.AppendChild(root);

        string? namespaceUri = doc.DocumentElement?.NamespaceURI;

        XmlElement annualSales = doc.CreateElement("AnnualSales", namespaceUri);
        annualSales.InnerText = storeSurvey.AnnualSales.ToString();
        root.AppendChild(annualSales);

        XmlElement annualRevenue = doc.CreateElement("AnnualRevenue", namespaceUri);
        annualRevenue.InnerText = storeSurvey.AnnualRevenue.ToString();
        root.AppendChild(annualRevenue);

        XmlElement bankName = doc.CreateElement("BankName", namespaceUri);
        bankName.InnerText = storeSurvey.BankName ?? string.Empty;
        root.AppendChild(bankName);

        XmlElement businessType = doc.CreateElement("BusinessType", namespaceUri);
        businessType.InnerText = storeSurvey.BusinessType ?? string.Empty;
        root.AppendChild(businessType);

        XmlElement yearOpened = doc.CreateElement("YearOpened", namespaceUri);
        yearOpened.InnerText = storeSurvey.YearOpened.ToString();
        root.AppendChild(yearOpened);

        XmlElement specialty = doc.CreateElement("Specialty", namespaceUri);
        specialty.InnerText = storeSurvey.Specialty ?? string.Empty;
        root.AppendChild(specialty);

        XmlElement squareFeet = doc.CreateElement("SquareFeet", namespaceUri);
        squareFeet.InnerText = storeSurvey.SquareFeet.ToString();
        root.AppendChild(squareFeet);

        XmlElement brands = doc.CreateElement("Brands", namespaceUri);
        brands.InnerText = storeSurvey.Brands ?? string.Empty;
        root.AppendChild(brands);

        XmlElement internet = doc.CreateElement("Internet", namespaceUri);
        internet.InnerText = storeSurvey.Internet ?? string.Empty;
        root.AppendChild(internet);

        XmlElement numberEmployees = doc.CreateElement("NumberEmployees", namespaceUri);
        numberEmployees.InnerText = storeSurvey.NumberEmployees.ToString();
        root.AppendChild(numberEmployees);

        return new XmlNodeReader(doc);
    }

    private void Fill(DbDataReader reader)
    {
        using XmlReader? salesReaderXml = reader.GetXml(ordDemographics);
        if (salesReaderXml is not null)
        {
            StoreSurvey storeSurvey = GetStoreSurvey(salesReaderXml);
            _storesSurvey.Add(storeSurvey);
        }
    }

    private void FillXml(XmlReader salesReaderXml)
    {
        _storeSurvey = GetStoreSurvey(salesReaderXml);
    }

    private async Task FillXmlAsync(XmlReader salesReaderXml)
    {
        _storeSurvey = await GetStoreSurveyAsync(salesReaderXml);
    }

    private static StoreSurvey GetStoreSurvey(XmlReader salesReaderXml)
    {
        StoreSurvey storeSurvey = new();

        while (salesReaderXml.Read())
        {
            if (salesReaderXml.NodeType == XmlNodeType.Element)
            {
                var elementLocalName = salesReaderXml.LocalName;
                salesReaderXml.Read();

                switch (elementLocalName)
                {
                    case "AnnualSales":
                        storeSurvey.AnnualSales = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "AnnualRevenue":
                        storeSurvey.AnnualRevenue = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "BankName":
                        storeSurvey.BankName = salesReaderXml.Value;
                        break;
                    case "BusinessType":
                        storeSurvey.BusinessType = salesReaderXml.Value;
                        break;
                    case "YearOpened":
                        storeSurvey.YearOpened = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Specialty":
                        storeSurvey.Specialty = salesReaderXml.Value;
                        break;
                    case "SquareFeet":
                        storeSurvey.SquareFeet = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Brands":
                        storeSurvey.Brands = salesReaderXml.Value;
                        break;
                    case "Internet":
                        storeSurvey.Internet = salesReaderXml.Value;
                        break;
                    case "NumberEmployees":
                        storeSurvey.NumberEmployees = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        return storeSurvey;
    }

    private static async Task<StoreSurvey> GetStoreSurveyAsync(XmlReader salesReaderXml)
    {
        StoreSurvey storeSurvey = new();

        while (await salesReaderXml.ReadAsync())
        {
            if (salesReaderXml.NodeType == XmlNodeType.Element)
            {
                var elementLocalName = salesReaderXml.LocalName;
                await salesReaderXml.ReadAsync();

                switch (elementLocalName)
                {
                    case "AnnualSales":
                        storeSurvey.AnnualSales = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "AnnualRevenue":
                        storeSurvey.AnnualRevenue = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "BankName":
                        storeSurvey.BankName = salesReaderXml.Value;
                        break;
                    case "BusinessType":
                        storeSurvey.BusinessType = salesReaderXml.Value;
                        break;
                    case "YearOpened":
                        storeSurvey.YearOpened = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Specialty":
                        storeSurvey.Specialty = salesReaderXml.Value;
                        break;
                    case "SquareFeet":
                        storeSurvey.SquareFeet = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Brands":
                        storeSurvey.Brands = salesReaderXml.Value;
                        break;
                    case "Internet":
                        storeSurvey.Internet = salesReaderXml.Value;
                        break;
                    case "NumberEmployees":
                        storeSurvey.NumberEmployees = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        return storeSurvey;
    }

    private void SetOrdinal(IDataRecord reader)
    {
        ordDemographics = reader.GetOrdinal("Demographics");
    }

    public StoreSurvey Find(int id)
    {
        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(FindDemographics)
            .AddInParameter("BusinessEntityID", id)
            .ScalarXml(FillXml);

        return _storeSurvey;
    }

    public async Task<StoreSurvey> FindAsync(int id)
    {
        using SqlServerStatement statement = builder.Build();
        
        await statement
            .SetSql(FindDemographics)
            .AddInParameter("BusinessEntityID", id)
            .ScalarXmlAsync(FillXmlAsync);

        return _storeSurvey;
    }

    public List<StoreSurvey> Read(int id)
    {
        _storesSurvey.Clear();

        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(SelectDemographics)
            .AddInParameter("SalesPersonID", id)
            .Read(SetOrdinal, Fill);

        return _storesSurvey;
    }

    public async Task<List<StoreSurvey>> ReadAsync(int id)
    {
        _storesSurvey.Clear();

        using SqlServerStatement statement = builder.Build();

        await statement
            .SetSql(SelectDemographics)
            .AddInParameter("SalesPersonID", id)
            .ReadAsync(SetOrdinal, Fill);

        return _storesSurvey;
    }

    public void Update(int id, StoreSurvey storeSurvey)
    {
        using SqlServerStatement statement = builder.Build();
        using XmlReader xmlReader = ConvertToXml(storeSurvey);

        statement
            .SetSql(UpdateDemographics)
            .AddInParameter("Demographics", xmlReader)
            .AddInParameter("SalesPersonID", id)
            .Execute();
    }

    public async Task UpdateAsync(int id, StoreSurvey storeSurvey)
    {
        using SqlServerStatement statement = builder.Build();
        using XmlReader xmlReader = ConvertToXml(storeSurvey);

        await statement
            .SetSql(UpdateDemographics)
            .AddInParameter("Demographics", xmlReader)
            .AddInParameter("SalesPersonID", id)
            .ExecuteAsync();
    }
}
```

### Operations with prepared statements

AdoNet.Fluent.SqlServer supports ADO.Net prepared statements features, which increases performance when the same SQL operation is executed multiple times only changing values of parameters.

> **Note** The async method of example below is for sake of simplicity. Of course there are better ways to delete rows in a table.

```csharp
using AdoNet.Fluent.SqlServer;

public class PrepareRepository(ISqlServerStatementBuilder builder)
{
    private const string InsertDepartmentHistory = @"
        INSERT INTO HumanResources.EmployeeDepartmentHistory
            (BusinessEntityID, DepartmentId, ShiftID, StartDate, ModifiedDate)
        VALUES 
            (@BusinessEntityID, @DepartmentId, 3, CONVERT(Date, GETDATE()), GETDATE())
    ";

    private const string DeleteDepartmentHistory = @"
        DELETE FROM HumanResources.EmployeeDepartmentHistory
        WHERE BusinessEntityID = @BusinessEntityID And DepartmentID = @DepartmentID AND ShiftID = 3
    ";

    public void Execute(int departmentId, int[] businessEntityID)
    {
        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(InsertDepartmentHistory)
            .AddInParameter("BusinessEntityID", NumericType.Int32)
            .AddInParameter("DepartmentID", departmentId);

        statement.Prepare();

        for (int i = 0; i < businessEntityID.Length; i++)
        {
            statement
                .SetParameter("BusinessEntityID", businessEntityID[i])
                .Execute();
        }   
    }

    public async Task ExecuteAsync(int departmentId, int[] businessEntityID)
    {
        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(DeleteDepartmentHistory)
            .AddInParameter("BusinessEntityID", NumericType.Int32)
            .AddInParameter("DepartmentID", departmentId);

        await statement.PrepareAsync();

        for (int i = 0; i < businessEntityID.Length; i++)
        {
            await statement
                .SetParameter("BusinessEntityID", businessEntityID[i])
                .ExecuteAsync();
        }
    }
}
```

### Database transactions handling

AdoNet.Fluent.SqlServer supports SQL Server transactions handling via ADO.Net client with `SqlServerTransaction` class and its corresponding `SqlServerTransactionBuilder`.

> **Note** The async method of example below is for sake of simplicity. Of course there are better ways to delete rows in tables.

```csharp
using AdoNet.Fluent.SqlServer;


public sealed class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? GroupName { get; set; }

    public DateTime ModifiedDate { get; set; }
}

public class EmployeeDepartmentHistory : Entity
{
    public int Id { get; set; }

    public int DepartmentID { get; set; }

    public int ShiftID { get; set; }
}

public sealed class TransactionRepository(ISqlServerTransactionBuilder builder)
{
    private const string InsertDepartment = @"
        INSERT INTO HumanResources.Department
            ([Name], GroupName, ModifiedDate)
        OUTPUT INSERTED.DepartmentID
        VALUES 
            (@Name, @GroupName, @ModifiedDate)
    ";

    private const string InsertDepartmentHistory = @"
        INSERT INTO HumanResources.EmployeeDepartmentHistory
            (BusinessEntityID, DepartmentId, ShiftID, StartDate, ModifiedDate)
        VALUES 
            (@BusinessEntityID, @DepartmentId, @ShiftID, CONVERT(Date, GETDATE()), GETDATE())
    ";

    private const string DeleteDepartmentHistory = @"
        DELETE FROM HumanResources.EmployeeDepartmentHistory
        WHERE DepartmentID = @DepartmentID
    ";

    private const string DeleteDepartment = @"
        DELETE FROM HumanResources.Department
        WHERE DepartmentID = @DepartmentID
    ";

    public int Execute(Department department)
    {
        int departmentId;
        using SqlServerTransaction transaction = builder.Build();

        try
        {
            departmentId = transaction
                .SetSql(InsertDepartment)
                .AddInParameter("Name", department.Name, 50)
                .AddInParameter("GroupName", department.GroupName, 50)
                .AddInParameter("ModifiedDate", department.ModifiedDate)
                .ScalarInt32() ?? 0;

            EmployeeDepartmentHistory employeeDepartmentHistory = new()
            {
                Id = 1,
                ShiftID = 1,
                DepartmentID = departmentId
            };
            
            if (employeeDepartmentHistory.DepartmentID > 0)
            {
                transaction
                    .SetSql(InsertDepartmentHistory)
                    .AddInParameter("BusinessEntityID", employeeDepartmentHistory.Id)
                    .AddInParameter("DepartmentId", employeeDepartmentHistory.DepartmentID)
                    .AddInParameter("ShiftID", employeeDepartmentHistory.ShiftID)
                    .Execute();

                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }

        return departmentId;   
    }

    public async Task ExecuteAsync(Department department)
    {
        int? departmentId = department.Id;
        using SqlServerTransaction transaction = builder.Build();

        try
        {
            await transaction
                .SetSql(DeleteDepartmentHistory)
                .AddInParameter("DepartmentID", departmentId)
                .ExecuteAsync();

            await transaction
                .SetSql(DeleteDepartment)
                .AddInParameter("DepartmentID", departmentId)
                .ExecuteAsync();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
```