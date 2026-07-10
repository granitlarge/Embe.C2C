namespace Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;


// Whether the SearchProfile belongs to a candidate of the current user. This is used to determine whether the current user can view the SearchProfile.
public record IsCandidateForUserFact(Guid SearchProfileId, bool Value) : AuthorizationFact(SearchProfileId);