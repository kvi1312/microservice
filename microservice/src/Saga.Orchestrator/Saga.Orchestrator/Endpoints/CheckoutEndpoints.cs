using System.ComponentModel.DataAnnotations;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOS.Basket;

namespace Saga.Orchestrator.Endpoints;

public class CheckoutEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/checkout/{userName}", Checkout);
    }

    private async Task<IResult> Checkout(ICheckoutSagaService service, [Required] string userName, [FromBody] BasketCheckoutDto dto)
    {
        var result = await service.CheckoutOrder(userName, dto);
        return Results.Accepted();
    }
}