using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;
namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private ILogger _logger;
    private const string MethodName = "CreateOrderCommandHandler";
    public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); ;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
        var orderEntity = _mapper.Map<Order>(request);
         _orderRepository.CreateOrder(orderEntity);
         
         // Publishing event sourcing
        orderEntity.AddedOrder();
        
        await _orderRepository.SaveChangesAsync();
        
        _logger.Information($"END: {MethodName} - Username: {request.UserName}");
        return new ApiSuccessResult<long>(orderEntity.Id);
    }
}
