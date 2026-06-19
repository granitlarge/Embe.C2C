using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Judgements;
using Embe.C2C.Application.Commands.Judgements.Handlers;
using Embe.C2C.Application.Queries.Judgements;
using Embe.C2C.Application.Queries.Judgements.Handlers;
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
        group.MapGet("/positive", GetPositiveJudgements);
    }

    private static async Task<IResult> Judge([FromBody] JudgeCommand command, [FromServices] JudgeHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private static async Task<IResult> GetPositiveJudgements([FromQuery] int page, [FromQuery] int size, [FromServices] GetPositiveJudgementsHandler handler, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(new GetPositiveJudgementsQuery(page, size), cancellationToken);
        return result.ToResult();
    }

}