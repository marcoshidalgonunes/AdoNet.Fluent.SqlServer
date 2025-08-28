using System.Text.Json;
using System.Text.Json.Serialization;
using AdoNet.Fluent.SqlServer;
using AdoNet.Fluent.SqlServer.Demo.Commands;
using AdoNet.Fluent.SqlServer.Demo.Models;
using AdoNet.Fluent.SqlServer.Demo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var config = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json")
    .Build();

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "[HH:mm:ss.fff] ";
});

var services = builder.Services;

services.AddSingleton<ISqlServerStatementBuilder>(new SqlServerStatementBuilder(config, "DefaultConnection"));
services.AddSingleton<ISqlServerTransactionBuilder>(new SqlServerTransactionBuilder(config, "DefaultConnection"));
services.AddSingleton(new JsonSerializerOptions() { 
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
    WriteIndented = true 
});

// Services that use AdoNet.Fluent.SqlServer functionality
services.AddScoped<ICRUDService<Department>, CRUDService>();
services.AddScoped<IReadService<Hierarchy>, ReadService>();
services.AddScoped<IMARSService<Department>, MARSService>();
services.AddScoped<ITransactionService<Department>, TransactionService>();
services.AddScoped<IPrepareService, PrepareService>();
services.AddScoped<IImageService, ImageService>();
services.AddScoped<IXMLService, XMLService>();

// Commands requesting services using AdoNet.Fluent.SqlServer functionality
services.AddScoped<CRUDCommand>();
services.AddScoped<ReadCommand>();
services.AddScoped<MARSCommand>();
services.AddScoped<TransactionCommand>();
services.AddScoped<PrepareCommand>();
services.AddScoped<ImageCommand>();
services.AddScoped<XMLCommand>();

using IHost host = builder.Build();

try
{
    CRUDCommand commands = host.Services.GetRequiredService<CRUDCommand>();
    commands.Execute();
    await commands.ExecuteAsync();

    ReadCommand read = host.Services.GetRequiredService<ReadCommand>();
    read.Execute();
    await read.ExecuteAsync();

    MARSCommand mars = host.Services.GetRequiredService<MARSCommand>();
    mars.Execute();
    await mars.ExecuteAsync();

    TransactionCommand transaction = host.Services.GetRequiredService<TransactionCommand>();
    transaction.Execute();
    await transaction.ExecuteAsync();

    PrepareCommand prepare = host.Services.GetRequiredService<PrepareCommand>();
    prepare.Execute();
    await prepare.ExecuteAsync();

    ImageCommand image = host.Services.GetRequiredService<ImageCommand>();
    image.Execute();
    await image.ExecuteAsync();

    XMLCommand xml = host.Services.GetRequiredService<XMLCommand>();
    xml.Execute();
    await xml.ExecuteAsync();
}
catch (Exception)
{
    Environment.Exit(1);
}
