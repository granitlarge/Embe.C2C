using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactStores.Matches;

public class MatchingAuthorizationFactStore(IRepository repo, IAuthenticatedUserService currentUserService) : AuthorizationFactStore(currentUserService)
{
    private readonly IRepository _repo = repo;

    internal IsParticipantMatchFact GetIsParticipantFact(Matching matching)
    {
        var fact = GetFact<IsParticipantMatchFact>(matching.Id) ??
            SetFact(new IsParticipantMatchFact(matching.Id, matching.UserId1 == CurrentUserId || matching.UserId2 == CurrentUserId));
        return fact;
    }

    internal async ValueTask<IsParticipantMatchFact> GetIsParticipantFactAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsParticipantMatchFact>(matchingId);
        if (fact != null)
        {
            return fact;
        }

        fact = await _repo.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => new IsParticipantMatchFact(m.Id, m.UserId1 == CurrentUserId || m.UserId2 == CurrentUserId))
            .FirstOrDefaultAsync(cancellationToken) ?? new IsParticipantMatchFact(matchingId, false);

        return SetFact(fact);
    }
}