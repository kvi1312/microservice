using Infrastructure.Extensions;
using Shared.DTOS.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _client;

    public BasketHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> DeleteBasket(string userName)
    {
        var response = await _client.DeleteAsync($"basket/{userName}");

        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete basket for UserName: {userName} failed");

        var result = await response.ReadContentAs<bool>();
        return result;
    }

    public async Task<CartDto> GetBasket(string userName)
    {
        var cart = await _client.GetFromJsonAsync<CartDto>($"basket/{userName}");
        if (cart == null || !cart.Items.Any()) return new CartDto();
        return cart;
    }
}
