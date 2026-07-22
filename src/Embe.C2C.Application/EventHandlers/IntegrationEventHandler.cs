using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.EventHandlers;

public class IntegrationEventHandler
(
    IRepository repository,
    IRealTimeNotificationService notificationService,
    IImageService imageService,
    IWorkItemService workItemService
)
{
    private readonly IRepository _repository = repository;
    private readonly IRealTimeNotificationService _notificationService = notificationService;
    private readonly IImageService _imageService = imageService;
    private readonly IWorkItemService _workItemService = workItemService;

    public async Task HandleAsync(IntegrationEventCollector eventCollector, CancellationToken cancellationToken = default)
    {
#warning consider whether we should catch exceptions here or not, i.e. are integration-events uncorrelated or correlated?
        var events = eventCollector.CollectedEvents;
        foreach (var @event in events.OrderBy(e => e.Timestamp))
        {
            await HandleAsync(@event, cancellationToken);
        }
    }

    private async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        // Notify clients about the event using the notification service
        await _notificationService.SendAsync(integrationEvent, cancellationToken);

        switch (integrationEvent)
        {
            case NotificationCreatedEvent notificationCreatedEvent:
                await HandleNotificationCreatedEventAsync(notificationCreatedEvent, cancellationToken);
                break;
            case ImageRemovedEvent imageRemovedEvent:
                await HandleImageRemovedEventAsync(imageRemovedEvent, cancellationToken);
                break;
            case ImageStatusChangedEvent imageMovedEvent:
                await HandleStatusChangedEventAsync(imageMovedEvent, cancellationToken);
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
            await Task.WhenAll(Enum.GetValues<ImageSize>().Select(imageSize => _imageService.DeleteImageAsync(imageRemovedEvent.ImageName, imageRemovedEvent.ImageStatus, imageSize, cancellationToken)));
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete image, sending to work-item service {0}.", e);
            var urls = await Task.WhenAll(Enum.GetValues<ImageSize>().Select(imageSize => _imageService.GetImageUrlAsync(imageRemovedEvent.ImageName, imageRemovedEvent.ImageStatus, imageSize, cancellationToken)));
            await Task.WhenAll(urls.Select(url => _workItemService.PerformAsync(new DeleteImage(url), cancellationToken)));
        }
    }

    private async Task HandleStatusChangedEventAsync
    (
        ImageStatusChangedEvent imageStatusChangedEvent,
        CancellationToken cancellationToken
    )
    {
        var fromUrl = await _imageService.GetImageUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.OldStatus, ImageSize.Original, cancellationToken);
        var toUrl = await _imageService.GetImageUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Original, cancellationToken);

        var fromExists = await _imageService.ExistsByUrlAsync(fromUrl, cancellationToken);
        var toExists = await _imageService.ExistsByUrlAsync(toUrl, cancellationToken);

        if (fromExists && !toExists)
        {
            try
            {
                await _imageService.MoveImageAsync(fromUrl, toUrl, cancellationToken);
            }
            catch (Exception)
            {
                await _workItemService.PerformAsync(new ExecuteIntegrationEventHandler<ImageStatusChangedEvent>(imageStatusChangedEvent), cancellationToken);
                throw;
            }
        }
        else if (!fromExists && toExists)
        {
            // image has already been moved
        }
        else
        {
            Console.WriteLine("The source blob was deleted before it could be moved.");
            return;
        }

        if (imageStatusChangedEvent.NewStatus != Domain.ValueObjects.ImageStatus.Accepted)
        {
            return;
        }

        // If the image has been accepted, we need to resize it.
        // width 1000
        // width 500
        // width 250

        var user = await _repository.DomainUsersQuery.FirstOrDefaultAsync(du => du.Id == imageStatusChangedEvent.UserId, cancellationToken: cancellationToken);
        if (user == null)
            return;

        var image = user.Images.FirstOrDefault(i => i.Id == imageStatusChangedEvent.ImageId);
        if (image == null)
            return;

        var cropOffsetX = (int)image.ImageDetails.CropOffsetX;
        var cropOffsetY = (int)image.ImageDetails.CropOffsetY;

        Console.WriteLine("CROPPING WITH OFFSET: " + cropOffsetX + ", " + cropOffsetY);
        var cropWidth = 1000;
        var cropHeight = (int)(cropWidth * 2.1);

        string croppedImageUrl;
        if (!await _imageService.ExistsAsync(image.ImageDetails.Name, image.ImageDetails.Status, ImageSize.Large, cancellationToken))
        {
            try
            {
                croppedImageUrl = await _imageService.CropImageAsync
                (
                    toUrl,
                    cropWidth,
                    cropHeight,
                    cropOffsetX,
                    cropOffsetY,
                    image.ImageDetails.Name,
                    image.ImageDetails.Status,
                    ImageSize.Large,
                    cancellationToken
                );
            }
            catch (Exception)
            {
                await _workItemService.PerformAsync(new ExecuteIntegrationEventHandler<ImageStatusChangedEvent>(imageStatusChangedEvent), cancellationToken);
                throw;
            }
        }
        else
        {
            croppedImageUrl = await _imageService.GetImageUrlAsync(image.ImageDetails.Name, image.ImageDetails.Status, ImageSize.Large, cancellationToken);
        }

        var scalingFactors = new double[] { 0.5, 0.25 };
        string mediumUrl;
        string smallUrl;
        foreach (var scalingFactor in scalingFactors)
        {
            var imageSize = Math.Abs(scalingFactor - 0.5) < .001 ? ImageSize.Medium : ImageSize.Small;
            if (!await _imageService.ExistsAsync(image.ImageDetails.Name, image.ImageDetails.Status, imageSize, cancellationToken))
            {
                try
                {
                    var url = await _imageService.ScaleImageAsync
                    (
                        croppedImageUrl,
                        scalingFactor,
                        image.ImageDetails.Name,
                        image.ImageDetails.Status,
                        imageSize,
                        cancellationToken
                    );
                    if (imageSize == ImageSize.Medium)
                    {
                        mediumUrl = url;
                    }
                    else
                    {
                        smallUrl = url;
                    }
                }
                catch (Exception)
                {
                    await _workItemService.PerformAsync(new ExecuteIntegrationEventHandler<ImageStatusChangedEvent>(imageStatusChangedEvent), cancellationToken);
                    throw;
                }
            }
        }

        var originalUrl = await _imageService.GetImageUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Original, cancellationToken);
        var sasUrls = await Task.WhenAll
        (
            _imageService.GenerateImageSasUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Original, FilePermissions.Read, TimeSpan.FromHours(1)),
            _imageService.GenerateImageSasUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Large, FilePermissions.Read, TimeSpan.FromHours(1)),
            _imageService.GenerateImageSasUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Medium, FilePermissions.Read, TimeSpan.FromHours(1)),
            _imageService.GenerateImageSasUrlAsync(imageStatusChangedEvent.ImageName, imageStatusChangedEvent.NewStatus, ImageSize.Small, FilePermissions.Read, TimeSpan.FromHours(1))
        );

        var originalSasUrl = sasUrls[0];
        var largeSasUrl = sasUrls[1];
        var mediumSasUrl = sasUrls[2];
        var smallSasUrl = sasUrls[3];

        await HandleAsync(new ImageResizedEvent(imageStatusChangedEvent.UserId, imageStatusChangedEvent.ImageId, originalSasUrl, largeSasUrl, mediumSasUrl, smallSasUrl), cancellationToken);

    }
}