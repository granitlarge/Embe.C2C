using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingByIdHandler : TransactionalQueryHandler<GetMatchingByIdQuery, ErrorOr<ReadDto<MatchingDto, MatchingPermission>>>
{
    private readonly IMatchingRepository _matchingRepo;
    private readonly IUserRepository _userRepo;
    private readonly MatchingAuthorizationService _matchingAuthorizationService;
    private readonly MatchingDtoMapper _matchingDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;

    public GetMatchingByIdHandler
    (
        IMatchingRepository matchingRepo,
        IUserRepository userRepo,
        IRepository repository,
        MatchingAuthorizationService matchingAuthorizationService,
        MatchingDtoMapper matchingDtoMapper,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        IAuthenticatedUserService authenticatedUserService,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper
    ) : base(repository)
    {
        _matchingRepo = matchingRepo;
        _matchingAuthorizationService = matchingAuthorizationService;
        _matchingDtoMapper = matchingDtoMapper;
        _userAuthorizationService = userAuthorizationService;
        _userDtoMapper = userDtoMapper;
        _messageAuthorizationService = messageAuthorizationService;
        _messageDtoMapper = messageDtoMapper;
        _authenticatedUserService = authenticatedUserService;
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
        _searchProfileDtoMapper = searchProfileDtoMapper;
        _userRepo = userRepo;
    }

    protected override async Task<ErrorOr<ReadDto<MatchingDto, MatchingPermission>>> ExecuteAsync(GetMatchingByIdQuery query, CancellationToken cancellationToken = default)
    {
        var permissions = await _matchingAuthorizationService.GetPermissionsAsync(query.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var matching = await _matchingRepo.GetMatchingByIdAsync
        (
            query.MatchingId,
            cancellationToken
        );

        if (matching == null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var queryingUser = await _userRepo.GetByIdAsync(_authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
        var readDto = await _matchingDtoMapper.ToDtoAsync(matching, queryingUser, cancellationToken);
        if (readDto == null)
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        return readDto;
    }
}