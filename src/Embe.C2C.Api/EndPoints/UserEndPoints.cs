using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Users;
using Embe.C2C.Application.Commands.Users.Handlers;
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
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete", Delete).RequireAuthorization();
    }

    private static async Task<IResult> Register([FromBody]RegisterCommand command, [FromServices]RegisterHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Update([FromBody]UpdateCommand command, [FromServices]UpdateHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Delete([FromBody]DeleteCommand command, [FromServices]DeleteHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }
}