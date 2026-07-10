using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.SearchProfiles;

namespace Embe.C2C.Application.Authorizations;

public class SearchProfileAuthorizationService
{

    public async ValueTask<(ImmutableHashSet<SearchProfilePermission> Permissions, SearchProfileVariant Variant)> GetAsync
    (
        Guid searchProfileId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async ValueTask<(ImmutableHashSet<SearchProfilePermission> Permissions, SearchProfileVariant Variant)> GetAsync
    (
        SearchProfile searchProfile,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

}

public enum SearchProfilePermission
{
    View,

}