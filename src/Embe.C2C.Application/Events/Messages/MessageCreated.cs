namespace Embe.C2C.Application.Events.Messages;

public record MessageCreated
(
    Guid MatchingId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageCreated);
