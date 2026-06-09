using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Dtos.Aggregates;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.IntegrationEntities.Notifications;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Events;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Microsoft.EntityFrameworkCore;
using NotificationUpdatedEvent = Embe.C2C.Domain.Aggregates.Notifications.Events.NotificationUpdatedEvent;

namespace Embe.C2C.Application.EventHandlers;

public class DomainEventHandler : IntegrationEventCollector
{
    private readonly IRepository _context;

    public DomainEventHandler(IRepository context)
    {
        _context = context;
    }

    public async Task HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        switch (domainEvent)
        {
            case UserCreatedEvent userCreatedEvent:
                await HandleUserCreatedEventAsync(userCreatedEvent, cancellationToken);
                break;
            case MatchingCreatedEvent matchingCreatedEvent:
                await HandleMatchingCreatedEventAsync(matchingCreatedEvent, cancellationToken);
                break;
            case MatchingRemovedEvent matchingRemovedEvent:
                await HandleMatchingRemovedEventAsync(matchingRemovedEvent, cancellationToken);
                break;
            case NotificationUpdatedEvent notificationUpdatedEvent:
                await HandleNotificationUpdatedEventAsync(notificationUpdatedEvent, cancellationToken);
                break;
            case NotificationRemovedEvent notificationRemovedEvent:
                await HandleNotificationRemovedEventAsync(notificationRemovedEvent, cancellationToken);
                break;
            default:
                break;
        }
    }

    private async Task HandleUserCreatedEventAsync
    (
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        return;
    }

    private async Task HandleMatchingCreatedEventAsync
    (
        MatchingCreatedEvent matchingCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingCreatedEvent.Matching;
        var userIdToNotify = matchingCreatedEvent.LastJudgeUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;
        var userIdThatCausedEvent = matchingCreatedEvent.LastJudgeUserId;

        var notification = new MatchingCreated(userIdToNotify, matching.Id, matchingCreatedEvent.LastJudgeUserId);
        _context.Notifications.Add(notification);

        var partnerUser = await _context.DomainUsersQuery.AsNoTracking().SingleAsync(u => u.Id == userIdThatCausedEvent, cancellationToken);
        var partnerUserName = partnerUser.UserName.Value;
        var partnerProfileImageUrl = partnerUser.Files.OrderBy(f => f.FileDetails.Order).First().FileDetails.Url;
        var notificationDto = notification.ToDto();
        var integrationEntity = new MatchingCreatedNotificationIntegrationEntity
        (
            notificationDto.Id,
            notificationDto.RecipientUserId,
            notificationDto.IsRead,
            notificationDto.ReadAt,
            notificationDto.CreatedAt,
            notificationDto.UpdatedAt,
            matching.Id,
            partnerUserName,
            partnerProfileImageUrl
        );

        var integrationEvent = new NotificationCreatedEvent(notificationDto);
        AddIntegrationEvent(integrationEvent);
    }

    private Task HandleMatchingRemovedEventAsync
    (
        MatchingRemovedEvent matchingRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingRemovedEvent.Matching;
        var userIdToNotify = matchingRemovedEvent.RemoverUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;

        var notification = new MatchingRemoved(userIdToNotify, matching.Id, matchingRemovedEvent.RemoverUserId);
        _context.Notifications.Add(notification);
        AddIntegrationEvent(new NotificationCreatedEvent(notification.ToDto()));
        return Task.CompletedTask;
    }

    private Task HandleNotificationUpdatedEventAsync
    (
        NotificationUpdatedEvent notificationUpdatedEvent,
        CancellationToken cancellationToken
    )
    {
        var notification = notificationUpdatedEvent.Notification;
        AddIntegrationEvent(new Events.Notifications.NotificationUpdatedEvent(notification.ToDto()));
        return Task.CompletedTask;
    }

    private Task HandleNotificationRemovedEventAsync
    (
        NotificationRemovedEvent notificationRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var notification = notificationRemovedEvent.Notification;
        AddIntegrationEvent(new NotificationDeletedEvent(notification.Id));
        return Task.CompletedTask;
    }
}