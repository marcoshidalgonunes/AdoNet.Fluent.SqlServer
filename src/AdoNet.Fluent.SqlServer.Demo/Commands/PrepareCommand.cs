using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal sealed class PrepareCommand(IPrepareService service, ILogger<PrepareCommand> logger) : Command(logger)
{
    private readonly IPrepareService _service = service;

    private readonly int[] _businessEntityID = [2, 3, 4, 5, 6, 14, 15];

    private readonly int _departmentID = 1;

    public override void Execute()
    {
        try
        {
            Logger.LogInformation("Insert ShiftID 3 into EmployeeDepartmentHistory for {departmentID}...", _departmentID);
            _service.Execute(_departmentID, _businessEntityID);
            Logger.LogInformation("ShiftID 3 inserted into EmployeeDepartmentHistory for {departmentID}...", _departmentID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PrepareCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            Logger.LogInformation("Delete ShiftID 3 from EmployeeDepartmentHistory for {departmentID}...", _departmentID);
            await _service.ExecuteAsync(_departmentID, _businessEntityID);
            Logger.LogInformation("ShiftID 3 deleted from EmployeeDepartmentHistory for {departmentID}...\r\n", _departmentID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PrepareCommand.Execute() exception");
            throw;
        }
    }
}
