namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal class PrepareService(ISqlServerStatementBuilder builder) : IPrepareService
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
