namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record BlockedByUserFact(Guid UserId, bool Value) : AuthorizationFact(UserId);