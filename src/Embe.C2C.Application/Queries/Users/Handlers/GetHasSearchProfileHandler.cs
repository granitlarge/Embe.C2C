using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetHasSearchProfileHandler
(
    IUserRepository userRepo,
    IAuthenticatedUserService authenticatedUserService
)
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    public async Task<ErrorOr<bool>> HandleAsync(GetHasSearchProfileQuery query, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var result = await _userRepo.HasSearchProfilesAsync(userId, cancellationToken);
        return result;
    }

}