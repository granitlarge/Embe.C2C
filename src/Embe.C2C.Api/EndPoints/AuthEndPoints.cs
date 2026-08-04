using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Auth;
using Embe.C2C.Application.Commands.Auth.Handlers;
using Embe.C2C.Application.Queries.Auth;
using Embe.C2C.Application.Queries.Auth.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class AuthEndPoints
{
    public static void MapAuthEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapGet("/account-exists", AccountExists);
        group.MapPost("/signin", SignIn);
        group.MapPost("/signout", SignOut).RequireAuthorization();
        group.MapPost("/refresh", Refresh).RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = "Refresh"
        });
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/reset-password", ResetPassword).RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = "Bearer,ResetPassword"
        });
    }

    private static async Task<IResult> AccountExists([FromQuery] string email, [FromServices] AccountExistsHandler handler, CancellationToken cancellationToken = default)
    {
        var query = new AccountExistsQuery(email);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToResult();
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

    private static async Task<IResult> ForgotPassword([FromBody] SendResetPasswordEmailCommand command, [FromServices] SendResetPasswordEmailHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> ResetPassword([FromBody] ResetPasswordCommand command, [FromServices] ResetPasswordHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}