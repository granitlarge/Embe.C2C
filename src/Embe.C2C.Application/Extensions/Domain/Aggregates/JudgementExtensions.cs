using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class JudgementExtensions
{
    public static async Task<ReadDto<JudgementDto, JudgementPermission>?> ToDtoAsync
    (
        this Judgement judgement,
        User? queryingUser,
        JudgementAuthorizationService authorizationService,
        JudgementDtoMapper judgementDtoMapper,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        CancellationToken cancellationToken
    )
    {
        ReadDto<UserDto, UserPermission>? judgeReadDto = null;
        if (judgement.Judge != null)
        {
            var enrichedUser = judgement.Judge.Enrich(queryingUser);
            judgeReadDto = await enrichedUser.ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken);
        }
        var (permissions, variant) = authorizationService.Get(judgement);
        var dto = judgementDtoMapper.ToDto(judgement, variant, judgeReadDto);
        if (dto != null)
            return new ReadDto<JudgementDto, JudgementPermission>(dto, permissions);
        return null;
    }
}