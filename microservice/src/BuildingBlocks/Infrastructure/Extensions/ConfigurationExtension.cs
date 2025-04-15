using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ConfigurationExtension
{
    /// <summary>
    /// Options Pattern to get section from json file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static T GetOptions<T>(this IServiceCollection service, string sectionName) where T : new()
    {
        using var serviceProvider = service.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(sectionName);
        var option = new T();
        section.Bind(option);
        return option;
    }
}
