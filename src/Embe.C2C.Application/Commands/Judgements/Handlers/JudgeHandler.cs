using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain.Services;

using ResultType = Embe.C2C.Application.Abstractions.EntityWithPermissions<Embe.C2C.Domain.Aggregates.Matchings.Matching?, System.Collections.Immutable.ImmutableHashSet<MatchingPermission>>;

namespace Embe.C2C.Application.Commands.Judgements.Handlers;

public class JudgeHandler : TransactionalCommandHandler<JudgeCommand, Result<ResultType>>
{
    private readonly JudgementAuthorizationPolicy _authorizationPolicy;
    private readonly JudgementService _judgementService;
    private readonly IAuthenticatedUserService _userService;
    private readonly MatchingAuthorizationPolicy _matchingAuthorizationPolicy;

    public JudgeHandler
    (
        IRepository context,
        JudgementAuthorizationPolicy authorizationPolicy,
        JudgementService judgementService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthenticatedUserService userService,
        MatchingAuthorizationPolicy matchingAuthorizationPolicy
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _judgementService = judgementService;
        _userService = userService;
        _matchingAuthorizationPolicy = matchingAuthorizationPolicy;
    }

    protected override async Task<TransactionalCommandResult<Result<ResultType>>> HandleAsync
    (
        ISparseRepository context,
        JudgeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var isAuthorized = (await _authorizationPolicy.GetPermissionsAsync(command.JudgeeUserId, cancellationToken)).Contains(JudgementPermission.Judge);
        if (!isAuthorized)
        {
            return new TransactionalCommandResult<Result<ResultType>>(false, Result<ResultType>.Failure(FailureReason.Forbidden, "User is not authorized to judge."));
        }

        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var judge = await context.DomainUsers.FindAsync([userId], cancellationToken);
        if (judge == null)
        {
            return new TransactionalCommandResult<Result<ResultType>>(false, Result<ResultType>.Failure(FailureReason.NotFound, "User not found."));
        }

        var judgee = await context.DomainUsers.FindAsync([command.JudgeeUserId], cancellationToken);
        if (judgee == null)
        {
            return new TransactionalCommandResult<Result<ResultType>>(false, Result<ResultType>.Failure(FailureReason.NotFound, "Judgee not found."));
        }

        var existingJudgement = await context.Judgements.FindAsync([userId, command.JudgeeUserId], cancellationToken);
        var oppositeJudgement = await context.Judgements.FindAsync([command.JudgeeUserId, userId], cancellationToken);

        var match = _judgementService.Judge(judge, judgee, command.IsPositive, existingJudgement, oppositeJudgement);
        if (match != null)
        {
            context.Matchings.Add(match);
        }

        var matchingPermissions = match != null ? await _matchingAuthorizationPolicy.GetPermissionsAsync(match.Id, cancellationToken) : [];

        var result = Result<ResultType>.Success(new ResultType(match, matchingPermissions));
        return new TransactionalCommandResult<Result<ResultType>>(result.IsSuccess, result);
    }
}