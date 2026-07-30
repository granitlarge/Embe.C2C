using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.Events.SearchProfiles;
using Embe.C2C.Application.Services;
using Embe.C2C.Domain.Aggregates.SearchProfiles.Events;

namespace Embe.C2C.Application.EventHandlers;

public class IntegrationEventHandler
(
    INotificationService notificationService,
    IImageService imageService,
    IWorkItemService workItemService,
    IRealTimeUpdateService realTimeUpdateService,
    ILoggerFactory loggerFactory
)
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IRealTimeUpdateService _realTimeUpdateService = realTimeUpdateService;
    private readonly IImageService _imageService = imageService;
    private readonly IWorkItemService _workItemService = workItemService;
    private readonly ILogger<IntegrationEventHandler> _logger = loggerFactory.Create<IntegrationEventHandler>();

    public async Task HandleAsync(IntegrationEventCollector eventCollector, CancellationToken cancellationToken = default)
    {
        await _logger.TraceAsync(nameof(HandleAsync));
        var events = eventCollector.CollectedEvents;
        foreach (var @event in events.OrderBy(e => e.Timestamp))
        {
            try
            {
                await HandleAsync(@event, cancellationToken);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync(e.ToString());
            }
        }
    }

    private async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await _logger.TraceAsync(nameof(HandleAsync));
        if (integrationEvent is NotificationIntegrationEvent notificationIntegrationEvent)
        {
            try
            {
                await _notificationService.SendAsync(notificationIntegrationEvent, cancellationToken);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync(e.ToString());
            }
        }

        try
        {
            await _realTimeUpdateService.SendAsync(integrationEvent, cancellationToken);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.ToString());
        }

        switch (integrationEvent)
        {
            case NotificationCreatedIntegrationEvent notificationCreatedEvent:
                await HandleNotificationCreatedEventAsync(notificationCreatedEvent, cancellationToken);
                break;
            case ImageRemovedEvent imageRemovedEvent:
                await HandleImageRemovedEventAsync(imageRemovedEvent, cancellationToken);
                break;
            case SearchProfileUpdatedIntegrationEvent searchProfileUpdatedEvent:
                await HandleSearchProfileUpdatedEventAsync(searchProfileUpdatedEvent, cancellationToken);
                break;
            case SearchProfileDescriptionChangedIntegrationEvent searchProfileDescriptionChangedEvent:
                await HandleSearchProfileDescriptionChangedEventAsync(searchProfileDescriptionChangedEvent, cancellationToken);
                break;
            default:
                break;
        }
    }

    private async Task HandleSearchProfileDescriptionChangedEventAsync(SearchProfileDescriptionChangedIntegrationEvent searchProfileDescriptionChangedEvent, CancellationToken cancellationToken)
    {
        var payload = new GenerateSearchProfileDescriptionEmbedding(searchProfileDescriptionChangedEvent.SearchProfileId, searchProfileDescriptionChangedEvent.NewDescription);
        var workItem = WorkItem.Create(payload, WorkItemType.GenerateSearchProfileDescriptionEmbedding);
        await _workItemService.PerformAsync(workItem, cancellationToken);
    }

    private Task HandleSearchProfileUpdatedEventAsync(SearchProfileUpdatedIntegrationEvent searchProfileUpdatedEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
        await _logger.TraceAsync(nameof(HandleImageRemovedEventAsync));

        try
        {
            await _imageService.DeleteImageAsync(imageRemovedEvent.ImageName, cancellationToken);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.ToString());
            var urls = await Task.WhenAll(Enum.GetValues<ImageSize>().Select(imageSize => _imageService.GetImageUrlAsync(imageRemovedEvent.ImageName, imageSize, cancellationToken)));
            await Task.WhenAll(urls.Select(url => _workItemService.PerformAsync(WorkItem.Create(new DeleteImage(url), WorkItemType.DeleteImage), cancellationToken)));
        }
    }
}