    namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record JudgementWriteDto
(
    Guid Id,
    Guid JudgeUserId,
    Guid JudgeeUserId,
    bool IsPositive,
    DateTimeOffset EditedAt,
    DateTimeOffset CreatedAt
);