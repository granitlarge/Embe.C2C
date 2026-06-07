using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Auth;
using Embe.C2C.Application.Commands.Auth.Handlers;
using Embe.C2C.Application.Queries.Auth;
using Embe.C2C.Application.Queries.Auth.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class AuthEndPoints
{
    public static void MapAuthEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/account-exists", AccountExists);
        group.MapPost("/signin", SignIn);
        group.MapPost("/signout", SignOut).RequireAuthorization();
        group.MapPost("/refresh", Refresh).RequireAuthorization();
    }

    private static async Task<IResult> AccountExists([FromServices] ServiceProvider services)
    {
        Console.WriteLine("Before service injection");
        var handler = services.GetService<AccountExistsHandler>();
        if (handler == null)
        {
            Console.WriteLine("Handler is null");
            return Results.Problem("Handler not found");
        }
        Console.WriteLine("After service injection");
        return Results.Ok();
    }

    private static async Task<IResult> SignIn([FromBody] SignInCommand command, [FromServices] SignInHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> SignOut([FromBody] SignOutCommand command, [FromServices] SignOutHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> Refresh([FromBody] RefreshCommand command, [FromServices] RefreshHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}