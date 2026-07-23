using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class CandidateExtensions
{
    public static async Task<ReadDto<CandidateDto, CandidatePermission>?> ToDtoAsync
    (
        this Candidate candidate,
        User? queryingUser,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CandidateAuthorizationService candidateAuthorizationService,
        CandidateDtoMapper candidateDtoMapper,
        CancellationToken cancellationToken
    )
    {
        var (permissions, variant) = candidateAuthorizationService.Get(candidate);
        var candidateUserDto = await
        (
            candidate.User?.Enrich(queryingUser).ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken) ??
            Task.FromResult<ReadDto<UserDto, UserPermission>?>(null)
        );

        var candidateSearchProfileDto = await
        (
            candidate.CandidateSearchProfile?.ToDtoAsync(searchProfileAuthorizationService, searchProfileDtoMapper, cancellationToken) ??
            Task.FromResult<ReadDto<SearchProfileDto, SearchProfilePermission>?>(null)
        );

        var candidateDto = candidateDtoMapper.ToDto
        (
            candidate,
            variant,
            null,
            candidateUserDto,
            null,
            candidateSearchProfileDto
        );

        if (candidateDto is null)
            return null;

        return new ReadDto<CandidateDto, CandidatePermission>(candidateDto, permissions);
    }
}