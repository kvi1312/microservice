using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;
namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;
        public BasketRepository(IDistributedCache redisCacheService, ILogger logger, ISerializeService serializeService)
        {
            _redisCacheService = redisCacheService;
            _logger = logger;
            _serializeService = serializeService;
        }

        public async Task<bool> DeletedBasketFromUserName(string userName)
        {
            try
            {
                _logger.Information("[BEGIN] Deleting basket for user {UserName}", userName);
                await _redisCacheService.GetStringAsync(userName);
                _logger.Information("[END] Deleting basket for user {UserName}", userName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting basket for user {UserName}", userName);
                return false;
            }
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            _logger.Information("[BEGIN] Getting basket for user {UserName}", userName);
            var basket = await _redisCacheService.GetStringAsync(userName);
            _logger.Information("[END] Getting basket for user {UserName}", userName);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            if (options is not null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            }
            return await GetBasketByUserName(cart.UserName);
        }
    }
}
