using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;

        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<IResult> DeleteBasketByUserName(string userName)
        {
            var result = await _basketRepository.DeletedBasketFromUserName(userName);
            return Results.Ok(result);  
        }

        public async Task<IResult> GetBasketByUserName(string userName)
        {
            var result = await _basketRepository.GetBasketByUserName(userName);
            return Results.Ok(result);
        }

        public async Task<IResult> UpdateBasket(Cart cart)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            var result = await _basketRepository.UpdateBasket(cart, options);
            return Results.Ok(result);
        }
    }
}
