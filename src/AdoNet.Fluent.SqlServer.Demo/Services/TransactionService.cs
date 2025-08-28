using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal sealed class TransactionService(ISqlServerTransactionBuilder builder) : ITransactionService<Department>
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
            transaction
                .SetSql(InsertDepartment)
                .AddInParameter("Name", department.Name, 50)
                .AddInParameter("GroupName", department.GroupName, 50)
                .AddInParameter("ModifiedDate", department.ModifiedDate);

            departmentId = transaction.ScalarInt32() ?? 0;

            EmployeeDepartmentHistory employeeDepartmentHistory = new()
            {
                Id = 1,
                ShiftID = 1,
                DepartmentID = departmentId
            };
            ;
            if (employeeDepartmentHistory.DepartmentID > 0)
            {
                transaction
                    .SetSql(InsertDepartmentHistory)
                    .AddInParameter("BusinessEntityID", employeeDepartmentHistory.Id)
                    .AddInParameter("DepartmentId", employeeDepartmentHistory.DepartmentID)
                    .AddInParameter("ShiftID", employeeDepartmentHistory.ShiftID);

                transaction.Execute();

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
            transaction
                .SetSql(DeleteDepartmentHistory)
                .AddInParameter("DepartmentID", departmentId);

            await transaction.ExecuteAsync();

            transaction
                .SetSql(DeleteDepartment)
                .AddInParameter("DepartmentID", departmentId);

            await transaction.ExecuteAsync();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
