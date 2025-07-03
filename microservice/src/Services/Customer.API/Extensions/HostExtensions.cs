using Hangfire;
using Shared.Configurations;

namespace Customer.API.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var enviroment = context.HostingEnvironment;
                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
}
