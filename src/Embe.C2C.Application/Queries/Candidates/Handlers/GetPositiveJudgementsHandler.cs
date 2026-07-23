using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Candidates.Handlers;

public class GetPositiveJudgementsHandler : TransactionalQueryHandler
<
    GetPositiveJudgementsQuery,
    Result<List<ReadDto<CandidateDto, CandidatePermission>>>
>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;
    private readonly CandidateAuthorizationService _candidateAuthorizationService;
    private readonly CandidateDtoMapper _candidateDtoMapper;

    public GetPositiveJudgementsHandler
    (
        IUserRepository userRepo,
        IRepository repository,
        IAuthenticatedUserService authenticatedUserService,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CandidateAuthorizationService candidateAuthorizationService,
        CandidateDtoMapper candidateDtoMapper
    ) : base(repository)
    {
        _authenticatedUserService = authenticatedUserService;
        _userAuthorizationService = userAuthorizationService;
        _userDtoMapper = userDtoMapper;
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
        _searchProfileDtoMapper = searchProfileDtoMapper;
        _candidateAuthorizationService = candidateAuthorizationService;
        _candidateDtoMapper = candidateDtoMapper;
        _userRepo = userRepo;
    }

    protected async override Task<Result<List<ReadDto<CandidateDto, CandidatePermission>>>> ExecuteAsync
    (
        GetPositiveJudgementsQuery query,
        ISparseRepository repository,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user isn't authenticated");
        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var candidates = await repository
            .CandidatesQuery
            .AsNoTracking()
            .AsSplitQuery()
            .Where(c => c.CandidateUserId == userId)
            .Include(c => c.User)
            .Include(c => c.UserSearchProfile)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<CandidateDto, CandidatePermission>>();
        foreach (var candidate in candidates)
        {
            var dto = await candidate.ToDtoAsync
            (
                queryingUser,
                _userAuthorizationService,
                _userDtoMapper,
                _searchProfileAuthorizationService,
                _searchProfileDtoMapper,
                _candidateAuthorizationService,
                _candidateDtoMapper,
                cancellationToken
            );

            if (dto is not null)
            {
                dtos.Add(dto);
            }
        }

        return Result<List<ReadDto<CandidateDto, CandidatePermission>>>.Success(dtos);
    }
}