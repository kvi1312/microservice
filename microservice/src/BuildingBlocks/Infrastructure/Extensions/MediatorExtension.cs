using Contracts.Common.Events;
using MediatR;

namespace Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventAsync(this IMediator mediator, List<BaseEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents) { 
            await mediator.Publish(domainEvent);
        }
    }
}
