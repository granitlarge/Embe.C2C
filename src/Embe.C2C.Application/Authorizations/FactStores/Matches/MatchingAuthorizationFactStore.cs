using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Authorizations.FactStores.Matches;

public class MatchingAuthorizationFactStore
(
    MatchingFactGenerator factGenerator,
    IAuthenticatedUserService currentUserService
) : AuthorizationFactStore(currentUserService)
{
    private readonly MatchingFactGenerator _factGenerator = factGenerator;

    internal IsParticipantInMatchingFact GetIsParticipantFact(Matching matching)
    {
        var fact = GetFact<IsParticipantInMatchingFact>(matching.Id) ?? SetFact(_factGenerator.GetIsParticipantFact(matching));
        return fact;
    }

    internal async ValueTask<IsParticipantInMatchingFact> GetIsParticipantFactAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsParticipantInMatchingFact>(matchingId) ?? SetFact(await _factGenerator.GetIsParticipantFactAsync(matchingId, cancellationToken));
        return fact;
    }
}