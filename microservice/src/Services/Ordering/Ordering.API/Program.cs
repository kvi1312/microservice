using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;
using Common.Logging;
using Ordering.API.Extensions;
using Ordering.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHostedService<OrderContextSeed>();
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Host.AddAppConfigurations();
Log.Information("Starting Ordering API up");
// Add services to the container.
try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

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

    
    Console.WriteLine("EXCEPTION: " + ex.Message);
    if (ex.InnerException != null)
    {
        Console.WriteLine("INNER: " + ex.InnerException.Message);
    }
    
    Log.Fatal(ex, "Unhandled Exception");
}
finally
{
    Log.Information("Shut down Ordering API complete");
    Log.CloseAndFlush();
}
