using Contracts.ScheduledJobs;
using Infrastructure.ScheduledJobs;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        services.AddSingleton(hangfireSettings);
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IScheduledServices, HangfireServices>();
        return services;
    }
}
