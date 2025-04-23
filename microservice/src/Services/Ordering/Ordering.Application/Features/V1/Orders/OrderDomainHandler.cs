using Contracts.Services;
using MediatR;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;
using Shared.Services.Email;

namespace Ordering.Application.Features.V1.Orders;

public class OrderDomainHandler : INotificationHandler<OrderCreatedEvent>, INotificationHandler<OrderDeletedEvent>
{
    private readonly ILogger _logger;
    private readonly ISmtpEmailService _smtpEmailService;
    public OrderDomainHandler(ILogger logger, ISmtpEmailService smtpEmailService)
    {
        _logger = logger;
        _smtpEmailService = smtpEmailService;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Ordering Domain Event {DomainEvent}", notification.GetType().Name);
        var message = new MailRequest
        {
            Body = $"Your order detail <p> Order ID : {notification.DocumentNo}</p> <p> Total : {notification.TotalPrice}</p>",
            Subject = $"Hello {notification.FullName}, your order was created",
            ToAddress = notification.EmailAddress
        };
        try
        {
            await _smtpEmailService.SendEmailAsync(message);
            _logger.Information("Ordering Domain Event {DomainEvent}", notification.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to send test email with message with message {Message}", ex.Message);
        }
      
    }

    public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Ordering Domain Event {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
