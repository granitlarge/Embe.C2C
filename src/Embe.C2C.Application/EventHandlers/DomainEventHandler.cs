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
    INotificationRepository notificationRepository,
    IUserRepository userRepo,
    IRepository context,
    IMatchingRepository matchingRepo
) : IntegrationEventCollector
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IRepository _context = context;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IMatchingRepository _matchingRepo = matchingRepo;

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
        var matcherUser = await _userRepo.GetByIdAsync(matcherUserId, cancellationToken);

        if (matcherUser is null)
        {
            // Matcher has been deleted -> no matching.
            return;
        }

        var notification = new MatchingCreated
        (
            matcheeUserId,
            matching.Id,
            matcherUserId,
            matcherUser.Alias.Value,
            matcherUser.ProfilePicture?.ImageDetails.Name
        );

        _notificationRepository.Set.Add(notification);

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
        var matcherUser = await _userRepo.GetByIdAsync(matchRemoveeUserId, cancellationToken);

        if (matcherUser is null)
        {
            return;
        }

        var notification = new MatchingCreated
        (
            matchRemoveeUserId,
            matching.Id,
            matchRemoverUserId,
            matcherUser.Alias.Value,
            matcherUser.ProfilePicture?.ImageDetails.Name
        );

        _notificationRepository.Set.Add(notification);
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
        var matching = await _matchingRepo.GetByIdAsync(matchingId, cancellationToken);
        if (matching is null)
            return;
        var recipientUserId = matching.UserId1 == authorUserId ? matching.UserId2 : matching.UserId1;
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
        var matching = await _matchingRepo.GetByIdAsync(matchingId, cancellationToken);
        if (matching is null)
            return;
        var recipientUserId = matching.UserId1 == authorUserId ? matching.UserId2 : matching.UserId1;
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
        var matching = await _matchingRepo.GetByIdAsync(matchingId, cancellationToken);
        if (matching is null)
            return;
        var recipientUserId = matching.UserId1 == authorUserId ? matching.UserId2 : matching.UserId1;
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
        var matching = await _matchingRepo.GetByIdAsync(matchingId, cancellationToken);
        if (matching is null)
            return;
        var recipientUserId = matching.UserId2 == authorUserId ? matching.UserId2 : matching.UserId1;
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
        var matching = await _matchingRepo.GetByIdAsync(matchingId, cancellationToken);
        if (matching is null)
            return;
        var recipientUserId = matching.UserId2 == authorUserId ? matching.UserId2 : matching.UserId1;
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