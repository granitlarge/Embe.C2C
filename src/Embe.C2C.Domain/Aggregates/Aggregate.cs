using System.ComponentModel.DataAnnotations;

namespace Embe.C2C.Domain.Aggregates;

public abstract class Aggregate : DomainEventCollector
{
    [Timestamp]
    public uint RowVersion { get; private set; }
}