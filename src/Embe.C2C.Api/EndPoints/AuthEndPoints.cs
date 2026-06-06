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

    private static async Task<IResult> AccountExists([FromBody] string email, [FromServices] AccountExistsHandler handler)
    {
        var result = await handler.HandleAsync(new AccountExistsQuery(email));
        return result.ToResult();
    }

    private static async Task<IResult> SignIn([FromBody] SignInCommand command, [FromServices] SignInHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> SignOut([FromBody] SignOutCommand command, [FromServices] SignOutHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Refresh([FromBody] RefreshCommand command, [FromServices] RefreshHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }
}