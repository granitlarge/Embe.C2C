namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record MatchedUserFact(Guid Id, bool Value) : AuthorizationFact(Id);