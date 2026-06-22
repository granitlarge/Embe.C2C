using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;

namespace Embe.C2C.Application.EventHandlers;

public class IntegrationEventHandler
(
    INotificationService notificationService
)
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task HandleAsync(IntegrationEventCollector eventCollector, CancellationToken cancellationToken = default)
    {
        var events = eventCollector.CollectedEvents;
        await Task.WhenAll(events.Select(e => HandleAsync(e, cancellationToken)));
    }

    private async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        // Notify clients about the event using the notification service
        await _notificationService.SendNotificationAsync(integrationEvent, cancellationToken);

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

    }

    private async Task HandleMessageCreatedEventAsync
    (
        MessageCreated messageCreatedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageEditedEventAsync
    (
        MessageEdited messageEditedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageDeletedEventAsync
    (
        MessageDeleted messageDeletedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageSeenEventAsync
    (
        MessageSeen messageSeenEvent,
        CancellationToken cancellationToken = default
    )
    {

    }
}