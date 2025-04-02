using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories.Interface;
using Product.API.Repostiories;
using Product.API.Services;

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
        return services;
    }

    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        var builder = new MySqlConnectionStringBuilder(connectionString);

        services.AddDbContext<ProductContext>(
            m => m.UseMySql(builder.ConnectionString,
                ServerVersion.AutoDetect(builder.ConnectionString),
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
                typeof(RepositoryBaseAsync<,,>)) // must ensure correct total declaration of generic type quantity => 3 here
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductService, ProductService>();
    }
}