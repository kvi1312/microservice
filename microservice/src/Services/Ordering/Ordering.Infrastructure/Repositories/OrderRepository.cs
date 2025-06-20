﻿using Contracts.Common.Interfaces;
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

    public void CreateOrder(Order order) => Create(order);

    public void DeleteOrder(Order order)
    {
        Delete(order);
    }

    public Task<Order> GetOrderByDocumentNo(string documentNo) => FindByCondition(x => x.DocumentNo.ToString() == documentNo).FirstOrDefaultAsync();

    public async Task<IEnumerable<Order>> GetOrderByUserName(string userName)
        => await FindByCondition(x => x.UserName == userName).ToListAsync();

    public async Task<Order> UpdateOrderAsync(Order order){
        await UpdateAsync(order);
        return order;
    }
}