using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, ApiResult<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private const string MethodName = "DeleteOrderCommandHandler";
    public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }
    public async Task<ApiResult<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN [DeleteOrderCommandHandler] Removing Order...");
        var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
        if (orderEntity == null) throw new NotFoundException(nameof(Order), request.Id);
        await _orderRepository.DeleteOrder(orderEntity);
        await _orderRepository.SaveChangesAsync();

        _logger.Information($"END Order {orderEntity.Id} was successfully deleted.");

        return new ApiResult<bool>(true);
    }
}
