using Embe.C2C.Domain.Aggregates.Judgements;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record JudgementDto
(
    Guid Id,
    Guid JudgeUserId,
    Guid JudgeeUserId,
    bool IsPositive,
    DateTimeOffset EditedAt,
    DateTimeOffset CreatedAt
);

public static class JudgementDtoExtensions
{
    public static JudgementDto ToDto(this Judgement judgement)
    {
        return new JudgementDto
        (
            judgement.Id,
            judgement.JudgeUserId,
            judgement.JudgeeUserId,
            judgement.IsPositive,
            judgement.EditedAt,
            judgement.CreatedAt
        );
    }
}
