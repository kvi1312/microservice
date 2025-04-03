using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Services.Interfaces
{
    public interface IBasketService
    {
        Task<IResult> GetBasketByUserName(string userName);
        Task<IResult> DeleteBasketByUserName(string userName);
        Task<IResult> UpdateBasket(Cart cart);
    }
}
