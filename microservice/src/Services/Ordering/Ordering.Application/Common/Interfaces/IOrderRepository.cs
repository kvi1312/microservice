using Contracts.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<IEnumerable<Order>> GetOrderByUserName(string userName);
    void CreateOrder(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task DeleteOrder(Order order);
}