namespace Embe.C2C.Application.Commands.Messages;

public record EditMessageCommand(Guid MessageId, string NewContent);