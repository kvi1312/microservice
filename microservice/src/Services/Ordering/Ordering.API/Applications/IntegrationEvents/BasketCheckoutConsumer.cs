using AutoMapper;
using EventBus.Messages.IntegrationEvent.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.V1.Orders;
using ILogger = Serilog.ILogger;
namespace Ordering.API.Applications.IntegrationEvents;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public BasketCheckoutConsumer(IMediator mediator, ILogger logger, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var command = _mapper.Map<CreateOrderCommand>(context.Message);
        var result = await _mediator.Send(command);
        _logger.Information("BasketCheckoutEvent consumed successfully. Order is created with Id: {newOrderId}", result.Data);
    }
}
