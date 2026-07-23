namespace Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;

public record IsParticipantInMatchingFact(Guid MatchingId, bool Value) : AuthorizationFact(MatchingId);