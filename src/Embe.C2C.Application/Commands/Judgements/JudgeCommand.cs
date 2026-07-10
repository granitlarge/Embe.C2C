namespace Embe.C2C.Application.Commands.Judgements;

public record JudgeCommand(Guid CandidateId, bool IsPositive);