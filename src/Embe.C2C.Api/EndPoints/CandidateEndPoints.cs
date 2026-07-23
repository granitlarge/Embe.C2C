using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Commands.Candidates;
using Embe.C2C.Application.Commands.Candidates.Handlers;
using Embe.C2C.Application.Queries.Candidates.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class CandidateEndPoints
{
    public static void MapCandidateEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/candidate").RequireAuthorization();

        group.MapGet("/positive", GetPositiveJudgements);
        group.MapPost("/judge", Judge);
        group.MapGet("/", Get);
    }

    private async static Task<IResult> GetPositiveJudgements
    (
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromServices] GetPositiveJudgementsHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(new Application.Queries.Candidates.GetPositiveJudgementsQuery(page ?? 1, size ?? 20), cancellationToken);
        return result.ToResult();
    }

    private async static Task<IResult> Judge
    (
        [FromBody] JudgeCandidateCommand command,
        [FromServices] JudgeCandidateHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToResult();
    }

    private async static Task<IResult> Get
    (
        [FromServices] GenerateCandidatesHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(GenerateCandidatesCommand.Instance, cancellationToken);
        return result.ToResult();
    }
}