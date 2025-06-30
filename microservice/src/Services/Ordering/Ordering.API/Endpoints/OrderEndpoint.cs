using AutoMapper;
using Carter;
using Contracts.Services;
using MediatR;
using Ordering.Application.Features.V1.Orders;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocumentNo;
using Shared.DTOS.Order;
using Shared.SeedWork;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API.Endpoints
{
    public class OrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/order/{userName}", GetOrder)
                .Produces((int)HttpStatusCode.OK, typeof(IEnumerable<Application.Common.Models.OrderDto>));

            app.MapGet("api/v1/order/{id:long}", GetOrderById)
                .Produces((int)HttpStatusCode.OK, typeof(Application.Common.Models.OrderDto));

            app.MapPost("api/v1/order", CreateOrder)
                .Produces((int)HttpStatusCode.OK, typeof(ApiResult<long>));

            app.MapPut("api/v1/order/{id:long}", UpdateOrder)
                .Produces((int)HttpStatusCode.OK, typeof(ApiResult<Application.Common.Models.OrderDto>));

            app.MapDelete("api/v1/order/document-no/{documentNo}", DeleteOrderByDocumentNo)
                .Produces((int)HttpStatusCode.OK, typeof(ApiResult<bool>));

            app.MapGet("api/v1/order/test-email", SendTestEmail);
        }

        private async Task<IResult> DeleteOrderByDocumentNo([Required] string documentNo, IMediator mediator)
        {
            var command = new DeleteOrderByDocumentNoCommand(documentNo);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }

        private async Task<IResult> UpdateOrder([Required] long id, UpdateOrderCommand command, IMediator mediator)
        {
            command.SetId(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }

        private async Task<IResult> CreateOrder(CreateOrderDto model, IMapper mapper, IMediator mediator)
        {
            var command = mapper.Map<CreateOrderCommand>(model);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }

        private async Task<IResult> GetOrderById([Required] long id, IMediator mediator)
        {
            var orders = new GetOrderByIdQuery(id);
            var result = await mediator.Send(orders);
            return Results.Ok(result);
        }

        private async Task<IResult> GetOrder([Required] string userName, IMediator mediator)
        {
            var orders = new GetOrdersQuery(userName);
            var result = await mediator.Send(orders);
            return Results.Ok(result);
        }

        private async Task<IResult> SendTestEmail(ISmtpEmailService smtpEmailService)
        {
            var message = new MailRequest
            {
                Body = "<h1>Hello from microservice</h1>",
                Subject = "Send Test Email ^^",
                ToAddress = "lenguyenkhai2611@gmail.com"
            };
            await smtpEmailService.SendEmailAsync(message);
            return Results.Ok();
        }
    }
}