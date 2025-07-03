using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories.Interface;
using Product.API.Repostiories;
using Product.API.Services;
using Shared.Configurations;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
        services.ConfigureHealthChecks();
        return services;
    }

    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        if (settings is null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("Product DatabaseSettings is not configured");

        var connectionString = settings.ConnectionString;
        services.AddDbContext<ProductContext>(m => m.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
            e =>
            {
                e.MigrationsAssembly("Product.API");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services
            .AddScoped(typeof(IRepositoryBaseAsync<,,>),
                typeof(RepositoryBase<,,>)) // must ensure correct total declaration of generic type quantity => 3 here
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductService, ProductService>();
    }

    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        services.AddSingleton(jwtSettings);

        var dbSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        services.AddSingleton(dbSettings);
        return services;
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks().AddMySql(settings.ConnectionString, "MySql Health", HealthStatus.Degraded);
    }
}