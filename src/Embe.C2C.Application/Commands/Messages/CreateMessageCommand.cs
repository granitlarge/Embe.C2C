namespace Embe.C2C.Application.Commands.Messages;

public record CreateMessageCommand(Guid MatchingId, string Content);