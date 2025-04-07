using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed : IHostedService
{
    private readonly ILogger<OrderContextSeed> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OrderContextSeed(ILogger<OrderContextSeed> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        try
        {
            if (context.Database.IsSqlServer())
            {
                _logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync(cancellationToken);
                await CreateOrder(context);
                _logger.LogInformation("Seeding Orders completed!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task CreateOrder(OrderContext context)
    {
        if (!context.Orders.Any())
        {
            await context.Orders.AddRangeAsync(new Order
            {
                UserName = "Bruce",
                FirstName = "Bruce",
                LastName = "Wayne",
                EmailAddress = "brucewayne@gmail.com",
                ShippingAddress = "18 Wall Street",
                InvoiceAddress = "America",
                TotalPrice = 250
            });
            await context.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}