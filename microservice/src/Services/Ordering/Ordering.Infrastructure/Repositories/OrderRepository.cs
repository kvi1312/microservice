using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<long> CreateOrderAsync(Order order) => await CreateAsync(order);

    public async Task DeleteOrder(Order order)
    {
        await DeleteAsync(order);
    }

    public async Task<IEnumerable<Order>> GetOrderByUserName(string userName)
        => await FindByCondition(x => x.UserName == userName).ToListAsync();

    public async Task<Order> UpdateOrderAsync(Order order){
        await UpdateAsync(order);
        return order;
    }


}