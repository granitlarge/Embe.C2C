namespace Embe.C2C.Domain.Aggregates;

public abstract class Aggregate : DomainEventCollector
{
    public byte[] RowVersion { get; private set; } = null!;
}