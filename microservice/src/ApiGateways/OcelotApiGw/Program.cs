using Common.Logging;
using Contracts.Identity;
using Infrastructure.Identity;
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
builder.Services.AddJwtAuthentication();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
try
{
    //app.UseHttpsRedirection();

    app.UseCors("CorsPolicy");

    //app.UseMiddleware<ErrorWrappingMiddleware>();
    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(e =>
    {
        e.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("hello world");
        });
    });
    app.MapControllers();
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
