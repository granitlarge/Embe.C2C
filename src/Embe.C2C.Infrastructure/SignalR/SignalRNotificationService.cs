using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Matchings;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService(SignalRServiceHubContextPool pool) : INotificationService
{
    private readonly SignalRServiceHubContextPool _pool = pool;

    public Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default)
    {
        return notification switch
        {
            MessageCreatedIntegrationEvent messageCreated => SendMessageCreatedNotificationAsync(messageCreated, cancellationToken),
            MessageEditedIntegrationEvent messageEdited => SendMessageEditedNotificationAsync(messageEdited, cancellationToken),
            MessageDeletedIntegrationEvent messageDeleted => SendMessageDeletedNotificationAsync(messageDeleted, cancellationToken),
            MessageSeenIntegrationEvent messageSeen => SendMessageSeenNotificationAsync(messageSeen, cancellationToken),
            MessageUnseenIntegrationEvent messageUnseen => SendMessageUnseenNotificationAsync(messageUnseen, cancellationToken),

            NotificationCreatedIntegrationEvent notificationCreated => SendNotificationCreatedAsync(notificationCreated, cancellationToken),
            NotificationRemovedIntegrationEvent notificationRemoved => SendNotificationRemovedAsync(notificationRemoved, cancellationToken),

            MatchingCreatedIntegrationEvent matchingCreated => SendMatchingCreatedNotificationAsync(matchingCreated, cancellationToken),
            MatchingRemovedIntegrationEvent matchingRemoved => SendMatchingRemovedNotificationAsync(matchingRemoved, cancellationToken),

            _ => Task.CompletedTask
        };
    }

    private async Task SendNotificationRemovedAsync(NotificationRemovedIntegrationEvent notificationRemoved, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext
                .Clients
                .User(notificationRemoved.RecipientUserId.ToString())
                .SendAsync("NotificationRemoved", notificationRemoved.NotificationId, cancellationToken);
    }

    private async Task SendNotificationCreatedAsync(NotificationCreatedIntegrationEvent notificationCreated, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext
                .Clients
                .User(notificationCreated.RecipientUserId.ToString())
                .SendAsync("NotificationCreated", notificationCreated.NotificationId, cancellationToken);
    }

    #region messages
    private async Task SendMessageCreatedNotificationAsync(MessageCreatedIntegrationEvent messageCreated, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageCreated.RecipientUserId.ToString())
            .SendAsync("MessageAdded", messageCreated.MessageId, messageCreated.MatchingId, cancellationToken);
    }

    private async Task SendMessageEditedNotificationAsync(MessageEditedIntegrationEvent messageEdited, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageEdited.RecipientUserId.ToString())
            .SendAsync("MessageEdited", messageEdited.MessageId, messageEdited.ConversationId, cancellationToken);
    }

    private async Task SendMessageDeletedNotificationAsync(MessageDeletedIntegrationEvent messageDeleted, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageDeleted.RecipientUserId.ToString())
            .SendAsync("MessageDeleted", messageDeleted.MessageId, messageDeleted.ConversationId, cancellationToken);
    }

    private async Task SendMessageSeenNotificationAsync(MessageSeenIntegrationEvent messageSeen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageSeen.AuthorUserId.ToString())
            .SendAsync("MessagesSeen", new[] { messageSeen.MessageId }, messageSeen.ConversationId, cancellationToken);
    }

    private async Task SendMessageUnseenNotificationAsync(MessageUnseenIntegrationEvent messageUnseen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageUnseen.AuthorUserId.ToString())
            .SendAsync("MessagesUnseen", new[] { messageUnseen.MessageId }, messageUnseen.ConversationId, cancellationToken);
    }

    #endregion

    #region matchings

    private async Task SendMatchingCreatedNotificationAsync
    (
        MatchingCreatedIntegrationEvent matchingCreated,
        CancellationToken cancellationToken
    )
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(matchingCreated.MatcheeUserId.ToString())
            .SendAsync("MatchingCreated", matchingCreated.MatchingId, cancellationToken: cancellationToken);
    }

    private async Task SendMatchingRemovedNotificationAsync
    (
        MatchingRemovedIntegrationEvent matchingRemoved,
        CancellationToken cancellationToken
    )
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(matchingRemoved.RecipientUserId.ToString())
            .SendAsync("MatchingRemoved", matchingRemoved.MatchingId, cancellationToken: cancellationToken);
    }

    #endregion

    private async Task SendImageResizedNotificationAsync(ImageResizedEvent imageResizedEvent, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(imageResizedEvent.UserId.ToString())
            .SendAsync
            (
                "ImageResized",
                imageResizedEvent.ImageId,
                imageResizedEvent.OriginalUrl,
                imageResizedEvent.LargeUrl,
                imageResizedEvent.MediumUrl,
                imageResizedEvent.SmallUrl,
                cancellationToken
            );
    }

}