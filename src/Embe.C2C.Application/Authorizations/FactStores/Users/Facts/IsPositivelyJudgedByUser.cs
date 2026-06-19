namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record IsPositivelyJudgedByUser(Guid UserId, bool Value) : AuthorizationFact(UserId);