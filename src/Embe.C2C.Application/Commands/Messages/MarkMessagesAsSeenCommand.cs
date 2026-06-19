namespace Embe.C2C.Application.Commands.Messages;

public record MarkMessagesAsSeenCommand(Guid[] MessageIds);