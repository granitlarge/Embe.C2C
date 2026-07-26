namespace Embe.C2C.Application.Events.Matchings;

public record MatchingCreatedIntegrationEvent
(
    Guid MatchingId,
    Guid MatcheeUserId
) : IntegrationEvent(IntegrationEventType.MatchingCreated);