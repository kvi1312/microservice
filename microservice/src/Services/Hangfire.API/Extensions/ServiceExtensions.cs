using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        services.AddSingleton(hangfireSettings);

        var emailSettings = configuration.GetSection(nameof(SMTPEmailSettings)).Get<SMTPEmailSettings>();
        services.AddSingleton(emailSettings);
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IScheduledServices, HangfireServices>();
        services.AddScoped<ISmtpEmailService, SMTPEmailService>();
        services.AddTransient<IBackgroundJobServices, BackgroundJobServices>();
        return services;
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<HangfireSettings>(nameof(HangfireSettings));
        services.AddHealthChecks()
            .AddMongoDb(
                sp => new MongoClient(databaseSettings.Storage.ConnectionString),
                name: "MongoDb Health",
                failureStatus: HealthStatus.Degraded);
    }
}
