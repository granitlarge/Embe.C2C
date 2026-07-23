namespace Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;

public record IsPositivelyJudgedCandidate(Guid CandidateId, bool Value) : AuthorizationFact(CandidateId);