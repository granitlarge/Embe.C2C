using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Matchings;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Events;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications.Messages;
using Embe.C2C.Domain.Aggregates.Users.Events;

namespace Embe.C2C.Application.EventHandlers;

public class DomainEventHandler
(
    INotificationRepository notificationRepository,
    IUserRepository userRepo,
    IMatchingRepository matchingRepo
) : IntegrationEventCollector
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;

    private readonly IUserRepository _userRepo = userRepo;
    private readonly IMatchingRepository _matchingRepo = matchingRepo;

    public async Task HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {

        switch (domainEvent)
        {
            case UserCreatedEvent userCreatedEvent:
                await HandleUserCreatedEventAsync(userCreatedEvent, cancellationToken);
                break;
            case MatchingCreatedDomainEvent matchingCreatedEvent:
                await HandleMatchingCreatedEventAsync(matchingCreatedEvent, cancellationToken);
                break;
            case MatchingRemovedEvent matchingRemovedEvent:
                await HandleMatchingRemovedEventAsync(matchingRemovedEvent, cancellationToken);
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

            case UserImageRemovedEvent userImageRemovedEvent:
                await HandleUserImageRemovedEventAsync(userImageRemovedEvent, cancellationToken);
                break;
            
            case NotificationRemovedEvent notificationRemovedEvent:
                await HandleNotificationRemovedEventAsync(notificationRemovedEvent, cancellationToken);
                break;

            default:
                break;

        }

    }

    private async Task HandleNotificationRemovedEventAsync(NotificationRemovedEvent notificationRemovedEvent, CancellationToken cancellationToken)
    {
        AddIntegrationEvent(new NotificationRemovedIntegrationEvent(notificationRemovedEvent.Notification.Id, notificationRemovedEvent.Notification.RecipientUserId));
    }

    private async Task HandleUserCreatedEventAsync
    (
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        return;
    }

    private Task HandleMatchingCreatedEventAsync
    (
        MatchingCreatedDomainEvent matchingCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingCreatedEvent.Matching;
        var matcheeUserId = matchingCreatedEvent.LastJudgeUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;
        var matcherUserId = matchingCreatedEvent.LastJudgeUserId;

        var notification = new MatchingCreatedNotification
        (
            matcheeUserId,
            matching.Id,
            matcherUserId
        );

        _notificationRepository.Set.Add(notification);

        var notificationCreatedIntegrationEvent = new NotificationCreatedIntegrationEvent(matcheeUserId, notification.Id);
        var matchingCreatedIntegrationEvent = new MatchingCreatedIntegrationEvent(matchingCreatedEvent.Matching.Id, matcheeUserId);
        AddIntegrationEvent(notificationCreatedIntegrationEvent);
        AddIntegrationEvent(matchingCreatedIntegrationEvent);

        return Task.CompletedTask;
    }

    private async Task HandleMatchingRemovedEventAsync
    (
        MatchingRemovedEvent matchingRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingRemovedEvent.Matching;
        var matchRemoveeUserId = matchingRemovedEvent.RemoverUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;
        AddIntegrationEvent(new MatchingRemovedIntegrationEvent(matching.Id, matchRemoveeUserId));
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
        var messageCreated = new MessageCreatedIntegrationEvent(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageCreated);

        var notification = MessageCreatedNotification.Create(messageCreatedEvent.Message.Id, recipientUserId);
        _notificationRepository.Set.Add(notification);
        AddIntegrationEvent(new NotificationCreatedIntegrationEvent(notification.RecipientUserId, notification.Id));
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
        var messageEdited = new MessageEditedIntegrationEvent(matchingId, authorUserId, recipientUserId, messageId);
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
        var messageDeleted = new MessageDeletedIntegrationEvent(matchingId, authorUserId, recipientUserId, messageId);
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
        var messageSeen = new MessageSeenIntegrationEvent(matchingId, authorUserId, recipientUserId, messageId);
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
        var messageUnseen = new MessageUnseenIntegrationEvent(matchingId, authorUserId, recipientUserId, messageId);
        AddIntegrationEvent(messageUnseen);
    }

    private async Task HandleUserImageRemovedEventAsync
    (
        UserImageRemovedEvent userImageRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var image = userImageRemovedEvent.Image;
        AddIntegrationEvent(new ImageRemovedEvent(image.OwnerUserId, image.Id, image.ImageDetails.Name));
    }
}