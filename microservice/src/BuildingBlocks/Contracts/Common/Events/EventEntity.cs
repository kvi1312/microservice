using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events;

public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _domainEvent = new();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvent.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvent.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvent.Clear();
    }

    public IReadOnlyCollection<BaseEvent> GetDomainEvents() => _domainEvent.AsReadOnly();
}