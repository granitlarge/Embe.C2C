using Embe.C2C.Application.Events;

namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;

public record ExecuteIntegrationEventHandler<T>(T IntegrationEvent, WorkItemType Type) : IWorkItem
where T : IntegrationEvent;