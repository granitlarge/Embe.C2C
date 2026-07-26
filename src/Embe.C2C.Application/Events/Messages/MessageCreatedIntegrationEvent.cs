namespace Embe.C2C.Application.Events.Messages;

public record MessageCreatedIntegrationEvent
(
    Guid MatchingId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageCreated);
