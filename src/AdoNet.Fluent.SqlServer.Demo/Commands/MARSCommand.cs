using System.Text.Json;
using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal class MARSCommand(IMARSService<Department> service, ILogger<MARSCommand> logger, JsonSerializerOptions options) : Command(logger)
{
    private readonly IMARSService<Department> _service = service;

    private readonly JsonSerializerOptions _options = options;

    private const string GroupName = "Research and Development";

    #region ICommand members

    public override void Execute()
    {
        try
        {
            Logger.LogInformation("Reading Departments of '{groupName}'...", GroupName);
            List<Department> departments = _service.Read(GroupName);
            Logger.LogInformation("Departments:\r\n{departments}", JsonSerializer.Serialize(departments, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "MARSCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            Logger.LogInformation("Reading Departments of '{groupName}'...", GroupName);
            List<Department> departments = await _service.ReadAsync(GroupName);
            Logger.LogInformation("Departments:\r\n{departments}\r\n", JsonSerializer.Serialize(departments, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "MARSCommand.Execute() exception");
            throw;
        }
    }

    #endregion
}
