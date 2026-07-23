using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores.Matches;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Authorizations;

public class MatchingAuthorizationService
{
    private readonly IMatchingRepository _matchingRepo;
    private readonly IRepository _repo;
    private readonly MatchingAuthorizationFactStore _facts;

    public MatchingAuthorizationService
    (
        IMatchingRepository matchingRepo,
        IRepository repo,
        MatchingAuthorizationFactStore facts
    )
    {
        _matchingRepo = matchingRepo;
        _repo = repo;
        _facts = facts;
    }

    public async Task<ImmutableHashSet<MatchingPermission>> GetPermissionsAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        return GetPermissions(await _facts.GetIsParticipantFactAsync(matchingId, cancellationToken));
    }

    public (ImmutableHashSet<MatchingPermission> Permissions, MatchingVariant Variant) Get
    (
        Matching matching
    )
    {
        var isParticipantFact = _facts.GetIsParticipantFact(matching);
        var permissions = GetPermissions(isParticipantFact);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return (permissions, MatchingVariant.Empty);
        }

        return (permissions, isParticipantFact.Value ? MatchingVariant.Full : MatchingVariant.Empty);
    }

    private static ImmutableHashSet<MatchingPermission> GetPermissions
    (
        IsParticipantInMatchingFact fact
    )
    {
        var permissions = new HashSet<MatchingPermission>();
        if (fact.Value)
        {
            permissions.Add(MatchingPermission.View);
            permissions.Add(MatchingPermission.Unmatch);
            permissions.Add(MatchingPermission.Chat);
        }

        return [.. permissions];
    }

}

public enum MatchingPermission
{
    View = 0,
    Unmatch = 1,
    Chat = 2
}