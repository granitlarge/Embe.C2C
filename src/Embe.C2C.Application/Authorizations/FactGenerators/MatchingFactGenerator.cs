using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class MatchingFactGenerator
(
    IRepository repo,
    IAuthenticatedUserService currentUserService
) : AuthorizationFactGenerator(currentUserService)
{
    private readonly IRepository _repo = repo;

    public IsParticipantInMatchingFact GetIsParticipantFact(Matching matching)
    {
        var fact = new IsParticipantInMatchingFact(matching.Id, matching.UserId1 == CurrentUserId || matching.UserId2 == CurrentUserId);
        return fact;
    }

    public async ValueTask<IsParticipantInMatchingFact> GetIsParticipantFactAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        var fact = await _repo.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => new IsParticipantInMatchingFact(m.Id, m.UserId1 == CurrentUserId || m.UserId2 == CurrentUserId))
            .SingleOrDefaultAsync(cancellationToken) ?? new IsParticipantInMatchingFact(matchingId, false);

        return fact;
    }
}