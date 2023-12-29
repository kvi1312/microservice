using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure); // config logger để reference tới common logging với Configure là Action được xác định trong file common logging
Log.Information("Starting product API up");

try
{

    builder.Host.UseSerilog((hostBuilder, loggerConfiguration) =>
    loggerConfiguration.WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} {NewLine}{Message:lj}{NewLine}{Exception}{NewLine}") // template 
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(hostBuilder.Configuration));
    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex) {
    Log.Fatal(ex, "Unhandle Exception");
}
finally
{
    Log.Information("Shut down product API complete");
    Log.CloseAndFlush();
}


