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

public class GetUserByIdHandler
(
    IUserRepository userRepo,
    UserAuthorizationService authorizationPolicy,
    IRepository context,
    UserDtoMapper userDtoMapper,
    IAuthenticatedUserService authenticatedUserService
) : TransactionalQueryHandler<GetUserByIdQuery, ErrorOr<ReadDto<UserDto, UserPermission>?>>(context)
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly UserAuthorizationService _authorizationService = authorizationPolicy;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    protected override async Task<ErrorOr<ReadDto<UserDto, UserPermission>?>> ExecuteAsync
    (
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var (permissions, variant) = await _authorizationService.GetAsync(request.Id, cancellationToken);
        if (!permissions.Contains(UserPermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var user = await _userRepo.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var readDto = await _userDtoMapper.ToDtoAsync(user, queryingUser, cancellationToken);
        return readDto;
    }
}