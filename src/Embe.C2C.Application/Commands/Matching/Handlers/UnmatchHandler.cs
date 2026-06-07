using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;

namespace Embe.C2C.Application.Commands.Matching.Handlers;

public class UnmatchHandler
{
    private readonly IC2CContext _context;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly MatchingAuthorizationPolicy _authorizationPolicy;
    private readonly IAuthenticatedUserService _userService;

    public UnmatchHandler
    (
        IC2CContext context,
        DomainEventHandler domainEventHandler,
        MatchingAuthorizationPolicy authorizationPolicy,
        IAuthenticatedUserService userService
    )
    {
        _context = context;
        _domainEventHandler = domainEventHandler;
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
    }

    public async Task<Result> HandleAsync
    (
        UnmatchCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Unmatch))
        {
            return Result.Failure(FailureReason.Forbidden, "You do not have permission to unmatch this matching.");
        }

        var actorId = _userService.UserId ?? throw new InvalidOperationException("Unauthorized"); ;
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        var matching = await _context.Matchings.FindAsync([command.MatchingId], cancellationToken);
        if (matching == null)
        {
            return Result.Failure(FailureReason.NotFound, "Matching not found.");
        }

        matching.Remove(actorId);
        foreach (var domainEvent in matching.DomainEvents)
        {
            await _domainEventHandler.HandleAsync(_context, domainEvent, cancellationToken);
        }
        _context.Matchings.Remove(matching);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);


        return Result.Success();
    }
}