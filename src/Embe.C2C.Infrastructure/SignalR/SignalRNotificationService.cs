using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Events.Messages;
using Embe.C2C.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRNotificationService(IHubContext<MainHub> hubContext) : INotificationService
{
    private readonly IHubContext<MainHub> _hubContext = hubContext;

    public Task SendNotificationAsync<T>(T notification, CancellationToken cancellationToken = default)
    {
        return notification switch
        {
            MessageCreated messageCreated => SendMessageCreatedNotificationAsync(messageCreated, cancellationToken),
            MessageEdited messageEdited => SendMessageEditedNotificationAsync(messageEdited, cancellationToken),
            MessageDeleted messageDeleted => SendMessageDeletedNotificationAsync(messageDeleted, cancellationToken),
            MessageSeen messageSeen => SendMessageSeenNotificationAsync(messageSeen, cancellationToken),
            MessageUnseen messageUnseen => SendMessageUnseenNotificationAsync(messageUnseen, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    #region messages
    private Task SendMessageCreatedNotificationAsync(MessageCreated messageCreated, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(messageCreated.RecipientUserId.ToString())
            .SendAsync("MessageAdded", messageCreated.MessageId, messageCreated.ConversationId, cancellationToken);
    }

    private Task SendMessageEditedNotificationAsync(MessageEdited messageEdited, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(messageEdited.RecipientUserId.ToString())
            .SendAsync("MessageEdited", messageEdited.MessageId, messageEdited.ConversationId, cancellationToken);
    }

    private Task SendMessageDeletedNotificationAsync(MessageDeleted messageDeleted, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(messageDeleted.RecipientUserId.ToString())
            .SendAsync("MessageDeleted", messageDeleted.MessageId, messageDeleted.ConversationId, cancellationToken);
    }

    private Task SendMessageSeenNotificationAsync(MessageSeen messageSeen, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(messageSeen.AuthorUserId.ToString())
            .SendAsync("MessagesSeen", new[] { messageSeen.MessageId }, messageSeen.ConversationId, cancellationToken);
    }

    private Task SendMessageUnseenNotificationAsync(MessageUnseen messageUnseen, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(messageUnseen.AuthorUserId.ToString())
        .SendAsync("MessagesUnseen", new[] { messageUnseen.MessageId }, messageUnseen.ConversationId, cancellationToken);
    }

    #endregion
}