namespace Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;

public record IsOwner(Guid CandidateId, bool Value) : AuthorizationFact(CandidateId);