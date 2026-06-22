namespace Embe.C2C.Domain.Aggregates.Messages.Events
{
    public record MessageUnseenEvent(Message Message) : DomainEvent;
}