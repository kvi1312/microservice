namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services;
    }
}
