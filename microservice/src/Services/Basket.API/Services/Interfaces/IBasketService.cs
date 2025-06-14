using Basket.API.Entities;
using Shared.DTOS.Basket;

namespace Basket.API.Services.Interfaces
{
    public interface IBasketService
    {
        Task<IResult> GetBasketByUserName(string userName);
        Task<IResult> DeleteBasketByUserName(string userName);
        Task<IResult> UpdateBasket(CartDto cart);
        Task<IResult> Checkout(BasketCheckout basketCheckout, string userName);
    }
}
