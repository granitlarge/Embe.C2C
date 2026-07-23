using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class UserFactGenerator
(
    IUserRepository userRepo,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IUserRepository _userRepo = userRepo;
    public async Task<AuthorizationFact[]> GetAllFactsAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        if (CurrentUserId is null)
        {
            throw new InvalidOperationException("no authenticated user");
        }
        return await _userRepo.GetAuthorizationFactsAsync(CurrentUserId.Value, userId, cancellationToken);
    }
}