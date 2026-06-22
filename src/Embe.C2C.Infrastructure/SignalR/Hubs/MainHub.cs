using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Embe.C2C.Infrastructure.SignalR.Hubs;

[Authorize]
public class MainHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return Task.CompletedTask;
    }
    public async Task StartedTyping
    (
        Guid recipientUserId,
        Guid conversationId
    )
    {
        await Clients.User(recipientUserId.ToString()).SendAsync("StartedTyping", conversationId);
    }

    public async Task StoppedTyping
    (
        Guid recipientUserId,
        Guid conversationId
    )
    {
        await Clients.User(recipientUserId.ToString()).SendAsync("StoppedTyping", conversationId);
    }
}