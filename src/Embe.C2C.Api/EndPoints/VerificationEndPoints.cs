using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Verifications;
using Embe.C2C.Application.Commands.Verifications.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class VerificationEndPoints
{
    public static void MapVerificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/verification");
        group.MapPost("/email", SendEmailVerificationCode);
    }

    private static async Task<IResult> SendEmailVerificationCode
    (
        [FromBody] SendVerificationEmailCommand command,
        [FromServices] SendVerificationEmailHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}