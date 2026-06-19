namespace Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;

public record IsJudge(Guid JudgementId, bool Value) : AuthorizationFact(JudgementId);