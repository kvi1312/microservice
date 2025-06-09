using Hangfire;
using Hangfire.API.Extensions;
using Infrastructure.ScheduledJobs;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurations(builder.Configuration);
    builder.Services.AddInfrastructureHangfireService();
    builder.Services.ConfigureServices();
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
    Log.Information("Shut down ScheduledJob complete");
    Log.CloseAndFlush();
}
