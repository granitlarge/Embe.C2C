using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Notifications;
using Embe.C2C.Application.Commands.Notifications.Handlers;
using Embe.C2C.Application.Queries;
using Embe.C2C.Application.Queries.Notifications.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class NotificationEndPoints
{
    public static void MapNotificationEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/notifications").RequireAuthorization();

        group.MapPost("/mark-as-read", MarkAsRead)
            .WithName("MarkNotificationAsRead");

        group.MapPost("/get", GetNotifications)
            .WithName("GetNotifications");
    }

    private static async Task<IResult> GetNotifications
    (
        [FromBody] PagedQuery query,
        [FromServices] GetNotificationsHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> MarkAsRead
    (
        [FromBody] MarkAsReadCommand command,
        [FromServices] MarkAsReadHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}