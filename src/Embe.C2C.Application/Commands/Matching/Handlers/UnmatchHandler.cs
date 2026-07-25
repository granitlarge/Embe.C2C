using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Matching.Handlers;

public class UnmatchHandler
(
    IMatchingRepository matchingRepo,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    MatchingAuthorizationService authorizationPolicy,
    IAuthenticatedUserService userService,
    DomainEventStore domainEventStore
) : CommandHandler<UnmatchCommand, ErrorOr<Success>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{
    private readonly MatchingAuthorizationService _authorizationPolicy = authorizationPolicy;
    private readonly IAuthenticatedUserService _userService = userService;
    private readonly IMatchingRepository _matchingRepo = matchingRepo;

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        UnmatchCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Unmatch))
        {
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        }

        var actorId = _userService.UserId ?? throw new InvalidOperationException("Unauthorized"); ;
        var matching = await _matchingRepo.GetByIdAsync(command.MatchingId, cancellationToken);
        if (matching == null)
        {
            return new(false, ApplicationErrors.NotFound.ToForbiddenErrorOr());
        }

        matching.Remove(actorId);
        _matchingRepo.Set.Remove(matching);

        return new(true, Result.Success);
    }
}