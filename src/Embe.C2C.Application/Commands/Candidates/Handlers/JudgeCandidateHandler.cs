using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Candidates.Handlers;

public class JudgeCandidateHandler : CommandHandler<JudgeCandidateCommand, ErrorOr<ReadDto<MatchingDto, MatchingPermission>?>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IMatchingRepository _matchingRepo;
    private readonly IUserRepository _userRepo;
    private readonly CandidateAuthorizationService _candidateAuthorizationService;
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly CandidateService _judgementService;
    private readonly IAuthenticatedUserService _userService;
    private readonly MatchingAuthorizationService _matchingAuthorizationService;
    private readonly MatchingDtoMapper _matchingDtoMapper;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;

    public JudgeCandidateHandler
    (
        ICandidateRepository candidateRepository,
        IMatchingRepository matchingRepo,
        IUserRepository userRepo,
        IRepository context,
        UserAuthorizationService userAuthorizationService,
        CandidateService judgementService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthenticatedUserService userService,
        MatchingAuthorizationService matchingAuthorizationPolicy,
        DomainEventStore domainEventStore,
        MatchingDtoMapper matchingDtoMapper,
        UserDtoMapper userDtoMapper,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CandidateAuthorizationService candidateAuthorizationService
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _userAuthorizationService = userAuthorizationService;
        _judgementService = judgementService;
        _userService = userService;
        _matchingAuthorizationService = matchingAuthorizationPolicy;
        _matchingDtoMapper = matchingDtoMapper;
        _userDtoMapper = userDtoMapper;
        _messageAuthorizationService = messageAuthorizationService;
        _messageDtoMapper = messageDtoMapper;
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
        _searchProfileDtoMapper = searchProfileDtoMapper;
        _candidateAuthorizationService = candidateAuthorizationService;
        _userRepo = userRepo;
        _matchingRepo = matchingRepo;
        _candidateRepository = candidateRepository;
    }

    protected override async Task<CommandResult<ErrorOr<ReadDto<MatchingDto, MatchingPermission>?>>> InternalHandleAsync
    (
        JudgeCandidateCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var permissions = await _candidateAuthorizationService.GetPermissionsAsync(command.CandidateId, cancellationToken);
        if (!permissions.Contains(CandidatePermission.Judge))
        {
            return new
            (
                false,
                ApplicationErrors.Forbidden.ToForbiddenErrorOr()
            );
        }

        var candidate = await _candidateRepository.GetByIdAsync(command.CandidateId, cancellationToken);
        if (candidate is null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var candidateUser = await _userRepo.GetByIdAsync(candidate.CandidateUserId, cancellationToken);
        if (candidateUser == null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var oppositeCandidate = await _candidateRepository.GetByParametersAsync(candidate.CandidateUserId, userId, candidate.CandidateSearchProfileId, candidate.UserSearchProfileId, cancellationToken);
        if (oppositeCandidate is null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var matching = _judgementService.Judge(user, candidate, oppositeCandidate, command.IsPositive);
        if (matching.IsError)
        {
            return new
            (
                false,
                ErrorOrFactory.From<ReadDto<MatchingDto, MatchingPermission>?>(matching.Errors.ToApplicationError())
            );
        }

        if (matching.Value != null)
        {
            candidate.Remove();
            oppositeCandidate?.Remove();
            _candidateRepository.Set.Remove(candidate);
            _candidateRepository.Set.Remove(oppositeCandidate!);
            _matchingRepo.Set.Add(matching.Value);
        }

        if (matching.Value == null)
        {
            return new
            (
                true,
                ErrorOrFactory.From((ReadDto<MatchingDto, MatchingPermission>?)null)
            );
        }

        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var readDto = await matching.Value.ToDtoAsync
        (
            queryingUser,
            user,
            candidateUser,
            _matchingAuthorizationService,
            _matchingDtoMapper,
            _userAuthorizationService,
            _userDtoMapper,
            _messageAuthorizationService,
            _messageDtoMapper,
            _searchProfileAuthorizationService,
            _searchProfileDtoMapper,
            cancellationToken
        );

        return new(true, readDto);
    }
}