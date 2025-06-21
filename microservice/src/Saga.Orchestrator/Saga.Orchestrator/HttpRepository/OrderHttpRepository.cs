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
        var response = await _client.PostAsJsonAsync("Order", order);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode) return -1;
        var orderId = await response.ReadContentAs<ApiSuccessResult<long>>();
        return orderId.Data;
    }

    public async Task<bool> DeleteOrder(long id)
    {
        var response = await _client.DeleteAsync($"Order/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _client.DeleteAsync($"Order/document-no/{documentNo}");
        return response.IsSuccessStatusCode;
    }

    public async Task<OrderDto> GetOrder(long id)
    {
        try
        {
            var response = await _client.GetAsync($"Order/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to get order {id}. Status: {response.StatusCode}, Error: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiSuccessResult<OrderDto>>();
            return result?.Data ?? throw new InvalidOperationException($"Order {id} returned null data");
        }
        catch (HttpRequestException)
        {
            throw; // Re-throw HTTP exceptions as-is
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting order {id}: {ex.Message}", ex);
        }
    }
}