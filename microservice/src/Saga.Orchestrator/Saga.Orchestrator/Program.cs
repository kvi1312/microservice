using Carter;
using Common.Logging;
using Saga.Orchestrator;
using Saga.Orchestrator.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

try
{
    builder.Host.AddAppConfigurations();
    builder.Services.ConfigureServices();
    builder.Services.ConfigureHttpRepositories();
    builder.Services.ConfigureHttpClients();
    builder.Services.AddCarter();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
    }

    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
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