namespace Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;

public record IsJudgee(Guid JudgementId, bool Value) : AuthorizationFact(JudgementId);
