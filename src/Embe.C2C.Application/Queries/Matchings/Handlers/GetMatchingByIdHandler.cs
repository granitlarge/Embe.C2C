using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingByIdHandler : TransactionalQueryHandler<GetMatchingByIdQuery, Result<ReadDto<MatchingDto, MatchingPermission>>>
{
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

    protected override async Task<Result<ReadDto<MatchingDto, MatchingPermission>>> ExecuteAsync(GetMatchingByIdQuery query, ISparseRepository repository, CancellationToken cancellationToken = default)
    {
        var permissions = await _matchingAuthorizationService.GetPermissionsAsync(query.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.Forbidden, "You do not have permission to view this matching.");
        }

        var matching = await repository
            .MatchingsQuery
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.User1SearchProfile)
            .Include(m => m.User2SearchProfile)
            .Include(m => m.Messages!.OrderByDescending(mes => mes.CreatedAt).Take(50))
                .ThenInclude(mes => mes.ReplyToMessage)
            .SingleOrDefaultAsync(m => m.Id == query.MatchingId, cancellationToken);

        if (matching == null)
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.NotFound, "Matching not found.");
        }
        var queryingUser = await _userRepo.GetByIdAsync(_authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
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

        if (readDto == null)
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.Forbidden, "You do not have permission to view this matching.");
        }

        return Result<ReadDto<MatchingDto, MatchingPermission>>.Success(readDto);
    }
}