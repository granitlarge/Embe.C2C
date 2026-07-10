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
        var result = await _repository
            .SearchProfilesQuery
            .AsNoTracking()
            .Where(sp => sp.Id == searchProfileId)
            .Select(sp => new
            {
                sp.Id,
                IsOwnedByUser = sp.UserId == CurrentUserId,
                IsMatchedWithUser = sp.MatchingsUserId1!.Any(m => m.UserId2 == CurrentUserId) || sp.MatchingsUserId2!.Any(m => m.UserId1 == CurrentUserId),
                IsCandidateForUser = sp.User!.CandidateUsers!.Any(c => c.CandidateUserId == CurrentUserId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var facts = new List<AuthorizationFact>
        {
            new IsOwnerFact(searchProfileId, result?.IsOwnedByUser ?? false),
            new IsMatchedFact(searchProfileId, result?.IsMatchedWithUser ?? false),
            new IsCandidateForUserFact(searchProfileId, result?.IsCandidateForUser ?? false)
        };
        return facts;
    }

}