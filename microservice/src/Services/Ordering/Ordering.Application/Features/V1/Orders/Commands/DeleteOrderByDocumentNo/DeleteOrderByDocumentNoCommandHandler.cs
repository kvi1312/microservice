using AutoMapper;
using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;
using System.Reflection;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocumentNo;

public class DeleteOrderByDocumentNoCommandHandler : IRequestHandler<DeleteOrderByDocumentNoCommand, ApiResult<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    public DeleteOrderByDocumentNoCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger;
    }


    public async Task<ApiResult<bool>> Handle(DeleteOrderByDocumentNoCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = await _orderRepository.GetOrderByDocumentNo(request.DocumentNo);
        if (orderEntity is null) return new ApiResult<bool>(true);
        _orderRepository.DeleteOrder(orderEntity);
        orderEntity.DeletedOrder();
        await _orderRepository.SaveChangesAsync();
        return new ApiResult<bool>(true);
    }
}
