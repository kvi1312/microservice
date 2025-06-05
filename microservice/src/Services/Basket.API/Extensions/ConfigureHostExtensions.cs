using Hangfire;
using Shared.Configurations;

namespace Basket.API.Extensions
{
    public static class ConfigureHostExtensions
    {
        public static void AddAppConfigurations(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var enviroment = context.HostingEnvironment;
                config
                    .AddJsonFile("appsettings.json", optional:false)
                    .AddJsonFile($"appsettings.{enviroment.EnvironmentName}.json", optional:true ,reloadOnChange: true)
                    .AddEnvironmentVariables();
            });
        }
    }
}
