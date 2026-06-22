namespace Embe.C2C.Application.Events.Messages;

public record MessageDeleted
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageDeleted);
