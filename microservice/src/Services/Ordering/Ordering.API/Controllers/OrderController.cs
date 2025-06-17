using AutoMapper;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.V1.Orders;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocumentNo;
using Shared.DTOS.Order;
using Shared.SeedWork;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API.Endpoints;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _smtpEmailService;
    private readonly IMapper _mapper;
    public OrderController(IMediator mediator, ISmtpEmailService smtpEmailService, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _smtpEmailService = smtpEmailService;
        _mapper = mapper;
    }

    private static class RoutesName
    {
        public const string GetOrders = nameof(GetOrders);
        public const string GetOrderById = nameof(GetOrderById);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
        public const string DeleteOrderByDocumentNo = nameof(DeleteOrderByDocumentNo);

    }

    [HttpGet("{userName}", Name = RoutesName.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<Application.Common.Models.OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Application.Common.Models.OrderDto>>> GetOrders([Required] string userName)
    {
        var orders = new GetOrdersQuery(userName);
        var result = await _mediator.Send(orders);
        return Ok(result);
    }

    [HttpGet("{id:long}", Name = RoutesName.GetOrderById)]
    [ProducesResponseType(typeof(IEnumerable<Application.Common.Models.OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Application.Common.Models.OrderDto>>> GetOrderById([Required] long orderId)
    {
        var orders = new GetOrderByIdQuery(orderId);
        var result = await _mediator.Send(orders);
        return Ok(result);
    }

    [HttpPost(Name = RoutesName.CreateOrder)]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderDto model)
    {
        var command = _mapper.Map<CreateOrderCommand>(model);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut(Name = RoutesName.UpdateOrder)]
    [ProducesResponseType(typeof(ApiResult<Application.Common.Models.OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> UpdateOrder([Required] long id,[FromBody]UpdateOrderCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("document-no/{documentNo}", Name = RoutesName.DeleteOrderByDocumentNo)]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<ApiResult<bool>> DeleteOrderByDocumentNo([Required] string documentNo)
    {
        var command = new DeleteOrderByDocumentNoCommand(documentNo);
        var result = await _mediator.Send(command);
        return result;
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> SendTestEmail()
    {
        var message = new MailRequest
        {
            Body = "<h1>Hello from microservice</h1>",
            Subject = "Send Test Email ^^",
            ToAddress = "lenguyenkhai2611@gmail.com"
        };
        await _smtpEmailService.SendEmailAsync(message);
        return Ok();
    }
}