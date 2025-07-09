using IdentityServer.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            .AddDeveloperSigningCredential() // not recommend for PROD
                                             // .AddInMemoryIdentityResources(Config.IdentityResources)
                                             // .AddInMemoryApiScopes(Config.ApiScopes)
                                             // .AddInMemoryClients(Config.Clients)
                                             // .AddInMemoryApiResources(Config.ApiResources)
                                             // .AddTestUsers(TestUsers.Users)
                                             // .AddLicenseSummary()
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
            .AddAspNetIdentity<User>();
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
}