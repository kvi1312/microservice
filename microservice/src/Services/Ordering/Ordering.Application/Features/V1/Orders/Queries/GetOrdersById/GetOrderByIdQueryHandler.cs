using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery ,ApiResult<OrderDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrderByUserName(request.UserName);
        var result = _mapper.Map<List<OrderDto>>(orders);
        return new ApiSuccessResult<List<OrderDto>>(result);
    }

    public async Task<ApiResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByIdAsync(request.Id);
        var result = _mapper.Map<OrderDto>(orders);
        return new ApiSuccessResult<OrderDto>(result);
    }
}
