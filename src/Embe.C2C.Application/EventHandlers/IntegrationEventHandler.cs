using Embe.C2C.Application.Abstractions.Repos;
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
            case NotificationCreatedIntegrationEvent notificationCreatedEvent:
                await HandleNotificationCreatedEventAsync(notificationCreatedEvent, cancellationToken);
                break;
            case ImageRemovedEvent imageRemovedEvent:
                await HandleImageRemovedEventAsync(imageRemovedEvent, cancellationToken);
                break;
            default:
                break;
        }
    }

    private async Task HandleNotificationCreatedEventAsync
    (
        NotificationCreatedIntegrationEvent notificationCreatedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageCreatedEventAsync
    (
        MessageCreatedIntegrationEvent messageCreatedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageEditedEventAsync
    (
        MessageEditedIntegrationEvent messageEditedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageDeletedEventAsync
    (
        MessageDeletedIntegrationEvent messageDeletedEvent,
        CancellationToken cancellationToken = default
    )
    {

    }

    private async Task HandleMessageSeenEventAsync
    (
        MessageSeenIntegrationEvent messageSeenEvent,
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
            await _imageService.DeleteImageAsync(imageRemovedEvent.ImageName, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete image, sending to work-item service {0}.", e);
            var urls = await Task.WhenAll(Enum.GetValues<ImageSize>().Select(imageSize => _imageService.GetImageUrlAsync(imageRemovedEvent.ImageName, imageSize, cancellationToken)));
            await Task.WhenAll(urls.Select(url => _workItemService.PerformAsync(new DeleteImage(url), cancellationToken)));
        }
    }
}