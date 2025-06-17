using Shared.DTOS.Basket;

namespace Saga.Orchestrator.HttpRepository;

public interface IBasketHttpRepository
{
    Task<CartDto> GetBasket(string userName);
    Task<bool> DeleteBasket(string userName);
}
