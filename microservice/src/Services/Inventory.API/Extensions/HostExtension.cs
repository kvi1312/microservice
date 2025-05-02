using Inventory.API.Persistence;
using MongoDB.Driver;

namespace Inventory.API.Extensions;

public static class HostExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serivices = scope.ServiceProvider;
        var settings = serivices.GetRequiredService<DatabaseSettings>();

        if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("MongoDB connection string is not configured.");

        var mongoClient = serivices.GetRequiredService<IMongoClient>();

        new InventoryDbSeed()
            .SeedDataAsync(mongoClient, settings)
            .Wait();

        return host;
    }
}
