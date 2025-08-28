using System.Text.Json;
using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal sealed class CRUDCommand(ICRUDService<Department> service, ILogger<CRUDCommand> logger, JsonSerializerOptions options) : Command(logger)
{
    private readonly ICRUDService<Department> _service = service;

    private readonly JsonSerializerOptions _options = options;

    #region ICommand members

    public override void Execute()
    {
        try
        {
            Department department = new()
            {
                Name = "Information Technology",
                GroupName = "Executive General and Administration",
                ModifiedDate = DateTime.Today.AddDays(-1)
            };

            Logger.LogInformation("Creating Department '{name}'...", department.Name);
            department.Id = _service.Create(department);

            Logger.LogInformation("Reading Department '{name}'...", department.Name);
            department = _service.Read(department.Id);
            Logger.LogInformation("Department\r\n{department}", JsonSerializer.Serialize(department, _options));  

            department.ModifiedDate = DateTime.Today;
            Logger.LogInformation("Updating Department '{name}'...", department.Name);
            _service.Update(department);
            Logger.LogInformation("Department\r\n{department}", JsonSerializer.Serialize(department, _options));

            Logger.LogInformation("Deleting Department '{name}'...", department.Name);
            _service.Delete(department.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CRUDCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            Department department = new()
            {
                Name = "Information Technology",
                GroupName = "Executive General and Administration",
                ModifiedDate = DateTime.Today.AddDays(-1)
            };

            Logger.LogInformation("Creating Department '{name}'...", department.Name);
            department.Id = await _service.CreateAsync(department);

            Logger.LogInformation("Reading Department '{name}'...", department.Name);
            department = await _service.ReadAsync(department.Id);
            Logger.LogInformation("Department\r\n{department}", JsonSerializer.Serialize(department, _options));

            department.ModifiedDate = DateTime.Today;
            Logger.LogInformation("Updating Department '{name}'...", department.Name);
            await _service.UpdateAsync(department);
            Logger.LogInformation("Department\r\n{department}", JsonSerializer.Serialize(department, _options));

            Logger.LogInformation("Deleting Department '{name}'...\r\n", department.Name);
            await _service.DeleteAsync(department.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CRUDCommand.Execute() exception");
            throw;
        }
    }

    #endregion
}
