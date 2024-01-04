using Microsoft.EntityFrameworkCore;

namespace Product.API.Extensions;

public static class HostExtension
{
    public static IHost MigrateDataBase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext:DbContext // where : stand for check only context is extended from dbcontext that can using this service
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating mysql db...");
                ExcuteMigration(context);
                logger.LogInformation("Migrated mysql db...");
                InvokeSeeder(seeder, context, services);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating mysql db");
            }
        }
        return host;
    }

    private static void ExcuteMigration<TContext>(TContext context) where TContext: DbContext
    {
        context.Database.Migrate();
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services) where TContext:DbContext
    {
        seeder(context, services);
    }
}