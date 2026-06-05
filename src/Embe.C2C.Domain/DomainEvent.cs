namespace Embe.C2C.Domain;

public abstract record DomainEvent
{
    public Guid Id = Guid.CreateVersion7();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}