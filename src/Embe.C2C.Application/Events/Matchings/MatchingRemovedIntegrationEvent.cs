namespace Embe.C2C.Application.Events.Matchings;

public record MatchingRemovedIntegrationEvent(Guid MatchingId, Guid RecipientUserId) : IntegrationEvent(IntegrationEventType.MatchingRemoved);