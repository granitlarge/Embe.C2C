using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.SearchProfiles;
using Embe.C2C.Application.Commands.SearchProfiles.Handlers;
using Embe.C2C.Application.Queries.SearchProfiles;
using Embe.C2C.Application.Queries.SearchProfiles.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class SearchProfileEndPoints
{
    public static void MapSearchProfileEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/search-profile")
            .WithTags("Search Profiles")
            .RequireAuthorization();

        group.MapPost("", Create);
        group.MapPut("", Update);
        group.MapGet("{id}", Get);
    }

    private static async Task<IResult> Create
    (
        [FromBody] CreateSearchProfileCommand command,
        [FromServices] CreateSearchProfileHandler handler,
        CancellationToken cancellationToken = default
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Update
    (
        [FromBody] UpdateSearchProfileCommand command,
        [FromServices] UpdateSearchProfileHandler handler,
        CancellationToken cancellationToken = default
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Get
    (
        [FromRoute] Guid id,
        [FromServices] GetSearchProfileHandler handler,
        CancellationToken cancellationToken = default
    )
    {
        var result = await handler.HandleAsync(new GetSearchProfileQuery(id), cancellationToken);
        return result.ToResult();
    }
}