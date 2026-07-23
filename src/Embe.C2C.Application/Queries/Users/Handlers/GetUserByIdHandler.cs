using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetUserByIdHandler
(
    IUserRepository userRepo,
    UserAuthorizationService authorizationPolicy,
    IRepository context,
    UserDtoMapper userDtoMapper,
    IAuthenticatedUserService authenticatedUserService
) : TransactionalQueryHandler<GetUserByIdQuery, Result<ReadDto<UserDto, UserPermission>?>>(context)
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly UserAuthorizationService _authorizationService = authorizationPolicy;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    protected override async Task<Result<ReadDto<UserDto, UserPermission>?>> ExecuteAsync
    (
        GetUserByIdQuery request,
        ISparseRepository repo,
        CancellationToken cancellationToken
    )
    {
        var (permissions, variant) = await _authorizationService.GetAsync(request.Id, cancellationToken);
        if (!permissions.Contains(UserPermission.View))
        {
            return Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.Forbidden, "You are not authorized to view this user.");
        }

        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var user = await _userRepo.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.NotFound, "User not found.");

        var enrichedUser = user.Enrich(queryingUser);
        var dto = await _userDtoMapper.ToDtoAsync(enrichedUser, variant, cancellationToken);
        var readDto = new ReadDto<UserDto, UserPermission>(dto!, permissions);
        return Result<ReadDto<UserDto, UserPermission>?>.Success(readDto);
    }
}