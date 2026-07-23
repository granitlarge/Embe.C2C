namespace Embe.C2C.Application.Commands.Candidates;

public record JudgeCandidateCommand(Guid CandidateId, bool IsPositive);