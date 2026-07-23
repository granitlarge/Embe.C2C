using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetMeHandler : TransactionalQueryHandler<GetMeQuery, Result<ReadDto<UserDto, UserPermission>>>
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

    protected override async Task<Result<ReadDto<UserDto, UserPermission>>> ExecuteAsync(GetMeQuery query, ISparseRepository repository, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<ReadDto<UserDto, UserPermission>>.Failure(FailureReason.NotFound, "user does not exist");
        }

        var enrichedUser = user.Enrich(user);
        var readDto = await enrichedUser.ToDtoAsync(_userAuthorizationService, _userDtoMapper, cancellationToken: cancellationToken);
        if (readDto is null)
            return Result<ReadDto<UserDto, UserPermission>>.Failure(FailureReason.Forbidden, "User does not have permission to view their own profile.");
        return Result<ReadDto<UserDto, UserPermission>>.Success(readDto);
    }
}