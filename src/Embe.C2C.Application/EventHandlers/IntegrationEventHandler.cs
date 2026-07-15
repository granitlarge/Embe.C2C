using System.ComponentModel.DataAnnotations.Schema;
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
            case ImageRemovedEvent imageRemovedEvent:
                await HandleImageRemovedEventAsync(imageRemovedEvent, cancellationToken);
                break;
            case ImageMovedEvent imageMovedEvent:
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
        try
        {
            await _imageService.DeleteImageByUrlAsync(imageRemovedEvent.Url, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception while attempting to delete an image with url {0}: {1}", imageRemovedEvent.Url, e);
            await _workItemService.PerformAsync(new DeleteImage(imageRemovedEvent.Url), cancellationToken);
        }
    }

    private async Task HandleImageMovedEventAsync
    (
        ImageMovedEvent imageMovedEvent,
        CancellationToken cancellationToken
    )
    {
        var fromUrl = imageMovedEvent.FromUrl;
        var toUrl = imageMovedEvent.ToUrl;
        try
        {
            await _imageService.MoveImageAsync(fromUrl, toUrl, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception while attempting to move an image from {0} to {1}: {2}", imageMovedEvent.FromUrl, imageMovedEvent.ToUrl, e);
            await _workItemService.PerformAsync(new MoveFile(fromUrl, toUrl), cancellationToken);
        }
    }
}