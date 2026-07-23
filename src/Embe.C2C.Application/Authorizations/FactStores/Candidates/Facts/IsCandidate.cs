namespace Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;

public record IsCandidate(Guid CandidateId, bool Value) : AuthorizationFact(CandidateId);