namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record SameUserFact(Guid UserId, bool Value) : AuthorizationFact(UserId);