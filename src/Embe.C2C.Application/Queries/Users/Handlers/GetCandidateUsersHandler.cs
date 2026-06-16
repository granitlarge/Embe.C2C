using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetCandidateUsersHandler : TransactionalQueryHandler<GetCandidateUsersQuery, Result<List<ReadDto<UserDto, UserPermission>>>>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly UserAuthorizationPolicy _userAuthorizationPolicy;

    public GetCandidateUsersHandler
    (
        IRepository repo,
        IAuthenticatedUserService authenticatedUserService,
        UserAuthorizationPolicy userAuthorizationPolicy
    ) : base(repo)
    {
        _authenticatedUserService = authenticatedUserService;
        _userAuthorizationPolicy = userAuthorizationPolicy;
    }

    protected override async Task<Result<List<ReadDto<UserDto, UserPermission>>>> ExecuteAsync
    (
        GetCandidateUsersQuery request,
        ISparseRepository context,
        CancellationToken cancellationToken
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var users = await context.GetCandidatesForUserIdAsync(userId);
        var dtos = new List<ReadDto<UserDto, UserPermission>>();
        foreach (var user in users)
        {
            var dto = await _userAuthorizationPolicy.ToDtoAsync(user, cancellationToken);
            if (dto is not null)
            {
                dtos.Add(dto);
            }
        }
        return Result<List<ReadDto<UserDto, UserPermission>>>.Success(dtos);
    }
}