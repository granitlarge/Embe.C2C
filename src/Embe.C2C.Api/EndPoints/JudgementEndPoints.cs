using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Judgements;
using Embe.C2C.Application.Commands.Judgements.Handlers;
using Microsoft.AspNetCore.Mvc;
namespace Embe.C2C.Api.EndPoints;

public static class JudgementEndPoints
{
    public static void MapJudgementEndPoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/judgement")
            .WithTags("Judgement")
            .RequireAuthorization();

        group.MapPost("/", Judge);
    }

    private static async Task<IResult> Judge([FromBody] JudgeCommand command, [FromServices] JudgeHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }
}