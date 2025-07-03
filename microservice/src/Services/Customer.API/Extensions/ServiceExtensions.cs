using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interface;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.ScheduledJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;

namespace Customer.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.ConfigureCustomerDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddInfrastructureHangfireService();
        return services;
    }

    private static IServiceCollection ConfigureCustomerDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        if (settings is null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("Customer DatabaseSettings is not configured");

        services.AddDbContext<CustomerContext>(options =>
        {
            options.UseNpgsql(settings.ConnectionString);
        });
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services
        .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>)) // must ensure correct total declaration of generic type quantity => 3 here
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
        .AddScoped<ICustomerService, CustomerService>()
        .AddScoped<ICustomerRepository, CustomerRepository>();
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        services.AddSingleton(hangfireSettings);

        var dbSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        services.AddSingleton(dbSettings);
        return services;
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgresQL Health",
                failureStatus: HealthStatus.Degraded);
    }
}
