using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRRealTimeNotificationService(SignalRServiceHubContextPool pool) : IRealTimeNotificationService
{
    private readonly SignalRServiceHubContextPool _pool = pool;

    public Task<bool> SendAsync<T>
    (
        T notification,
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent
    {
        return notification switch
        {
            MessageCreated messageCreated => SendMessageCreatedNotificationAsync(messageCreated, cancellationToken),
            MessageEdited messageEdited => SendMessageEditedNotificationAsync(messageEdited, cancellationToken),
            MessageDeleted messageDeleted => SendMessageDeletedNotificationAsync(messageDeleted, cancellationToken),
            MessageSeen messageSeen => SendMessageSeenNotificationAsync(messageSeen, cancellationToken),
            MessageUnseen messageUnseen => SendMessageUnseenNotificationAsync(messageUnseen, cancellationToken),

            ImageStatusChangedEvent imageStatusChangedEvent => SendImageStatusChangedNotificationAsync(imageStatusChangedEvent, cancellationToken),
            ImageResizedEvent imageResizedEvent => SendImageResizedNotificationAsync(imageResizedEvent, cancellationToken),
            _ => Task.FromResult(false)
        };
    }

    #region messages
    private async Task<bool> SendMessageCreatedNotificationAsync(MessageCreated messageCreated, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(messageCreated.RecipientUserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(messageCreated.RecipientUserId.ToString())
            .SendAsync("MessageAdded", messageCreated.MessageId, messageCreated.ConversationId, cancellationToken);
        return true;
    }

    private async Task<bool> SendMessageEditedNotificationAsync(MessageEdited messageEdited, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(messageEdited.RecipientUserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(messageEdited.RecipientUserId.ToString())
            .SendAsync("MessageEdited", messageEdited.MessageId, messageEdited.ConversationId, cancellationToken);
        return true;
    }

    private async Task<bool> SendMessageDeletedNotificationAsync(MessageDeleted messageDeleted, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(messageDeleted.RecipientUserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(messageDeleted.RecipientUserId.ToString())
            .SendAsync("MessageDeleted", messageDeleted.MessageId, messageDeleted.ConversationId, cancellationToken);
        return true;
    }

    private async Task<bool> SendMessageSeenNotificationAsync(MessageSeen messageSeen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(messageSeen.AuthorUserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(messageSeen.AuthorUserId.ToString())
            .SendAsync("MessagesSeen", new[] { messageSeen.MessageId }, messageSeen.ConversationId, cancellationToken);
        return true;
    }

    private async Task<bool> SendMessageUnseenNotificationAsync(MessageUnseen messageUnseen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(messageUnseen.AuthorUserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(messageUnseen.AuthorUserId.ToString())
            .SendAsync("MessagesUnseen", new[] { messageUnseen.MessageId }, messageUnseen.ConversationId, cancellationToken);
        return true;
    }
    #endregion

    #region images
    private async Task<bool> SendImageStatusChangedNotificationAsync(ImageStatusChangedEvent imageStatusChanged, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(imageStatusChanged.UserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
        await hubContext.Clients.User(imageStatusChanged.UserId.ToString())
        .SendAsync
        (
            "ImageStatusChanged",
            imageStatusChanged.ImageId,
            imageStatusChanged.NewStatus,
            cancellationToken
        );
        return true;
    }

    private async Task<bool> SendImageResizedNotificationAsync(ImageResizedEvent imageResizedEvent, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        var isUserOnline = await hubContext.ClientManager.UserExistsAsync(imageResizedEvent.UserId.ToString(), cancellationToken);
        if (!isUserOnline)
            return false;
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
        return true;
    }

    #endregion
}