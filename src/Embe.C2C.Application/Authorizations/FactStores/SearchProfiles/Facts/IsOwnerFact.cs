namespace Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;

public record IsOwnerFact(Guid SearchProfileId, bool Value) : AuthorizationFact(SearchProfileId);