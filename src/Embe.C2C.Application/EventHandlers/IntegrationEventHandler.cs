using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;

namespace Embe.C2C.Application.EventHandlers;

public class IntegrationEventHandler
(
    INotificationService notificationService,
    IImageService imageService,
    IWorkItemService workItemService
)
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IImageService _imageService = imageService;
    private readonly IWorkItemService _workItemService = workItemService;

    public async Task HandleAsync(IntegrationEventCollector eventCollector, CancellationToken cancellationToken = default)
    {
        var events = eventCollector.CollectedEvents;
        foreach (var @event in events.OrderBy(e => e.Timestamp))
        {
            await HandleAsync(@event, cancellationToken);
        }
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
            case ImageRemovedEvent imageRemovedEvent:
                await HandleImageRemovedEventAsync(imageRemovedEvent, cancellationToken);
                break;
            case ImageStatusChangedEvent imageMovedEvent:
                await HandleImageMovedEventAsync(imageMovedEvent, cancellationToken);
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

    private async Task HandleImageRemovedEventAsync
    (
        ImageRemovedEvent imageRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"Deleting image '{imageRemovedEvent.ImageId}'.");
        try
        {
            await _imageService.DeleteImageAsync(imageRemovedEvent.ImageName, imageRemovedEvent.ImageStatus, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete image, sending to work-item service {0}.", e);
            var url = await _imageService.GetImageUrlAsync(imageRemovedEvent.ImageName, imageRemovedEvent.ImageStatus, cancellationToken);
            await _workItemService.PerformAsync(new DeleteImage(url), cancellationToken);
        }
    }

    private async Task HandleImageMovedEventAsync
    (
        ImageStatusChangedEvent imageMovedEvent,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"Moving image '{imageMovedEvent.ImageId}'.");
        var fromUrl = await _imageService.GetImageUrlAsync(imageMovedEvent.ImageName, imageMovedEvent.OldStatus, cancellationToken);
        var toUrl = await _imageService.GetImageUrlAsync(imageMovedEvent.ImageName, imageMovedEvent.NewStatus, cancellationToken);
        try
        {

            await _imageService.MoveImageAsync(fromUrl, toUrl, cancellationToken);
            Console.WriteLine($"Moved image '{imageMovedEvent.ImageId}'");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to move image, sending to work-item service. {0}", e);
            await _workItemService.PerformAsync(new MoveFile(fromUrl, toUrl), cancellationToken);
        }
    }
}