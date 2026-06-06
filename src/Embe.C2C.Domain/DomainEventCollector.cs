using System.ComponentModel.DataAnnotations.Schema;

namespace Embe.C2C.Domain;

public abstract class DomainEventCollector
{
    [NotMapped]
    private readonly List<DomainEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}