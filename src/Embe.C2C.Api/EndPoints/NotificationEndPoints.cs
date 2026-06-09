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
        var group = app.MapGroup("/api/notification").RequireAuthorization();

        group.MapGet("", GetNotifications)
            .WithName("GetNotifications");

        group.MapPost("/mark-as-read", MarkAsRead)
            .WithName("MarkNotificationAsRead");
    }

    private static async Task<IResult> GetNotifications
    (
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromServices] GetNotificationsHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = pageNumber.HasValue && pageSize.HasValue
            ? new PagedQuery(pageNumber.Value, pageSize.Value)
            : new PagedQuery(1, 10);
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