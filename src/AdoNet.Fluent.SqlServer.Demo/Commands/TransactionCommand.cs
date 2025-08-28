using System.Text.Json;
using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal class TransactionCommand(ITransactionService<Department> service, ILogger<TransactionCommand> logger, JsonSerializerOptions options) : Command(logger)
{
    private readonly ITransactionService<Department> _service = service;

    private readonly JsonSerializerOptions _options = options;

    private readonly Department _department = new()
    {
        Name = "Information Technology",
        GroupName = "Executive General and Administration",
        ModifiedDate = DateTime.Today
    };

    public override void Execute()
    {
        try
        {
            Logger.LogInformation("Creating department '{name}' and its history...", _department.Name);
            _department.Id = _service.Execute(_department);

            Logger.LogInformation("Department:\r\n{department}", JsonSerializer.Serialize(_department, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "TransactionCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            Logger.LogInformation("Deleting department '{name}' and its history...\r\n", _department.Name);
            await _service.ExecuteAsync(_department);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "TransactionCommand.Execute() exception");
            throw;
        }
    }
}
