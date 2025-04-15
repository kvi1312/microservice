using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using EventBus.Messages.IntegrationEvent.Events;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        public BasketService(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        public async Task<IResult> Checkout(BasketCheckout basketCheckout)
        {
            var basket = await _basketRepository.GetBasketByUserName(basketCheckout.UserName);

            if (basket == null) return Results.NotFound();

            // publish checkout event to EventBus message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);
            await _basketRepository.DeletedBasketFromUserName(basketCheckout.UserName);
            return Results.Accepted();
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
