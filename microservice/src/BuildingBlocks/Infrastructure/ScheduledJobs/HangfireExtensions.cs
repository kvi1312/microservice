using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.PostgreSql;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Shared.Configurations;

namespace Infrastructure.ScheduledJobs;

public static class HangfireExtensions
{
    public static IServiceCollection AddInfrastructureHangfireService(this IServiceCollection services)
    {
        var hangfireSettings = services.GetOptions<HangfireSettings>("HangfireSettings");
        if (hangfireSettings == null || hangfireSettings.Storage is null || string.IsNullOrEmpty(hangfireSettings.Storage.ConnectionString))
        {
            throw new ArgumentNullException(nameof(hangfireSettings), "Hangfire settings cannot be null.");
        }
        services.AddSingleton(hangfireSettings);

        services.ConfigureHangfireService(hangfireSettings);
        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = hangfireSettings.ServerName;
        });

        return services;
    }

    private static IServiceCollection ConfigureHangfireService(this IServiceCollection services, HangfireSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Storage.DbProvider))
            throw new ArgumentNullException(nameof(settings.Storage.DbProvider), "Database provider cannot be null or empty.");

        switch (settings.Storage.DbProvider.ToLower())
        {
            case "mssql":
                break;
            case "mongodb":
                var mongoUrlBuilder = new MongoUrlBuilder(settings.Storage.ConnectionString);
                var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(mongoUrlBuilder.ToString()));
                mongoClientSettings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                };
                var mongoClient = new MongoClient(mongoClientSettings);
                var mongoStorageOpts = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    CheckConnection = true,
                    Prefix = "SchedulerQueue",
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                };
                services.AddHangfireConsoleExtensions();
                services.AddHangfire((provider, config) =>
                {
                    config.UseSimpleAssemblyNameTypeSerializer()
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseRecommendedSerializerSettings()
                    .UseConsole()
                    .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOpts);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                    };
                    config.UseSerializerSettings(jsonSettings);
                });
                break;
            case "postgresql":
                services.AddHangfire(config =>
                {
                    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(settings.Storage.ConnectionString));
                });
                break;
            default:
                throw new NotSupportedException($"Database provider '{settings.Storage.DbProvider}' is not supported.");
        }
        return services;
    }
}
