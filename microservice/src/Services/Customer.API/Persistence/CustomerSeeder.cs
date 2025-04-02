using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Customer.API.Persistence;

public class CustomerSeeder : IHostedService
{
    private readonly IServiceProvider _provider;

    public CustomerSeeder(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CustomerContext>();

        try
        {
            Log.Information("Applying database migrations...");
            await context.Database.MigrateAsync(cancellationToken);
            await CreateCustomer(context, "Bruce", "Wayne", "Bruce", "batman@gmail.com");
            await CreateCustomer(context, "Clark", "Kent", "Clark", "superman@gmail.com");
            Log.Information("Seeding customers completed.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error during data seeding.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task CreateCustomer(CustomerContext context, string userName, string firstName, string lastName, string emailAddress)
    {
        var customer = await context.Customers.SingleOrDefaultAsync(x => x.UserName == userName || x.EmailAdress == emailAddress);
        if (customer is null)
        {
            var newCustomer = new Entities.Customer
            {
                UserName = userName,
                EmailAdress = emailAddress,
                FirstName = firstName,
                LastName = lastName
            };
            await context.Customers.AddAsync(newCustomer);
            await context.SaveChangesAsync();
        }
    }
}