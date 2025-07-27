using Common.Logging;
using Contracts.Identity;
using Infrastructure.Identity;
using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;
Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json").Build();

builder.Host.AddAppConfigurations();
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOcelot(builder.Configuration);
builder.Services.ConfigureCors(builder.Configuration);
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddSwaggerForOcelot(configuration);
builder.Services.ConfigureAuthenticationHandler();
var app = builder.Build();

try
{
    app.UseCors("CorsPolicy");
    app.UseMiddleware<ErrorWrappingMiddleware>();

    app.UseRouting();
    app.UseEndpoints(e =>
    {
        e.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("hello world");
        });
    });
    app.MapControllers();
    app.UseSwaggerForOcelotUI(c =>
    {
        c.PathToSwaggerGenerator = "/swagger/docs";
    }, uiConfig =>
    {
        uiConfig.OAuthClientId("microservices_swagger");
        uiConfig.DisplayRequestDuration();
    });
    await app.UseOcelot();

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
