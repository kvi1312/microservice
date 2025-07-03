using Carter;
using Common.Logging;
using Hangfire.API.Extensions;
using HealthChecks.UI.Client;
using Infrastructure.ScheduledJobs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurations(builder.Configuration);
    builder.Services.AddInfrastructureHangfireService();
    builder.Services.ConfigureServices();
    builder.Services.AddCarter();
    builder.Services.ConfigureHealthChecks();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
    app.UseCustomHangfireDashboard(builder.Configuration);
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
    });
    app.MapCarter();
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
    Log.Information("Shut down ScheduledJob complete");
    Log.CloseAndFlush();
}
