using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores.Candidates;
using Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Candidates;

namespace Embe.C2C.Application.Authorizations;

public class CandidateAuthorizationService(CandidateAuthorizationFactStore candidateAuthorizationFactStore)
{
    private readonly CandidateAuthorizationFactStore _candidateAuthorizationFactStore = candidateAuthorizationFactStore;

    public async Task<ImmutableHashSet<CandidatePermission>> GetPermissionsAsync
    (
        Guid candidateId,
        CancellationToken cancellationToken
    )
    {
        var isOwner = await _candidateAuthorizationFactStore.GetIsOwnerAsync(candidateId, cancellationToken);
        var isCandidate = await _candidateAuthorizationFactStore.GetIsCandidateAsync(candidateId, cancellationToken);
        var permissions = GetPermissions(isOwner, isCandidate);
        return permissions;
    }

    public ImmutableHashSet<CandidatePermission> GetPermissions(Candidate candidate)
    {
        var isOwner = _candidateAuthorizationFactStore.GetIsOwner(candidate);
        var isCandidate = _candidateAuthorizationFactStore.GetIsCandidate(candidate);
        return GetPermissions(isOwner, isCandidate);
    }

    public (ImmutableHashSet<CandidatePermission> Permissions, CandidateVariant Variant) Get(Candidate candidate)
    {
        var isOwner = _candidateAuthorizationFactStore.GetIsOwner(candidate);
        var isCandidate = _candidateAuthorizationFactStore.GetIsCandidate(candidate);
        var permissions = GetPermissions(candidate);
        var variant = GetVariant(isOwner, isCandidate);

        return (permissions, variant);
    }

    private static CandidateVariant GetVariant
    (
        IsOwner isOwner,
        IsCandidate isCandidate
    )
    {
        if (isOwner.Value)
        {
            return CandidateVariant.Full;
        }

        return CandidateVariant.Empty;
    }

    private static ImmutableHashSet<CandidatePermission> GetPermissions
    (
        IsOwner isOwner,
        IsCandidate isCandidate
    )
    {
        var permissions = new HashSet<CandidatePermission>();
        if (isOwner.Value)
        {
            permissions.Add(CandidatePermission.Judge);
            permissions.Add(CandidatePermission.View);
        }

        return [.. permissions];
    }
}

public enum CandidatePermission
{
    View = 1,
    Judge = 2
}