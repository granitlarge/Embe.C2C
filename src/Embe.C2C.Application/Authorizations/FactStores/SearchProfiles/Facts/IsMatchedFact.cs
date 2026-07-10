namespace Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;

public record IsMatchedFact(Guid SearchProfileId, bool Value) : AuthorizationFact(SearchProfileId);