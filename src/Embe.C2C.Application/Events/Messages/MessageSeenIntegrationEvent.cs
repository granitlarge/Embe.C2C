namespace Embe.C2C.Application.Events.Messages;

public record MessageSeenIntegrationEvent
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageSeen);