namespace Embe.C2C.Application.Events.Messages;

public record MessageUnseen
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageUnseen);
