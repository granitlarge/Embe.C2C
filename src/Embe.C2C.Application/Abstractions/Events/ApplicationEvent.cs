namespace Embe.C2C.Application.Abstractions.Events;

public record ApplicationEvent()
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}