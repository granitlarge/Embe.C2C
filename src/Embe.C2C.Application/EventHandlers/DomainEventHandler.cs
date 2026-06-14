using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
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
        var matcheeUserId = matchingCreatedEvent.LastJudgeUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;
        var matcherUserId = matchingCreatedEvent.LastJudgeUserId;
        var matcheeUser = await _context.DomainUsersQuery.AsNoTracking().SingleAsync(u => u.Id == matcheeUserId, cancellationToken);

        var notification = new MatchingCreated
        (
            matcheeUserId,
            matching.Id,
            matcherUserId,
            matcheeUser.UserName.Value,
            matcheeUser.ProfilePicture.FileDetails.Name
        );
        _context.Notifications.Add(notification);

        var notificationDto = notification.ToDto();
        var integrationEvent = new NotificationCreatedEvent(notificationDto);
        AddIntegrationEvent(integrationEvent);
    }

    private async Task HandleMatchingRemovedEventAsync
    (
        MatchingRemovedEvent matchingRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingRemovedEvent.Matching;
        var matchRemoveeUserId = matchingRemovedEvent.RemoverUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;
        var matchRemoverUserId = matchingRemovedEvent.RemoverUserId;
        var matcheeUser = await _context.DomainUsersQuery.AsNoTracking().SingleAsync(u => u.Id == matchRemoveeUserId, cancellationToken);

        var notification = new MatchingCreated
        (
            matchRemoveeUserId,
            matching.Id,
            matchRemoverUserId,
            matcheeUser.UserName.Value,
            matcheeUser.ProfilePicture.FileDetails.Name
        );
        _context.Notifications.Add(notification);
        AddIntegrationEvent(new NotificationCreatedEvent(notification.ToDto()));
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