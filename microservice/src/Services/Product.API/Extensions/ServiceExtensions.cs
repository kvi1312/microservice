using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories.Interface;
using Product.API.Repostiories;
using Product.API.Services;
using Shared.Configurations;
using System.Reflection;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger(configuration);
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
        services.ConfigureAuthenticationHandler();
        services.ConfigureAuthorization();
        services.ConfigureHealthChecks();
        services.AddMediatR(Assembly.GetExecutingAssembly());
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

        var apiSettings = configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
        services.AddSingleton(apiSettings);
        return services;
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks().AddMySql(settings.ConnectionString, "MySql Health", HealthStatus.Degraded);
    }

    public static void ConfigureSwagger(this IServiceCollection service, IConfiguration configuration)
    {
        var setting = configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();

        if (setting == null || string.IsNullOrEmpty(setting.IssuerUri) || string.IsNullOrEmpty(setting.ApiName))
        {
            throw new ArgumentNullException($"Api configuration is not configured");
        }

        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Product API",
                Version = "v1",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Email = "lenguyenkhai2611@gmail.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{setting.IdentityServerBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>()
                        {
                            {"microservices_api.read", "Microservices API Read Scope" },
                            {"microservices_api.write", "Microservices API Write Scope" }
                        }
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id= "Bearer" },
                        Name = "Bearer"
                    },
                    new List<string>
                    {
                        "microservices_api.read",
                        "microservices_api.write"
                    }
                }
            });
        });
    }
}