using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal sealed class ReadCommand(IReadService<Hierarchy> service, ILogger<ReadCommand> logger, JsonSerializerOptions options) : Command(logger)
{
    private readonly IReadService<Hierarchy> _service = service;

    private readonly JsonSerializerOptions _options = options;

    public override void Execute()
    {
        int businessEntityID = 16;

        try
        {
            Logger.LogInformation("Reading Hierarchy from BusinessUnitId {businessEntityID}...", businessEntityID);
            List<Hierarchy> hierarchies = _service.Read(businessEntityID);
            Logger.LogInformation("Hierarchies:\r\n{hierarchies}", JsonSerializer.Serialize(hierarchies, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ReadCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        int businessEntityID = 2;

        try
        {    
            Logger.LogInformation("Reading Hierarchy from BusinessUnitId {businessEntityID}...", businessEntityID);
            List<Hierarchy> hierarchies = await _service.ReadAsync(businessEntityID);
            Logger.LogInformation("Hierarchies:\r\n{hierarchies}\r\n", JsonSerializer.Serialize(hierarchies, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ReadCommand.Execute() exception");
            throw;
        }
    }
}
