using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Candidates.Handlers;

public class GetCandidateByIdHandler
(
    IAuthenticatedUserService authenticatedUserService,
    IUserRepository userRepository,
    UserAuthorizationService userAuthorizationService,
    UserDtoMapper userDtoMapper,
    ICandidateRepository candidateRepository,
    CandidateAuthorizationService candidateAuthorizationService,
    CandidateDtoMapper candidateDtoMapper,
    IRepository repository,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper
) :
    TransactionalQueryHandler<GetCandidateByIdQuery, ErrorOr<ReadDto<CandidateDto, CandidatePermission>?>>(repository)
{

    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly ICandidateRepository _candidateRepository = candidateRepository;
    private readonly CandidateAuthorizationService _candidateAuthorizationService = candidateAuthorizationService;
    private readonly CandidateDtoMapper _candidateDtoMapper = candidateDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;

    protected override async Task<ErrorOr<ReadDto<CandidateDto, CandidatePermission>?>> ExecuteAsync
    (
        GetCandidateByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _candidateAuthorizationService.GetPermissionsAsync(query.CandidateId, cancellationToken);
        if (!permissions.Contains(CandidatePermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var candidate = await _candidateRepository.GetByIdAsync(query.CandidateId, cancellationToken);
        if (candidate is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var queryingUser = _authenticatedUserService.UserId != null ?
            await _userRepository.GetByIdAsync(_authenticatedUserService.UserId.Value, cancellationToken) : null;


        var dto = await candidate.ToDtoAsync
        (
            queryingUser,
            _userAuthorizationService,
            _userDtoMapper,
            _searchProfileAuthorizationService,
            _searchProfileDtoMapper,
            _candidateAuthorizationService,
            _candidateDtoMapper,
            cancellationToken
        );

        if (dto is null)
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        return dto;
    }

}