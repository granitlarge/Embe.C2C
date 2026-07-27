using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record CandidateDto
(
    Guid Id,
    Guid UserId,
    Guid CandidateUserId,
    Guid UserSearchProfileId,
    Guid CandidateSearchProfileId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    bool? Judgement,
    ReadDto<UserDto, UserPermission>? User,
    ReadDto<UserDto, UserPermission>? Candidate,
    ReadDto<SearchProfileDto, SearchProfilePermission>? UserSearchProfile,
    ReadDto<SearchProfileDto, SearchProfilePermission>? CandidateSearchProfile
);

public class CandidateDtoMapper
{
    private readonly UserDtoMapper _userDtoMapper;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;
    private readonly CandidateAuthorizationService _candidateAuthorizationService;
    private readonly ILogger<CandidateDtoMapper> _logger;

    public CandidateDtoMapper
    (
        UserDtoMapper userDtoMapper,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CandidateAuthorizationService candidateAuthorizationService,
        ILoggerFactory loggerFactory
    )
    {
        _userDtoMapper = userDtoMapper;
        _searchProfileDtoMapper = searchProfileDtoMapper;
        _candidateAuthorizationService = candidateAuthorizationService;
        _logger = loggerFactory.Create<CandidateDtoMapper>();
    }

    public async Task<ReadDto<CandidateDto, CandidatePermission>?> ToDtoAsync
    (
        Candidate candidate,
        User? queryingUser,
        CancellationToken cancellationToken
    )
    {
        var candidateCandidateDto = candidate.CandidateUser != null ? await _userDtoMapper.ToDtoAsync(candidate.CandidateUser, queryingUser, cancellationToken) : null;
        var candidateUserDto = candidate.User != null ? await _userDtoMapper.ToDtoAsync(candidate.User, queryingUser, cancellationToken) : null;
        var userSearchProfileDto = candidate.UserSearchProfile != null ? await _searchProfileDtoMapper.ToDtoAsync(candidate.UserSearchProfile, cancellationToken) : null;
        var candidateSearchProfileDto = candidate.CandidateSearchProfile != null ? await _searchProfileDtoMapper.ToDtoAsync(candidate.CandidateSearchProfile, cancellationToken) : null;

        var (permissions, variant) = _candidateAuthorizationService.Get(candidate);

        if (variant == CandidateVariant.Empty)
            return null;

        var dto = new CandidateDto
        (
            candidate.Id,
            candidate.UserId,
            candidate.CandidateUserId,
            candidate.UserSearchProfileId,
            candidate.CandidateSearchProfileId,
            variant.IncludeCreatedAt ? candidate.CreatedAt : null,
            variant.IncludeUpdatedAt ? candidate.UpdatedAt : null,
            variant.IncludeJudgement ? candidate.Judgement : null,
            candidateUserDto,
            candidateCandidateDto,
            userSearchProfileDto,
            candidateSearchProfileDto
        );

        var readDto = new ReadDto<CandidateDto, CandidatePermission>(dto, permissions);
        return readDto;

    }
}