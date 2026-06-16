
namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record BlockingUserFact(Guid UserId, bool Value) : AuthorizationFact(UserId);