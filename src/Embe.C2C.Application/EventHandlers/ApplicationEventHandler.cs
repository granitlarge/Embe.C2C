using Embe.C2C.Application.Abstractions.Events;
using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.EventHandlers;

public class ApplicationEventHandler
{
    private readonly INotificationService _notificationService;

    public ApplicationEventHandler
    (
        INotificationService notificationService
    )
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(ApplicationEvent applicationEvent, CancellationToken cancellationToken = default)
    {
        switch (applicationEvent)
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