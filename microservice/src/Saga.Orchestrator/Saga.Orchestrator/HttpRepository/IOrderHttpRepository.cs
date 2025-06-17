using Shared.DTOS.Order;

namespace Saga.Orchestrator.HttpRepository;

public interface IOrderHttpRepository
{
    Task<long> CreateOrder(CreateOrderDto order);
    Task<OrderDto> GetOrder(long orderId);
    Task<bool> DeleteOrder(long orderId);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}
