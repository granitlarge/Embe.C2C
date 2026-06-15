using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Messages;
using Embe.C2C.Application.Commands.Messages.Handlers;
using Embe.C2C.Application.Queries.Messages;
using Embe.C2C.Application.Queries.Messages.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class MessageEndPoints
{
    public static void MapMessageEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/messages").RequireAuthorization();
        group.MapGet("/", GetMatchingMessages);
        group.MapPost("/", CreateMessage);
        group.MapDelete("/{messageId:guid}", DeleteMessage);
        group.MapPut("/", EditMessage);
    }

    private static async Task<IResult> GetMatchingMessages
    (
        Guid matchingId,
        int page,
        int size,
        [FromServices] GetMessagesByMatchingIdHandler handler,
        CancellationToken cancellationToken
    )
    {
        var query = new GetMessagesByMatchingIdQuery(matchingId, page, size);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> CreateMessage
    (
        CreateMessageCommand command,
        [FromServices] CreateMessageHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> DeleteMessage
    (
        Guid messageId,
        [FromServices] DeleteMessageHandler handler,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteMessageCommand(messageId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> EditMessage
    (
        [FromBody] EditMessageCommand command,
        [FromServices] EditMessageHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}