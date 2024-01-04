using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting product API up");

try
{
    builder.Host.UseSerilog(SeriLogger.Configure); // config logger for reference to common logging with Configure is Action is function was identified in file common logging
    builder.Host.AddAppConfigurations(); // using UseInfrastructure function from ConfigureHostExtensions
    builder.Services.AddInfrastructure(builder.Configuration); // using UseInfrastructure function from ServiceExtensions
    var app = builder.Build();
    app.UseInfrastructure(); // using UseInfrastructure function from ApplicationExtensions
    
    app.MigrateDataBase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    });
    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }
    Log.Fatal(ex, "Unhandle Exception");
}
finally
{
    Log.Information("Shut down product API complete");
    Log.CloseAndFlush();
}


