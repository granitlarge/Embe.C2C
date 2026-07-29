
namespace Embe.C2C.Application.Events.SearchProfiles;

public record SearchProfileDescriptionChangedIntegrationEvent(Guid SearchProfileId, string NewDescription) : IntegrationEvent(IntegrationEventType.SearchProfileDescriptionChanged);