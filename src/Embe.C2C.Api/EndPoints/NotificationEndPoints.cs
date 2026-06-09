using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Notifications;
using Embe.C2C.Application.Commands.Notifications.Handlers;

namespace Embe.C2C.Api.EndPoints;

public static class NotificationEndPoints
{
    public static void MapNotificationEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/notifications").RequireAuthorization();
        group.MapPost("/mark-as-read", MarkAsRead)
            .WithName("MarkNotificationAsRead");
    }

    private static async Task<IResult> MarkAsRead(MarkAsReadCommand command, MarkAsReadHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }
}