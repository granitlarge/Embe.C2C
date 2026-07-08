using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Judgements.Handlers;

public class GetPositiveJudgementsHandler
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService,
    JudgementAuthorizationService authorizationPolicy,
    JudgementDtoMapper judgementDtoMapper
)
{

    private readonly IRepository _repository = repository;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly JudgementAuthorizationService _authorizationPolicy = authorizationPolicy;
    private readonly JudgementDtoMapper _judgementDtoMapper = judgementDtoMapper;

    public async Task<Result<List<ReadDto<JudgementDto, JudgementPermission>>>> HandleAsync
    (
        GetPositiveJudgementsQuery query,
        CancellationToken cancellationToken
    )
    {

        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User must be authenticated to get positive judgements.");
        var judgements = await _repository
            .JudgementsQuery
                .Include(j => j.Judge)
            .Where(j => j.JudgeeUserId == userId && j.IsPositive)
            .OrderByDescending(j => j.EditedAt)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        var judgementDtos = new List<ReadDto<JudgementDto, JudgementPermission>>();
        foreach (var judgement in judgements)
        {
            var (permissions, variant) = _authorizationPolicy.Get(judgement);
            var dto = _judgementDtoMapper.ToDto(judgement, variant);
            if (dto == null || !permissions.Contains(JudgementPermission.View))
            {
                return Result<List<ReadDto<JudgementDto, JudgementPermission>>>.Failure(FailureReason.Forbidden, "User does not have permission to view some of the judgements.");
            }
            judgementDtos.Add(new ReadDto<JudgementDto, JudgementPermission>(dto, permissions));
        }

        return Result<List<ReadDto<JudgementDto, JudgementPermission>>>.Success(judgementDtos);

    }

}