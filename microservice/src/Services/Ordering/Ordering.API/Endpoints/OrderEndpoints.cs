

using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;

namespace Ordering.API.Endpoints;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderEndpoints : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrderEndpoints(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
}