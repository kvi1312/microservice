using Common.Logging;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.SagaOrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOS.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICheckoutSagaService, CheckoutSagaService>();
        services.AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager.SagaOrderManager>();
        services.AddTransient<LoggingDelegatingHandler>();
        return services;
    }

    public static IServiceCollection ConfigureHttpRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();
        services.AddScoped<IBasketHttpRepository, BasketHttpRepository>();
        services.AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
        return services;
    }

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureOrderHttpClients(services);
        ConfigureBasketHttpClients(services);
        ConfigureInventoryHttpClients(services);
    }

    private static void ConfigureOrderHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrderAPI",
            (provider, config) => { config.BaseAddress = new Uri("http://localhost:5005/api/v1/"); }).AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddScoped(provider => provider.GetService<IHttpClientFactory>().CreateClient("OrderAPI"));
    }

    private static void ConfigureBasketHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketAPI",
            (provider, config) => { config.BaseAddress = new Uri("http://localhost:5004/api/"); }).AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddScoped(provider => provider.GetService<IHttpClientFactory>().CreateClient("BasketAPI"));
    }

    private static void ConfigureInventoryHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI",
            (provider, config) => { config.BaseAddress = new Uri("http://localhost:5006/api/"); }).AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddScoped(provider => provider.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }
}