namespace Embe.C2C.Application.Events.SearchProfiles;

public record SearchProfileUpdatedIntegrationEvent(Guid SearchProfileId, string Description) : IntegrationEvent(IntegrationEventType.SearchProfileUpdated);