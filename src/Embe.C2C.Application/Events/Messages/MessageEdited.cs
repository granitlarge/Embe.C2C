namespace Embe.C2C.Application.Events.Messages;

public record MessageEdited
(
    Guid ConversationId,
    Guid AuthorUserId,
    Guid RecipientUserId,
    Guid MessageId
) : IntegrationEvent(IntegrationEventType.MessageEdited);
