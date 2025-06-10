using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvent.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory.GRPC.Client;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection service)
        {
            service.AddScoped<IBasketRepository, BasketRepository>();
            service.AddTransient<ISerializeService, SerializeService>();
            service.AddScoped<IBasketService, BasketService>();
            return service;
        }

        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSettings);

            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            services.AddSingleton(cacheSettings);

            var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
                .Get<GrpcSettings>();
            services.AddSingleton(grpcSettings);

            var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings)).Get<BackgroundJobSettings>();
            services.AddSingleton(backgroundJobSettings);

            return services;
        }

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));

            if(string.IsNullOrEmpty(settings.ConnectionString)) 
                throw new ArgumentException("Redis connection string is not configured.");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.ConnectionString;
            });
        }

        public static void ConfigureMasstransit(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));

            if(settings is null || string.IsNullOrEmpty(settings.HostAddress)) 
                throw new ArgumentNullException("EventBusSettings is not configured");

            var mqConnection = new Uri(settings.HostAddress);

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(mqConnection);
                });
                // Publish submit order message, instead of sending it to a specific queue directly.
                config.AddRequestClient<IBasketCheckoutEvent>();
            });
        }

        public static IServiceCollection ConfigureGrpcService(this IServiceCollection services)
        {
            var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
            services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x => x.Address = new Uri(settings.StockUrl));
            services.AddScoped<StockItemGrpcService>();
            services.AddTransient<IEmailTemplateServices, BasketEmailTemplateService>();
            return services;
        }

        public static void ConfigureHttpClientService(this IServiceCollection services)
        {
            services.AddHttpClient<BackgroundJobHttpServices>();
        }
    }
}
