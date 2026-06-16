using Embe.C2C.Application.Authorizations.FactStores;

namespace Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

public record CandidateUserFact(Guid UserId, bool Value) : AuthorizationFact(UserId);