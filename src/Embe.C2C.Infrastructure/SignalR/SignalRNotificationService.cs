using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Candidates;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Matchings;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService(SignalRServiceHubContextPool pool) : IRealTimeNotificationService, IRealTimeUpdateService
{
    private readonly SignalRServiceHubContextPool _pool = pool;

    public Task SendAsync<T>(T update, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
    {
        return update switch
        {
            MessageCreatedIntegrationEvent messageCreated => SendMessageCreatedNotificationAsync(messageCreated, cancellationToken),
            MessageEditedIntegrationEvent messageEdited => SendMessageEditedNotificationAsync(messageEdited, cancellationToken),
            MessageDeletedIntegrationEvent messageDeleted => SendMessageDeletedNotificationAsync(messageDeleted, cancellationToken),
            MessageSeenIntegrationEvent messageSeen => SendMessageSeenNotificationAsync(messageSeen, cancellationToken),
            MessageUnseenIntegrationEvent messageUnseen => SendMessageUnseenNotificationAsync(messageUnseen, cancellationToken),

            MatchingCreatedIntegrationEvent matchingCreated => SendMatchingCreatedNotificationAsync(matchingCreated, cancellationToken),
            MatchingRemovedIntegrationEvent matchingRemoved => SendMatchingRemovedNotificationAsync(matchingRemoved, cancellationToken),

            PositivelyJudgedIntegrationEvent positivelyJudged => SendPositivelyJudgedNotificationAsync(positivelyJudged, cancellationToken),

            _ => Task.CompletedTask
        };
    }

    Task<bool> IRealTimeNotificationService.SendAsync<T>(T notification, CancellationToken cancellationToken)
    {
        return notification switch
        {
            NotificationCreatedIntegrationEvent notificationCreated => SendNotificationCreatedAsync(notificationCreated, cancellationToken),
            NotificationRemovedIntegrationEvent notificationRemoved => SendNotificationRemovedAsync(notificationRemoved, cancellationToken),
            _ => Task.FromResult(false)
        };
    }

    private async Task SendPositivelyJudgedNotificationAsync(PositivelyJudgedIntegrationEvent positivelyJudged, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext
                .Clients
                .User(positivelyJudged.RecipientUserId.ToString())
                .SendAsync("PositivelyJudged", positivelyJudged.CandidateId, cancellationToken);
    }

    private async Task<bool> SendNotificationRemovedAsync(NotificationRemovedIntegrationEvent notificationRemoved, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        if (!await hubContext.ClientManager.UserExistsAsync(notificationRemoved.RecipientUserId.ToString(), cancellationToken))
        {
            return false;
        }

        await hubContext
                .Clients
                .User(notificationRemoved.RecipientUserId.ToString())
                .SendAsync("NotificationRemoved", notificationRemoved.NotificationId, cancellationToken);

        return true;
    }

    private async Task<bool> SendNotificationCreatedAsync(NotificationCreatedIntegrationEvent notificationCreated, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        if (!await hubContext.ClientManager.UserExistsAsync(notificationCreated.RecipientUserId.ToString(), cancellationToken))
        {
            return false;
        }
        await hubContext
                .Clients
                .User(notificationCreated.RecipientUserId.ToString())
                .SendAsync("NotificationCreated", notificationCreated.NotificationId, cancellationToken);
        return true;
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