using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;
namespace Embe.C2C.Application.Commands.Judgements.Handlers;

public class JudgeHandler : TransactionalCommandHandler<JudgeCommand, Result<ReadDto<MatchingDto, MatchingPermission>?>>
{
    private readonly UserAuthorizationPolicy _userAuthorizationPolicy;
    private readonly JudgementService _judgementService;
    private readonly IAuthenticatedUserService _userService;
    private readonly MatchingAuthorizationPolicy _matchingAuthorizationPolicy;

    public JudgeHandler
    (
        IRepository context,
        UserAuthorizationPolicy userAuthorizationPolicy,
        JudgementService judgementService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthenticatedUserService userService,
        MatchingAuthorizationPolicy matchingAuthorizationPolicy
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _userAuthorizationPolicy = userAuthorizationPolicy;
        _judgementService = judgementService;
        _userService = userService;
        _matchingAuthorizationPolicy = matchingAuthorizationPolicy;
    }

    protected override async Task<TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>> HandleAsync
    (
        ISparseRepository context,
        JudgeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, _) = await _userAuthorizationPolicy.GetAsync(command.JudgeeUserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Judge))
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.Forbidden, "User is not authorized to judge."));
        }

        var userId = _userService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var judge = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (judge == null)
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "User not found."));
        }

        var judgee = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == command.JudgeeUserId, cancellationToken);
        if (judgee == null)
        {
            return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(false, Result<ReadDto<MatchingDto, MatchingPermission>?>.Failure(FailureReason.NotFound, "Judgee not found."));
        }

        var existingJudgement = await context.JudgementsQuery.SingleOrDefaultAsync(j => j.JudgeUserId == userId && j.JudgeeUserId == command.JudgeeUserId, cancellationToken);
        var oppositeJudgement = await context.JudgementsQuery.SingleOrDefaultAsync(j => j.JudgeUserId == command.JudgeeUserId && j.JudgeeUserId == userId, cancellationToken);

        var match = _judgementService.Judge(judge, judgee, command.IsPositive, existingJudgement, oppositeJudgement);
        if (match != null)
        {
            context.Matchings.Add(match);
        }

        var matchingDto = match != null ? await _matchingAuthorizationPolicy.ToDtoAsync(match, cancellationToken) : null;
        var result = Result<ReadDto<MatchingDto, MatchingPermission>?>.Success(matchingDto);

        return new TransactionalCommandResult<Result<ReadDto<MatchingDto, MatchingPermission>?>>(true, result);
    }
}