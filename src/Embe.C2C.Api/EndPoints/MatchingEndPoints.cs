using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Matching;
using Embe.C2C.Application.Commands.Matching.Handlers;
using Embe.C2C.Application.Queries.Matchings;
using Embe.C2C.Application.Queries.Matchings.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class MatchingEndPoints
{
    public static void MapMatchingEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/matching")
            .WithTags("Matching")
            .RequireAuthorization();

        group.MapGet("/", Get);
        group.MapPost("/unmatch", Unmatch);
    }

    private static async Task<IResult> Get([FromServices] GetMatchingsHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(GetMatchingsQuery.Instance, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Unmatch([FromBody] UnmatchCommand command, [FromServices] UnmatchHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}