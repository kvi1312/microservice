using System.ComponentModel.DataAnnotations;
using Carter;
using Contracts.Sagas.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.SagaOrderManager;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOS.Basket;

namespace Saga.Orchestrator.Endpoints;

public class CheckoutEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/checkout/{userName}", Checkout);
    }

    private IResult Checkout(ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager, [Required] string userName, [FromBody] BasketCheckoutDto model)
    {
        model.UserName = userName;
        var result =  sagaOrderManager.CreateOrder(model);
        return Results.Ok(result);
    }
}