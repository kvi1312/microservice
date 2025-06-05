using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ConfigureHostExtensions
{
    // this function handle to load appsettings file
    public static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var enviroment = context.HostingEnvironment;
            config
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{enviroment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
    }

    internal static IApplicationBuilder UseCustomHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>();
        var dashboardSettings = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        app.UseHangfireDashboard(hangfireSettings.Route, new DashboardOptions
        {
            DashboardTitle = dashboardSettings.DashboardTitle,
            StatsPollingInterval = dashboardSettings.StatsPollingInterval,
            AppPath = dashboardSettings.AppPath,
            IgnoreAntiforgeryToken = true,
        });
        return app;
    }
}