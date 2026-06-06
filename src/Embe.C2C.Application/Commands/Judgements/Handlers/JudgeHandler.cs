using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;

using ResultType = Embe.C2C.Application.Abstractions.EntityWithPermissions<Embe.C2C.Domain.Aggregates.Matchings.Matching?, System.Collections.Immutable.ImmutableHashSet<MatchingPermission>>;

namespace Embe.C2C.Application.Commands.Judgements.Handlers;

public class JudgeHandler
{
    private readonly IC2CContext _context;
    private readonly JudgementAuthorizationPolicy _authorizationPolicy;
    private readonly JudgementService _judgementService;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly IAuthenticatedUserService _userService;
    private readonly MatchingAuthorizationPolicy _matchingAuthorizationPolicy;

    public JudgeHandler
    (
        IC2CContext context,
        JudgementAuthorizationPolicy authorizationPolicy,
        JudgementService judgementService,
        DomainEventHandler domainEventHandler,
        IAuthenticatedUserService userService,
        MatchingAuthorizationPolicy matchingAuthorizationPolicy
    )
    {
        _context = context;
        _authorizationPolicy = authorizationPolicy;
        _judgementService = judgementService;
        _domainEventHandler = domainEventHandler;
        _userService = userService;
        _matchingAuthorizationPolicy = matchingAuthorizationPolicy;
    }

    public async Task<Result<ResultType>> HandleAsync
    (
        JudgeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var isAuthorized = (await _authorizationPolicy.GetPermissionsAsync(command.JudgeeUserId, cancellationToken)).Contains(JudgementPermission.Judge);
        if (!isAuthorized)
        {
            return Result<ResultType>.Failure(FailureReason.Forbidden, "User is not authorized to judge.");
        }

        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        try
        {

            using var transaction = await _context.BeginTransactionAsync();

            var judge = await _context.DomainUsers.FindAsync([userId], cancellationToken);
            if (judge == null)
            {
                return Result<ResultType>.Failure(FailureReason.NotFound, "User not found.");
            }

            var judgee = await _context.DomainUsers.FindAsync([command.JudgeeUserId], cancellationToken);
            if (judgee == null)
            {
                return Result<ResultType>.Failure(FailureReason.NotFound, "Judgee not found.");
            }

            var existingJudgement = await _context.Judgements.FindAsync([userId, command.JudgeeUserId], cancellationToken);
            var oppositeJudgement = await _context.Judgements.FindAsync([command.JudgeeUserId, userId], cancellationToken);

            var match = _judgementService.Judge(judge, judgee, command.IsPositive, existingJudgement, oppositeJudgement);
            if (match != null)
            {
                _context.Matchings.Add(match);
            }

            await ProcessDomainEvents(cancellationToken, judge, judgee, existingJudgement, oppositeJudgement, match);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var matchingPermissions = match != null ? await _matchingAuthorizationPolicy.GetPermissionsAsync(match.Id, cancellationToken) : [];

            return Result<ResultType>.Success(new ResultType(match, matchingPermissions));

        }
        catch (Exception)
        {
            return Result<ResultType>.Failure(FailureReason.Unknown, "An error occurred while processing the judgement.");
        }

        async Task ProcessDomainEvents(CancellationToken cancellationToken = default, params DomainEventCollector?[] collectors)
        {
            var domainEvents = collectors.Where(collector => collector != null).SelectMany(collector => collector!.DomainEvents).ToList();
            foreach (var domainEvent in domainEvents)
            {
                await _domainEventHandler.HandleAsync(_context, domainEvent, cancellationToken);
            }
        }
    }
}