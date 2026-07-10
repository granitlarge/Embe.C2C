using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;

namespace Embe.C2C.Application.Authorizations.FactStores.SearchProfiles;

public class SearchProfileAuthorizationFactStore
(
    SearchProfileFactGenerator searchProfileFactGenerator,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly SearchProfileFactGenerator _searchProfileFactGenerator = searchProfileFactGenerator;

    public void SetIsOwnedByUserFact(Guid searchProfileId, bool value)
    {
        var fact = new IsOwnerFact(searchProfileId, value);
        SetFact(fact);
    }

    public void SetIsCandidateForUserFact(Guid searchProfileId, bool value)
    {
        var fact = new IsCandidateForUserFact(searchProfileId, value);
        SetFact(fact);
    }

    public void SetIsMatchedWithUserFact(Guid searchProfileId, bool value)
    {
        var fact = new IsMatchedFact(searchProfileId, value);
        SetFact(fact);
    }

    public async ValueTask<IsOwnerFact> GetIsOwnedByUserFactAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<IsOwnerFact>(searchProfileId);
        if (fact is not null)
            return fact;

        await LoadSearchProfileFactsAsync(searchProfileId, cancellationToken);
        return GetFact<IsOwnerFact>(searchProfileId) ?? throw new InvalidOperationException("IsOwnedByUserFact should have been loaded.");
    }

    public async ValueTask<IsCandidateForUserFact> GetIsCandidateForUserFactAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<IsCandidateForUserFact>(searchProfileId);
        if (fact is not null)
            return fact;

        await LoadSearchProfileFactsAsync(searchProfileId, cancellationToken);
        return GetFact<IsCandidateForUserFact>(searchProfileId) ?? throw new InvalidOperationException("IsCandidateForUserFact should have been loaded.");
    }

    public async ValueTask<IsMatchedFact> GetIsMatchedWithUserFactAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<IsMatchedFact>(searchProfileId);
        if (fact is not null)
            return fact;

        await LoadSearchProfileFactsAsync(searchProfileId, cancellationToken);
        return GetFact<IsMatchedFact>(searchProfileId) ?? throw new InvalidOperationException("IsMatchedWithUserFact should have been loaded.");
    }

    private async ValueTask LoadSearchProfileFactsAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        var facts = await _searchProfileFactGenerator.GetAuthorizationFactsAsync(searchProfileId, cancellationToken);
        foreach (var fact in facts)
        {
            SetFact(fact);
        }
    }
}