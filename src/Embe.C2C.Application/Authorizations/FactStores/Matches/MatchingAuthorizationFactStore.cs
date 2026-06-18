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

    internal IsParticipantMatchFact GetIsParticipantFact(Matching matching)
    {
        var fact = GetFact<IsParticipantMatchFact>(matching.Id) ?? SetFact(_factGenerator.GetIsParticipantFact(matching));
        return fact;
    }

    internal async ValueTask<IsParticipantMatchFact> GetIsParticipantFactAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        var fact = GetFact<IsParticipantMatchFact>(matchingId) ?? SetFact(await _factGenerator.GetIsParticipantFactAsync(matchingId, cancellationToken));
        return fact;
    }
}