using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores.Judgements;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Judgements;

namespace Embe.C2C.Application.Authorizations;

public class JudgementAuthorizationPolicy
{

    private readonly UserAuthorizationPolicy _userAuthorizationPolicy;
    private readonly JudgementAuthorizationFactStore _factStore;

    public JudgementAuthorizationPolicy
    (
        UserAuthorizationPolicy userAuthorizationPolicy,
        JudgementAuthorizationFactStore factStore
    )
    {
        _userAuthorizationPolicy = userAuthorizationPolicy;
        _factStore = factStore;
    }

    public async Task<ReadDto<JudgementDto, JudgementPermission>?> ToDtoAsync
    (
        Judgement judgement,
        CancellationToken cancellationToken = default
    )
    {
        var isJudgeFact = _factStore.GetIsJudgeFact(judgement);
        var isJudgeeFact = _factStore.GetIsJudgeeFact(judgement);
        var isPositivelyJudgedFact = _factStore.GetIsPositivelyJudgedFact(judgement);
        var permissions = GetPermissions(isJudgeFact, isJudgeeFact, isPositivelyJudgedFact);
        var variant = GetVariant(isJudgeFact, isJudgeeFact, isPositivelyJudgedFact);
        var userDto = judgement.Judge != null ? await _userAuthorizationPolicy.ToDtoAsync(judgement.Judge, cancellationToken) : null;
        var dto = judgement.ToDto(variant, userDto);

        if (dto == null)
            return null;

        return new ReadDto<JudgementDto, JudgementPermission>(dto, permissions);
    }

    private JudgementVariant GetVariant
    (
        IsJudge? isJudgeFact,
        IsJudgee? isJudgeeFact,
        IsPositivelyJudged? isPositivelyJudgedFact = null
    )
    {
        if (isJudgeFact?.Value == true || isPositivelyJudgedFact?.Value == true)
        {
            return JudgementVariant.Full;
        }

        return JudgementVariant.Empty;
    }

    private ImmutableHashSet<JudgementPermission> GetPermissions
    (
        IsJudge? isJudgeFact,
        IsJudgee? isJudgeeFact,
        IsPositivelyJudged? isPositivelyJudgedFact = null
    )
    {
        var permissions = new HashSet<JudgementPermission>();
        if (isJudgeFact?.Value == true || isPositivelyJudgedFact?.Value == true)
        {
            permissions.Add(JudgementPermission.View);
        }

        return [.. permissions];
    }

}

public enum JudgementPermission
{
    View = 0
}