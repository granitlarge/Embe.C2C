using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class DeleteHandler
{
    private readonly IC2CContext _context;
    private readonly UserAuthorizationPolicy _authorizationPolicy;
    private readonly UserService _userService;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly IAuthService _authService;

    public DeleteHandler
    (
        IC2CContext context,
        UserAuthorizationPolicy authorizationPolicy,
        UserService userService,
        DomainEventHandler domainEventHandler,
        IAuthService authService
    )
    {
        _context = context;
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
        _domainEventHandler = domainEventHandler;
        _authService = authService;
    }

    public async Task<Result> HandleAsync
    (
        DeleteCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Delete))
        {
            return Result.Failure(FailureReason.Forbidden, "You are not authorized to delete this user.");
        }

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        var user = await _context.DomainUsers.FindAsync([command.UserId], cancellationToken);
        if (user is null)
        {
            return Result.Failure(FailureReason.NotFound, "User not found.");
        }

        var deleteIdentityUserResult = await _authService.DeleteUserAsync(user.IdentityUserId, cancellationToken);
        if (!deleteIdentityUserResult.IsSuccess)
        {
            return Result.Failure(FailureReason.Unknown, deleteIdentityUserResult.Message!);
        }

        var accounts = await _context.Accounts.Where(a => a.UserId == command.UserId).ToListAsync(cancellationToken);
        _userService.Delete(user, [.. accounts]);

        var domainEventCollectors = new DomainEventCollector[] { user, _userService }.Concat(accounts);
        foreach (var domainEvent in domainEventCollectors.SelectMany(c => c.DomainEvents))
        {
            await _domainEventHandler.HandleAsync(_context, domainEvent, cancellationToken);
        }

        _context.DomainUsers.Remove(user);
        foreach (var account in accounts)
        {
            _context.Accounts.Remove(account);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result.Success();
    }
}