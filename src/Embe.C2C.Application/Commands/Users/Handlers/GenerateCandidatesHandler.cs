using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class GenerateCandidatesHandler
(
    IAuthenticatedUserService authenticatedUserService,
    UserAuthorizationService userAuthorizationService,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    UserAuthorizationFactStore userAuthorizationFactStore,
    DomainEventStore domainEventStore,
    UserDtoMapper userDtoMapper,
    SearchProfileAuthorizationFactStore searchProfileAuthorizationFactStore,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper
) : CommandHandler<GenerateCandidatesCommand, Result<List<GeneratedCandidate>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserAuthorizationFactStore _userAuthorizationFactStore = userAuthorizationFactStore;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileAuthorizationFactStore _searchProfileAuthorizationFactStore = searchProfileAuthorizationFactStore;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;

    protected override async Task<CommandResult<Result<List<GeneratedCandidate>>>> HandleAsync
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
            return new CommandResult<Result<List<GeneratedCandidate>>>(true, Result<List<GeneratedCandidate>>.Success([]));
        }

        var candidates = await context.CandidatesQuery
            .Include(c => c.CandidateUser)
            .Include(c => c.CandidateSearchProfile)
            .Where(c => c.UserId == userId)
            .Take(20)
            .ToListAsync(cancellationToken);

        var dtos = new List<GeneratedCandidate>();
        foreach (var candidate in candidates)
        {
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
        }

        var result = Result<List<GeneratedCandidate>>.Success(dtos);
        return new CommandResult<Result<List<GeneratedCandidate>>>(true, result);
    }
}

public record GeneratedCandidate
(
    Guid Id,
    ReadDto<UserDto, UserPermission> Candidate,
    Guid UserSearchProfileId,
    ReadDto<SearchProfileDto, SearchProfilePermission> CandidateSearchProfile
);