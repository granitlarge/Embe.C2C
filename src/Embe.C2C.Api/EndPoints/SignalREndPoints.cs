using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class SignalREndPoints
{
    public static void MapSignalREndPoints(this WebApplication webApplication)
    {
        webApplication.MapPost("/hubs/main/negotiate", Negotiate);
    }

    private static async Task<IResult> Negotiate
    (
            [FromServices] SignalRServiceHubContextPool hubContextPool,
            HttpContext httpContext,
            CancellationToken cancellationToken
    )
    {
        var hubContext = await hubContextPool.GetHubContextAsync(cancellationToken);
        var response = await hubContext.NegotiateAsync(new Microsoft.Azure.SignalR.Management.NegotiationOptions
        {
            HttpContext = httpContext,
            CloseOnAuthenticationExpiration = true
        }, cancellationToken);
        return Results.Ok(response);

    }
}