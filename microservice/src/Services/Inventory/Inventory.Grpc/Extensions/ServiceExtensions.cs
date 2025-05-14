using Infrastructure.Extensions;
using Inventory.GRPC.Repositories;
using Inventory.GRPC.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.GRPC.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }

    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        services.AddSingleton(databaseSettings);
        return services;
    }

    public static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(GetMongoConnectionString(services)))
            .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
    }

    private static string GetMongoConnectionString(this IServiceCollection services)
    {
        var settings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));

        if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("MongoDB connection string is not configured.");

        var databaseName = settings.DatabaseName;
        var mongoConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";
        return mongoConnectionString;
    }
}
