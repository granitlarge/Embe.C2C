namespace Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;

public record IsParticipantInMatchingFact(Guid UserId, bool Value) : AuthorizationFact(UserId);