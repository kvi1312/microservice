using Infrastructure.Extensions;
using Shared.DTOS.Order;
using Shared.SeedWork;

namespace Saga.Orchestrator.HttpRepository;

public class OrderHttpRepository : IOrderHttpRepository
{
    private readonly HttpClient _client;

    public OrderHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<long> CreateOrder(CreateOrderDto order)
    {
        var response = await _client.PostAsJsonAsync("order", order);
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) return -1;

        var orderId = await response.ReadContentAs<ApiSuccessResult<long>>();
        return orderId.Data;
    }

    public async Task<bool> DeleteOrder(long orderId)
    {
        var response = await _client.DeleteAsync($"order/{orderId.ToString()}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _client.DeleteAsync($"document-no/{documentNo}");
        return response.IsSuccessStatusCode;
    }

    public async Task<OrderDto> GetOrder(long orderId)
    {
        var order = await _client.GetFromJsonAsync<ApiSuccessResult<OrderDto>>($"order/{orderId.ToString()}");
        return order.Data;
    }
}
