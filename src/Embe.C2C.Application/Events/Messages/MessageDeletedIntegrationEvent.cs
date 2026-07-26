namespace Embe.C2C.Application.Events.Messages;

public record MessageDeletedIntegrationEvent
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageDeleted);
