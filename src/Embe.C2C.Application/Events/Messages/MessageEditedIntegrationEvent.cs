namespace Embe.C2C.Application.Events.Messages;

public record MessageEditedIntegrationEvent
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageEdited);
