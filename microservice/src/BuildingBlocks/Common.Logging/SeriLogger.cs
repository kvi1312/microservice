using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Logging;

public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure => //sử dụng static để sử dụng thuộc tính có thể gọi qua tên lớp, và tính dùng chung cho mọi đối tượng của static
        (context, configuration) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");
            var envName = context.HostingEnvironment?.EnvironmentName ?? "Development";

            configuration
            .WriteTo.Debug()
            .WriteTo.Console(outputTemplate:
             "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} {NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", envName)
            .Enrich.WithProperty("Application", applicationName)
            .ReadFrom.Configuration(context.Configuration);
        };
}