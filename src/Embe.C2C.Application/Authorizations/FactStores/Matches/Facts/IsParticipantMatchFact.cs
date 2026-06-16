namespace Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;

public record IsParticipantMatchFact(Guid UserId, bool Value) : AuthorizationFact(UserId);