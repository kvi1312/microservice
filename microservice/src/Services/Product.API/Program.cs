using Carter;
using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting product API up");

try
{
    // builder.Services.AddCarter();
    builder.Host.UseSerilog(SeriLogger.Configure);
    builder.Host.AddAppConfigurations();
    builder.Services.AddInfrastructure(builder.Configuration);
    var app = builder.Build();
    // app.MapCarter();
    app.UseInfrastructure();
    app.MigrateDataBase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    });
    app.Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled Exception");
}
finally
{
    Log.Information("Shut down product API complete");
    Log.CloseAndFlush();
}