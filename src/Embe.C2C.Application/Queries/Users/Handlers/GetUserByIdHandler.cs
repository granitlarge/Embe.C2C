using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetUserByIdHandler
(
    UserAuthorizationService authorizationPolicy,
    IRepository context,
    UserDtoMapper userDtoMapper
) : TransactionalQueryHandler<GetUserByIdQuery, Result<ReadDto<UserDto, UserPermission>?>>(context)
{
    private readonly UserAuthorizationService _authorizationService = authorizationPolicy;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

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

        var user = await repo.DomainUsersQuery.AsNoTracking().SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (user is null)
            return Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.NotFound, "User not found.");

        var dto = await _userDtoMapper.ToDtoAsync(user, variant, cancellationToken);
        var readDto = new ReadDto<UserDto, UserPermission>(dto!, permissions);
        return Result<ReadDto<UserDto, UserPermission>?>.Success(readDto);
    }
}