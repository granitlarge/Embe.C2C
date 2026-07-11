using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;
namespace Embe.C2C.Application.Commands.Judgements.Handlers;

public class JudgeHandler : TransactionalCommandHandler<JudgeCommand, Result<ReadDto<MatchingDto, MatchingPermission>?>>
{
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly JudgementService _judgementService;
    private readonly IAuthenticatedUserService _userService;
    private readonly MatchingAuthorizationService _matchingAuthorizationService;
    private readonly UserAuthorizationFactStore _userAuthorizationFactStore;
    private readonly MatchingDtoMapper _matchingDtoMapper;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper;
    private readonly ConversationDtoMapper _conversationDtoMapper;

    public JudgeHandler
    (
        IRepository context,
        UserAuthorizationService userAuthorizationPolicy,
        JudgementService judgementService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthenticatedUserService userService,
        MatchingAuthorizationService matchingAuthorizationPolicy,
        UserAuthorizationFactStore userAuthorizationFactStore,
        DomainEventStore domainEventStore,
        MatchingDtoMapper matchingDtoMapper,
        UserDtoMapper userDtoMapper,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        ConversationDtoMapper conversationDtoMapper
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _userAuthorizationService = userAuthorizationPolicy;
        _judgementService = judgementService;
        _userService = userService;
        _matchingAuthorizationService = matchingAuthorizationPolicy;
        _userAuthorizationFactStore = userAuthorizationFactStore;
        _matchingDtoMapper = matchingDtoMapper;
        _userDtoMapper = userDtoMapper;
        _messageAuthorizationService = messageAuthorizationService;
        _messageDtoMapper = messageDtoMapper;
        _conversationDtoMapper = conversationDtoMapper;
    }

    protected override async Task<TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>> HandleAsync
    (
        ISparseRepository context,
        JudgeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var candidate = await context.CandidatesQuery.SingleOrDefaultAsync(c => c.Id == command.CandidateId && c.UserId == userId, cancellationToken);
        if (candidate == null)
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>
            (
                CommitChanges: false,
                Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure
                (
                    FailureReason.Forbidden,
                    "Judgee is not a candidate for the judge."
                )
            );
        }

        _userAuthorizationFactStore.SetCandidateUserFact(candidate.CandidateUserId, isCandidate: true);

        var (candidatePermissions, candidateVariant) = await _userAuthorizationService.GetAsync(candidate.CandidateUserId, cancellationToken);
        if (!candidatePermissions.Contains(UserPermission.Judge))
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.Forbidden, "User is not authorized to judge."));
        }

        var judge = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (judge == null)
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "User not found."));
        }

        var judgee = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == candidate.CandidateUserId, cancellationToken);
        if (judgee == null)
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "Judgee not found."));
        }

        var existingJudgement = await context.JudgementsQuery.SingleOrDefaultAsync(j => j.CandidateId == candidate.Id, cancellationToken);
        var oppositeCandidate = await context.CandidatesQuery.SingleOrDefaultAsync
        (
            c => c.UserId == candidate.CandidateUserId &&
                c.CandidateUserId == userId &&
                c.UserSearchProfileId == candidate.CandidateSearchProfileId &&
                c.CandidateSearchProfileId == candidate.UserSearchProfileId,
                cancellationToken
        );

        Judgement? oppositeJudgement = null;
        if (oppositeCandidate != null)
        {
            oppositeJudgement = await context.JudgementsQuery.SingleOrDefaultAsync(j => j.CandidateId == oppositeCandidate.Id, cancellationToken);
        }

        var (matching, judgement) = _judgementService.Judge(judge, candidate, command.IsPositive, existingJudgement, oppositeJudgement);
        if (matching != null)
        {
            judgement.Remove();
            candidate.Remove();
            oppositeCandidate?.Remove();
            context.Candidates.Remove(candidate);
            context.Candidates.Remove(oppositeCandidate ?? throw new InvalidOperationException("opposite candidate cannot be null when a match is created."));
            context.Matchings.Add(matching);
            if (oppositeJudgement != null)
            {
                oppositeJudgement.Remove();
                context.Judgements.Remove(oppositeJudgement);
            }
        }
        else if (existingJudgement == null)
        {
            context.Judgements.Add(judgement);
        }

        if (matching == null)
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(true, Result<ReadDto<MatchingDto, MatchingPermission>?>.Success(null));

        var queryingUser = await context.DomainUsersQuery.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var readDto = await matching.ToDtoAsync
        (
            queryingUser,
            judge,
            judgee,
            _matchingAuthorizationService,
            _matchingDtoMapper,
            _userAuthorizationService,
            _userDtoMapper,
            _messageAuthorizationService,
            _messageDtoMapper,
            _conversationDtoMapper,
            cancellationToken
        );

        return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(true, Result<ReadDto<MatchingDto, MatchingPermission>?>.Success(readDto));
    }
}