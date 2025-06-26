using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using EventBus.Messages.IntegrationEvent.Events;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOS.Basket;

namespace Basket.API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        private readonly StockItemGrpcService _stockItemGrpcService;
        private readonly IEmailTemplateServices _emailTemplateServices;
        public BasketService(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper, StockItemGrpcService stockItemGrpcService, IEmailTemplateServices emailTemplateServices)
        {
            _basketRepository = basketRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _stockItemGrpcService = stockItemGrpcService;
            _emailTemplateServices = emailTemplateServices;
        }

        public async Task<IResult> Checkout(BasketCheckout basketCheckout, string userName)
        {
            var basket = await _basketRepository.GetBasketByUserName(userName);

            if (basket == null) return Results.NotFound();

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            // publish checkout event to EventBus message
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
            var cart = await _basketRepository.GetBasketByUserName(userName);
            var result = _mapper.Map<CartDto>(cart) ?? new CartDto(); 
            return Results.Ok(result);
        }

        public async Task<IResult> UpdateBasket(CartDto model)
        {
            // communicate with grpc service
            foreach (var item in model.Items)
            {
                var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
                item.SetAvailabelQuantity(stock.Quantity);
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            var cart = _mapper.Map<Cart>(model);
            var updatedCart = await _basketRepository.UpdateBasket(cart, options);
            var result = _mapper.Map<CartDto>(updatedCart);
            return Results.Ok(result);
        }
    }
}
