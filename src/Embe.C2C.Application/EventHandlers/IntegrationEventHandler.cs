using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Notifications;

namespace Embe.C2C.Application.EventHandlers;

public class IntegrationEventHandler
{
    private readonly INotificationService _notificationService;

    public IntegrationEventHandler
    (
        INotificationService notificationService
    )
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(IntegrationEventCollector eventCollector, CancellationToken cancellationToken = default)
    {
        var events = eventCollector.CollectedEvents;
        await Task.WhenAll(events.Select(e => HandleAsync(e, cancellationToken)));
    }

    private async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        switch (integrationEvent)
        {
            case NotificationCreatedEvent notificationCreatedEvent:
                await HandleNotificationCreatedEventAsync(notificationCreatedEvent, cancellationToken);
                break;
            default:
                break;
        }
    }

    private async Task HandleNotificationCreatedEventAsync
    (
        NotificationCreatedEvent notificationCreatedEvent,
        CancellationToken cancellationToken = default
    )
    {
        var notification = notificationCreatedEvent.Notification;
        await _notificationService.SendNotificationAsync(notification, cancellationToken);
    }
}