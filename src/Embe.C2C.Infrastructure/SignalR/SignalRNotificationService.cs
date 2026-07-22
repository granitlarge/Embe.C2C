using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events.Images;
using Embe.C2C.Application.Events.Messages;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService(SignalRServiceHubContextPool pool) : INotificationService
{
    private readonly SignalRServiceHubContextPool _pool = pool;

    public Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default)
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
            _ => Task.CompletedTask
        };
    }

    #region messages
    private async Task SendMessageCreatedNotificationAsync(MessageCreated messageCreated, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageCreated.RecipientUserId.ToString())
            .SendAsync("MessageAdded", messageCreated.MessageId, messageCreated.MatchingId, cancellationToken);
    }

    private async Task SendMessageEditedNotificationAsync(MessageEdited messageEdited, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageEdited.RecipientUserId.ToString())
            .SendAsync("MessageEdited", messageEdited.MessageId, messageEdited.ConversationId, cancellationToken);
    }

    private async Task SendMessageDeletedNotificationAsync(MessageDeleted messageDeleted, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageDeleted.RecipientUserId.ToString())
            .SendAsync("MessageDeleted", messageDeleted.MessageId, messageDeleted.ConversationId, cancellationToken);
    }

    private async Task SendMessageSeenNotificationAsync(MessageSeen messageSeen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageSeen.AuthorUserId.ToString())
            .SendAsync("MessagesSeen", new[] { messageSeen.MessageId }, messageSeen.ConversationId, cancellationToken);
    }

    private async Task SendMessageUnseenNotificationAsync(MessageUnseen messageUnseen, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
        await hubContext.Clients.User(messageUnseen.AuthorUserId.ToString())
            .SendAsync("MessagesUnseen", new[] { messageUnseen.MessageId }, messageUnseen.ConversationId, cancellationToken);
    }

    private async Task SendImageStatusChangedNotificationAsync(ImageStatusChangedEvent imageStatusChanged, CancellationToken cancellationToken)
    {
        var hubContext = await _pool.GetHubContextAsync(cancellationToken);
            await hubContext.Clients.User(imageStatusChanged.UserId.ToString())
            .SendAsync
            (
                "ImageStatusChanged",
                imageStatusChanged.ImageId,
                imageStatusChanged.NewStatus,
                cancellationToken
            );
    }

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

    #endregion
}