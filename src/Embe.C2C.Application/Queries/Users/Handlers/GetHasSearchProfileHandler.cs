using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetHasSearchProfileHandler
{
    private readonly IRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetHasSearchProfileHandler
    (
        IRepository repository,
        IAuthenticatedUserService authenticatedUserService
    )
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
    }

    public async Task<Result<bool>> HandleAsync(GetHasSearchProfileQuery query, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var result = await _repository.DomainUsersQuery.AnyAsync(u => u.Id == userId && u.SearchProfiles!.Any(), cancellationToken);
        return Result<bool>.Success(result);
    }

}