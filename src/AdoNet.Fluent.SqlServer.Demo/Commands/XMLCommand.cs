using System.Text.Json;
using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal class XMLCommand(IXMLService service, ILogger<XMLCommand> logger, JsonSerializerOptions options) : Command(logger)
{
    private readonly IXMLService _service = service;

    private readonly JsonSerializerOptions _options = options;

    public override void Execute()
    {
        int businessEntityID = 298;
        int salesPersonId = 281;

        try
        {
            Logger.LogInformation("Reading Store Survey from businessEntityID {businessEntityID}...", businessEntityID);
            StoreSurvey storeSurvey = _service.Find(businessEntityID);
            Logger.LogInformation("Store Survey :\r\n{storeSurvey}", JsonSerializer.Serialize(storeSurvey, _options));

            storeSurvey.AnnualSales = DateTime.Now.Second * 100000;
            Logger.LogInformation("Updating Store Survey of salesPersonId {salesPersonId}...", salesPersonId);
            _service.Update(salesPersonId, storeSurvey);
            Logger.LogInformation("Store Survey :\r\n{storeSurvey}", JsonSerializer.Serialize(storeSurvey, _options));

            Logger.LogInformation("Reading Stores Survey from salesPersonId {salesPersonId}...", salesPersonId);
            List<StoreSurvey> storesSurvey = _service.Read(salesPersonId);
            Logger.LogInformation("Stores Survey :\r\n{storesSurvey}", JsonSerializer.Serialize(storesSurvey, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ReadCommand.Execute() exception");
            throw;
        }
    }

    public override async Task ExecuteAsync()
    {
        int businessEntityID = 298;
        int salesPersonId = 279;

        try
        {
            Logger.LogInformation("Reading Store Survey from businessEntityID {businessEntityID}...", businessEntityID);
            StoreSurvey storeSurvey = await _service.FindAsync(businessEntityID);
            Logger.LogInformation("Store Survey :\r\n{storeSurvey}", JsonSerializer.Serialize(storeSurvey, _options));

            storeSurvey.AnnualSales = DateTime.Now.Second * 100000;
            Logger.LogInformation("Updating Store Survey of salesPersonId {salesPersonId}...", salesPersonId);
            _service.Update(salesPersonId, storeSurvey);
            Logger.LogInformation("Store Survey :\r\n{storeSurvey}", JsonSerializer.Serialize(storeSurvey, _options));

            Logger.LogInformation("Reading Stores Survey  from salesPersonId {salesPersonId}...", salesPersonId);
            List<StoreSurvey> storesSurvey = await _service.ReadAsync(salesPersonId);
            Logger.LogInformation("Stores Survey :\r\n{storesSurvey}\r\n", JsonSerializer.Serialize(storesSurvey, _options));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ReadCommand.ExecuteAsync() exception");
            throw;
        }
    }
}
