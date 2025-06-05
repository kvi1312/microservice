using Carter;
using Common.Logging;
using Customer.API.Extensions;
using Customer.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

Log.Information("Starting Customer API up");

try
{
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurations(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCarter();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddHostedService<CustomerSeeder>();
    var app = builder.Build();
    app.MapCarter();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseRouting();
    app.UseAuthorization();
    app.UseCustomHangfireDashboard(builder.Configuration);
    app.UseEndpoints((endpoint) => {
        endpoint.MapDefaultControllerRoute();
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
