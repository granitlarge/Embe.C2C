using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;

namespace Embe.C2C.Application.Commands.Matching.Handlers;

public class UnmatchHandler : TransactionalCommandHandler<UnmatchCommand, Result>
{
    private readonly MatchingAuthorizationPolicy _authorizationPolicy;
    private readonly IAuthenticatedUserService _userService;

    public UnmatchHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        MatchingAuthorizationPolicy authorizationPolicy,
        IAuthenticatedUserService userService
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
    }

    protected override async Task<TransactionalCommandResult<Result>> HandleAsync
    (
        ISparseRepository context,
        UnmatchCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Unmatch))
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You do not have permission to unmatch this matching."));
        }

        var actorId = _userService.UserId ?? throw new InvalidOperationException("Unauthorized"); ;
        var matching = await context.Matchings.FindAsync([command.MatchingId], cancellationToken);
        if (matching == null)
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "Matching not found."));
        }

        matching.Remove(actorId);
        return new TransactionalCommandResult<Result>(true, Result.Success());
    }
}