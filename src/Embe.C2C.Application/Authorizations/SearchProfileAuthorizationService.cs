using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Authorizations;

public class SearchProfileAuthorizationService
{
    private readonly SearchProfileAuthorizationFactStore _searchProfileAuthorizationFactStore;

    public SearchProfileAuthorizationService(SearchProfileAuthorizationFactStore searchProfileAuthorizationFactStore)
    {
        _searchProfileAuthorizationFactStore = searchProfileAuthorizationFactStore;
    }

    public async ValueTask<(ImmutableHashSet<SearchProfilePermission> Permissions, SearchProfileVariant Variant)> GetAsync
    (
        Guid searchProfileId,
        CancellationToken cancellationToken = default
    )
    {
        var isCandidateForUserFact = await _searchProfileAuthorizationFactStore.GetIsCandidateForUserFactAsync(searchProfileId, cancellationToken);
        var isMatchedFact = await _searchProfileAuthorizationFactStore.GetIsMatchedWithUserFactAsync(searchProfileId, cancellationToken);
        var isOwnerFact = await _searchProfileAuthorizationFactStore.GetIsOwnedByUserFactAsync(searchProfileId, cancellationToken);

        var permissions = GetPermissions(isCandidateForUserFact, isMatchedFact, isOwnerFact);
        var variant = GetVariant(isCandidateForUserFact, isMatchedFact, isOwnerFact);

        return (permissions, variant);
    }

    public ValueTask<(ImmutableHashSet<SearchProfilePermission> Permissions, SearchProfileVariant Variant)> GetAsync
    (
        SearchProfile searchProfile,
        CancellationToken cancellationToken = default
    )
    {
        #warning can we do better than this? there's plenty of information in the passed object
        return GetAsync(searchProfile.Id, cancellationToken);
    }

    private ImmutableHashSet<SearchProfilePermission> GetPermissions
    (
        IsCandidateForUserFact isCandidateForUserFact,
        IsMatchedFact isMatchedFact,
        IsOwnerFact isOwnerFact
    )
    {
        var permissions = ImmutableHashSet.CreateBuilder<SearchProfilePermission>();
        if (isOwnerFact.Value)
        {
            permissions.Add(SearchProfilePermission.View);
            permissions.Add(SearchProfilePermission.Modify);
            permissions.Add(SearchProfilePermission.Delete);
        }

        if (isMatchedFact.Value)
        {
            permissions.Add(SearchProfilePermission.View);
        }

        if (isCandidateForUserFact.Value)
        {
            permissions.Add(SearchProfilePermission.View);
        }

        return permissions.ToImmutable();
    }

    private SearchProfileVariant GetVariant
    (
        IsCandidateForUserFact isCandidateForUserFact,
        IsMatchedFact isMatchedFact,
        IsOwnerFact isOwnerFact
    )
    {
        if (isOwnerFact.Value)
            return SearchProfileVariant.Full;

        if (isMatchedFact.Value)
            return SearchProfileVariant.Matched;

        if (isCandidateForUserFact.Value)
            return SearchProfileVariant.Candidate;

        return SearchProfileVariant.Empty;
    }
}

public enum SearchProfilePermission
{
    View,
    Modify,
    Delete
}