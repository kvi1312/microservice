using Basket.API;
using Basket.API.Extensions;
using Carter;
using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);
Log.Information("Starting Basket API up");
// Add services to the container.
try
{
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Host.AddAppConfigurations();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCarter();
    builder.Services.ConfigureServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });
    builder.Services.ConfigureMasstransit();
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile(new MappingProfile());
    });
    var app = builder.Build();
    app.MapCarter();
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
    Log.Fatal(ex, "Unhandle exception");
}
finally
{
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}
