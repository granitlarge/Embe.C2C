using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Matching.Handlers;

public class UnmatchHandler : CommandHandler<UnmatchCommand, Result>
{
    private readonly MatchingAuthorizationService _authorizationPolicy;
    private readonly IAuthenticatedUserService _userService;

    public UnmatchHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        MatchingAuthorizationService authorizationPolicy,
        IAuthenticatedUserService userService,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
    }

    protected override async Task<CommandResult<Result>> HandleAsync
    (
        ISparseRepository context,
        UnmatchCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Unmatch))
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You do not have permission to unmatch this matching."));
        }

        var actorId = _userService.UserId ?? throw new InvalidOperationException("Unauthorized"); ;
        var matching = await context.MatchingsQuery.SingleOrDefaultAsync(m => m.Id == command.MatchingId, cancellationToken);
        if (matching == null)
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "Matching not found."));
        }

        matching.Remove(actorId);
        context.Matchings.Remove(matching);

        return new CommandResult<Result>(true, Result.Success());
    }
}