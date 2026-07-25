using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingsHandler
(
    IMatchingRepository matchingRepo,
    IUserRepository userRepo,
    IRepository repository,
    IImageService fileService,
    MatchingAuthorizationService matchingAuthorizationService,
    MatchingDtoMapper matchingDtoMapper,
    UserAuthorizationService userAuthorizationService,
    UserDtoMapper userDtoMapper,
    MessageAuthorizationService messageAuthorizationService,
    MessageDtoMapper messageDtoMapper,
    IAuthenticatedUserService authenticatedUserService,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper
) : TransactionalQueryHandler<GetMatchingsQuery, ErrorOr<List<ReadDto<MatchingDto, MatchingPermission>>>>(repository)
{
    private readonly IMatchingRepository _matchingRepo = matchingRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IImageService _fileService = fileService;
    private readonly MatchingAuthorizationService _matchingAuthorizationService = matchingAuthorizationService;
    private readonly MatchingDtoMapper _matchingDtoMapper = matchingDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly MessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper = messageDtoMapper;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;

    protected override async Task<ErrorOr<List<ReadDto<MatchingDto, MatchingPermission>>>> ExecuteAsync
    (
        GetMatchingsQuery query,
        CancellationToken cancellationToken
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var matchings = await _matchingRepo.GetMatchingsAsync
        (
            userId,
            query.Page,
            query.Size,
            cancellationToken
        );

        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var dtos = new List<ReadDto<MatchingDto, MatchingPermission>>();
        foreach (var matching in matchings)
        {
            var readDto = await matching.ToDtoAsync
            (
                queryingUser,
                matching.User1,
                matching.User2,
                _matchingAuthorizationService,
                _matchingDtoMapper,
                _userAuthorizationService,
                _userDtoMapper,
                _messageAuthorizationService,
                _messageDtoMapper,
                _searchProfileAuthorizationService,
                _searchProfileDtoMapper,
                cancellationToken
            );

            if (readDto is not null)
            {
                dtos.Add(readDto);
            }
        }

        return dtos;
    }
}