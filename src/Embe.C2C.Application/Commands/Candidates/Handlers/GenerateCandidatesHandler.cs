using System.Text.Json;
using System.Xml;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Candidates;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Candidates.Handlers;

public class GenerateCandidatesHandler
(
    ICandidateRepository candidateRepository,
    IUserRepository userRepo,
    IAuthenticatedUserService authenticatedUserService,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore,
    CandidateDtoMapper candidateDtoMapper,
    ILoggerFactory loggerFactory
) : CommandHandler<GenerateCandidatesCommand, ErrorOr<List<ReadDto<CandidateDto, CandidatePermission>>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly ICandidateRepository _candidateRepository = candidateRepository;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly CandidateDtoMapper _candidateDtoMapper = candidateDtoMapper;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly ILogger<GenerateCandidatesHandler> _logger = loggerFactory.Create<GenerateCandidatesHandler>();

    protected override async Task<CommandResult<ErrorOr<List<ReadDto<CandidateDto, CandidatePermission>>>>> InternalHandleAsync
    (
        GenerateCandidatesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(nameof(GenerateCandidatesHandler));
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var userHasCandidates = await _candidateRepository.GenerateCandidatesForUserIdAsync(userId, cancellationToken);
        if (!userHasCandidates)
        {
            return new(true, ErrorOrFactory.From(new List<ReadDto<CandidateDto, CandidatePermission>>()));
        }

        var candidates = await _candidateRepository.GetByUserIdAsync(userId, cancellationToken);

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
            await _logger.TraceAsync("CANDIDATE.USER IS NULL:" + (candidate.User is null));
            await _logger.TraceAsync("CANDIDATE.CandidateUser IS NULL:" + (candidate.CandidateUser is null));
            var candidateDto = await _candidateDtoMapper.ToDtoAsync(candidate, queryingUser, cancellationToken);
            if (candidateDto != null)
            {
                dtos.Add(candidateDto);
            }
        }

        return new(true, dtos);
    }
}