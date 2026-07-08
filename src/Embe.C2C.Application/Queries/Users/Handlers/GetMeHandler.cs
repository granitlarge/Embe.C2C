using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetMeHandler : TransactionalQueryHandler<GetMeQuery, Result<ReadDto<UserDto, UserPermission>>>
{
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly UserDtoMapper _userDtoMapper;

    public GetMeHandler
    (
        IRepository repository,
        UserAuthorizationService userAuthorizationPolicy,
        IAuthenticatedUserService authenticatedUserService,
        UserDtoMapper userDtoMapper
    ) : base(repository)
    {
        _userAuthorizationService = userAuthorizationPolicy;
        _authenticatedUserService = authenticatedUserService;
        _userDtoMapper = userDtoMapper;
    }

    protected override async Task<Result<ReadDto<UserDto, UserPermission>>> ExecuteAsync(GetMeQuery query, ISparseRepository repository, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var user = await repository.DomainUsersQuery.SingleAsync(u => u.Id == userId, cancellationToken);
        var readDto = await user.ToDtoAsync(_userAuthorizationService, _userDtoMapper);
        if (readDto is null)
            return Result<ReadDto<UserDto, UserPermission>>.Failure(FailureReason.Forbidden, "User does not have permission to view their own profile.");
        return Result<ReadDto<UserDto, UserPermission>>.Success(readDto);
    }
}