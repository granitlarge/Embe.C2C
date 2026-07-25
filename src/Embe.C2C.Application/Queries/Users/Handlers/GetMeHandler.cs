using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetMeHandler : TransactionalQueryHandler<GetMeQuery, ErrorOr<ReadDto<UserDto, UserPermission>>>
{
    private readonly IUserRepository _userRepo;
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly UserDtoMapper _userDtoMapper;

    public GetMeHandler
    (
        IUserRepository userRepo,
        IRepository repository,
        UserAuthorizationService userAuthorizationPolicy,
        IAuthenticatedUserService authenticatedUserService,
        UserDtoMapper userDtoMapper
    ) : base(repository)
    {
        _userAuthorizationService = userAuthorizationPolicy;
        _authenticatedUserService = authenticatedUserService;
        _userDtoMapper = userDtoMapper;
        _userRepo = userRepo;
    }

    protected override async Task<ErrorOr<ReadDto<UserDto, UserPermission>>> ExecuteAsync(GetMeQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var enrichedUser = user.Enrich(user);
        var readDto = await enrichedUser.ToDtoAsync(_userAuthorizationService, _userDtoMapper, cancellationToken: cancellationToken);
        if (readDto is null)
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }
        return readDto;
    }
}