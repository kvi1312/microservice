using Basket.API.Entities;

namespace Basket.API.Services.Interfaces
{
    public interface IBasketService
    {
        Task<IResult> GetBasketByUserName(string userName);
        Task<IResult> DeleteBasketByUserName(string userName);
        Task<IResult> UpdateBasket(Cart cart);
        Task<IResult> Checkout(BasketCheckout basketCheckout);
    }
}
