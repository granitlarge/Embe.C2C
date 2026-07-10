using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record JudgementDto
(
    Guid Id,
    Guid CandidateId,
    bool? IsPositive,
    DateTimeOffset? EditedAt,
    DateTimeOffset? CreatedAt,
    ReadDto<UserDto, UserPermission>? Judge
);

public class JudgementDtoMapper
{
    public JudgementDtoMapper()
    {

    }

    public JudgementDto? ToDto
    (
        Judgement judgement,
        JudgementVariant variant,
        ReadDto<UserDto, UserPermission>? judge = null
    )
    {
        if (variant == JudgementVariant.Empty)
            return null;

        return new JudgementDto
        (
            judgement.Id,
            judgement.CandidateId,
            variant.IncludeIsPositive ? judgement.IsPositive : null,
            variant.IncludeEditedAt ? judgement.EditedAt : null,
            variant.IncludeCreatedAt ? judgement.CreatedAt : null,
            judge
        );

    }
}