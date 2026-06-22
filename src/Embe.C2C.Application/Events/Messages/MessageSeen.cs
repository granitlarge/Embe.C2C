namespace Embe.C2C.Application.Events.Messages;

public record MessageSeen
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageSeen);