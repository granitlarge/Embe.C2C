using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Candidates.Handlers;

public class GenerateCandidatesHandler
(
    IAuthenticatedUserService authenticatedUserService,
    UserAuthorizationService userAuthorizationService,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore,
    UserDtoMapper userDtoMapper,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper,
    CandidateDtoMapper candidateDtoMapper,
    CandidateAuthorizationService candidateAuthorizationService
) : CommandHandler<GenerateCandidatesCommand, Result<List<ReadDto<CandidateDto, CandidatePermission>>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly CandidateDtoMapper _candidateDtoMapper = candidateDtoMapper;
    private readonly CandidateAuthorizationService _candidateAuthorizationService = candidateAuthorizationService;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;

    protected override async Task<CommandResult<Result<List<ReadDto<CandidateDto, CandidatePermission>>>>> HandleAsync
    (
        ISparseRepository context,
        GenerateCandidatesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var queryingUser = await context.DomainUsersQuery.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var userHasCandidates = await context.GenerateCandidatesForUserIdAsync(userId, cancellationToken);
        if (!userHasCandidates)
        {
            return new CommandResult<Result<List<ReadDto<CandidateDto, CandidatePermission>>>>(true, Result<List<ReadDto<CandidateDto, CandidatePermission>>>.Success([]));
        }

        var candidates = await context.CandidatesQuery
            .Include(c => c.CandidateUser)
            .Include(c => c.CandidateSearchProfile)
            .Where(c => c.UserId == userId)
            .Take(20)
            .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<CandidateDto, CandidatePermission>>();
        foreach (var candidate in candidates)
        {
#warning we're not setting facts here anymore, which means we'll query the database for every candidate, find a way of avoiding that
            /*
            var user = candidate.CandidateUser!;
            var candidateSearchProfile = candidate.CandidateSearchProfile!;

            _userAuthorizationFactStore.SetCandidateUserFact(user.Id, true);
            _searchProfileAuthorizationFactStore.SetIsCandidateForUserFact(candidateSearchProfile.Id, true);

            var userDto = await user.Enrich(queryingUser).ToDtoAsync(_userAuthorizationService, _userDtoMapper, cancellationToken);
            var candidateSearchProfileDto = await candidateSearchProfile.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);

            if (userDto is null || candidateSearchProfileDto is null)
            {
                throw new InvalidOperationException($"User DTO for user {user.Id} or candidate search profile DTO is null.");
            }

            dtos.Add(new GeneratedCandidate(candidate.Id, userDto, candidate.UserSearchProfileId, candidateSearchProfileDto));
            */
            var candidateDto = await candidate.ToDtoAsync
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

            if (candidateDto != null)
                dtos.Add(candidateDto);
        }

        var result = Result<List<ReadDto<CandidateDto, CandidatePermission>>>.Success(dtos);
        return new CommandResult<Result<List<ReadDto<CandidateDto, CandidatePermission>>>>(true, result);
    }
}