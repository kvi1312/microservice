using Shared.DTOS.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _client;

    public BasketHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public Task<bool> DeleteBasket(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<CartDto> GetBasket(string userName)
    {
        var cart = await _client.GetFromJsonAsync<CartDto>($"basket/{userName}");
        if (cart == null || !cart.Items.Any()) return new CartDto();
        return cart;
    }
}
