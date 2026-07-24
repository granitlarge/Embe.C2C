using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Candidates;
using Embe.C2C.Application.Commands.Candidates.Handlers;
using Embe.C2C.Application.Commands.Users;
using Embe.C2C.Application.Commands.Users.Handlers;
using Embe.C2C.Application.Queries.Users;
using Embe.C2C.Application.Queries.Users.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class UserEndPoints
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/user")
            .WithTags("Users");

        group.MapPost("/register", Register);
        group.MapPut("", Update).RequireAuthorization();
        group.MapDelete("", Delete).RequireAuthorization();
        group.MapGet("/candidates", GenerateCandidates).RequireAuthorization();
        group.MapGet("/me", GetMe).RequireAuthorization();
        group.MapGet("/{id:guid}", GetById).RequireAuthorization();
        group.MapGet("/has-search-profile", HasSearchProfile).RequireAuthorization();
        group.MapPost("/upload-images", AddImages).RequireAuthorization();
    }

    private static async Task<IResult> Register([FromBody] RegisterCommand command, [FromServices] RegisterHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Update([FromBody] UpdateCommand command, [FromServices] UpdateHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Delete([FromBody] DeleteCommand command, [FromServices] DeleteHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> GenerateCandidates([FromServices] GenerateCandidatesHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(GenerateCandidatesCommand.Instance, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> GetMe([FromServices] GetMeHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(GetMeQuery.Instance, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> GetById([FromRoute] Guid id, [FromServices] GetUserByIdHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(new GetUserByIdQuery(id), cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> HasSearchProfile([FromServices] GetHasSearchProfileHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(GetHasSearchProfileQuery.Instance, cancellationToken);
        return result.ToResult();
    }
    private static async Task<IResult> AddImages([FromBody] AddImagesCommand command, [FromServices] AddImagesHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}