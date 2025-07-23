using IdentityServer.Common;
using IdentityServer.Persistence;
using Microservice.IDP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace IdentityServer.Extensions;

public static class ServiceExtension
{
    public static void AddConfiguration(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
    }

    public static void ConfigureSeriLog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");
            var envName = context.HostingEnvironment?.EnvironmentName ?? "Development";
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var elasticUserName = context.Configuration.GetValue<string>("ElasticConfiguration:UserName");
            var elasticPassword = context.Configuration.GetValue<string>("ElasticConfiguration:Password");

            configuration
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} {NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    IndexFormat = $"logs-{applicationName}-{envName}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    ModifyConnectionSettings = x => x.BasicAuthentication(elasticUserName, elasticPassword)
                })
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", envName)
                .Enrich.WithProperty("Application", applicationName)
                .ReadFrom.Configuration(context.Configuration);
        });
    }

    public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        services.AddIdentityServer((options) =>
            {
                options.EmitStaticAudienceClaim = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddDeveloperSigningCredential()
            .AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = c =>
                    c.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("IdentityServer"));
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = c =>
                    c.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("IdentityServer"));
            })
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>();
    }

    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString))
            .AddIdentity<User, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequiredLength = 6;
            opt.User.RequireUniqueEmail = true;
            opt.Lockout.AllowedForNewUsers = true;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            opt.Lockout.MaxFailedAccessAttempts = 3;
        })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddUserStore<UserStore>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
            });
        });
    }

    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSettings)).Get<SMTPEmailSettings>();
        services.AddSingleton(emailSettings);
        return services;
    }

    public static void ConfigureSwagger(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Identity Server API",
                Version = "v1",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Email = "lenguyenkhai2611@gmail.com"
                }
            });

            var identityServerBaseUrl = configuration.GetSection("IdentityServer:BaseUrl").Value;

            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{identityServerBaseUrl}/connect/authorize"), // Duende Identity server required this endpoint
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

    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddLocalApi("Bearer", (options) =>
            {
                options.ExpectedScope = "microservices_api.read";
            });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Bearer", policy =>
            {
                policy.AddAuthenticationSchemes("Bearer");
                policy.RequireAuthenticatedUser();
            });
        });
    }
}