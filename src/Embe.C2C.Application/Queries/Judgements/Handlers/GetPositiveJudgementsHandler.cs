using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Judgements.Handlers;

public class GetPositiveJudgementsHandler
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService,
    JudgementAuthorizationService authorizationPolicy,
    JudgementDtoMapper judgementDtoMapper,
    UserAuthorizationService userAuthorizationService,
    UserDtoMapper userDtoMapper
)
{

    private readonly IRepository _repository = repository;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly JudgementAuthorizationService _judgementAuthorizationService = authorizationPolicy;
    private readonly JudgementDtoMapper _judgementDtoMapper = judgementDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

    public async Task<Result<List<ReadDto<JudgementDto, JudgementPermission>>>> HandleAsync
    (
        GetPositiveJudgementsQuery query,
        CancellationToken cancellationToken
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User must be authenticated to get positive judgements.");
        var queryingUser = await _repository.DomainUsersQuery.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var judgements = await _repository
            .JudgementsQuery
                .Include(j => j.Candidate)
                .ThenInclude(c => c!.User)
            .Where(j => j.Candidate!.CandidateUserId == userId && j.IsPositive)
            .OrderByDescending(j => j.EditedAt)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        var judgementDtos = new List<ReadDto<JudgementDto, JudgementPermission>>();
        foreach (var judgement in judgements)
        {
            var readDto = await judgement.ToDtoAsync
            (
                queryingUser,
                _judgementAuthorizationService,
                _judgementDtoMapper, 
                _userAuthorizationService,
                _userDtoMapper,
                cancellationToken
            );
            if (readDto != null)
                judgementDtos.Add(readDto);
        }

        return Result<List<ReadDto<JudgementDto, JudgementPermission>>>.Success(judgementDtos);

    }

}