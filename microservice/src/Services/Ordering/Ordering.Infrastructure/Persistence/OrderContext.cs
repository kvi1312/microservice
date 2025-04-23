using System.Reflection;
using Contracts.Common.Events;
using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using ILogger = Serilog.ILogger;
namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    public OrderContext(DbContextOptions<OrderContext> options, IMediator mediator, ILogger logger) : base(options)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public DbSet<Order> Orders { get; set; }
    private List<BaseEvent> _tempBaseEvents;

    private void SetBaseEventsBeforeSaveChanges()
    {
        var domainEntities = ChangeTracker.Entries<IEventEntity>()
            .Select(x => x.Entity)
            .Where(x => x.GetDomainEvents().Any())
            .ToList();

        _tempBaseEvents = domainEntities.SelectMany(x=> x.GetDomainEvents()).ToList();

        // Prevent looping published event
        domainEntities.ForEach(x => x.ClearDomainEvents());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetBaseEventsBeforeSaveChanges();
        var modified = ChangeTracker.Entries().Where(entity => entity.State == EntityState.Added || entity.State == EntityState.Modified || entity.State == EntityState.Deleted);

        foreach (var item in modified)
        {

            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedEntity)
                    {
                        addedEntity.CreatedDate = DateTime.UtcNow;
                        item.State = EntityState.Added;
                    }
                    break;
                case EntityState.Modified:
                    Entry(item.Entity).Property("Id").IsModified = false;
                    if (item.Entity is IDateTracking modifiedEntity)
                    {
                        modifiedEntity.LastModifiedDate = DateTime.UtcNow;
                        item.State = EntityState.Modified;
                    }
                    break;
            }
        }
        var result = base.SaveChangesAsync(cancellationToken);
        _mediator.DispatchDomainEventAsync(_tempBaseEvents);
        return result;
    }
}