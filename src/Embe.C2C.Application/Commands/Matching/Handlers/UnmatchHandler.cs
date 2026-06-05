using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;

namespace Embe.C2C.Application.Commands.Matching.Handlers;

public class UnmatchHandler
{
    private readonly C2CContext _context;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly MatchingAuthorizationPolicy _authorizationPolicy;
    private readonly IUserService _userService;

    public UnmatchHandler
    (
        C2CContext context,
        DomainEventHandler domainEventHandler,
        MatchingAuthorizationPolicy authorizationPolicy,
        IUserService userService
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
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
            _context.Remove(matching);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            return Result.Failure(FailureReason.Unknown, "An error occurred while unmatching.");
        }

        return Result.Success();
    }
}