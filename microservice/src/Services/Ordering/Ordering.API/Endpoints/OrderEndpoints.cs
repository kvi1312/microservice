

using System.ComponentModel.DataAnnotations;
using System.Net;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Shared.Services.Email;

namespace Ordering.API.Endpoints;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderEndpoints : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _smtpEmailService;
    public OrderEndpoints(IMediator mediator, ISmtpEmailService smtpEmailService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _smtpEmailService = smtpEmailService;
    }

    private static class RoutesName
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{userName}", Name = RoutesName.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([Required] string userName)
    {
        var orders = new GetOrdersQuery(userName);
        var result = await _mediator.Send(orders);
        return Ok(result);
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