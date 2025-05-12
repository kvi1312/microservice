// using Inventory.Grpc.Services;

using Common.Logging;
using Inventory.Product.Grpc.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureMongoDbClient();
    builder.Services.AddInfrastructureServices();
    builder.Services.AddGrpc();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // app.MapGrpcService<GreeterService>();
    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, $"Unhandled Exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}
