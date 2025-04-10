using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
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
    public OrderController(IMediator mediator, ISmtpEmailService smtpEmailService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _smtpEmailService = smtpEmailService;
    }

    private static class RoutesName
    {
        public const string GetOrders = nameof(GetOrders);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
        // create -> Id created, update -> orderdto, delete -> no content,
    }

    [HttpGet("{userName}", Name = RoutesName.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([Required] string userName)
    {
        var orders = new GetOrdersQuery(userName);
        var result = await _mediator.Send(orders);
        return Ok(result);
    }

    [HttpPost(Name = RoutesName.CreateOrder)]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut(Name = RoutesName.UpdateOrder)]
    public async Task<ActionResult> UpdateOrder()
    {
        return Ok();
    }

    [HttpDelete(Name = RoutesName.DeleteOrder)]
    public async Task<ActionResult> DeleteOrder()
    {
        return Ok();
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