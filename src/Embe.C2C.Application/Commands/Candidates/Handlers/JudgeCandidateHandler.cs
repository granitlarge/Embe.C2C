using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Candidates.Handlers;

public class JudgeCandidateHandler : CommandHandler<JudgeCandidateCommand, Result<ReadDto<MatchingDto, MatchingPermission>?>>
{
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
    }

    protected override async Task<CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>> HandleAsync
    (
        ISparseRepository context,
        JudgeCandidateCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var permissions = await _candidateAuthorizationService.GetPermissionsAsync(command.CandidateId, cancellationToken);
        if (!permissions.Contains(CandidatePermission.Judge))
        {
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(Commit: false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.Forbidden, "You do not have permission to judge this candidate."));
        }

        var candidate = await context.CandidatesQuery.FirstOrDefaultAsync(c => c.Id == command.CandidateId, cancellationToken: cancellationToken);
        if (candidate is null)
        {
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(Commit: false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "The candidate does not exist."));
        }

        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "User not found."));
        }

        var candidateUser = await _userRepo.GetByIdAsync(candidate.CandidateUserId, cancellationToken);
        if (candidateUser == null)
        {
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "Judgee not found."));
        }

        var oppositeCandidate = await context.CandidatesQuery.SingleOrDefaultAsync
        (
            c => c.UserId == candidate.CandidateUserId &&
                c.CandidateUserId == userId &&
                c.UserSearchProfileId == candidate.CandidateSearchProfileId &&
                c.CandidateSearchProfileId == candidate.UserSearchProfileId,
                cancellationToken
        );

        if (oppositeCandidate is null)
        {
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "Opposite candidate not found"));
        }

        var matching = _judgementService.Judge(user, candidate, oppositeCandidate, command.IsPositive);
        if (matching != null)
        {
            candidate.Remove();
            oppositeCandidate?.Remove();
            context.Candidates.Remove(candidate);
            context.Candidates.Remove(oppositeCandidate ?? throw new InvalidOperationException("opposite candidate cannot be null when a match is created."));
            context.Matchings.Add(matching);
        }

        if (matching == null)
            return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(true, Result<ReadDto<MatchingDto, MatchingPermission>?>.Success(null));

        var queryingUser = await _userRepo.GetByIdAsync(userId, cancellationToken);
        var readDto = await matching.ToDtoAsync
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

        return new CommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(true, Result<ReadDto<MatchingDto, MatchingPermission>?>.Success(readDto));
    }
}