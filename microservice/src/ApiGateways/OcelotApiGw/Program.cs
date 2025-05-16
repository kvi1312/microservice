using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json").Build();

builder.Host.AddAppConfigurations();
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOcelot(builder.Configuration);
builder.Services.ConfigureCors(builder.Configuration);

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

    app.UseMiddleware<ErrorWrappingMiddleware>();

    await app.UseOcelot();

    app.UseAuthorization();

    app.MapControllers();

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
