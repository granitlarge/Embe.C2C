using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class MatchingFactGenerator
(
    IMatchingRepository matchingRepo,
    IAuthenticatedUserService currentUserService
) : AuthorizationFactGenerator(currentUserService)
{
    private readonly IMatchingRepository _matchingRepo = matchingRepo;

    public IsParticipantInMatchingFact GetIsParticipantFact(Matching matching)
    {
        var fact = new IsParticipantInMatchingFact(matching.Id, matching.UserId1 == CurrentUserId || matching.UserId2 == CurrentUserId);
        return fact;
    }

    public async ValueTask<IsParticipantInMatchingFact> GetIsParticipantFactAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        var fact = await _matchingRepo.GetIsParticipantInMatchingFactAsync(CurrentUserId ?? Guid.Empty, matchingId, cancellationToken);
        return fact;
    }
}