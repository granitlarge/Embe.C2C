using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetHasSearchProfileHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetHasSearchProfileHandler
    (
        IUserRepository userRepo,
        IRepository repository,
        IAuthenticatedUserService authenticatedUserService
    )
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
        _userRepo = userRepo;
    }

    public async Task<bool> HandleAsync(GetHasSearchProfileQuery query, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var result = await _userRepo.HasSearchProfilesAsync(userId, cancellationToken);
        return result;
    }

}