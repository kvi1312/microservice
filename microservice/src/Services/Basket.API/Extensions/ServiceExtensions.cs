using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

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

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;

            if(string.IsNullOrEmpty(redisConnectionString)) throw new ArgumentException("Redis connection string is not configured.");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
    }
}
