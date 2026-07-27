namespace Embe.C2C.Application.Events.Candidates;

public record PositivelyJudgedIntegrationEvent(Guid CandidateId, Guid RecipientUserId) : IntegrationEvent(IntegrationEventType.PositivelyJudged);