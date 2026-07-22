using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Microsoft.EntityFrameworkCore;
using NotificationUpdatedEvent = Embe.C2C.Domain.Aggregates.Notifications.Events.NotificationUpdatedEvent;

namespace Embe.C2C.Application.EventHandlers;

public class DomainEventHandler
(
    IRepository context
) : IntegrationEventCollector
{
    private readonly IRepository _context = context;

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

            case MessageCreatedEvent messageCreatedEvent:
                await HandleMessageCreatedEventAsync(messageCreatedEvent, cancellationToken);
                break;
            case MessageEditedEvent messageEditedEvent:
                await HandleMessageEditedEventAsync(messageEditedEvent, cancellationToken);
                break;
            case MessageRemovedEvent messageRemovedEvent:
                await HandleMessageRemovedEventAsync(messageRemovedEvent, cancellationToken);
                break;
            case MessageSeenEvent messageSeenEvent:
                await HandleMessageSeenEventAsync(messageSeenEvent, cancellationToken);
                break;
            case MessageUnseenEvent messageUnseenEvent:
                await HandleMessageUnseenEventAsync(messageUnseenEvent, cancellationToken);
                break;

            case UserImageStatusChangedEvent userImageStatusChangedEvent:
                await HandleUserImageStatusChangedEventAsync(userImageStatusChangedEvent, cancellationToken);
                break;
            case UserImageRemovedEvent userImageRemovedEvent:
                await HandleUserImageRemovedEventAsync(userImageRemovedEvent, cancellationToken);
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
            matcheeUser.Alias.Value,
            matcheeUser.ProfilePicture?.ImageDetails.Name
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
            matcheeUser.Alias.Value,
            matcheeUser.ProfilePicture?.ImageDetails.Name
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

    private async Task HandleMessageCreatedEventAsync
    (
        MessageCreatedEvent messageCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        var matchingId = messageCreatedEvent.Message.MatchingId;
        var authorUserId = messageCreatedEvent.Message.AuthorUserId;
        var recipientUserId = await _context.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => m.UserId1 == authorUserId ? m.UserId2 : m.UserId1)
            .FirstOrDefaultAsync(cancellationToken);
        var messageId = messageCreatedEvent.Message.Id;
        var messageCreated = new MessageCreated(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageCreated);
    }

    private async Task HandleMessageEditedEventAsync
    (
        MessageEditedEvent messageEditedEvent,
        CancellationToken cancellationToken
    )
    {
        var matchingId = messageEditedEvent.Message.MatchingId;
        var authorUserId = messageEditedEvent.Message.AuthorUserId;
        var recipientUserId = await _context.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => m.UserId1 == authorUserId ? m.UserId2 : m.UserId1)
            .FirstOrDefaultAsync(cancellationToken);
        var messageId = messageEditedEvent.Message.Id;
        var messageEdited = new MessageEdited(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageEdited);
    }

    private async Task HandleMessageRemovedEventAsync
    (
        MessageRemovedEvent messageRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var matchingId = messageRemovedEvent.Message.MatchingId;
        var authorUserId = messageRemovedEvent.Message.AuthorUserId;
        var recipientUserId = await _context.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => m.UserId1 == authorUserId ? m.UserId2 : m.UserId1)
            .FirstOrDefaultAsync(cancellationToken);
        var messageId = messageRemovedEvent.Message.Id;
        var messageDeleted = new MessageDeleted(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageDeleted);
    }

    private async Task HandleMessageSeenEventAsync
    (
        MessageSeenEvent messageSeenEvent,
        CancellationToken cancellationToken
    )
    {
        var matchingId = messageSeenEvent.Message.MatchingId;
        var authorUserId = messageSeenEvent.Message.AuthorUserId;
        var recipientUserId = await _context.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => m.UserId1 == authorUserId ? m.UserId2 : m.UserId1)
            .FirstOrDefaultAsync(cancellationToken);
        var messageId = messageSeenEvent.Message.Id;
        var messageSeen = new MessageSeen(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageSeen);
    }

    private async Task HandleMessageUnseenEventAsync
    (
        MessageUnseenEvent messageUnseenEvent,
        CancellationToken cancellationToken
    )
    {
        var matchingId = messageUnseenEvent.Message.MatchingId;
        var authorUserId = messageUnseenEvent.Message.AuthorUserId;
        var recipientUserId = await _context.MatchingsQuery
            .Where(m => m.Id == matchingId)
            .Select(m => m.UserId1 == authorUserId ? m.UserId2 : m.UserId1)
            .FirstOrDefaultAsync(cancellationToken);
        var messageId = messageUnseenEvent.Message.Id;
        var messageUnseen = new MessageUnseen(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageUnseen);
    }

    private async Task HandleUserImageStatusChangedEventAsync
    (
        UserImageStatusChangedEvent userImageStatusChangedEvent,
        CancellationToken cancellationToken
    )
    {
        var oldStatus = userImageStatusChangedEvent.OldStatus;
        var image = userImageStatusChangedEvent.Image;
        AddIntegrationEvent(new ImageStatusChangedEvent(image.OwnerUserId, image.Id, image.ImageDetails.Name, oldStatus, image.ImageDetails.Status)); ;
    }

    private async Task HandleUserImageRemovedEventAsync
    (
        UserImageRemovedEvent userImageRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var image = userImageRemovedEvent.Image;
        AddIntegrationEvent(new ImageRemovedEvent(image.OwnerUserId, image.Id, image.ImageDetails.Name, image.ImageDetails.Status));
    }
}