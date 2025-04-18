using EventBus.Messages.IntegrationEvent.Events;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.API.Applications.IntegrationEvents;
using Shared.Configurations;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSettings))
                                            .Get<SMTPEmailSettings>();
        services.AddSingleton(emailSettings);

        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>();
        services.AddSingleton(eventBusSettings);
        return services;
    }

    public static void ConfigureMasstransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));

        if (settings is null || string.IsNullOrEmpty(settings.HostAddress))
            throw new ArgumentNullException("EventBusSettings is not configured");

        var mqConnection = new Uri(settings.HostAddress);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.AddConsumersFromNamespaceContaining<BasketCheckoutConsumer>();
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(mqConnection);
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}