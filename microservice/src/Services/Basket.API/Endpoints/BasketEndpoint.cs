using Basket.API.Entities;
using Basket.API.Services.Interfaces;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Endpoints
{
    public class BasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/basket/{userName}", GetBasketByUserName).Produces((int)HttpStatusCode.OK, typeof(Cart));
            app.MapDelete("api/basket/{userName}", DeleteBasketByUserName).Produces((int)HttpStatusCode.OK, typeof(bool));
            app.MapPost("api/basket/{userName}", UpdateBasketByUserName).Produces((int)HttpStatusCode.OK, typeof(Cart));
            app.MapPost("api/basket/checkout/{userName}", Checkout).Produces((int)HttpStatusCode.Accepted).Produces((int)HttpStatusCode.NotFound);
        }

        private async Task<IResult> Checkout(IBasketService basketService, [FromBody] BasketCheckout basketCheckout)
            => await basketService.Checkout(basketCheckout);

        private async Task<IResult> UpdateBasketByUserName(IBasketService basketService, [FromBody] Cart cart)
            => await basketService.UpdateBasket(cart);

        private async Task<IResult> DeleteBasketByUserName(IBasketService basketService, string userName)
            => await basketService.DeleteBasketByUserName(userName);

        private async Task<IResult> GetBasketByUserName(IBasketService basketService, string userName)
            => await basketService.GetBasketByUserName(userName);
    }
}
