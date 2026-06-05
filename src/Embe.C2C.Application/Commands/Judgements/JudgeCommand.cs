namespace Embe.C2C.Application.Commands.Judgements;

public record JudgeCommand(Guid JudgeeUserId, bool IsPositive);