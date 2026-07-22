using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class SearchProfileFactGenerator
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repository = repository;

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        var searchProfile = await _repository.SearchProfilesQuery
            .Include(sp => sp.MatchingsUserId1!.Where(m => m.UserId2 == CurrentUserId))
            .Include(sp => sp.MatchingsUserId2!.Where(m => m.UserId1 == CurrentUserId))
            .Include(sp => sp.User)
                .ThenInclude(u => u!.CandidateUsers)
            .SingleOrDefaultAsync(sp => sp.Id == searchProfileId);

        var result = searchProfile != null ? new
        {
            searchProfile.Id,
            IsOwnedByUser = searchProfile.UserId == CurrentUserId,
            IsMatchedWithUser = searchProfile.MatchingsUserId1!.Any(m => m.UserId2 == CurrentUserId) || searchProfile.MatchingsUserId2!.Any(m => m.UserId1 == CurrentUserId),
            IsCandidateForUser = searchProfile.User!.CandidateUsers!.Any(c => c.CandidateUserId == CurrentUserId)
        } : null;

        var facts = new List<AuthorizationFact>
        {
            new IsOwnerFact(searchProfileId, result?.IsOwnedByUser ?? false),
            new IsMatchedFact(searchProfileId, result?.IsMatchedWithUser ?? false),
            new IsCandidateForUserFact(searchProfileId, result?.IsCandidateForUser ?? false)
        };

        return facts;
    }

}